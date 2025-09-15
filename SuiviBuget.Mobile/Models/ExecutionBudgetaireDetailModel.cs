using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SuiviBuget.Mobile.Models
{
    public partial class ExecutionBudgetaireDetailModel : ObservableObject
    {
        [ObservableProperty]
        public Guid executionBudgetaireID;
        [ObservableProperty]
        public string codeBudget;
        [ObservableProperty]
        public string codeLigneBudgetaire;
        [ObservableProperty]
        public string codeModePaiement;
        [ObservableProperty]
        public decimal montant;
        [ObservableProperty]
        public string description;
        [ObservableProperty]
        public DateTime dateExecution;
    }
}
