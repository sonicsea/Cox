//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cox.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Category_Task
    {
        public int ID { get; set; }
        public int Task_ID { get; set; }
        public int Category_ID { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual Task Task { get; set; }
    }
}
