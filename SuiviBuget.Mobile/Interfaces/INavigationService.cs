using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuiviBudget.Core.Interfaces
{
    public interface INavigationService
    {
        Task NavigateToAsync(string pageName, string code="");
        Task GoBackAsync();
    }
}
