using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.Services;

namespace SuiviBudge.Validators
{
    public static class Validator
    {

        static string dbPath = Helper.GetDatabaseFullPath();
        static IService adminService;
        static Validator()
        {
            adminService = new Services(dbPath);
        }

        #region Ligne budgetaire
        public static async Task<(bool isSuccess, string message)> ValidateLigneBugetaireCreate(LigneBudgetaireModel ligneBugetaire)
        {
            if (ligneBugetaire == null)
                return (false, "Aucune donnée disponible pour la creation de la ligne budgetaire");

            if (string.IsNullOrEmpty(ligneBugetaire.CodeLigneBudgetaire) || string.IsNullOrEmpty(ligneBugetaire.LibelleLigneBudgetaire))
                return (false, "Veuillez saisir obligatoirement le code ou libellé de la ligne budgetaire");

            var getLigne = await adminService.GetLigneBudgetaireByCode(ligneBugetaire.CodeLigneBudgetaire);
            if (getLigne != null)
                return (false, $"La ligne budgetaire [{ligneBugetaire.CodeLigneBudgetaire}] existe dejà dans notre base de donnée");

            return (true, string.Empty);
        }
        public static async Task<(bool isSuccess, string message)> ValidateLigneBugetaireUpdate(LigneBudgetaireModel ligneBugetaire)
        {

            if (ligneBugetaire == null)
                return (false, "Aucune donnée disponible pour la creation de la ligne budgetaire");

            if (string.IsNullOrEmpty(ligneBugetaire.CodeLigneBudgetaire) || string.IsNullOrEmpty(ligneBugetaire.LibelleLigneBudgetaire))
                return (false, "Veuillez saisir obligatoirement le code ou libellé de la ligne budgetaire");

            var getLigne = await adminService.GetLigneBudgetaireByCode(ligneBugetaire.CodeLigneBudgetaire);
            if (getLigne == null)
                return (false, "La ligne budgetaire à modifier n'existe pas dans la base de donnée");

            return (true, string.Empty);
        }
        public static async Task<(bool isSuccess, string message)> ValidateLigneBugetaireDelete(LigneBudgetaireModel ligneBugetaire)
        {
            if (ligneBugetaire == null)
                return (false, "Aucune donnée disponible pour la suppression de la ligne budgetaire");

            var getLigne = await adminService.GetLigneBudgetaireByCode(ligneBugetaire.CodeLigneBudgetaire);
            if (getLigne == null)
                return (false, "La ligne budgetaire à supprimer n'existe pas dans la base de donnée");

            return (true, string.Empty);
        }

        #endregion

        #region Budget
        public static async Task<(bool isSuccess, string message)> ValidateBudgetCreateAsync(BudgetModel budget)
        {
            if (budget == null)
                return (false, "Aucune donnée disponible pour la création du budget");

            if (string.IsNullOrEmpty(budget.LibelleBudget))
                return (false, "Veuillez saisir obligatoirement le libellé du budget");

            // Utilisation d'await au lieu de .Result
            var getLigne = await adminService.GetBudgetByCode(budget.CodeBudget);
            if (getLigne != null) // si getLigne existe déjà, le budget est dupliqué
                return (false, "Le budget existe déjà dans notre base de données");

            if (budget.DateDebutBudget > budget.DateFinBudget)
                return (false, "Période définie est incorrecte");

            if (DateTime.Now.Date > budget.DateFinBudget)
                return (false, "Impossible de créer un budget déjà fermé");

            return (true, string.Empty);
        }
        public static (bool isSuccess, string message) ValidateBudgetUpdate(BudgetModel budget)
        {

            if (budget == null)
                return (false, "Aucune donnée disponible pour la modification du budget");

            if (string.IsNullOrEmpty(budget.LibelleBudget))
                return (false, "Veuillez saisir obligatoirement le libellé du budget");

            var getBudget = adminService.GetBudgetByCode(budget.CodeBudget);
            if (getBudget == null)
                return (false, "Le budget à modifier n'existe pas dans la base de donnée");

            return (true, string.Empty);
        }
        public static (bool isSuccess, string message) ValidateBudgeteDelete(BudgetModel budget)
        {
            if (budget == null)
                return (false, "Aucune donnée disponible pour la suppression de la ligne budgetaire");

            var getBudget = adminService.GetBudgetByCode(budget.CodeBudget);
            if (getBudget == null)
                return (false, "Le budget à supprimer n'existe pas dans la base de donnée");

            return (true, string.Empty);
        }
        #endregion

      
        #region BudgetDetail
        public static async Task<(bool isSuccess, string message)> ValidateBudgetDetailCreate(BudgetDetailModel ligneBugetaire)
        {
            if (ligneBugetaire == null)
                return (false, "Aucune donnée disponible pour la creation de la ligne budgetaire du budget");

            if (string.IsNullOrEmpty(ligneBugetaire.CodeLigneBudgetaire))
                return (false, "Veuillez selectionner la ligne budgetaire du budget");

            if (ligneBugetaire.Montant <= 0)
                return (false, "Veuillez saisir un montant valide");

            var getLigne = await adminService.GetBudgetDetailByCode(ligneBugetaire.BudgetDetailID);
            if (getLigne != null)
                return (false, $"La ligne budgetaire [{ligneBugetaire.CodeLigneBudgetaire}] existe dejà dans notre base de donnée pour ce budget");

            return (true, string.Empty);
        }
        public static async Task<(bool isSuccess, string message)> ValidateBudgetDetailUpdate(BudgetDetailModel ligneBugetaire)
        {
            if (ligneBugetaire == null)
                return (false, "Aucune donnée disponible pour la creation de la ligne budgetaire du budget");

            if (string.IsNullOrEmpty(ligneBugetaire.CodeLigneBudgetaire))
                return (false, "Veuillez selectionner la ligne budgetaire du budget");

            if (ligneBugetaire.Montant <= 0)
                return (false, "Veuillez saisir un montant valide");

            var getLigne = await adminService.GetBudgetDetailByCode(ligneBugetaire.BudgetDetailID);
            if (getLigne == null)
                return (false, $"Modification impossible.La ligne budgetaire [{ligneBugetaire.CodeLigneBudgetaire}] existe pas;");

            return (true, string.Empty);
        }

        #endregion
    }
}
