module TestCSV

open System.IO
open Sync.Today
open NUnit.Framework

let linePageMatch_1 line =
    0

let pageStructure_1 firstLine pageNum =
    "Date,Open,High,Low,Close,Volume,Adj Close"

let fileContent_1 =
    [|   
        "Date,Open,High,Low,Close,Volume,Adj Close";
        "2012-01-27,29.45,29.53,29.17,29.23,44187700,29.23";
        "2012-01-26,29.61,29.70,29.40,29.50,49102800,29.50";
        "2012-01-25,29.07,29.65,29.07,29.56,59231700,29.56";
        "2012-01-24,29.47,29.57,29.18,29.34,51703300,29.34"       
    |] 

[<Test>]
let ``dividing simple file with structure should result it just the file content`` () =
  let fileName = Common.createTemporaryFile
  File.WriteAllLines( 
    fileName, fileContent_1
  )

  let lines = File.ReadAllLines( fileName )
  let result = CSV.divideFileIntoPages lines linePageMatch_1 pageStructure_1
  Assert.IsNotNull(result)
  let page1 = Seq.head result
  let pageNum, lines = page1
  Assert.AreEqual( 0, pageNum )
  let linesA = Seq.toArray lines
  Assert.AreEqual( 5, linesA.Length )
  Assert.IsFalse( Seq.map2 (=) ( Array.toSeq fileContent_1 ) lines |> Seq.exists( fun p -> not p ) )
