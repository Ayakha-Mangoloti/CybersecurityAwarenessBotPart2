# Cybersecurity Awareness Chatbot – Part 3

## Project Overview

The Cybersecurity Awareness Chatbot is a C# Windows Presentation Foundation (WPF) application developed to promote cybersecurity awareness through an interactive chatbot.

Part 3 extends the existing chatbot by introducing a Task Assistant, reminder functionality, a cybersecurity quiz, an activity log and a simulated Natural Language Processing (NLP) feature.

The application helps users learn about cybersecurity while interacting with the chatbot and managing simple tasks.

## Features

### 1. Task Assistant
- Add new tasks.
- View saved tasks.
- Mark tasks as completed.
- Track pending and completed tasks.

### 2. Reminder System
- Create reminders with specific dates.
- View saved reminders.
- Associate reminder dates with tasks.

### 3. Cybersecurity Mini-Game
- Interactive multiple-choice cybersecurity quiz.
- Three cybersecurity questions.
- Immediate progression between questions.
- Automatic score calculation.
- Final quiz results.

### 4. Activity Log
- Records user interactions.
- Captures activity dates and times.
- Displays a history of chatbot activities.

### 5. NLP Simulation
- Identifies user message intents using keyword recognition.
- Recognises cybersecurity topics such as phishing, passwords, scams, privacy and malware.
- Detects selected emotional expressions.
- Records detected intents in the activity log.

### 6. Existing Chatbot Features
- Interactive chatbot conversations.
- Cybersecurity tips and information.
- User name and favourite topic memory during a conversation.
- Emotion recognition.
- Context-aware responses using previously discussed topics.
- Audio greeting.

## Technologies Used

- C#
- .NET 8
- Windows Presentation Foundation (WPF)
- XAML
- Visual Studio 2022
- Object-Oriented Programming

## How to Run the Application

1. Clone or download this repository.
2. Open the solution in Visual Studio 2022.
3. Ensure that the .NET 8 Windows Desktop development requirements are installed.
4. Build the solution.
5. Run the application using the Start button or F5.

## How to Use the Chatbot

The following commands can be entered into the chatbot:

| Command | Function |
|---|---|
| `Add task: Task title` | Creates a task |
| `View tasks` | Displays saved tasks |
| `Complete task: Task title` | Marks a task as completed |
| `Add reminder: Task title | YYYY-MM-DD` | Creates a dated reminder |
| `View reminders` | Displays saved reminders |
| `Start quiz` | Starts the cybersecurity quiz |
| `View activity log` | Displays recorded activities |

During the quiz, users answer each question by entering A or B.

## Data Storage

Tasks, reminders, quiz information and activity history are maintained in application memory during the running session. They are not currently stored in a permanent database.

## Project Purpose

The purpose of this project is to demonstrate the development of an interactive cybersecurity awareness application while applying programming concepts, event handling, collections, conditional logic, methods and simulated NLP techniques.

## Author

Developed as part of the Cybersecurity Awareness Bot Part 3 project.
