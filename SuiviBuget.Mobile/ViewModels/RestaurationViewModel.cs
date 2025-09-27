using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SuiviBuget.Mobile.Helpers;

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
        public RestaurationViewModel()
        {

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
                string dbPath = Helper.GetDatabaseFullPath();
                File.Copy(backupPath, dbPath, true);
                IsBusy = false;
                await App.Current.MainPage.DisplayAlert("Succès", "Restauration terminée.", "OK");
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await App.Current.MainPage.DisplayAlert("Erreur", ex.Message, "OK");
            }
        }

    }
}
