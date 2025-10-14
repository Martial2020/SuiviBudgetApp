using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuiviBuget.Mobile.Models
{
    public class BudgetManageModel
    {
        public string CodeBudget { get; set; }
        public string LibelleBudget { get; set; }
        public string DescriptionBudget { get; set; }
        public decimal MontantBudget { get; set; }
        public decimal MontantAlloue { get; set; }
        public decimal MontantNonAlloue { get; set; }
        public decimal MontantReajustement { get; set; }
        
        public decimal MontantUtilise { get; set; }
        public decimal MontantRestant{ get; set; }
        public DateTime DateCreationBudget { get; set; }
        public DateTime DateDebutBudget { get; set; }
        public DateTime DateFinBudget { get; set; }
        public string StatutBudget { get; set; }
        public int NbreLigneBudgetaire { get; set; }
        public Color BackgroundColorStatut { get; set; }
    }
}
