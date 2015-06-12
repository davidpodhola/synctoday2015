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
let pageStructureExample firstLine pageNum =
    "Date,Open,High,Low,Close,Volume,Adj Close"

/// Take lines and divide it info several files based on line page matching
/// Each page must start with the page structure definition line
let divideFileIntoPages lines linePageMatch pageStructureExample =
    lines |> 
    Seq.map( fun line -> 
        let pageNum = linePageMatch line
        ( pageNum, line )
    ) // TODO: finish here
    |> ignore // TODO: remove later
    // TODO: remove dummy result below
    seq [ 
        ( 0, seq [ "Name,Value"; "Cost,1000" ] ); 
        ( 1, seq [ "Date,Open,High,Low,Close,Volume,Adj Close"; "2012-01-27,29.45,29.53,29.17,29.23,44187700,29.23"; "2012-01-26,29.61,29.70,29.40,29.50,49102800,29.50" ] ) 
    ] 
    (* uncomment this to just return the content of lines unchanged  
    |> ignore
    seq [ 
        ( 0, seq lines )
    ]
    *)
