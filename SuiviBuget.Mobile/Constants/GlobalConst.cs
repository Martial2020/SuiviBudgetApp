using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuiviBudget.Mobile.Constants
{
    public static class GlobalConst
    {
        public const string DbPath = "budget.db";
        public const string Add = "Add";
        public const string Edit = "Edit";
        public const string Delete = "Delete";
        public const string Read = "Read";
        public const string CodeTousLesBudgets = "BG-000";
        public const string AccessIllimite = "2411-2509-0807";
        public const string MaCleSecrete = "BleuBantyBrielle";
        public const string AdministratorCodeActive = "Administrator Code Active";       
    }

    public static class StatutBudgetConst
    {
        public const string Ouvert = "Ouvert";
        public const string Cloture = "Clôturé";
        public const string Encours = "En cours";
        public const string Tous = "Tous";
    }

    public static class ParametreCompteurConst
    {
        public const string BG = "BG";
        public const string LB = "TD";
        public const string MP = "MP";
        public const string TR = "TR";
        public const string SR = "SR";
    }
    
}
