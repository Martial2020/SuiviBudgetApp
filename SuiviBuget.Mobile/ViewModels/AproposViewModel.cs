using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Services;

namespace SuiviBuget.Mobile.ViewModels
{
    public partial class AproposViewModel : ObservableObject
    {
        public string NomApplication => AppInfo.Name;
        public string Version => $"{AppInfo.VersionString}";
        public string Developpeur => "Martial Bleu";

        [ObservableProperty]
        public string licence ;
        public string Statut => "Active ✅";

        [ObservableProperty]
        public DateTime expiration;

        [ObservableProperty]
        public DateTime dateActivation;

        [ObservableProperty]
        public string messageExpiration;


        IServices _service;
        public AproposViewModel()
        {
            var dbPath = Helper.GetDatabaseFullPath();
            _service = new Services.Services(dbPath);
            GetInfoLicence();
        }

        private async void GetInfoLicence()
        {
            var licence = await _service.GetLicence();
            if (licence == null)
                return;

            Expiration = licence.DateExpiration.Value;
            Licence = licence.CodeActivation;
            DateActivation = licence.DateActivation.Value;

            var nbreJours = (Expiration - DateTime.Today).Days;
            MessageExpiration = $"Votre licence expire dans {nbreJours} jour(s).";
        }
    }
}
