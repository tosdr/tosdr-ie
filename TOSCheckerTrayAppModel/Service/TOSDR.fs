module TOSCheckerTrayApp.Service.TOSDR

open System
open System.Net
open System.Diagnostics
open System.Text.RegularExpressions
open Newtonsoft.Json

open TOSCheckerTrayApp.Model

let deserialize<'T> (data : string) = JsonConvert.DeserializeObject<'T>(data)

let load<'T> (uriFormat : string -> string) name =
    let client = new WebClient()
    let uri = uriFormat name
    try client.DownloadString(uri) |> deserialize<'T> |> Some with
    | :? Reflection.TargetInvocationException as e -> 
        let response = (e.InnerException :?> WebException).Response :?> HttpWebResponse
        match response.StatusCode with
        | HttpStatusCode.NotFound -> sprintf "Item [%s] could not be found at [%s]" name uri |> Debug.WriteLine; None
        | _ -> sprintf "Unexpected HTTP error trying to load item [%s] from [%s]: %s" name uri e.InnerException.Message |> Debug.WriteLine; None
    | e -> sprintf "Unexpected exception trying to load and parse data for item [%s] from [%s]: %s" name uri e.Message |> Debug.WriteLine; None

let loadService = load<ServiceItem> (fun name -> sprintf "http://tos-dr.info/services/%s.json" name)

let mergeService (KeyValue (k : string, v : ServicesItem)) =
    match loadService k with
    | Some(service) ->
        if service.Url <> null then
            let domain = 
                match service.Url.IsAbsoluteUri with
                | true -> service.Url.Host
                | false -> service.Url.OriginalString
            service.UrlRegex <- new Regex("https?://(?:[^:]\.)?" + domain + "(?:/.*)?")
        Some(service)
    | None -> None

let loadServices callback =
    let client = new WebClient()
    let uri = new Uri("http://tos-dr.info/index/services.json")
    try
        let services = client.DownloadString(uri) |> deserialize<Services>
        Seq.map (fun service -> async { return mergeService service }) services |> Async.Parallel |> Async.RunSynchronously |> Seq.choose id
    with
    | :? Reflection.TargetInvocationException as e -> 
        let response = (e.InnerException :?> WebException).Response :?> HttpWebResponse
        match response.StatusCode with
        | HttpStatusCode.NotFound -> sprintf "Service list could not be found" |> Debug.WriteLine; raise e
        | _ -> sprintf "Unexpected HTTP error trying to load service list: %s" e.InnerException.Message |> Debug.WriteLine; raise e
    | e -> sprintf "Unexpected exception trying to load and parse services: %s" e.Message |> Debug.WriteLine; raise e
