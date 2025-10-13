using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SuiviBudget.Mobile.Constants;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.DataAccess;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Interfaces;
using SuiviBuget.Mobile.Services;
using static SuiviBuget.Mobile.Messages.Messages;

namespace SuiviBuget.Mobile.ViewModels
{

    public partial class LicenceViewModel : ObservableObject
    {
        [ObservableProperty]
        private ActivationLicence dataItem = new();

        IServices _service;
        INavigationService _navigation;
        IAlertService _alert;
        public LicenceViewModel()
        {
            var dbPath = Helper.GetDatabaseFullPath();
            _service = new Services.Services(dbPath);
            _navigation = new NavigationService();
            _alert = new AlertService();
            DataItem = new ActivationLicence
            {
                DateActivation = DateTime.Now,
                DateExpiration = DateTime.Now
            };
        }

        [RelayCommand]
        public async void Submit()
        {
            if (DataItem == null)
            {
                _alert.ShowAlertAsync("Information", "Aucune donnée disponible");
                return;
            }

            if (string.IsNullOrEmpty(DataItem.Identifiant))
            {
                _alert.ShowAlertAsync("Information", "Veuillez saisir obligatoirement l'identifiant.");
                return;
            }

            if (DataItem.DateActivation > DataItem.DateExpiration)
            {
                _alert.ShowAlertAsync("Information", "La date d'activation ne doit pas etre superieur à la date d'expiration de la licence.");
                return;

            }
            var texteChiffre = $"{DataItem.Identifiant}-{DataItem.DateActivation}-{DataItem.DateExpiration}";
            var codeActivation = Helper.Encrypt(texteChiffre, GlobalConst.MaCleSecrete);
            var data = new ActivationLicence
            {
                ID = Guid.NewGuid(),
                CodeActivation = codeActivation,
                DateExpiration = DataItem.DateExpiration,
                DateActivation = DataItem.DateActivation,
                Identifiant = DataItem.Identifiant,
                Statut = DataItem.DateExpiration.Value.Date > DateTime.Now.Date
            };

            var isOk = await _service.AddActivationLicenceAsync(data);

            if (!isOk)
            {
                _alert.ShowAlertAsync("Information", "Nous rencontrons une erreur lors de la creation de la licence.");
                return;
            }
            WeakReferenceMessenger.Default.Send(new RefreshLicenceList());
            await _navigation.GoBackAsync();
        }

    }
}
