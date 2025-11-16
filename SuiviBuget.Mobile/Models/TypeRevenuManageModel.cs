using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SuiviBuget.Mobile.Models
{
    public partial class TypeRevenuManageModel
    {
        public string CodeTypeRevenu { get; set; }
        public string LibelleTypeRevenu { get; set; }
        public bool EstActive { get; set; }
    }
}
