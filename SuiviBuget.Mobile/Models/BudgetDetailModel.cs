using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace SuiviBuget.Mobile.Models
{
    public partial class BudgetDetailModel : ObservableObject
    {
        [ObservableProperty]
        private Guid budgetDetailID;
        [ObservableProperty]
        private string codeBudget;
        [ObservableProperty]
        private string codeLigneBudgetaire;
        [ObservableProperty]
        private decimal montant;
    }
}
