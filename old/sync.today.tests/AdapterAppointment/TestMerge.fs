﻿module TestMerge

open Common
open NUnit.Framework
open sync.today.Models
open AdapterAppointmentsSQL
open FsUnit
open System
open AdapterAppointmentRepository

[<TestFixture>] 
type ``Adapter Apointment Merge`` ()=
    //[<BeforeTest>] member x.

    let logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    let emptyAdapterAppointment : AdapterAppointmentDTO = 
        { Id = 0; InternalId = Guid.Empty; LastModified = DateTime.Now; Category = None; Location = None; Content = None; 
          Title=Some("Title"); DateFrom = DateTime.Parse("2015-01-01");
          DateTo = DateTime.Parse("2015-02-02"); ReminderMinutesBeforeStart = 15; Notification = false; IsPrivate = false; 
          Priority = byte 0; AppointmentId = 0; AdapterId = 0; Tag = None }

    [<TestFixtureSetUp>] 
    member x.``Log Test At the beginning`` ()=         
            logger.Info("Test")

    [<SetUp>] 
    member x.``empty and prepare database`` ()=         
            logger.Info("Entered")
            let seed = new Oak.Seed()
            seed.PurgeDb()
            let schema = new CreateDB.Schema(seed)
            for s in schema.Scripts() do 
                let sql = s.Invoke()
                seed.ExecuteNonQuery( sql )
            logger.Info("Structure done ")

    [<Test>] 
    member x.``save adapter appointment`` ()=
            let consumerId = ConsumerRepository.Insert( { Id = 0; Name = "Consumer" } ).Id
            let adapterId  = AdapterRepository.EnsureAdapter("A", "A").Id
            let internalId = Guid.Parse("f182ba28-24cc-402b-b593-0e64dfd3fce8");
            let app =                 {Id = 3163;
                 InternalId = internalId;
                 LastModified = DateTime.Parse("2015-02-02");
                 Category = None;
                 Location = None;
                 Content =
                  Some
                    """DaADKASDKASO
                        """;
                 Title = Some "BANK CITY";
                 DateFrom = DateTime.Parse("2015-03-03");
                 DateTo = DateTime.Parse("2015-03-03");
                 ReminderMinutesBeforeStart = 15;
                 Notification = true;
                 IsPrivate = false;
                 Priority = byte 0;
                 AppointmentId = 1576;
                 AdapterId = 1;
                 Tag = Some -1;}
            insertAppointmentAndAdapterAppointments( app, consumerId  )
            AdapterAppointmentRepository.InsertOrUpdate app |> ignore

    [<Test>] 
    member x.``when I merge one adapter appointment, it is the winner`` ()=
            let consumerId = ConsumerRepository.Insert( { Id = 0; Name = "Consumer" } ).Id
            let adapterId  = AdapterRepository.EnsureAdapter("A", "A").Id
            insertAppointmentAndAdapterAppointments( emptyAdapterAppointment, consumerId  )
            let ada = Get( emptyAdapterAppointment.InternalId, adapterId ).Value
            ada.Equals(ada) |> should equal true
            let mergeWinner = merge( [| ada |] )
            mergeWinner.Equals(ada) |> should equal true

    [<Test>] 
    member x.``when I merge two adapter appointments without history and with one change, the later modified is the winner`` ()=
            let consumerId = ConsumerRepository.Insert( { Id = 0; Name = "Consumer" } ).Id
            let adapter1Id  = AdapterRepository.EnsureAdapter("A", "A").Id
            let adapter2Id  = AdapterRepository.EnsureAdapter("B", "B").Id
            insertAppointmentAndAdapterAppointments( emptyAdapterAppointment, consumerId  )
            let adapterAppointmentInDb = (Seq.toArray (AdapterAppointments(1))).[0]
            insertAppointmentAndAdapterAppointments( {emptyAdapterAppointment with InternalId = Guid.NewGuid(); Location = Some( optionString2String emptyAdapterAppointment.Location + "2" ) } , consumerId  )
            let adapterAppointmentInDb2 = (Seq.toArray (AdapterAppointments(2))).[0]
            adapterAppointmentInDb.Equals( adapterAppointmentInDb ) |> should equal true
            adapterAppointmentInDb.Equals( adapterAppointmentInDb2 ) |> should equal false
            let mergeWinner = merge( [| adapterAppointmentInDb; adapterAppointmentInDb2 |] )
            mergeWinner.Equals(adapterAppointmentInDb) |> should equal false
            mergeWinner.Equals(adapterAppointmentInDb2) |> should equal true

    [<Test>] 
    member x.``when I merge two adapter appointments with history and with not overlapping changes, the composed merge is the winner`` ()=
            let consumerId = ConsumerRepository.Insert( { Id = 0; Name = "Consumer" } ).Id
            let adapter1Id  = AdapterRepository.EnsureAdapter("A", "A").Id
            let adapter2Id  = AdapterRepository.EnsureAdapter("B", "B").Id
            insertAppointmentAndAdapterAppointments( emptyAdapterAppointment, consumerId  )
            let adapterAppointmentInDb = (Seq.toArray (AdapterAppointments(1))).[0]
            insertAppointmentAndAdapterAppointments( {emptyAdapterAppointment with InternalId = Guid.NewGuid() } , consumerId  )
            let adapterAppointmentInDb2 = (Seq.toArray (AdapterAppointments(2))).[0]

            save2OldAdapterAppointments() |> ignore

            Update( adapterAppointmentInDb.InternalId, {adapterAppointmentInDb with Location = Some( optionString2String adapterAppointmentInDb.Location + "2" ) } ) |> ignore
            Update( adapterAppointmentInDb2.InternalId, {adapterAppointmentInDb2 with Content = Some( optionString2String adapterAppointmentInDb2.Content + "2" ) } ) |> ignore

            let adapterAppointmentInDb = (Seq.toArray (AdapterAppointments(1))).[0]
            let adapterAppointmentInDb2 = (Seq.toArray (AdapterAppointments(2))).[0]

            let mergeWinner = merge( [| adapterAppointmentInDb; adapterAppointmentInDb2 |] )

            //let mergeWinner = {emptyAdapterAppointment with Location = ( emptyAdapterAppointment.Location + "2" ); Content = ( emptyAdapterAppointment.Content + "2" ) }

            mergeWinner.Location |> should equal adapterAppointmentInDb.Location
            mergeWinner.Content |> should equal adapterAppointmentInDb2.Content


    [<Test>] 
    member x.``when I merge two adapter appointments with history and with overlapping changes, the composed merge with latest changes is the winner`` ()=
            let consumerId = ConsumerRepository.Insert( { Id = 0; Name = "Consumer" } ).Id
            let adapter1Id  = AdapterRepository.EnsureAdapter("A", "A").Id
            let adapter2Id  = AdapterRepository.EnsureAdapter("B", "B").Id
            insertAppointmentAndAdapterAppointments( emptyAdapterAppointment, consumerId  )
            let adapterAppointmentInDb = (Seq.toArray (AdapterAppointments(1))).[0]
            insertAppointmentAndAdapterAppointments( {emptyAdapterAppointment with InternalId = Guid.NewGuid() } , consumerId  )
            let adapterAppointmentInDb2 = (Seq.toArray (AdapterAppointments(2))).[0]

            save2OldAdapterAppointments() |> ignore

            Update( adapterAppointmentInDb2.InternalId, {adapterAppointmentInDb2 with Content = Some( optionString2String adapterAppointmentInDb2.Content + "2" ) } ) |> ignore
            Update( adapterAppointmentInDb.InternalId, 
                {adapterAppointmentInDb with 
                    LastModified = DateTime.Now.AddHours(float -10.0); 
                    Location = Some( optionString2String adapterAppointmentInDb.Location + "2" );
                    Content = Some( optionString2String adapterAppointmentInDb.Content + "1" ) } ) |> ignore

            let adapterAppointmentInDb = (Seq.toArray (AdapterAppointments(1))).[0]
            let adapterAppointmentInDb2 = (Seq.toArray (AdapterAppointments(2))).[0]

            let mergeWinner = merge( [| adapterAppointmentInDb; adapterAppointmentInDb2 |] )

            //let mergeWinner = {emptyAdapterAppointment with Location = ( emptyAdapterAppointment.Location + "2" ); Content = ( emptyAdapterAppointment.Content + "2" ) }

            mergeWinner.Location |> should equal adapterAppointmentInDb.Location
            mergeWinner.Content |> should equal adapterAppointmentInDb2.Content
