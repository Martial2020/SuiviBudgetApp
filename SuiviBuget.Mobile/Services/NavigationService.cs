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
                case "LigneBudgetaireManageView":
                    await Shell.Current.GoToAsync(nameof(LigneBudgetaireManageView));
                    break;
                case "ParametreManageView":
                    await Shell.Current.GoToAsync(nameof(ParametreManageView));
                    break;
                case "LigneBudgetaireView":
                    //await Shell.Current.GoToAsync(nameof(LigneBudgetaireView));
                    //await Shell.Current.GoToAsync($"LigneBudgetaireView?Code={code}");
                    await Shell.Current.GoToAsync($"{nameof(LigneBudgetaireView)}?Code={code}&&Action={action}");
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
                    
                default:
                    break;
            }
        }
        public async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }


    }
}
