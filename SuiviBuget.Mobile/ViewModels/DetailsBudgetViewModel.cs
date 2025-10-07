using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Models;

namespace SuiviBuget.Mobile.ViewModels
{
    public partial class DetailsBudgetViewModel : ObservableObject
    {
        [ObservableProperty]
        private BudgetManageModel dataItem = new();
        [ObservableProperty]
        private string title;
        IServices service { get; set; }

        public DetailsBudgetViewModel()
        {
            var dbPath = Helper.GetDatabaseFullPath();
            service = new Services.Services(dbPath);
        }
        public async Task InitializePageAsync(string code, string action)
        {
           DataItem = await service.GetBudgetByCode1(code);
            Title = $"Details {DataItem.CodeBudget}";
        }
    }
}
