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
using Intuit.Ipp.Data;
using SuiviBudge.Validators;
using SuiviBudget.Mobile.Constants;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.DataAccess;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Interfaces;
using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.Services;
using static SuiviBuget.Mobile.Messages.Messages;

namespace SuiviBuget.Mobile.ViewModels
{
    public partial class ReajusterManageViewModel : ObservableObject
    {
        [ObservableProperty]
        private string title = "Reajustement";

        [ObservableProperty]
        private string codeBudget;

        [ObservableProperty]
        private bool actionPossible = true;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private ObservableCollection<ReajusterManageModel> reajustementItems;

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
                    LoadReajustementAsync(_searchText); // Charge la liste initialement
                }
            }
        }
        IServices service { get; set; }

        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;
        public ICommand DeleteCommand { get; }

        public ReajusterManageViewModel()
        {
            var dbPath = Helper.GetDatabaseFullPath();
            service = new Services.Services(dbPath);
            _navigationService = new NavigationService();
            _alertService = new AlertService();
            RegisterMessenger();
            DeleteCommand = new RelayCommand<ReajusterManageModel>(OnDelete);
            ResetAppMessage();

        }

        private void ResetAppMessage()
        {
            WeakReferenceMessenger.Default.Register<ResetAppMessage>(this, (r, m) =>
            {
                reajustementItems.Clear();
            });
        }
        private async void OnDelete(ReajusterManageModel model)
        {
            var confirm = await Shell.Current.CurrentPage.DisplayAlert("Confirmation", "Voulez vous supprimer ce réajustement ?", "Oui", "Non");
            if (confirm)
            {
                var result = await Validator.ValidateReajustementDelete(model);
                if (!result.isSuccess)
                {
                    await _alertService.ShowAlertAsync("Information", result.message);
                    return;
                }
                var entity = new Reajustement
                {
                    ReajustementID = model.ReajusterID,
                    CodeBudget = model.CodeBudget,
                    CodeLigneBudgetaire = model.CodeLigneBudgetaire,
                    Montant = model.Montant
                };
                var isOk = await service.DeleteReajustementAsync(entity);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de la suppression");
                    return;
                }

                await _alertService.ShowAlertAsync("Information", $"Le réajustement a été supprimé avec succès");
                WeakReferenceMessenger.Default.Send(new RefreshList());
            }
        }
        private void RegisterMessenger()
        {
            WeakReferenceMessenger.Default.Register<RefreshList>(this, async (r, m) =>
            {
                LoadReajustementAsync(SearchText);
            });
        }

        [RelayCommand]
        private async void AddReajustement(string codeBudget)
        {
            await _navigationService.NavigateToAsync("ReajusterView", codeBudget);
        }
        public async void InitializePageAsync(string code, string action)
        {
            CodeBudget = code;
            Title = $"Réajustement du {CodeBudget}";
            LoadReajustementAsync(SearchText); // Charge la liste initialement
            var budget = await service.GetBudgetByCode(CodeBudget);
            if (budget != null && budget.StatutBudget == StatutBudgetConst.Cloture)
                ActionPossible = false;
        }
        private async void LoadReajustementAsync(string searchText)
        {
            IsBusy = true;
            try
            {
                var code = new List<string> { CodeBudget };

                var details = await service.GetReajustementItems(code, searchText);

                // Vérifie si details est null avant le Select
                if (details == null)
                {
                    ReajustementItems = new ObservableCollection<ReajusterManageModel>();
                    return;
                }

                ReajustementItems = new ObservableCollection<ReajusterManageModel>(
                  details.Select(x => new ReajusterManageModel
                  {
                      CodeLigneBudgetaire = x.CodeLigneBudgetaire,
                      LibelleLigneBudgetaire = x.LibelleLigneBudgetaire,
                      Montant = x.Montant,
                      ReajusterID = x.ReajusterID,
                      CodeBudget = x.CodeBudget,
                      Motif = x.Motif,
                      DateReajustement= x.DateReajustement
                  })
                  .OrderByDescending(x => x.DateReajustement)
              );

            }
            catch (Exception ex)
            {
                // 👉 Tu peux logger ici
                Console.WriteLine($"Erreur dans LoadReajustementAsync : {ex.Message}");
                // Ou afficher une alerte si tu es dans MAUI ou Blazor
            }
            finally
            {
                // Ce bloc est TOUJOURS exécuté, même si une erreur survient
                IsBusy = false;
            }
        }

    }
}
