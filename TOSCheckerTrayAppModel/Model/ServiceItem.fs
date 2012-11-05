namespace TOSCheckerTrayApp.Model

open System
open System.Runtime.Serialization
open System.Runtime.Serialization.Json
open System.Text.RegularExpressions

type TosRating = A | B | C | D | E | F | Unrated

[<DataContract>]
type ServiceItemTosDr = { [<DataMember(Name = "rated")>] mutable Rated : string }

[<DataContract>]
type ServiceItem =
    { [<DataMember(Name = "id")>] mutable Id : string
      [<DataMember(Name = "type")>] mutable Type : string
      [<DataMember(Name = "name")>] mutable Name : string
      [<DataMember(Name = "url")>] mutable Url : Uri
      [<DataMember(Name = "tosdr")>] mutable TosDr : ServiceItemTosDr
      // late init
      mutable UrlRegex : Regex }
