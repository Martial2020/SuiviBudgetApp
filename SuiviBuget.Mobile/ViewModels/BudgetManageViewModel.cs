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
using Microsoft.Maui.Controls;
using SuiviBudge.Validators;
using SuiviBudget.Mobile.Constants;
using SuiviBudget.Mobile.Interfaces;
using SuiviBudget.Services.DataAccess;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Interfaces;
using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.Services;
using static SuiviBuget.Mobile.Messages.Messages;
using Budget = SuiviBudget.Services.DataAccess.Budget;

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
                    LoadBudgetAsync(SelectedFilter, _searchText); // Charge la liste initialement
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
        IServices service { get; set; }
        public ICommand SubmitLigneBugetaireCommand { get; }
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;

        #endregion

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
        public ICommand AddBugetCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand BudgetDetailCommand { get; }
        public ICommand CloturerCommand { get; }
        public ICommand EncoursCommand { get; }
        public ICommand FilterStatutCommand { get; }

        public BudgetManageViewModel()
        {
            var dbPath = Helper.GetDatabaseFullPath();
            service = new Services.Services(dbPath);
            _alertService = new AlertService();
            RegisterMessenger(); // Enregistre l'écoute du message
            ResetAppMessage();
            SelectedFilter = StatutBudgetConst.Tous;
            //_ = LoadBudgetAsync(SelectedFilter, SearchText); // Charge la liste initialement
            _navigationService = new NavigationService();
            AddBugetCommand = new RelayCommand(OnAddBugetCommand);
            EditCommand = new RelayCommand<BudgetManageModel>(OnEdit);
            DeleteCommand = new RelayCommand<BudgetManageModel>(OnDelete);
            BudgetDetailCommand = new RelayCommand<BudgetManageModel>(OnBudgetDetailCommand);
            CloturerCommand = new RelayCommand<BudgetManageModel>(OnCloturerCommand);
            EncoursCommand = new RelayCommand<BudgetManageModel>(OnEncoursCommand);
            FilterStatutCommand = new RelayCommand<string>(OnFilterStatutCommand);
        }
        private async void ActualiserNombreBudget()
        {
            var statuts = new List<string> { StatutBudgetConst.Ouvert, StatutBudgetConst.Encours, StatutBudgetConst.Cloture };
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

        private async void OnEncoursCommand(BudgetManageModel budget)
        {
            var confirm = await Shell.Current.CurrentPage.DisplayAlert("Confirmation", $"Changer le statut du budget [{budget.CodeBudget}] ?", "Oui", "Non");
            if (confirm)
            {
                var getBudget = await service.GetBudgetByCode(budget.CodeBudget);
                if (getBudget == null)
                {
                    await _alertService.ShowAlertAsync("Information", "Le budget selectionné est indisponible.");
                    return;
                }
                if (getBudget.NbreLigneBudgetaire <= 0)
                {
                    await _alertService.ShowAlertAsync("Information", "Impossible de mettre en cours car il ne contient pas de type de dépense.");
                    return;
                }


                if (getBudget.statutBudget == StatutBudgetConst.Encours || getBudget.statutBudget == StatutBudgetConst.Cloture)
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
        private async void OnAddBugetCommand() => await _navigationService.NavigateToAsync("BudgetView");

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
            var result = await  Validator.ValidateBudgeteDelete(budget);
            if (!result.isSuccess)
            {
                await _alertService.ShowAlertAsync("Erreur", result.message);
                return;
            }
            var confirm = await Shell.Current.CurrentPage.DisplayAlert("Confirmation", $"Supprimer le budget [{budget.CodeBudget}] ?", "Oui", "Non");
            if (confirm)
            {
                var entity = new Budget
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

        private void ResetAppMessage()
        {
            WeakReferenceMessenger.Default.Register<ResetAppMessage>(this, (r, m) =>
            {
                BudgetItems.Clear();
            });
        }
        private void RegisterMessenger()
        {
            WeakReferenceMessenger.Default.Register<RefreshList>(this, async (r, m) =>
            {
                LoadBudgetAsync(SelectedFilter, SearchText); // Rafraîchit la liste si un ajout est effectué
            });
        }

        private async void LoadBudgetAsync(string statut, string searchText)
        {

            List<string> statuts = new List<string>();
            BudgetItems = new ObservableCollection<BudgetManageModel>();
            IsBusy = true;

            if (string.IsNullOrEmpty(statut) || statut == StatutBudgetConst.Tous)
                statuts = new List<string> { StatutBudgetConst.Ouvert, StatutBudgetConst.Encours, StatutBudgetConst.Cloture };
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
                    BackgroundColorStatut = Helper.GetBackgroundColor(x.StatutBudget)
                }));
            ActualiserNombreBudget();
            IsBusy = false;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
