using System.Collections.ObjectModel;
using System.Windows.Input;
using Budget.Services.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SuiviBudget.Core.Constants;
using SuiviBudget.Core.Interfaces;
using SuiviBuget.Mobile.Interfaces;
using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.Services;
using static SuiviBuget.Mobile.Messages.Messages;

namespace SuiviBuget.Mobile.ViewModels
{
    public partial class LigneBudgetaireManageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<LigneBudgetaireManageModel> ligneBudgetaireItems; IAdminService adminService { get; set; }
        public ICommand AddLigneBugetaireCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;

        public LigneBudgetaireManageViewModel()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), GlobalConst.dbPath);
            adminService = new AdminServices(dbPath);
            _alertService = new AlertService();
            RegisterMessenger(); // Enregistre l'écoute du message
            _ = LoadLigneBudgetaireAsync(); // Charge la liste initialement
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
            // Navigation vers la page d'édition avec l'item
            await _navigationService.NavigateToAsync("LigneBudgetaireView", item?.CodeLigneBudgetaire ?? "");
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
                var isOk = await adminService.DeleteLigneBudgetaireAsync(entity);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de la suppression");
                    return;
                }

                await _alertService.ShowAlertAsync("Information", "Ligne budgetaire supprimé avec succès");
                WeakReferenceMessenger.Default.Send(new RefreshList());
            }
        }
        private void RegisterMessenger()
        {
            WeakReferenceMessenger.Default.Register<RefreshList>(this, async (r, m) =>
            {
                await LoadLigneBudgetaireAsync(); // Rafraîchit la liste si un ajout est effectué
            });
        }
        private async Task LoadLigneBudgetaireAsync()
        {
            var ligneItems = await adminService.GetLigneBudgetaireItems();

            LigneBudgetaireItems = new ObservableCollection<LigneBudgetaireManageModel>(
                ligneItems.Select(x => new LigneBudgetaireManageModel
                {
                    CodeLigneBudgetaire = x.CodeLigneBudgetaire,
                    LibelleLigneBudgetaire = x.LibelleLigneBudgetaire
                }));
        }

    }
}
