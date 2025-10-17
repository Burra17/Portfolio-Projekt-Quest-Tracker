using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Portfolio_Projekt_Quest_Tracker
{
    public class QuestManager
    {
        // === Lista över alla quests (lagras i minnet under körning) ===
        public static List<Quest> quests = new List<Quest>();


        // === SKAPA NYTT UPPDRAG ===
        public static void AddQuest()
        {
            try
            {
                Console.Clear(); // Rensar konsolen innan vi börjar
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=== CREATE NEW QUEST ===\n");
                Console.ResetColor();

                // --- Titel ---
                Console.Write("Enter quest title: ");
                string title = Console.ReadLine();

                // Loop tills användaren anger en giltig titel
                while (string.IsNullOrWhiteSpace(title))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Title cannot be empty. Please enter a title: ");
                    Console.ResetColor();
                    title = Console.ReadLine();
                }

                // --- Beskrivning ---
                Console.WriteLine("\nEnter quest description (press Enter and type 'END' when done):");
                StringBuilder descriptionBuilder = new StringBuilder();
                string line;

                // Tar in flera rader tills användaren skriver END
                while ((line = Console.ReadLine()) != null && line.ToUpper() != "END")
                {
                    descriptionBuilder.AppendLine(line);
                }

                string description = descriptionBuilder.ToString().Trim();
                if (string.IsNullOrWhiteSpace(description))
                    description = "No description provided.";

                // --- Slutdatum ---
                Console.Write("\nEnter due date (YYYY-MM-DD): ");
                DateTime dueDate;

                // Loopar tills giltigt datum anges
                while (!DateTime.TryParse(Console.ReadLine(), out dueDate))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Invalid date format. Try again (YYYY-MM-DD): ");
                    Console.ResetColor();
                }

                // --- Prioritet ---
                Console.Write("Enter priority (High / Medium / Low): ");
                string priority = Console.ReadLine();

                // Loop tills giltig prioritet anges
                while (string.IsNullOrWhiteSpace(priority) ||
                      !(priority.Equals("High", StringComparison.OrdinalIgnoreCase) ||
                        priority.Equals("Medium", StringComparison.OrdinalIgnoreCase) ||
                        priority.Equals("Low", StringComparison.OrdinalIgnoreCase)))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Invalid priority. Please enter High, Medium, or Low: ");
                    Console.ResetColor();
                    priority = Console.ReadLine();
                }

                // --- Skapa och lägg till quest ---
                quests.Add(new Quest(title, description, dueDate, priority));

                // --- Bekräftelse ---
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✅ Quest '{title}' added successfully!");
                Console.ResetColor();

                Console.WriteLine($"Due Date: {dueDate:yyyy-MM-dd}");
                Console.WriteLine($"Priority: {priority}");
                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                // Fångar eventuella fel
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nAn error occurred: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }


        // === VISA ALLA QUESTS ===
        public static void ShowAllQuests()
        {
            try
            {
                Console.Clear(); // Rensar terminalen innan listan skrivs ut
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("=== ALL QUESTS ===\n");
                Console.ResetColor();

                // Om inga quests finns
                if (quests.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("No quests available.\n");
                    Console.ResetColor();
                }
                else
                {
                    // Loopa igenom och skriv ut varje quest
                    foreach (var quest in quests)
                    {
                        Console.ForegroundColor = quest.IsCompleted ? ConsoleColor.DarkGreen : ConsoleColor.White;
                        Console.WriteLine($"Title: {quest.Title}");
                        Console.ResetColor();

                        Console.WriteLine($"Description:\n{quest.Description}");
                        Console.WriteLine($"Due Date: {quest.DueDate:yyyy-MM-dd}");
                        Console.WriteLine($"Priority: {quest.Priority}");
                        Console.WriteLine($"Completed: {(quest.IsCompleted ? "✅ Yes" : "❌ No")}");
                        Console.WriteLine(new string('-', 40));
                    }

                    // Visar antal quests totalt
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\nTotal quests: {quests.Count}");
                    Console.ResetColor();
                }

                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                // Felhantering vid utskrift
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nAn error occurred while displaying quests: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }


        // === HANTERA QUEST (uppdatera eller markera som klar) ===
        public static void ManageQuest()
        {
            try
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("=== MANAGE QUEST ===\n");
                Console.ResetColor();

                // Fråga efter quest-titel
                Console.Write("Enter the title of the quest to update or complete: ");
                string title = Console.ReadLine();

                // Hitta rätt quest i listan
                Quest quest = quests.FirstOrDefault(q => q.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

                if (quest == null)
                {
                    // Om inget matchande quest hittades
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nQuest not found.\n");
                    Console.ResetColor();
                    Console.WriteLine("Press any key to return...");
                    Console.ReadKey();
                    return;
                }

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Found quest: {quest.Title}\n");
                Console.ResetColor();

                Console.WriteLine("1. Update quest details");
                Console.WriteLine("2. Mark as completed");
                Console.Write("Choose an option (1 or 2): ");
                string choice = Console.ReadLine();

                // === VAL 1: Uppdatera quest ===
                if (choice == "1")
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"=== UPDATE QUEST: {quest.Title} ===\n");
                    Console.ResetColor();
                    Console.WriteLine("Leave a field empty to keep the current value.\n");

                    // --- Uppdatera beskrivning ---
                    Console.WriteLine($"Current description:\n{quest.Description}");
                    Console.WriteLine("\nEnter new description (type 'END' when done or leave empty to skip):");

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
                    if (!string.IsNullOrWhiteSpace(newPriority) &&
                        (newPriority.Equals("High", StringComparison.OrdinalIgnoreCase) ||
                         newPriority.Equals("Medium", StringComparison.OrdinalIgnoreCase) ||
                         newPriority.Equals("Low", StringComparison.OrdinalIgnoreCase)))
                    {
                        quest.Priority = char.ToUpper(newPriority[0]) + newPriority.Substring(1).ToLower();
                    }

                    // Bekräftelse på uppdatering
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n✅ Quest updated successfully!\n");
                    Console.ResetColor();

                    // Visa uppdaterad quest
                    Console.WriteLine($"Title: {quest.Title}");
                    Console.WriteLine($"Description:\n{quest.Description}");
                    Console.WriteLine($"Due Date: {quest.DueDate:yyyy-MM-dd}");
                    Console.WriteLine($"Priority: {quest.Priority}");
                    Console.WriteLine($"Completed: {(quest.IsCompleted ? "Yes" : "No")}");
                    Console.WriteLine(new string('-', 40));

                    Console.WriteLine("Press any key to return...");
                    Console.ReadKey();
                }

                // === VAL 2: Markera som klar ===
                else if (choice == "2")
                {
                    Console.Clear();

                    // Om quest redan är klart
                    if (quest.IsCompleted)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Quest '{quest.Title}' is already completed.\n");
                        Console.ResetColor();
                        Console.WriteLine("Press any key to return...");
                        Console.ReadKey();
                        return;
                    }

                    // Bekräftelse innan slutförande
                    Console.Write("Are you sure you want to mark this quest as completed? (yes/no): ");
                    string confirm = Console.ReadLine();

                    if (confirm.Equals("yes", StringComparison.OrdinalIgnoreCase))
                    {
                        // Markera som klart
                        quest.IsCompleted = true;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\n🏁 Quest '{quest.Title}' marked as completed!\n");
                        Console.ResetColor();

                        // Visa information om questet efter ändring
                        Console.WriteLine($"Title: {quest.Title}");
                        Console.WriteLine($"Description:\n{quest.Description}");
                        Console.WriteLine($"Due Date: {quest.DueDate:yyyy-MM-dd}");
                        Console.WriteLine($"Priority: {quest.Priority}");
                        Console.WriteLine($"Completed: ✅ Yes");
                        Console.WriteLine(new string('-', 40));

                        Console.WriteLine("Press any key to return...");
                        Console.ReadKey();
                    }
                    else
                    {
                        // Om användaren ångrar sig
                        Console.WriteLine("\nAction canceled.\n");
                        Console.WriteLine("Press any key to return...");
                        Console.ReadKey();
                    }
                }

                // === Ogiltigt val ===
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nInvalid choice. No changes were made.\n");
                    Console.ResetColor();
                    Console.WriteLine("Press any key to return...");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                // Felhantering
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nAn error occurred: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
            }
        }
    }
}
