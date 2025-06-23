using System;
using System.Windows;

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Здесь можно добавить глобальные обработчики, если потребуется
        }

        // Глобальный метод для переключения темы (может быть вызван из MainWindow)
        public static void ToggleTheme()
        {
            var dict = new ResourceDictionary();
            if (Application.Current.Resources.MergedDictionaries[0].Source.OriginalString.Contains("Light"))
                dict.Source = new Uri("Themes/DarkTheme.xaml", UriKind.Relative);
            else
                dict.Source = new Uri("Themes/LightTheme.xaml", UriKind.Relative);

            Application.Current.Resources.MergedDictionaries[0] = dict;
        }
    }
}
