using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuiviBuget.Mobile.DataAccess
{
    public class SourceBudgetDetails
    {
        public string CodeBudget { get; set; }
        public decimal Montant { get; set; }
        public DateTime DateCreation { get; set; }
        public bool EstAnnule { get; set; }
    }
}
