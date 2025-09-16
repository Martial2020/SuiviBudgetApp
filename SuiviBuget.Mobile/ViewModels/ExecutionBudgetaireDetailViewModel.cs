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
    public partial class ExecutionBudgetaireDetailViewModel : ObservableObject
    {
        #region Propriete
        [ObservableProperty]
        private ExecutionBudgetaireDetailModel dataItem = new();

        [ObservableProperty]
        private ObservableCollection<LigneBudgetaireManageModel> ligneBudgetaireItems;

        [ObservableProperty]
        private ObservableCollection<ModePaiementManageModel> modePaiementItems;


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


        private ModePaiementManageModel _selectedModePaiement;
        public ModePaiementManageModel SelectedModePaiement
        {
            get => _selectedModePaiement;
            set
            {
                _selectedModePaiement = value;
                OnPropertyChanged();
                // mettre à jour le code dans DataItem
                DataItem.CodeModePaiement = value?.CodeModePaiement;
            }
        }

        [ObservableProperty]
        private string title;

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
        IServices adminService { get; set; }
        public ICommand SubmitCommand { get; }
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;

        #endregion
        public ExecutionBudgetaireDetailViewModel()
        {
            string dbPath = Helper.GetDatabaseFullPath();
            adminService = new Services.Services(dbPath);
            _navigationService = new NavigationService();
            _alertService = new AlertService();
            SubmitCommand = new RelayCommand(OnSubmitCommand);
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
        }

        private async Task LoadModePaiementAsync(string searchText)
        {
            var ligneItems = await adminService.GetModePaiementItems(searchText);

            ModePaiementItems = new ObservableCollection<ModePaiementManageModel>(
                ligneItems.Select(x => new ModePaiementManageModel
                {
                    CodeModePaiement = x.CodeModePaiement,
                    LibelleModePaiement = $"[{x.CodeModePaiement}] - {x.LibelleModePaiement}"
                })
            );
        }

        private async void OnSubmitCommand()
        {
            switch (Action)
            {
                case GlobalConst.Add:
                    Create();
                    break;
                case GlobalConst.Edit:
                    //BudgetDetailUpdate();
                    break;
                default:
                    await _alertService.ShowAlertAsync("Erreur", "Aucune action.");
                    break;
            }
        }

        public async Task InitializePageAsync(string code, string action)
        {
            Action = action;

            DataItem.CodeBudget = code;
            switch (Action)
            {
                case GlobalConst.Add:
                    Title = $"Exécution du budget {DataItem.CodeBudget}";
                    DataItem.ExecutionBudgetaireID = Guid.NewGuid();
                    DataItem.DateExecution = DateTime.Now;
                    _ = LoadLigneBudgetaireAsync(DataItem.CodeBudget, "");
                    _ = LoadModePaiementAsync("");
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

        private async void Create()
        {
            try
            {

                var isValid = await Validator.ValidateExecutionBudgetaireDetailCreateAsync(DataItem);
                if (!isValid.isSuccess)
                {
                    await _alertService.ShowAlertAsync("Erreur", isValid.message);
                    return;
                }

                var data = new ExecutionBudgetaire
                {
                    DateExecution = DataItem.DateExecution,
                    CodeBudget = DataItem.CodeBudget,
                    CodeLigneBudgetaire = DataItem.CodeLigneBudgetaire,
                    CodeModePaiement = DataItem.CodeModePaiement,
                    Montant = DataItem.Montant,
                    Description = DataItem.Description,
                    ExecutionBudgetaireID = DataItem.ExecutionBudgetaireID
                };
                var isOk = await adminService.AddExecutionBudgetaireDetailAsync(data);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de l'enregistrement");
                    return;
                }

                await _alertService.ShowAlertAsync("Information", $"Execution effectuée sur la ligne budgetaire [{DataItem.CodeLigneBudgetaire}] avec succès");
                WeakReferenceMessenger.Default.Send(new RefreshList());
                await _navigationService.GoBackAsync();
            }
            catch (Exception ex)
            {
                await _alertService.ShowAlertAsync("Erreur", ex.Message); return;
            }

        }

        //private async void BudgetDetailUpdate()
        //{
        //    try
        //    {
        //        var isValid = await Validator.ValidateBudgetDetailUpdate(DataItem);
        //        if (!isValid.isSuccess)
        //        {
        //            await _alertService.ShowAlertAsync("Erreur", isValid.message);
        //            return;
        //        }

        //        var isOk = await adminService.UpdateBudgetDetailAsync(DataItem);
        //        if (!isOk)
        //        {
        //            await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de l'enregistrement"); return;
        //        }

        //        await _alertService.ShowAlertAsync("Information", $"Ligne budgetaire [{DataItem.CodeLigneBudgetaire}] a été modifiée  avec succès");
        //        WeakReferenceMessenger.Default.Send(new RefreshList());
        //        await _navigationService.GoBackAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        await _alertService.ShowAlertAsync("Erreur", ex.Message); return;
        //    }
        //}
    }
}
