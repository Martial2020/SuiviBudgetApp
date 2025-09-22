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
        private static int _colorIndex = 0;

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

    }
}
