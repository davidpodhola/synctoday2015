﻿module ConsumerAdaptersSQL

open Common
open System
open System.Data
open System.Data.SqlClient
open sync.today.Models
open FSharp.Data

type private InsertOrUpdateConsumerAdapterQuery = SqlCommandProvider<"InsertOrUpdateConsumerAdapter.sql", ConnectionStringName>
type private GetConsumerAdaptersQuery = SqlCommandProvider<"GetConsumerAdapters.sql", ConnectionStringName>

let internal convert( r : InsertOrUpdateConsumerAdapterQuery.Record ) : ConsumerAdapterDTO =
    { Id = r.Id; AdapterId = r.AdapterId; ConsumerId = r.ConsumerId; DataJSON = r.DateJSON }
let internal convert2( r : GetConsumerAdaptersQuery.Record ) : ConsumerAdapterDTO =
    { Id = r.Id; AdapterId = r.AdapterId; ConsumerId = r.ConsumerId; DataJSON = r.DateJSON }

let convertOp(c) = 
    convertOption( c, convert2 )

let insertOrUpdateConsumerAdapter( consumerAdapter : ConsumerAdapterDTO ) = 
    ( new InsertOrUpdateConsumerAdapterQuery() ).AsyncExecute(consumerAdapter.AdapterId, consumerAdapter.ConsumerId, consumerAdapter.DataJSON) 
        |> Async.RunSynchronously |> Seq.head |> convert

let consumerAdapters() : ConsumerAdapterDTO seq =
    ( new GetConsumerAdaptersQuery() ).AsyncExecute(0,0, null) |> Async.RunSynchronously |> Seq.map ( fun t -> convert2(t) )

#if consumerAdapterById
let consumerAdapterById( id : int ) : ConsumerAdapterDTO option =
    let db = db()
    query {
        for r in db.ConsumerAdapters do
        where ( r.Id = id )
        select ( convert(r) )
    } |> Seq.tryHead
#endif

let consumerAdapterByConsumerAdapter( consumerId : int, adapterId : int ) : ConsumerAdapterDTO option =
    ( new GetConsumerAdaptersQuery() ).AsyncExecute(adapterId,consumerId, null) |> Async.RunSynchronously |> Seq.tryHead |> convertOp

let consumerAdapter( consumer : ConsumerDTO, adapter : AdapterDTO ) : ConsumerAdapterDTO option =
    consumerAdapterByConsumerAdapter( consumer.Id, adapter.Id )

#if ensureConsumerAdapter
let ensureConsumerAdapter( consumerAdapter : ConsumerAdapterDTO ) = 
    let potentialConsumerAdapter = consumerAdapterByConsumerAdapter( consumerAdapter.ConsumerId, consumerAdapter.AdapterId )
    if potentialConsumerAdapter.IsNone then
        insertConsumerAdapter( consumerAdapter )
    else
        potentialConsumerAdapter.Value
#endif

let getConsumerAdapterByAdapterIdAndData( adapterId : int, data : string ) : ConsumerAdapterDTO option =
    ( new GetConsumerAdaptersQuery() ).AsyncExecute(adapterId,0, data) |> Async.RunSynchronously |> Seq.tryHead |> convertOp
