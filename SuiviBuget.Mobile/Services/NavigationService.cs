using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuiviBudget.Mobile.Constants;
using SuiviBudget.Mobile.Interfaces;

using SuiviBuget.Mobile.Views;

namespace SuiviBuget.Mobile.Services
{
    public class NavigationService : INavigationService
    {
        public async Task NavigateToAsync(string pageName, string code = "", string action = GlobalConst.Add)
        {
            switch (pageName)
            {
                case "RevenuDetailView":
                    await Shell.Current.GoToAsync($"{nameof(RevenuDetailView)}?Code={code}&&Action={action}");
                    break;
                case "RevenuDetailManageView":
                    await Shell.Current.GoToAsync($"{nameof(RevenuDetailManageView)}?Code={code}&&Action={action}");
                    break;
                case "DeviseManageView":
                    await Shell.Current.GoToAsync($"{nameof(DeviseManageView)}");
                    break;
                case "AproposView":
                    await Shell.Current.GoToAsync($"{nameof(AproposView)}");
                    break;
                case "LicenceView":
                    await Shell.Current.GoToAsync($"{nameof(LicenceView)}");
                    break;
                case "ActivationLicenceManageView":
                    await Shell.Current.GoToAsync($"{nameof(ActivationLicenceManageView)}");
                    break;
                case "DetailsBudgetView":
                    await Shell.Current.GoToAsync($"{nameof(DetailsBudgetView)}?Code={code}&&Action={action}");
                    break;
                case "ReajusterView":
                    await Shell.Current.GoToAsync($"{nameof(ReajusterView)}?Code={code}&&Action={action}");
                    break;
                case "ReajusterManageView":
                    await Shell.Current.GoToAsync($"{nameof(ReajusterManageView)}?Code={code}&&Action={action}");
                    break;
                case "RestaurationView":
                    await Shell.Current.GoToAsync(nameof(RestaurationView));
                    break;
                case "ReinitialiserView":
                    await Shell.Current.GoToAsync(nameof(ReinitialiserView));
                    break;

                case "TypeSourceRevenuManageView":
                    await Shell.Current.GoToAsync(nameof(TypeSourceRevenuManageView));
                    break;
                case "LigneBudgetaireManageView":
                    await Shell.Current.GoToAsync(nameof(LigneBudgetaireManageView));
                    break;
                case "LigneBudgetaireView":
                    await Shell.Current.GoToAsync($"{nameof(LigneBudgetaireView)}?Code={code}&&Action={action}");
                    break;
                case "TypeSourceRevenuView":
                    await Shell.Current.GoToAsync($"{nameof(TypeSourceRevenuView)}?Code={code}&&Action={action}");
                    break;
                case "ModePaiementManageView":
                    await Shell.Current.GoToAsync(nameof(ModePaiementManageView));
                    break;
                case "ModePaiementView":
                    await Shell.Current.GoToAsync($"{nameof(ModePaiementView)}?Code={code}&&Action={action}");
                    break;
                case "BudgetView":
                    await Shell.Current.GoToAsync($"{nameof(BudgetView)}?Code={code}&&Action={action}");
                    break;
                case "BudgetDetailManageView":
                    await Shell.Current.GoToAsync($"{nameof(BudgetDetailManageView)}?Code={code}&&Action={action}");
                    break;
                case "BudgetDetailView":
                    await Shell.Current.GoToAsync($"{nameof(BudgetDetailView)}?Code={code}&&Action={action}");
                    break;
                case "ExecutionBudgetaireManageDetailView":
                    await Shell.Current.GoToAsync($"{nameof(ExecutionBudgetaireManageDetailView)}?Code={code}&&Action={action}");
                    break;
                case "ExecutionBudgetaireDetailView":
                    await Shell.Current.GoToAsync($"{nameof(ExecutionBudgetaireDetailView)}?Code={code}&&Action={action}");
                    break;

                default:
                    break;
            }
        }
        public async Task GoBackAsync(string code = "")
        {
            await Shell.Current.GoToAsync("..");
        }


    }
}
