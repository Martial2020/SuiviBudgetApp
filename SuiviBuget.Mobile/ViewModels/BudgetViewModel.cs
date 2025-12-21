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
    public partial class BudgetViewModel : ObservableObject
    {
        #region Propriete
        [ObservableProperty]
        private BudgetModel dataItem = new();

        [ObservableProperty]
        private string title = "Ajouter un budget";

        [ObservableProperty]
        private string labelButton = "+ Ajouter";

        [ObservableProperty]
        private bool revenuVisible = false;

        [ObservableProperty]
        private bool descriptionVisible = false;

        [ObservableProperty]
        private bool isEnabled = true;

        [ObservableProperty]
        private bool isReadOnly = false;

        [ObservableProperty]
        private string revenuTotal;


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


        public bool IsInterne
        {
            get => DataItem?.SourceBudget == "Interne";
            set
            {
                if (value)
                {
                    _ = GetMontantRevenu();
                    DescriptionVisible = false;
                    DataItem.SourceBudget = "Interne";
                    OnPropertyChanged(nameof(IsInterne));
                    OnPropertyChanged(nameof(IsExterne));
                }
            }
        }

        public bool IsExterne
        {
            get => DataItem?.SourceBudget == "Externe";
            set
            {
                if (value)
                {
                    RevenuVisible = false;
                    DescriptionVisible = true;
                    DataItem.SourceBudget = "Externe";
                    OnPropertyChanged(nameof(IsInterne));
                    OnPropertyChanged(nameof(IsExterne));
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

        partial void OnDataItemChanged(BudgetModel value)
        {
            OnPropertyChanged(nameof(IsInterne));
            OnPropertyChanged(nameof(IsExterne));
            DescriptionVisible = IsExterne == true;
            RevenuVisible = IsInterne == true && Action != GlobalConst.Edit;
        }

        private async Task GetMontantRevenu()
        {
            var devise = await Helper.GetDeviseActiveAsyn();
            var revenus = await adminService.GetRevenuItems(string.Empty);
            DataItem.TotalRevenu = revenus.Sum(r => r.Montant);
            RevenuTotal = $"Revenu:{(DataItem.TotalRevenu):N0} {devise}";
            RevenuVisible = Action != GlobalConst.Edit;
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
                    LabelButton = "✎ Modifier";
                    IsEnabled = false;
                    IsReadOnly = true;
                    var budget = await adminService.GetBudgetByCode(code);
                    if (budget == null)
                        return;

                    DataItem = new BudgetModel
                    {
                        CodeBudget = budget.CodeBudget,
                        LibelleBudget = budget.LibelleBudget,
                        DateDebutBudget = budget.DateDebutBudget,
                        DateFinBudget = budget.DateFinBudget,
                        StatutBudget = budget.StatutBudget,
                        MontantBudget = budget.MontantBudget,
                        Description = budget.Description,
                        SourceBudget = budget.SourceBudget,
                    };
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
                    Description = IsExterne ? DataItem.Description : "",
                    MontantBudget = DataItem.MontantBudget,
                    NbreLigneBudgetaire = 0,
                    StatutBudget = StatutBudgetConst.Ouvert,
                    SourceBudget = DataItem.SourceBudget

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
                    Description = IsExterne ? DataItem.Description : "",
                    MontantBudget = DataItem.MontantBudget,
                    NbreLigneBudgetaire = DataItem.NbreLigneBudgetaire,
                    StatutBudget = DataItem.StatutBudget,
                    SourceBudget = DataItem.SourceBudget
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

