using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuiviBuget.Mobile.Interfaces;

namespace SuiviBuget.Mobile.Services
{
    public class DialogService: IDialogService
    {
        public async Task<bool> ShowAlert(string title, string message, string ok, string cancel = null)
        {
            if (cancel == null)
                return false;
            else
                return await Application.Current.MainPage.DisplayAlert(title, message, ok, cancel);
        }

        public async Task<string> ShowActionSheet(string title, string cancel, string destruction, params string[] buttons)
        {
            return await Application.Current.MainPage.DisplayActionSheet(title, cancel, destruction, buttons);
        }
    }
}
