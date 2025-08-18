using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuiviBudget.Core.Constants;

namespace SuiviBudget.Core.Interfaces
{
    public interface INavigationService
    {
        Task NavigateToAsync(string pageName, string code="",string action=GlobalConst.Add);
        Task GoBackAsync();
    }
}
