using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace CyberBotSA_part2
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Reminder { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TaskManager
    {
        private DatabaseManager db = new DatabaseManager();

        public string AddTask(string title, string description, string reminder)
        {
            try
            {
                string query = "INSERT INTO tasks (title, description, reminder) VALUES (@title, @desc, @reminder)";
                db.ExecuteNonQuery(query, cmd =>
                {
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@desc", description);
                    cmd.Parameters.AddWithValue("@reminder", reminder ?? "");
                });
                return $"Task added: '{title}'. {(string.IsNullOrEmpty(reminder) ? "No reminder set." : $"Reminder: {reminder}.")}";
            }
            catch (Exception ex)
            {
                return $"Error adding task: {ex.Message}";
            }
        }

        public List<TaskItem> GetAllTasks()
        {
            List<TaskItem> tasks = new List<TaskItem>();
            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    MySqlDataReader reader = db.ExecuteReader("SELECT * FROM tasks WHERE is_completed = FALSE", conn);
                    while (reader.Read())
                    {
                        tasks.Add(new TaskItem
                        {
                            Id = reader.GetInt32("id"),
                            Title = reader.GetString("title"),
                            Description = reader.GetString("description"),
                            Reminder = reader.GetString("reminder"),
                            IsCompleted = reader.GetBoolean("is_completed"),
                            CreatedAt = reader.GetDateTime("created_at")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting tasks: {ex.Message}");
            }
            return tasks;
        }

        public string CompleteTask(int id)
        {
            try
            {
                string query = "UPDATE tasks SET is_completed = TRUE WHERE id = @id";
                db.ExecuteNonQuery(query, cmd =>
                {
                    cmd.Parameters.AddWithValue("@id", id);
                });
                return $"Task {id} marked as completed!";
            }
            catch (Exception ex)
            {
                return $"Error completing task: {ex.Message}";
            }
        }

        public string DeleteTask(int id)
        {
            try
            {
                string query = "DELETE FROM tasks WHERE id = @id";
                db.ExecuteNonQuery(query, cmd =>
                {
                    cmd.Parameters.AddWithValue("@id", id);
                });
                return $"Task {id} deleted successfully.";
            }
            catch (Exception ex)
            {
                return $"Error deleting task: {ex.Message}";
            }
        }

        public string FormatTaskList(List<TaskItem> tasks)
        {
            if (tasks.Count == 0)
                return "You have no pending tasks. Type 'add task' to add one.";

            string result = "Your cybersecurity tasks:\n\n";
            foreach (TaskItem task in tasks)
            {
                result += $"[{task.Id}] {task.Title}\n";
                result += $"     {task.Description}\n";
                if (!string.IsNullOrEmpty(task.Reminder))
                    result += $"     Reminder: {task.Reminder}\n";
                result += "\n";
            }
            result += "Type 'complete task [number]' or 'delete task [number]' to manage tasks.";
            return result;
        }
    }
}
