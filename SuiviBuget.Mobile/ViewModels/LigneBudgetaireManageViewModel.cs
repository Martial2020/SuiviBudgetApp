using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SuiviBudget.Mobile.Constants;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.Interfaces;
using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.Services;
using static SuiviBuget.Mobile.Messages.Messages;
using SuiviBuget.Mobile.Helpers;


namespace SuiviBuget.Mobile.ViewModels
{
    public partial class LigneBudgetaireManageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<LigneBudgetaireManageModel> ligneBudgetaireItems; 
        IService adminService { get; set; }
        public ICommand AddLigneBugetaireCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;

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
                    _ = LoadLigneBudgetaireAsync(_searchText); // Charge la liste initialement
                }
            }
        }
        public LigneBudgetaireManageViewModel()
        {
            string dbPath = Helper.GetDatabaseFullPath();
            adminService = new Services.Services(dbPath);
            _alertService = new AlertService();
            RegisterMessenger(); // Enregistre l'écoute du message
            _ = LoadLigneBudgetaireAsync(SearchText); // Charge la liste initialement
            _navigationService = new NavigationService();
            AddLigneBugetaireCommand = new RelayCommand(OnAddLigneBugetaireCommand);
            EditCommand = new RelayCommand<LigneBudgetaireManageModel>(OnEdit);
            DeleteCommand = new RelayCommand<LigneBudgetaireManageModel>(OnDelete);
        }

        private async void OnAddLigneBugetaireCommand()
        {
            await _navigationService.NavigateToAsync("LigneBudgetaireView");
        }
        private async void OnEdit(LigneBudgetaireManageModel item)
        {
            if (string.IsNullOrEmpty(item?.CodeLigneBudgetaire))
            {
                await _alertService.ShowAlertAsync("Erreur", "Veuillez selectionner une ligne");
                return;
            }
            // Navigation vers la page d'édition avec l'item
            await _navigationService.NavigateToAsync("LigneBudgetaireView", item?.CodeLigneBudgetaire, GlobalConst.Edit);
        }
        private async void OnDelete(LigneBudgetaireManageModel item)
        {
            var confirm = await Shell.Current.CurrentPage.DisplayAlert("Confirmation", "Supprimer cet élément ?", "Oui", "Non");
            if (confirm)
            {
                var entity = new LigneBudgetaireModel
                {
                    CodeLigneBudgetaire = item.CodeLigneBudgetaire,
                    LibelleLigneBudgetaire = item.LibelleLigneBudgetaire
                };
                var isOk = await adminService.DeleteDetailBudgetAsync(entity);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de la suppression");
                    return;
                }

                await _alertService.ShowAlertAsync("Information", $"Ligne budgetaire [{entity.LibelleLigneBudgetaire}] a été supprimée avec succès");
                WeakReferenceMessenger.Default.Send(new RefreshList());
            }
        }
        private void RegisterMessenger()
        {
            WeakReferenceMessenger.Default.Register<RefreshList>(this, async (r, m) =>
            {
                await LoadLigneBudgetaireAsync(SearchText); // Rafraîchit la liste si un ajout est effectué
            });
        }
        private async Task LoadLigneBudgetaireAsync(string searchText)
        {
            var ligneItems = await adminService.GetLigneBudgetaireItems(searchText);

            LigneBudgetaireItems = new ObservableCollection<LigneBudgetaireManageModel>(
                ligneItems.Select(x => new LigneBudgetaireManageModel
                {
                    CodeLigneBudgetaire = x.CodeLigneBudgetaire,
                    LibelleLigneBudgetaire = x.LibelleLigneBudgetaire
                }));
        }

    }
}
