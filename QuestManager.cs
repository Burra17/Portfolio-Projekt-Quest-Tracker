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
        // --- Lägg till nytt quest ---
        public static void AddQuest(User user)
        {
            try
            {
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("[bold cyan]=== CREATE NEW QUEST ===[/]\n");

                Console.Write("Enter quest title: ");
                string title = Console.ReadLine();

                Console.Write("Enter quest description: ");
                string description = Console.ReadLine();

                Console.Write("Enter due date (YYYY-MM-DD): ");
                DateTime dueDate = DateTime.Parse(Console.ReadLine());

                Console.Write("Enter priority (High / Medium / Low): ");
                string priority = Console.ReadLine();

                var quest = new Quest(title, description, dueDate, priority);
                user.Quests.Add(quest);

                Authenticator.SaveAll();

                AnsiConsole.MarkupLine($"[green]Quest '{title}' added successfully![/]");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
                Console.ReadKey();
            }
        }

        // --- Visa alla quests ---
        public static void ShowAllQuests(User user)
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine($"[bold yellow]=== {user.Username}'s Quests ===[/]\n");

            if (user.Quests.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]No quests yet.[/]");
                Console.ReadKey();
                return;
            }

            foreach (var quest in user.Quests)
            {
                string status = quest.IsCompleted ? "[green] Completed[/]" : "[white] Active[/]";
                var panel = new Panel($"[bold]{quest.Title}[/]\n{quest.Description}\nDue: {quest.DueDate:yyyy-MM-dd}\nPriority: {quest.Priority}\nStatus: {status}")
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(Style.Parse("cyan"));
                AnsiConsole.Write(panel);
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        // --- Hantera quest ---
        public static void ManageQuest(User user)
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold magenta]=== MANAGE QUEST ===[/]\n");

            if (user.Quests.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]No quests to manage.[/]");
                Console.ReadKey();
                return;
            }

            var titles = user.Quests.Select(q => q.Title).ToList();
            string selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select a quest to update or complete:")
                    .AddChoices(titles));

            var quest = user.Quests.First(q => q.Title == selected);

            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose an action:")
                    .AddChoices("Mark as completed", "Edit quest", "Cancel"));

            if (action == "Mark as completed")
            {
                quest.IsCompleted = true;
                AnsiConsole.MarkupLine($"[green]Quest '{quest.Title}' marked as completed![/]");
            }
            else if (action == "Edit quest")
            {
                Console.Write("New description (leave empty to keep): ");
                string desc = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(desc))
                    quest.Description = desc;

                Console.Write("New due date (leave empty to keep): ");
                string dateInput = Console.ReadLine();
                if (DateTime.TryParse(dateInput, out DateTime newDate))
                    quest.DueDate = newDate;

                Console.Write("New priority (leave empty to keep): ");
                string prio = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(prio))
                    quest.Priority = prio;
            }

            Authenticator.SaveAll();
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        // --- Guild report (visar bara info, skickar inte notiser längre) ---
        public static async Task ShowGuildReportAsync(NotificationService notifier, User user)
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine($"[bold yellow]=== {user.Username}'s Guild Report ===[/]\n");

            int total = user.Quests.Count;
            int completed = user.Quests.Count(q => q.IsCompleted);
            int active = total - completed;
            int nearDeadline = user.Quests.Count(q => !q.IsCompleted && (q.DueDate - DateTime.Now).TotalHours <= 24);

            AnsiConsole.MarkupLine($"Total quests: [bold]{total}[/]");
            AnsiConsole.MarkupLine($"Completed: [green]{completed}[/]");
            AnsiConsole.MarkupLine($"Active: [white]{active}[/]");
            AnsiConsole.MarkupLine($"Near deadline (<24h): [red]{nearDeadline}[/]");

            Console.WriteLine("\nPress Enter to return...");
            Console.ReadLine();
        }

        // --- Ny metod: kolla deadlines vid inloggning ---
        public static async Task CheckDeadlinesOnLoginAsync(User user, NotificationService notifier)
        {
            var soonDue = user.Quests
                .Where(q => !q.IsCompleted && !q.DeadlineNotified && (q.DueDate - DateTime.Now).TotalHours <= 24)
                .ToList();

            if (soonDue.Count == 0)
                return;

            foreach (var quest in soonDue)
            {
                await notifier.SendDeadlineNotificationAsync(quest, user.PhoneNumber);
                quest.DeadlineNotified = true;
            }

            Authenticator.SaveAll(); // Spara att notiser skickats
        }
    }
}
