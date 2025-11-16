using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuiviBuget.Mobile.Models
{
    public class RevenuManageModel
    {
        public string CodeRevenu { get; set; }
        public string LibelleTypeRevenu { get; set; }
        public decimal Montant { get; set; }
        public string MontantAvecDevise { get; set; }
        public DateTime DateDernierMisAJour { get; set; }
    }
}
