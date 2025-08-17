using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SuiviBudget.Core.Interfaces;
using SuiviBudget.Services.DataAccess;
using SuiviBuget.Mobile.Models;

namespace Budget.Services.Services
{
    public class AdminServices : IAdminService
    {
        private readonly SQLiteAsyncConnection _db;

        public AdminServices(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);
            _db.CreateTableAsync<LigneBudgetaire>().Wait();
        }
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
        public async Task<List<LigneBudgetaireModel>> GetLigneBudgetaireItems()
        {
            try
            {
                var ligneBugetaires = await _db.Table<LigneBudgetaire>().ToListAsync();
                return ligneBugetaires.Select(x => new LigneBudgetaireModel
                {
                    CodeLigneBudgetaire = x.CodeLigneBudgetaire,
                    LibelleLigneBudgetaire = x.LibelleLigneBudgetaire
                }).OrderBy(x =>x.CodeLigneBudgetaire)
                    .ToList();
            }
            catch (Exception ex)
            {
                // Log erreur (peut-être un fichier ou un service de journalisation)
                Console.WriteLine($"Erreur lors de la récupération des lignes budgétaires: {ex.Message}");
                return new List<LigneBudgetaireModel>();
            }
        }
    }
}
