using System.Windows;

namespace CybersecurityAwarenessBotPart2
{
    public partial class MainWindow : Window
    {
        private string favoriteTopic = "";
        private readonly Random random = new Random(); 
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string userMessage = UserInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(userMessage))
            {
                return;
            }
            ChatHistory.AppendText(
                Environment.NewLine + "You: " + userMessage
            );
            string message = userMessage.ToLower();
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
            string response;

           if (isWorried)
           {
                response = "It's understandable to feel worried about cybersecurity. Let's take it step by step. Use strong passwords, enable two-factor authentication, and avoid suspicious links.";
           }
           else if (isFrustrated)
            {
                response = "I understand that cybersecurity can feel confusing or frustrating. Lets take it one step at a time. What would you like help with?";
            }
           else if (isPositive)
            {
                response = "That's great to hear! Keep building your cybersecurity knowledge and practising safe online haits.";
            }
            else if (message.Contains("my favorite topic is "))
            {
                favoriteTopic = userMessage.Substring(
                    "my favorite topic is ".Length
                ).Trim();

                response = "I'll remember that your favorite topic is "
                    + favoriteTopic + " for this conversation.";
            }
            else if (message.Contains("what is my favorite topic"))
            {
                if (string.IsNullOrWhiteSpace(favoriteTopic))
                {
                    response = "You haven't told me your favorite topic yet.";
                }
                else
                {
                    response = "Your favorite topic is " + favoriteTopic + ".";
                }
            }
            else if (message.Contains("password"))
            {
                response = "Use a strong password with a mix of letters, numbers and symbols. Never share your password with anyone.";
            }
            else if (message.Contains("phishing"))
            {
                response = "Phishing is a scam where criminals try to steal your information through fake emails, messages or websites. Always check links before clicking.";
            }
            else if (message.Contains("scam"))
            {
                response = "Be careful with messages promising prizes or asking for money urgently.Verify the sender before responding.";
            }
            else if (message.Contains("malware") || message.Contains("virus"))
            {
                response = "Malware is harmful software that can damage your device or steal your information. Keep your software updated and use trusted security tools.";
            }
            else if (message.Contains("wifi") || message.Contains("wi-fi"))
            {
                response = "Avoid accessing sensitive accounts on unsecured public wi-fi. Use trusted networks and protect your accounts.";
            }
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
                response = "I can help you learn about passwords, phishing, scams, malware and Wi-Fi safety. Which topic would you like to explore?";
            }
            ChatHistory.AppendText(
                Environment.NewLine + "Bot: " + response
                    + Environment.NewLine
                );

            UserInput.Clear();
            ChatHistory.ScrollToEnd();
        }
    }
}