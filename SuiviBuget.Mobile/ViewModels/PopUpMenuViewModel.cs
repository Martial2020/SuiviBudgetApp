using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Intuit.Ipp.DataService;
using MvvmHelpers;
using SQLite;
using SuiviBudget.Mobile.Interfaces;
using SuiviBudget.Services.DataAccess;
using SuiviBuget.Mobile.DataAccess;
using SuiviBuget.Mobile.Helpers;

namespace SuiviBuget.Mobile.ViewModels
{
    public class PopUpMenuViewModel : BaseViewModel
    {
        private readonly SQLiteAsyncConnection _db;
        private readonly INavigationService _navigationService;
        public ICommand LigneBudgetCommand { get; }
        public ICommand ModePaiementCommand { get; }
        public ICommand ReinitialiserCommand { get; }
        public Action? ClosePopupAction { get; set; }


        public PopUpMenuViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            LigneBudgetCommand = new RelayCommand(OnLigneBudget);
            ModePaiementCommand = new RelayCommand(OnModePaiement);
            ReinitialiserCommand = new RelayCommand(OnReinitialiser);
        }

        private async void OnReinitialiser()
        {
            var confirm = await Shell.Current.CurrentPage.DisplayAlert(
              "Réinitialisation",
              "Êtes-vous sur(e) de vouloir réinitialiser votre application ?\nToutes vos données seront perdues.",
              "Oui", "Non");

            if (confirm)
            {
                var dbPath = Helper.GetDatabaseFullPath();
                if (File.Exists(dbPath))
                    File.Delete(dbPath);

                // Supprimer les Preferences locales
                Preferences.Clear();


                //#if ANDROID
                //                RestartAppAndroid();
                //#else
                //        //ResetAppUI();
                //#endif





                ClosePopupAction?.Invoke(); // Ferme le popup                                        
                Application.Current.MainPage = new AppShell();

            }
        }

        //private void RestartAppAndroid()
        //{
        //    var context = Android.App.Application.Context;
        //    Intent intent = new Intent(context, typeof(MainActivity));
        //    int pendingIntentId = 123456;
        //    PendingIntent pendingIntent = PendingIntent.GetActivity(context, pendingIntentId, intent, PendingIntentFlags.CancelCurrent);
        //    AlarmManager mgr = (AlarmManager)context.GetSystemService(Context.AlarmService);
        //    mgr.Set(AlarmType.Rtc, Java.Lang.JavaSystem.CurrentTimeMillis() + 100, pendingIntent);

        //    // Ferme l'application
        //    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        //}

        private async void OnLigneBudget()
        {
            ClosePopupAction?.Invoke(); // Ferme le popup
            // Navigation vers la page LigneBudgetPage
            await _navigationService.NavigateToAsync("LigneBudgetaireManageView");
        }

        private async void OnModePaiement()
        {
            ClosePopupAction?.Invoke(); // Ferme le popup
            // Navigation vers la page LigneBudgetPage
            await _navigationService.NavigateToAsync("ModePaiementManageView");
        }

    }
}
