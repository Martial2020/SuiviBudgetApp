using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SuiviBudget.Services.DataAccess
{
    public class LigneBudgetaire
    {
        [PrimaryKey]
        public string CodeLigneBudgetaire { get; set; }
        public string LibelleLigneBudgetaire { get; set; }
    }
}
