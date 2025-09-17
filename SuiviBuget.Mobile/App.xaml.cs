using SQLite;

namespace SuiviBuget.Mobile
{
    public partial class App : Application
    {
        private readonly SQLiteAsyncConnection _db;

        public App()
        {
            InitializeComponent();
            UserAppTheme =AppTheme.Light;

        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
           return new Window(new AppShell());
        }
    }
}