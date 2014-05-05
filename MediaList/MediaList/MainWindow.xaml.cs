// <copyright file="MainWindow.xaml.cs" company="corb.co">
//     corb.co. All rights reserved.
// </copyright>
// <author>Corban Mailloux</author>
namespace MediaList
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for the Main Window
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The total list of Movies.
        /// </summary>
        private List<Movie> movieList = new List<Movie>();

        /// <summary>
        /// The total list of TVShows.
        /// </summary>
        private List<TVShow> tvList = new List<TVShow>();

        /// <summary>
        /// The Movies currently in the list.
        /// </summary>
        private List<Movie> currMovieList = new List<Movie>();

        /// <summary>
        /// The TVShows currently in the list.
        /// </summary>
        private List<TVShow> currTVList = new List<TVShow>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// Build the main window, causing the initial indexing.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.SetUpExcludedFiles();
            this.SetUpMovieFolders();
            this.SetUpTVFolders();
            MovieListBox.ItemsSource = this.currMovieList;
            TVListBox.ItemsSource = this.currTVList;
        }

        /// <summary>
        /// Go through each of the Movie folders, indexing each Movie.
        /// </summary>
        public void SetUpMovieFolders()
        {
            this.movieList.Clear();
            this.currMovieList.Clear();

            if (Properties.Settings.Default.MovieFolders == null || Properties.Settings.Default.MovieFolders.Count == 0)
            {
                MessageBox.Show("No movie folders are selected to search.\nYou can add folders through the \"Edit\" menu.");
                Properties.Settings.Default.MovieFolders = new System.Collections.Specialized.StringCollection();
            }

            foreach (string inStr in Properties.Settings.Default.MovieFolders)
            {
                DirectoryInfo di = new DirectoryInfo(inStr);
                
                if (di.Exists)
                {
                    foreach (FileInfo inFile in di.EnumerateFiles())
                    {
                        // Ensure excluded files are... excluded.
                        if (!Properties.Settings.Default.ExcludedFiles.Contains(inFile.Name))
                        {
                            this.movieList.Add(new Movie(inFile));
                        }
                    }
                }
            }

            this.movieList.Sort();
            this.currMovieList.AddRange(this.movieList);
            this.UpdateMovieList();
        }

        /// <summary>
        /// Go through each of the TV folders, indexing each TV folder.
        /// </summary>
        public void SetUpTVFolders()
        {
            this.tvList.Clear();
            this.currTVList.Clear();

            if (Properties.Settings.Default.TVFolders == null || Properties.Settings.Default.TVFolders.Count == 0)
            {
                MessageBox.Show("No TV folders are selected to search.\nYou can add folders through the \"Edit\" menu.");
                Properties.Settings.Default.TVFolders = new System.Collections.Specialized.StringCollection();
            }

            foreach (string inStr in Properties.Settings.Default.TVFolders)
            {
                DirectoryInfo di = new DirectoryInfo(inStr);

                if (di.Exists)
                {
                    foreach (DirectoryInfo inDir in di.EnumerateDirectories())
                    {
                        // TODO: Do some checking if this is a TV folder?
                        this.tvList.Add(new TVShow(inDir));
                    }
                }
            }

            this.tvList.Sort();
            this.currTVList.AddRange(this.tvList);
            this.UpdateTVList();
        }
        
        /// <summary>
        /// Close the window when the "Exit" menu item is selected.
        /// </summary>
        /// <param name="sender">The caller.</param>
        /// <param name="e">The arguments.</param>
        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Create and show the "Add or Remove Folders" window.
        /// </summary>
        /// <param name="sender">The caller.</param>
        /// <param name="e">The arguments.</param>
        private void ChangeFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new FolderDialog(this).Show();
        }

        /// <summary>
        /// Create and show the "Options" window.
        /// </summary>
        /// <param name="sender">The caller.</param>
        /// <param name="e">The arguments.</param>
        private void OptionsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new OptionsWindow(this).Show();
        }

        /// <summary>
        /// Ensure that the excluded file list is created.
        /// </summary>
        private void SetUpExcludedFiles()
        {
            if (Properties.Settings.Default.ExcludedFiles == null)
            {
                Properties.Settings.Default.ExcludedFiles = new System.Collections.Specialized.StringCollection();
            }
        }

        /// <summary>
        /// Force the Movie list box to update, also updating the status bar.
        /// </summary>
        private void UpdateMovieList()
        {
            MovieListBox.Items.Refresh();
            MovieStatusBarText.Text = "Number of Movies: " + this.currMovieList.Count.ToString();
        }

        /// <summary>
        /// Open the currently selected movie in Windows Explorer.
        /// </summary>
        private void OpenSelectedMovieInExplorer()
        {
            if (MovieListBox.SelectedIndex != -1)
            {
                Movie selection = (Movie)MovieListBox.SelectedItem;
                selection.OpenInExplorer();
            }
        }

        /// <summary>
        /// Add a listener to add real-time searching.
        /// </summary>
        /// <param name="sender">The caller.</param>
        /// <param name="e">The arguments.</param>
        private void MovieSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            /*
             * Not the most elegant or efficient solution, but it works.
             * Ideally, some kind of filtering would happen, instead of rebuilding the 
             *  whole list each time the search text changes.
             */

            this.currMovieList.Clear();
            foreach (Movie m in this.movieList)
            {
                if (m.Name().ToLower().Contains(MovieSearchBox.Text.ToLower()))
                {
                    this.currMovieList.Add(m);
                }
            }

            // Force an update.
            this.UpdateMovieList();
        }

        /// <summary>
        /// Catch when an item is double-clicked to open it in explorer.
        /// </summary>
        /// <param name="sender">The caller.</param>
        /// <param name="e">The arguments.</param>
        private void MovieListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.OpenSelectedMovieInExplorer();
        }

        /// <summary>
        /// Catch when the Enter key is pressed to open selection in explorer.
        /// </summary>
        /// <param name="sender">The caller.</param>
        /// <param name="e">The arguments.</param>
        private void MovieListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.OpenSelectedMovieInExplorer();
            }
        }

        /// <summary>
        /// Update the Name, Path, and Size boxes when a new Movie is selected.
        /// </summary>
        /// <param name="sender">The caller.</param>
        /// <param name="e">The arguments.</param>
        private void MovieListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MovieListBox.SelectedIndex == -1)
            {
                MovieNameBox.Text = string.Empty;
                MoviePathBox.Text = string.Empty;
                MovieSizeBox.Text = string.Empty;
            }
            else
            {
                MovieNameBox.Text = ((Movie)MovieListBox.SelectedItem).Name();
                MoviePathBox.Text = ((Movie)MovieListBox.SelectedItem).Path();
                double size = ((Movie)MovieListBox.SelectedItem).Size();
                size = size / 1073741824.0; // Convert bytes to GB
                MovieSizeBox.Text = Math.Round(size, 2).ToString() + " GB";
            }
        }

        /// <summary>
        /// Force the TV list box to update, also updating the status bar.
        /// </summary>
        private void UpdateTVList()
        {
            TVListBox.Items.Refresh();
            TVStatusBarText.Text = "Number of TV Shows: " + this.currTVList.Count.ToString();
        }

        /// <summary>
        /// Open the currently selected TV show in Windows Explorer.
        /// </summary>
        private void OpenSelectedTVShowInExplorer()
        {
            if (TVListBox.SelectedIndex != -1)
            {
                TVShow selection = (TVShow)TVListBox.SelectedItem;
                selection.OpenInExplorer();
            }
        }

        /// <summary>
        /// Add a listener to add real-time searching.
        /// </summary>
        /// <param name="sender">The caller.</param>
        /// <param name="e">The arguments.</param>
        private void TVSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.currTVList.Clear();
            foreach (TVShow show in this.tvList)
            {
                if (show.Name().ToLower().Contains(TVSearchBox.Text.ToLower()))
                {
                    this.currTVList.Add(show);
                }
            }

            // Force an update.
            this.UpdateTVList();
        }

        /// <summary>
        /// Catch when an item is double-clicked to open it in explorer.
        /// </summary>
        /// <param name="sender">The caller.</param>
        /// <param name="e">The arguments.</param>
        private void TVListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.OpenSelectedTVShowInExplorer();
        }

        /// <summary>
        /// Catch when the Enter key is pressed to open selection in explorer.
        /// </summary>
        /// <param name="sender">The caller.</param>
        /// <param name="e">The arguments.</param>
        private void TVListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.OpenSelectedTVShowInExplorer();
            }
        }
        
        /// <summary>
        /// Update the Name, Path, and Newest Episode boxes when a new TVShow is selected.
        /// </summary>
        /// <param name="sender">The caller.</param>
        /// <param name="e">The arguments.</param>
        private void TVListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TVListBox.SelectedIndex == -1)
            {
                TVNameBox.Text = string.Empty;
                TVPathBox.Text = string.Empty;
                TVNewestEpisodeBox.Text = string.Empty;
            }
            else
            {
                TVNameBox.Text = ((TVShow)TVListBox.SelectedItem).Name();
                TVPathBox.Text = ((TVShow)TVListBox.SelectedItem).Path();
                TVNewestEpisodeBox.Text = ((TVShow)TVListBox.SelectedItem).NewestEpisode();
            }
        }

        /// <summary>
        /// Generate a MessageBox with the missing episodes.
        /// </summary>
        /// <param name="sender">The caller.</param>
        /// <param name="e">The arguments.</param>
        private void TVCheckMissingEpisodesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (TVListBox.SelectedIndex != -1)
            {
                MessageBox.Show(((TVShow)TVListBox.SelectedItem).Name() + ": " + ((TVShow)TVListBox.SelectedItem).MissingEpisodes(), "Missing Episodes");
            }
        }

        /// <summary>
        /// Generate a MessageBox with the extensions used.
        /// </summary>
        /// <param name="sender">The caller.</param>
        /// <param name="e">The arguments.</param>
        private void TVSeeFileExtensionsUsedMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (TVListBox.SelectedIndex != -1)
            {
                MessageBox.Show(((TVShow)TVListBox.SelectedItem).Name() + ": " + ((TVShow)TVListBox.SelectedItem).ExtensionsUsed(), "Extensions Used");
            }
        }
    }
}
