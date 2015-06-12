(**
# Sync.Today
Some more documentation using `Markdown`.
## Library
Sync.Today can be used as a library. Any consuming application can add the NuGet and
start calling 
*)
#load "Library.fs"
open Sync.Today

let num = Library.hello 42
printfn "%i" num
