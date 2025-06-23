using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp4.Models;
using WpfApp4.ViewModels;

namespace WpfApp4
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // XAML инициализация отключена для устранения ошибки
            // InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void TaskName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && tb.DataContext is TaskItem task)
            {
                task.IsEditing = true;
            }
        }

        private void TaskNameEdit_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && tb.DataContext is TaskItem task)
            {
                task.IsEditing = false;
            }
        }

        private void TaskNameEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender is TextBox tb && tb.DataContext is TaskItem task)
            {
                task.IsEditing = false;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DataContext is MainViewModel vm && vm.AddTaskCommand.CanExecute(null))
                {
                    vm.AddTaskCommand.Execute(null);
                }
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Key == Key.Delete)
            {
                if (DataContext is MainViewModel vm &&
                    (FocusManager.GetFocusedElement(this) as ListBox)?.SelectedItem is TaskItem task &&
                    vm.DeleteTaskCommand.CanExecute(task))
                {
                    vm.DeleteTaskCommand.Execute(task);
                }
            }
        }

        private void ToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            App.ToggleTheme();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}