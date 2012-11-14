namespace TOSCheckerService.Contracts

open System.Runtime.Serialization
open System.ServiceModel

[<ServiceContract>]
type ITOSCheckerService =
    [<OperationContract(IsOneWay = true)>]
    abstract LoadDomain : value:string -> unit

module Constants = 
    [<Literal>]
    let ServiceUrl = "net.pipe://localhost/TOSCheckerService"
