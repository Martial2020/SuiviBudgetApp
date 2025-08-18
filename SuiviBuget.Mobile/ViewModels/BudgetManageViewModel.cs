using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuiviBuget.Mobile.Models;

namespace SuiviBuget.Mobile.ViewModels
{
    public class BudgetManageViewModel
    {
        public ObservableCollection<BudgetManageModel> BudgetItems { get; set; }

        public BudgetManageViewModel()
        {
            BudgetItems = new ObservableCollection<BudgetManageModel>
        {
            new BudgetManageModel
            {
                CodeBudget = "001",
                LibelleBudget = "Budget Marketing",
                DescriptionBudget = "Budget pour les campagnes marketing 2025",
                MontantBudget = 200_000m,
                DateCreationBudget = DateTime.Now.AddDays(-10),
                DateDebutBudget = DateTime.Now,
                DateFinBudget = DateTime.Now.AddMonths(1),
                StatutBudget = true,
                NbreLigneBudgetaire = 5
            },
            new BudgetManageModel
            {
                CodeBudget = "002",
                LibelleBudget = "Budget IT",
                DescriptionBudget = "Budget pour le développement et la maintenance IT",
                MontantBudget = 678_000m,
                DateCreationBudget = DateTime.Now.AddDays(-20),
                DateDebutBudget = DateTime.Now,
                DateFinBudget = DateTime.Now.AddMonths(3),
                StatutBudget = false,
                NbreLigneBudgetaire = 10
            },
            new BudgetManageModel   
            {
                CodeBudget = "003",
                LibelleBudget = "Budget RH",
                DescriptionBudget = "Budget pour les salaires et formations",
                MontantBudget = 450_000m,
                DateCreationBudget = DateTime.Now.AddDays(-5),
                DateDebutBudget = DateTime.Now,
                DateFinBudget = DateTime.Now.AddMonths(2),
                StatutBudget = true,
                NbreLigneBudgetaire = 7
            },
            new BudgetManageModel
            {
                CodeBudget = "004",
                LibelleBudget = "Budget Opérations",
                DescriptionBudget = "Budget pour les opérations quotidiennes",
                MontantBudget = 300_000m,
                DateCreationBudget = DateTime.Now.AddDays(-15),
                DateDebutBudget = DateTime.Now,
                DateFinBudget = DateTime.Now.AddMonths(1),
                StatutBudget = false,
                NbreLigneBudgetaire = 12
            }
        };

        }
    }
}
