namespace sync.today.Models

open System

[<CLIMutable>]
type ExchangeAppointmentDTO =
    {   Id : int
        InternalId : Guid
        ExternalId : string option
        Body : string option
        Start : DateTime
        End : DateTime
        LastModifiedTime : DateTime
        Location : string option
        IsReminderSet : bool
        AppointmentState : byte
        Subject : string option
        RequiredAttendeesJSON : string option
        ReminderMinutesBeforeStart : int
        Sensitivity : byte
        RecurrenceJSON : string option
        ModifiedOccurrencesJSON : string option
        LastOccurrenceJSON : string option
        IsRecurring : bool
        IsCancelled : bool
        ICalRecurrenceId : string option
        FirstOccurrenceJSON : string option
        DeletedOccurrencesJSON : string option
        AppointmentType : byte
        Duration : int
        StartTimeZone : string option
        EndTimeZone : string option
        AllowNewTimeProposal : bool
        CategoriesJSON : string option
        ServiceAccountId : int
        Tag : int option
    }
    override m.ToString() = sprintf "[%A] (%A-%A) %A [%A]" m.Id m.InternalId m.ExternalId m.Subject m.ServiceAccountId 
