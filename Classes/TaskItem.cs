
namespace CybersecurityAwarenessBotPart2
{
    public class TaskItem
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }

        public TaskItem(string title, string description, DateTime? reminderDate)
        {
            Title = title;
            Description = description;
            ReminderDate = reminderDate;
            IsCompleted = false;
        }
    }
}
