namespace Sync.Today

 open System  
 open System.Management.Automation  
 open System.ComponentModel  

 [<RunInstaller(true)>]  
 type FSharpPowerShellSnapIn() =  
   inherit PSSnapIn()  
   override this.Name with get() = "Sync.Today"  
   override this.Vendor with get() = "Sync.Today Ltd"  
   override this.Description with get() = "Sync.Today is a business processes automation platform"  
 
 [<Cmdlet(VerbsCommunications.Connect, "SyncTodayService")>]  
 type ``Connect-SyncTodayService``() =   
   inherit Cmdlet()  

// see http://apollo13cn.blogspot.cz/2013/11/use-f-to-write-powershell-snapin-cmdlet.html


