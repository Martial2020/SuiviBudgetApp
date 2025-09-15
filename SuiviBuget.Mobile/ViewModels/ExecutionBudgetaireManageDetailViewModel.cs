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
    public partial class ExecutionBudgetaireManageDetailViewModel : ObservableObject
    {
        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private ObservableCollection<LigneBudgetaireManageModel> ligneBudgetaireItems;
        private LigneBudgetaireManageModel _selectedLigneBudgetaire;
        public LigneBudgetaireManageModel SelectedLigneBudgetaire
        {
            get => _selectedLigneBudgetaire;
            set
            {
                _selectedLigneBudgetaire = value;
                OnPropertyChanged();
                // mettre à jour le code dans DataItem
                //DataItem.CodeLigneBudgetaire = value?.CodeLigneBudgetaire;
                _ = LoadExecutionBudgetaireAsync(CodeBudget, value?.CodeLigneBudgetaire);
            }
        }

        [ObservableProperty]
        private ObservableCollection<ExecutionBudgetaireDetailManageModel> executionBugetaireDetailItems;
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
                    //_ = LoadBudgetDetailsAsync(_searchText); // Charge la liste initialement
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
                    //_ = LoadBudgetDetailsAsync(_searchText); // Charge la liste initialement
                }
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;
        IServices _service { get; set; }
        public ExecutionBudgetaireManageDetailViewModel()
        {
            AddCommand = new RelayCommand(OnAddCommand);
            //EditCommand = new RelayCommand<BudgetManageModel>(OnEdit);
            DeleteCommand = new RelayCommand<ExecutionBudgetaireDetailManageModel>(OnDelete);
            _alertService = new AlertService();
            _navigationService = new NavigationService();
            string dbPath = Helper.GetDatabaseFullPath();
            _service = new Services.Services(dbPath);
            RegisterMessenger(); // Enregistre l'écoute du message

        }
        private void RegisterMessenger()
        {
            WeakReferenceMessenger.Default.Register<RefreshList>(this, async (r, m) =>
            {
                await LoadLigneBudgetaireAsync(CodeBudget, SelectedLigneBudgetaire.CodeLigneBudgetaire);// Rafraîchit la liste si un ajout est effectué
            });
        }

        private async Task LoadLigneBudgetaireAsync(string codeBudget, string searchText = "")
        {
            var ligneItems = await _service.GetBudgetDetailItems(codeBudget, searchText);

            LigneBudgetaireItems = new ObservableCollection<LigneBudgetaireManageModel>(
                ligneItems.Select(x => new LigneBudgetaireManageModel
                {
                    CodeLigneBudgetaire = x.CodeLigneBudgetaire,
                    LibelleLigneBudgetaire = $"[{x.CodeLigneBudgetaire}] - {x.LibelleLigneBudgetaire}"
                })
            );
        }

        private async Task LoadExecutionBudgetaireAsync(string codeBudget,string ligne)
        {
            var executeItems = await _service.GetExecutionBudgetaireDetailsItems(codeBudget,ligne);

            ExecutionBugetaireDetailItems = new ObservableCollection<ExecutionBudgetaireDetailManageModel>(
                executeItems.Select(x => new ExecutionBudgetaireDetailManageModel
                {
                    DateExecution = x.DateExecution,
                    ExecutionBudgetaireID = x.ExecutionBudgetaireID,
                    LibelleLigneBudgetaire = x.LibelleLigneBudgetaire,
                    ModePaiement = x.ModePaiement,
                    Montant = x.Montant,
                    CodeBudget=codeBudget,
                    CodeLigneBudgetaire=x.CodeLigneBudgetaire
                })
            );
        }

        private async void OnDelete(ExecutionBudgetaireDetailManageModel model)
        {
            var confirm = await Shell.Current.CurrentPage.DisplayAlert("Confirmation", "Supprimer cet élément ?", "Oui", "Non");
            if (confirm)
            {
                var entity = new ExecutionBudgetaire
                {
                   ExecutionBudgetaireID= model.ExecutionBudgetaireID,
                   CodeBudget=model.CodeBudget

                };
                var isOk = await _service.DeleteExecutionBudgetaireDetailAsync(entity);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de la suppression");
                    return;
                }

                await _alertService.ShowAlertAsync("Information", $"Ligne budgetaire [{model.LibelleLigneBudgetaire}] a été supprimée avec succès");
                SelectedLigneBudgetaire.CodeLigneBudgetaire = model.CodeLigneBudgetaire;
                WeakReferenceMessenger.Default.Send(new RefreshList());
            }
        }

        private async void OnAddCommand()
        {
            if (string.IsNullOrEmpty(CodeBudget))
            {
                await _alertService.ShowAlertAsync("Erreur", "Veuillez selectionner un budget");
                return;
            }
            await _navigationService.NavigateToAsync("ExecutionBudgetaireDetailView", CodeBudget);
        }

        public async Task InitializePageAsync(string code, string action)
        {
            CodeBudget = code;
            Title = $"Liste des exécutions du budget {CodeBudget}";
            _ = LoadLigneBudgetaireAsync(CodeBudget, "");
        }
    }
}
