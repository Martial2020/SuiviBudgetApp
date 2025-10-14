using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuiviBuget.Mobile.Models
{
    public class GrapheModel
    {
        public string LigneBudgetaire { get; set; }
        public float MontantLigneBudgetaire { get; set; }
        public float MontantLigneUtilise { get; set; }
        public string Description { get; set; }
        public float Depassement { get; set; }
    }
}
