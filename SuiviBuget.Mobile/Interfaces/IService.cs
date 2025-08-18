using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuiviBuget.Mobile.Models;

namespace SuiviBudget.Core.Interfaces
{
    public interface IService
    {
        #region Ligne budgetaire
        Task<List<LigneBudgetaireModel>> GetLigneBudgetaireItems(string searchText);
        Task<LigneBudgetaireModel> GetLigneBudgetaireByCode(string code);
        Task<bool> AddLigneBudgetaireAsync(LigneBudgetaireModel ligne);
        Task<bool> UpdateLigneBudgetaireAsync(LigneBudgetaireModel ligne);
        Task<bool> DeleteLigneBudgetaireAsync(LigneBudgetaireModel ligne);
        #endregion
    }
}
