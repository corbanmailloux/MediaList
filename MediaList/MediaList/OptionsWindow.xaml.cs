using System;
using System.Windows;

namespace MediaList
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        /*
         * Keep the MainWindow around.
         */
        private MainWindow mainWindow;

        /*
         * Create the "Options" window. 
         * At the moment, it only has an exclude files section.
         */
        public OptionsWindow(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();

            // Set up the listbox
            ExcludedFilesListBox.ItemsSource = Properties.Settings.Default.ExcludedFiles;
        }

        /*
         * Force the listbox to update.
         */
        private void UpdateList()
        {
            ExcludedFilesListBox.Items.Refresh();
        }

        /*
         * Save the list and update MainWindow before closing.
         */
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            mainWindow.SetUpMovieFolders();
            // This doesn't affect TV Shows at the moment.
            //mainWindow.SetUpTVFolders();
        }

        /*
         * Add the text from the text box to the list.
         */
        private void AddExcludedFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(ExcludedFileTextBox.Text))
            {
                if (Properties.Settings.Default.ExcludedFiles.Contains(ExcludedFileTextBox.Text))
                {
                    System.Windows.MessageBox.Show("This filename has already been added.");
                }
                else
                {
                    Properties.Settings.Default.ExcludedFiles.Add(ExcludedFileTextBox.Text);
                    UpdateList();
                }
            }
        }

        /*
         * Remove the selected file name from the list of exclusions.
         */
        private void RemoveExcludedFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExcludedFilesListBox.SelectedIndex != -1) // Ensure that there is a selection.
            {
                Properties.Settings.Default.ExcludedFiles.RemoveAt(ExcludedFilesListBox.SelectedIndex);
                UpdateList();
            }
        }
    }
}
