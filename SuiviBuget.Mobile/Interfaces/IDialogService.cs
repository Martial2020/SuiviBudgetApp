using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuiviBuget.Mobile.Interfaces
{
    public interface IDialogService
    {
        Task<bool> ShowAlert(string title, string message, string ok, string cancel = null);
        Task<string> ShowActionSheet(string title, string cancel, string destruction, params string[] buttons);
    }

}
