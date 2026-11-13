using System.Windows;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Data.SqlClient;

namespace CybersecurityAwarenessBotPart2
{
    public partial class MainWindow : Window
    {
        private readonly string connectionString =
            @"Server=.\SQLEXPRESS;Database=CybersecurityAwarenessDB;Trusted_Connection=True;TrustServerCertificate=True;";

        private List<TaskItem> tasks = new List<TaskItem>();
        private List<string> activityLog = new List<string>();
        private int activityLogDisplayCount = 5;
        private int quizScore = 0;
        private int quizQuestionIndex = 0;
        private bool quizActive = false;


        private readonly string[] quizQuestions =
        {
    "1. What should you use to protect your online accounts? \nA. Strong passwords \nB. Your birthday",
    "2. What is phishing? \nA. A computer game \nB. A scam used to steal personal information",
    "3. What should you do with suspicious links? \nA. Click them \nB. Avoid clicking them",
    "4. What is malware? \nA. Malicious software \nB. A security update",
    "5. Why should you enable two-factor authentication? \nA. To add extra account security \nB. To make passwords shorter",
    "6. What should you do when using public Wi-Fi? \nA. Access your banking account freely \nB. Avoid sensitive transactions on unsecured networks",
    "7. What should you do if you receive a suspicious email attachment? \nA. Open it immediately \nB. Verify the sender before opening it",
    "8. What is the safest way to protect personal information online? \nA. Share it with everyone \nB. Only share it with trusted services when necessary",
    "9. What should you do if your account is hacked? \nA. Change your password and secure the account \nB. Ignore the problem",
    "10. Why are software updates important? \nA. They fix security weaknesses \nB. They only change the appearance"
};

        private readonly string[] quizAnswers =
        {
    "a",
    "b",
    "b",
    "a",
    "a",
    "b",
    "b",
    "b",
    "a",
    "a"
};

        private delegate string ResponseHandler(string message);
        private string favoriteTopic = "";
        private string userName = "";
        private string lastTopic = "";
        private readonly Random random = new Random(); 

        private readonly List<string> cybersecurityTips = new List<string>
        {
            "Use a unique, strong password for every account.",
            "Enable two-factor authentication to add another layer of security.",
            "Never click suspicious links in emails or text messages.",
            "Keep your operating system and applications updated.",
            "Avoid sharing sensitive personal information on public platforms.",
            "Verify unexpected messages before sending money or sharing information." 
        };
        public MainWindow()
        {
            InitializeComponent();
            LoadTasksIntoList();

            try
            {
                string audioPath = System.IO.Path.Combine( 
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Audio",
                    "greeting.wav");
                System.Media.SoundPlayer greetingPlayer =
                    new System.Media.SoundPlayer(audioPath);

                greetingPlayer.Play();
            }
            catch (Exception)
            {
                MessageBox.Show("The voice greeting could not be played.");
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string userMessage = UserInput.Text.Trim();

            if (!string.IsNullOrWhiteSpace(userMessage))
            {
                activityLog.Add(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - " + userMessage);
            }

            if (string.IsNullOrWhiteSpace(userMessage))
            {
                return;
            }
            ChatHistory.AppendText(Environment.NewLine + "You: " + userMessage);
            string message = userMessage.ToLower();

            string detectedIntent = DetectIntent(message);
            activityLog.Add(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                + " - Detected Intent: " + detectedIntent);

            if (message == "show more")
            {
                activityLogDisplayCount += 5;

                string log = ViewActivityLog();

                ChatHistory.AppendText(Environment.NewLine + "Bot: " + log + Environment.NewLine);
                UserInput.Clear();
                ChatHistory.ScrollToEnd();
                return;
            }
            if (message == "view activity log" || message == "show activity log")
            {
                activityLogDisplayCount = 5;
                string log = ViewActivityLog();

                ChatHistory.AppendText(Environment.NewLine + "Bot: " + log + Environment.NewLine);
                UserInput.Clear();
                ChatHistory.ScrollToEnd();
                return;
            }

            if (message == "start quiz")
            {
                string quizMessage = StartQuiz();

                ChatHistory.AppendText(Environment.NewLine + "Bot: " + quizMessage + Environment.NewLine);
                UserInput.Clear();
                ChatHistory.ScrollToEnd();
                return;
            }

            if (quizActive)
            {
                string answer = message.Trim();

                if (answer == "a" || answer == "b")
                {
                    string quizMessage = CheckQuizAnswer(answer);

                    ChatHistory.AppendText(Environment.NewLine + "Bot: " + quizMessage + Environment.NewLine);
                }
                else
                {
                    ChatHistory.AppendText(Environment.NewLine + "Bot: Please answer using A or B." + Environment.NewLine);
                }

                UserInput.Clear();
                ChatHistory.ScrollToEnd();
                return;
            }
            if (message.StartsWith("add task:"))
            {
                string taskTitle = userMessage.Substring("add task:".Length).Trim();

                if (!string.IsNullOrWhiteSpace(taskTitle))
                {
                    AddTask(taskTitle, "No description provided.", null);
                    ChatHistory.AppendText(Environment.NewLine + "Bot: Your task has been saved!" + Environment.NewLine);
                    UserInput.Clear();
                    return;
                }
            }

            if (message == "view tasks" || message == "show my tasks")
            {
                string taskList = ViewTasks();

                ChatHistory.AppendText(Environment.NewLine + "Bot: " + taskList + Environment.NewLine);
                UserInput.Clear();
                return;
            }
            if (message.StartsWith("delete task:"))
            {
                string taskTitle = userMessage.Substring("delete task:".Length).Trim();

                string result = DeleteTask(taskTitle);

                ChatHistory.AppendText(Environment.NewLine + "Bot: " + result + Environment.NewLine);
                UserInput.Clear();
                ChatHistory.ScrollToEnd();
                return;
            }

            if (message.StartsWith("complete task:"))
            {
                string taskTitle = userMessage.Substring("complete task:".Length).Trim();

                string result = CompleteTask(taskTitle);

                ChatHistory.AppendText(Environment.NewLine + "Bot: " + result + Environment.NewLine);
                UserInput.Clear();
                ChatHistory.ScrollToEnd();
                return;
            }
            if (message.StartsWith("add reminder:"))
            {
                string reminderDetails = userMessage.Substring("add reminder:".Length).Trim();
                string[] parts = reminderDetails.Split('|');

                if (parts.Length == 2 &&
                    !string.IsNullOrWhiteSpace(parts[0]) &&
                    DateTime.TryParse(parts[1].Trim(), out DateTime reminderDate))
                {
                    string result = AddReminder(parts[0].Trim(), reminderDate);

                    ChatHistory.AppendText(Environment.NewLine + "Bot: " + result + Environment.NewLine);
                }
                else
                {
                    ChatHistory.AppendText(
                        Environment.NewLine +
                        "Bot: Please use this format: add reminder: Task title | YYYY-MM-DD" +
                        Environment.NewLine);
                }

                UserInput.Clear();
                ChatHistory.ScrollToEnd();
                return;
            }
            if (message == "view reminders" || message == "show reminders")
            {
                string reminders = ViewReminders();

                ChatHistory.AppendText(Environment.NewLine + "Bot: " + reminders + Environment.NewLine);
                UserInput.Clear();
                ChatHistory.ScrollToEnd();
                return;
            }

            string response;

            if (message.Contains("password"))
                lastTopic = "passwords";
            else if (message.Contains("phishing"))
                lastTopic = "phishing";
            else if (message.Contains("privacy") || message.Contains("personal information"))
                lastTopic = "privacy";
            else if (message.Contains("scam"))
                lastTopic = "scams";
            else if (message.Contains("malware") || message.Contains("virus"))
                lastTopic = "malware";
            else if (message.Contains("wifi") || message.Contains("wi-fi"))
                lastTopic = "Wi-Fi safety";

            bool isWorried = message.Contains("worried")
                    || message.Contains("scared")
                    || message.Contains("afraid")
                    || message.Contains("anxious");

            bool isFrustrated = message.Contains("frustrated")
                || message.Contains("annoyed")
                || message.Contains("confused")
                || message.Contains("afraid");

            bool isPositive = message.Contains("happy")
                || message.Contains("confident")
                || message.Contains("excited")
                || message.Contains("great");

            if (isWorried)
            {
                response = "It's understandable to feel worried about cybersecurity. Let's take it step by step. Use strong passwords, enable two-factor authentication, and avoid suspicious links.";
            }
            else if (isFrustrated)
            {
                response = "I understand that cybersecurity can feel confusing or frustrating. Let's take it one step at a time. What would you like help with?";
            }
            else if (isPositive)
            {
                response = "That's great to hear! Keep building your cybersecurity knowledge and practising safe online haits.";
            }
            else if (message.StartsWith("my name is "))
            {
                userName = userMessage.Substring("my name is ".Length).Trim();

                response = "Nice to meet you, " + userName + "! I'll remember your name during this conversation.";
            }
            else if (message.Contains("what is my name") || message.Contains("what's my name") || message.Contains("who am I"))
            {
                if (string.IsNullOrWhiteSpace(userName))
                {
                    response = "You haven't told me your name yet. Try syaing 'My name is Ayakha'.";
                }
                else
                {
                    response = "Your name is " + userName + ".";
                }
            }
            else if (message.StartsWith("my favorite topic is "))
            {
                favoriteTopic = userMessage.Substring("my favorite topic is ".Length).Trim();
                response = "I'll remember that your favorite topic is " + favoriteTopic + " for this conversation.";
            }
            else if (message.Contains("what is my favorite topic"))
            {
                response = string.IsNullOrWhiteSpace(favoriteTopic)
                 ? "You haven't told me your favorite topic yet."
                 : "Your favorite topic is " + favoriteTopic + ".";
            }
            else if (message.Contains("explain more") || message.Contains("tell me more"))
            {
                if (lastTopic == "passwords")
                    response = "A strong password should be long, unique, and difficult to guess. Use a different password for each account and consider a trusted manager.";
                else if (lastTopic == "phishing")
                    response = "Phishing messages often create urgency or pretend to come from trusted organisations. Check the sender's address, inspect links carefully, and never enter passwords on suspicious websites.";
                else if (lastTopic == "privacy")
                    response = "Online privacy means controlling how your personal information is collected and shared. Review app permissions, limit what you post publicly, and use privacy settings.";
                else if (lastTopic == "scams")
                    response = "Online scams may involve fake prizes, urgent payment requests, or people pretending to be someone you trust. Verify claims independently before sending money or sharing information.";
                else if (lastTopic == "malware")
                    response = "Malware is harmful software that can steal information or damage devices. Keep software updated, avoid suspicious downloads, and use reputable security tools.";
                else if (lastTopic == "Wi-Fi safety")
                    response = "Avoid sensitive transactions on untrusted public Wi-Fi networks.";
                else
                    response = "Which topic would you like to explain: passwords, phishing, privacy, scams, malware or Wi-Fi safety";
            }
            else if (message.Contains("password"))
                response = "Use a strong password with a mix of letters, numbers and symbols. Never share your password with anyone.";
            else if (message.Contains("phishing"))
                response = "Phishing is a scam where criminals try to steal your information through fake emails, messages or websites. Always check links before clicking.";
            else if (message.Contains("scam"))
                response = "Be careful with messages promising prizes or asking for money urgently.Verify the sender before responding.";
            else if (message.Contains("privacy") || message.Contains("personal information"))
                response = "Protect your privacy by linking the personal information you share online. Check app permissions, use privacy settings, and never share sensitive information with untrusted websites or people.";
            else if (message.Contains("tip") || message.Contains("another tip"))
                response = cybersecurityTips[random.Next(cybersecurityTips.Count)];
            else if (message.Contains("malware") || message.Contains("virus"))
                response = "Malware is harmful software that can damage your device or steal your information. Keep your software updated and use trusted security tools.";
            else if (message.Contains("wifi") || message.Contains("wi-fi"))
                response = "Avoid accessing sensitive accounts on unsecured public wi-fi. Use trusted networks and protect your accounts.";
            else if (message.Contains("hello") || message == "hi" || message.StartsWith("hi "))
            {
                string[] greetings =
                {
                    "Hello! What would you like to learn about cybersecurity today?",
                    "Hi there! I'm ready to help you to stay safe online.",
                    "Hey! Let's protect your accounts andpersonal information.",
                    "Hello! Would you like to learn about passwords, phishing or online scams?"
                };
                response = greetings[random.Next(greetings.Length)];
            }
            else
            {
                ResponseHandler handler = GetDefaultResponse;
                response = handler(userMessage);
            }
            ChatHistory.AppendText(Environment.NewLine + "Bot: " + response + Environment.NewLine);
            UserInput.Clear();
            ChatHistory.ScrollToEnd();
        }
        private string GetDefaultResponse(string message)
        {
            return "I can help you learn about passwords, phishing, scams, malware, privacy and Wi-Fi safety. Which topic would you like to explore?";
        }

        private void AddTask(string title, string description, DateTime? reminderDate)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                string query = @"INSERT INTO Tasks (Title, Description, ReminderDate)
                         VALUES (@Title, @Description, @ReminderDate)";

                using SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Title", title);
                command.Parameters.AddWithValue("@Description", description);
                command.Parameters.AddWithValue("@ReminderDate",
                    reminderDate.HasValue ? reminderDate.Value : DBNull.Value);

                command.ExecuteNonQuery();

                MessageBox.Show("Task added successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save task: " + ex.Message);
            }
        }

        private string AddReminder(string title, DateTime reminderDate)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                string query = @"INSERT INTO Tasks (Title, Description, ReminderDate)
                         VALUES (@Title, @Description, @ReminderDate)";

                using SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Title", title);
                command.Parameters.AddWithValue("@Description", "Reminder");
                command.Parameters.AddWithValue("@ReminderDate", reminderDate);

                command.ExecuteNonQuery();

                return "Reminder saved for " + reminderDate.ToShortDateString();
            }
            catch (Exception ex)
            {
                return "Could not save reminder: " + ex.Message;
            }
        }

        private string ViewReminders()
        {
            try
            {
                using SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                string query = @"SELECT Title, ReminderDate
                         FROM Tasks
                         WHERE ReminderDate IS NOT NULL
                         ORDER BY ReminderDate";

                using SqlCommand command = new SqlCommand(query, connection);
                using SqlDataReader reader = command.ExecuteReader();

                string reminderList = "Your saved reminders:\n";
                bool found = false;

                while (reader.Read())
                {
                    reminderList += "- " + reader["Title"] +
                                    " | " + Convert.ToDateTime(reader["ReminderDate"]).ToShortDateString() +
                                    "\n";
                    found = true;
                }

                return found ? reminderList : "You currently have no saved reminders.";
            }
            catch (Exception ex)
            {
                return "Could not load reminders: " + ex.Message;
            }
        }

        private string ViewTasks()
        {
            try
            {
                using SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                string query = @"SELECT Title, IsCompleted
                         FROM Tasks
                         ORDER BY TaskID DESC";

                using SqlCommand command = new SqlCommand(query, connection);
                using SqlDataReader reader = command.ExecuteReader();

                string taskList = "Your tasks:\n";
                bool found = false;

                while (reader.Read())
                {
                    string status = Convert.ToBoolean(reader["IsCompleted"])
                        ? " (Completed)"
                        : " (Pending)";

                    taskList += "- " + reader["Title"] + status + "\n";
                    found = true;
                }

                return found ? taskList : "You currently have no saved tasks.";
            }
            catch (Exception ex)
            {
                return "Could not load tasks: " + ex.Message;
            }
        }

        private string CompleteTask(string title)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                string query = @"UPDATE Tasks
                         SET IsCompleted = 1
                         WHERE Title = @Title AND IsCompleted = 0";

                using SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Title", title);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return "Task marked as completed: " + title;
                }

                return "I couldn't find a pending task with that title.";
            }
            catch (Exception ex)
            {
                return "Could not update task: " + ex.Message;
            }
        }
        private string DeleteTask(string title)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                string query = @"DELETE FROM Tasks
                         WHERE TaskID = (
                             SELECT TOP 1 TaskID
                             FROM Tasks
                             WHERE Title = @Title
                             ORDER BY TaskID DESC
                         )";

                using SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Title", title);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return "Task deleted successfully: " + title;
                }

                return "I couldn't find a task with that title.";
            }
            catch (Exception ex)
            {
                return "Could not delete task: " + ex.Message;
            }
        }


        private string StartQuiz()
        {
            quizScore = 0;
            quizQuestionIndex = 0;
            quizActive = true;

            return "Welcome to the Cybersecurity Quiz! Answer each question using A or B.\n\n"
                   + quizQuestions[quizQuestionIndex];
        }

        private string CheckQuizAnswer(string answer)
        {
            if (answer == quizAnswers[quizQuestionIndex])
            {
                quizScore++;
            }

            quizQuestionIndex++;

            if (quizQuestionIndex < quizQuestions.Length)
            {
                return quizQuestions[quizQuestionIndex];
            }

            quizActive = false;

            return "Quiz completed! Your score is "
                   + quizScore + "/" + quizQuestions.Length
                   + ". Well done for testing your cybersecurity knowledge!";
        }
        private string ViewActivityLog()
        {
            if (activityLog.Count == 0)
            {
                return "No activities have been recorded yet.";
            }

            string log = "Chatbot Activity History:\n";

            int startIndex = Math.Max(0, activityLog.Count - activityLogDisplayCount);

            for (int i = activityLog.Count - 1; i >= startIndex; i--)
            {
                log += "- " + activityLog[i] + "\n";
            }

            if (startIndex > 0)
            {
                log += "\nType 'show more' to view older activities.";
            }
            else
            {
                log += "\nYou have reached the beginning of your activity history.";
            }

            return log;
        }
        private string DetectIntent(string message)
        {
            message = message.ToLower().Trim();

            if (message.Contains("hello") || message.Contains("hi there") ||
                message.Contains("good morning") || message.Contains("good afternoon") ||
                message.Contains("good evening"))
                return "Greeting";

            if (message.Contains("password") || message.Contains("login details") ||
                message.Contains("account security") || message.Contains("passcode"))
                return "Password Security";

            if (message.Contains("phishing") || message.Contains("fake email") ||
                message.Contains("suspicious email") || message.Contains("fake website") ||
                message.Contains("suspicious message"))
                return "Phishing Awareness";

            if (message.Contains("scam") || message.Contains("fraud") ||
                message.Contains("stolen money") || message.Contains("online fraud"))
                return "Online Scams";

            if (message.Contains("privacy") || message.Contains("personal information") ||
                message.Contains("data protection") || message.Contains("private details"))
                return "Online Privacy";

            if (message.Contains("malware") || message.Contains("virus") ||
                message.Contains("ransomware") || message.Contains("infected computer"))
                return "Malware Protection";

            if (message.Contains("worried") || message.Contains("scared") ||
                message.Contains("afraid") || message.Contains("confused"))
                return "Emotional Support";

            if (message.Contains("hacked") || message.Contains("hacking") ||
                message.Contains("unauthorised access"))
                return "Account Security";

            if (message.Contains("safe browsing") || message.Contains("secure website") ||
                message.Contains("internet safety"))
                return "Safe Browsing";

            return "General Cybersecurity Query";
        }

        private void LoadTasksIntoList()
        {
            try
            {
                using SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                string query = @"SELECT TaskID, Title, Description, ReminderDate, IsCompleted
                         FROM Tasks ORDER BY TaskID DESC";

                using SqlCommand command = new SqlCommand(query, connection);
                using SqlDataReader reader = command.ExecuteReader();

                List<TaskItem> taskList = new List<TaskItem>();

                while (reader.Read())
                {
                    TaskItem task = new TaskItem(
                        reader["Title"].ToString() ?? "",
                        reader["Description"].ToString() ?? "",
                        reader["ReminderDate"] == DBNull.Value
                            ? null
                            : Convert.ToDateTime(reader["ReminderDate"]));

                    task.TaskID = Convert.ToInt32(reader["TaskID"]);
                    task.IsCompleted = Convert.ToBoolean(reader["IsCompleted"]);

                    taskList.Add(task);
                }

                TaskListBox.ItemsSource = taskList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load tasks: " + ex.Message);
            }
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TaskTitleInput.Text.Trim();
            string description = TaskDescriptionInput.Text.Trim();
            DateTime? reminderDate = TaskReminderDatePicker.SelectedDate;

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please enter a task title.");
                return;
            }

            AddTask(title, description, reminderDate);
            LoadTasksIntoList();

            TaskTitleInput.Clear();
            TaskDescriptionInput.Clear();
            TaskReminderDatePicker.SelectedDate = null;
        }

        private void CompleteSelectedTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListBox.SelectedItem is not TaskItem selectedTask)
            {
                MessageBox.Show("Please select a task first.");
                return;
            }

            string result = CompleteTask(selectedTask.Title);
            MessageBox.Show(result);
            LoadTasksIntoList();
        }

        private void DeleteSelectedTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListBox.SelectedItem is not TaskItem selectedTask)
            {
                MessageBox.Show("Please select a task first.");
                return;
            }

            string result = DeleteTask(selectedTask.Title);
            MessageBox.Show(result);
            LoadTasksIntoList();
        }

        private void RefreshTasksButton_Click(object sender, RoutedEventArgs e)
        {
            LoadTasksIntoList();
        }
    }
}
