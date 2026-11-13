# Cybersecurity Awareness Bot – Part 3

## Project Overview

The Cybersecurity Awareness Bot is a C# WPF desktop application designed to educate users about cybersecurity through interactive conversations, task management, reminders, quizzes and activity tracking.

Part 3 builds on the functionality developed in Parts 1 and 2 by introducing database integration and additional interactive features.

## Features

### 1. Interactive Chatbot
- Greets users when the application starts.
- Remembers the user's name and favourite cybersecurity topic.
- Provides information and tips about cybersecurity.
- Uses simulated Natural Language Processing (NLP) to recognise user intent and respond to different questions and phrases.

### 2. Task Assistant and Reminders
- Add tasks using the graphical user interface.
- Enter task titles and descriptions.
- Set reminder dates.
- View saved tasks.
- Mark tasks as completed.
- Delete selected tasks.
- Refresh the task list.

### 3. SQL Server Database Integration
The application uses Microsoft SQL Server Express to store task information.

The database supports:
- Creating new tasks.
- Retrieving saved tasks.
- Updating task completion status.
- Deleting tasks.
- Retaining task information after the application closes.

Database operations use parameterised SQL commands and exception handling.

### 4. Cybersecurity Quiz
The application includes an interactive cybersecurity quiz with 10 questions covering topics such as:
- Strong passwords.
- Phishing attacks.
- Suspicious links.
- Malware.
- Two-factor authentication.
- Public Wi-Fi safety.
- Suspicious email attachments.
- Personal information protection.
- Compromised accounts.
- Software updates.

Users receive feedback and a final score.

### 5. Activity Log
- Records user interactions and activities.
- Displays five activities initially.
- Allows users to request additional entries using the `show more` command.
- Helps users review their recent chatbot interactions.

### 6. Audio Greeting
The application includes an audio greeting using a WAV file when the application starts.

## Technologies Used

- C#
- .NET 8
- Windows Presentation Foundation (WPF)
- Microsoft Visual Studio 2022
- Microsoft SQL Server Express
- SQL Server Management Studio (SSMS)
- Microsoft.Data.SqlClient
- Git and GitHub

## Requirements

Before running the application, ensure that you have:

1. Windows operating system.
2. Visual Studio 2022.
3. .NET 8 SDK.
4. SQL Server Express.
5. SQL Server Management Studio (SSMS).

## Database Setup

1. Start your SQL Server Express service.
2. Open SQL Server Management Studio.
3. Connect using Windows Authentication.
4. Use the server name:

   `.\SQLEXPRESS`

5. Create the database and Tasks table using the following SQL script:

```sql
CREATE DATABASE CybersecurityAwarenessDB;
GO

USE CybersecurityAwarenessDB;
GO

CREATE TABLE Tasks (
    TaskID INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500),
    ReminderDate DATETIME NULL,
    IsCompleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO
```

The application connection string is configured for the local SQL Server Express instance:

`Server=.\SQLEXPRESS;Database=CybersecurityAwarenessDB;Trusted_Connection=True;TrustServerCertificate=True;`

If your SQL Server instance uses a different name, update the connection string in `MainWindow.xaml.cs`.

## Running the Application

1. Clone or download the Part 3 GitHub repository.
2. Open the project solution in Visual Studio 2022.
3. Ensure SQL Server Express is running and the database has been created.
4. Restore the NuGet packages if required.
5. Build the solution.
6. Run the application using F5.

## Example Chatbot Commands

| Command | Description |
|---|---|
| `add task: Complete assignment` | Adds a task |
| `view tasks` | Displays saved tasks |
| `complete task: Complete assignment` | Marks a task as completed |
| `delete task: Complete assignment` | Deletes a task |
| `add reminder: Submit assignment \| 2026-11-20` | Saves a reminder |
| `view reminders` | Displays saved reminders |
| `start quiz` | Starts the cybersecurity quiz |
| `view activity log` | Displays recent activities |
| `show more` | Displays additional activity log entries |

Tasks can also be managed directly through the graphical user interface.

## Project Purpose

The purpose of this project is to promote cybersecurity awareness and encourage safer online behaviour through an interactive desktop application.

It combines educational content, simulated NLP, database operations, task management and interactive learning activities in one application.

## Author

**Ayakha Mangoloti**

Diploma in Information Technology – Software Development

Rosebank College
