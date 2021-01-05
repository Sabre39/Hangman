using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Hangman;
using Windows.Media.Playback;
using Windows.Media.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Hangman
{
    /// <summary>
    /// Assignment completed by Nathan Smith. Images provided by:           
    ///https://commons.wikimedia.org/wiki/File:Hangman-0.png
    ///https://commons.wikimedia.org/wiki/File:Hangman-1.png
    ///https://commons.wikimedia.org/wiki/File:Hangman-2.png
    ///https://commons.wikimedia.org/wiki/File:Hangman-3.png
    ///https://commons.wikimedia.org/wiki/File:Hangman-4.png
    ///https://commons.wikimedia.org/wiki/File:Hangman-5.png
    ///https://commons.wikimedia.org/wiki/File:Hangman-6.png
    /// by user Demi -->
    /// Audio provided by freesound.org
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MediaPlayer player;
        public MainPage()
        {
            this.InitializeComponent();
            player = new MediaPlayer();
        }

        string word;
        char[] wordArray;
        char[] hiddenArray;
        int missCount = 0;
        bool warning = false;

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            //generate an integer using RandomNumber method, pass it to SelectWord function in Words.cs in order to generate a category and word
            int result = RandomNumber();
            word = Words.SelectWord(result);

            //break selected word into array, hiddenArray is the selected word converted to underscores
            wordArray = word.ToCharArray();
            hiddenArray = wordArray;
            StartUp();

            //for loop that changes letters to underscores and keeps spaces in place
            for (int i = 0; i < wordArray.Length; i++)
            {
                if (hiddenArray[i] != " ".ToCharArray()[0])
                {

                    hiddenArray[i] = "_".ToCharArray()[0];
                }
                else
                {
                    hiddenArray[i] = " ".ToCharArray()[0];
                }
            }
            //convert char array back to a string and put it in textbox
            string hiddenWord = new string(hiddenArray);
            txtSelectedWord.Text = hiddenWord;

            //check value of result and provide user the category for the word selected
            if (result == 1)
            {
                txtCategoryName.Text = "Object";
            }
            else if (result == 2)
            {
                txtCategoryName.Text = "Location";
            }
            else if (result == 3)
            {
                txtCategoryName.Text = "Person";
            }
            //wordArray was being converted to underscores as well, so this changes it back to a character array of the chosen word
            wordArray = word.ToCharArray();
        }

        private void btnGuessLetter_Click(object sender, RoutedEventArgs e)
        {
            //this bool will check if we get a match in the selected word based on the user's letter guess
            bool match = false;

            //perform checks for bad input (special character check adapted from https://stackoverflow.com/questions/4503542/check-for-special-characters-in-a-string answer from Emanuel Faisca) 
            string specialChar = @"\|!#$%&/()=?»«@£§€{}.*^+[]:-;'\<>_, ";

            foreach (char item in specialChar)
            {
                if (txtLetterGuess.Text.Contains(item) == true)
                {
                    tbError.Text = "Guess should not contain special characters";
                    txtLetterGuess.Text = "";
                    txtLetterGuess.Focus(FocusState.Programmatic);
                    return;
                }
            }
                if (int.TryParse(txtLetterGuess.Text, out int result))
                {
                    tbError.Text = "Guess should not contain a number";
                    txtLetterGuess.Text = "";
                }
                else
                //if input is good, loop through wordArray, and if match is found, add guessed character to that spot
                {
                    tbError.Text = "";
                    string userGuess = txtLetterGuess.Text;

                    for (int i = 0; i < wordArray.Length; i++)
                    {
                        if (userGuess.ToUpper().ToCharArray()[0] == wordArray[i] || userGuess.ToLower().ToCharArray()[0] == wordArray[i])
                        {
                            hiddenArray[i] = userGuess.ToCharArray()[0];
                            match = true;
                        }
                    }

                    //if match is not found, check if letter was previously guessed. If so, display error. If not, add to count, add letter to wrong guesses list and display next body part
                    if (match == false)
                    {
                        if (tbWrongGuesses.Text.Contains(userGuess.ToLower()) || tbWrongGuesses.Text.Contains(userGuess.ToUpper()))
                        {
                            tbError.Text = "Letter already guessed";
                        }
                        else
                        {
                            missCount += 1;
                            tbWrongGuesses.Text += ' ' + userGuess;
                            BodyReveal();
                        }
                    }
                //warning that next wrong guess results in loss
                if (missCount == 5 && warning == false)
                    {
                        Warning();
                    }
                    //game loss - disable further guessing
                    if (missCount == 6)
                    {
                        tbError.Text = "You lose. Click new game to play again!";
                        GameOver();
                    }
            }
                //convert char array to string, put string through ProperCase method to capitalize first letters
            string revealedWord = new string(hiddenArray);
            txtSelectedWord.Text = ProperCase(revealedWord);
            txtLetterGuess.Text = "";
            txtLetterGuess.Focus(FocusState.Programmatic);

            //checks if all letters have been guessed. If there are no underscores, win condition
            if (revealedWord.Contains('_') == false) 
            {
                tbError.Foreground = new SolidColorBrush(Colors.Green);
                tbError.Text = "You win! Click New Game to play again.";
                GameOver();
            }
        }

        #region 

        //generates a random number from which we select a category and word
        private int RandomNumber()
        {
            int min = 1;
            int max = 4;

            Random rnd = new Random();
            int result = rnd.Next(min, max);
            return result;
        }


        //function allows letter guess on enter instead of pushing button, adapted from our CAA project (originally written by Brian Culp)
        private void txtLetterGuess_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;
                btnGuessLetter_Click(sender, e);
            }
        }
        //new game commands - enable input, clear boxes and reset counts/bools. Reset to initial image, focus on input textbox, play startup "ping"
        private async void StartUp()
        {
            txtLetterGuess.IsEnabled = true;
            btnGuessLetter.IsEnabled = true;
            warning = false;
            txtSelectedWord.Text = "";
            tbError.Text = "";
            tbWrongGuesses.Text = "";
            missCount = 0;
            txtLetterGuess.Focus(FocusState.Programmatic);
            tbError.Foreground = new SolidColorBrush(Colors.DarkRed);
            Canvas.SetZIndex(imgPlatform, 1);
            Canvas.SetZIndex(imgPlatform1, 0);
            Canvas.SetZIndex(imgPlatform2, 0);
            Canvas.SetZIndex(imgPlatform3, 0);
            Canvas.SetZIndex(imgPlatform4, 0);
            Canvas.SetZIndex(imgPlatform5, 0);
            Canvas.SetZIndex(imgPlatform6, 0);

            //audio code adapted from code at https://www.youtube.com/watch?v=hPxExtLCMK0
            Windows.Storage.StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\Audio");
            Windows.Storage.StorageFile file = await folder.GetFileAsync("341871__edsward__ping.wav");
            player.Source = MediaSource.CreateFromStorageFile(file);
            player.Play();

        }
        //Warns user with text and sound on 5th wrong guess, sets warning to true so sound won't repeat on subsequent correct guesses
        private async void Warning()
        {
            tbError.Text = "You're on your last leg! ;)";
            warning = true;

            //audio code adapted from code at https://www.youtube.com/watch?v=hPxExtLCMK0
            Windows.Storage.StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\Audio");
            Windows.Storage.StorageFile file = await folder.GetFileAsync("51752__erkanozan__warning.wav");
            player.Source = MediaSource.CreateFromStorageFile(file);
            player.Play();
        }

        //end game conditions - disable input, play win/loss sound
        private async void GameOver()
        {
            txtLetterGuess.IsEnabled = false;
            btnGuessLetter.IsEnabled = false;
            Windows.Storage.StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\Audio");
            if (missCount == 6)
            {
                //audio code adapted from code at https://www.youtube.com/watch?v=hPxExtLCMK0
                Windows.Storage.StorageFile file = await folder.GetFileAsync("25762__wolfsinger__wap-wap-wap-wap-waaaaaah.wav");
                player.Source = MediaSource.CreateFromStorageFile(file);
                player.Play();
            }
            else
            {
                //audio code adapted from code at https://www.youtube.com/watch?v=hPxExtLCMK0
                Windows.Storage.StorageFile file = await folder.GetFileAsync("391539__mativve__electro-win-sound.wav");
                player.Source = MediaSource.CreateFromStorageFile(file);
                player.Play();
            }
        }

        //reveal new body part based on number of incorrect guesses (missCount)
        private void BodyReveal()
        {
            if (missCount == 1)
            {
                Canvas.SetZIndex(imgPlatform1, 1);
            }
            else if (missCount ==2)
            {
                Canvas.SetZIndex(imgPlatform2, 1);
            }
            else if (missCount == 3)
            {
                Canvas.SetZIndex(imgPlatform3, 1);
            }
            else if (missCount == 4)
            {
                Canvas.SetZIndex(imgPlatform4, 1);
            }
            else if (missCount == 5)
            {
                Canvas.SetZIndex(imgPlatform5, 1);
            }
            else if (missCount == 6)
            {
                Canvas.SetZIndex(imgPlatform6, 1);
            }
        }

        //converts the selected word to an array, capitalizes the first letter of each word and drops the rest to lowercase(in case user enters capital letters, those silly users)
        //and returns it to string for output
        public string ProperCase(string words)
        {
            string[] caseArray = words.Split(' ');
            List<string> properCase = new List<string>();
            string fullWords = " ";
            for (int i = 0; i < caseArray.Length; i++)
            {
                string firstLetter = caseArray[i].Substring(0, 1).ToUpper();
                string otherLetters = caseArray[i].Substring(1).ToLower();
                string fullWord = firstLetter + otherLetters;
                properCase.Add(fullWord);

                fullWords = string.Join(" ", properCase.ToArray());
            }
            return fullWords.ToString();
        }
        #endregion
    }
}
