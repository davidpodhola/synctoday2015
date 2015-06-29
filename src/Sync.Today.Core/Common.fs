(**
# Common functions
Publicly usable common functions
*)
module Common

open System
open System.IO
open System.Xml
open System.Text
open Newtonsoft.Json
open Newtonsoft.Json.Linq
open System.Collections

[<Literal>]
let ConnectionStringName="name=sync-today-mssql"

module Seq =
    ///Try to get the first element from a sequence    
    let tryHead xs = xs |> Seq.tryPick Some

/// Object to Json 
let public json<'t> (myObj:'t) =   
        JsonConvert.SerializeObject(myObj)

/// Object from Json 
let public unjson<'t> (jsonString:string)  : 't =  
        let obj = JsonConvert.DeserializeObject(jsonString)
        if obj :? 't then 
            obj :?> 't
        else
            let jToken = obj :?> JToken
            jToken.ToObject<'t>()

/// remove miliseconds from a DateTime
let fixDateTime( a : DateTime ) : DateTime =
    a.AddTicks( -(a.Ticks % TimeSpan.TicksPerSecond) )

/// remove miliseconds and seconds from a DateTime
let fixDateSecs( a : DateTime ) : DateTime =
    fixDateTime( a.AddSeconds( float -a.Second ) )

/// intersect a sequence and an array    
let intersect x y = Set.intersect (Set.ofSeq x) (Set.ofArray y)

/// convert `String option` to `String` the way that if the parameter is `None` then return `null`
let optionString2String( optionString : String option ) =
    if optionString.IsNone then null else optionString.Value 

/// log for logging the developer's information not expected to be read by admins
let devlog = log4net.LogManager.GetLogger("DevLog");
/// log to write an entity save was ignored because of being same as the data already stored in the database
let ignlog = log4net.LogManager.GetLogger( "IgnoreLog" )

/// convert `String` to `String option` the way that if the parameter is `null` then return `None`
let string2optionString( s : string ) : string option =
    match s with
    | null -> None
    | some -> Some(some)

/// 
type IEqualityComparer<'T> = Generic.IEqualityComparer<'T>

/// Equals using function f
let equalIf f (x:'T) (y:obj) =
  if obj.ReferenceEquals(x, y) then true
  else
    match box x, y with
    | null, _ | _, null -> false
    | _, (:? 'T as y) -> f x y
    | _ -> false

/// Equal by comporer 
let equalByWithComparer (comparer:IEqualityComparer<_>) f (x:'T) (y:obj) = 
  (x, y) ||> equalIf (fun x y -> comparer.Equals(f x, f y))

/// Equal by projection
let equalByProjection proj (comparer:IEqualityComparer<_>) f (x:'T) (y:obj) = 
  (x, y) ||> equalIf (fun x y -> 
    Seq.zip (proj x) (proj y)
    |> Seq.forall obj.Equals && comparer.Equals(f x, f y))

/// Equal by string 
let equalByString f (x:'T) (y:obj) = 
  (x, y) ||> equalIf (fun x y -> StringComparer.InvariantCulture.Equals(f x, f y))

/// apply convert to an option or return None for None
let convertOption convert a =
    match a with
    | Some r -> Some(convert(r))
    | None -> None

/// return true if both strings are null/empty or have the same content
let stringsAreEqual a b =
    if String.IsNullOrEmpty a then
        String.IsNullOrEmpty b 
    else
        String.Equals( a, b )

/// Return true if two 
let optionstringsAreEqual a b =
    let a_s = optionString2String a
    let b_s = optionString2String b
    stringsAreEqual a_s b_s

/// Return true if string option is None or the content is Null or empty
let optionstringIsEmpty a =
    match a with
    | Some r -> String.IsNullOrEmpty( r )
    | None -> true


let createTemporaryFile () =
        let fileName = Path.GetTempFileName()

        let fileInfo = new FileInfo(fileName)
        fileInfo.Attributes <- FileAttributes.Temporary

        fileName

let int2string p=
    ( p : int ).ToString()