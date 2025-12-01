using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GrammarFixer.Models;
using GrammarFixer.Services;

namespace GrammarFixer
{
    public partial class UsageWindow : Window
    {
        private SettingsService _settingsService;
        private AppSettings? _settings; 

        public UsageWindow()
        {
            InitializeComponent();
            _settingsService = new SettingsService();
            LoadUsageData();
        }

        private void LoadUsageData()
        {
            _settings = _settingsService.LoadSettings();
            

            PriceBox.TextChanged -= PriceBox_TextChanged;
            PriceBox.Text = _settings.TokenPricePerMillion.ToString("F2");
            PriceBox.TextChanged += PriceBox_TextChanged;
            
            UpdateStats();
        }

        private void UpdateStats()
        {
            if (_settings == null) return;

            var now = DateTime.Now;
            var today = _settings.TokenUsageHistory.Where(u => u.Timestamp.Date == now.Date);
            var week = _settings.TokenUsageHistory.Where(u => u.Timestamp >= now.AddDays(-7));
            var month = _settings.TokenUsageHistory.Where(u => u.Timestamp >= now.AddDays(-30));

            int todayTotal = today.Sum(u => u.TotalTokens);
            int weekTotal = week.Sum(u => u.TotalTokens);
            int monthTotal = month.Sum(u => u.TotalTokens);
            int allTimeTotal = _settings.TokenUsageHistory.Sum(u => u.TotalTokens);

            double pricePerToken = _settings.TokenPricePerMillion / 1000000.0;

            TodayTokens.Text = $"{todayTotal:N0} tokens";
            TodayCost.Text = $"${(todayTotal * pricePerToken):F4}";

            WeekTokens.Text = $"{weekTotal:N0} tokens";
            WeekCost.Text = $"${(weekTotal * pricePerToken):F4}";

            MonthTokens.Text = $"{monthTotal:N0} tokens";
            MonthCost.Text = $"${(monthTotal * pricePerToken):F4}";

            TotalTokens.Text = $"Total Tokens: {allTimeTotal:N0}";
            TotalCost.Text = $"Total Cost: ${(allTimeTotal * pricePerToken):F4}";

            UsageList.ItemsSource = _settings.TokenUsageHistory.OrderByDescending(u => u.Timestamp).Take(20).ToList();
        }

        private void PriceBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_settings == null) return;

            if (double.TryParse(PriceBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
            {
                _settings.TokenPricePerMillion = price;
                _settingsService.SaveSettings(_settings);
                UpdateStats();
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}