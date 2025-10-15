using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Portfolio_Projekt_Quest_Tracker
{
    public class QuestManager
    {
        public static List<Quest> quests = new List<Quest>(); // lista för alla quests


        // --- Skapa nytt uppdrag ---
        public static void AddQuest()
        {
            // === Titel ===
            Console.Write("Enter quest title: ");
            string title = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(title))
            {
                Console.Write("Title cannot be empty. Please enter a title: ");
                title = Console.ReadLine();
            }

            // === Beskrivning ===
            Console.WriteLine("Enter quest description (press enter and type 'END' when done):");

            // Bygger upp texten rad för rad
            StringBuilder descriptionBuilder = new StringBuilder();
            string line;
            while ((line = Console.ReadLine()) != null && line.ToUpper() != "END")
            {
                descriptionBuilder.AppendLine(line);
            }

            string description = descriptionBuilder.ToString().Trim();

            if (string.IsNullOrWhiteSpace(description))
                description = "No description provided.";

            // === Slutdatum ===
            Console.Write("Enter due date (YYYY-MM-DD): ");
            DateTime dueDate;
            while (!DateTime.TryParse(Console.ReadLine(), out dueDate))
            {
                Console.Write("Invalid date format. Try again (YYYY-MM-DD): ");
            }

            // === Prioritet ===
            Console.Write("Enter priority (High / Medium / Low): ");
            string priority = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(priority) ||
                  !(priority.Equals("High", StringComparison.OrdinalIgnoreCase) ||
                    priority.Equals("Medium", StringComparison.OrdinalIgnoreCase) ||
                    priority.Equals("Low", StringComparison.OrdinalIgnoreCase)))
            {
                Console.Write("Invalid priority. Please enter High, Medium, or Low: ");
                priority = Console.ReadLine();
            }

            // === Lägg till quest ===
            quests.Add(new Quest(title, description, dueDate, priority));

            Console.WriteLine($"\n✅ Quest '{title}' added successfully!");
            Console.WriteLine($"Due Date: {dueDate:yyyy-MM-dd}");
            Console.WriteLine($"Priority: {priority}\n");
        }



        // --- Visa alla quests ---
        public static void ShowAllQuests()
        {
            if (quests.Count == 0)
            {
                Console.WriteLine("No quests available.\n");
                return;
            }

            Console.WriteLine("=== ALL QUESTS ===");
            foreach (var quest in quests)
            {
                Console.WriteLine($"Title: {quest.Title}");
                Console.WriteLine($"Description:\n{quest.Description}");
                Console.WriteLine($"Due Date: {quest.DueDate:yyyy-MM-dd}");
                Console.WriteLine($"Priority: {quest.Priority}");
                Console.WriteLine($"Completed: {(quest.IsCompleted ? "Yes" : "No")}");
                Console.WriteLine("-------------------------");
            }

            Console.WriteLine($"Total quests: {quests.Count}\n");
        }



        // --- Uppdatera eller markera ett uppdrag som slutfört ---
        public static void ManageQuest()
        {
            Console.Write("Enter the title of the quest to update or complete: ");
            string title = Console.ReadLine();

            Quest quest = quests.FirstOrDefault(q => q.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

            if (quest == null)
            {
                Console.WriteLine("Quest not found.\n");
                return;
            }

            Console.WriteLine($"\nFound quest: {quest.Title}");
            Console.WriteLine("1. Update quest details");
            Console.WriteLine("2. Mark as completed");
            Console.Write("Choose an option (1 or 2): ");
            string choice = Console.ReadLine();

            // === Uppdatera detaljer ===
            if (choice == "1")
            {
                Console.WriteLine("\nLeave a field empty to keep the current value.\n");

                // --- Uppdatera beskrivning ---
                Console.WriteLine($"Current description:\n{quest.Description}");
                Console.WriteLine("Enter new description (paste multiple lines, type 'END' when done or leave empty to skip):");

                StringBuilder newDescBuilder = new StringBuilder();
                string line;
                while ((line = Console.ReadLine()) != null && line.ToUpper() != "END")
                {
                    newDescBuilder.AppendLine(line);
                }

                string newDesc = newDescBuilder.ToString().Trim();
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

                Console.WriteLine("\n✅ Quest updated successfully!\n");
            }

            // === Markera som klar ===
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
            else
            {
                Console.WriteLine("\nInvalid choice. No changes were made.\n");
            }
        }
    }
}

