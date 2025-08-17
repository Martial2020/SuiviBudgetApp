using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuiviBuget.Mobile.Interfaces;

namespace SuiviBuget.Mobile.Services
{
    internal class AlertService:IAlertService
    {
        public async Task ShowAlertAsync(string title, string message, string buttonText = "OK")
        {
            // On récupère la page active
            var currentPage = Shell.Current?.CurrentPage;
            if (currentPage != null)
            {
                await currentPage.DisplayAlert(title, message, buttonText);
            }
        }
    }
}
