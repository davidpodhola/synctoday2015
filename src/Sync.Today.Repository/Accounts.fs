module Accounts

open sync.today.Models

type IRepository =
    abstract member insertAccount : AccountDTO -> AccountDTO 

