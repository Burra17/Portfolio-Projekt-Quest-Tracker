
using System.Text.RegularExpressions;

namespace Portfolio_Projekt_Quest_Tracker
{
    public class User
    {
        // --- Egenskaper ---
        public string Username { get; set; }
        public string Password { get; set; }

        // --- Konstruktor ---
        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}