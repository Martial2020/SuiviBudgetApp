using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Services;

namespace SuiviBuget.Mobile.ViewModels
{
    public partial class RestaurationViewModel : ObservableObject
    {
        [ObservableProperty]
        private string selectedFile = "";

        private string backupPath;
        public ICommand PickFileCommand => new AsyncRelayCommand(PickFileAsync);
        public ICommand RestoreCommand => new AsyncRelayCommand(RestoreAsync);
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }

        IServices _service;
        public RestaurationViewModel()
        {
            var context = Helper.GetDatabaseFullPath();
            _service = new Services.Services(context);
        }
        private async Task PickFileAsync()
        {
            try
            {
                var fileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "*/*" } },       // tous les fichiers sur Android
                    { DevicePlatform.iOS, new[] { "public.data" } },  // tous les fichiers sur iOS
                    { DevicePlatform.WinUI, new[] { "*.*" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.data" } }
                });

                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Choisir un fichier backup",
                    FileTypes = fileTypes,
                });

                if (result != null)
                {
                    backupPath = result.FullPath;
                    SelectedFile = backupPath; // affiche le chemin complet
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Erreur", ex.Message, "OK");
            }
        }

        private async Task RestoreAsync()
        {
            IsBusy = true;
            if (string.IsNullOrEmpty(backupPath))
            {
                IsBusy = false;
                await App.Current.MainPage.DisplayAlert("Erreur", "Veuillez d'abord choisir le fichier de donnée à restaurer.", "OK");
                return;
            }

            try
            {
                var licenceEnCours = await _service.GetLicence();
                string dbPath = Helper.GetDatabaseFullPath();
                File.Copy(backupPath, dbPath, true);
                var licenceBackup = await _service.GetLicence();
                if (licenceBackup != null)
                    await _service.DeleteLicenceAsync(licenceBackup);
                if (licenceEnCours != null)
                    await _service.CreateLicenceAsync(licenceEnCours);
                IsBusy = false;
                await App.Current.MainPage.DisplayAlert("Succès", "Restauration terminée. Votre application se fermera, nous vous prions de relancer afin qu'elle prenne en compte les nouvelles données restaurées.", "OK");
                Application.Current.Quit();
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await App.Current.MainPage.DisplayAlert("Erreur", ex.Message, "OK");
            }
        }

    }
}
