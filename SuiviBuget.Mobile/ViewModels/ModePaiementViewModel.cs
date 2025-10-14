using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuiviBudget.Mobile.Interfaces;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SuiviBudget.Mobile.Constants;
using SuiviBuget.Mobile.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using SuiviBuget.Mobile.Helpers;
using SuiviBudge.Validators;
using CommunityToolkit.Mvvm.Messaging;
using static SuiviBuget.Mobile.Messages.Messages;
using SuiviBuget.Mobile.Interfaces;
using SuiviBuget.Mobile.Services;
using static SQLite.SQLite3;
using SuiviBuget.Mobile.DataAccess;

namespace SuiviBuget.Mobile.ViewModels
{
    public partial class ModePaiementViewModel : ObservableObject
    {
        #region Propriete
        [ObservableProperty]
        private ModePaiementModel dataItem = new();

        [ObservableProperty]
        private string title = "Ajouter un mode de paiement";

        [ObservableProperty]
        private string labelButton = "+ Ajouter";

        private string _action;
        public string Action
        {
            get => _action;
            set
            {
                if (_action != value)
                {
                    _action = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsEnabled = true;
        public bool IsEnabled
        {
            get => _IsEnabled;
            set
            {
                if (_IsEnabled != value)
                {
                    _IsEnabled = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Interfaces
        IServices adminService { get; set; }
        public ICommand SubmitCommand { get; }
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;

        #endregion
        public ModePaiementViewModel()
        {
            string dbPath = Helper.GetDatabaseFullPath();
            adminService = new Services.Services(dbPath);
            _navigationService = new NavigationService();
            _alertService = new AlertService();
            SubmitCommand = new RelayCommand(OnSubmitCommand);
        }

        private async void OnSubmitCommand()
        {
            switch (Action)
            {
                case GlobalConst.Add:
                    Create();
                    break;
                case GlobalConst.Edit:
                    Update();
                    break;
                default:
                    await _alertService.ShowAlertAsync("Erreur", "Aucune action.");
                    break;
            }
        }

        public async Task InitializePageAsync(string code, string action)
        {
            Action = action;
            switch (Action)
            {
                case GlobalConst.Add:                  
                    DataItem.CodeModePaiement = await adminService.GetNumeroForCodeEntityAsync(ParametreCompteurConst.MP);
                    IsEnabled = false;
                    break;
                case GlobalConst.Edit:
                    Title = "Modifier un mode de paiement";
                    LabelButton = "✎ Modifier";
                    var modePaiement = await adminService.GetModePaiementByCode(code);
                    if (modePaiement == null) { return; }
                    DataItem.CodeModePaiement = modePaiement.CodeModePaiement;
                    DataItem.LibelleModePaiement = modePaiement.LibelleModePaiement;
                    IsEnabled = false;
                    break;
                default:
                    break;
            }

        }

        private async void Create()
        {      
            try
            {
                var result = await Validator.ValidateModePaiementCreate(DataItem);
                if (!result.isSuccess)
                {
                    await _alertService.ShowAlertAsync("Erreur", result.message);
                    return;
                }

                var dataEntity = new ModePaiement
                {
                    LibelleModePaiement = DataItem.LibelleModePaiement,
                    CodeModePaiement = DataItem.CodeModePaiement
                };

                var isOk = await adminService.AddModePaiementAsync(dataEntity);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de l'enregistrement");
                    return;
                }

                await _alertService.ShowAlertAsync("Information", $"Mode de paiement [{dataEntity.LibelleModePaiement}] a été créée avec succès");
                WeakReferenceMessenger.Default.Send(new RefreshList());
                await _navigationService.GoBackAsync();
            }
            catch (Exception ex)
            {
                await _alertService.ShowAlertAsync("Erreur", ex.Message);
                return;
            }


        }

        private async void Update()
        {
           
            try
            {
                var result = await Validator.ValidateModePaiementUpdate(DataItem);
                if (!result.isSuccess)
                {
                    await _alertService.ShowAlertAsync("Erreur", result.message);
                    return;
                }

                var dataEntity = new ModePaiement
                {
                    LibelleModePaiement = DataItem.LibelleModePaiement,
                    CodeModePaiement = DataItem.CodeModePaiement
                };
                var isOk = await adminService.UpdateModePaiementAsync(dataEntity);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de la modification");
                    return;
                }

                await _alertService.ShowAlertAsync("Information", $"Mode paiement [{dataEntity.LibelleModePaiement}] a été modifiée avec succès");
                WeakReferenceMessenger.Default.Send(new RefreshList());
                await _navigationService.GoBackAsync();
            }
            catch (Exception ex)
            {
                await _alertService.ShowAlertAsync("Erreur", ex.Message);
                return;
            }

        }
    }
}

