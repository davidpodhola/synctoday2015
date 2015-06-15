module CSV

open System

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
let divideFileIntoPages lines linePageMatch pageStructureExample =
  lines |> Seq.groupBy (fun line -> linePageMatch line) |> Seq.map (fun (ix, sq) -> (ix, seq { yield pageStructureExample ix; yield! sq }))
