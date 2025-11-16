using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SuiviBuget.Mobile.Models
{
    public partial class RevenuDetailManageModel
    {
        public string CodeTypeRevenu { get; set; }
        public string LibelleTypeRevenu { get; set; }
        public string LibelleModePaiement { get; set; }
        public decimal Montant { get; set; }
        public DateTime DateReception { get; set; }
        public string Description { get; set; }
    }
}
