module TestCSV

open System
open System.IO
open Sync.Today
open NUnit.Framework

let linePageMatch_1 line =
    0

let pageStructure_1 pageNum =
    "Date,Open,High,Low,Close,Volume,Adj Close"

let fileContent_1 =
    [|   
        "Date,Open,High,Low,Close,Volume,Adj Close";
        "2012-01-27,29.45,29.53,29.17,29.23,44187700,29.23";
        "2012-01-26,29.61,29.70,29.40,29.50,49102800,29.50";
        "2012-01-25,29.07,29.65,29.07,29.56,59231700,29.56";
        "2012-01-24,29.47,29.57,29.18,29.34,51703300,29.34"       
    |] 


let linePageMatch_2 line =
//  match line with
//    | [] -> 0
//    | head::_ ->
//      let (_, res) = Int32.TryParse head
//      res
    let firstChar = ( ( line : string ).[0] ).ToString()
    let section = ref 0
    if Int32.TryParse( firstChar, section ) then
        section.Value
    else
        0

let pageStructure_2 pageNum =
    match pageNum with
    | 0 -> "PageNum;Comment"
    | 3 -> "PageNum;MP;OD;DO;TAR;Q;CONST;NR;CAT;R;REC;SELF"
    | 9 -> "PageNum;Comment"

let fileContent_2_0 =
    [|   
        "0;AISIOJASD;01.05.2015;01.06.2015;115493";
        "0;JuJsa;3063.000";
        "0;DSJCVLKMA";
    |] 
let fileContent_2_3 =
    [|   
        "3;1301372912;10.02.2015 00:00;01.03.2015 00:00;AAA;16;1;66703378274;K0;O;0;;";
        "3;1301372912;10.02.2015 00:00;01.03.2015 00:00;AAA;-167;1;66703378274;K0;O;1;;";
        "3;1301372912;10.02.2015 00:00;01.03.2015 00:00;BBB;2;1;66703378274;K0;O;0;;";
        "3;1301372912;10.02.2015 00:00;01.03.2015 00:00;BBB;-22;1;66703378274;K0;O;1;;";
    |] 
let fileContent_2_9 =
    [|   
        "9;4"
    |] 

     
let fileContent_2 = Array.concat ( seq [ fileContent_2_0; fileContent_2_3; fileContent_2_9 ] )


[<Test>]
let ``dividing simple file with structure should result it just the file content`` () =
  let fileName = Common.createTemporaryFile ()
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

[<Test>]
let ``dividing file by first char without structure should result divided lines with structure added`` () =
  let fileName = Common.createTemporaryFile ()
  File.WriteAllLines( 
    fileName, fileContent_2
  )

  let lines = File.ReadAllLines( fileName )
  let result = CSV.divideFileIntoPages lines linePageMatch_2 pageStructure_2
  Assert.IsNotNull(result)
  let resultA = Seq.toArray result
  Assert.AreEqual( 3, resultA.Length )
  
  let page = resultA.[0]
  let pageNum, lines = page
  Assert.AreEqual( 0, pageNum )
  let linesA = Seq.toArray lines
  Assert.AreEqual( 4, linesA.Length )
  Assert.AreEqual(pageStructure_2 0, linesA.[0] )
  Assert.IsFalse( Seq.map2 (=) ( Array.toSeq fileContent_2_0 ) (lines |> Seq.skip 1) |> Seq.exists( fun p -> not p ) )

  let page = resultA.[1]
  let pageNum, lines = page
  Assert.AreEqual( 3, pageNum )
  let linesA = Seq.toArray lines
  Assert.AreEqual( 5, linesA.Length )
  Assert.AreEqual(pageStructure_2 3, linesA.[0] )
  Assert.IsFalse( Seq.map2 (=) ( Array.toSeq fileContent_2_3 ) (linesA |> Seq.skip 1) |> Seq.exists( fun p -> not p ) )
