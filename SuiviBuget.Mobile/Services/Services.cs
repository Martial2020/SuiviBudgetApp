using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.ApplicationModel;
using SQLite;
using SuiviBudget.Mobile.Constants;
using SuiviBudget.Mobile.Interfaces;
using SuiviBudget.Services.DataAccess;
using SuiviBuget.Mobile.DataAccess;
using SuiviBuget.Mobile.Models;

namespace SuiviBuget.Mobile.Services
{
    public class Services : IService
    {
        private readonly SQLiteAsyncConnection _db;

        #region Constructeur
        public Services(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);
            // _db.DeleteAllAsync<Budget>();
            //_db.DeleteAllAsync<ParametreCompteur>();
            _db.CreateTableAsync<LigneBudgetaire>().Wait();
            _db.CreateTableAsync<Budget>().Wait();
            _db.CreateTableAsync<ParametreCompteur>().Wait();
            _db.CreateTableAsync<BudgetDetail>().Wait();
        }
        #endregion

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
                AddCompteurAsync(newLigne.CodeLigneBudgetaire);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeleteDetailBudgetAsync(LigneBudgetaireModel ligne)
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
                                          .Where(x => x.CodeLigneBudgetaire.ToLower() == codeLigneBudgetaire.ToLower())
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
        public async Task<bool> AddBudgetAsync(BudgetModel budget)
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
                    DateFinBudget = budget.DateFinBudget,
                    //DescriptionBudget = budget.DescriptionBudget,
                    LibelleBudget = budget.LibelleBudget,
                    MontantBudget = budget.MontantBudget,
                    NbreLigneBudgetaire = budget.NbreLigneBudgetaire,
                    StatutBudget = budget.StatutBudget
                };

                await _db.InsertAsync(newLigne);
                await AddCompteurAsync(newLigne.CodeBudget); // bien await
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeleteBudgetAsync(BudgetModel budget)
        {
            var getBudget = await _db.Table<Budget>()
                 .FirstOrDefaultAsync(x => x.CodeBudget == budget.CodeBudget);

            if (getBudget == null)
                return false; // Ligne non trouvée

            await _db.DeleteAsync(getBudget);
            await DeleteBudgetDetailByCodeBudgetAsync(getBudget.CodeBudget);
            return true;
        }
        public async Task<bool> UpdateBudgetAsync(BudgetModel budget)
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
                getBudget.DateFinBudget = budget.DateFinBudget;
                getBudget.LibelleBudget = budget.LibelleBudget;
                getBudget.MontantBudget = budget.MontantBudget;
                getBudget.NbreLigneBudgetaire = budget.NbreLigneBudgetaire;
                getBudget.StatutBudget = budget.StatutBudget;
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
                    DateFinBudget = budgetItem.DateFinBudget,
                    //DescriptionBudget = budgetItem.DescriptionBudget,
                    LibelleBudget = budgetItem.LibelleBudget,
                    MontantBudget = budgetItem.MontantBudget,
                    NbreLigneBudgetaire = budgetItem.NbreLigneBudgetaire,
                     StatutBudget=budgetItem.StatutBudget
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
                        DateFinBudget = budgetItem.DateFinBudget,
                        DescriptionBudget = budgetItem.DescriptionBudget,
                        LibelleBudget = budgetItem.LibelleBudget,
                        MontantBudget = budgetItem.MontantBudget,
                        NbreLigneBudgetaire = budgetItem.NbreLigneBudgetaire,
                        StatutBudget = budgetItem.StatutBudget,
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

        #region BudgetDetail
        public async Task<bool> AddBudgetDetailAsync(BudgetDetailModel detail)
        {
            try
            {
                var data = new BudgetDetail
                {
                    CodeBudget = detail.CodeBudget,
                    CodeLigneBudgetaire = detail.CodeLigneBudgetaire,
                    Montant = (decimal)detail.Montant,
                    BudgetDetailID = Guid.NewGuid()
                };
                await _db.InsertAsync(data);
                MisAjourBudget(data.CodeBudget);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeleteBudgetDetailAsync(BudgetDetailManageModel detail)
        {
            var getDetail = await _db.Table<BudgetDetail>()
                 .FirstOrDefaultAsync(x => x.BudgetDetailID == detail.BudgetDetailID);

            if (getDetail == null)
                return false; // Ligne non trouvée

            await _db.DeleteAsync(getDetail);
            MisAjourBudget(detail.CodeBudget);
            return true;
        }

        public async Task DeleteBudgetDetailByCodeBudgetAsync(string codeBudget)
        {
            var getDetail = await _db.Table<BudgetDetail>()
                 .FirstOrDefaultAsync(x => x.CodeBudget == codeBudget);

            if (getDetail == null)
                return ; // Ligne non trouvée

            await _db.DeleteAsync(getDetail);
        }
        public async Task<bool> UpdateBudgetDetailAsync(BudgetDetailModel detail)
        {
            try
            {
                var getDetail = await _db.Table<BudgetDetail>().FirstOrDefaultAsync(x => x.BudgetDetailID == detail.BudgetDetailID);
                if (getDetail == null) return false;

                getDetail.Montant = detail.Montant;
                getDetail.CodeBudget = detail.CodeBudget;
                getDetail.CodeLigneBudgetaire = detail.CodeLigneBudgetaire;
                await _db.UpdateAsync(getDetail);
                MisAjourBudget(detail.CodeBudget);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la mise à jour: {ex.Message}");
                return false;
            }
        }
        public async Task<BudgetDetail> GetBudgetDetailByCode(Guid detailID)
        {
            try
            {
                var getDetail = await _db.Table<BudgetDetail>()
                .FirstOrDefaultAsync(x => x.BudgetDetailID == detailID);
                return getDetail;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération de la ligne budgétaire par code: {ex.Message}");
                return null;
            }
        }
        public async Task<BudgetDetail> GetBudgetDetailByBudgetLigne(BudgetDetailModel detail)
        {
            try
            {
                var getDetail = await _db.Table<BudgetDetail>()
                .FirstOrDefaultAsync(x => x.CodeBudget == detail.CodeBudget && x.CodeLigneBudgetaire == detail.CodeLigneBudgetaire);
                return getDetail;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération de la ligne budgétaire par code: {ex.Message}");
                return null;
            }
        }
        public async Task<List<BudgetDetailManageModel>> GetBudgetDetailItems(string codeBudget, string searchText)
        {
            try
            {
                var isSearchEmpty = string.IsNullOrWhiteSpace(searchText?.ToLower() ?? "");
                var budgetDetails = await _db.Table<BudgetDetail>().ToListAsync();
                var lignes = await _db.Table<LigneBudgetaire>().ToListAsync();

                var query = (from b in budgetDetails
                             join l in lignes on b.CodeLigneBudgetaire equals l.CodeLigneBudgetaire
                             where (string.IsNullOrEmpty(searchText)
                                    || l.LibelleLigneBudgetaire.ToLower().Contains(searchText.ToLower()))
                                   && b.CodeBudget == codeBudget
                             select new BudgetDetailManageModel
                             {
                                 BudgetDetailID = b.BudgetDetailID,
                                 CodeBudget = b.CodeBudget,
                                 CodeLigneBudgetaire = l.CodeLigneBudgetaire,
                                 LibelleLigneBudgetaire = l.LibelleLigneBudgetaire,
                                 Montant = b.Montant
                             }).ToList();

                return query;
            }
            catch (Exception ex)
            {
                // Log erreur (peut-être un fichier ou un service de journalisation)
                Console.WriteLine($"Erreur lors de la récupération des lignes budgétaires: {ex.Message}");
                return new List<BudgetDetailManageModel>();
            }
        }
        #endregion

        #region ParametreCompteur
        public async Task<string> GetNumeroForCodeEntityAsync(string codeParametre)
        {
            int cpt;
            var compteur = await GetParametreCompteurAsync(codeParametre);
            if (compteur == null)
                cpt = 1;
            else
                cpt = compteur.DernierNumeroEnregistre + 1;

            return $"{codeParametre}-{cpt.ToString("000")}";
        }
        public async Task AddCompteurAsync(string codeBudget)
        {
            var compteur = new ParametreCompteur();
            if (codeBudget.Length > 0)
            {
                var parts = codeBudget.Split('-');
                var data = await GetParametreCompteurAsync(parts[0].Trim());
                if (data == null)
                {
                    compteur.CodeParametre = parts[0].Trim();
                    compteur.DernierNumeroEnregistre = Convert.ToInt32(parts[1].Trim());
                    await _db.InsertAsync(compteur);
                }
                else
                {
                    data.CodeParametre = parts[0].Trim();
                    data.DernierNumeroEnregistre = Convert.ToInt32(parts[1].Trim());
                    await _db.UpdateAsync(data);
                }
            }
        }

        public async Task<ParametreCompteur> GetParametreCompteurAsync(string codeParametre)
        {
            return await _db.Table<ParametreCompteur>()
                    .Where(x => x.CodeParametre == codeParametre)
                    .FirstOrDefaultAsync();
        }
        #endregion

        #region Other Functions
        private async void MisAjourBudget(string codeBudget)
        {
            decimal montant = 0;
            var details = await _db.Table<BudgetDetail>().Where(x => x.CodeBudget == codeBudget).ToListAsync();

            if (details.Any())
                montant = details.Sum(x => x.Montant);

            var budget = await GetBudgetByCode(codeBudget);

            if (budget == null)
                return;

            budget.MontantBudget = montant;
            budget.NbreLigneBudgetaire = details.Count();
            var isUpdate = await UpdateBudgetAsync(budget);
        }
        #endregion
    }
}
