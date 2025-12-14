using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuiviBuget.Mobile.Models
{
    public class ReajusterManageModel
    {
        public Guid ReajusterID { get; set; }
        public string CodeBudget { get; set; }
        public string CodeLigneBudgetaire { get; set; }
        public string LibelleLigneBudgetaire { get; set; }
        public decimal Montant { get; set; }
        public string MontantAvecDevise { get; set; }
        public string Motif { get; set; }
        public DateTime DateReajustement { get; set; }
    }
}
