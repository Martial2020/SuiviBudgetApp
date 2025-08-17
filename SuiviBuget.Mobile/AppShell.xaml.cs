using SuiviBuget.Mobile.Views;

namespace SuiviBuget.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(LigneBudgetaireManageView), typeof(LigneBudgetaireManageView));
            Routing.RegisterRoute(nameof(ParametreManageView), typeof(ParametreManageView));
            Routing.RegisterRoute(nameof(LigneBudgetaireView), typeof(LigneBudgetaireView));
        }
    }
}
