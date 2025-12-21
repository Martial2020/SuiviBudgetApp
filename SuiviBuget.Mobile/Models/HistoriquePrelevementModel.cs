using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuiviBuget.Mobile.Models
{
    public class HistoriquePrelevementModel
    {
        public string CodeBudget { get; set; }
        public string LibelleBudget { get; set; }
        public decimal Montant { get; set; }
        public string MontantAvecDevise { get; set; }
        public string EstAnnule { get; set; }
        public DateTime DateCreation { get; set; }
    }
}
