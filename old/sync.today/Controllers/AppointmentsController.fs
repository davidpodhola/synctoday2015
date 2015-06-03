﻿module AppointmentsController

open System.Net
open System.Net.Http
open System.Web.Http
open sync.today.Models

/// Retrieves values.
[<RoutePrefix("api")>]
type AppointmentsController() =
    inherit ApiController()

    let logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    /// Gets all values.
    [<Route("appointments")>]
    member x.Get() = 
        logger.Debug("appointments called")
        AppointmentRepository.Appointments()

    (* 
    /// Gets a single value at the specified index.
    [<Route("journals/{id}")>]
    member x.Get(request: HttpRequestMessage, id: int) =
        if id >= 0 && values.Length > id then
            request.CreateResponse(values.[id])
        else 
            request.CreateResponse(HttpStatusCode.NotFound)
    *)
