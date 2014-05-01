using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace MediaList
{
    /// <summary>
    /// Interaction logic for FolderDialog.xaml
    /// </summary>
    public partial class FolderDialog : Window
    {
        /*
         * Keep the MainWindow around.
         */
        private MainWindow mainWindow;

        /*
         * Create the "Add or Remove Folders" window.
         */
        public FolderDialog(MainWindow main)
        {
            mainWindow = main;
            InitializeComponent();

            // Set up the listboxes
            MovieFolderListBox.ItemsSource = Properties.Settings.Default.MovieFolders;
            TVFolderListBox.ItemsSource = Properties.Settings.Default.TVFolders;
        }

        /*
         * Force the listboxes to update.
         */
        private void UpdateList()
        {
            MovieFolderListBox.Items.Refresh();
            TVFolderListBox.Items.Refresh();
        }

        /*
         * Prompt the user to select a folder.
         */
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            DialogResult result = dlg.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                FolderToAddTextBox.Text = dlg.SelectedPath;
            }
        }

        /*
         * Add the folder to the movie folder list.
         */
        private void MovieAddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateFolder(FolderToAddTextBox.Text))
            {
                if (Properties.Settings.Default.MovieFolders.Contains(FolderToAddTextBox.Text))
                {
                    System.Windows.MessageBox.Show("This folder has already been added.");
                }
                else
                {
                    Properties.Settings.Default.MovieFolders.Add(FolderToAddTextBox.Text);
                    UpdateList();
                }
            }
        }

        /*
         * Remove the selected movie folder from the list.
         */
        private void MovieRemoveFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (MovieFolderListBox.SelectedIndex != -1) // Ensure that there is a selection.
            {
                Properties.Settings.Default.MovieFolders.RemoveAt(MovieFolderListBox.SelectedIndex);
                UpdateList();
            }
        }

        /*
         * Add the folder to the TV folder list.
         */
        private void TVAddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateFolder(FolderToAddTextBox.Text))
            {
                if (Properties.Settings.Default.TVFolders.Contains(FolderToAddTextBox.Text))
                {
                    System.Windows.MessageBox.Show("This folder has already been added.");
                }
                else
                {
                    Properties.Settings.Default.TVFolders.Add(FolderToAddTextBox.Text);
                    UpdateList();
                }                
            }
        }

        /*
         * Remove the selected TV folder from the list.
         */
        private void TVRemoveFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (TVFolderListBox.SelectedIndex != -1) // Ensure that there is a selection.
            {
                Properties.Settings.Default.TVFolders.RemoveAt(TVFolderListBox.SelectedIndex);
                UpdateList();
            }
        }

        /*
         * Validate that the folder is populated and contains a valid folder.
         */
        private bool ValidateFolder(string path)
        {
            if (!String.IsNullOrEmpty(path) && !String.IsNullOrWhiteSpace(path))
            {
                try
                {
                    if (!(new DirectoryInfo(path).Exists))
                    {
                        System.Windows.MessageBox.Show("Invalid folder path.");
                        return false;
                    }
                }
                catch
                {
                    System.Windows.MessageBox.Show("Invalid folder path.");
                    return false;
                }
                return true;
            }
            return false;
        }

        /*
         * Save the new lists and update MainWindow before closing.
         */
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            mainWindow.SetUpMovieFolders();
            mainWindow.SetUpTVFolders();
        }
    }
}
