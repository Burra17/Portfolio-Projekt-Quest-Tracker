namespace Portfolio_Projekt_Quest_Tracker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool running = true;
            User loggedInUser = null;

            while (running)
            {
                if (loggedInUser == null)
                {
                    MenuHelper.MainMenu();
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            User.RegisterHero();
                            break;
                        case "2":
                            loggedInUser = User.LoginHero();
                            break;
                        case "3":
                            running = false;
                            break;
                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }
                }
                else
                {
                    MenuHelper.HeroMenu(loggedInUser.Username);
                    string heroChoice = Console.ReadLine();

                    switch (heroChoice)
                    {
                        case "6":
                            loggedInUser = null;
                            Console.WriteLine("You have logged out.\n");
                            break;
                        default:
                            Console.WriteLine("Feature coming soon...");
                            break;
                    }
                }
            }
        }
    }
}
