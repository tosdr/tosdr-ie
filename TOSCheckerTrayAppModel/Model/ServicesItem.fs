namespace TOSCheckerTrayApp.Model

open System
open System.Collections.Generic
open System.Runtime.Serialization
open System.Runtime.Serialization.Json

[<DataContract>]
type LastArchiveItem =
    { [<DataMember(Name = "uri")>] mutable Uri : Uri
      [<DataMember(Name = "time")>] mutable Time : string }

[<DataContract>]
type LinkItem =
    { [<DataMember(Name = "name")>] mutable Name : string
      [<DataMember(Name = "url")>] mutable Url : Uri
      [<DataMember(Name = "last-archive")>] mutable LastArchive : LastArchiveItem }

type Points = string[]
type Links = Dictionary<string, LinkItem>

[<DataContract>]
type ServicesItem =
    { [<DataMember(Name = "points")>] mutable Points : Points
      [<DataMember(Name = "links")>] mutable Links : Links }

[<DataContract>]
type Services = Dictionary<string, ServicesItem>
