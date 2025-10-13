using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.DataAccess;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Interfaces;
using SuiviBuget.Mobile.Services;
using static SuiviBuget.Mobile.Messages.Messages;

namespace SuiviBuget.Mobile.ViewModels
{
    public partial class ActivationLicenceManageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ActivationLicence> licenceItems;

        [ObservableProperty]
        private string searchText;

        IAlertService _alert;
        IServices _service;
        INavigationService _navigationService;
        public ActivationLicenceManageViewModel()
        {
            var dbPath = Helper.GetDatabaseFullPath();
            _service = new Services.Services(dbPath);
            _navigationService = new NavigationService();
            _alert = new AlertService();
            UpdateStatutLicence();
            GetActivationLicence();
            RegisterMessenger();
        }

        private async void UpdateStatutLicence()
        {
            var licences = await _service.GetLicenceItems("");
            foreach (var item in licences)
            {
                if (item.Statut && item.DateExpiration.Value.Date < DateTime.Now.Date)
                {
                    var licence = new ActivationLicence
                    {
                        CodeActivation = item.CodeActivation,
                        DateActivation = item.DateActivation,
                        DateExpiration = item.DateExpiration,
                        ID = item.ID,
                        Identifiant = item.Identifiant,
                        Statut = false
                    };
                    var isOk=_service.UpdateActivationLicenceAsync(licence);
                }
            }
            WeakReferenceMessenger.Default.Send(new RefreshLicenceList());

        }
        private void RegisterMessenger()
        {
            WeakReferenceMessenger.Default.Register<RefreshLicenceList>(this, async (r, m) =>
            {
                GetActivationLicence();
            });
        }
        private async void GetActivationLicence()
        {
            LicenceItems = new ObservableCollection<ActivationLicence>();
            LicenceItems = await _service.GetLicenceItems(SearchText);
        }

        partial void OnSearchTextChanged(string value)
        {
            GetActivationLicence();
        }
        [RelayCommand]
        private async void OpenLicence()
        {
            // Navigation vers la page d'édition avec l'item
            await _navigationService.NavigateToAsync("LicenceView");
        }

        [RelayCommand]
        private async void DeleteLicence(ActivationLicence licence)
        {
            if (licence == null)
            {
                _alert.ShowAlertAsync("Information", "Aucune donnée disponible à supprimer.");
                return;
            }
            var isOk = await _service.DeleteActivationLicenceAsync(licence);
            if (!isOk)
            {
                _alert.ShowAlertAsync("Information", "Nous rencontrons une erreur lors de la suppression.");
                return;
            }
            _alert.ShowAlertAsync("Information", $"La licence [{licence.CodeActivation}] a été supprimer avec succès.");
            WeakReferenceMessenger.Default.Send(new RefreshLicenceList());
        }

        [RelayCommand]
        private async void CopyCodeActivation(ActivationLicence licence)
        {
            if (licence == null)
            {
                _alert.ShowAlertAsync("Information", "Pas de donnée disponible.");
                return;
            }
            Clipboard.Default.SetTextAsync(licence.CodeActivation);
            _alert.ShowAlertAsync("Information", $"La licence [{licence.CodeActivation}] a été copié avec succès.");

        }

    }
}
