﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DigitalTVBilling.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DigitalTVBillingEntities : DbContext
    {
        public DigitalTVBillingEntities()
            : base("name=DigitalTVBillingEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<st_Customers> st_Customers { get; set; }

        public System.Data.Entity.DbSet<DigitalTVBilling.Models.ReceiverAttachment> ReceiverAttachments { get; set; }
    }
}
