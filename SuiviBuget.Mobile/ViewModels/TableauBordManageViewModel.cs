using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microcharts;
using Microsoft.Maui.Controls.Shapes;
using SkiaSharp;
using SuiviBudget.Mobile.Constants;
using SuiviBudget.Mobile.Interfaces;
using SuiviBudget.Services.DataAccess;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static SuiviBuget.Mobile.Messages.Messages;
using Microcharts.Maui;
using SkiaSharp;


namespace SuiviBuget.Mobile.ViewModels
{
    partial class TableauBordManageViewModel : ObservableObject
    {

        [ObservableProperty]
        private ObservableCollection<ExecutionBudgetaireDetailManageModel> executionBudgetaireItems;

        [ObservableProperty]
        private ObservableCollection<ExecutionBudgetaireDetailManageModel> depassementItems;

        [ObservableProperty]
        public ObservableCollection<BudgetManageModel> budgetItems;

        [ObservableProperty]
        public ChartEntry[] entries;

        [ObservableProperty]
        public Chart chart;

        private BudgetManageModel _selectedBudget;
        public BudgetManageModel SelectedBudget
        {
            get => _selectedBudget;
            set
            {
                _selectedBudget = value;
                OnPropertyChanged();
                if (_selectedBudget != null)
                    _ = ChargerGraphesAsync(_selectedBudget);
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

        IServices _service { get; set; }
        public TableauBordManageViewModel()
        {
            string dbPath = Helper.GetDatabaseFullPath();
            _service = new Services.Services(dbPath);
            //LoadDashbordByDate(DateTime.Now);
            RegisterMessenger();
            // Dans le constructeur ou avant l'utilisation
            ResetAppMessage();
            LoadBudgetAsync();
        }

        public async Task ChargerGraphesAsync(BudgetManageModel budget)
        {

            if (budget == null) return;

            // Charger les données de façon asynchrone
            var ligneBudgetaire = await _service.GetBudgetDetailItems(budget.CodeBudget, "");

            // Créer un tableau ChartEntry à partir de la liste
            Entries = ligneBudgetaire.Select(l => new ChartEntry((float)l.Montant)
            {
                Label = l.LibelleLigneBudgetaire, // ou l.CodeLigneBudgetaire selon ton besoin
                //ValueLabel = $"{Math.Round((l.Montant / budget.MontantBudget) * 100)}%",
                ValueLabel = "",
                //Color = SKColor.Parse("#FF0000")
                Color = SKColor.Parse("#FF6347")
            }).ToArray();

            //// Créer le graphique
            Chart = new BarChart
            {
                Entries = entries,
                LabelTextSize = 26,
                ValueLabelOrientation = Orientation.Horizontal,
                BackgroundColor = SKColors.White
            };

            await DepensesDuJourItems(DateTime.Now, budget.CodeBudget);


        }


        private void ResetAppMessage()
        {
            WeakReferenceMessenger.Default.Register<ResetAppMessage>(this, (r, m) =>
            {
                ExecutionBudgetaireItems.Clear();
                DepassementItems.Clear();
            });
        }
        private void RegisterMessenger()
        {
            WeakReferenceMessenger.Default.Register<RefreshList>(this, async (r, m) =>
            {
                LoadDashbordByDate(DateTime.Now);// Rafraîchit la liste si un ajout est effectué
            });
        }
        private async void LoadDashbordByDate(DateTime date)
        {
            ExecutionBudgetaireItems = new ObservableCollection<ExecutionBudgetaireDetailManageModel>();
            IsBusy = true;
            await Task.Delay(1000); // ⏳ attend 1,5 secondes (1500 ms)
            LoadBudgetAsync();
            //DepensesDuJourItems(date);
        }

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
                    MontantRestant = x.MontantRestant
                }));

            if (BudgetItems.Count() > 0)
                SelectedBudget = BudgetItems.FirstOrDefault();

        }

        private async Task DepensesDuJourItems(DateTime date, string codeBudget)
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
                await DepassementDuJourItems(ExecutionBudgetaireItems);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private async Task DepassementDuJourItems(ObservableCollection<ExecutionBudgetaireDetailManageModel> datas)
        {
            try
            {
                DepassementItems = new ObservableCollection<ExecutionBudgetaireDetailManageModel>();
                DepassementItems.Clear();
                var groupedItems = datas
            .GroupBy(x => new { x.CodeLigneBudgetaire, x.LibelleLigneBudgetaire, x.CodeBudget, x.LibelleBudget })
                    .Select(g => new
                    {
                        g.Key.CodeLigneBudgetaire,
                        g.Key.LibelleLigneBudgetaire,
                        g.Key.LibelleBudget,
                        g.Key.CodeBudget,
                        MontantTotal = g.Sum(x => x.Montant)
                    }).ToList();

                foreach (var item in groupedItems)
                {
                    var detail = await _service.GetBudgetDetailByBudgetLigne(item.CodeBudget, item.CodeLigneBudgetaire);
                    if (detail != null)
                    {
                        var depense = await _service.GetExecutionBudgetaireDetailsItems(item.CodeBudget, item.CodeLigneBudgetaire);

                        if (depense != null)
                        {
                            decimal montantTotal = 0;
                            montantTotal = depense.Sum(d => d.Montant);
                            var difference = detail.Montant - montantTotal;
                            if (difference < 0)
                            {
                                var nouvelItem = new ExecutionBudgetaireDetailManageModel
                                {
                                    DateExecution = DateTime.Now,
                                    ExecutionBudgetaireID = Guid.NewGuid(),
                                    LibelleLigneBudgetaire = item.LibelleLigneBudgetaire,
                                    ModePaiement = "Cash",
                                    Montant = Math.Abs(difference),
                                    CodeBudget = item.CodeBudget,
                                    CodeLigneBudgetaire = item.CodeLigneBudgetaire,
                                    Description = "Dépassement transport",
                                    LibelleBudget = item.LibelleBudget,
                                };
                                // Ajouter à la collection
                                DepassementItems.Add(nouvelItem);
                            }
                        }

                    }
                }
                DepassementItems = new ObservableCollection<ExecutionBudgetaireDetailManageModel>(
    DepassementItems.GroupBy(x => new { x.CodeBudget, x.CodeLigneBudgetaire }).Select(g => g.First()));

            }
            catch (Exception ex)
            {
                throw ex;
            }

            IsBusy = false;
        }
    }
}
