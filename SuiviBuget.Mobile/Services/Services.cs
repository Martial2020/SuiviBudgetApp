using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.ApplicationModel;
using SQLite;
using SuiviBudget.Mobile.Constants;
using SuiviBudget.Mobile.Interfaces;
using SuiviBudget.Services.DataAccess;
using SuiviBuget.Mobile.DataAccess;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Models;

namespace SuiviBuget.Mobile.Services
{
    public class Services : IServices
    {
        #region propriete
        private readonly SQLiteAsyncConnection _db;

        #endregion

        #region Constructeur
        public Services(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);
            //_db.DropTableAsync<Licence>().Wait();      // supprime la table
            _db.CreateTableAsync<ActivationLicence>().Wait();
            _db.CreateTableAsync<Licence>().Wait();
            _db.CreateTableAsync<TypeRevenu>().Wait();
            _db.CreateTableAsync<Devise>().Wait();
            _db.CreateTableAsync<ModePaiement>().Wait();
            _db.CreateTableAsync<LigneBudgetaire>().Wait();
            _db.CreateTableAsync<Budget>().Wait();
            _db.CreateTableAsync<ParametreCompteur>().Wait();
            _db.CreateTableAsync<BudgetDetail>().Wait();
            _db.CreateTableAsync<ExecutionBudgetaire>().Wait();
            _db.CreateTableAsync<Reajustement>().Wait();
        }
        #endregion


        #region Activation de licence
        public async Task<bool> UpdateActivationLicenceAsync(ActivationLicence licence)
        {
            try
            {
                await _db.UpdateAsync(licence);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> AddActivationLicenceAsync(ActivationLicence licence)
        {
            try
            {
                await _db.InsertAsync(licence);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteActivationLicenceAsync(ActivationLicence licence)
        {
            var getLicence = await _db.Table<ActivationLicence>().FirstOrDefaultAsync(x => x.ID == licence.ID);
            if (getLicence == null)
                return false; // Ligne non trouvée

            await _db.DeleteAsync(getLicence);
            return true;
        }
        public async Task<ObservableCollection<ActivationLicence>> GetLicenceItems(string searchText)
        {
            try
            {
                var search = searchText?.ToLower() ?? string.Empty;
                var isSearchEmpty = string.IsNullOrWhiteSpace(search);

                var licences = await _db.Table<ActivationLicence>()
                    .Where(l => isSearchEmpty
                        || l.Identifiant.ToLower().Contains(search)
                        || l.CodeActivation.ToLower().Contains(search))
                    .OrderByDescending(x => x.DateActivation)
                    .ToListAsync();

                return new ObservableCollection<ActivationLicence>(licences);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des licences : {ex.Message}");
                return new ObservableCollection<ActivationLicence>();
            }
        }

        #endregion

        #region Devise
        public Task<bool> AddDeviseAsync(Devise devise)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteDeviseAsync(Devise devise)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateDeviseAsync(ModePaiement devise)
        {
            throw new NotImplementedException();
        }

        public Task<Devise> GetDeviseByCode(string codeDevise)
        {
            throw new NotImplementedException();
        }

        public Task<List<Devise>> GetDeviseItems(string searchText)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Ligne budgetaire   
        public async Task<bool> AddLigneBudgetaireAsync(LigneBudgetaireModel ligne)
        {
            try
            {
                if (ligne == null || string.IsNullOrEmpty(ligne.CodeLigneBudgetaire))
                    throw new ArgumentException("Code de type de dépense est invalide");

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
                    .OrderBy(x => x.LibelleLigneBudgetaire).ToList();

            }
            catch (Exception ex)
            {
                // Log erreur (peut-être un fichier ou un service de journalisation)
                Console.WriteLine($"Erreur lors de la récupération des lignes budgétaires: {ex.Message}");
                return new List<LigneBudgetaireModel>();
            }
        }
        public async Task<List<LigneBudgetaireModel>> GetLigneBudgetaireExclusionItems(string code, string ligne)
        {
            try
            {
                var details = await GetBudgetDetailItems(code, "");
                var codesDetails = details.Select(d => d.CodeLigneBudgetaire).ToList();
                if (!string.IsNullOrEmpty(ligne))
                    codesDetails = details.Where(l => l.CodeLigneBudgetaire != ligne).Select(d => d.CodeLigneBudgetaire).ToList();

                // récupérer tous les codes des détails
                // exclure ces codes de la requête
                var ligneBudgetaires = await _db.Table<LigneBudgetaire>()
                    .Where(l => !codesDetails.Contains(l.CodeLigneBudgetaire)).ToListAsync();

                return ligneBudgetaires
                    .Select(x => new LigneBudgetaireModel
                    {
                        CodeLigneBudgetaire = x.CodeLigneBudgetaire,
                        LibelleLigneBudgetaire = x.LibelleLigneBudgetaire
                    })
                    .OrderBy(x => x.LibelleLigneBudgetaire).ToList();

            }
            catch (Exception ex)
            {
                // Log erreur (peut-être un fichier ou un service de journalisation)
                Console.WriteLine($"Erreur lors de la récupération des lignes budgétaires: {ex.Message}");
                return new List<LigneBudgetaireModel>();
            }
        }
        #endregion

        #region Mode de paiement
        public async Task<bool> AddModePaiementAsync(ModePaiement paiement)
        {
            try
            {
                await _db.InsertAsync(paiement);
                AddCompteurAsync(paiement.CodeModePaiement);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeleteModePaiementAsync(ModePaiement paiement)
        {
            var getPaiement = await _db.Table<ModePaiement>()
                 .FirstOrDefaultAsync(x => x.CodeModePaiement == paiement.CodeModePaiement);

            if (getPaiement == null)
                return false; // Ligne non trouvée

            await _db.DeleteAsync(getPaiement);
            return true;
        }
        public async Task<bool> UpdateModePaiementAsync(ModePaiement paiement)
        {
            try
            {
                var getPaiement = await _db.Table<ModePaiement>()
                    .FirstOrDefaultAsync(x => x.CodeModePaiement == paiement.CodeModePaiement);

                if (getPaiement == null)
                    return false; // Ligne non trouvée

                getPaiement.LibelleModePaiement = paiement.LibelleModePaiement;
                await _db.UpdateAsync(getPaiement);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la mise à jour: {ex.Message}");
                return false;
            }
        }
        public async Task<ModePaiement> GetModePaiementByCode(string codeModePaiement)
        {
            try
            {
                return await _db.Table<ModePaiement>()
                                           .Where(x => x.CodeModePaiement.ToLower() == codeModePaiement.ToLower())
                                           .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération de la ligne budgétaire par code: {ex.Message}");
                return null;
            }
        }
        public async Task<List<ModePaiement>> GetModePaiementItems(string searchText)
        {
            try
            {
                var search = searchText?.ToLower() ?? "";
                var isSearchEmpty = string.IsNullOrWhiteSpace(search);

                return await _db.Table<ModePaiement>()
                 .Where(l => isSearchEmpty
                     || l.CodeModePaiement.ToLower().Contains(searchText)
                     || l.LibelleModePaiement.ToLower().Contains(searchText))
                 .OrderBy(l => l.LibelleModePaiement)   // ✅ tri par libellé
                 .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log erreur (peut-être un fichier ou un service de journalisation)
                Console.WriteLine($"Erreur lors de la récupération des lignes budgétaires: {ex.Message}");
                return new List<ModePaiement>();
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
                    DescriptionBudget = budget.Description,
                    LibelleBudget = budget.LibelleBudget,
                    MontantBudget = budget.MontantBudget,
                    MontantAlloue = 0,
                    MontantNonAlloue = budget.MontantBudget,
                    MontantRestant = 0,
                    MontantUtilise = 0,
                    MontantReajustement = 0,
                    NbreLigneBudgetaire = budget.NbreLigneBudgetaire,
                    StatutBudget = budget.StatutBudget,
                    DateCreation = DateTime.Now
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
        public async Task<bool> DeleteBudgetAsync(Budget budget)
        {

            await DeleteBudgetDetailByCodeBudgetAsync(budget.CodeBudget);
            await DeleteReajustementByCodeBudgetAsync(budget.CodeBudget);
            await _db.DeleteAsync(budget);
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
                //getBudget.DateCreationBudget = budget.DateCreationBudget;
                getBudget.DateDebutBudget = budget.DateDebutBudget;
                getBudget.DateFinBudget = budget.DateFinBudget;
                getBudget.LibelleBudget = budget.LibelleBudget;
                getBudget.MontantBudget = budget.MontantBudget;
                getBudget.DescriptionBudget = budget.Description;
                //getBudget.NbreLigneBudgetaire = budget.NbreLigneBudgetaire;
                getBudget.StatutBudget = budget.StatutBudget;
                await _db.UpdateAsync(getBudget);
                MisAjourBudget(getBudget.CodeBudget);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la mise à jour: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> UpdateBudgetAsync(Budget budget)
        {
            try
            {
                await _db.UpdateAsync(budget);
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
                    Description = budgetItem.DescriptionBudget,
                    LibelleBudget = budgetItem.LibelleBudget,
                    MontantBudget = budgetItem.MontantBudget,
                    NbreLigneBudgetaire = budgetItem.NbreLigneBudgetaire,
                    StatutBudget = budgetItem.StatutBudget,
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération de la ligne budgétaire par code: {ex.Message}");
                return null;
            }
        }
        public async Task<BudgetManageModel> GetBudgetByCode1(string codeBudget)
        {
            try
            {
                var budgetItem = await _db.Table<Budget>()
                                          .Where(x => x.CodeBudget == codeBudget)
                                          .FirstOrDefaultAsync();
                if (budgetItem == null)
                    return null;

                return new BudgetManageModel
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
                    MontantNonAlloue = budgetItem.MontantNonAlloue,
                    MontantAlloue = budgetItem.MontantAlloue,
                    MontantReajustement = budgetItem.MontantReajustement,
                    MontantRestant = budgetItem.MontantRestant,
                    MontantUtilise = budgetItem.MontantUtilise,
                    BackgroundColorStatut = Helper.GetBackgroundColor(budgetItem.StatutBudget)
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération de la ligne budgétaire par code: {ex.Message}");
                return null;
            }
        }

        public async Task<Budget> GetBudgetByCodeBudget(string codeBudget)
        {
            try
            {
                return await _db.Table<Budget>()
                                          .Where(x => x.CodeBudget == codeBudget)
                                          .FirstOrDefaultAsync();
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
                        || l.LibelleBudget.ToLower().Contains(searchText)
                        || l.StatutBudget.ToLower().Contains(searchText)).OrderByDescending(l => l.DateCreation)
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
                        MontantUtilise = budgetItem.MontantUtilise,
                        MontantRestant = budgetItem.MontantRestant,
                        MontantNonAlloue = budgetItem.MontantNonAlloue,
                        MontantAlloue = budgetItem.MontantAlloue,
                        MontantReajustement = budgetItem.MontantReajustement

                    })
                    .OrderByDescending(x => x.DateCreationBudget)
                    .ToList();
            }
            catch (Exception ex)
            {
                // Log erreur (peut-être un fichier ou un service de journalisation)
                Console.WriteLine($"Erreur lors de la récupération des lignes budgétaires: {ex.Message}");
                return new List<BudgetManageModel>();
            }
        }

        public async Task<List<BudgetManageModel>> GetBudgetItemsByStatus(string searchText, List<string> statuts)
        {
            try
            {
                var search = searchText?.ToLower() ?? "";
                var isSearchEmpty = string.IsNullOrWhiteSpace(search);
                var ligneBudgetaires = await _db.Table<Budget>()
                    .Where(l => (isSearchEmpty
                        || l.CodeBudget.ToLower().Contains(search)
                        || l.LibelleBudget.ToLower().Contains(search)
                        || l.StatutBudget.ToLower().Contains(search))
                        && statuts.Contains(l.StatutBudget)).ToListAsync();

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
                        MontantUtilise = budgetItem.MontantUtilise,
                        MontantRestant = budgetItem.MontantRestant,
                        MontantNonAlloue = budgetItem.MontantNonAlloue,
                        MontantAlloue = budgetItem.MontantAlloue,
                        MontantReajustement = budgetItem.MontantReajustement

                    })
                    .OrderByDescending(x => x.DateCreationBudget)
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
                return; // Ligne non trouvée

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
        public async Task<bool> UpdateBudgetDetailAsync(BudgetDetail detail)
        {
            try
            {
                await _db.UpdateAsync(detail);
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

        public async Task<BudgetDetail> GetBudgetDetailByBudgetLigne(string codeBudget, string codeLigneBudgetaire)
        {
            try
            {
                var getDetail = await _db.Table<BudgetDetail>()
                .FirstOrDefaultAsync(x => x.CodeBudget == codeBudget && x.CodeLigneBudgetaire == codeLigneBudgetaire);
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
                var lignes = await _db.Table<LigneBudgetaire>().ToListAsync();
                var budgetDetails = await _db.Table<BudgetDetail>()
                    .Where(b => b.CodeBudget == codeBudget)
                    .ToListAsync();

                var query = (from b in budgetDetails
                             join l in lignes on b.CodeLigneBudgetaire equals l.CodeLigneBudgetaire
                             where (string.IsNullOrEmpty(searchText)
                                    || l.LibelleLigneBudgetaire.ToLower().Contains(searchText.ToLower()))
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

        public async Task<BudgetDetail> GetBudgetDetailByCodeLigneBudgetaire(string codeLigne)
        {
            try
            {
                var getDetail = await _db.Table<BudgetDetail>()
                .FirstOrDefaultAsync(x => x.CodeLigneBudgetaire == codeLigne);
                return getDetail;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération de la ligne budgétaire par code: {ex.Message}");
                return null;
            }
        }
        #endregion

        #region Reajustement
        public async Task<List<ReajusterManageModel>> GetReajustementItems(List<string> codeBudgets, string searchText)
        {
            try
            {
                var isSearchEmpty = string.IsNullOrWhiteSpace(searchText?.ToLower() ?? "");
                var lignes = await _db.Table<LigneBudgetaire>().ToListAsync();
                var budgetDetails = await _db.Table<Reajustement>()
                    .Where(b => codeBudgets.Contains(b.CodeBudget))
                    .ToListAsync();

                var query = (from b in budgetDetails
                             join l in lignes on b.CodeLigneBudgetaire equals l.CodeLigneBudgetaire
                             where (string.IsNullOrEmpty(searchText)
                                    || l.LibelleLigneBudgetaire.ToLower().Contains(searchText.ToLower())
                                    || l.CodeLigneBudgetaire.ToLower().Contains(searchText.ToLower())
                                    )
                             select new ReajusterManageModel
                             {
                                 ReajusterID = b.ReajustementID,
                                 CodeBudget = b.CodeBudget,
                                 CodeLigneBudgetaire = l.CodeLigneBudgetaire,
                                 LibelleLigneBudgetaire = l.LibelleLigneBudgetaire,
                                 Montant = b.Montant,
                                 Motif = b.Motif,
                                 DateReajustement = b.DateReajustement
                             }).ToList();

                return query;
            }
            catch (Exception ex)
            {
                // Log erreur (peut-être un fichier ou un service de journalisation)
                Console.WriteLine($"Erreur lors de la récupération des lignes budgétaires: {ex.Message}");
                return new List<ReajusterManageModel>();
            }
        }
        public async Task<bool> AddReajustementAsync(Reajustement reajustement)
        {
            try
            {
                await _db.InsertAsync(reajustement);
                var typeDepense = await GetBudgetDetailByBudgetLigne(reajustement.CodeBudget, reajustement.CodeLigneBudgetaire);
                typeDepense.Montant += reajustement.Montant;
                var ok = UpdateBudgetDetailAsync(typeDepense);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeleteReajustementAsync(Reajustement reajustement)
        {
            var reaj = await _db.Table<Reajustement>()
                 .FirstOrDefaultAsync(x => x.ReajustementID == reajustement.ReajustementID);

            if (reaj == null)
                return false; // Ligne non trouvée

            await _db.DeleteAsync(reaj);
            var typeDepense = await GetBudgetDetailByBudgetLigne(reajustement.CodeBudget, reajustement.CodeLigneBudgetaire);
            typeDepense.Montant -= reajustement.Montant;
            var ok = UpdateBudgetDetailAsync(typeDepense);
            return true;
        }
        public async Task DeleteReajustementByCodeBudgetAsync(string codeBudget)
        {
            var getDetail = await _db.Table<Reajustement>()
                 .FirstOrDefaultAsync(x => x.CodeBudget == codeBudget);

            if (getDetail == null)
                return; // Ligne non trouvée

            await _db.DeleteAsync(getDetail);
        }
        public async Task<Reajustement> GetReajustementByCode(string codeBudget, string ligne)
        {
            try
            {
                return await _db.Table<Reajustement>()
                 .FirstOrDefaultAsync(x => x.CodeBudget == codeBudget && x.CodeLigneBudgetaire == ligne);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération de la ligne budgétaire par code: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region ExecutionBudgetaire
        public async Task<bool> AddExecutionBudgetaireDetailAsync(ExecutionBudgetaire execution)
        {
            try
            {
                await _db.InsertAsync(execution);
                MisAjourBudget(execution.CodeBudget);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeleteExecutionBudgetaireDetailAsync(ExecutionBudgetaire execution)
        {
            var getDetail = await _db.Table<ExecutionBudgetaire>()
                 .FirstOrDefaultAsync(x => x.ExecutionBudgetaireID == execution.ExecutionBudgetaireID);

            if (getDetail == null)
                return false; // Ligne non trouvée

            await _db.DeleteAsync(getDetail);
            MisAjourBudget(execution.CodeBudget);
            return true;
        }
        public async Task<List<ExecutionBudgetaireDetailManageModel>> GetExecutionBudgetaireDetailsItems(string codeBudget, string ligneBudgetaire)
        {
            try
            {
                var executions = await _db.Table<ExecutionBudgetaire>()
                    .Where(e => e.CodeBudget == codeBudget)
                    .ToListAsync();
                var lignes = await _db.Table<LigneBudgetaire>()
                    .Where(x => x.CodeLigneBudgetaire == ligneBudgetaire).ToListAsync();
                var paiement = await _db.Table<ModePaiement>().ToListAsync();
                var query = (from e in executions
                             join l in lignes on e.CodeLigneBudgetaire equals l.CodeLigneBudgetaire
                             join m in paiement on e.CodeModePaiement equals m.CodeModePaiement
                             orderby e.DateExecution descending, e.DateCreation descending
                             select new ExecutionBudgetaireDetailManageModel
                             {
                                 DateExecution = e.DateExecution,
                                 ExecutionBudgetaireID = e.ExecutionBudgetaireID,
                                 LibelleLigneBudgetaire = l.LibelleLigneBudgetaire,
                                 ModePaiement = m.LibelleModePaiement,
                                 Montant = e.Montant,
                                 CodeLigneBudgetaire = l.CodeLigneBudgetaire,
                                 CodeBudget = e.CodeBudget,
                                 Description = e.Description,
                                 DateCreation = e.DateCreation
                             }).ToList();

                return query;
            }
            catch (Exception ex)
            {
                // Log erreur (peut-être un fichier ou un service de journalisation)
                Console.WriteLine($"Erreur lors de la récupération des lignes budgétaires: {ex.Message}");
                return new List<ExecutionBudgetaireDetailManageModel>();
            }
        }

        public async Task<ExecutionBudgetaire> GetExecutionBudgetaireDetailsById(Guid id)
        {
            try
            {
                return await _db.Table<ExecutionBudgetaire>()
                .FirstOrDefaultAsync(x => x.ExecutionBudgetaireID == id);
            }
            catch (Exception ex)
            {
                // Log erreur (peut-être un fichier ou un service de journalisation)
                Console.WriteLine($"Erreur lors de la récupération des lignes budgétaires: {ex.Message}");
                return new ExecutionBudgetaire();
            }
        }

        public async Task<ExecutionBudgetaire> GetExecutionBudgetaireDetailsByModePaiement(string codeModePaiament)
        {
            try
            {
                return await _db.Table<ExecutionBudgetaire>()
                .FirstOrDefaultAsync(x => x.CodeModePaiement == codeModePaiament);
            }
            catch (Exception ex)
            {
                // Log erreur (peut-être un fichier ou un service de journalisation)
                Console.WriteLine($"Erreur lors de la récupération des lignes budgétaires: {ex.Message}");
                return new ExecutionBudgetaire();
            }
        }

        public async Task<ExecutionBudgetaire> GetExecutionBudgetaireDetailsByBudgetDetail(string codeBudget, string codeLigneBudgetaire)
        {
            try
            {
                return await _db.Table<ExecutionBudgetaire>()
                .FirstOrDefaultAsync(x => x.CodeBudget == codeBudget && x.CodeLigneBudgetaire == codeLigneBudgetaire);
            }
            catch (Exception ex)
            {
                // Log erreur (peut-être un fichier ou un service de journalisation)
                Console.WriteLine($"Erreur lors de la récupération des lignes budgétaires: {ex.Message}");
                return new ExecutionBudgetaire();
            }
        }

        #endregion

        #region Tableau de bord
        public async Task<List<ExecutionBudgetaireDetailManageModel>> GetDepenseItemsByDate(DateTime date, string codeBudget)
        {
            try
            {
                var startDate = date.Date;          // 00:00:00 du jour
                var endDate = date.Date.AddDays(1); // 00:00:00 du jour suivant

                var executions = await _db.Table<ExecutionBudgetaire>()
                 .Where(x => x.DateExecution >= startDate && x.DateExecution < endDate).ToListAsync();
                var lignes = await _db.Table<LigneBudgetaire>().ToListAsync();
                var paiement = await _db.Table<ModePaiement>().ToListAsync();

                var budget = await _db.Table<Budget>()
                    .Where(b => b.CodeBudget == codeBudget).ToListAsync();


                var query = (from e in executions
                             join l in lignes on e.CodeLigneBudgetaire equals l.CodeLigneBudgetaire
                             join m in paiement on e.CodeModePaiement equals m.CodeModePaiement
                             join b in budget on e.CodeBudget equals b.CodeBudget
                             select new ExecutionBudgetaireDetailManageModel
                             {
                                 DateExecution = e.DateExecution,
                                 ExecutionBudgetaireID = e.ExecutionBudgetaireID,
                                 LibelleLigneBudgetaire = l.LibelleLigneBudgetaire,
                                 ModePaiement = m.LibelleModePaiement,
                                 Montant = e.Montant,
                                 CodeLigneBudgetaire = l.CodeLigneBudgetaire,
                                 CodeBudget = e.CodeBudget,
                                 Description = e.Description,
                                 LibelleBudget = b.LibelleBudget,
                                 DateCreation = e.DateCreation
                             }).OrderByDescending(x => x.DateCreation).ToList();
                return query;
            }
            catch (Exception ex)
            {
                // Log erreur (peut-être un fichier ou un service de journalisation)
                Console.WriteLine($"Erreur lors de la récupération des lignes budgétaires: {ex.Message}");
                return new List<ExecutionBudgetaireDetailManageModel>();
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
        public async Task AddCompteurAsync(string code)
        {
            var compteur = new ParametreCompteur();
            if (code.Length > 0)
            {
                var parts = code.Split('-');
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

        #region Statistiques
        public async Task<List<GrapheModel>> GetConsommationByLigneBudgetaire(List<Budget> budgets, DateTime dateDebut, DateTime dateFin)
        {
            var startDate = dateDebut.Date;          // 00:00:00 du jour
            var endDate = dateFin.Date.AddDays(1); // 00:00:00 du jour suivant
            // recupere le code des budgets concerné
            var budgetCodes = budgets.Select(b => b.CodeBudget).ToList();

            var budgetDetails = await _db.Table<BudgetDetail>()
                .Where(d => budgetCodes.Contains(d.CodeBudget)).ToListAsync();

            // Groupement par ligne budgétaire 
            var groupedDetails = budgetDetails
                .GroupBy(d => new { d.CodeLigneBudgetaire })
                .Select(g => new GrapheModel
                {
                    LigneBudgetaire = g.Key.CodeLigneBudgetaire,
                    MontantLigneBudgetaire = (float)g.Sum(x => x.Montant),
                }).ToList();

            // Groupement par ligne budgétaire des depenses
            var ligneCodes = groupedDetails.Select(b => b.LigneBudgetaire).ToList();

            var depenses = await _db.Table<ExecutionBudgetaire>()
           .Where(d => ligneCodes.Contains(d.CodeLigneBudgetaire) && budgetCodes.Contains(d.CodeBudget)
           && d.DateExecution >= startDate && d.DateExecution < endDate).ToListAsync();

            if (depenses.Count == 0) return null;

            var groupedDepense = depenses
               .GroupBy(d => new { d.CodeLigneBudgetaire })
               .Select(g => new GrapheModel
               {
                   LigneBudgetaire = g.Key.CodeLigneBudgetaire,
                   MontantLigneUtilise = (float)g.Sum(x => x.Montant),
               }).ToList();

            var ligneTable = await GetLigneBudgetaireItems("");

            var result = (from d in groupedDetails
                          join e in groupedDepense on d.LigneBudgetaire equals e.LigneBudgetaire
                          join l in ligneTable on d.LigneBudgetaire equals l.CodeLigneBudgetaire

                          select new GrapheModel
                          {
                              MontantLigneUtilise = e.MontantLigneUtilise,
                              LigneBudgetaire = l.LibelleLigneBudgetaire,
                              MontantLigneBudgetaire = d.MontantLigneBudgetaire
                          })
                          .ToList();

            return result;
        }

        public async Task<List<Budget>> GetBudgetItems(DateTime dateDebut, DateTime dateFin, string codeBudget)
        {
            try
            {
                List<Budget> budgets;
                var startDate = dateDebut.Date;          // 00:00:00 du jour
                var endDate = dateFin.Date.AddDays(1); // 00:00:00 du jour suivant
                bool isAll = codeBudget == GlobalConst.CodeTousLesBudgets ? true : false;

                if (!isAll) // si on a fourni un code
                    budgets = await _db.Table<Budget>().Where(x => x.CodeBudget == codeBudget).ToListAsync();

                else // sinon on filtre par période
                    budgets = await _db.Table<Budget>().Where(x => x.DateDebutBudget >= startDate && x.DateDebutBudget < endDate && x.NbreLigneBudgetaire > 0).ToListAsync();


                return budgets;

            }
            catch (Exception ex)
            {
                // Log erreur (peut-être un fichier ou un service de journalisation)
                Console.WriteLine($"Erreur lors de la récupération des lignes budgétaires: {ex.Message}");
                return new List<Budget>();
            }
        }

        public async Task<List<ExecutionBudgetaireDetailManageModel>> GetDepenseItemsByPeriode(DateTime dateDebut, DateTime dateFin, List<Budget> budgets)
        {
            try
            {
                var startDate = dateDebut.Date;          // 00:00:00 du jour
                var endDate = dateFin.Date.AddDays(1); // 00:00:00 du jour suivant

                var executions = await _db.Table<ExecutionBudgetaire>()
                 .Where(x => x.DateExecution >= startDate && x.DateExecution < endDate).ToListAsync();
                var lignes = await _db.Table<LigneBudgetaire>().ToListAsync();
                var paiement = await _db.Table<ModePaiement>().ToListAsync();

                var budget = budgets;


                var query = (from e in executions
                             join l in lignes on e.CodeLigneBudgetaire equals l.CodeLigneBudgetaire
                             join m in paiement on e.CodeModePaiement equals m.CodeModePaiement
                             join b in budgets on e.CodeBudget equals b.CodeBudget
                             select new ExecutionBudgetaireDetailManageModel
                             {
                                 DateExecution = e.DateExecution,
                                 ExecutionBudgetaireID = e.ExecutionBudgetaireID,
                                 LibelleLigneBudgetaire = l.LibelleLigneBudgetaire,
                                 ModePaiement = m.LibelleModePaiement,
                                 Montant = e.Montant,
                                 CodeLigneBudgetaire = l.CodeLigneBudgetaire,
                                 CodeBudget = e.CodeBudget,
                                 Description = e.Description,
                                 LibelleBudget = b.LibelleBudget,
                                 DateCreation = e.DateCreation
                             }).OrderByDescending(x => x.DateCreation).ToList();
                return query;
            }
            catch (Exception ex)
            {
                // Log erreur (peut-être un fichier ou un service de journalisation)
                Console.WriteLine($"Erreur lors de la récupération des lignes budgétaires: {ex.Message}");
                return new List<ExecutionBudgetaireDetailManageModel>();
            }
        }


        #endregion

        #region Licence 
        public async Task<Licence> GetLicence()
        {
            try
            {
                return await _db.Table<Licence>().FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération de la licence par code: {ex.Message}");
                return null;
            }
        }
        public async Task<bool> CreateLicenceAsync(Licence licence)
        {
            try
            {
                await _db.InsertAsync(licence);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateLicenceAsync(Licence licence)
        {
            try
            {

                await _db.UpdateAsync(licence);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteLicenceAsync(Licence licence)
        {
            try
            {

                await _db.DeleteAsync(licence);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout: {ex.Message}");
                return false;
            }
        }


        #endregion

        #region Other Functions
        public async void ReinitialiseApp()
        {
            try
            {
                _db.DeleteAllAsync<ActivationLicence>().Wait();
                _db.DeleteAllAsync<Reajustement>().Wait();
                _db.DeleteAllAsync<ExecutionBudgetaire>().Wait();
                _db.DeleteAllAsync<BudgetDetail>().Wait();
                _db.DeleteAllAsync<BudgetDetail>().Wait();
                _db.DeleteAllAsync<Budget>().Wait();
                _db.DeleteAllAsync<LigneBudgetaire>().Wait();
                _db.DeleteAllAsync<ModePaiement>().Wait();
                _db.DeleteAllAsync<ParametreCompteur>().Wait();


            }
            catch (Exception ex)
            {

            }

        }
        private async void MisAjourBudget(string codeBudget)
        {
            var budget = await GetBudgetByCodeBudget(codeBudget);

            if (budget == null) return;

            var ligneBudgetaire = await _db.Table<BudgetDetail>().Where(x => x.CodeBudget == codeBudget).ToListAsync();
            var depenses = await _db.Table<ExecutionBudgetaire>().Where(x => x.CodeBudget == codeBudget).ToListAsync();
            var reajustement = await _db.Table<Reajustement>().Where(x => x.CodeBudget == codeBudget).ToListAsync();

            //Montant definie pour le budget
            if (ligneBudgetaire.Count() > 0)
            {
                budget.MontantAlloue = ligneBudgetaire.Sum(x => x.Montant);
            }
            else
            {
                budget.MontantAlloue = 0;
            }

            //Montant utilise dans le budget
            if (depenses.Count() > 0)
            {
                budget.MontantUtilise = depenses.Sum(x => x.Montant);
                budget.MontantRestant = budget.MontantAlloue - budget.MontantUtilise;
            }
            else
            {
                budget.MontantUtilise = 0;
                budget.MontantRestant = budget.MontantAlloue;
            }

            if (reajustement.Count() > 0)
            {
                budget.MontantReajustement = reajustement.Sum(x => x.Montant);
            }
            else
            {
                budget.MontantReajustement = 0;
            }
            budget.MontantNonAlloue = budget.MontantBudget - budget.MontantAlloue;
            budget.NbreLigneBudgetaire = ligneBudgetaire.Count();
            var isUpdate = await UpdateBudgetAsync(budget);
        }

        #endregion
    }
}
