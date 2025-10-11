
namespace Portfolio_Projekt_Quest_Tracker
{
    public class QuestManager
    {
        // Attribut
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }
        public bool IsCompleted { get; set; }

        // Konstruktor
        public QuestManager(string title, string description, DateTime dueDate, string priority)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
            IsCompleted = false;
        }

        private static List<QuestManager> quests = new List<QuestManager>(); // lista för alla hjältar


        // --- Skapa nytt uppdrag ---
        public static void AddQuest()
        {
            // === Titel ===
            Console.Write("Enter quest title: ");
            string title = Console.ReadLine();

            // Kolla att användaren inte lämnat titeln tom
            while (string.IsNullOrWhiteSpace(title))
            {
                Console.Write("Title cannot be empty. Please enter a title: ");
                title = Console.ReadLine();
            }

            // === Beskrivning ===
            Console.Write("Enter description: ");
            string description = Console.ReadLine();

            // Om användaren lämnar fältet tomt kan vi sätta en standardbeskrivning
            if (string.IsNullOrWhiteSpace(description))
            {
                description = "No description provided.";
            }

            // === Slutdatum ===
            Console.Write("Enter due date (YYYY-MM-DD): ");
            DateTime dueDate;

            // Validerar att användaren anger ett giltigt datum
            while (!DateTime.TryParse(Console.ReadLine(), out dueDate))
            {
                Console.Write("Invalid date format. Try again (YYYY-MM-DD): ");
            }

            // === Prioritet ===
            Console.Write("Enter priority (High / Medium / Low): ");
            string priority = Console.ReadLine();

            // Säkerställer att användaren anger en giltig prioritet
            while (string.IsNullOrWhiteSpace(priority) ||
                  !(priority.Equals("High", StringComparison.OrdinalIgnoreCase) ||
                    priority.Equals("Medium", StringComparison.OrdinalIgnoreCase) ||
                    priority.Equals("Low", StringComparison.OrdinalIgnoreCase)))
            {
                Console.Write("Invalid priority. Please enter High, Medium, or Low: ");
                priority = Console.ReadLine();
            }

            // === Lägg till uppdraget ===
            // Skapar ett nytt Quest-objekt och lägger till det i listan "quests"
            quests.Add(new QuestManager(title, description, dueDate, priority));

            // === Bekräftelse ===
            Console.WriteLine($"\nQuest '{title}' added successfully!");
            Console.WriteLine($"Description: {description}");
            Console.WriteLine($"Due Date: {dueDate:yyyy-MM-dd}");
            Console.WriteLine($"Priority: {priority}\n");
        }

        // --- Visa alla quests ---
        public static void ShowAllQuests()
        {
            // === Kolla om det finns några uppdrag ===
            if (quests.Count == 0)
            {
                // Om listan är tom, visa ett meddelande och avsluta metoden
                Console.WriteLine("No quests available.\n");
                return;
            }

            // === Rubrik ===
            Console.WriteLine("=== ALL QUESTS ===");

            // === Loopa igenom alla quests i listan ===
            foreach (var quest in quests)
            {
                // Skriver ut information om varje quest
                Console.WriteLine($"Title: {quest.Title}");
                Console.WriteLine($"Description: {quest.Description}");
                Console.WriteLine($"Due Date: {quest.DueDate:yyyy-MM-dd}");
                Console.WriteLine($"Priority: {quest.Priority}");
                Console.WriteLine($"Completed: {(quest.IsCompleted ? "Yes" : "No")}");
                Console.WriteLine("-------------------------");
            }

            // === Summering ===
            Console.WriteLine($"Total quests: {quests.Count}\n");
        }



        // --- Uppdatera eller markera ett uppdrag som slutfört ---
        public static void ManageQuest()
        {
            // === Be användaren ange vilken quest som ska hanteras ===
            Console.Write("Enter the title of the quest to update or complete: ");
            string title = Console.ReadLine();

            // Försök hitta questen i listan (case-insensitive)
            QuestManager quest = quests.FirstOrDefault(q => q.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

            // === Om inget uppdrag hittas ===
            if (quest == null)
            {
                Console.WriteLine("Quest not found.\n");
                return;
            }

            // === Visa meny för vad användaren vill göra ===
            Console.WriteLine($"\nFound quest: {quest.Title}");
            Console.WriteLine("1. Update quest details");
            Console.WriteLine("2. Mark as completed");
            Console.Write("Choose an option (1 or 2): ");
            string choice = Console.ReadLine();

            // === Alternativ 1: Uppdatera detaljer ===
            if (choice == "1")
            {
                Console.WriteLine("\nLeave a field empty to keep the current value.\n");

                // --- Uppdatera beskrivning ---
                Console.Write($"New description ({quest.Description}): ");
                string newDesc = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newDesc))
                    quest.Description = newDesc;

                // --- Uppdatera datum ---
                Console.Write($"New due date ({quest.DueDate:yyyy-MM-dd}): ");
                string newDate = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newDate))
                {
                    if (DateTime.TryParse(newDate, out DateTime parsedDate))
                        quest.DueDate = parsedDate;
                    else
                        Console.WriteLine("Invalid date format. Keeping previous due date.");
                }

                // --- Uppdatera prioritet ---
                Console.Write($"New priority ({quest.Priority}): ");
                string newPriority = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newPriority))
                {
                    if (newPriority.Equals("High", StringComparison.OrdinalIgnoreCase) ||
                        newPriority.Equals("Medium", StringComparison.OrdinalIgnoreCase) ||
                        newPriority.Equals("Low", StringComparison.OrdinalIgnoreCase))
                    {
                        quest.Priority = char.ToUpper(newPriority[0]) + newPriority.Substring(1).ToLower();
                    }
                    else
                    {
                        Console.WriteLine("Invalid priority entered. Keeping previous priority.");
                    }
                }

                Console.WriteLine("\n✅ Quest updated successfully!");
            }

            // === Alternativ 2: Markera som slutfört ===
            else if (choice == "2")
            {
                if (quest.IsCompleted)
                {
                    Console.WriteLine($"Quest '{quest.Title}' is already completed.\n");
                    return;
                }

                Console.Write("Are you sure you want to mark this quest as completed? (yes/no): ");
                string confirm = Console.ReadLine();

                if (confirm.Equals("yes", StringComparison.OrdinalIgnoreCase))
                {
                    quest.IsCompleted = true;
                    Console.WriteLine($"\n🏁 Quest '{quest.Title}' marked as completed!\n");
                }
                else
                {
                    Console.WriteLine("\nAction canceled.\n");
                }
            }

            // === Om användaren skrev något annat ===
            else
            {
                Console.WriteLine("\nInvalid choice. No changes were made.\n");
            }
        }
    }
}
