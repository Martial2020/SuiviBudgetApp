using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuiviBuget.Mobile.Models
{
    public class BudgetModel
    {
        public string CodeBudget { get; set; }
        public string LibelleBudget { get; set; }
        public string DescriptionBudget { get; set; }
        public decimal MontantBudget { get; set; }
        public DateTime DateCreationBudget { get; set; }
        public DateTime DateDebutBudget { get; set; }
        public DateTime DateDebutFin { get; set; }
        public bool StatutBudget { get; set; }
        public int NbreLigneBudgetaire { get; set; }
    }
}
