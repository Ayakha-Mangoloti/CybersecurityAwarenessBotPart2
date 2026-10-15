using System.Windows;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace CybersecurityAwarenessBotPart2
{
    public partial class MainWindow : Window
    {
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

            if (string.IsNullOrWhiteSpace(userMessage))
            {
                return;
            }
            ChatHistory.AppendText(Environment.NewLine + "You: " + userMessage);
            string message = userMessage.ToLower();
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
                response = "I understand that cybersecurity can feel confusing or frustrating. Lets take it one step at a time. What would you like help with?";
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
    }
}
