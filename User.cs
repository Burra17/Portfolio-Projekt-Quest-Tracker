
using System.Text.RegularExpressions;

namespace Portfolio_Projekt_Quest_Tracker
{
    public class User
    {
        // --- Egenskaper ---
        public string Username { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }

        // --- Konstruktor ---
        public User(string username, string password, string phoneNumber)
        {
            Username = username;
            Password = password;
            PhoneNumber = phoneNumber;
        }
    }
}