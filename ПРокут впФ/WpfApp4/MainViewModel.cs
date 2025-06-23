using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfApp4.Models;
using WpfApp4.Services;
using System.Linq;
using System;
using System.IO;
using System.Text.Json;

namespace WpfApp4.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public enum SortType { ByName, ByDeadline, ByStatus }

        private string _newTaskName = string.Empty;
        private DateTime? _newTaskDeadline;
        private string _newTaskLabel = "Без метки";
        private TaskItem.Priority _newTaskPriority = TaskItem.Priority.Medium;
        private string _searchText = string.Empty;
        private SortType _selectedSort = SortType.ByName;
        private string _selectedFilter = "Все";

        public ObservableCollection<TaskItem> Tasks { get; } = new();
        public ObservableCollection<TaskItem> FilteredTasks { get; } = new();
        public ObservableCollection<TaskItem> ArchivedTasks { get; } = new();
        public string[] Filters { get; } = new[] { "Все", "Активные", "Выполненные" };
        public string[] Labels { get; } = new[] { "Без метки", "Работа", "Учёба", "Личное" };
        public TaskItem.Priority[] Priorities { get; } = (TaskItem.Priority[])Enum.GetValues(typeof(TaskItem.Priority));
        public SortType[] SortTypes { get; } = (SortType[])Enum.GetValues(typeof(SortType));

        public string NewTaskName
        {
            get => _newTaskName;
            set { _newTaskName = value; OnPropertyChanged(); }
        }
        public DateTime? NewTaskDeadline
        {
            get => _newTaskDeadline;
            set { _newTaskDeadline = value; OnPropertyChanged(); }
        }
        public string NewTaskLabel
        {
            get => _newTaskLabel;
            set { _newTaskLabel = value; OnPropertyChanged(); }
        }
        public TaskItem.Priority NewTaskPriority
        {
            get => _newTaskPriority;
            set { _newTaskPriority = value; OnPropertyChanged(); }
        }
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; ApplyFilter(); OnPropertyChanged(); }
        }
        public SortType SelectedSort
        {
            get => _selectedSort;
            set { _selectedSort = value; ApplyFilter(); OnPropertyChanged(); }
        }
        public string SelectedFilter
        {
            get => _selectedFilter;
            set { _selectedFilter = value; ApplyFilter(); OnPropertyChanged(); }
        }
        public int RemainingCount => Tasks.Count(t => !t.IsCompleted);
        public double CompletedPercent => Tasks.Count == 0 ? 0 : 100.0 * Tasks.Count(t => t.IsCompleted) / Tasks.Count;

        public ICommand AddTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand ClearCompletedCommand { get; }
        public ICommand ArchiveCompletedCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand ImportCommand { get; }

        public MainViewModel()
        {
            AddTaskCommand = new RelayCommand(AddTask);
            DeleteTaskCommand = new RelayCommand(DeleteTask);
            ClearCompletedCommand = new RelayCommand(_ => ClearCompleted());
            ArchiveCompletedCommand = new RelayCommand(_ => ArchiveCompleted());
            ExportCommand = new RelayCommand(_ => ExportTasks("tasks_export.json"));
            ImportCommand = new RelayCommand(_ => ImportTasks("tasks_export.json"));
            LoadTasks();
            SelectedFilter = Filters[0];
        }

        private void AddTask(object? parameter)
        {
            if (!string.IsNullOrWhiteSpace(NewTaskName))
            {
                var task = new TaskItem
                {
                    Name = NewTaskName.Trim(),
                    Deadline = NewTaskDeadline,
                    Label = NewTaskLabel,
                    TaskPriority = NewTaskPriority,
                    CreatedAt = DateTime.Now
                };
                task.PropertyChanged += Task_PropertyChanged;
                Tasks.Add(task);
                NewTaskName = string.Empty;
                NewTaskDeadline = null;
                NewTaskLabel = Labels[0];
                NewTaskPriority = TaskItem.Priority.Medium;
                SaveTasks();
                ApplyFilter();
                OnTasksChanged();
            }
        }

        private void DeleteTask(object? parameter)
        {
            if (parameter is TaskItem task)
            {
                Tasks.Remove(task);
                SaveTasks();
                ApplyFilter();
                OnTasksChanged();
            }
        }

        private void LoadTasks()
        {
            var tasks = JsonService.LoadTasks();
            Tasks.Clear();
            foreach (var task in tasks)
            {
                task.PropertyChanged += Task_PropertyChanged;
                Tasks.Add(task);
            }
            ApplyFilter();
            OnTasksChanged();
        }

        private void SaveTasks()
        {
            JsonService.SaveTasks(Tasks.ToList());
        }

        private void ApplyFilter()
        {
            FilteredTasks.Clear();
            IEnumerable<TaskItem> filtered = SelectedFilter switch
            {
                "Активные" => Tasks.Where(t => !t.IsCompleted),
                "Выполненные" => Tasks.Where(t => t.IsCompleted),
                _ => Tasks
            };
            if (!string.IsNullOrWhiteSpace(SearchText))
                filtered = filtered.Where(t => t.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            switch (SelectedSort)
            {
                case SortType.ByName:
                    filtered = filtered.OrderBy(t => t.Name);
                    break;
                case SortType.ByDeadline:
                    filtered = filtered.OrderBy(t => t.Deadline ?? DateTime.MaxValue);
                    break;
                case SortType.ByStatus:
                    filtered = filtered.OrderBy(t => t.IsCompleted);
                    break;
            }
            foreach (var task in filtered)
                FilteredTasks.Add(task);
        }

        private void ClearCompleted()
        {
            var completed = Tasks.Where(t => t.IsCompleted).ToList();
            foreach (var task in completed)
                Tasks.Remove(task);
            SaveTasks();
            ApplyFilter();
            OnTasksChanged();
        }

        private void ArchiveCompleted()
        {
            var completed = Tasks.Where(t => t.IsCompleted).ToList();
            foreach (var task in completed)
            {
                Tasks.Remove(task);
                ArchivedTasks.Add(task);
            }
            SaveTasks();
            ApplyFilter();
            OnTasksChanged();
        }

        public void ExportTasks(string path)
        {
            File.WriteAllText(path, JsonSerializer.Serialize(Tasks));
        }
        public void ImportTasks(string path)
        {
            var imported = JsonSerializer.Deserialize<List<TaskItem>>(File.ReadAllText(path));
            if (imported != null)
            {
                Tasks.Clear();
                foreach (var t in imported)
                {
                    t.PropertyChanged += Task_PropertyChanged;
                    Tasks.Add(t);
                }
                SaveTasks();
                ApplyFilter();
                OnTasksChanged();
            }
        }

        private void Task_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TaskItem.IsCompleted))
            {
                SaveTasks();
                ApplyFilter();
                OnTasksChanged();
            }
        }

        private void OnTasksChanged()
        {
            OnPropertyChanged(nameof(RemainingCount));
            OnPropertyChanged(nameof(CompletedPercent));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}