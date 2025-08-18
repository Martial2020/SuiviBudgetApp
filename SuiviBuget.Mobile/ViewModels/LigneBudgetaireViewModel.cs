using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuiviBudget.Core.Interfaces;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SuiviBudget.Core.Constants;
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
    public partial class LigneBudgetaireViewModel : ObservableObject
    {
        #region Propriete
        [ObservableProperty]
        private LigneBudgetaireModel dataItem = new();

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
        public ICommand SubmitLigneBugetaireCommand { get; }
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;

        #endregion
        public LigneBudgetaireViewModel()
        {
            string dbPath = Helper.GetDatabaseFullPath();
            adminService = new Services.Services(dbPath);
            _navigationService = new NavigationService();
            _navigationService = new NavigationService();
            _alertService = new AlertService();
            SubmitLigneBugetaireCommand = new RelayCommand(OnSubmitLigneBugetaireCommand);
        }

        private async void OnSubmitLigneBugetaireCommand()
        {
            switch (Action)
            {
                case GlobalConst.Add:
                    CreateLigneBudgetaire();
                    break;
                case GlobalConst.Edit:
                    UpdateLigneBudgetaire();
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
                case GlobalConst.Edit:
                    Title = "Modifier une ligne budgetaire";
                    LabelButton = "Modifier";
                    var ligne = await adminService.GetLigneBudgetaireByCode(code);
                    if (ligne == null) { return; }
                    DataItem.CodeLigneBudgetaire = ligne.CodeLigneBudgetaire;
                    DataItem.LibelleLigneBudgetaire = ligne.LibelleLigneBudgetaire;
                    CodeLigneBudgetaireIsEnabled = false;
                    break;
                default:
                    break;
            }

            // Le titre de la page
            if (!string.IsNullOrEmpty(code))
            {

            }
        }

        private async void CreateLigneBudgetaire()
        {
            var dataEntity = new LigneBudgetaireModel
            {
                CodeLigneBudgetaire = dataItem.CodeLigneBudgetaire,
                LibelleLigneBudgetaire = dataItem.LibelleLigneBudgetaire
            };
            try
            {
                var result = Validator.ValidateLigneBugetaireCreate(dataEntity);
                if (!result.isSuccess)
                {
                    await _alertService.ShowAlertAsync("Erreur", result.message);
                    return;
                }
                var isOk = await adminService.AddLigneBudgetaireAsync(dataEntity);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de l'enregistrement");
                    return;
                }

                await _alertService.ShowAlertAsync("Information", $"Ligne budgetaire [{dataEntity.LibelleLigneBudgetaire}] a été créée avec succès");
                WeakReferenceMessenger.Default.Send(new RefreshList());
                await _navigationService.GoBackAsync();
            }
            catch (Exception ex)
            {
                await _alertService.ShowAlertAsync("Erreur", ex.Message);
                return;
            }


        }

        private async void UpdateLigneBudgetaire()
        {
            var dataEntity = new LigneBudgetaireModel
            {
                CodeLigneBudgetaire = dataItem.CodeLigneBudgetaire,
                LibelleLigneBudgetaire = dataItem.LibelleLigneBudgetaire
            };
            try
            {
                var result = Validator.ValidateLigneBugetaireUpdate(dataEntity);
                if (!result.isSuccess)
                {
                    await _alertService.ShowAlertAsync("Erreur", result.message);
                    return;
                }
                var isOk = await adminService.UpdateLigneBudgetaireAsync(dataEntity);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de la modification");
                    return;
                }

                await _alertService.ShowAlertAsync("Information", $"Ligne budgetaire [{dataEntity.CodeLigneBudgetaire}] a été modifiée avec succès");
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

