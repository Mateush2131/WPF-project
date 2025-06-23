using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using WpfApp4.Models;

namespace WpfApp4.Services
{
    public static class JsonService
    {
        private static readonly string FilePath = "tasks.json";

        public static List<TaskItem> LoadTasks()
        {
            if (!File.Exists(FilePath)) return new List<TaskItem>();

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
        }

        public static void SaveTasks(List<TaskItem> tasks)
        {
            var json = JsonSerializer.Serialize(tasks);
            File.WriteAllText(FilePath, json);
        }
    }
}