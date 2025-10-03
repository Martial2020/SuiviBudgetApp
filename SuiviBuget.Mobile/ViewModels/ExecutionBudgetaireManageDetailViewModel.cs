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
using Microsoft.Maui.Controls.Shapes;
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
                _ = LoadExecutionBudgetaireAsync(CodeBudget, _selectedLigneBudgetaire.CodeLigneBudgetaire);
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

        private bool _isBusy = false;
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

        private bool _IsVisibleBadgeFrame = false;
        public bool IsVisibleBadgeFrame
        {
            get => _IsVisibleBadgeFrame;
            set
            {
                if (_IsVisibleBadgeFrame != value)
                {
                    _IsVisibleBadgeFrame = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal _montantBudget = 0;
        public decimal MontantBudget
        {
            get => _montantBudget;
            set
            {
                if (_montantBudget != value)
                {
                    _montantBudget = value;
                    OnPropertyChanged();
                }
            }
        }


        private decimal _montantUtilise = 0;
        public decimal MontantUtilise
        {
            get => _montantUtilise;
            set
            {
                if (_montantUtilise != value)
                {
                    _montantUtilise = value;
                    OnPropertyChanged();
                }
            }
        }


        private decimal _montantRestant = 0;
        public decimal MontantRestant
        {
            get => _montantRestant;
            set
            {
                if (_montantRestant != value)
                {
                    _montantRestant = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _actionPossible = true;
        public bool ActionPossible
        {
            get => _actionPossible;
            set
            {
                if (_actionPossible != value)
                {
                    _actionPossible = value;
                    OnPropertyChanged();
                }
            }
        }


        #region Progress Bar
        private double _progressValue;
        public double ProgressValue
        {
            get => _progressValue;
            set { _progressValue = value; OnPropertyChanged(); }
        }
        private Color _progressColor = Colors.Green;
        public Color ProgressColor
        {
            get => _progressColor;
            set { _progressColor = value; OnPropertyChanged(); }
        }

        private string _progressText;
        public string ProgressText
        {
            get => _progressText;
            set { _progressText = value; OnPropertyChanged(); }
        }
        #endregion
        public ICommand AddCommand { get; }
        public ICommand DescriptionCommand { get; }
        public ICommand DeleteCommand { get; }
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;
        IServices _service { get; set; }
        public ExecutionBudgetaireManageDetailViewModel()
        {
            AddCommand = new RelayCommand(OnAddCommand);
            DescriptionCommand = new RelayCommand<ExecutionBudgetaireDetailManageModel>(OnDescription);
            DeleteCommand = new RelayCommand<ExecutionBudgetaireDetailManageModel>(OnDelete);
            _alertService = new AlertService();
            _navigationService = new NavigationService();
            string dbPath = Helper.GetDatabaseFullPath();
            _service = new Services.Services(dbPath);
            RegisterMessenger(); // Enregistre l'écoute du message
            ResetAppMessage();

        }

        private void ProgressBarResult()
        {

        }
        private void ResetAppMessage()
        {
            WeakReferenceMessenger.Default.Register<ResetAppMessage>(this, (r, m) =>
            {
                LigneBudgetaireItems.Clear();
                ExecutionBugetaireDetailItems.Clear();
            });
        }
        private async void ActualiserMontants(string codeBudget, string ligne)
        {
            var detail = await _service.GetBudgetDetailByBudgetLigne(codeBudget, ligne);
            MontantBudget = detail == null ? 0 : detail.Montant;
            MontantUtilise = ExecutionBugetaireDetailItems?.Sum(x => x.Montant) ?? 0;
            MontantRestant = MontantBudget - MontantUtilise;

            var ratio = MontantBudget == 0 ? 0 : (double)(MontantUtilise / MontantBudget);
            ProgressValue = Math.Min(ratio, 1); // ProgressBar = 0.16 (16%)
            //ProgressText = $"{(ratio <= 1 ? ratio : 1) * 100:0}%"; // Affiche "16%"
            ProgressText = $"{Math.Round((ratio <= 1 ? ratio : 1) * 100)}%"; // Affiche "16%"
            ProgressColor = ratio <= 1 ? Colors.Green : Colors.Red;

        }
        private void RegisterMessenger()
        {
            WeakReferenceMessenger.Default.Register<RefreshList>(this, async (r, m) =>
            {
                //await LoadExecutionBudgetaireAsync(CodeBudget, SelectedLigneBudgetaire.CodeLigneBudgetaire);// Rafraîchit la liste si un ajout est effectué
                SelectedLigneBudgetaire = LigneBudgetaireItems.FirstOrDefault(l => l.CodeLigneBudgetaire == SelectedLigneBudgetaire.CodeLigneBudgetaire);
            });
        }

        private async Task LoadLigneBudgetaireAsync(string codeBudget, string searchText = "")
        {
            var ligneItems = await _service.GetBudgetDetailItems(codeBudget, searchText);

            LigneBudgetaireItems = new ObservableCollection<LigneBudgetaireManageModel>(
                ligneItems.Select(x => new LigneBudgetaireManageModel
                {
                    CodeLigneBudgetaire = x.CodeLigneBudgetaire,
                    LibelleLigneBudgetaire = $"[{x.CodeLigneBudgetaire}] - {x.LibelleLigneBudgetaire}",
                    LibelleLigneBudgetaireSimple=x.LibelleLigneBudgetaire
                }).OrderBy(l =>l.LibelleLigneBudgetaireSimple)
            );

            if (LigneBudgetaireItems.Count > 0 && SelectedLigneBudgetaire == null)
                SelectedLigneBudgetaire = LigneBudgetaireItems.FirstOrDefault();

        }

        private async Task LoadExecutionBudgetaireAsync(string codeBudget, string ligne)
        {
            ExecutionBugetaireDetailItems = new ObservableCollection<ExecutionBudgetaireDetailManageModel>();
            IsBusy = true;
            IsVisibleBadgeFrame = true;
            //await Task.Delay(1000); // ⏳ attend 1,5 secondes (1500 ms)
            var executeItems = await _service.GetExecutionBudgetaireDetailsItems(codeBudget, ligne);
            ExecutionBugetaireDetailItems = new ObservableCollection<ExecutionBudgetaireDetailManageModel>(
                executeItems.Select(x => new ExecutionBudgetaireDetailManageModel
                {
                    DateExecution = x.DateExecution,
                    ExecutionBudgetaireID = x.ExecutionBudgetaireID,
                    LibelleLigneBudgetaire = x.LibelleLigneBudgetaire,
                    ModePaiement = x.ModePaiement,
                    Montant = x.Montant,
                    CodeBudget = x.CodeBudget,
                    CodeLigneBudgetaire = x.CodeLigneBudgetaire,
                    Description = x.Description,
                })
            );

            ActualiserMontants(codeBudget, ligne);
            //SelectedLigneBudgetaire = LigneBudgetaireItems.FirstOrDefault(x => x.CodeLigneBudgetaire == ligne);

            IsBusy = false;
        }


        private async void OnDelete(ExecutionBudgetaireDetailManageModel model)
        {
            var confirm = await Shell.Current.CurrentPage.DisplayAlert("Confirmation", "Supprimer cet élément ?", "Oui", "Non");
            if (confirm)
            {
                var entity = new ExecutionBudgetaire
                {
                    ExecutionBudgetaireID = model.ExecutionBudgetaireID,
                    CodeBudget = model.CodeBudget

                };
                var isOk = await _service.DeleteExecutionBudgetaireDetailAsync(entity);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de la suppression");
                    return;
                }

                await _alertService.ShowAlertAsync("Information", $"La dépense a été supprimée avec succès");
                WeakReferenceMessenger.Default.Send(new RefreshList());
            }
        }

        private async void OnDescription(ExecutionBudgetaireDetailManageModel model)
        {
            if (string.IsNullOrEmpty(model.Description))
                await _alertService.ShowAlertAsync("Information", "Pas de description");
            else
                await _alertService.ShowAlertAsync("Information", model.Description);
        }

        private async void OnAddCommand()
        {
            if (string.IsNullOrEmpty(CodeBudget))
            {
                await _alertService.ShowAlertAsync("Erreur", "Veuillez selectionner un budget");
                return;
            }
            var budgetEtLigne = $"{CodeBudget}X{SelectedLigneBudgetaire.CodeLigneBudgetaire}";
            await _navigationService.NavigateToAsync("ExecutionBudgetaireDetailView", budgetEtLigne);
        }

        public async Task InitializePageAsync(string code, string action)
        {
            CodeBudget = code;
            Title = $"Dépenses {CodeBudget}";
            _ = LoadLigneBudgetaireAsync(CodeBudget, "");
            var buget = await _service.GetBudgetByCode(CodeBudget);
            if (buget != null && buget.StatutBudget == StatutBudgetConst.Cloture)
                ActionPossible = false;
        }
    }
}
