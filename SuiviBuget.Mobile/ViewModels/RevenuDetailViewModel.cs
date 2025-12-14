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
    public partial class RevenuDetailViewModel : ObservableObject
    {
        [ObservableProperty]
        private RevenuDetailModel dataItem = new();

        [ObservableProperty]
        private string actionAFaire;

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string labelButton;

        [ObservableProperty]
        private string codeRevenu;
        private ModePaiementManageModel _selectedModePaiement;
        public ModePaiementManageModel SelectedModePaiement
        {
            get => _selectedModePaiement;
            set
            {
                _selectedModePaiement = value;
                OnPropertyChanged();
                // mettre à jour le code dans DataItem
                DataItem.CodeModePaiement = value?.CodeModePaiement;
            }
        }



        [ObservableProperty]
        private ObservableCollection<ModePaiementManageModel> modePaiementItems;

        IServices service { get; set; }
        public ICommand SubmitCommand { get; }
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;
        public RevenuDetailViewModel()
        {
            string dbPath = Helper.GetDatabaseFullPath();
            service = new Services.Services(dbPath);
            _navigationService = new NavigationService();
            _alertService = new AlertService();
            SubmitCommand = new RelayCommand(OnSubmitCommand);
        }

        private async void OnSubmitCommand()
        {
            switch (ActionAFaire)
            {
                case GlobalConst.Add:
                    Create();
                    break;
                case GlobalConst.Edit:
                    Update();
                    break;
                default:
                    break;
            }
        }

        private void Update()
        {
            throw new NotImplementedException();
        }

        private async void Create()
        {
            try
            {
                var isValid = await Validator.ValidateSourceDetailCreate(DataItem);
                if (!isValid.isSuccess)
                {
                    await _alertService.ShowAlertAsync("Erreur", isValid.message);
                    return;
                }

                var source = new RevenuDetail
                {
                    CodeModePaiement = DataItem.CodeModePaiement,
                    CodeRevenu = CodeRevenu,
                    DateReception = DataItem.DateReception,
                    Description = DataItem.Description,
                    Montant = DataItem.Montant,
                    RevenuDetailID = Guid.NewGuid()
                };

                var isOk = await service.AddRevenuDetailAsync(source);
                if (!isOk)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Nous rencontrons une erreur lors de l'enregistrement");
                    return;
                }

                await _alertService.ShowAlertAsync("Information", $"La source de la revenue est enregistrée avec succès");

                WeakReferenceMessenger.Default.Send(new RefreshList());
                await _navigationService.GoBackAsync();
            }
            catch (Exception ex)
            {
                await _alertService.ShowAlertAsync("Erreur", ex.Message); return;
            }

        }

        public async Task InitializePageAsync(string code, string action)
        {
            ActionAFaire = action;
            CodeRevenu = code;
            switch (ActionAFaire)
            {
                case GlobalConst.Add:
                    Title = $"Ajouter un revenu";
                    LabelButton = "Ajouter";
                    DataItem.DateReception = DateTime.Now;
                    await LoadModePaiementAsync("");
                    break;
                case GlobalConst.Edit:
                    //Title = "Modifier un detail";
                    //LabelButton = "Modifier";                
                    //var ligne = await adminService.getex
                    //if (ligne == null) return;
                    //DataItem.CodeLigneBudgetaire = ligne.CodeLigneBudgetaire;
                    //DataItem.Montant = ligne.Montant;
                    //DataItem.CodeBudget = ligne.CodeBudget;
                    //CodeLigneBudgetaireIsEnabled = false;
                    //SelectedLigneBudgetaire = LigneBudgetaireItems.FirstOrDefault(l => l.CodeLigneBudgetaire == DataItem.CodeLigneBudgetaire);

                    break;
                default:
                    break;
            }


        }
        private async Task LoadModePaiementAsync(string searchText)
        {
            var ligneItems = await service.GetModePaiementItems(searchText);

            ModePaiementItems = new ObservableCollection<ModePaiementManageModel>(
                ligneItems.Select(x => new ModePaiementManageModel
                {
                    CodeModePaiement = x.CodeModePaiement,
                    LibelleModePaiement = $"[{x.CodeModePaiement}] - {x.LibelleModePaiement}"
                })
            );
        }
    }
}
