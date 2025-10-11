namespace Portfolio_Projekt_Quest_Tracker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool running = true; // För att enkelt kunna avsluta while-loopen
            User loggedInUser = null; // För att hålla koll när användare är inloggad

            while (running)
            {
                if (loggedInUser == null) // Loopar huvudmenyn så länge användaren inte är inloggad
                {
                    MenuHelper.MainMenu(); // Skriver ut huvudmeny
                    string choice = Console.ReadLine();

                    switch (choice) // switch sats baserat på användarens val
                    {
                        case "1":
                            User.RegisterHero();
                            break;
                        case "2":
                            loggedInUser = User.LoginHero();
                            break;
                        case "3":
                            running = false;
                            Console.WriteLine("Exiting program...");
                            break;
                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }
                }
                else
                {
                    MenuHelper.HeroMenu(loggedInUser.Username); // Skriver ut menyn för hjälte som är inloggad
                    string heroChoice = Console.ReadLine();

                    switch (heroChoice) // Switch sats på samma sätt som tar emot det användaren väljer
                    {
                        case "1":
                            QuestManager.AddQuest();
                            break;
                        case "2":
                            QuestManager.ShowAllQuests();
                            break;
                        case "3":
                            QuestManager.ManageQuest();
                            break;
                        //case "4":
                        //Request Guild advisor help (ai)
                        //break
                        //case "5":
                        //Show guild report

                        case "6":
                            loggedInUser = null; // Logga ut användare, går tillbaka till huvudmenyn
                            Console.WriteLine("You have logged out.\n");
                            break;
                        default:
                            Console.WriteLine("Feature coming soon..."); // Tillfälligt för att jag inte gjort klart metoderna
                            break;
                    }
                }
            }
        }
    }
}
