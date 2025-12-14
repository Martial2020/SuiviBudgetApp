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
        public string MontantBudgetAvecDevise { get; set; }

        public decimal MontantAlloue { get; set; }
        public string MontantAlloueAvecDevise { get; set; }

        public decimal MontantNonAlloue { get; set; }
        public string MontantNonAlloueAvecDevise { get; set; }

        public decimal MontantReajustement { get; set; }
        public string MontantReajustementAvecDevise { get; set; }

        public decimal MontantUtilise { get; set; }
        public string MontantUtiliseAvecDevise { get; set; }
        public decimal MontantRestant{ get; set; }
        public string MontantRestantAvecDevise { get; set; }
        public DateTime DateCreationBudget { get; set; }
        public DateTime DateDebutBudget { get; set; }
        public DateTime DateFinBudget { get; set; }
        public string StatutBudget { get; set; }
        public int NbreLigneBudgetaire { get; set; }
        public Color BackgroundColorStatut { get; set; }
    }
}
