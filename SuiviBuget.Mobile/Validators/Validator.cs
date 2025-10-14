using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuiviBudget.Mobile.Constants;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.Services;

namespace SuiviBudge.Validators
{
    public static class Validator
    {

        static string dbPath = Helper.GetDatabaseFullPath();
        static IServices adminService;
        static Validator()
        {
            adminService = new Services(dbPath);
        }

        #region Ligne budgetaire
        public static async Task<(bool isSuccess, string message)> ValidateLigneBugetaireCreate(LigneBudgetaireModel ligneBugetaire)
        {
            if (ligneBugetaire == null)
                return (false, "Aucune donnée disponible pour le type de dépense");

            if (string.IsNullOrEmpty(ligneBugetaire.CodeLigneBudgetaire) || string.IsNullOrEmpty(ligneBugetaire.LibelleLigneBudgetaire))
                return (false, "Veuillez saisir obligatoirement le code ou libellé du type de dépense");

            var getLigne = await adminService.GetLigneBudgetaireByCode(ligneBugetaire.CodeLigneBudgetaire);
            if (getLigne != null)
                return (false, $"Le type de dépense [{ligneBugetaire.CodeLigneBudgetaire}] existe dejà dans notre base de donnée");

            return (true, string.Empty);
        }
        public static async Task<(bool isSuccess, string message)> ValidateLigneBugetaireUpdate(LigneBudgetaireModel ligneBugetaire)
        {

            if (ligneBugetaire == null)
                return (false, "Aucune donnée disponible pour la modification du type de dépense");

            if (string.IsNullOrEmpty(ligneBugetaire.CodeLigneBudgetaire) || string.IsNullOrEmpty(ligneBugetaire.LibelleLigneBudgetaire))
                return (false, "Veuillez saisir obligatoirement le code ou libellé du type de dépense");

            var getLigne = await adminService.GetLigneBudgetaireByCode(ligneBugetaire.CodeLigneBudgetaire);
            if (getLigne == null)
                return (false, "Le type de dépense à modifier n'existe pas dans la base de donnée");

            return (true, string.Empty);
        }
        public static async Task<(bool isSuccess, string message)> ValidateLigneBugetaireDelete(LigneBudgetaireModel ligneBugetaire)
        {
            if (ligneBugetaire == null)
                return (false, "Aucune donnée disponible pour la suppression du type de dépense");

            var getLigne = await adminService.GetLigneBudgetaireByCode(ligneBugetaire.CodeLigneBudgetaire);
            if (getLigne == null)
                return (false, "Le type de dépense à supprimer n'existe pas dans la base de donnée");

            var executionLigne = await adminService.GetBudgetDetailByCodeLigneBudgetaire(ligneBugetaire.CodeLigneBudgetaire);
            if (executionLigne != null)
                return (false, "Impossible de supprimer ce type de dépense , car il appartient dejà a un budget.");


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

            if (budget.MontantBudget <= 0)
                return (false, "Veuillez saisir obligatoirement un montant valide.");

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

            if (budget.MontantBudget <= 0)
                return (false, "Veuillez saisir obligatoirement un montant valide.");

            var getBudget = adminService.GetBudgetByCode(budget.CodeBudget);
            if (getBudget == null)
                return (false, "Le budget à modifier n'existe pas dans la base de donnée");

            return (true, string.Empty);
        }
        public static async Task<(bool isSuccess, string message)> ValidateBudgeteDelete(BudgetManageModel budget)
        {
            if (budget == null)
                return (false, "Aucune donnée disponible pour la suppression du budget");

            var getBudget = await adminService.GetBudgetByCodeBudget(budget.CodeBudget);
            if (getBudget == null)
                return (false, "Le budget à supprimer n'existe pas dans la base de donnée");

            if (getBudget.MontantUtilise > 0)
                return (false, "Impossible de supprimer car ce budget a fait l'objet d'une dépense.Veuillez supprimer ces depense avant la suppression.");

            if (getBudget.StatutBudget == StatutBudgetConst.Cloture)
                return (false, "Impossible de supprimer car ce budget dejà Clôturé");


            return (true, string.Empty);
        }
        #endregion

        #region BudgetDetail
        public static async Task<(bool isSuccess, string message)> ValidateBudgetDetailCreate(BudgetDetailModel ligneBugetaire)
        {
            if (ligneBugetaire == null)
                return (false, "Aucune donnée disponible pour l'ajout des allocations budgétaire'");

            if (string.IsNullOrEmpty(ligneBugetaire.CodeLigneBudgetaire))
                return (false, "Veuillez selectionner le type de dépense");

            if (ligneBugetaire.Montant <= 0)
                return (false, "Veuillez saisir un montant valide");

            var getLigne = await adminService.GetBudgetDetailByCode(ligneBugetaire.BudgetDetailID);
            if (getLigne != null)
                return (false, $"Le type de dépense [{ligneBugetaire.CodeLigneBudgetaire}] existe dejà dans notre base de donnée pour ce budget");

            var budget = await adminService.GetBudgetByCode1(ligneBugetaire.CodeBudget);
            if (budget == null)
                return (false, $"Le budget sur lequel vous souhaitez ajouté ce detail n'existe pas dans notre système.");

            if (budget.MontantNonAlloue < ligneBugetaire.Montant)
                return (false, $"Impossible, Le montant non alloué [{budget.MontantNonAlloue}] disponible est inferieur au montant saisi pour l'allocation.");

            var typeDepense = await adminService.GetExecutionBudgetaireDetailsByBudgetDetail(ligneBugetaire.CodeBudget, ligneBugetaire.CodeLigneBudgetaire);
            if (typeDepense != null)
                return (false, $"Le type de dépense [{ligneBugetaire.CodeLigneBudgetaire}] existe dejà dans notre base de donnée pour ce budget");

            return (true, string.Empty);
        }
        public static async Task<(bool isSuccess, string message)> ValidateBudgetDetailUpdate(BudgetDetailModel ligneBugetaire)
        {
            if (ligneBugetaire == null)
                return (false, "Aucune donnée disponible pour la modification de l'allocation budgétaire");

            if (string.IsNullOrEmpty(ligneBugetaire.CodeLigneBudgetaire))
                return (false, "Veuillez selectionner le type de dépense du budget");

            if (ligneBugetaire.Montant <= 0)
                return (false, "Veuillez saisir un montant valide");

            var getLigne = await adminService.GetBudgetDetailByCode(ligneBugetaire.BudgetDetailID);
            if (getLigne == null)
                return (false, $"Modification impossible.Le type de dépense[{ligneBugetaire.CodeLigneBudgetaire}] existe pas;");

            var budget = await adminService.GetBudgetByCode1(ligneBugetaire.CodeBudget);
            if (budget == null)
                return (false, $"Le budget sur lequel vous souhaitez ajouté ce detail n'existe pas dans notre système.");

            if (budget.MontantNonAlloue < ligneBugetaire.Montant)
                return (false, $"Impossible, Le montant non alloué [{budget.MontantNonAlloue}] disponible est inferieur au montant saisi pour l'allocation.");
            return (true, string.Empty);
        }

        public static async Task<(bool isSuccess, string message)> ValidateBudgetDetailDelete(BudgetDetailManageModel ligneBugetaire)
        {
            if (ligneBugetaire == null)
                return (false, "Aucune donnée disponible pour la suppression de l'allocation du budgétaire");


            var getLigne = await adminService.GetExecutionBudgetaireDetailsByBudgetDetail(ligneBugetaire.CodeBudget, ligneBugetaire.CodeLigneBudgetaire);
            if (getLigne != null)
                return (false, $"Impossible de supprimer, ce detail du budget {ligneBugetaire.CodeBudget} a dejà fait l'objet d'une exécution budgetaire");

            return (true, string.Empty);
        }

        #endregion

        #region Reajustement 
        public static async Task<(bool isSuccess, string message)> ValidateReajustementCreate(ReajusterModel reajustement)
        {
            if (reajustement == null)
                return (false, "Aucune donnée disponible pour l'ajout des allocations budgétaire'");

            var budget = await adminService.GetBudgetByCode1(reajustement.CodeBudget);
            if (budget == null)
                return (false, $"Le budget sur lequel vous souhaitez ajouté ce detail n'existe pas dans notre système.");

            if (budget.MontantNonAlloue < reajustement.Montant)
                return (false, $"Impossible, car le montant non alloué [{budget.MontantNonAlloue}] est inferieur au montant de réajustement");

            return (true, string.Empty);
        }

        public static async Task<(bool isSuccess, string message)> ValidateReajustementDelete(ReajusterManageModel reaj)
        {
            if (reaj == null)
                return (false, "Aucune donnée disponible pour la suppression de ce réajustement");

            var reajustement = await adminService.GetReajustementByCode(reaj.CodeBudget, reaj.CodeLigneBudgetaire);
            if (reajustement == null)
                return (false, "Le réajustement à supprimer n'existe pas dans la base de donnée");

            return (true, string.Empty);
        }

        #endregion

        #region Mode de paiement
        public static async Task<(bool isSuccess, string message)> ValidateModePaiementCreate(ModePaiementModel paiement)
        {
            if (paiement == null)
                return (false, "Aucune donnée disponible pour la creation du mode de paiement");

            if (string.IsNullOrEmpty(paiement.CodeModePaiement) || string.IsNullOrEmpty(paiement.LibelleModePaiement))
                return (false, "Veuillez saisir obligatoirement le code ou libellé du mode paiement");

            var getLigne = await adminService.GetModePaiementByCode(paiement.CodeModePaiement);
            if (getLigne != null)
                return (false, $"Le mode de paiement [{paiement.CodeModePaiement}] existe dejà dans notre base de donnée");

            return (true, string.Empty);
        }
        public static async Task<(bool isSuccess, string message)> ValidateModePaiementUpdate(ModePaiementModel paiement)
        {

            if (paiement == null)
                return (false, "Aucune donnée disponible pour la creation du mode paiement");

            if (string.IsNullOrEmpty(paiement.CodeModePaiement) || string.IsNullOrEmpty(paiement.LibelleModePaiement))
                return (false, "Veuillez saisir obligatoirement le code ou libellé du mode paiement");

            var getLigne = await adminService.GetModePaiementByCode(paiement.CodeModePaiement);
            if (getLigne == null)
                return (false, "Le mode de paiement à modifier n'existe pas dans la base de donnée");

            return (true, string.Empty);
        }
        public static async Task<(bool isSuccess, string message)> ValidateModePaiementDelete(ModePaiementManageModel paiement)
        {
            if (paiement == null)
                return (false, "Aucune donnée disponible pour la suppression du mode paiement");

            var getLigne = await adminService.GetModePaiementByCode(paiement.CodeModePaiement);
            if (getLigne == null)
                return (false, "Le mode de paiement à supprimer n'existe pas dans la base de donnée");

            var paiementExecution = await adminService.GetExecutionBudgetaireDetailsByModePaiement(paiement.CodeModePaiement);
            if (paiementExecution != null)
                return (false, "Impossible de supprimer ce mode de paiement car il a dejà fait l'objet d'une exécution budgetaire");

            return (true, string.Empty);
        }

        #endregion

        #region Execution budgetaire
        public static async Task<(bool isSuccess, string message)> ValidateExecutionBudgetaireDetailCreateAsync(ExecutionBudgetaireDetailModel execution)
        {
            if (execution == null)
                return (false, "Aucune donnée disponible.");

            if (string.IsNullOrEmpty(execution.CodeBudget))
                return (false, "Veuillez choisir un budget");

            if (string.IsNullOrEmpty(execution.CodeLigneBudgetaire))
                return (false, "Veuillez choisir un type de dépense");

            if (string.IsNullOrEmpty(execution.CodeModePaiement))
                return (false, "Veuillez choisir un mode de paiement");

            if (execution.Montant <= 0)
                return (false, "Veuillez saisir un montant valide");

            if (string.IsNullOrEmpty(execution.Description))
                return (false, "Veuillez saisir le détail de votre exécution budgetaire");

            if (execution.DateExecution == DateTime.MinValue)
                return (false, "Veuillez saisir une date valide");

            var typeDepense = await adminService.GetBudgetDetailByBudgetLigne(execution.CodeBudget, execution.CodeLigneBudgetaire);

            if (typeDepense == null)
                return (false, "Cette type de depense n'existe pas dans la liste definie dans notre système.");

            var depense = await adminService.GetExecutionBudgetaireDetailsItems(execution.CodeBudget, execution.CodeLigneBudgetaire);

            if (depense.Count == 0)
            {
                if (typeDepense.Montant < execution.Montant)
                {
                    return (false, $"Impossible, car le montant disponible pour cette depense est [{typeDepense.Montant}], il est inferieur au montant saisi");
                }
            }
            else
            {
                var total = depense.Sum(x => x.Montant) + execution.Montant;
                if (typeDepense.Montant < total)
                {
                    return (false, $"Impossible, car vous avez epuisé votre solde pour ce type de depense.");
                }
            }

            // Utilisation d'await au lieu de .Result
            var budget = await adminService.GetBudgetByCode(execution.CodeBudget);

            if (budget == null) // si getLigne existe déjà, le budget est dupliqué
                return (false, "Le budget choisit n'existe pas");

            if (budget.DateDebutBudget > execution.DateExecution)
                return (false, "Impossible que l'execution du buget soit faite avant la création du budget");

            return (true, string.Empty);
        }

        public static (bool isSuccess, string message) ValidateExecutionBudgetaireDetailDelete(ExecutionBudgetaireDetailModel execution)
        {
            if (execution == null)
                return (false, "Aucune donnée disponible pour la suppression");

            var detail = adminService.GetExecutionBudgetaireDetailsById(execution.ExecutionBudgetaireID);
            if (detail == null)
                return (false, "L'element à supprimer n'existe pas dans la base de donnée");

            return (true, string.Empty);
        }
        #endregion

        #region Statistiques
        public static async Task<(bool isSuccess, string message)> ValidateRechercheAsync(DateTime dateDebut, DateTime dateFin, BudgetManageModel budget)
        {
            if (dateDebut == DateTime.MinValue || dateFin == DateTime.MinValue)
                return (false, "Date saisie est invalide.");

            if (dateDebut > dateFin)
                return (false, "Période definie est incorrecte.");

            return (true, string.Empty);

        }
        #endregion
    }
}
