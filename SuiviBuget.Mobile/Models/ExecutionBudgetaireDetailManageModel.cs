using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuiviBuget.Mobile.Models
{
    public partial class ExecutionBudgetaireDetailManageModel
    {
        public Guid ExecutionBudgetaireID { get; set; }
        public string CodeLigneBudgetaire { get; set; }
        public string LibelleLigneBudgetaire { get; set; }
        public string ModePaiement { get; set; }
        public decimal Montant { get; set; }
        public string CodeBudget { get; set; }
        public string Descritpion { get; set; }
        public DateTime DateExecution { get; set; }

    }
}
