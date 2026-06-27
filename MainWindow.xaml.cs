using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CyberBotSA_part2
{
    public partial class MainWindow : Window
    {
        private ResponseEngine responseEngine = new ResponseEngine();
        private MemoryManager memory = new MemoryManager();
        private TaskManager taskManager = new TaskManager();
        private QuizManager quizManager = new QuizManager();
        private ActivityLog activityLog = new ActivityLog();
        private bool nameEntered = false;
        private string userName = "";

        public MainWindow()
        {
            InitializeComponent();
            AudioPlayer.PlayGreeting();
            AddBotMessage("Welcome to CyberBot SA! 🛡");
            AddBotMessage("I'm your Cybersecurity Awareness Assistant.");
            AddBotMessage("Please enter your name to get started.");
            AddBotMessage("You can also use the Tasks, Quiz and Activity Log tabs above.");
        }

        // ── Chat Tab ────────────────────────────────────────────────

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessInput();
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ProcessInput();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ChatPanel.Children.Clear();
            AddBotMessage("Chat cleared. How can I help you, " + userName + "?");
        }

        private void ProcessInput()
        {
            string input = UserInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            AddUserMessage(input);
            UserInput.Clear();

            if (!nameEntered)
            {
                userName = input;
                nameEntered = true;
                memory.Store("name", userName);
                UserNameLabel.Text = "👤 " + userName;
                activityLog.AddEntry($"User '{userName}' started a session.");
                AddBotMessage($"Hello, {userName}! Great to have you here. 😊");
                AddBotMessage("I'm here to help you stay safe online.");
                AddBotMessage("Ask me about password safety, phishing, privacy, scams, malware, or Wi-Fi safety.");
                AddBotMessage("You can also type 'start quiz', 'add task', 'show tasks', or 'show activity log'.");
                return;
            }

            string inputLower = input.ToLower();

            // NLP - Activity log commands
            if (inputLower.Contains("activity log") || inputLower.Contains("what have you done") ||
                inputLower.Contains("recent actions"))
            {
                activityLog.AddEntry("User requested activity log.");
                AddBotMessage(activityLog.GetLog());
                return;
            }

            // NLP - Quiz commands
            if (inputLower.Contains("start quiz") || inputLower.Contains("begin quiz") ||
                inputLower.Contains("play quiz") || inputLower.Contains("test me"))
            {
                activityLog.AddEntry("Quiz started.");
                AddBotMessage(quizManager.StartQuiz());
                return;
            }

            // NLP - Answer quiz question
            if (quizManager.IsQuizActive)
            {
                string quizResponse = quizManager.AnswerQuestion(input);
                activityLog.AddEntry($"Quiz answer submitted: {input}");
                AddBotMessage(quizResponse);
                return;
            }

            // NLP - Task commands
            if (inputLower.Contains("add task") || inputLower.Contains("new task") ||
                inputLower.Contains("create task") || inputLower.Contains("remind me"))
            {
                AddBotMessage("Please use the Tasks tab to add a new cybersecurity task with a title, description and reminder.");
                activityLog.AddEntry("User directed to Tasks tab.");
                return;
            }

            if (inputLower.Contains("show tasks") || inputLower.Contains("my tasks") ||
                inputLower.Contains("view tasks") || inputLower.Contains("list tasks"))
            {
                List<TaskItem> tasks = taskManager.GetAllTasks();
                string taskResponse = taskManager.FormatTaskList(tasks);
                activityLog.AddEntry("User viewed task list.");
                AddBotMessage(taskResponse);
                return;
            }

            // NLP - Complete task
            if (inputLower.Contains("complete task") || inputLower.Contains("finish task") ||
                inputLower.Contains("done with task"))
            {
                AddBotMessage("Please use the Tasks tab to complete or delete tasks.");
                return;
            }

            // Normal chat response
            string response = responseEngine.GetResponse(input, userName, memory);

            if (memory.Has("favouriteTopic"))
                FavTopicLabel.Text = "⭐ Interested in: " + memory.Retrieve("favouriteTopic");

            activityLog.AddEntry($"Chat: User asked about '{input.Substring(0, Math.Min(30, input.Length))}'");
            AddBotMessage(response);
        }

        private void AddUserMessage(string message)
        {
            Border bubble = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(15, 52, 96)),
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(100, 5, 5, 5),
                Padding = new Thickness(10)
            };
            TextBlock text = new TextBlock
            {
                Text = "👤 " + message,
                Foreground = Brushes.White,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap
            };
            bubble.Child = text;
            ChatPanel.Children.Add(bubble);
            ChatScrollViewer.ScrollToBottom();
        }

        private void AddBotMessage(string message)
        {
            Border bubble = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(22, 33, 62)),
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(5, 5, 100, 5),
                Padding = new Thickness(10),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 212, 255)),
                BorderThickness = new Thickness(1)
            };
            TextBlock text = new TextBlock
            {
                Text = "🛡 CyberBot: " + message,
                Foreground = new SolidColorBrush(Color.FromRgb(0, 212, 255)),
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap
            };
            bubble.Child = text;
            ChatPanel.Children.Add(bubble);
            ChatScrollViewer.ScrollToBottom();
        }

        // ── Tasks Tab ────────────────────────────────────────────────

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = TaskTitle.Text.Trim();
            string description = TaskDescription.Text.Trim();
            string reminder = TaskReminder.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                TaskStatusLabel.Text = "Please enter a task title.";
                TaskStatusLabel.Foreground = new SolidColorBrush(Colors.Red);
                return;
            }

            if (string.IsNullOrWhiteSpace(description))
                description = "No description provided.";

            string result = taskManager.AddTask(title, description, reminder);
            TaskStatusLabel.Text = result;
            TaskStatusLabel.Foreground = new SolidColorBrush(Color.FromRgb(0, 212, 255));

            activityLog.AddEntry($"Task added: '{title}'.");

            TaskTitle.Clear();
            TaskDescription.Clear();
            TaskReminder.Clear();

            RefreshTaskList();
        }

        private void ViewTasks_Click(object sender, RoutedEventArgs e)
        {
            RefreshTaskList();
            activityLog.AddEntry("User viewed task list.");
        }

        private void CompleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListBox.SelectedItem == null)
            {
                TaskStatusLabel.Text = "Please select a task first.";
                TaskStatusLabel.Foreground = new SolidColorBrush(Colors.Red);
                return;
            }

            string selected = TaskListBox.SelectedItem.ToString();
            int id = ExtractTaskId(selected);

            if (id > 0)
            {
                string result = taskManager.CompleteTask(id);
                TaskStatusLabel.Text = result;
                TaskStatusLabel.Foreground = new SolidColorBrush(Color.FromRgb(0, 212, 255));
                activityLog.AddEntry($"Task {id} marked as completed.");
                RefreshTaskList();
            }
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListBox.SelectedItem == null)
            {
                TaskStatusLabel.Text = "Please select a task first.";
                TaskStatusLabel.Foreground = new SolidColorBrush(Colors.Red);
                return;
            }

            string selected = TaskListBox.SelectedItem.ToString();
            int id = ExtractTaskId(selected);

            if (id > 0)
            {
                string result = taskManager.DeleteTask(id);
                TaskStatusLabel.Text = result;
                TaskStatusLabel.Foreground = new SolidColorBrush(Color.FromRgb(0, 212, 255));
                activityLog.AddEntry($"Task {id} deleted.");
                RefreshTaskList();
            }
        }

        private void RefreshTaskList()
        {
            TaskListBox.Items.Clear();
            List<TaskItem> tasks = taskManager.GetAllTasks();
            if (tasks.Count == 0)
            {
                TaskListBox.Items.Add("No pending tasks.");
                return;
            }
            foreach (TaskItem task in tasks)
            {
                string item = $"[{task.Id}] {task.Title}";
                if (!string.IsNullOrEmpty(task.Reminder))
                    item += $" (Reminder: {task.Reminder})";
                TaskListBox.Items.Add(item);
            }
        }

        private int ExtractTaskId(string selected)
        {
            try
            {
                int start = selected.IndexOf('[') + 1;
                int end = selected.IndexOf(']');
                string idStr = selected.Substring(start, end - start);
                return int.Parse(idStr);
            }
            catch
            {
                return -1;
            }
        }

        // ── Quiz Tab ────────────────────────────────────────────────

        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            QuizText.Text = quizManager.StartQuiz();
            activityLog.AddEntry("Quiz started from Quiz tab.");
        }

        private void SubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            string answer = QuizAnswer.Text.Trim();
            if (string.IsNullOrWhiteSpace(answer)) return;

            string result = quizManager.AnswerQuestion(answer);
            QuizText.Text = result;
            activityLog.AddEntry($"Quiz answer submitted: {answer}");
            QuizAnswer.Clear();
        }

        // ── Activity Log Tab ─────────────────────────────────────────

        private void RefreshLog_Click(object sender, RoutedEventArgs e)
        {
            ActivityLogText.Text = activityLog.GetLog();
        }
    }
}