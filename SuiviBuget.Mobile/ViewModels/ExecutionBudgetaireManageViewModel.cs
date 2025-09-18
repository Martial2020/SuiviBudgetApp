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
using SuiviBudge.Validators;
using SuiviBudget.Mobile.Constants;
using SuiviBudget.Mobile.Interfaces;
using SuiviBudget.Services.DataAccess;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Interfaces;
using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.Services;
using static SuiviBuget.Mobile.Messages.Messages;

namespace SuiviBuget.Mobile.ViewModels
{
    public partial class ExecutionBudgetaireManageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<BudgetManageModel> budgetItems;

        private BudgetManageModel _selectedBudget;
        public BudgetManageModel SelectedBudget
        {
            get => _selectedBudget;
            set
            {
                _selectedBudget = value;
                OnPropertyChanged();
            }
        }

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
        private readonly IServices service;
        public ICommand ExecutionBudgetDetailCommand { get; }
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;
        #endregion

        public ExecutionBudgetaireManageViewModel()
        {
            string dbPath = Helper.GetDatabaseFullPath();
            service = new Services.Services(dbPath);
            _navigationService = new NavigationService();
            _alertService = new AlertService();
            ExecutionBudgetDetailCommand = new RelayCommand<BudgetManageModel>(OnExecutionBudgetDetailCommand);
            RegisterMessenger();
            ResetAppMessage();
            // Charger au démarrage
            Task.Run(() => LoadBudgetAsync(string.Empty));
        }

        private void ResetAppMessage()
        {
            WeakReferenceMessenger.Default.Register<ResetAppMessage>(this, (r, m) =>
            {
                BudgetItems.Clear();
            });
        }
        private async void OnExecutionBudgetDetailCommand(BudgetManageModel budget)
        {
            if (budget == null)
            {
                await _alertService.ShowAlertAsync("Erreur", "Veuillez selectionner un budget");
                return;
            }

            if (string.IsNullOrEmpty(budget.CodeBudget))
            {
                await _alertService.ShowAlertAsync("Erreur", "Le code du budget n'existe pas.");
                return;
            }

            // Navigation vers la page d'édition avec l'item
            await _navigationService.NavigateToAsync("ExecutionBudgetaireManageDetailView", budget.CodeBudget);

        }
        public async Task LoadBudgetAsync(string searchText)
        {
            BudgetItems = new ObservableCollection<BudgetManageModel>();
            IsBusy = true;
            await Task.Delay(1000); // ⏳ attend 1,5 secondes (1500 ms)
            var statutList = new List<string> { StatutBudgetConst.Encours, StatutBudgetConst.Cloture };     
            var budgets = await service.GetBudgetItemsByStatus(searchText, statutList);
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
                    MontantUtilise = x.MontantUtilise,
                    MontantRestant = x.MontantRestant
                }));
            IsBusy = false;
        }

        private void RegisterMessenger()
        {
            WeakReferenceMessenger.Default.Register<RefreshList>(this, async (r, m) =>
            {
                await LoadBudgetAsync(SearchText); // Rafraîchit la liste si un ajout est effectué
            });
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}


