using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amorim_P3_Books_Finalforreal.Models
{
    public class UpsertInvoiceModel
    {
        public Invoice Invoice { get; set; }
        public List<Customer> Customers { get; set; }
    }
}