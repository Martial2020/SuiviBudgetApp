using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using SuiviBudget.Mobile.Constants;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.Services;


namespace SuiviBuget.Mobile.Helpers
{
    public static class Helper
    {
        private static int _colorIndex = 0;

        static string dbPath = Helper.GetDatabaseFullPath();
        static IServices service;
        static Helper()
        {
            service = new Services.Services(dbPath);
        }

        public static async void OpenWhatsApp()
        {
            string phone = "2250779095469"; // mets ton numéro SANS le +
            string url = $"https://wa.me/{phone}";
            await Launcher.OpenAsync(url);
        }

        private static void Notifications()
        {
            _ = PlanifierNotificationsQuotidiennes();
        }
        public static async Task PlanifierNotificationExpirationLicence()
        {
            try
            {
                // Heures des rappels de licence (matin, midi, soir)
                int[] heuresLicence = { 8, 13, 19 };

                // 3️⃣ Récupérer la licence
                var licence = await service.GetLicence(); // ta méthode pour récupérer la licence
                if (licence?.DateExpiration == null)
                    return;

                DateTime aujourdHui = DateTime.Today;
                int joursRestants = (licence.DateExpiration.Value - aujourdHui).Days;

                // 4️⃣ Vérifier si joursRestants correspond aux jours d’avertissement
                int[] joursAvertissement = { 1, 2, 3 };
                if (!joursAvertissement.Contains(joursRestants))
                    return;

                // 5️⃣ Message à envoyer
                string message = $"Ta licence expire dans {joursRestants} jour{(joursRestants > 1 ? "s" : "")}. Pense à la renouveler pour éviter toute interruption.";

                // 6️⃣ Planifier sur 30 jours
                for (int i = 0; i < 30; i++)
                {
                    foreach (var heure in heuresLicence)
                    {
                        DateTime notificationTime = DateTime.Today.AddDays(i).AddHours(heure);

                        // Créer un ID unique par jour + heure
                        int notificationId = int.Parse($"{DateTime.Today.AddDays(i):MMdd}{heure}4000");

                        var notification = new NotificationRequest
                        {
                            NotificationId = notificationId,
                            Title = "⚠️ Avertissement de licence",
                            Description = message,
                            Schedule = new NotificationRequestSchedule
                            {
                                NotifyTime = notificationTime,
                                RepeatType = NotificationRepeat.No // notification unique
                            }
                        };

                        await LocalNotificationCenter.Current.Show(notification);
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur notifications", ex.Message, "OK");
            }
        }

        public static async Task PlanifierNotificationsQuotidiennes()
        {
            try
            {
                var status = await Permissions.RequestAsync<Permissions.PostNotifications>();
                if (status != PermissionStatus.Granted)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Permission requise",
                        "Les notifications sont nécessaires pour te rappeler d'enregistrer tes dépenses.",
                        "OK");
                    return;
                }
                LocalNotificationCenter.Current.CancelAll();
                var messages = new Dictionary<int, string>
                {
                    { 7,  "🌅 Bonjour ! Commence la journée en enregistrant tes revenus et dépenses du matin." },
                    { 10, "⏰ Petit rappel : n'oublie pas d'ajouter toutes tes dépenses et revenus de la matinée." },
                    { 12, "🍽️ Midi : prends un moment pour noter tes dépenses et revenus avant le déjeuner." },
                    { 15, "☕ Après-midi : pense à mettre à jour tes dépenses et revenus du jour." },
                    { 18, "🏠 Fin de journée : enregistre toutes tes dépenses et revenus pour clôturer la journée." },
                    { 21, "🌙 Avant de dormir : assure-toi que toutes tes dépenses et revenus du jour sont enregistrés." },
                    { 23, "🌌 Dernier rappel : vérifie et enregistre toutes tes dépenses et revenus pour aujourd'hui." }
                };


                for (int i = 0; i <= 30; i++)
                {
                    foreach (var heure in messages.Keys)
                    {
                        DateTime notificationTime = DateTime.Today.AddDays(i).AddHours(heure);

                        // Si l’heure est déjà passée, on décale à demain
                        if (notificationTime < DateTime.Now)
                            notificationTime = notificationTime.AddDays(1);

                        // 🔔 4. Crée la notification planifiéea
                        var notification = new NotificationRequest
                        {
                            NotificationId = heure,
                            Title = "Rappel journalier 💡",
                            Description = messages[heure],
                            Schedule = new NotificationRequestSchedule
                            {
                                NotifyTime = notificationTime,
                                RepeatType = NotificationRepeat.Daily,
                            }
                        };

                        await LocalNotificationCenter.Current.Show(notification);
                    }
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
            // 🔐 1. Demande de permission de notification

        }
        public static string GetDatabaseFullPath()
        {
            //return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), GlobalConst.DbPath);
            return Path.Combine(FileSystem.AppDataDirectory, GlobalConst.DbPath);
        }
        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convertit la chaîne en bytes et calcule le hash
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convertit le hash en chaîne hexadécimale
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));

                return builder.ToString();
            }
        }
        public static string GetCodeActivation()
        {
            var codeGenere = Guid.NewGuid().ToString();
            var parts = codeGenere.Split('-');
            return parts[parts.Length - 1].ToUpper().Trim();
        }

        public static Color GetBackgroundColor(string statut)
        {
            if (statut == StatutBudgetConst.Ouvert)
                return Color.FromArgb("#4CAF50");

            if (statut == StatutBudgetConst.Encours)
                return Color.FromArgb("#2196F3");

            if (statut == StatutBudgetConst.Cloture)
                return Color.FromArgb("#FF6347");

            return Colors.Gray;

        }

        public static string GetNextColor()
        {
            // Si on atteint la fin, on boucle
            var color = _palette[_colorIndex % _palette.Count];
            _colorIndex++;
            return color;
        }
        private static readonly List<string> _palette = new()
        {
            "#e6194B", // rouge
            "#3cb44b", // vert
            "#ffe119", // jaune
            "#4363d8", // bleu
            "#f58231", // orange
            "#911eb4", // violet
            "#42d4f4", // turquoise
            "#f032e6", // magenta
            "#bfef45", // vert clair
            "#a9a9a9", // gris

            "#800000", // marron foncé
            "#808000", // olive
            "#000075", // bleu marine
            "#aaffc3", // vert menthe
            "#ffd8b1", // beige
            "#fabed4", // rose clair
            "#dcbeff", // lavande
            "#9A6324", // brun
            "#469990", // teal
            "#000000", // noir

            "#ffe4e1", // rose très pâle
            "#ff7f50", // corail
            "#6495ed", // bleu clair
            "#ff1493", // rose vif
            "#7fff00", // vert citron
            "#00ced1", // cyan foncé
            "#ff8c00", // orange foncé
            "#9932cc", // violet foncé
            "#8b0000", // rouge foncé
            "#20b2aa"  // turquoise foncé
        };
        public static double CalculerHauteurChart()
        {
            var display = DeviceDisplay.MainDisplayInfo;

            // Dimensions logiques (DIPs)
            var screenWidth = display.Width / display.Density;
            var screenHeight = display.Height / display.Density;

            // On prend la plus petite dimension (pour garder le rond correct)
            var minSize = Math.Min(screenWidth, screenHeight);

            // 60% de la plus petite dimension
            return minSize * 0.6;
            //var screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
            //return screenWidth * 0.6;
        }


        #region Cryptage et Decryptage
        // 🔒 Fonction pour crypter un texte avec une clé
        public static string Encrypt(string plainText, string key)
        {
            using (Aes aes = Aes.Create())
            {
                // Génère clé et IV à partir de la clé fournie
                using var sha = SHA256.Create();
                aes.Key = sha.ComputeHash(Encoding.UTF8.GetBytes(key));
                aes.IV = new byte[16]; // IV vide (simple, pas le plus sûr mais pratique)

                using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream();
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        // 🔓 Fonction pour décrypter un texte avec une clé
        public static string Decrypt(string cipherText, string key)
        {
            using (Aes aes = Aes.Create())
            {
                using var sha = SHA256.Create();
                aes.Key = sha.ComputeHash(Encoding.UTF8.GetBytes(key));
                aes.IV = new byte[16]; // même IV que pour le cryptage

                using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                byte[] buffer = Convert.FromBase64String(cipherText);

                using var ms = new MemoryStream(buffer);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }

            #endregion
        }
    }
}
