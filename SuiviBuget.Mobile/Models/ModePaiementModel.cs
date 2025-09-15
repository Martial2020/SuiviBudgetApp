using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace SuiviBuget.Mobile.Models
{
    public partial class ModePaiementModel : ObservableObject
    {
        [ObservableProperty]
        public string codeModePaiement;
        [ObservableProperty]
        public string libelleModePaiement;
    }
}
