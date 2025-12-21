using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Models;

namespace SuiviBuget.Mobile.ViewModels
{
    public partial class HistoriquePrelevementViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<HistoriquePrelevementModel> prelevementstems;

        [ObservableProperty]
        private bool isBusy;
        IServices service { get; set; }
        public HistoriquePrelevementViewModel()
        {
            string dbPath = Helper.GetDatabaseFullPath();
            service = new Services.Services(dbPath);
            _ = LoadPrelevementAsync();
        }

        private async Task LoadPrelevementAsync()
        {
            IsBusy = true;
            var prelevements = await service.GetPrelevementItems();
            var devise = await Helper.GetDeviseActiveAsyn();
            Prelevementstems = new ObservableCollection<HistoriquePrelevementModel>(
                 prelevements.Select(x => new HistoriquePrelevementModel
                 {
                     CodeBudget = x.CodeBudget,
                     Montant = x.Montant,
                     MontantAvecDevise = $"{x.Montant:N0} {devise}",
                     DateCreation = x.DateCreation,
                     EstAnnule = x.EstAnnule,
                     LibelleBudget = x.LibelleBudget
                 }));
            IsBusy = false;
        }
    }
}
