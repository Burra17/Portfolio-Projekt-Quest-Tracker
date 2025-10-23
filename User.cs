using System;
using System.Collections.Generic;

namespace Portfolio_Projekt_Quest_Tracker
{
    /// <summary>
    /// Representerar en hjälte (användare) med namn, lösenord, telefonnummer och personliga quests.
    /// </summary>
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public List<Quest> Quests { get; set; } = new List<Quest>(); // Hjältens personliga uppdrag

        // Parameterlös konstruktor krävs för JSON-deserialisering
        public User() { }

        // Konstruktor för att skapa nya användare
        public User(string username, string password, string phone)
        {
            Username = username;
            Password = password;
            PhoneNumber = phone;
        }
    }
}
