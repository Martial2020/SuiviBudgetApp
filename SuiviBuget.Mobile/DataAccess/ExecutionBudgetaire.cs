using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SuiviBuget.Mobile.DataAccess
{
    public class ExecutionBudgetaire
    {
        [PrimaryKey]
        public Guid ExecutionBudgetaireID { get; set; }
        public string CodeBudget { get; set; }
        public string CodeLigneBudgetaire { get; set; }
        public string CodeModePaiement { get; set; }
        public string Description { get; set; }
        public decimal Montant { get; set; }
        public DateTime DateExecution { get; set; }
    }
}
