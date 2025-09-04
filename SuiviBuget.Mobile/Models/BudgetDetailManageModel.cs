using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SuiviBuget.Mobile.Models
{
    public class BudgetDetailManageModel
    {
        
        public Guid BudgetDetailID { get; set; }
        public string CodeBudget { get; set; }
        public string CodeLigneBudgetaire { get; set; }
        public string LibelleLigneBudgetaire { get; set; }
        public decimal Montant { get; set; }
    }
}
