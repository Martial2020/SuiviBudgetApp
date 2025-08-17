    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuiviBuget.Mobile.Interfaces
{
    internal interface IAlertService
    {
        Task ShowAlertAsync(string title, string message, string buttonText = "OK");
        //Task ShowAlertChooseAsync(string title, string message, string oui, string non);
    }
}
