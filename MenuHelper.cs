using Spectre.Console;

namespace Portfolio_Projekt_Quest_Tracker
{
    public static class MenuHelper
    {
        // --- Huvudmeny för icke-inloggad användare ---
        public static string ShowMainMenu()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("Quest Tracker").Centered().Color(Color.Green));

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Main Menu[/]")
                    .PageSize(5)
                    .AddChoices("Register Hero", "Login Hero", "Exit Program")
            );

            return choice;
        }

        // --- Hjältemeny för inloggad användare ---
        public static string ShowHeroMenu(string heroName)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText($"Welcome {heroName}").Centered().Color(Color.Cyan1));

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Hero Menu[/]")
                    .PageSize(8)
                    .AddChoices(
                        "Add New Quest",
                        "View All Quests",
                        "Update/Complete Quest",
                        "Guild Advisor (AI)",
                        "Guild Report",
                        "Logout"
                    )
            );

            return choice;
        }
    }
}
