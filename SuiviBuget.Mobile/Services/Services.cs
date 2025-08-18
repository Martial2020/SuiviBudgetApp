using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.ApplicationModel;
using SQLite;
using SuiviBudget.Core.Interfaces;
using SuiviBudget.Services.DataAccess;
using SuiviBuget.Mobile.Models;

namespace SuiviBuget.Mobile.Services
{
    public class Services : IService
    {
        private readonly SQLiteAsyncConnection _db;

        public Services(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);
            _db.CreateTableAsync<LigneBudgetaire>().Wait();
            _db.CreateTableAsync<Budget>().Wait();
        }

        #region Ligne budgetaire   
        public async Task<bool> AddLigneBudgetaireAsync(LigneBudgetaireModel ligne)
        {
            try
            {
                if (ligne == null || string.IsNullOrEmpty(ligne.CodeLigneBudgetaire))
                    throw new ArgumentException("Code de la ligne budgétaire est requis");

                var newLigne = new LigneBudgetaire
                {
                    CodeLigneBudgetaire = ligne.CodeLigneBudgetaire,
                    LibelleLigneBudgetaire = ligne.LibelleLigneBudgetaire
                };

                await _db.InsertAsync(newLigne);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeleteLigneBudgetaireAsync(LigneBudgetaireModel ligne)
        {
            var getLigne = await _db.Table<LigneBudgetaire>()
                 .FirstOrDefaultAsync(x => x.CodeLigneBudgetaire == ligne.CodeLigneBudgetaire);

            if (getLigne == null)
                return false; // Ligne non trouvée

            await _db.DeleteAsync(getLigne);
            return true;
        }
        public async Task<bool> UpdateLigneBudgetaireAsync(LigneBudgetaireModel ligne)
        {
            try
            {
                var getLigne = await _db.Table<LigneBudgetaire>()
                    .FirstOrDefaultAsync(x => x.CodeLigneBudgetaire == ligne.CodeLigneBudgetaire);

                if (getLigne == null)
                    return false; // Ligne non trouvée

                getLigne.LibelleLigneBudgetaire = ligne.LibelleLigneBudgetaire;
                await _db.UpdateAsync(getLigne);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la mise à jour: {ex.Message}");
                return false;
            }
        }
        public async Task<LigneBudgetaireModel> GetLigneBudgetaireByCode(string codeLigneBudgetaire)
        {
            try
            {
                var item = await _db.Table<LigneBudgetaire>()
                                          .Where(x => x.CodeLigneBudgetaire == codeLigneBudgetaire)
                                          .FirstOrDefaultAsync();
                if (item == null)
                    return null;

                return new LigneBudgetaireModel
                {
                    CodeLigneBudgetaire = item.CodeLigneBudgetaire,
                    LibelleLigneBudgetaire = item.LibelleLigneBudgetaire
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération de la ligne budgétaire par code: {ex.Message}");
                return null;
            }
        }
        public async Task<List<LigneBudgetaireModel>> GetLigneBudgetaireItems(string searchText)
        {
            try
            {
                var search = searchText?.ToLower() ?? "";
                var isSearchEmpty = string.IsNullOrWhiteSpace(search);

                var ligneBudgetaires = await _db.Table<LigneBudgetaire>()
                    .Where(l => isSearchEmpty
                        || l.CodeLigneBudgetaire.ToLower().Contains(searchText)
                        || l.LibelleLigneBudgetaire.ToLower().Contains(searchText))
                    .ToListAsync();

                return ligneBudgetaires
                    .Select(x => new LigneBudgetaireModel
                    {
                        CodeLigneBudgetaire = x.CodeLigneBudgetaire,
                        LibelleLigneBudgetaire = x.LibelleLigneBudgetaire
                    })
                    .OrderBy(x => x.CodeLigneBudgetaire)
                    .ToList();
            }
            catch (Exception ex)
            {
                // Log erreur (peut-être un fichier ou un service de journalisation)
                Console.WriteLine($"Erreur lors de la récupération des lignes budgétaires: {ex.Message}");
                return new List<LigneBudgetaireModel>();
            }
        }
        #endregion

        #region Budget 
        public async Task<bool> AddBudgetAsync(BudgetManageModel budget)
        {
            try
            {
                if (string.IsNullOrEmpty(budget.CodeBudget))
                    throw new ArgumentException("Code du budget est requis");

                var newLigne = new Budget
                {
                    CodeBudget = budget.CodeBudget,
                    DateCreationBudget = budget.DateCreationBudget,
                    DateDebutBudget = budget.DateDebutBudget,
                    DateDebutFin = budget.DateFinBudget,
                    DescriptionBudget = budget.DescriptionBudget,
                    LibelleBudget = budget.LibelleBudget,
                    MontantBudget = budget.MontantBudget,
                    NbreLigneBudgetaire = budget.NbreLigneBudgetaire
                };

                await _db.InsertAsync(newLigne);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeleteBudgetAsync(BudgetManageModel budget)
        {
            var getBudget = await _db.Table<LigneBudgetaire>()
                 .FirstOrDefaultAsync(x => x.CodeLigneBudgetaire == budget.CodeBudget);

            if (getBudget == null)
                return false; // Ligne non trouvée

            await _db.DeleteAsync(getBudget);
            return true;
        }
        public async Task<bool> UpdateBudgetAsync(BudgetManageModel budget)
        {
            try
            {
                var getBudget = await _db.Table<Budget>()
                    .FirstOrDefaultAsync(x => x.CodeBudget == budget.CodeBudget);

                if (getBudget == null)
                    return false; // Ligne non trouvée
                getBudget.CodeBudget = budget.CodeBudget;
                getBudget.DateCreationBudget = budget.DateCreationBudget;
                getBudget.DateDebutBudget = budget.DateDebutBudget;
                getBudget.DateDebutFin = budget.DateFinBudget;
                getBudget.DescriptionBudget = budget.DescriptionBudget;
                getBudget.LibelleBudget = budget.LibelleBudget;
                getBudget.MontantBudget = budget.MontantBudget;
                getBudget.NbreLigneBudgetaire = budget.NbreLigneBudgetaire;
                await _db.UpdateAsync(getBudget);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la mise à jour: {ex.Message}");
                return false;
            }
        }
        public async Task<BudgetModel> GetBudgetByCode(string codeBudget)
        {
            try
            {
                var budgetItem = await _db.Table<Budget>()
                                          .Where(x => x.CodeBudget == codeBudget)
                                          .FirstOrDefaultAsync();
                if (budgetItem == null)
                    return null;

                return new BudgetModel
                {
                    CodeBudget = budgetItem.CodeBudget,
                    DateCreationBudget = budgetItem.DateCreationBudget,
                    DateDebutBudget = budgetItem.DateDebutBudget,
                    DateDebutFin = budgetItem.DateDebutFin,
                    DescriptionBudget = budgetItem.DescriptionBudget,
                    LibelleBudget = budgetItem.LibelleBudget,
                    MontantBudget = budgetItem.MontantBudget,
                    NbreLigneBudgetaire = budgetItem.NbreLigneBudgetaire
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération de la ligne budgétaire par code: {ex.Message}");
                return null;
            }
        }
        public async Task<List<BudgetManageModel>> GetBudgetItems(string searchText)
        {
            try
            {
                var search = searchText?.ToLower() ?? "";
                var isSearchEmpty = string.IsNullOrWhiteSpace(search);

                var ligneBudgetaires = await _db.Table<Budget>()
                    .Where(l => isSearchEmpty
                        || l.CodeBudget.ToLower().Contains(searchText)
                        || l.LibelleBudget.ToLower().Contains(searchText))
                    .ToListAsync();

                return ligneBudgetaires
                    .Select(budgetItem => new BudgetManageModel
                    {
                        CodeBudget = budgetItem.CodeBudget,
                        DateCreationBudget = budgetItem.DateCreationBudget,
                        DateDebutBudget = budgetItem.DateDebutBudget,
                        DateFinBudget = budgetItem.DateDebutFin,
                        DescriptionBudget = budgetItem.DescriptionBudget,
                        LibelleBudget = budgetItem.LibelleBudget,
                        MontantBudget = budgetItem.MontantBudget,
                        NbreLigneBudgetaire = budgetItem.NbreLigneBudgetaire
                    })
                    .OrderBy(x => x.CodeBudget)
                    .ToList();
            }
            catch (Exception ex)
            {
                // Log erreur (peut-être un fichier ou un service de journalisation)
                Console.WriteLine($"Erreur lors de la récupération des lignes budgétaires: {ex.Message}");
                return new List<BudgetManageModel>();
            }
        }
        #endregion
    }
}
