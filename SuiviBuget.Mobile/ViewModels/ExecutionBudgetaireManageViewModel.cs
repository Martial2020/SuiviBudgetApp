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
using Intuit.Ipp.Data;
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
                     LoadBudgetAsync(SelectedFilter,_searchText); // Charge la liste initialement
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
        #region Background Color Statut

        [ObservableProperty]
        public Color tousBackground = Colors.White;

        [ObservableProperty]
        public Color ouvertBackground = Colors.White;
        [ObservableProperty]
        public Color enCoursBackground = Colors.White;
        [ObservableProperty]
        public Color clotureBackground = Colors.White;


        [ObservableProperty]
        public string tousLabel = "Tous";

        [ObservableProperty]
        public string ouvertLabel = "Ouvert";

        [ObservableProperty]
        public string enCoursLabel = "En cours";
        [ObservableProperty]
        public string clotureLabel = "Clôturé";

        private string _selectedFilter;
        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                //Color.FromArgb("#FF7F50");
                if (_selectedFilter != value)
                {
                    _selectedFilter = value;
                    OnPropertyChanged(nameof(SelectedFilter));
                    TousBackground = Colors.White;
                    OuvertBackground = Colors.White;
                    EnCoursBackground = Colors.White;
                    ClotureBackground = Colors.White;

                    if (_selectedFilter == StatutBudgetConst.Encours)
                        EnCoursBackground = Colors.AliceBlue;
                    if (_selectedFilter == StatutBudgetConst.Cloture)
                        ClotureBackground = Colors.AliceBlue;
                    if (_selectedFilter == StatutBudgetConst.Ouvert)
                        OuvertBackground = Colors.AliceBlue;
                    if (_selectedFilter == StatutBudgetConst.Tous)
                        TousBackground = Colors.AliceBlue;

                    LoadBudgetAsync(_selectedFilter, SearchText);

                }
            }
        }

        #endregion
        #region Interfaces
        private readonly IServices service;
        public ICommand FilterStatutCommand { get; }

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
            SelectedFilter = StatutBudgetConst.Tous;

            RegisterMessenger();
            ResetAppMessage();
            // Charger au démarrage
            FilterStatutCommand = new RelayCommand<string>(OnFilterStatutCommand);

        }

        private async void LoadBudgetAsync(string statut, string searchText)
        {

            List<string> statuts = new List<string>();
            BudgetItems = new ObservableCollection<BudgetManageModel>();
            IsBusy = true;

            if (string.IsNullOrEmpty(statut) || statut == StatutBudgetConst.Tous)
                statuts = new List<string> { StatutBudgetConst.Encours, StatutBudgetConst.Cloture };
            else
                statuts.Add(statut);

            var budgets = await service.GetBudgetItemsByStatus(searchText, statuts);
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
                    MontantRestant = x.MontantRestant,
                    MontantAlloue=x.MontantAlloue,
                    MontantNonAlloue=x.MontantNonAlloue,
                    MontantReajustement=x.MontantReajustement,
                    BackgroundColorStatut = Helper.GetBackgroundColor(x.StatutBudget)
                }));
            ActualiserNombreBudget();
            IsBusy = false;
        }
        private async void ActualiserNombreBudget()
        {
            var statuts = new List<string> { StatutBudgetConst.Encours, StatutBudgetConst.Cloture };
            var budgets = await service.GetBudgetItemsByStatus(SearchText, statuts);

            TousLabel = "Tous"; OuvertLabel = "Ouvert"; EnCoursLabel = "En cours"; ClotureLabel = "Clôturé";

            if (budgets.Count > 0)
                TousLabel = $"{TousLabel.Trim()} {budgets.Count()}";

            if (budgets.Where(e => e.StatutBudget == StatutBudgetConst.Ouvert).Count() > 0)
                OuvertLabel = $"{OuvertLabel.Trim()} {budgets.Where(e => e.StatutBudget == StatutBudgetConst.Ouvert).Count()}";

            if (budgets.Where(e => e.StatutBudget == StatutBudgetConst.Encours).Count() > 0)
                EnCoursLabel = $"{EnCoursLabel.Trim()} {budgets.Where(e => e.StatutBudget == StatutBudgetConst.Encours).Count()}";

            if (budgets.Where(e => e.StatutBudget == StatutBudgetConst.Cloture).Count() > 0)
                ClotureLabel = $"{ClotureLabel.Trim()} {budgets.Where(e => e.StatutBudget == StatutBudgetConst.Cloture).Count()}";
        }

        private void OnFilterStatutCommand(string statut) => SelectedFilter = statut;

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
        //public async void LoadBudgetAsync(string searchText)
        //{
        //    BudgetItems = new ObservableCollection<BudgetManageModel>();
        //    IsBusy = true;
        //    //await Task.Delay(1000); // ⏳ attend 1,5 secondes (1500 ms)
        //    var statutList = new List<string> { StatutBudgetConst.Encours, StatutBudgetConst.Cloture };
        //    var budgets = await service.GetBudgetItemsByStatus(searchText, statutList);
        //    BudgetItems = new ObservableCollection<BudgetManageModel>(
        //        budgets.Select(x => new BudgetManageModel
        //        {
        //            CodeBudget = x.CodeBudget,
        //            DateCreationBudget = x.DateCreationBudget,
        //            DateDebutBudget = x.DateDebutBudget,
        //            DateFinBudget = x.DateFinBudget,
        //            DescriptionBudget = x.DescriptionBudget,
        //            LibelleBudget = x.LibelleBudget,
        //            MontantBudget = x.MontantBudget,
        //            NbreLigneBudgetaire = x.NbreLigneBudgetaire,
        //            StatutBudget = x.StatutBudget,
        //            MontantUtilise = x.MontantUtilise,
        //            MontantRestant = x.MontantRestant,
        //            BackgroundColorStatut = Helper.GetBackgroundColor(x.StatutBudget)
        //        }));
        //    IsBusy = false;
        //}

        private void RegisterMessenger()
        {
            WeakReferenceMessenger.Default.Register<RefreshList>(this, async (r, m) =>
            {
                 LoadBudgetAsync(SelectedFilter,SearchText); // Rafraîchit la liste si un ajout est effectué
            });
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}


