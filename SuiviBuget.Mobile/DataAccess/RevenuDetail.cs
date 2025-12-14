using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SuiviBuget.Mobile.DataAccess
{
    public class RevenuDetail
    {
        [PrimaryKey]
        public Guid RevenuDetailID { get; set; }
        //public string CodeTypeRevenu { get; set; }
        public string CodeRevenu { get; set; }
        public string CodeModePaiement { get; set; }
        public decimal Montant { get; set; }
        public DateTime DateReception { get; set; }
        public string Description { get; set; }
    }
}
