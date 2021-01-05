using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangman;

namespace Hangman
{
    //potential selections for game separated by category
    class Words
    {
        static string[] objects = new string[] { "lampshade", "thermometer", "tweezers", "television", "microwave", "extinguisher", "tambourine", "screwdriver", "cobweb", "boxcar", "vodka", "wristwatch", "knapsack"};

        static string[] places = new string[] { "Casablanca", "Bucharest", "Philadelphia", "Birmingham", "Yellowknife", "Phoenix", "Istanbul", "Vancouver", "Gatineau", "Nanaimo", "Falkland Islands"};

        static string[] people = new string[] { "William Tell", "Elizabeth Bathory", "Julius Caesar", "Julius Oppenheimer", "Wilt Chamberlain", "Wayne Gretzky", "Pierre Trudeau", "Attila the Hun", "Neil DeGrasse Tyson", "Weird Al Yankovic", "Isaac Newton", "Muhammad Ali", "Stephen King", "Edgar Allan Poe", "Marilyn Monroe", "Nikola Tesla", "Joan of Arc", "Jane Austen", "Emily Dickenson", "Marie Antoinette" };



        public static string SelectWord(int result)
        {
            //select word from category based on int passed from MainPage's RandomNumber method

            if (result == 1)
            {
                Random random = new Random();
                int selection = random.Next(objects.Length);
                string SelectedWord = objects[selection];
                return SelectedWord;
            }
            else if (result == 2)
            {
                Random random = new Random();
                int selection = random.Next(places.Length);
                string SelectedWord = places[selection];
                return SelectedWord;
            }
            else
            {
                Random random = new Random();
                int selection = random.Next(people.Length);
                string SelectedWord = people[selection];
                return SelectedWord;
            }
        }
    }
}
