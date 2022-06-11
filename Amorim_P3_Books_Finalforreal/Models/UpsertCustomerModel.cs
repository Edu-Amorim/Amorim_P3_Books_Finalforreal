using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amorim_P3_Books_Finalforreal.Models
{
    public class UpsertCustomerModel
    {
        public Customer Customer { get; set; }
        public List<State> States { get; set; }
    }
}