using SuiviBuget.Mobile.ViewModels;
using SuiviBuget.Mobile.Views;

namespace SuiviBuget.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();        
            Routing.RegisterRoute(nameof(HistoriquePrelevementView), typeof(HistoriquePrelevementView));
            Routing.RegisterRoute(nameof(RevenuDetailView), typeof(RevenuDetailView));
            Routing.RegisterRoute(nameof(RevenuDetailManageView), typeof(RevenuDetailManageView));
            Routing.RegisterRoute(nameof(TypeSourceRevenuView), typeof(TypeSourceRevenuView));
            Routing.RegisterRoute(nameof(TypeSourceRevenuManageView), typeof(TypeSourceRevenuManageView));          
            Routing.RegisterRoute(nameof(DeviseManageView), typeof(DeviseManageView));
            Routing.RegisterRoute(nameof(AproposView), typeof(AproposView));
            Routing.RegisterRoute(nameof(RevenuManageView), typeof(RevenuManageView));
            Routing.RegisterRoute(nameof(LicenceView), typeof(LicenceView));
            Routing.RegisterRoute(nameof(ActivationLicenceManageView), typeof(ActivationLicenceManageView));
            Routing.RegisterRoute(nameof(DetailsBudgetView), typeof(DetailsBudgetView));
            Routing.RegisterRoute(nameof(ReajusterView), typeof(ReajusterView));
            Routing.RegisterRoute(nameof(ReajusterManageView), typeof(ReajusterManageView));
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
