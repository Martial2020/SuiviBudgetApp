using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SuiviBuget.Mobile.Models
{
    public partial class LigneBudgetaireModel : ObservableObject
    {
        [ObservableProperty]
        private string? codeLigneBudgetaire;

        [ObservableProperty]
        private string? libelleLigneBudgetaire;
    }
}
