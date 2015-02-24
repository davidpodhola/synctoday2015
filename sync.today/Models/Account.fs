﻿namespace sync.today.Models

open System

[<CLIMutable>]
type AccountDTO =
    {   
        Id : int
        Name : string
        ConsumerId : Nullable<int>
    }
    override m.ToString() = sprintf "[%A] %A (%A)" m.Id m.Name m.ConsumerId 