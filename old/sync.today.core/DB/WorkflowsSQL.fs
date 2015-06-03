﻿module WorkflowsSQL

open Common
open System
open System.Data
open System.Data.SqlClient
open sync.today.Models
open FSharp.Data

type GetWorkflowsQuery = SqlCommandProvider<"GetAllWorkflows.sql", ConnectionStringName>
type InsertOrUpdateWorkflowQuery = SqlCommandProvider<"InsertOrUpdateWorkflow.sql", ConnectionStringName>
type DeleteWorkflowQuery = SqlCommandProvider<"DeleteWorkflow.sql", ConnectionStringName>

let internal convert( r : GetWorkflowsQuery.Record ) : WorkflowDTO =
    { Id = r.Id; CreatedOn = r.CreatedOn; Name = r.Name; XamlCode = r.XamlCode }
let internal convert2( r : InsertOrUpdateWorkflowQuery.Record ) : WorkflowDTO =
    { Id = r.Id; CreatedOn = r.CreatedOn; Name = r.Name; XamlCode = r.XamlCode }

let workflows()  = 
    ( new GetWorkflowsQuery() ).AsyncExecute(null) |> Async.RunSynchronously |> Seq.map ( fun t -> convert(t) )

let workflowByName( name : string)  = 
    ( new GetWorkflowsQuery() ).AsyncExecute(name) |> Async.RunSynchronously |> Seq.head |> convert

let ensureWorkflow( name : string, xaml : string ) :  WorkflowDTO =
    ( new InsertOrUpdateWorkflowQuery() ).AsyncExecute(xaml, name) 
                                                        |> Async.RunSynchronously 
                                                        |> Seq.head |> convert2

let deleteWorkflow( name : string)  = 
    ( new DeleteWorkflowQuery() ).AsyncExecute(name)|> Async.RunSynchronously
