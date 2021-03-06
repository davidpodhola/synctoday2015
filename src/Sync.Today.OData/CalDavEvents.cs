//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sync.Today.OData
{
    using System;
    using System.Collections.Generic;
    
    public partial class CalDavEvents
    {
        public int Id { get; set; }
        public System.Guid InternalId { get; set; }
        public string ExternalId { get; set; }
        public string Description { get; set; }
        public System.DateTime Start { get; set; }
        public System.DateTime End { get; set; }
        public System.DateTime LastModified { get; set; }
        public string Location { get; set; }
        public string Summary { get; set; }
        public string CategoriesJSON { get; set; }
        public int ServiceAccountId { get; set; }
        public bool Upload { get; set; }
        public Nullable<int> Tag { get; set; }
        public bool IsNew { get; set; }
        public bool WasJustUpdated { get; set; }
        public string LastDLError { get; set; }
        public string LastUPError { get; set; }
    
        public virtual ServiceAccounts ServiceAccounts { get; set; }
    }
}
