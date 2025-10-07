using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SuiviBudge.Validators;
using SuiviBudget.Mobile.Constants;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.DataAccess;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Interfaces;
using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.Services;
using static SuiviBuget.Mobile.Messages.Messages;

namespace SuiviBuget.Mobile.ViewModels
{
    public partial class ReajusterViewModel : ObservableObject
    {
        [ObservableProperty]
        private ReajusterModel dataItem = new();
        [ObservableProperty]
        private ObservableCollection<LigneBudgetaireManageModel> ligneBudgetaireItems;
        private LigneBudgetaireManageModel _selectedLigneBudgetaire;
        public LigneBudgetaireManageModel SelectedLigneBudgetaire
        {
            get => _selectedLigneBudgetaire;
            set
            {
                _selectedLigneBudgetaire = value;
                OnPropertyChanged();
                // mettre à jour le code dans DataItem
                DataItem.CodeLigneBudgetaire = value?.CodeLigneBudgetaire;
            }
        }

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string labelButton = "+ Ajouter";

        [ObservableProperty]
        private string action;
        
        IServices adminService { get; set; }
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;

        public ReajusterViewModel()
        {
            string dbPath = Helper.GetDatabaseFullPath();
            adminService = new Services.Services(dbPath);
            _navigationService = new NavigationService();
            _alertService = new AlertService();
        }

        [RelayCommand]
        private async void Submit()
        {
            switch (Action)
            {
                case GlobalConst.Add:
                    Create();
                    break;
                case GlobalConst.Edit:
                    Update();
                    break;
                default:
                    await _alertService.ShowAlertAsync("Erreur", "Aucune action.");
                    break;
            }
        }
        private async void Update()
        { }
        private async void Create()
        {
            try
            {
                var result = await Validator.ValidateReajustementCreate(DataItem);
                if (!result.isSuccess)
                {
                    await _alertService.ShowAlertAsync("Information", result.message);
                    return;
                }
                var model = new Reajustement
                {
                    CodeBudget = DataItem.CodeBudget,
                    CodeLigneBudgetaire = DataItem.CodeLigneBudgetaire,
                    DateReajustement = DataItem.DateExecution,
                    Montant = DataItem.Montant,
                    Motif = DataItem.Description,
                    ReajustementID = DataItem.ReajusterID
                };

                var isOk = await adminService.AddReajustementAsync(model);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de l'enregistrement");
                    return;
                }

                await _alertService.ShowAlertAsync("Information", $"Réajustement a été effectué avec succès");
                WeakReferenceMessenger.Default.Send(new RefreshList());
                await _navigationService.GoBackAsync();
            }
            catch (Exception ex)
            {
                await _alertService.ShowAlertAsync("Erreur", ex.Message);
                return;
            }


        }
        private async Task LoadLigneBudgetaireAsync(string codeBudget, string searchText = "")
        {
            var ligneItems = await adminService.GetBudgetDetailItems(DataItem.CodeBudget, searchText);

           LigneBudgetaireItems = new ObservableCollection<LigneBudgetaireManageModel>(
                ligneItems.Select(x => new LigneBudgetaireManageModel
                {
                    CodeLigneBudgetaire = x.CodeLigneBudgetaire,
                    LibelleLigneBudgetaire = $"[{x.CodeLigneBudgetaire}] - {x.LibelleLigneBudgetaire}"
                })
            );
            SelectedLigneBudgetaire = LigneBudgetaireItems.FirstOrDefault(l => l.CodeLigneBudgetaire == DataItem.CodeLigneBudgetaire);
        }

        public async Task InitializePageAsync(string code, string action)
        {
            Action = action;
            DataItem.CodeBudget = code;

            switch (Action)
            {
                case GlobalConst.Add:
                    Title = $"Ajouter un réajustement";
                    DataItem.ReajusterID = Guid.NewGuid();
                    DataItem.DateExecution = DateTime.Now;
                    _ = LoadLigneBudgetaireAsync(DataItem.CodeBudget, "");
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

    }
}
