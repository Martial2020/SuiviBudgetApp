using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuiviBudget.Mobile.Constants;

namespace SuiviBudget.Mobile.Interfaces
{
    public interface INavigationService
    {
        Task NavigateToAsync(string pageName, string code="",string action=GlobalConst.Add);
        Task GoBackAsync();
    }
}
