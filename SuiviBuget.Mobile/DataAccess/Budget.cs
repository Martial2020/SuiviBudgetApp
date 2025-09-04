using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SuiviBudget.Services.DataAccess
{
    public class Budget
    {
        [PrimaryKey]
        public string CodeBudget { get; set; }
        public string LibelleBudget { get; set; }
        public string DescriptionBudget { get; set; }
        public decimal MontantBudget { get; set; }
        public decimal MontantUtilise { get; set; }
        public decimal MontantRestant { get; set; }
        public DateTime DateCreationBudget { get; set; }
        public DateTime DateDebutBudget { get; set; }
        public DateTime DateFinBudget{ get; set; }
        public string StatutBudget{ get; set; }
        public int  NbreLigneBudgetaire{ get; set; }
    }
}
