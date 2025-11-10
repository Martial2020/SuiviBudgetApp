using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SuiviBuget.Mobile.DataAccess
{
    public class Devise
    {
        [PrimaryKey]

        public string CodeDevise { get; set; }
        public string LibelleDevise { get; set; }
        public bool EstActive { get; set; }
    }
}
