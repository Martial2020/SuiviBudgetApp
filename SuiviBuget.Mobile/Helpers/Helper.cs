using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuiviBudget.Mobile.Constants;

namespace SuiviBuget.Mobile.Helpers
{
    public static class Helper
    {
        public static string GetDatabaseFullPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), GlobalConst.DbPath);
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
    }
}
