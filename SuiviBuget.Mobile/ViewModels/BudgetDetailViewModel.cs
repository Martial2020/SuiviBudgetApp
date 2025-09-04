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
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Interfaces;
using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.Services;
using static SQLite.SQLite3;
using static SuiviBuget.Mobile.Messages.Messages;

namespace SuiviBuget.Mobile.ViewModels
{
    public partial class BudgetDetailViewModel : ObservableObject
    {
        #region Propriete
        [ObservableProperty]
        private BudgetDetailModel dataItem = new();

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
        private string title = "Ajouter une ligne budgetaire";

        [ObservableProperty]
        private string labelButton = "Ajouter";

        private string _action;
        public string Action
        {
            get => _action;
            set
            {
                if (_action != value)
                {
                    _action = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _CodeLigneBudgetaireIsEnabled = true;
        public bool CodeLigneBudgetaireIsEnabled
        {
            get => _CodeLigneBudgetaireIsEnabled;
            set
            {
                if (_CodeLigneBudgetaireIsEnabled != value)
                {
                    _CodeLigneBudgetaireIsEnabled = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Interfaces
        IService adminService { get; set; }
        public ICommand SubmitCommand { get; }
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;

        #endregion
        public BudgetDetailViewModel()
        {
            string dbPath = Helper.GetDatabaseFullPath();
            adminService = new Services.Services(dbPath);
            _navigationService = new NavigationService();
            _navigationService = new NavigationService();
            _alertService = new AlertService();
            _ = LoadLigneBudgetaireAsync("");
            SubmitCommand = new RelayCommand(OnSubmitCommand);
        }

        private async Task LoadLigneBudgetaireAsync(string searchText)
        {
            var ligneItems = await adminService.GetLigneBudgetaireItems(searchText);

            LigneBudgetaireItems = new ObservableCollection<LigneBudgetaireManageModel>(
                ligneItems.Select(x => new LigneBudgetaireManageModel
                {
                    CodeLigneBudgetaire = x.CodeLigneBudgetaire,
                    LibelleLigneBudgetaire = $"[{x.CodeLigneBudgetaire}] - {x.LibelleLigneBudgetaire}"
                }));
        }
        private async Task<LigneBudgetaireManageModel> GetLigneBudgetaireByCodeAsync(string codeLine)
        {
            var ligne = await adminService.GetLigneBudgetaireByCode(codeLine);
            if (ligne == null) return null;
            return new LigneBudgetaireManageModel
            {
                CodeLigneBudgetaire = ligne.CodeLigneBudgetaire,
                LibelleLigneBudgetaire = ligne.LibelleLigneBudgetaire
            };
        }

        private async void OnSubmitCommand()
        {
            switch (Action)
            {
                case GlobalConst.Add:
                    BudgetDetailCreate();
                    break;
                case GlobalConst.Edit:
                    BudgetDetailUpdate();
                    break;
                default:
                    await _alertService.ShowAlertAsync("Erreur", "Aucune action.");
                    break;
            }
        }

        public async Task InitializePageAsync(string code, string action)
        {
            Action = action;
            switch (Action)
            {
                case GlobalConst.Add:
                    DataItem.CodeBudget = code;
                    break;
                case GlobalConst.Edit:
                    Title = "Modifier un detail";
                    LabelButton = "Modifier";
                    DataItem.BudgetDetailID = Guid.Parse(code);
                    var ligne = await adminService.GetBudgetDetailByCode(DataItem.BudgetDetailID);
                    if (ligne == null) return;
                    DataItem.CodeLigneBudgetaire = ligne.CodeLigneBudgetaire;
                    DataItem.Montant = ligne.Montant;
                    DataItem.CodeBudget = ligne.CodeBudget;
                    CodeLigneBudgetaireIsEnabled = false;
                    SelectedLigneBudgetaire = LigneBudgetaireItems.FirstOrDefault(l => l.CodeLigneBudgetaire == DataItem.CodeLigneBudgetaire);

                    break;
                default:
                    break;
            }


        }

        private async void BudgetDetailCreate()
        {
            try
            {
                var isValid = await Validator.ValidateBudgetDetailCreate(DataItem);
                if (!isValid.isSuccess)
                {
                    await _alertService.ShowAlertAsync("Erreur", isValid.message);
                    return;
                }


                var isOk = await adminService.AddBudgetDetailAsync(DataItem);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de l'enregistrement"); return;
                }

                await _alertService.ShowAlertAsync("Information", $"Ligne budgetaire [{DataItem.CodeLigneBudgetaire}] a été Ajoutée au budget [{DataItem.CodeBudget}] avec succès");

                WeakReferenceMessenger.Default.Send(new RefreshList());
                await _navigationService.GoBackAsync();
            }
            catch (Exception ex)
            {
                await _alertService.ShowAlertAsync("Erreur", ex.Message); return;
            }

        }

        private async void BudgetDetailUpdate()
        {
            try
            {
                var isValid = await Validator.ValidateBudgetDetailUpdate(DataItem);
                if (!isValid.isSuccess)
                {
                    await _alertService.ShowAlertAsync("Erreur", isValid.message);
                    return;
                }

                var isOk = await adminService.UpdateBudgetDetailAsync(DataItem);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de l'enregistrement"); return;
                }

                await _alertService.ShowAlertAsync("Information", $"Ligne budgetaire [{DataItem.CodeLigneBudgetaire}] a été modifiée  avec succès");
                WeakReferenceMessenger.Default.Send(new RefreshList());
                await _navigationService.GoBackAsync();
            }
            catch (Exception ex)
            {
                await _alertService.ShowAlertAsync("Erreur", ex.Message); return;
            }
        }
    }
}

