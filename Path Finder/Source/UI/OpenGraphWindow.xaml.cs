using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path_Finder.DataTypes;

namespace Path_Finder.Source.UI
{
    /// <summary>
    /// Interaction logic for OpenGraphWindow.xaml
    /// </summary>
    public partial class OpenGraphWindow : Window
    {
        #region Data

        private CitiesLocations citiesLocations;        // Locations
        private CitiesConnections citiesConnections;    // Connections

        private bool result;    // Result of some operation (used in dialogs)

        #endregion Data

        #region Constructors

        /// <summary>
        /// Main constructor
        /// </summary>
        public OpenGraphWindow()
        {
            citiesLocations = null;
            citiesConnections = null;

            InitializeComponent();
        }

        #endregion Constructors

        // Used to get data from the window after it was closed
        #region Get data methods

        /// <summary>
        /// Get locations data
        /// </summary>
        /// <returns></returns>
        public CitiesLocations getLocations()
        {
            return (citiesLocations);
        }

        /// <summary>
        /// Get connections data
        /// </summary>
        /// <returns></returns>
        public CitiesConnections getConnections()
        {
            return (citiesConnections);
        }

        /// <summary>
        /// Get the dialog result
        /// </summary>
        /// <returns></returns>
        public bool getResult()
        {
            return (result);
        }

        #endregion Get data methods

        #region Event handlers

        /// <summary>
        /// Cancel the file opening. Close the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            result = false;
            this.Close();
        }

        /// <summary>
        /// Start open file dialog to choose the locations file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBrowseLocations_Click(object sender, RoutedEventArgs e)
        {
             // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.DefaultExt = ".txt"; // Default file extension
            openFileDialog.Filter = "Text documents (.txt)|*.txt|All files (*.*)|*.*"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = openFileDialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                this.textBoxLocations.Text = openFileDialog.FileName;
            }
        }

        /// <summary>
        /// Start open file dialog to choose the connections file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBrowseConnections_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.DefaultExt = ".txt"; // Default file extension
            openFileDialog.Filter = "Text documents (.txt)|*.txt|All files (*.*)|*.*"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = openFileDialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                this.textBoxConnections.Text = openFileDialog.FileName;
            }
        }

        /// <summary>
        /// Load the graph if all the input data is chosen correctly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLoadGraph_Click(object sender, RoutedEventArgs e)
        {
            // Get the files' paths
            string locationsPath = this.textBoxLocations.Text;
            string connectionsPath = this.textBoxConnections.Text;

            // Load data if the files are not corrupted
            if (FileManager.VerifyLocationsFile(locationsPath) &&
                FileManager.VerifyConnectionsFile(connectionsPath))
            {
                citiesLocations = FileManager.ReadLocations(locationsPath);
                citiesConnections = FileManager.ReadConnections(connectionsPath);

                result = true;
                this.Close();
            }
            // If file is corrupted
            else
            {
                result = false;
                MessageBox.Show("ERROR! Chosen files are corrupted. Choose another files.",
                    "Path Finder", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion Event handlers
    }
}
