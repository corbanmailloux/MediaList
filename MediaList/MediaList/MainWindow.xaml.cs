using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MediaList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // I don't know about this...
        private List<Movie> movieList = new List<Movie>();

        // The list currently displayed.
        private List<Movie> currMovieList = new List<Movie>();


        public MainWindow()
        {
            InitializeComponent();
            SetUpFolders();
            MainListBox.ItemsSource = currMovieList;
        }


        public void SetUpFolders()
        {
            movieList.Clear();
            currMovieList.Clear();

            if (Properties.Settings.Default.MovieFolders == null || Properties.Settings.Default.MovieFolders.Count == 0)
            {
                MessageBox.Show("No folders are selected to search.\nYou can add folders through the \"Edit\" menu.");
                Properties.Settings.Default.MovieFolders = new System.Collections.Specialized.StringCollection();
            }

            foreach (String inStr in Properties.Settings.Default.MovieFolders)
            {
                DirectoryInfo di = new DirectoryInfo(inStr);
                
                if (di.Exists)
                {
                    foreach (FileInfo inFile in di.EnumerateFiles())
                    {
                        // TODO: Do some checking if this is a movie?
                        movieList.Add(new Movie(inFile));
                    }
                }
            }
            movieList.Sort();
            currMovieList.AddRange(movieList);
            UpdateList();
        }



        // Force the listbox to update.
        public void UpdateList()
        {
            MainListBox.Items.Refresh();
            StatusBarText.Text = "Number of Movies: " + currMovieList.Count.ToString();
        }


        /*
         * Add a listener to add real-time searching.
         * 
         * Not the most elegant or efficient solution, but it works.
         * Ideally, some kind of filtering would happen, instead of rebuilding the 
         *  whole list each time the search text changes.
         */
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            currMovieList.Clear();
            foreach (Movie m in movieList)
            {
                if (m.Name().ToLower().Contains(SearchBox.Text.ToLower()))
                {
                    currMovieList.Add(m);
                }
            }
            // Force an update.
            UpdateList();
        }

        /*
         * Catch when an item is double-clicked to open it in explorer.
         */
        private void MainListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenSelectionInExplorer();
        }

        /*
         * Catch when the Enter key is pressed to open selection in explorer.
         */
        private void MainListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OpenSelectionInExplorer();
            }
        }

        /*
         * Open the currently selected movie in Windows Explorer.
         */
        private void OpenSelectionInExplorer()
        {
            if (MainListBox.SelectedIndex != -1)
            {
                Movie selection = (Movie)MainListBox.SelectedItem;
                selection.OpenInExplorer();
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ChangeFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new FolderDialog(this).Show();
        }

        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainListBox.SelectedIndex == -1)
            {
                NameBox.Text = String.Empty;
                PathBox.Text = String.Empty;
                SizeBox.Text = String.Empty;
            }
            else
            {
                NameBox.Text = ((Movie) MainListBox.SelectedItem).Name();
                PathBox.Text = ((Movie)MainListBox.SelectedItem).Path();
                double size = ((Movie)MainListBox.SelectedItem).Size();
                size = size / (1073741824.0); // Convert bytes to GB
                SizeBox.Text = Math.Round(size, 2).ToString() + " GB";
            }
        }



    }
}
