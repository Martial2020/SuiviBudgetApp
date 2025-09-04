using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SuiviBuget.Mobile.Models
{
    public partial class BudgetModel : ObservableObject
    {
        [ObservableProperty]
        private string? codeBudget;
        [ObservableProperty]
        public string libelleBudget;
        [ObservableProperty]
        public decimal montantBudget;
        [ObservableProperty]
        public DateTime dateCreationBudget;
        [ObservableProperty]
        public DateTime dateDebutBudget;
        [ObservableProperty]
        public DateTime dateFinBudget;
        [ObservableProperty]
        public string statutBudget;
        [ObservableProperty]
        public int nbreLigneBudgetaire;

    }
}
