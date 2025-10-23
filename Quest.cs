using System;

namespace Portfolio_Projekt_Quest_Tracker
{
    public class Quest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }
        public bool IsCompleted { get; set; }

        // Ny flagga – används för att inte skicka dubbla notiser
        public bool DeadlineNotified { get; set; } = false;

        public Quest() { }

        public Quest(string title, string description, DateTime dueDate, string priority)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
            IsCompleted = false;
        }
    }
}
