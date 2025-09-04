using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SuiviBudget.Mobile.Constants;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Interfaces;
using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.Services;
using static SuiviBuget.Mobile.Messages.Messages;

namespace SuiviBuget.Mobile.ViewModels
{
    partial class BudgetDetailManageViewModel : ObservableObject
    {
        [ObservableProperty]
        private string title = "Budget";

        [ObservableProperty]
        private ObservableCollection<BudgetDetailManageModel> budgetDetailsItems;
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
                    _ = LoadBudgetDetailsAsync(_searchText); // Charge la liste initialement
                }
            }
        }
        private string _codeBudget;
        public string CodeBudget
        {
            get => _codeBudget;
            set
            {
                if (_codeBudget != value)
                {
                    _codeBudget = value;
                    OnPropertyChanged();
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
        IService service { get; set; }

        public ICommand AddBudgetDetailCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;
        public BudgetDetailManageViewModel()
        {
            var dbPath = Helper.GetDatabaseFullPath();
            service = new Services.Services(dbPath);
            _alertService = new AlertService();
            _navigationService = new NavigationService();
            AddBudgetDetailCommand = new RelayCommand<string>(OnAddBudgetDetailCommand);
            DeleteCommand = new RelayCommand<BudgetDetailManageModel>(OnDelete);
            EditCommand = new RelayCommand<BudgetDetailManageModel>(OnEdit);

            RegisterMessenger(); // Enregistre l'écoute du message

        }
        private async void OnEdit(BudgetDetailManageModel item)
        {
            if (string.IsNullOrEmpty(item?.CodeLigneBudgetaire))
            {
                await _alertService.ShowAlertAsync("Erreur", "Veuillez selectionner une ligne");
                return;
            }
            // Navigation vers la page d'édition avec l'item
            await _navigationService.NavigateToAsync("BudgetDetailView", item.BudgetDetailID.ToString(), GlobalConst.Edit);
        }
        private async void OnDelete(BudgetDetailManageModel item)
        {
            var confirm = await Shell.Current.CurrentPage.DisplayAlert("Confirmation", "Supprimer cet élément ?", "Oui", "Non");
            if (confirm)
            {         
                var isOk = await service.DeleteBudgetDetailAsync(item);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de la suppression");
                    return;
                }

                await _alertService.ShowAlertAsync("Information", $"Ligne budgetaire [{item.LibelleLigneBudgetaire}] a été supprimée avec succès");
                WeakReferenceMessenger.Default.Send(new RefreshList());
            }
        }
        private void RegisterMessenger()
        {
            WeakReferenceMessenger.Default.Register<RefreshList>(this, async (r, m) =>
            {
                await LoadBudgetDetailsAsync(SearchText); // Rafraîchit la liste si un ajout est effectué
            });
        }
        private async Task LoadBudgetDetailsAsync(string searchText)
        {
            BudgetDetailsItems = null;
            IsBusy = true;
            var details = await service.GetBudgetDetailItems(CodeBudget,searchText);
            BudgetDetailsItems = new ObservableCollection<BudgetDetailManageModel>(
                details.Select(x => new BudgetDetailManageModel
                {
                    CodeLigneBudgetaire = x.CodeLigneBudgetaire,
                    LibelleLigneBudgetaire = x.LibelleLigneBudgetaire,
                    Montant=x.Montant,
                    BudgetDetailID=x.BudgetDetailID,
                    CodeBudget=x.CodeBudget
                }));
            IsBusy = false;
        }
        private async void OnAddBudgetDetailCommand(string codeBudget)
        {
            await _navigationService.NavigateToAsync("BudgetDetailView",codeBudget);
        }
        public async Task InitializePageAsync(string code, string action)
        {
            CodeBudget = code;
            Title = $"Détails du budget {CodeBudget}";
            _ = LoadBudgetDetailsAsync(SearchText); // Charge la liste initialement
        }
    }
}
