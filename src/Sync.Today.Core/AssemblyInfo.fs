namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("Sync.Today.Core")>]
[<assembly: AssemblyProductAttribute("Sync.Today")>]
[<assembly: AssemblyDescriptionAttribute("Sync.Today is a business processes automation platform. It is an application integration server. Exchanging business documents among different systems both within or across organizational boundaries Sync.Today integrates business processes.")>]
[<assembly: AssemblyVersionAttribute("1.0.1")>]
[<assembly: AssemblyFileVersionAttribute("1.0.1")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0.1"
