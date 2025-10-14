using System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Storage;
using SuiviBudget.Mobile.Constants;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Interfaces;
using SuiviBuget.Mobile.Services;
using SuiviBuget.Mobile.Views.Popups;
using static Microsoft.Maui.ApplicationModel.Permissions;

#if ANDROID
using Android.OS;
using Microsoft.Maui.ApplicationModel;
#endif

namespace SuiviBuget.Mobile.ViewModels
{
    public class AppShellViewModel : ObservableObject
    {
        // Commande pour afficher le menu
        public ICommand ShowMenuCommand { get; }
        IDialogService _service;
        IAlertService _alert;
        private readonly INavigationService _navigationService;
        IServices _context;
        public AppShellViewModel()
        {
            _navigationService = new NavigationService();
            _service = new DialogService();
            _alert = new AlertService();
            var dbPath = Helper.GetDatabaseFullPath();
            _context = new Services.Services(dbPath);
            // Initialiser la commande
            ShowMenuCommand = new RelayCommand(OnShowMenuClicked);
        }

        private async void OnShowMenuClicked()
        {
            const string typeDepense = "💰 Type de dépense";
            const string modePaiement = "💳 Mode de paiement";
            const string activationLicence = "🔑 Activation de licence";
            const string backupBD = "💾 Sauvegarder ses données";
            const string restaurationBD = "🗂️ Restaurer ses données";
            const string Apropos = "ℹ️ À propos";
            const string reinitialiser = "🔄 Réinitialiser";
            var options = new List<string>();
            var licence = await _context.GetLicence();
            if (licence == null)
            {
                _alert.ShowAlertAsync("Information", "Aucune licence disponible");
                return;
            }
            if (licence.CodeActivation == GlobalConst.AdministratorCodeActive)
            {
                options = new List<string> { typeDepense, modePaiement, activationLicence, backupBD, restaurationBD, reinitialiser, Apropos };
            }
            else
            {
                options = new List<string> { typeDepense, modePaiement, backupBD, restaurationBD, reinitialiser, Apropos };
            }
            string action = await _service.ShowActionSheet(
                   "⚙️ Paramètres",
                   "Fermer",
                   null,
                   options.ToArray()
               );
            // Gestion de l'action choisie
            switch (action)
            {
                case typeDepense:
                    await _navigationService.NavigateToAsync("LigneBudgetaireManageView");
                    break;

                case modePaiement:
                    await _navigationService.NavigateToAsync("ModePaiementManageView");
                    break;

                case backupBD:

                    await ExportDatabaseAsync();
                    break;

                case restaurationBD:
                    await _navigationService.NavigateToAsync("RestaurationView");
                    break;

                case reinitialiser:
                    await _navigationService.NavigateToAsync("ReinitialiserView");
                    break;
                case activationLicence:
                    await _navigationService.NavigateToAsync("ActivationLicenceManageView");
                    break;
                case Apropos:
                    await _navigationService.NavigateToAsync("AproposView");
                    break;

                default:
                    // Annuler ou fermers
                    break;
            }
            // Crée et affiche le PopUp ici
            //var menuPopup = new PopUpMenuView();
            //await Shell.Current.CurrentPage.ShowPopupAsync(menuPopup);
        }

        public async Task ExportDatabaseAsync()
        {
            var dbPath = Helper.GetDatabaseFullPath();
            var fileName = $"Backup_{DateTime.Now:ddMMyyyyHHmm}.db";
            var result = await FileSaver.Default.SaveAsync(
                fileName,
                new FileStream(dbPath, FileMode.Open, FileAccess.Read),
                new CancellationToken());

            if (result.IsSuccessful)
                await Application.Current.MainPage.DisplayAlert("Succès", $"Sauvegardé dans : {result.FilePath}", "OK");
            else
                await Application.Current.MainPage.DisplayAlert("Erreur", result.Exception?.Message, "OK");

        }

        private async Task BackupData()
        {

            try
            {
                // Demander confirmation
                bool confirm = await Shell.Current.CurrentPage.DisplayAlert(
                    "Confirmation",
                    "Voulez-vous sauvegarder vos données ?",
                    "Oui",
                    "Non"
                );
                if (!confirm) return;

#if ANDROID
        // Demander la permission d'écriture sur le stockage externe
        var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
        if (status != PermissionStatus.Granted)
        {
        await _alert.ShowAlertAsync("Information", $"Autorisation non accordée pour la sauvegarde des données");
            return;
        }

        // Chemin du dossier externe public (Documents ou Downloads)
        string externalPath = Android.OS.Environment
            .GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments)
            .AbsolutePath;

        // Créer ton propre dossier pour les backups
        string myFolder = Path.Combine(externalPath, "MesSauvegardes");
        if (!Directory.Exists(myFolder))
            Directory.CreateDirectory(myFolder);
#else
                // iOS : dossier Documents de l'app
                string myFolder = Path.Combine(FileSystem.AppDataDirectory, "MesSauvegardes");
                if (!Directory.Exists(myFolder))
                    Directory.CreateDirectory(myFolder);
#endif

                // Nom du fichier backup avec horodatage
                string backupFileName = $"Backup_{DateTime.Now:ddMMyyyyHHmm}.db";

                // Chemin complet du fichier backup
                string dbPath = Helper.GetDatabaseFullPath();
                string backupPath = Path.Combine(myFolder, backupFileName);

                // Copier la base SQLite
                File.Copy(dbPath, backupPath, true);
                await _alert.ShowAlertAsync("Information", $"Les données ont été sauvegardées avec succès dans le dossier suivant: {myFolder}");

            }
            catch (Exception ex)
            {
                await _alert.ShowAlertAsync("Erreur", ex.Message);
            }
        }

    }
}

