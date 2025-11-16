using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
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
    public partial class TypeSourceRevenuViewModel : ObservableObject
    {
        #region Propriete
        [ObservableProperty]
        private TypeRevenuModel dataItem = new();

        [ObservableProperty]
        private string title = "Ajouter un type de revenu";

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

        private bool _CodeLTypeRevenuIsEnabled = true;
        public bool CodeLTypeRevenuIsEnabled
        {
            get => _CodeLTypeRevenuIsEnabled;
            set
            {
                if (_CodeLTypeRevenuIsEnabled != value)
                {
                    _CodeLTypeRevenuIsEnabled = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Interfaces
        IServices service { get; set; }
        public ICommand SubmitCommand { get; }
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;

        #endregion
        public TypeSourceRevenuViewModel()
        {
            string dbPath = Helper.GetDatabaseFullPath();
            service = new Services.Services(dbPath);
            _navigationService = new NavigationService();
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
                    DataItem.CodeTypeRevenu = await service.GetNumeroForCodeEntityAsync(ParametreCompteurConst.TR);
                    CodeLTypeRevenuIsEnabled = false;
                    break;
                case GlobalConst.Edit:
                    Title = "Modifier un type de revenu";
                    LabelButton = "✎ Modifier";
                    var type = await service.GetSourceRevenuByCode(code);
                    if (type == null) { return; }
                    DataItem.CodeTypeRevenu = type.CodeTypeRevenu;
                    DataItem.LibelleTypeRevenu = type.LibelleTypeRevenu;
                    CodeLTypeRevenuIsEnabled = false;
                    break;
                default:
                    break;
            }

            // Le titre de la page

        }

        private async void Create()
        {

            try
            {
                var result = await Validator.ValidateTypeSourceCreate(DataItem);
                if (!result.isSuccess)
                {
                    await _alertService.ShowAlertAsync("Erreur", result.message);
                    return;
                }

                var dataEntity = new TypeRevenu
                {
                    CodeTypeRevenu = DataItem.CodeTypeRevenu,
                    LibelleTypeRevenu = DataItem.LibelleTypeRevenu
                };
                var isOk = await service.AddSourceRevenuAsync(dataEntity);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de l'enregistrement");
                    return;
                }

                await _alertService.ShowAlertAsync("Information", $"Le type de revenu [{dataEntity.LibelleTypeRevenu}] a été créé avec succès");
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
                var result = await Validator.ValidateTypeSourceUpdate(DataItem);
                if (!result.isSuccess)
                {
                    await _alertService.ShowAlertAsync("Erreur", result.message);
                    return;
                }
                var dataEntity = new TypeRevenu
                {
                    CodeTypeRevenu = DataItem.CodeTypeRevenu,
                    LibelleTypeRevenu = DataItem.LibelleTypeRevenu
                };
                var isOk = await service.UpdateSourceRevenuAsync(dataEntity);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de la modification");
                    return;
                }

                await _alertService.ShowAlertAsync("Information", $"Le type de dépense[{dataEntity.LibelleTypeRevenu}] a été modifié avec succès");
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
