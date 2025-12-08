using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SuiviBudget.Mobile.Constants;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Models;

namespace SuiviBuget.Mobile.ViewModels
{
    public partial class RevenuDetailViewModel : ObservableObject
    {
        [ObservableProperty]
        private RevenuDetailModel dataItem = new();

        [ObservableProperty]
        private string actionAFaire;

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string labelButton;


        [ObservableProperty]
        private ObservableCollection<ModePaiementManageModel> modePaiementItems;

        IServices service { get; set; }
        public RevenuDetailViewModel()
        {
            string dbPath = Helper.GetDatabaseFullPath();
            service = new Services.Services(dbPath);
        }
        public async Task InitializePageAsync(string code, string action)
        {
            ActionAFaire = action;
            switch (ActionAFaire)
            {
                case GlobalConst.Add:
                   Title = $"Ajouter un revenu";
                    LabelButton = "Ajouter";  
                    await LoadModePaiementAsync("");
                    break;
                case GlobalConst.Edit:
                    //Title = "Modifier un detail";
                    //LabelButton = "Modifier";                
                    //var ligne = await adminService.getex
                    //if (ligne == null) return;
                    //DataItem.CodeLigneBudgetaire = ligne.CodeLigneBudgetaire;
                    //DataItem.Montant = ligne.Montant;
                    //DataItem.CodeBudget = ligne.CodeBudget;
                    //CodeLigneBudgetaireIsEnabled = false;
                    //SelectedLigneBudgetaire = LigneBudgetaireItems.FirstOrDefault(l => l.CodeLigneBudgetaire == DataItem.CodeLigneBudgetaire);

                    break;
                default:
                    break;
            }


        }
        private async Task LoadModePaiementAsync(string searchText)
        {
            var ligneItems = await service.GetModePaiementItems(searchText);

            ModePaiementItems = new ObservableCollection<ModePaiementManageModel>(
                ligneItems.Select(x => new ModePaiementManageModel
                {
                    CodeModePaiement = x.CodeModePaiement,
                    LibelleModePaiement = $"[{x.CodeModePaiement}] - {x.LibelleModePaiement}"
                })
            );
        }
    }
}
