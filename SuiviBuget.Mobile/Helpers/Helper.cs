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
    }
}
