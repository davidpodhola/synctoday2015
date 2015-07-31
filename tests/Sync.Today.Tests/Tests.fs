module Sync.Today.Tests

open Sync.Today
open NUnit.Framework

[<Test>]
let ``hello returns 42`` () =
  let result = Client.hello 42
  printfn "%i" result
  Assert.AreEqual(42,result)
