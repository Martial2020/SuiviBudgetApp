using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Interfaces;
using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.Services;

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
        
        public RevenuDetailManageViewModel()
        {
            var dbPath = Helper.GetDatabaseFullPath();
            service = new Services.Services(dbPath);
            _alertService = new AlertService();
            _navigationService = new NavigationService();
            AddDetailRevenuCommand = new RelayCommand(OnAddDetailRevenuCommand);
        }

        private async void OnAddDetailRevenuCommand()
        {
            await _navigationService.NavigateToAsync("RevenuDetailView", CodeRevenu);
        }

        private async Task LoadRevenuAsync(string codeRevenu, string searchText)
        {
            RevenuDetailsItems = new ObservableCollection<RevenuDetailManageModel>();
            IsBusy = true;
            var details = await service.GetSourceRevenuItems(codeRevenu, searchText);
            RevenuDetailsItems = new ObservableCollection<RevenuDetailManageModel>(
                details.Select(x => new RevenuDetailManageModel
                {
                    CodeTypeRevenu = x.CodeTypeRevenu,
                    DateReception = x.DateReception,
                    Description = x.Description,
                    LibelleModePaiement = x.LibelleModePaiement,
                    LibelleTypeRevenu = x.LibelleTypeRevenu,
                    Montant = x.Montant
                }));
            IsBusy = false;
        }
        public async Task InitializePageAsync(string code, string action)
        {
            CodeRevenu = code;
            Title = $"Revenu {code}";
            await LoadRevenuAsync(code, string.Empty);
            //_ = LoadBudgetDetailsAsync(SearchText); // Charge la liste initialement
            //var budget = await service.GetBudgetByCode(CodeBudget);
            //if (budget != null && budget.StatutBudget == StatutBudgetConst.Cloture)
            //    ActionPossible = false;
        }
    }
}
