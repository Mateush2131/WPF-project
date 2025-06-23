using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;

namespace WpfApp4.Models
{
    public class TaskItem : INotifyPropertyChanged
    {
        public enum Priority { Low, Medium, High }

        private string _name = string.Empty;
        private bool _isCompleted;
        private bool _isEditing;
        private DateTime? _deadline;
        private string _label = "Без метки";
        private DateTime _createdAt = DateTime.Now;
        private DateTime? _completedAt;
        private Priority _priority = Priority.Medium;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (_isCompleted != value)
                {
                    _isCompleted = value;
                    if (_isCompleted) CompletedAt = DateTime.Now;
                    else CompletedAt = null;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
            }
        }

        public DateTime? Deadline
        {
            get => _deadline;
            set
            {
                _deadline = value;
                OnPropertyChanged();
            }
        }

        public string Label
        {
            get => _label;
            set
            {
                _label = value;
                OnPropertyChanged();
            }
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set
            {
                _createdAt = value;
                OnPropertyChanged();
            }
        }

        public DateTime? CompletedAt
        {
            get => _completedAt;
            set
            {
                _completedAt = value;
                OnPropertyChanged();
            }
        }

        public Priority TaskPriority
        {
            get => _priority;
            set
            {
                _priority = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}