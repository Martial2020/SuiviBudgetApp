using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microcharts;
using SkiaSharp;
using SuiviBudge.Validators;
using SuiviBudget.Mobile.Constants;
using SuiviBudget.Mobile.Interfaces;
using SuiviBudget.Services.DataAccess;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Interfaces;
using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.Services;
using static SQLite.SQLite3;
using static SuiviBuget.Mobile.Messages.Messages;

namespace SuiviBuget.Mobile.ViewModels
{
    partial class StatistiqueManageViewModel : ObservableObject
    {
        IServices _service { get; set; }
        public ICommand SubmitCommand { get; }
        private readonly IAlertService _alertService;
        [ObservableProperty]
        public ObservableCollection<BudgetManageModel> budgetItems;
        [ObservableProperty]
        private Chart topDepensesChart;

        [ObservableProperty]
        public ObservableCollection<GrapheModel> depassementsItems;

        [ObservableProperty]
        private ObservableCollection<ExecutionBudgetaireDetailManageModel> executionBudgetaireItems;

        [ObservableProperty]
        public ChartEntry[] entries;

        [ObservableProperty]
        private decimal totalDepense;

        [ObservableProperty]
        public Chart chart;

        [ObservableProperty]
        public Decimal totalDepassement;

        [ObservableProperty]
        private string rechercherLabel = "🔍 Réchercher";

        [ObservableProperty]
        private bool isEnabled = true; // génère public bool IsEnabled { get; set; }

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
            }
        }

        private DateTime _dateDebut = DateTime.Now;
        public DateTime DateDebut
        {
            get => _dateDebut;
            set
            {
                _dateDebut = value;
                OnPropertyChanged();
            }
        }

        private DateTime _dateFin = DateTime.Now;
        public DateTime DateFin
        {
            get => _dateFin;
            set
            {
                _dateFin = value;
                OnPropertyChanged();
            }
        }



        public StatistiqueManageViewModel()
        {
            string dbPath = Helper.GetDatabaseFullPath();
            _service = new Services.Services(dbPath);
            _alertService = new AlertService();
            SubmitCommand = new AsyncRelayCommand(OnSubmitCommand);
            LoadBudgetAsync();
            RegisterMessenger();
            ChartHeight = Helper.CalculerHauteurChart();
        }
      
        private void RegisterMessenger()
        {
            WeakReferenceMessenger.Default.Register<RefreshList>(this, async (r, m) =>
            {
                LoadBudgetAsync();
            });
        }

        private async void LoadBudgetAsync()
        {
            List<string> statuts = new List<string> { StatutBudgetConst.Ouvert, StatutBudgetConst.Encours, StatutBudgetConst.Cloture };
            BudgetItems = new ObservableCollection<BudgetManageModel>();
            BudgetItems.Clear();
            var budgets = await _service.GetBudgetItemsByStatus("", statuts);
            budgets = budgets.Where(x => x.NbreLigneBudgetaire > 0).ToList();

            BudgetItems = new ObservableCollection<BudgetManageModel>(
                budgets
                    .Select(x => new BudgetManageModel
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
                    })
                    .Prepend(new BudgetManageModel
                    {
                        CodeBudget = GlobalConst.CodeTousLesBudgets,
                        DateCreationBudget = DateTime.Today,
                        DateDebutBudget = DateTime.Today,
                        DateFinBudget = DateTime.Today.AddMonths(1),
                        DescriptionBudget = "Budget manuel",
                        LibelleBudget = $"{GlobalConst.CodeTousLesBudgets} - Tous les budgets",
                        MontantBudget = 0,
                        NbreLigneBudgetaire = 0,
                        StatutBudget = "En Cours",
                        MontantUtilise = 0,
                        MontantRestant = 0
                    }));

            if (BudgetItems.Count > 0)
                SelectedBudget = BudgetItems.FirstOrDefault(b => b.CodeBudget == GlobalConst.CodeTousLesBudgets);

        }
        private async Task OnSubmitCommand()
        {
            try
            {
              
                RechercherLabel = "🔍 Réchercher en cours ...";
                IsEnabled = false;
                var result = await Validator.ValidateRechercheAsync(DateDebut, DateFin, SelectedBudget);
                if (!result.isSuccess)
                {
                    await _alertService.ShowAlertAsync("Erreur", result.message);
                    RechercherLabel = "🔍 Réchercher";
                    IsEnabled = true;
                    return;
                }
                await ClassementBudgetAsync();
                RechercherLabel = "🔍 Réchercher";
                IsEnabled = true;

            }
            catch (Exception ex)
            {

                await _alertService.ShowAlertAsync("Exception", ex.Message);
                RechercherLabel = "🔍 Réchercher";
                IsEnabled = true;

            }
            return;
        }
        
        private async Task LoadChart(List<Budget> datas)
        {
            var resultat = await _service.GetConsommationByLigneBudgetaire(datas,DateDebut,DateFin);
            if (resultat == null)
            {
                //await _alertService.ShowAlertAsync("Information", "Aucun resultat disponible pour ce critère !!!");
                return;
            }

            var entries = resultat.Select(static l => new ChartEntry((float)l.MontantLigneUtilise)
            {
                Label = l.LigneBudgetaire,
                //ValueLabel = $"{Math.Round(l.MontantLigneUtilise / l.MontantLigneBudgetaire * 100)}%",
                ValueLabel = $"{l.MontantLigneUtilise.ToString("N0", CultureInfo.InvariantCulture).Replace(",", " ")} ({Math.Round(l.MontantLigneUtilise / l.MontantLigneBudgetaire * 100)}%)",
                //ValueLabel = $"{l.MontantLigneUtilise}",
                Color = SKColor.Parse(Helper.GetNextColor())
            }).OrderByDescending(e => e.Value).ToList();  // tri décroissant pour que le plus gros segment soit en premier

            TopDepensesChart = new DonutChart
            {
                Entries = entries,
                HoleRadius = 0.5f,
                LabelTextSize = 26
            };

            Depassements(resultat);
        }

        private async Task DepensesByPeriodeItems(List<Budget> budgets)
        {
            ExecutionBudgetaireItems = new ObservableCollection<ExecutionBudgetaireDetailManageModel>();
            ExecutionBudgetaireItems.Clear();
            try
            {
                var executeItems = await _service.GetDepenseItemsByPeriode(DateDebut,DateFin, budgets);
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
        private async void Depassements(List<GrapheModel> datas)
        {
            DepassementsItems = new ObservableCollection<GrapheModel>(
            datas.Select(x => new GrapheModel
            {
                LigneBudgetaire = x.LigneBudgetaire,
                MontantLigneBudgetaire = x.MontantLigneBudgetaire,
                MontantLigneUtilise = x.MontantLigneUtilise,
                Depassement = Math.Abs(x.MontantLigneBudgetaire - x.MontantLigneUtilise)
            }).Where(g => g.MontantLigneBudgetaire - g.MontantLigneUtilise < 0).OrderBy(d => d.LigneBudgetaire)); // garde uniquement les dépassements

            TotalDepassement = Math.Abs((decimal)DepassementsItems.Sum(x => x.Depassement));
        }
            
        public async Task ClassementBudgetAsync()
        {
            // Charger les données de façon asynchrone
            var budgets = await _service.GetBudgetItems(DateDebut, DateFin, SelectedBudget.CodeBudget);
            if (!budgets.Any())
            {
                await _alertService.ShowAlertAsync("Erreur", "Aucune donnée disponible pour ce critère.");

                // Laisser Chart = null quand il n’y a pas de données
                Chart = null;
                TopDepensesChart = null;
                DepassementsItems = new ObservableCollection<GrapheModel>();
                return;
            }

            var classement = budgets.Take(7).ToList();
            // Créer un tableau ChartEntry à partir de la liste
            Entries = classement.Select(l => new ChartEntry((float)l.MontantBudget)
            {
                Label = l.LibelleBudget, // ou l.CodeLigneBudgetaire selon ton besoin
                ValueLabel = l.MontantBudget.ToString("N0", CultureInfo.InvariantCulture).Replace(",", " "),
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
            await LoadChart(budgets);
            await DepensesByPeriodeItems(budgets);
            
        }




    }
}