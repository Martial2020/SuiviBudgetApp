using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Intuit.Ipp.Data;
using Microcharts;
using Microcharts.Maui;
using Microsoft.Maui.Controls.Shapes;
using SkiaSharp;
using SkiaSharp;
using SQLite;
using SuiviBudget.Mobile.Constants;
using SuiviBudget.Mobile.Interfaces;
using SuiviBudget.Services.DataAccess;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.Views.Popups;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static SuiviBuget.Mobile.Messages.Messages;


namespace SuiviBuget.Mobile.ViewModels
{
    partial class TableauBordManageViewModel : ObservableObject
    {

        [ObservableProperty]
        private ObservableCollection<ExecutionBudgetaireDetailManageModel> executionBudgetaireItems;

        [ObservableProperty]
        private ObservableCollection<ReajusterManageModel> depassementItems;

        [ObservableProperty]
        public ObservableCollection<BudgetManageModel> budgetItems;

        [ObservableProperty]
        public ChartEntry[] entries;

        [ObservableProperty]
        public Chart chart;

        [ObservableProperty]
        public decimal totalDepassement;

        [ObservableProperty]
        private decimal totalDepense;

        [ObservableProperty]
        private double chartHeight;

        private BudgetManageModel _selectedBudget;
        public BudgetManageModel SelectedBudget
        {
            get => _selectedBudget;
            set
            {
                _selectedBudget = value;
                OnPropertyChanged();
                if (_selectedBudget != null)
                {
                    ChargerGraphesAsync(_selectedBudget);
                    DepensesDuJourItems(DateTime.Now, _selectedBudget.CodeBudget);
                    DepassementDuJourItems();
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

        private bool _isVisibleBudgetDetail = false;
        public bool IsVisibleBudgetDetail
        {
            get => _isVisibleBudgetDetail;
            set
            {
                if (_isVisibleBudgetDetail != value)
                {
                    _isVisibleBudgetDetail = value;
                    OnPropertyChanged();
                }
            }
        }

        IServices _service { get; set; }
        public TableauBordManageViewModel()
        {

            string dbPath = Helper.GetDatabaseFullPath();
            _service = new Services.Services(dbPath);
            ExecutionBudgetaireItems = new ObservableCollection<ExecutionBudgetaireDetailManageModel>();
            BudgetItems = new ObservableCollection<BudgetManageModel>();
            DepassementItems = new ObservableCollection<ReajusterManageModel>();
            ChartHeight = Helper.CalculerHauteurChart();
            LoadBudgetAsync();
            //LoadDashbordByDate(DateTime.Now);
            RegisterMessenger();
            // Dans le constructeur ou avant l'utilisation
            ResetAppMessage();
        }

        private async System.Threading.Tasks.Task CheckLicenceAsync()
        {

            //Crée et affiche le PopUp ici
            var menuPopup = new ActivationLicenceView();
            await Shell.Current.CurrentPage.ShowPopupAsync(menuPopup);
        }
        public async void ChargerGraphesAsync(BudgetManageModel budget)
        {

            if (budget == null) return;

            // Charger les données de façon asynchrone
            var ligneBudgetaire = await _service.GetBudgetDetailItems(budget.CodeBudget, "");

            if (budget.MontantBudget == 0) return;

            // Créer un tableau ChartEntry à partir de la liste
            Entries = ligneBudgetaire.Select(l => new ChartEntry((float)l.Montant)
            {
                Label = l.LibelleLigneBudgetaire, // ou l.CodeLigneBudgetaire selon ton besoin
                ValueLabel = ligneBudgetaire.Count <= 15 ? $"{Math.Round((l.Montant / budget.MontantBudget) * 100)}%" : "",
                //ValueLabel = "",
                //Color = SKColor.Parse("#FF0000")
                Color = SKColor.Parse(Helper.GetNextColor())
                //Color = SKColor.Parse("#FF6347")
            }).ToArray();

            //// Créer le graphique
            Chart = new BarChart
            {
                Entries = entries,
                LabelTextSize = 26,
                ValueLabelOrientation = Orientation.Horizontal,
                BackgroundColor = SKColors.White
            };
        }

        private void ResetAppMessage()
        {
            WeakReferenceMessenger.Default.Register<ResetAppMessage>(this, (r, m) =>
            {
                ExecutionBudgetaireItems.Clear();
                DepassementItems.Clear();
                IsVisibleBudgetDetail = false;
                BudgetItems.Clear();
                SelectedBudget = BudgetItems.FirstOrDefault();
            });
        }
        private async void RegisterMessenger()
        {
            WeakReferenceMessenger.Default.Register<RefreshList>(this, async (r, m) =>
            {
                LoadBudgetAsync();
            });
        }
        //private async void LoadDashbordByDate(DateTime date)
        //{
        //    ExecutionBudgetaireItems = new ObservableCollection<ExecutionBudgetaireDetailManageModel>();
        //    IsBusy = true;
        //    //await Task.Delay(1000); // ⏳ attend 1,5 secondes (1500 ms)
        //    LoadBudgetAsync();
        //    //DepensesDuJourItems(date);
        //}

        private async void LoadBudgetAsync()
        {
            List<string> statuts = new List<string> { StatutBudgetConst.Encours }; ;
            BudgetItems = new ObservableCollection<BudgetManageModel>();
            BudgetItems.Clear();
            var budgets = await _service.GetBudgetItemsByStatus("", statuts);

            BudgetItems = new ObservableCollection<BudgetManageModel>(
                budgets.Select(x => new BudgetManageModel
                {
                    CodeBudget = x.CodeBudget,
                    DateCreationBudget = x.DateCreationBudget,
                    DateDebutBudget = x.DateDebutBudget,
                    DateFinBudget = x.DateFinBudget,
                    DescriptionBudget = x.DescriptionBudget,
                    LibelleBudget = $"{x.CodeBudget} - {x.LibelleBudget}",
                    MontantBudget = x.MontantBudget,
                    NbreLigneBudgetaire = x.NbreLigneBudgetaire,
                    StatutBudget = x.StatutBudget,
                    MontantUtilise = x.MontantUtilise,
                    MontantRestant = x.MontantRestant,
                    MontantAlloue=x.MontantAlloue,
                    MontantNonAlloue=x.MontantNonAlloue,
                    MontantReajustement=x.MontantReajustement
                }));

            if (BudgetItems.Count() > 0)
            {
                SelectedBudget = BudgetItems.FirstOrDefault();
                IsVisibleBudgetDetail = true;
            }
            else
            {
                IsVisibleBudgetDetail = false;
                ExecutionBudgetaireItems.Clear();
                DepassementItems.Clear();
            }

        }

        private async void DepensesDuJourItems(DateTime date, string codeBudget)
        {
            ExecutionBudgetaireItems = new ObservableCollection<ExecutionBudgetaireDetailManageModel>();
            ExecutionBudgetaireItems.Clear();
            try
            {
                var executeItems = await _service.GetDepenseItemsByDate(date, codeBudget);
                ExecutionBudgetaireItems = new ObservableCollection<ExecutionBudgetaireDetailManageModel>(
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
                        LibelleBudget = x.LibelleBudget
                    })
                );

                TotalDepense = ExecutionBudgetaireItems.Sum(x => x.Montant);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private async void DepassementDuJourItems()
        {
            try
            {
                DepassementItems = new ObservableCollection<ReajusterManageModel>();
                DepassementItems.Clear();
                var code = new List<string> { SelectedBudget.CodeBudget };
                var budgetDetails = await _service.GetReajustementItems(code, string.Empty);
                DepassementItems = new ObservableCollection<ReajusterManageModel>(
 budgetDetails.GroupBy(x => new { x.CodeBudget, x.CodeLigneBudgetaire,x.LibelleLigneBudgetaire }).Select(g => g.First()));
                TotalDepassement = DepassementItems.Sum(x => x.Montant);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            IsBusy = false;
        }
    }
}
