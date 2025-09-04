using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using SuiviBudget.Mobile.Constants;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Interfaces;
using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.Services;
using static SuiviBuget.Mobile.Messages.Messages;

namespace SuiviBuget.Mobile.ViewModels
{
    public partial class BudgetManageViewModel : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<BudgetManageModel> budgetItems;

        [ObservableProperty]
        private BudgetManageModel selectedBudget;

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    _ = LoadBudgetAsync(_searchText); // Charge la liste initialement
                }
            }
        }
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }
        #region Interfaces
        IService service { get; set; }
        public ICommand SubmitLigneBugetaireCommand { get; }
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;

        #endregion

        public ICommand AddBugetCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand BudgetDetailCommand { get; }
        public ICommand CloturerCommand { get; }
        public ICommand EncoursCommand { get; }

        public BudgetManageViewModel()
        {
            var dbPath = Helper.GetDatabaseFullPath();
            service = new Services.Services(dbPath);
            _alertService = new AlertService();
            RegisterMessenger(); // Enregistre l'écoute du message
            _ = LoadBudgetAsync(SearchText); // Charge la liste initialement
            _navigationService = new NavigationService();
            AddBugetCommand = new RelayCommand(OnAddBugetCommand);
            EditCommand = new RelayCommand<BudgetManageModel>(OnEdit);
            DeleteCommand = new RelayCommand<BudgetManageModel>(OnDelete);
            BudgetDetailCommand = new RelayCommand<BudgetManageModel>(OnBudgetDetailCommand);
            CloturerCommand = new RelayCommand<BudgetManageModel>(OnCloturerCommand);
            EncoursCommand = new RelayCommand<BudgetManageModel>(OnEncoursCommand);
        }
        private async void OnEncoursCommand(BudgetManageModel budget)
        {
            var confirm = await Shell.Current.CurrentPage.DisplayAlert("Confirmation", $"Changer le statut du budget [{budget.CodeBudget}] ?", "Oui", "Non");
            if (confirm)
            {
                var getBudget =await service.GetBudgetByCode(budget.CodeBudget);
                if (getBudget == null)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Le budget selectionné est indisponible.");
                    return;
                }

                if (getBudget.statutBudget == StatutBudgetConst.Encours|| getBudget.statutBudget == StatutBudgetConst.Cloture)
                {
                    await _alertService.ShowAlertAsync("Information", "Le budget selectionné est deja en cours ou cloturé.");
                    return;
                }
                getBudget.statutBudget = StatutBudgetConst.Encours;
                var isOk = await service.UpdateBudgetAsync(getBudget);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors du changement de statut");
                    return;
                }
                await _alertService.ShowAlertAsync("Information", $"Le budget [{getBudget.LibelleBudget}] a été mis en cours avec succès");
                WeakReferenceMessenger.Default.Send(new RefreshList());
            }
        }
        private async void OnCloturerCommand(BudgetManageModel budget)
        {
            var confirm = await Shell.Current.CurrentPage.DisplayAlert("Confirmation", $"Changer le statut du budget [{budget.CodeBudget}] ?", "Oui", "Non");
            if (confirm)
            {
                var getBudget = await service.GetBudgetByCode(budget.CodeBudget);
                if (getBudget == null)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Le budget selectionné est indisponible.");
                    return;
                }

                if (getBudget.statutBudget == StatutBudgetConst.Cloture)
                {
                    await _alertService.ShowAlertAsync("Information", "Le budget selectionné est deja cloturé.");
                    return;
                }
                getBudget.statutBudget = StatutBudgetConst.Cloture;
                var isOk = await service.UpdateBudgetAsync(getBudget);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors du changement de statut");
                    return;
                }
                await _alertService.ShowAlertAsync("Information", $"Le budget [{getBudget.LibelleBudget}] a été cloturé avec succès");
                WeakReferenceMessenger.Default.Send(new RefreshList());
            }
        }
        private async void OnBudgetDetailCommand(BudgetManageModel budget)
        {
            if (budget == null)
            {
                await _alertService.ShowAlertAsync("Information", "Veuillez selectionner un budget.");
                return;
            }

            await _navigationService.NavigateToAsync("BudgetDetailManageView", budget.CodeBudget);
        }
        private async void OnAddBugetCommand()
        {
            await _navigationService.NavigateToAsync("BudgetView");
        }
        private async void OnEdit(BudgetManageModel budget)
        {
            if (string.IsNullOrEmpty(budget?.CodeBudget))
            {
                await _alertService.ShowAlertAsync("Erreur", "Veuillez selectionner un budget");
                return;
            }
            // Navigation vers la page d'édition avec l'item
            await _navigationService.NavigateToAsync("BudgetView", budget.CodeBudget, GlobalConst.Edit);
        }
        private async void OnDelete(BudgetManageModel budget)
        {
            var confirm = await Shell.Current.CurrentPage.DisplayAlert("Confirmation", $"Supprimer le budget [{budget.CodeBudget}] ?", "Oui", "Non");
            if (confirm)
            {
                var entity = new BudgetModel
                {
                    CodeBudget = budget.CodeBudget,
                    LibelleBudget = budget.LibelleBudget
                };
                var isOk = await service.DeleteBudgetAsync(entity);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de la suppression");
                    return;
                }
                await _alertService.ShowAlertAsync("Information", $"Le budget [{entity.LibelleBudget}] a été supprimé avec succès");
                WeakReferenceMessenger.Default.Send(new RefreshList());
            }
        }

        private void RegisterMessenger()
        {
            WeakReferenceMessenger.Default.Register<RefreshList>(this, async (r, m) =>
            {
                await LoadBudgetAsync(SearchText); // Rafraîchit la liste si un ajout est effectué
            });
        }

        private async Task LoadBudgetAsync(string searchText)
        {
            BudgetItems = null;
            IsBusy = true;
            //await Task.Delay(1000); // Simule un temps de chargement
            // Reset la sélection
            var budgets = await service.GetBudgetItems(searchText);
            BudgetItems = new ObservableCollection<BudgetManageModel>(
                budgets.Select(x => new BudgetManageModel
                {
                    CodeBudget = x.CodeBudget,
                    DateCreationBudget = x.DateCreationBudget,
                    DateDebutBudget = x.DateDebutBudget,
                    DateFinBudget = x.DateFinBudget,
                    DescriptionBudget = x.DescriptionBudget,
                    LibelleBudget = x.LibelleBudget,
                    MontantBudget = x.MontantBudget,
                    NbreLigneBudgetaire = x.NbreLigneBudgetaire,
                    StatutBudget = x.StatutBudget,
                    MontantUtilise=x.MontantUtilise
                }));
            IsBusy = false;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
