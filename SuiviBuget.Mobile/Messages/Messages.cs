using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuiviBuget.Mobile.Messages
{
    internal class Messages
    {
        public record RefreshList;
        public record RefreshListDepense(string CodeLigneBudgetaire);
        public record ResetAppMessage;
        public record BudgetCloturerMessage;
    }
}
