using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuiviBudget.Services.DataAccess;
using SuiviBuget.Mobile.DataAccess;
using SuiviBuget.Mobile.Models;

namespace SuiviBudget.Mobile.Interfaces
{
    public interface IServices
    {
        Task<bool> UpdateActivationLicenceAsync(ActivationLicence licence);
        Task<ObservableCollection<ActivationLicence>> GetLicenceItems(string searchText);
        Task<bool> DeleteActivationLicenceAsync(ActivationLicence licence);
        Task<bool> AddActivationLicenceAsync(ActivationLicence licence);
        Task<bool> UpdateLicenceAsync(Licence licence);
        Task<bool> DeleteLicenceAsync(Licence licence);
        Task<bool> CreateLicenceAsync(Licence licence);
        Task<Licence> GetLicence();
        Task<List<GrapheModel>> GetConsommationByLigneBudgetaire(List<Budget> budgets,DateTime dateDebut,DateTime dateFin);
        void ReinitialiseApp();
        Task<List<Budget>> GetBudgetItems(DateTime dateDebut, DateTime dateFin, string codeBudget);
        Task<string> GetNumeroForCodeEntityAsync(string codeParametre);
        #region Ligne budgetaire
        Task<List<LigneBudgetaireModel>> GetLigneBudgetaireItems(string searchText);
        Task<List<LigneBudgetaireModel>> GetLigneBudgetaireExclusionItems(string code,string ligne);
        Task<LigneBudgetaireModel> GetLigneBudgetaireByCode(string code);
        Task<bool> AddLigneBudgetaireAsync(LigneBudgetaireModel ligne);
        Task<bool> UpdateLigneBudgetaireAsync(LigneBudgetaireModel ligne);
        Task<bool> DeleteDetailBudgetAsync(LigneBudgetaireModel ligne);
        #endregion

        #region Budget
        Task<bool> AddBudgetAsync(BudgetModel budget);
        Task<bool> DeleteBudgetAsync(Budget budget);
        Task<bool> UpdateBudgetAsync(BudgetModel budget);
        Task<BudgetModel> GetBudgetByCode(string codeBudget);
        Task<BudgetManageModel> GetBudgetByCode1(string codeBudget);
        Task<List<BudgetManageModel>> GetBudgetItems(string searchText);
        Task<Budget> GetBudgetByCodeBudget(string codeBudget);
        Task<List<BudgetManageModel>> GetBudgetItemsByStatus(string searchText, List<string> statuts);
        #endregion

        #region BudgetDetail 
        Task<bool> AddBudgetDetailAsync(BudgetDetailModel detail);
        Task<bool> DeleteBudgetDetailAsync(BudgetDetailManageModel detail);
        Task<bool> UpdateBudgetDetailAsync(BudgetDetailModel detail);
        Task<BudgetDetail> GetBudgetDetailByCode(Guid detailID);
        Task<BudgetDetail> GetBudgetDetailByBudgetLigne(BudgetDetailModel detail);

        Task<List<BudgetDetailManageModel>> GetBudgetDetailItems(string codeBudget, string searchText);

        Task<BudgetDetail> GetBudgetDetailByCodeLigneBudgetaire(string codeLigne);
        Task<BudgetDetail> GetBudgetDetailByBudgetLigne(string codeBudget, string codeLigneBudgetaire);
        #endregion


        #region Reajustement 
        Task<bool> DeleteReajustementAsync(Reajustement reajustement);
        Task<Reajustement> GetReajustementByCode(string codeBudget,string ligne);
        Task<List<ReajusterManageModel>> GetReajustementItems(List<string> codeBudgets, string searchText);
        Task<bool> AddReajustementAsync(Reajustement reajustement);
        #endregion

        #region Execution budgetaire
        Task<List<ExecutionBudgetaireDetailManageModel>> GetExecutionBudgetaireDetailsItems(string codeBudget,string ligneBudgetaire);
        Task<bool> AddExecutionBudgetaireDetailAsync(ExecutionBudgetaire execution);
        Task<bool> DeleteExecutionBudgetaireDetailAsync(ExecutionBudgetaire execution);
        Task<ExecutionBudgetaire> GetExecutionBudgetaireDetailsById(Guid id);
        Task<ExecutionBudgetaire> GetExecutionBudgetaireDetailsByModePaiement(string codeLigne);
        Task<ExecutionBudgetaire> GetExecutionBudgetaireDetailsByBudgetDetail(string codeBudget, string codeLigneBudgetaire);
        #endregion

        #region Tableau de bord
        Task<List<ExecutionBudgetaireDetailManageModel>> GetDepenseItemsByDate(DateTime date,string codeBudget);
        Task<List<ExecutionBudgetaireDetailManageModel>> GetDepenseItemsByPeriode(DateTime dateDebut,DateTime dateFin, List<Budget> budgets);
        #endregion

        #region Mode de paiement
        Task<bool> AddModePaiementAsync(ModePaiement paiement);
        Task<bool> DeleteModePaiementAsync(ModePaiement paiement);
        Task<bool> UpdateModePaiementAsync(ModePaiement paiement);
        Task<ModePaiement> GetModePaiementByCode(string codeModePaiement);
        Task<List<ModePaiement>> GetModePaiementItems(string searchText);
        #endregion
    }
}
