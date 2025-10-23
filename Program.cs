using Spectre.Console;
using System;
using System.Threading.Tasks;

namespace Portfolio_Projekt_Quest_Tracker
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var notifier = new NotificationService();
            var ai = new GuildAdvisorAI();
            bool running = true;
            User loggedInUser = null;

            while (running)
            {
                if (loggedInUser == null)
                {
                    string choice = MenuHelper.ShowMainMenu();

                    switch (choice)
                    {
                        case "Register Hero":
                            Authenticator.RegisterHero();
                            break;

                        case "Login Hero":
                            loggedInUser = Authenticator.LoginHero();
                            if (loggedInUser != null)
                            {
                                // ✅ Kolla deadlines direkt vid inloggning
                                await QuestManager.CheckDeadlinesOnLoginAsync(loggedInUser, notifier);
                            }
                            break;

                        case "Exit Program":
                            running = false;
                            break;
                    }
                }
                else
                {
                    string heroChoice = MenuHelper.ShowHeroMenu(loggedInUser.Username);

                    switch (heroChoice)
                    {
                        case "Add New Quest":
                            QuestManager.AddQuest(loggedInUser);
                            break;

                        case "View All Quests":
                            QuestManager.ShowAllQuests(loggedInUser);
                            break;

                        case "Update/Complete Quest":
                            QuestManager.ManageQuest(loggedInUser);
                            break;

                        case "Guild Advisor (AI)":
                            await ai.InteractWithUserAsync(loggedInUser);
                            break;

                        case "Guild Report":
                            await QuestManager.ShowGuildReportAsync(notifier, loggedInUser);
                            break;

                        case "Logout":
                            loggedInUser = null;
                            break;
                    }
                }
            }

            // Spara innan avslut
            Authenticator.SaveAll();
            AnsiConsole.MarkupLine("[grey]💾 Data saved. Goodbye hero![/]");
        }
    }
}
