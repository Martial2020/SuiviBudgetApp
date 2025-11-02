using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SuiviBuget.Mobile.DataAccess
{
    public class TypeRevenu
    {
        [PrimaryKey]

        public string CodeTypeRevenu { get; set; }
        public string LibelleTypeRevenu{ get; set; }
    }
}
