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

        // These should probably be moved into another class...
        private List<Movie> movieList = new List<Movie>();
        private List<TVShow> tvList = new List<TVShow>();

        // The list(s) currently displayed.
        private List<Movie> currMovieList = new List<Movie>();
        private List<TVShow> currTVList = new List<TVShow>();


        public MainWindow()
        {
            InitializeComponent();
            SetUpMovieFolders();
            SetUpTVFolders();
            MovieListBox.ItemsSource = currMovieList;
            TVListBox.ItemsSource = currTVList;
        }


        public void SetUpMovieFolders()
        {
            movieList.Clear();
            currMovieList.Clear();

            if (Properties.Settings.Default.MovieFolders == null || Properties.Settings.Default.MovieFolders.Count == 0)
            {
                MessageBox.Show("No movie folders are selected to search.\nYou can add folders through the \"Edit\" menu.");
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
            UpdateMovieList();
        }

        public void SetUpTVFolders()
        {
            tvList.Clear();
            currTVList.Clear();

            if (Properties.Settings.Default.TVFolders == null || Properties.Settings.Default.TVFolders.Count == 0)
            {
                MessageBox.Show("No TV folders are selected to search.\nYou can add folders through the \"Edit\" menu.");
                Properties.Settings.Default.TVFolders = new System.Collections.Specialized.StringCollection();
            }

            foreach (String inStr in Properties.Settings.Default.TVFolders)
            {
                DirectoryInfo di = new DirectoryInfo(inStr);

                if (di.Exists)
                {
                    foreach (DirectoryInfo inDir in di.EnumerateDirectories())
                    {
                        // TODO: Do some checking if this is a TV folder?
                        tvList.Add(new TVShow(inDir));
                    }
                }
            }
            tvList.Sort();
            currTVList.AddRange(tvList);
            UpdateTVList();
        }



        // Force the listbox to update.
        public void UpdateMovieList()
        {
            MovieListBox.Items.Refresh();
            MovieStatusBarText.Text = "Number of Movies: " + currMovieList.Count.ToString();
        }

        public void UpdateTVList()
        {
            TVListBox.Items.Refresh();
            TVStatusBarText.Text = "Number of TV Shows: " + currTVList.Count.ToString();
        }


        /*
         * Add a listener to add real-time searching.
         * 
         * Not the most elegant or efficient solution, but it works.
         * Ideally, some kind of filtering would happen, instead of rebuilding the 
         *  whole list each time the search text changes.
         */
        private void MovieSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            currMovieList.Clear();
            foreach (Movie m in movieList)
            {
                if (m.Name().ToLower().Contains(MovieSearchBox.Text.ToLower()))
                {
                    currMovieList.Add(m);
                }
            }
            // Force an update.
            UpdateMovieList();
        }

        /*
         * Catch when an item is double-clicked to open it in explorer.
         */
        private void MovieListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenSelectedMovieInExplorer();
        }

        /*
         * Catch when the Enter key is pressed to open selection in explorer.
         */
        private void MovieListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OpenSelectedMovieInExplorer();
            }
        }

        /*
         * Open the currently selected movie in Windows Explorer.
         */
        private void OpenSelectedMovieInExplorer()
        {
            if (MovieListBox.SelectedIndex != -1)
            {
                Movie selection = (Movie)MovieListBox.SelectedItem;
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

        private void MovieListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MovieListBox.SelectedIndex == -1)
            {
                MovieNameBox.Text = String.Empty;
                MoviePathBox.Text = String.Empty;
                MovieSizeBox.Text = String.Empty;
            }
            else
            {
                MovieNameBox.Text = ((Movie) MovieListBox.SelectedItem).Name();
                MoviePathBox.Text = ((Movie)MovieListBox.SelectedItem).Path();
                double size = ((Movie)MovieListBox.SelectedItem).Size();
                size = size / (1073741824.0); // Convert bytes to GB
                MovieSizeBox.Text = Math.Round(size, 2).ToString() + " GB";
            }
        }

        private void TVListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenSelectedTVShowInExplorer();
        }

        private void TVListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OpenSelectedTVShowInExplorer();
            }
        }

        private void TVListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TVListBox.SelectedIndex == -1)
            {
                TVNameBox.Text = String.Empty;
                TVPathBox.Text = String.Empty;
                TVNewestEpisodeBox.Text = String.Empty;
            }
            else
            {
                TVNameBox.Text = ((TVShow)TVListBox.SelectedItem).Name();
                TVPathBox.Text = ((TVShow)TVListBox.SelectedItem).Path();
                TVNewestEpisodeBox.Text = ((TVShow)TVListBox.SelectedItem).NewestEpisode(); ;
            }
        }

        private void TVSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            currTVList.Clear();
            foreach (TVShow show in tvList)
            {
                if (show.Name().ToLower().Contains(TVSearchBox.Text.ToLower()))
                {
                    currTVList.Add(show);
                }
            }
            // Force an update.
            UpdateTVList();
        }

        /*
         * Open the currently selected TV show in Windows Explorer.
         */
        private void OpenSelectedTVShowInExplorer()
        {
            if (TVListBox.SelectedIndex != -1)
            {
                TVShow selection = (TVShow)TVListBox.SelectedItem;
                selection.OpenInExplorer();
            }
        }

    }
}
