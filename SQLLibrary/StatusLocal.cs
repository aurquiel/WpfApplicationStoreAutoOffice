//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SQL
{
    using System;
    using System.Collections.Generic;
    
    public partial class StatusLocal
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StatusLocal()
        {
            this.Task = new HashSet<Task>();
        }
    
        public int status_local_id { get; set; }
        public string status_local_description { get; set; }
        public int status_local_audit_id { get; set; }
        public System.DateTime status_local_audit_date { get; set; }
        public bool status_local_audit_deleted { get; set; }
    
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Task> Task { get; set; }
    }
}
