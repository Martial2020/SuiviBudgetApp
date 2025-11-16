using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SuiviBuget.Mobile.DataAccess
{
    public class Revenu
    {
        [PrimaryKey]

        public string CodeRevenu { get; set; }
        public string CodeTypeRevenu { get; set; }
        public decimal Montant { get; set; }
        public DateTime DateDernierMisAJour { get; set; }
    }
}
