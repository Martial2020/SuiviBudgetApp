using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuiviBudget.Services.DataAccess;
using SuiviBuget.Mobile.DataAccess;
using SuiviBuget.Mobile.Models;

namespace SuiviBudget.Mobile.Interfaces
{
    public interface IService
    {
        Task<string> GetNumeroForCodeEntityAsync(string codeParametre);
        #region Ligne budgetaire
        Task<List<LigneBudgetaireModel>> GetLigneBudgetaireItems(string searchText);
        Task<LigneBudgetaireModel> GetLigneBudgetaireByCode(string code);
        Task<bool> AddLigneBudgetaireAsync(LigneBudgetaireModel ligne);
        Task<bool> UpdateLigneBudgetaireAsync(LigneBudgetaireModel ligne);
        Task<bool> DeleteDetailBudgetAsync(LigneBudgetaireModel ligne);
        #endregion

        #region Budget
        Task<bool> AddBudgetAsync(BudgetModel budget);
        Task<bool> DeleteBudgetAsync(BudgetModel budget);
        Task<bool> UpdateBudgetAsync(BudgetModel budget);
        Task<BudgetModel> GetBudgetByCode(string codeBudget);
        Task<List<BudgetManageModel>> GetBudgetItems(string searchText);
        #endregion

        #region BudgetDetail 
        Task<bool> AddBudgetDetailAsync(BudgetDetailModel detail);
        Task<bool> DeleteBudgetDetailAsync(BudgetDetailManageModel detail);
        Task<bool> UpdateBudgetDetailAsync(BudgetDetailModel detail);
        Task<BudgetDetail> GetBudgetDetailByCode(Guid detailID);
        Task<BudgetDetail> GetBudgetDetailByBudgetLigne(BudgetDetailModel detail);

        Task<List<BudgetDetailManageModel>> GetBudgetDetailItems(string codeBudget, string searchText);
        #endregion
    }
}
