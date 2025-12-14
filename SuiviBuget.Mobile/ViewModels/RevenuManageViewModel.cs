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
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Interfaces;
using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.Services;
using static SuiviBuget.Mobile.Messages.Messages;

namespace SuiviBuget.Mobile.ViewModels
{
    public partial class RevenuManageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<RevenuManageModel> revenuItems;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string totalRevenu;

        [ObservableProperty]
        private int nombreRevenu;

        IServices service { get; set; }
        public ICommand DetailCommand { get; }

        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;

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
                    _ = LoadRevenuAsync(_searchText); // Charge la liste initialement
                }
            }
        }

        public RevenuManageViewModel()
        {
            string dbPath = Helper.GetDatabaseFullPath();
            service = new Services.Services(dbPath);
            _alertService = new AlertService();
            RegisterMessenger(); // Enregistre l'écoute du message
            _ = LoadRevenuAsync(SearchText); // Charge la liste initialement
            _navigationService = new NavigationService();
            DetailCommand = new RelayCommand<RevenuManageModel>(OnDetailCommand);
            ResetAppMessage();
        }

        private async void OnDetailCommand(RevenuManageModel? model)
        {
            if (model == null)
                throw new NotImplementedException();

            await _navigationService.NavigateToAsync("RevenuDetailManageView", model.CodeRevenu);
        }

        private void ResetAppMessage()
        {
            WeakReferenceMessenger.Default.Register<ResetAppMessage>(this, (r, m) =>
            {
                RevenuItems.Clear();
            });
        }

        private void OnDelete(RevenuManageModel? model)
        {
            throw new NotImplementedException();
        }

        private void OnEdit(RevenuManageModel? model)
        {
            throw new NotImplementedException();
        }

        private void RegisterMessenger()
        {
            WeakReferenceMessenger.Default.Register<RefreshList>(this, async (r, m) =>
            {
                await LoadRevenuAsync(SearchText); // Rafraîchit la liste si un ajout est effectué
            });
        }
        private async Task LoadRevenuAsync(string searchText)
        {
            IsBusy = true;
            var devise = await Helper.GetDeviseActiveAsyn();
            var revenus = await service.GetRevenuItems(searchText);        
            RevenuItems = new ObservableCollection<RevenuManageModel>(
             revenus.Select(x => new RevenuManageModel
             {
                 CodeRevenu = x.CodeRevenu,
                 LibelleTypeRevenu = x.LibelleTypeRevenu,
                 Montant = x.Montant,
                 DateDernierMisAJour = x.DateDernierMisAJour,
                 MontantAvecDevise = $"Total : {x.Montant:N0} {devise}"
             }));

            NombreRevenu = RevenuItems.Count();
            TotalRevenu = $"{(RevenuItems.Sum(r => r.Montant)):N0} {devise}";
            IsBusy = false;
        }

    }
}
