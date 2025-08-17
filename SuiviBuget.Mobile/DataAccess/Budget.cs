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
        public DateTime DateDebutBudget { get; set; }
        public DateTime DateDebutFin{ get; set; }
        public bool StatutBudget{ get; set; }
    }
}
