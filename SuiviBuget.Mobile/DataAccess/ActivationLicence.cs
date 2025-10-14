using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SuiviBuget.Mobile.DataAccess
{
    public class ActivationLicence
    {
        [PrimaryKey]
        public Guid ID { get; set; }
        public string Identifiant { get; set; }
        public DateTime? DateActivation { get; set; }
        public DateTime? DateExpiration { get; set; }
        public string CodeActivation { get; set; }
        public bool Statut { get; set; }
    }
}
