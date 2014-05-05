// <copyright file="OptionsWindow.xaml.cs" company="corb.co">
//     corb.co. All rights reserved.
// </copyright>
// <author>Corban Mailloux</author>
namespace MediaList
{
    using System;
    using System.Windows;

    /// <summary>
    /// Interaction logic for the OptionsWindow
    /// </summary>
    public partial class OptionsWindow : Window
    {
        /// <summary>
        /// Keep the MainWindow around.
        /// </summary>
        private MainWindow mainWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsWindow"/> class.
        /// Create the "Options" window.
        /// At the moment, it only has an exclude files section.
        /// </summary>
        /// <param name="mainWindow">The MainWindow that created this.</param>
        public OptionsWindow(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.InitializeComponent();

            // Set up the listbox
            ExcludedFilesListBox.ItemsSource = Properties.Settings.Default.ExcludedFiles;
        }

        /// <summary>
        /// Force the list box to update.
        /// </summary>
        private void UpdateList()
        {
            ExcludedFilesListBox.Items.Refresh();
        }

        /// <summary>
        /// Save the list and update MainWindow before closing.
        /// </summary>
        /// <param name="sender">The caller.</param>
        /// <param name="e">The arguments.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            this.mainWindow.SetUpMovieFolders();

            // This doesn't affect TV Shows at the moment.
            ////mainWindow.SetUpTVFolders();
        }

        /// <summary>
        /// Add the text from the text box to the list.
        /// </summary>
        /// <param name="sender">The caller.</param>
        /// <param name="e">The arguments.</param>
        private void AddExcludedFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ExcludedFileTextBox.Text))
            {
                if (Properties.Settings.Default.ExcludedFiles.Contains(ExcludedFileTextBox.Text))
                {
                    System.Windows.MessageBox.Show("This filename has already been added.");
                }
                else
                {
                    Properties.Settings.Default.ExcludedFiles.Add(ExcludedFileTextBox.Text);
                    this.UpdateList();
                }
            }
        }

        /// <summary>
        /// Remove the selected file name from the list of exclusions.
        /// </summary>
        /// <param name="sender">The caller.</param>
        /// <param name="e">The arguments.</param>
        private void RemoveExcludedFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Ensure that there is a selection.
            if (ExcludedFilesListBox.SelectedIndex != -1) 
            {
                Properties.Settings.Default.ExcludedFiles.RemoveAt(ExcludedFilesListBox.SelectedIndex);
                this.UpdateList();
            }
        }
    }
}
