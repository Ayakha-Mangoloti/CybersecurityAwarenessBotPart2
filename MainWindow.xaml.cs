using System.Windows;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace CybersecurityAwarenessBotPart2
{
    public partial class MainWindow : Window
    {
        private List<TaskItem> tasks = new List<TaskItem>();
        private List<string> activityLog = new List<string>();
        private int quizScore = 0;
        private int quizQuestionIndex = 0;
        private bool quizActive = false;
        private readonly string[] quizQuestions =
{
    "1. What should you use to protect your online accounts? \nA. Strong passwords \nB. Your birthday",
    "2. What is phishing? \nA. A computer game \nB. A scam used to steal personal information",
    "3. What should you do with suspicious links? \nA. Click them \nB. Avoid clicking them"
};

        private readonly string[] quizAnswers =
        {
    "a",
    "b",
    "b"
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

            if (message == "view activity log" || message == "show activity log")
            {
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
            TaskItem newTask = new TaskItem(title, description, reminderDate);

            tasks.Add(newTask);

            MessageBox.Show("Task added successfully!");
        }
        private string AddReminder(string title, DateTime reminderDate)
        {
            TaskItem newTask = new TaskItem(
                title,
                "Reminder",
                reminderDate
            );

            tasks.Add(newTask);

            return "Reminder saved for " + reminderDate.ToShortDateString();
        }
        private string ViewReminders()
        {
            string reminderList = "Your saved reminders:\n";
            bool found = false;

            foreach (TaskItem task in tasks)
            {
                if (task.ReminderDate.HasValue)
                {
                    reminderList += "- " + task.Title +
                                    " | " + task.ReminderDate.Value.ToShortDateString() + "\n";
                    found = true;
                }
            }

            if (!found)
            {
                return "You currently have no saved reminders.";
            }

            return reminderList;
        }
        private string ViewTasks()
        {
            if (tasks.Count == 0)
            {
                return "You currently have no saved tasks.";
            }

            string taskList = "Your tasks:\n";

            foreach (TaskItem task in tasks)
            {
                taskList += "- " + task.Title +
                            (task.IsCompleted ? " (Completed)" : " (Pending)") + "\n";
            }

            return taskList;
        }
        private string CompleteTask(string title)
        {
            foreach (TaskItem task in tasks)
            {
                if (task.Title.Equals(title, StringComparison.OrdinalIgnoreCase))
                {
                    task.IsCompleted = true;
                    return "Task marked as completed: " + task.Title;
                }
            }

            return "I couldn't find a task with that title.";
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

            foreach (string activity in activityLog)
            {
                log += "- " + activity + "\n";
            }

            return log;
        }
        private string DetectIntent(string message)
        {
            message = message.ToLower();

            if (message.Contains("password"))
                return "Password Security";

            if (message.Contains("phishing"))
                return "Phishing Awareness";

            if (message.Contains("scam"))
                return "Online Scams";

            if (message.Contains("privacy"))
                return "Online Privacy";

            if (message.Contains("malware") || message.Contains("virus"))
                return "Malware Protection";

            if (message.Contains("worried") || message.Contains("scared"))
                return "Emotional Support";

            if (message.Contains("hello") || message.Contains("hi"))
                return "Greeting";

            return "General Cybersecurity Query";
        }
    }
}
