using SuiviBuget.Mobile.Views;

namespace SuiviBuget.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();         
            Routing.RegisterRoute(nameof(RestaurationView), typeof(RestaurationView));
            Routing.RegisterRoute(nameof(ReinitialiserView), typeof(ReinitialiserView));
            Routing.RegisterRoute(nameof(LigneBudgetaireManageView), typeof(LigneBudgetaireManageView));         
            Routing.RegisterRoute(nameof(LigneBudgetaireView), typeof(LigneBudgetaireView));
            Routing.RegisterRoute(nameof(ModePaiementManageView), typeof(ModePaiementManageView));
            Routing.RegisterRoute(nameof(ModePaiementView), typeof(ModePaiementView));
            Routing.RegisterRoute(nameof(ParametreManageView), typeof(ParametreManageView));
            Routing.RegisterRoute(nameof(BudgetView), typeof(BudgetView));
            Routing.RegisterRoute(nameof(BudgetDetailManageView), typeof(BudgetDetailManageView));
            Routing.RegisterRoute(nameof(BudgetDetailView), typeof(BudgetDetailView));
            Routing.RegisterRoute(nameof(ExecutionBudgetaireManageDetailView), typeof(ExecutionBudgetaireManageDetailView));
            Routing.RegisterRoute(nameof(ExecutionBudgetaireDetailView), typeof(ExecutionBudgetaireDetailView));
        }
    }
}
