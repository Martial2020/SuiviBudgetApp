using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls.Shapes;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static SuiviBuget.Mobile.Messages.Messages;

namespace SuiviBuget.Mobile.ViewModels
{
    partial class TableauBordManageViewModel : ObservableObject
    {

        [ObservableProperty]
        private ObservableCollection<ExecutionBudgetaireDetailManageModel> executionBudgetaireItems;

        [ObservableProperty]
        private ObservableCollection<ExecutionBudgetaireDetailManageModel> depassementItems;

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
            LoadDashbordByDate(DateTime.Now);
            RegisterMessenger();
            // Dans le constructeur ou avant l'utilisation
            DepassementItems ??= new ObservableCollection<ExecutionBudgetaireDetailManageModel>();
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
            DepensesDuJourItems(date);

        }

        private async void DepensesDuJourItems(DateTime date)
        {
            try
            {
                var executeItems = await _service.GetDepenseItemsByDate(date);
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
                DepassementDuJourItems(ExecutionBudgetaireItems);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        
        }
        private async void DepassementDuJourItems(ObservableCollection<ExecutionBudgetaireDetailManageModel> datas)
        {
            try
            {
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
                        var difference = detail.Montant - item.MontantTotal;
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
            catch (Exception ex)
            {
                throw ex;
            }

            IsBusy = false;
        }
    }
}
