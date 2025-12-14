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
using Intuit.Ipp.WebhooksService;
using SuiviBudge.Validators;
using SuiviBudget.Mobile.Constants;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.DataAccess;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Interfaces;
using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.Services;
using static SuiviBuget.Mobile.Messages.Messages;

namespace SuiviBuget.Mobile.ViewModels
{
    public partial class TypeSourceRevenuManageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<TypeRevenuManageModel> typeRevenuItems;
        IServices adminService { get; set; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand StatusCommand { get; }

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
                    _ = LoadTypeSourceAsync(_searchText); // Charge la liste initialement
                }
            }
        }
        public TypeSourceRevenuManageViewModel()
        {
            string dbPath = Helper.GetDatabaseFullPath();
            adminService = new Services.Services(dbPath);
            _alertService = new AlertService();
            RegisterMessenger(); // Enregistre l'écoute du message
            _ = LoadTypeSourceAsync(SearchText); // Charge la liste initialement
            _navigationService = new NavigationService();
            AddCommand = new RelayCommand(OnAddCommand);
            EditCommand = new RelayCommand<TypeRevenuManageModel>(OnEdit);
            DeleteCommand = new RelayCommand<TypeRevenuManageModel>(OnDelete);
            StatusCommand = new RelayCommand<TypeRevenuManageModel>(OnStatus);
            ResetAppMessage();
        }

        private async void OnStatus(TypeRevenuManageModel item)
        {
            var typeRevenu = await adminService.GetSourceRevenuByCode(item.CodeTypeRevenu);
            if (typeRevenu == null)
            {
                await _alertService.ShowAlertAsync("Erreur", "Cette donnée n'existe pas dans notre système");
                return;
            }
            var revenu = new Revenu
            {
                CodeRevenu = await adminService.GetNumeroForCodeEntityAsync(ParametreCompteurConst.SR),
                CodeTypeRevenu = item.CodeTypeRevenu,
                DateDernierMisAJour = DateTime.Now,
                Montant = 0
            };

            bool confirm;
            if (typeRevenu.EstActive == false)
            {
                confirm = await Shell.Current.CurrentPage.DisplayAlert("Confirmation", "Voulez vous afficher ce type de revenu dans la liste des revenus ?", "Oui", "Non");
                if (!confirm)
                    return;

                var result = await adminService.AddRevenuAsync(revenu);
                if (result)
                {
                    typeRevenu.EstActive = true;
                    _ = await adminService.UpdateSourceRevenuAsync(typeRevenu);
                    await _alertService.ShowAlertAsync("Information", $"La source de revenu [{revenu.CodeRevenu}] a été ajoutée avec succès");
                }
                else
                {
                    await _alertService.ShowAlertAsync("Erreur", $"Nous rencontrons une erreur lors de l'enregistrement");
                }
            }
            else
            {
                confirm = await Shell.Current.CurrentPage.DisplayAlert("Confirmation", "Voulez vous desactiver ce type de revenu dans la liste des revenus ?", "Oui", "Non");
                if (!confirm)
                    return;

                var revenuDetail = await adminService.GetRevenuDetailByCode(item.CodeTypeRevenu);
                if (revenuDetail != null)
                {
                    await _alertService.ShowAlertAsync("Information", $"Impossible de desactive ce type car il contient deja des details dans le menu source de revenu");
                }
                else
                {

                    var result = await adminService.DeleteRevenuAsync(revenu);
                    if (result)
                    {
                        typeRevenu.EstActive = false;
                        _ = await adminService.UpdateSourceRevenuAsync(typeRevenu);
                        await _alertService.ShowAlertAsync("Information", $"Desactivé avec succès");
                    }
                    else
                    {
                        await _alertService.ShowAlertAsync("Erreur", $"Nous rencontrons une erreur lors de la suppression");
                    }
                }

            }

            WeakReferenceMessenger.Default.Send(new RefreshList());

        }
        private void ResetAppMessage()
        {
            WeakReferenceMessenger.Default.Register<ResetAppMessage>(this, (r, m) =>
            {
                typeRevenuItems.Clear();
            });
        }
        private async void OnAddCommand()
        {
            await _navigationService.NavigateToAsync("TypeSourceRevenuView");
        }
        private async void OnEdit(TypeRevenuManageModel item)
        {
            if (string.IsNullOrEmpty(item?.CodeTypeRevenu))
            {
                await _alertService.ShowAlertAsync("Erreur", "Veuillez selectionner une ligne");
                return;
            }
            // Navigation vers la page d'édition avec l'item
            await _navigationService.NavigateToAsync("TypeSourceRevenuView", item?.CodeTypeRevenu, GlobalConst.Edit);
        }
        private async void OnDelete(TypeRevenuManageModel item)
        {
            if (item.EstActive)
            {
                await _alertService.ShowAlertAsync("Erreur", "Impossible de supprimer ce type car il est encours d'utilisation dans les sources de revenu");
                return;
            }
            var confirm = await Shell.Current.CurrentPage.DisplayAlert("Confirmation", "Supprimer cet élément ?", "Oui", "Non");
            if (confirm)
            {
                var result = await Validator.ValidateTypeSourceDelete(item);
                if (!result.isSuccess)
                {
                    await _alertService.ShowAlertAsync("Erreur", result.message);
                    return;
                }
                var entity = new TypeRevenu
                {
                    CodeTypeRevenu = item.CodeTypeRevenu,
                    LibelleTypeRevenu = item.LibelleTypeRevenu
                };
                var isOk = await adminService.DeleteSourceRevenuAsync(entity);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de la suppression");
                    return;
                }

                await _alertService.ShowAlertAsync("Information", $"Le type de source de revenu [{entity.LibelleTypeRevenu}] a été supprimé avec succès");
                WeakReferenceMessenger.Default.Send(new RefreshList());
            }
        }

        private async Task LoadTypeSourceAsync(string searchText)
        {
            //TypeRevenuItems = new ObservableCollection<TypeRevenuManageModel>();
            var revenus = await adminService.GetSourceRevenuItems(searchText);

            TypeRevenuItems = new ObservableCollection<TypeRevenuManageModel>(
                 revenus.Select(x => new TypeRevenuManageModel
                 {
                     CodeTypeRevenu = x.CodeTypeRevenu,
                     LibelleTypeRevenu = x.LibelleTypeRevenu,
                     EstActive = x.EstActive
                 }));
        }

        private void RegisterMessenger()
        {
            WeakReferenceMessenger.Default.Register<RefreshList>(this, async (r, m) =>
            {
                await LoadTypeSourceAsync(SearchText); // Rafraîchit la liste si un ajout est effectué
            });
        }
    }
}
