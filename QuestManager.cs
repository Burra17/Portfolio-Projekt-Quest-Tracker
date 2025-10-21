using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("[bold cyan]=== CREATE NEW QUEST ===[/]\n");

                // --- Titel ---
                string title;
                do
                {
                    Console.Write("Enter quest title: ");
                    title = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(title))
                        AnsiConsole.MarkupLine("[red]Title cannot be empty.[/]");
                } while (string.IsNullOrWhiteSpace(title));

                // --- Beskrivning ---
                AnsiConsole.MarkupLine("\nEnter quest description (press Enter and type 'END' when done):");
                StringBuilder descBuilder = new StringBuilder();
                string line;
                while ((line = Console.ReadLine()) != null && line.ToUpper() != "END")
                    descBuilder.AppendLine(line);

                string description = string.IsNullOrWhiteSpace(descBuilder.ToString()) ? "No description provided." : descBuilder.ToString().Trim();

                // --- Deadline ---
                DateTime dueDate;
                while (true)
                {
                    Console.Write("\nEnter due date (YYYY-MM-DD): ");
                    if (DateTime.TryParse(Console.ReadLine(), out dueDate))
                        break;
                    AnsiConsole.MarkupLine("[red]Invalid date format.[/]");
                }

                // --- Prioritet ---
                string priority;
                while (true)
                {
                    Console.Write("Enter priority (High / Medium / Low): ");
                    priority = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(priority) &&
                        (priority.Equals("High", StringComparison.OrdinalIgnoreCase) ||
                         priority.Equals("Medium", StringComparison.OrdinalIgnoreCase) ||
                         priority.Equals("Low", StringComparison.OrdinalIgnoreCase)))
                        break;
                    AnsiConsole.MarkupLine("[red]Invalid priority. Use High, Medium or Low.[/]");
                }

                // --- Lägg till quest ---
                quests.Add(new Quest(title, description, dueDate, char.ToUpper(priority[0]) + priority.Substring(1).ToLower()));

                // --- Bekräftelse panel ---
                var panel = new Panel($"✅ Quest '[bold]{title}[/]' added successfully!\nDue: {dueDate:yyyy-MM-dd}, Priority: {priority}")
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(Style.Parse("green"))
                    .Header("[bold green]SUCCESS[/]")
                    .HeaderAlignment(Justify.Center);
                AnsiConsole.Write(panel);

                Console.WriteLine("\nPress Enter to return to the menu...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An error occurred: {ex.Message}[/]");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
        }

        // === VISA ALLA QUESTS ===
        public static void ShowAllQuests()
        {
            try
            {
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("[bold yellow]=== ALL QUESTS ===[/]\n");

                if (quests.Count == 0)
                {
                    AnsiConsole.MarkupLine("[grey]No quests available.[/]");
                }
                else
                {
                    foreach (var quest in quests)
                    {
                        string status = quest.IsCompleted ? "[green]✅ Completed[/]" : "[white]❌ Active[/]";
                        var panel = new Panel($"[bold]{quest.Title}[/]\n{quest.Description}\nDue: {quest.DueDate:yyyy-MM-dd}\nPriority: {quest.Priority}\nStatus: {status}")
                            .Border(BoxBorder.Rounded)
                            .BorderStyle(Style.Parse("cyan"));
                        AnsiConsole.Write(panel);
                    }

                    AnsiConsole.MarkupLine($"\nTotal quests: [bold]{quests.Count}[/]");
                }

                Console.WriteLine("\nPress Enter to return to the menu...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An error occurred while displaying quests: {ex.Message}[/]");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
        }

        // === HANTERA QUEST (uppdatera eller markera som klar) ===
        public static void ManageQuest()
        {
            try
            {
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("[bold magenta]=== MANAGE QUEST ===[/]\n");

                // --- Visa questtitlar som lista för enklare val ---
                if (quests.Count == 0)
                {
                    AnsiConsole.MarkupLine("[grey]No quests available to manage.[/]");
                    Console.WriteLine("Press Enter to return...");
                    Console.ReadLine();
                    return;
                }

                var titles = quests.Select(q => q.Title).ToList();
                string selectedTitle = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select a quest to update or complete:")
                        .AddChoices(titles)
                );

                Quest quest = quests.First(q => q.Title.Equals(selectedTitle, StringComparison.OrdinalIgnoreCase));

                AnsiConsole.Clear();
                AnsiConsole.MarkupLine($"[cyan]Selected quest: {quest.Title}[/]\n");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose an action:")
                        .AddChoices("Update quest details", "Mark as completed")
                );

                if (choice == "Update quest details")
                {
                    AnsiConsole.MarkupLine("[bold]Leave empty to keep current value.[/]\n");

                    // --- Uppdatera beskrivning ---
                    Console.WriteLine($"Current description:\n{quest.Description}");
                    Console.WriteLine("Enter new description (type 'END' when done or leave empty to skip):");
                    string firstLine = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(firstLine))
                    {
                        StringBuilder newDescBuilder = new StringBuilder();
                        newDescBuilder.AppendLine(firstLine);
                        string line;
                        while ((line = Console.ReadLine()) != null && line.ToUpper() != "END")
                            newDescBuilder.AppendLine(line);

                        string newDesc = newDescBuilder.ToString().Trim();
                        if (!string.IsNullOrWhiteSpace(newDesc))
                            quest.Description = newDesc;
                    }

                    // --- Uppdatera datum ---
                    Console.Write($"New due date ({quest.DueDate:yyyy-MM-dd}): ");
                    string newDate = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newDate) && DateTime.TryParse(newDate, out DateTime parsedDate))
                        quest.DueDate = parsedDate;

                    // --- Uppdatera prioritet ---
                    Console.Write($"New priority ({quest.Priority}): ");
                    string newPriority = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newPriority) &&
                        (newPriority.Equals("High", StringComparison.OrdinalIgnoreCase) ||
                         newPriority.Equals("Medium", StringComparison.OrdinalIgnoreCase) ||
                         newPriority.Equals("Low", StringComparison.OrdinalIgnoreCase)))
                        quest.Priority = char.ToUpper(newPriority[0]) + newPriority.Substring(1).ToLower();

                    AnsiConsole.MarkupLine("[green]✅ Quest updated successfully![/]");
                }
                else if (choice == "Mark as completed")
                {
                    if (!quest.IsCompleted)
                        quest.IsCompleted = true;
                    AnsiConsole.MarkupLine($"[green]🏁 Quest '{quest.Title}' marked as completed![/]");
                }

                Console.WriteLine("\nPress Enter to return to the menu...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An error occurred: {ex.Message}[/]");
                Console.WriteLine("Press Enter to return...");
                Console.ReadLine();
            }
        }

        // === VISA GUILD REPORT OCH SKICKA NOTISER ===
        public static async Task ShowGuildReportAsync(NotificationService notifier, string userPhone)
        {
            try
            {
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("[bold yellow]=== GUILD REPORT ===[/]\n");

                int total = quests.Count;
                int completed = quests.Count(q => q.IsCompleted);
                int active = total - completed;
                int nearDeadline = quests.Count(q => !q.IsCompleted && (q.DueDate - DateTime.Now).TotalHours <= 24);

                AnsiConsole.MarkupLine($"Total quests: [bold]{total}[/]");
                AnsiConsole.MarkupLine($"Completed quests: [green]{completed}[/]");
                AnsiConsole.MarkupLine($"Active quests: [white]{active}[/]");
                AnsiConsole.MarkupLine($"Quests near deadline (<24h): [red]{nearDeadline}[/]");

                // --- Skicka notiser direkt ---
                foreach (var quest in quests.Where(q => !q.IsCompleted && (q.DueDate - DateTime.Now).TotalHours <= 24))
                {
                    await notifier.SendSmsAsync(userPhone, $"⚔️ Hjälte, ditt uppdrag '{quest.Title}' måste slutföras inom 24 timmar!");

                    var panel = new Panel($"Notification sent for '{quest.Title}'!")
                        .Border(BoxBorder.Rounded)
                        .BorderStyle(Style.Parse("green"))
                        .Header("[bold green]ALERT[/]")
                        .HeaderAlignment(Justify.Center);
                    AnsiConsole.Write(panel);
                }

                Console.WriteLine("\nPress Enter to return to the menu...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An error occurred while showing guild report: {ex.Message}[/]");
                Console.WriteLine("Press Enter to return...");
                Console.ReadLine();
            }
        }
    }
}
