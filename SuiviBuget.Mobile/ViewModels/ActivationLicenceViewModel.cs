using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SuiviBudget.Mobile.Constants;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.DataAccess;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Interfaces;
using SuiviBuget.Mobile.Services;

namespace SuiviBuget.Mobile.ViewModels
{
    partial class ActivationLicenceViewModel : ObservableObject
    {
        [ObservableProperty]
        private Licence dataItem = new();

        public Action? ClosePopupAction { get; set; }

        IServices _service { get; set; }
        private readonly IAlertService _alertService;

        public ActivationLicenceViewModel()
        {
            string dbPath = Helper.GetDatabaseFullPath();
            _service = new Services.Services(dbPath);
            _alertService = new AlertService();

        }



        [RelayCommand]
        private async void Submit()
        {
            try
            {
                if (DataItem == null)
                {
                    await _alertService.ShowAlertAsync("Erreur", "Aucune donnée n'est disponible");
                    return;
                }

                if (string.IsNullOrEmpty(DataItem.CodeActivation))
                {
                    await _alertService.ShowAlertAsync("Erreur", "Veuillez saisir le code de validation afin d'avoir accès à l'application");
                    return;
                }

                if (DataItem.CodeActivation == GlobalConst.AccessIllimite)
                {
                    var dateActivation = DateTime.Now.Date;
                    var identifiant = DataItem.Identifiant;
                    var activation = DataItem.CodeActivation;
                    DataItem = new Licence
                    {
                        Identifiant = identifiant,
                        DateActivation = dateActivation,
                        DateExpiration = dateActivation.AddMonths(12),
                        CodeActivation = GlobalConst.AdministratorCodeActive,
                        IsActive = true
                    };
                }
                else
                {
                    //DataItem.CodeActivation = "+IMJjzR6vRIwuCQ20b+WwI1pFeUtlZ8yPJaatAxxOJYQDPHUEvRiri5UgZFYcYqC";
                    var codeActivationChiffre = DataItem.CodeActivation;
                    var codeActivationDechiffre = Helper.Decrypt(DataItem.CodeActivation, GlobalConst.MaCleSecrete);

                    if (string.IsNullOrEmpty(codeActivationDechiffre))
                    {
                        await _alertService.ShowAlertAsync("Erreur", "Code d'activaition dechiffré est invalide.");
                        return;
                    }
                    var parts = codeActivationDechiffre.Split('-');
                    if (parts.Length < 3)
                    {
                        await _alertService.ShowAlertAsync("Erreur", "Le code dechiffré est incorrecte.");
                        return;
                    }

                    var identifiantDechiffre = parts[0].Trim();
                    var DateActivationDechiffre = parts[1].Trim();
                    var DateExpirationDechiffre = parts[2].Trim();

                    if (DataItem.Identifiant != identifiantDechiffre)
                    {
                        await _alertService.ShowAlertAsync("Erreur", "Idenitifiant du telephone est different de l'identifiant dechiffré.");
                        return;
                    }
                    if (Convert.ToDateTime(DateExpirationDechiffre).Date < DateTime.Now.Date)
                    {
                        await _alertService.ShowAlertAsync("Erreur", "Cette licence est dejà expiré.Veuillez contacter l'administrateur.");
                        return;
                    }

                    DataItem = new Licence
                    {
                        Identifiant = identifiantDechiffre,
                        DateActivation = Convert.ToDateTime(DateActivationDechiffre).Date,
                        DateExpiration = Convert.ToDateTime(DateExpirationDechiffre).Date,
                        CodeActivation = codeActivationChiffre,
                        IsActive = true
                    };
                }
                ClosePopupAction?.Invoke(); // Ferme le popup
                await _service.UpdateLicenceAsync(DataItem);
                await _alertService.ShowAlertAsync("Information", "Licence activée avec succès !!!");
            }
            catch (Exception ex)
            {

                await _alertService.ShowAlertAsync("Erreur", "Votre code d'activation saisi est invalide.Veuillez contacter l'administrateur.");
            }

        }

        [RelayCommand]
        private async void CopyId()
        {
            if (DataItem == null)
            {
                await _alertService.ShowAlertAsync("Erreur", "Aucune donnée n'est disponible");
                return;
            }
            if (string.IsNullOrEmpty(DataItem.Identifiant))
            {
                await _alertService.ShowAlertAsync("Erreur", "Votre identifiant n'existe pas alors copie impossible");
                return;
            }
            Clipboard.Default.SetTextAsync(DataItem.Identifiant);
            await _alertService.ShowAlertAsync("Erreur", "Identifiant copié avec succès");

        }
    }
}
