using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuiviBudget.Mobile.Interfaces;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SuiviBudget.Mobile.Constants;
using SuiviBuget.Mobile.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using SuiviBuget.Mobile.Helpers;
using SuiviBudge.Validators;
using CommunityToolkit.Mvvm.Messaging;
using static SuiviBuget.Mobile.Messages.Messages;
using SuiviBuget.Mobile.Interfaces;
using SuiviBuget.Mobile.Services;
using static SQLite.SQLite3;

namespace SuiviBuget.Mobile.ViewModels
{
    public partial class BudgetViewModel : ObservableObject
    {
        #region Propriete
        [ObservableProperty]
        private BudgetModel dataItem = new();

        [ObservableProperty]
        private string title = "Ajouter un budget";

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

        #endregion

        #region Interfaces
        IService adminService { get; set; }
        public ICommand SubmitCommand { get; }
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;

        #endregion
        public BudgetViewModel()
        {
            string dbPath = Helper.GetDatabaseFullPath();
            adminService = new Services.Services(dbPath);
            _navigationService = new NavigationService();
            _navigationService = new NavigationService();
            _alertService = new AlertService();
            SubmitCommand = new RelayCommand(OnSubmitCommand);

        }

        private async void OnSubmitCommand()
        {
            switch (Action)
            {
                case GlobalConst.Add:
                    CreateBudget();
                    break;
                case GlobalConst.Edit:
                    UpdateBudget();
                    break;
                default:
                    await _alertService.ShowAlertAsync("Erreur", "Aucune action définie.");
                    break;
            }
        }

        public async Task InitializePageAsync(string code, string action)
        {
            Action = action;
            switch (Action)
            {
                case GlobalConst.Add:
                    DataItem.CodeBudget = await adminService.GetNumeroForCodeEntityAsync(ParametreCompteurConst.BG);
                    DataItem.DateDebutBudget = DateTime.Today;
                    DataItem.DateFinBudget = DateTime.Today;
                    break;
                case GlobalConst.Edit:
                    Title = "Modifier un budget";
                    LabelButton = "Modifier";
                    var budget = await adminService.GetBudgetByCode(code);
                    if (budget == null) { return; }
                    DataItem.CodeBudget = budget.CodeBudget;
                    DataItem.LibelleBudget = budget.LibelleBudget;
                    DataItem.DateDebutBudget = budget.DateDebutBudget;
                    DataItem.DateFinBudget = budget.DateFinBudget;
                    DataItem.StatutBudget = budget.StatutBudget;
                    break;
                default:
                    break;
            }


        }

        private async void CreateBudget()
        {
            try
            {
                var result = await Validator.ValidateBudgetCreateAsync(DataItem);
                if (!result.isSuccess)
                {
                    await _alertService.ShowAlertAsync("Erreur", result.message);
                    return;
                }
                var dataEntity = new BudgetModel
                {
                    CodeBudget = DataItem.CodeBudget,
                    LibelleBudget = DataItem.LibelleBudget,
                    DateDebutBudget = DataItem.DateDebutBudget,
                    DateFinBudget = DataItem.DateFinBudget,
                    DateCreationBudget = DateTime.Now,
                    //DescriptionBudget = "",
                    MontantBudget = 0,
                    NbreLigneBudgetaire = 0,
                    StatutBudget = StatutBudgetConst.Ouvert
                };
                var isOk = await adminService.AddBudgetAsync(dataEntity);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de l'enregistrement");
                    return;
                }

                await _alertService.ShowAlertAsync("Information", $"Le budget[{dataEntity.LibelleBudget}] a été créé avec succès");
                WeakReferenceMessenger.Default.Send(new RefreshList());
                await _navigationService.GoBackAsync();
            }
            catch (Exception ex)
            {
                await _alertService.ShowAlertAsync("Erreur", ex.Message);
                return;
            }


        }

     
        private async void UpdateBudget()
        {
            try
            {
                var result = Validator.ValidateBudgetUpdate(DataItem);
                if (!result.isSuccess)
                {
                    await _alertService.ShowAlertAsync("Erreur", result.message);
                    return;
                }
                var dataEntity = new BudgetModel
                {
                    CodeBudget = DataItem.CodeBudget,
                    LibelleBudget = DataItem.LibelleBudget,
                    DateDebutBudget = DataItem.DateDebutBudget,
                    DateFinBudget = DataItem.DateFinBudget,
                    DateCreationBudget = DataItem.DateCreationBudget,
                    //DescriptionBudget = "",
                    MontantBudget = DataItem.MontantBudget,
                    NbreLigneBudgetaire = DataItem.NbreLigneBudgetaire,
                    StatutBudget = DataItem.statutBudget
                };
                var isOk = await adminService.UpdateBudgetAsync(dataEntity);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de la modification");
                    return;
                }

                await _alertService.ShowAlertAsync("Information", $"Le budget [{dataEntity.CodeBudget}] a été modifiée avec succs");
                WeakReferenceMessenger.Default.Send(new RefreshList());
                await _navigationService.GoBackAsync();
            }
            catch (Exception ex)
            {
                await _alertService.ShowAlertAsync("Erreur", ex.Message);
                return;
            }

        }
    }
}

