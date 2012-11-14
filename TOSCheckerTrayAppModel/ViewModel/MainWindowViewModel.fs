namespace TOSCheckerTrayApp.ViewModel
open System
open System.ComponentModel
open System.Web
open System.Windows
open TOSCheckerService
open TOSCheckerTrayApp.Model
open TOSCheckerTrayApp.Service

type MainWindowViewModel() as self =
    inherit ViewModelBase()
    
    let inDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject())
    let mutable server = null
    let mutable closedTimeStamp = 0L
    let mutable domain = null
    let mutable visible = false
    let mutable services = Seq.empty
    let mutable trayIcon = null

    let receiver (data : string) =
        self.Domain <- data

    do 
        if not inDesignMode then 
            server <- TOSCheckerService.startServer Application.Current receiver
            services <- TOSDR.loadServices()

    member x.ClosedTimestamp
        with get() = closedTimeStamp
        and set(value) = 
            closedTimeStamp <- value
            x.OnPropertyChanged("ClosedTimestamp")

    member x.Service
        with get() =
            match x.Domain with
            | null -> None
            | domain ->
                let matchService (service : ServiceItem) =
                    match service.UrlRegex <> null && service.UrlRegex.IsMatch(domain) with
                    | true -> Some(service)
                    | false -> None
                Seq.tryPick matchService services

    member x.ServiceUri
        with get() =
            match x.Service with
            | None -> ""
            | Some(service) ->
                let uri = 
                    match service.Url.IsAbsoluteUri with
                    | true -> service.Url
                    | false -> (new UriBuilder(service.Url.OriginalString, Scheme = "http")).Uri
                sprintf "http://didnotread.github.com/browser-extensions/popup/?url=%s" (uri.ToString() |> HttpUtility.UrlEncode)
                        
    member x.HasService with get() = x.Service <> None
    member x.HasNoService with get() = not x.HasService

    member x.Rating 
        with get() =
            match x.Service with
            | None -> None
            | Some(service) ->
                match service.TosDr.Rated with
                | "False" -> Some(TosRating.Unrated)
                | "A" -> Some(TosRating.A)
                | "B" -> Some(TosRating.B)
                | "C" -> Some(TosRating.C)
                | "D" -> Some(TosRating.D)
                | "E" -> Some(TosRating.E)
                | "F" -> Some(TosRating.F)
                | _ -> Some(TosRating.Unrated)

    member x.Domain
        with get() = domain
        and set(value) =
            domain <- value
            x.OnPropertyChanged("Domain")
            x.OnPropertyChanged("Service")
            x.OnPropertyChanged("ServiceUri")
            x.OnPropertyChanged("HasService")
            x.OnPropertyChanged("HasNoService")
            x.OnPropertyChanged("Rating")

    member x.Visible
        with get() =
            match inDesignMode with
            | true -> Visibility.Visible
            | false ->
                match visible with 
                | true -> Visibility.Visible
                | false -> Visibility.Hidden
        and set(value) = 
            visible <- 
                match value with 
                | Visibility.Visible -> true 
                | _ -> false

    member x.HideWindow (window : Window) =
        x.ClosedTimestamp <- DateTime.Now.Ticks
        window.Hide()

    member x.Recenter (window : Window) =
        let workArea = SystemParameters.WorkArea
        window.Left <- (workArea.Width - window.ActualWidth) / 2.0
        window.Top <- (workArea.Height - window.ActualHeight) / 2.0
