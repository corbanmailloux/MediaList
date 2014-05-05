// <copyright file="FolderDialog.xaml.cs" company="corb.co">
//     corb.co. All rights reserved.
// </copyright>
// <author>Corban Mailloux</author>
namespace MediaList
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Forms;
    
    /// <summary>
    /// Interaction logic for the FolderDialog.
    /// </summary>
    public partial class FolderDialog : Window
    {
        /// <summary>
        /// Keep the MainWindow around.
        /// </summary>
        private MainWindow mainWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderDialog"/> class.
        /// Create the "Add or Remove Folders" window.
        /// </summary>
        /// <param name="main">The MainWindow that created this.</param>
        public FolderDialog(MainWindow main)
        {
            this.mainWindow = main;
            this.InitializeComponent();

            // Set up the listboxes
            MovieFolderListBox.ItemsSource = Properties.Settings.Default.MovieFolders;
            TVFolderListBox.ItemsSource = Properties.Settings.Default.TVFolders;
        }

        /// <summary>
        /// Force the list boxes to update.
        /// </summary>
        private void UpdateList()
        {
            MovieFolderListBox.Items.Refresh();
            TVFolderListBox.Items.Refresh();
        }

        /// <summary>
        /// Prompt the user to select a folder.
        /// </summary>
        /// <param name="sender">The caller.</param>
        /// <param name="e">The arguments.</param>
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            DialogResult result = dlg.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                FolderToAddTextBox.Text = dlg.SelectedPath;
            }
        }

        /// <summary>
        /// Add the folder to the movie folder list.
        /// </summary>
        /// <param name="sender">The caller.</param>
        /// <param name="e">The arguments.</param>
        private void MovieAddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ValidateFolder(FolderToAddTextBox.Text))
            {
                if (Properties.Settings.Default.MovieFolders.Contains(FolderToAddTextBox.Text))
                {
                    System.Windows.MessageBox.Show("This folder has already been added.");
                }
                else
                {
                    Properties.Settings.Default.MovieFolders.Add(FolderToAddTextBox.Text);
                    this.UpdateList();
                }
            }
        }

        /// <summary>
        /// Remove the selected movie folder from the movie folder list.
        /// </summary>
        /// <param name="sender">The caller.</param>
        /// <param name="e">The arguments.</param>
        private void MovieRemoveFolderButton_Click(object sender, RoutedEventArgs e)
        {
            // Ensure that there is a selection.
            if (MovieFolderListBox.SelectedIndex != -1)
            {
                Properties.Settings.Default.MovieFolders.RemoveAt(MovieFolderListBox.SelectedIndex);
                this.UpdateList();
            }
        }

        /// <summary>
        /// Add the folder to the TV folder list.
        /// </summary>
        /// <param name="sender">The caller.</param>
        /// <param name="e">The arguments.</param>
        private void TVAddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ValidateFolder(FolderToAddTextBox.Text))
            {
                if (Properties.Settings.Default.TVFolders.Contains(FolderToAddTextBox.Text))
                {
                    System.Windows.MessageBox.Show("This folder has already been added.");
                }
                else
                {
                    Properties.Settings.Default.TVFolders.Add(FolderToAddTextBox.Text);
                    this.UpdateList();
                }                
            }
        }

        /// <summary>
        /// Remove the selected TV folder from the TV folder list.
        /// </summary>
        /// <param name="sender">The caller.</param>
        /// <param name="e">The arguments.</param>
        private void TVRemoveFolderButton_Click(object sender, RoutedEventArgs e)
        {
            // Ensure that there is a selection.
            if (TVFolderListBox.SelectedIndex != -1) 
            {
                Properties.Settings.Default.TVFolders.RemoveAt(TVFolderListBox.SelectedIndex);
                this.UpdateList();
            }
        }

        /// <summary>
        /// Validates the the path is not empty and is a valid folder.
        /// </summary>
        /// <param name="path">The path to validate.</param>
        /// <returns>
        /// true if valid.
        /// false if not valid.
        /// </returns>
        private bool ValidateFolder(string path)
        {
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    if (!new DirectoryInfo(path).Exists)
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

        /// <summary>
        /// Save the lists and update MainWindow before closing.
        /// </summary>
        /// <param name="sender">The caller.</param>
        /// <param name="e">The arguments.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            this.mainWindow.SetUpMovieFolders();
            this.mainWindow.SetUpTVFolders();
        }
    }
}
