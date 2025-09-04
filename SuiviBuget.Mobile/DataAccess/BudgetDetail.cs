using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SuiviBuget.Mobile.DataAccess
{
    public class BudgetDetail
    {
        [PrimaryKey]
        public Guid BudgetDetailID { get; set; }
        public string CodeBudget { get; set; }
        public string CodeLigneBudgetaire { get; set; }
        public decimal Montant { get; set; }
    }
}
