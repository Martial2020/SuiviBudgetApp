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
using SuiviBuget.Mobile.DataAccess;
using static SuiviBuget.Mobile.Messages.Messages;
using SuiviBuget.Mobile.Helpers;
using SuiviBudge.Validators;


namespace SuiviBuget.Mobile.ViewModels
{
    public partial class ModePaiementManageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ModePaiementManageModel> modePaiementItems;
        IServices adminService { get; set; }
        public ICommand AddModePaiementCommand { get; }
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
                    _ = LoadModePaiementAsync(_searchText); // Charge la liste initialement
                }
            }
        }
        public ModePaiementManageViewModel()
        {
            string dbPath = Helper.GetDatabaseFullPath();
            adminService = new Services.Services(dbPath);
            _alertService = new AlertService();
            RegisterMessenger(); // Enregistre l'écoute du message
            _ = LoadModePaiementAsync(SearchText); // Charge la liste initialement
            _navigationService = new NavigationService();
            AddModePaiementCommand = new RelayCommand(OnAddModePaiementCommand);
            EditCommand = new RelayCommand<ModePaiementManageModel>(OnEdit);
            DeleteCommand = new RelayCommand<ModePaiementManageModel>(OnDelete);
        }

        private async void OnAddModePaiementCommand()
        {
            await _navigationService.NavigateToAsync("ModePaiementView");
        }
        private async void OnEdit(ModePaiementManageModel item)
        {
            if (string.IsNullOrEmpty(item?.CodeModePaiement))
            {
                await _alertService.ShowAlertAsync("Erreur", "Veuillez selectionner une ligne");
                return;
            }
            // Navigation vers la page d'édition avec l'item
            await _navigationService.NavigateToAsync("ModePaiementView", item?.CodeModePaiement, GlobalConst.Edit);
        }
        private async void OnDelete(ModePaiementManageModel item)
        {
            var confirm = await Shell.Current.CurrentPage.DisplayAlert("Confirmation", "Supprimer cet élément ?", "Oui", "Non");
            if (confirm)
            {
                var result = await Validator.ValidateModePaiementDelete(item);
                if (!result.isSuccess)
                {
                    await _alertService.ShowAlertAsync("Erreur", result.message);
                    return;
                }
                var entity = new ModePaiement
                {
                    CodeModePaiement = item.CodeModePaiement,
                    LibelleModePaiement = item.LibelleModePaiement
                };
                var isOk = await adminService.DeleteModePaiementAsync(entity);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de la suppression");
                    return;
                }

                await _alertService.ShowAlertAsync("Information", $"Ligne budgetaire [{entity.LibelleModePaiement}] a été supprimée avec succès");
                WeakReferenceMessenger.Default.Send(new RefreshList());
            }
        }

        private async Task LoadModePaiementAsync(string searchText)
        {
            var paiemnts = await adminService.GetModePaiementItems(searchText);

            ModePaiementItems = new ObservableCollection<ModePaiementManageModel>(
                 paiemnts.Select(x => new ModePaiementManageModel
                 {
                     CodeModePaiement = x.CodeModePaiement,
                     LibelleModePaiement = x.LibelleModePaiement
                 }));
        }

        private void RegisterMessenger()
        {
            WeakReferenceMessenger.Default.Register<RefreshList>(this, async (r, m) =>
            {
                await LoadModePaiementAsync(SearchText); // Rafraîchit la liste si un ajout est effectué
            });
        }
    }
}
