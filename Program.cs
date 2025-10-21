using Spectre.Console;
using System;
using System.Threading.Tasks;

namespace Portfolio_Projekt_Quest_Tracker
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Initiera services
            var notifier = new NotificationService();
            var ai = new GuildAdvisorAI();

            bool running = true;
            User loggedInUser = null;

            // --- Huvudloop ---
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
                            QuestManager.AddQuest();
                            break;

                        case "View All Quests":
                            QuestManager.ShowAllQuests();
                            break;

                        case "Update/Complete Quest":
                            QuestManager.ManageQuest();
                            break;

                        case "Guild Advisor (AI)":
                            await ai.InteractWithUserAsync();
                            break;

                        case "Guild Report":
                            await QuestManager.ShowGuildReportAsync(notifier, loggedInUser.PhoneNumber);
                            break;

                        case "Logout":
                            loggedInUser = null;
                            break;
                    }
                }
            }
        }
    }
}
