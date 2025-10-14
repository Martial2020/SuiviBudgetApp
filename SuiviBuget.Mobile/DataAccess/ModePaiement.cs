using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SuiviBuget.Mobile.DataAccess
{
    public class ModePaiement
    {
        [PrimaryKey]

        public string CodeModePaiement { get; set; }
        public string LibelleModePaiement { get; set; }
    }
}
