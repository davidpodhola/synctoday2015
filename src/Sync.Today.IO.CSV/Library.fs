module CSV

open System
open FSharp.Data

type type1 = CsvProvider<"""..\..\docs\content\data\MSFT.csv""">
type type2 = CsvProvider<"""..\..\docs\content\data\MSFT.csv""">

/// Example how line page matching can look like
let linePageMatchExample line =
    let firstChar = ( ( line : string ).[0] ).ToString()
    let section = ref 0
    if Int32.TryParse( firstChar, section ) then
        section.Value
    else
        0

/// Example how page structure can look like
let pageStructureExample pageNum =
    "Date,Open,High,Low,Close,Volume,Adj Close"

/// Take lines and divide it info several files based on line page matching
/// Each page must start with the page structure definition line
let divideFileIntoPages (lines:seq<string>) linePageMatch pageStructureExample =
  lines |> Seq.groupBy (fun line -> linePageMatch line) |> Seq.map (fun (ix, sq) -> 
    if sq |> Seq.take 1 |> Seq.exactlyOne = pageStructureExample ix then (ix, seq {yield! sq})
    else (ix, seq { yield pageStructureExample ix; yield! sq })
  )

let processFile =
    Array.toSeq [| 
        ( 0, type1.Load("http://ichart.finance.yahoo.com/table.csv?s=MSFT") );
        ( 1, type2.Load("http://ichart.finance.yahoo.com/table.csv?s=MSFT") )
    |]
