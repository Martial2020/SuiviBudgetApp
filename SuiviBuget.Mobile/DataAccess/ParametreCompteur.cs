using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SuiviBudget.Services.DataAccess
{
    public class ParametreCompteur
    {
        [PrimaryKey]
        public string CodeParametre { get; set; }
        public int DernierNumeroEnregistre { get; set; }
    }
}
