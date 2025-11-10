using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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
    public partial class DeviseManageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Devise> deviseItems;
        IServices service { get; set; }
        public ICommand UtiliseCommand { get; }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    _ = LoadDeviseAsync(_searchText);
                }
            }
        }
        private readonly IAlertService _alertService;

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

        public DeviseManageViewModel()
        {
            string dbPath = Helper.GetDatabaseFullPath();
            service = new Services.Services(dbPath);
            // Chargement initial
            _ = LoadDeviseAsync(string.Empty);
            RegisterMessenger();

            UtiliseCommand = new RelayCommand<Devise>(OnUtilise);
            _alertService = new AlertService();


        }

        private async void OnUtilise(Devise devise)
        {
            var confirm = await Shell.Current.CurrentPage.DisplayAlert("Confirmation", "Voulez-vous changer de devise ?", "Oui", "Non");
            if (!confirm)
                return;

            var devises = await service.GetDeviseItems(string.Empty);
            foreach (var item in devises)
            {
                item.EstActive = false;
                await service.UpdateDeviseStatutAsync(item);
            }
            devise.EstActive = true;
            await service.UpdateDeviseStatutAsync(devise);
            await _alertService.ShowAlertAsync("Information", $"La devise [{devise.LibelleDevise}] a été mise en cours d'utilisation");
            WeakReferenceMessenger.Default.Send(new RefreshList());
        }

        private void RegisterMessenger()
        {
            WeakReferenceMessenger.Default.Register<RefreshList>(this, async (r, m) =>
            {
                await LoadDeviseAsync(SearchText); // Rafraîchit la liste si un ajout est effectué
            });
        }

        private async Task LoadDeviseAsync(string searchText)
        {
            DeviseItems = new ObservableCollection<Devise>();
            IsBusy = true;
            var devises = await service.GetDeviseItems(searchText);

            DeviseItems = new ObservableCollection<Devise>(
                devises.Select(x => new Devise
                {
                    CodeDevise = x.CodeDevise,
                    LibelleDevise = x.LibelleDevise,
                    EstActive = x.EstActive
                }));
            IsBusy = false;
        }
    }
}
