namespace TOSCheckerService

open System
open System.Windows
open System.ServiceModel
open TOSCheckerService.Contracts

[<ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)>]
type TOSCheckerService (loader : string -> unit) =
    interface ITOSCheckerService with
        member x.LoadDomain value =
            loader value

    static member startServer (app : Application) receiver =
        let server = new ServiceHost(new TOSCheckerService(receiver), new Uri(Constants.ServiceUrl))
        server.Open()
        app.Exit.Add(fun _ -> server.Close())
        server
