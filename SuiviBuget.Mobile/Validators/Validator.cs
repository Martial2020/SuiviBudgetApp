using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budget.Services.Services;
using SuiviBudget.Core.Interfaces;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Models;

namespace SuiviBudge.Validators
{
    public static class Validator
    {

        static string dbPath = Helper.GetDatabaseFullPath();
        static IAdminService adminService;
        static Validator()
        {
            adminService = new AdminServices(dbPath);
        }

        #region Ligne budgetaire
        public static (bool isSuccess, string message) ValidateLigneBugetaireCreate(LigneBudgetaireModel ligneBugetaire)
        {

            if (ligneBugetaire == null)
                return (false, "Aucune donnée disponible pour la creation de la ligne budgetaire");

            if (string.IsNullOrEmpty(ligneBugetaire.CodeLigneBudgetaire) || string.IsNullOrEmpty(ligneBugetaire.LibelleLigneBudgetaire))
                return (false, "Veuillez saisir obligatoirement le code ou libellé de la ligne budgetaire");

            return (true, string.Empty);
        }
        public static (bool isSuccess, string message) ValidateLigneBugetaireUpdate(LigneBudgetaireModel ligneBugetaire)
        {

            if (ligneBugetaire == null)
                return (false, "Aucune donnée disponible pour la creation de la ligne budgetaire");

            if (string.IsNullOrEmpty(ligneBugetaire.CodeLigneBudgetaire) || string.IsNullOrEmpty(ligneBugetaire.LibelleLigneBudgetaire))
                return (false, "Veuillez saisir obligatoirement le code ou libellé de la ligne budgetaire");

            var getLigne = adminService.GetLigneBudgetaireByCode(ligneBugetaire.CodeLigneBudgetaire);
            if (getLigne == null)
                return (false, "La ligne budgetaire à modifier n'existe pas dans la base de donnée");

            return (true, string.Empty);
        }
        public static (bool isSuccess, string message) ValidateLigneBugetaireDelete(LigneBudgetaireModel ligneBugetaire)
        {

            if (ligneBugetaire == null)
                return (false, "Aucune donnée disponible pour la suppression de la ligne budgetaire");

            var getLigne = adminService.GetLigneBudgetaireByCode(ligneBugetaire.CodeLigneBudgetaire);
            if (getLigne == null)
                return (false, "La ligne budgetaire à supprimer n'existe pas dans la base de donnée");

            return (true, string.Empty);
        }
        #endregion
    }
}
