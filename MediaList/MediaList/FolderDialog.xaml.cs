using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MediaList
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FolderDialog : Window
    {
        private MainWindow mainWindow;

        public FolderDialog(MainWindow main)
        {
            mainWindow = main;
            InitializeComponent();

            FolderListBox.ItemsSource = Properties.Settings.Default.MovieFolders;
        }

        // Force the listbox to update.
        private void UpdateList()
        {
            FolderListBox.Items.Refresh();
        }

        private void AddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(FolderToAddTextBox.Text))
            {
                Properties.Settings.Default.MovieFolders.Add(FolderToAddTextBox.Text);
                UpdateList();
            }
        }

        private void RemoveFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (FolderListBox.SelectedIndex != -1) // Ensure that there is a selection.
            {
                Properties.Settings.Default.MovieFolders.RemoveAt(FolderListBox.SelectedIndex);
                UpdateList();
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            DialogResult result = dlg.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                FolderToAddTextBox.Text = dlg.SelectedPath;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            mainWindow.SetUpFolders();
        }
    }
}
