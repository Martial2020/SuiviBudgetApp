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
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.DataAccess;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Interfaces;
using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.Services;
using static SuiviBuget.Mobile.Messages.Messages;

namespace SuiviBuget.Mobile.ViewModels
{
    public partial class RevenuDetailManageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<RevenuDetailManageModel> revenuDetailsItems;

        [ObservableProperty]
        private string title = "Revenu";

        [ObservableProperty]
        private string codeRevenu;

        [ObservableProperty]
        private bool isBusy;
        IServices service { get; set; }
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;
        public ICommand AddDetailRevenuCommand { get; }
        public ICommand DeleteDetailRevenuCommand { get; }

        public RevenuDetailManageViewModel()
        {
            var dbPath = Helper.GetDatabaseFullPath();
            service = new Services.Services(dbPath);
            _alertService = new AlertService();
            _navigationService = new NavigationService();
            AddDetailRevenuCommand = new RelayCommand(OnAddDetailRevenuCommand);
            DeleteDetailRevenuCommand = new RelayCommand<RevenuDetailManageModel>(OnDeleteDetailRevenuCommand);
            RegisterMessenger();
            ResetAppMessage();
        }

        private async void OnDeleteDetailRevenuCommand(RevenuDetailManageModel model)
        {
            var confirm = await Shell.Current.CurrentPage.DisplayAlert("Confirmation", "Voulez vous Supprimer cet élément ?", "Oui", "Non");
            if (confirm)
            {
                var result = await Validator.ValidateSourceDetailDelete(model);
                if (!result.isSuccess)
                {
                    await _alertService.ShowAlertAsync("Erreur", result.message);
                    return;
                }
                var entity = new RevenuDetail
                {
                    RevenuDetailID = model.RevenuDetailID,
                    Montant = model.Montant,
                    CodeRevenu=model.CodeRevenu
                };
                var isOk = await service.DeleteRevenuDetailAsync(entity);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de la suppression");
                    return;
                }
                await _alertService.ShowAlertAsync("Information", $"Suppression effectuée avec succès !!!");
                WeakReferenceMessenger.Default.Send(new RefreshList());
            }
        }

        private async void OnAddDetailRevenuCommand()
        {
            await _navigationService.NavigateToAsync("RevenuDetailView", CodeRevenu);
        }

        private async Task LoadRevenuAsync(string codeRevenu, string searchText)
        {
            IsBusy = true;
            RevenuDetailsItems = new ObservableCollection<RevenuDetailManageModel>();
            var devise = await Helper.GetDeviseActiveAsyn();
            var details = await service.GetSourceRevenuItems(codeRevenu, searchText);
            RevenuDetailsItems = new ObservableCollection<RevenuDetailManageModel>(
                details.Select(x => new RevenuDetailManageModel
                {
                    CodeTypeRevenu = x.CodeTypeRevenu,
                    DateReception = x.DateReception,
                    Description = x.Description,
                    LibelleModePaiement = $"{x.LibelleModePaiement}",
                    LibelleTypeRevenu = x.LibelleTypeRevenu,
                    Montant = x.Montant,
                    MontantAvecDevise = $"Montant : {x.Montant:N0} {devise}",
                    RevenuDetailID = x.RevenuDetailID,
                    CodeRevenu = x.CodeRevenu,
                }));
            IsBusy = false;
        }
        public async Task InitializePageAsync(string code, string action)
        {
            CodeRevenu = code;
            var source = await service.GetRevenuByCode(CodeRevenu);
            Title = $"Revenu {source.LibelleTypeRevenu}";
            await LoadRevenuAsync(code, string.Empty);
            //_ = LoadBudgetDetailsAsync(SearchText); // Charge la liste initialement
            //var budget = await service.GetBudgetByCode(CodeBudget);
            //if (budget != null && budget.StatutBudget == StatutBudgetConst.Cloture)
            //    ActionPossible = false;
        }

        private void RegisterMessenger()
        {
            WeakReferenceMessenger.Default.Register<RefreshList>(this, async (r, m) =>
            {
                await LoadRevenuAsync(CodeRevenu, string.Empty);
            });
        }

        private void ResetAppMessage()
        {
            WeakReferenceMessenger.Default.Register<ResetAppMessage>(this, (r, m) =>
            {
                RevenuDetailsItems.Clear();
            });
        }
    }
}
