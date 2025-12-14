using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SuiviBuget.Mobile.Models
{
    public partial class RevenuDetailModel : ObservableObject
    {
        [ObservableProperty]
        public string codeTypeRevenu;

        [ObservableProperty]
        public string description;

        [ObservableProperty]
        public DateTime dateReception;

        [ObservableProperty]
        public string codeModePaiement;

        [ObservableProperty]
        public decimal montant;
    }
}
