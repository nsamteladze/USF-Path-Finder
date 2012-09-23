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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Windows.Controls.Ribbon;
using QuickGraph;
using GraphSharp.Controls;
using Path_Finder.DataTypes;
using System.Threading;
using Path_Finder.Source.Managers;

namespace Path_Finder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        #region Data

        private LogicManager logicManager;  // Logic manager

        private bool EventIsHandeled;       // Flag that determines wether graph layout events
                                            // have been already handeled or not.

        #endregion Data

        #region Initialization methods

        /// <summary>
        /// Return the graph to the XAML control
        /// </summary>
        public GraphCity GraphToVisualize
        {
            get
            {
                return (logicManager.graphToVisualize);
            }
        }

        /// <summary>
        /// Main constructor
        /// </summary>
        public MainWindow()
        {
            logicManager = new LogicManager(this);

            InitializeComponent();
        }

        #endregion Initialization methods   

        // Handlers for the click buttons events
        // Generally just call the corresponding logic manager method
        #region Buttons click event handlers

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            zoomControl.Mode = WPFExtensions.Controls.ZoomControlModes.Fill;
            logicManager.LoadGraphFromFile(ref graphLayout);
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            logicManager.Exit();
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            if (logicManager.ReadyToStart())
            {
                logicManager.StartAlgorithm(ref graphLayout, ref richTextBoxLog, this);
            }
            else
            {
                MessageBox.Show("Cannot start A-star algorithm. Not all required data has been provided.",
                    "Path Finder", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            logicManager.StopAlgorithm();

            graphLayout.IsEnabled = true;
            ButtonOpen.IsEnabled = true;
            ButtonReset.IsEnabled = true;
            ButtonDefault.IsEnabled = true;
            RadioButtonDistance.IsEnabled = true;
            RadioButtonHops.IsEnabled = true;
            RadioButtonStep.IsEnabled = true;
            RadioButtonNormal.IsEnabled = true;
            RadioButtonFast.IsEnabled = true;
            ButtonClean.IsEnabled = true;
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            // Reset the coordinates of the all the nodes in the current graph
            logicManager.ResetGraph(ref graphLayout);
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            logicManager.ResetGraphToDefault(ref graphLayout);
        }

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            logicManager.OpenHelp();
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            logicManager.CleanGraph(ref graphLayout);
            this.richTextBoxLog.Text = "";
        }

        #endregion Buttons click event handlers

        // Checked event on the radio buttons
        // Generally change the data in the logic manager
        #region Radion button event handlers

        private void ButtonStep_Checked(object sender, RoutedEventArgs e)
        {
            logicManager.algSpeed = Alg_Speed.Steps;
            MessageBox.Show("Using step-by-step mode, press Start each time when you want to start the next step",
                "Path Finder", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonNormal_Checked(object sender, RoutedEventArgs e)
        {
            logicManager.algSpeed = Alg_Speed.Slow;
        }

        private void ButtonFast_Checked(object sender, RoutedEventArgs e)
        {
            logicManager.algSpeed = Alg_Speed.Fast;
        }

        private void RadioButtonDistance_Checked(object sender, RoutedEventArgs e)
        {
            logicManager.algHeuristic = Heuristic.Distance;
        }

        private void RadioButtonHops_Checked(object sender, RoutedEventArgs e)
        {
            logicManager.algHeuristic = Heuristic.Hops;
        }

        #endregion Radion button event handlers

        #region Mouse events handlers

        /// <summary>
        /// Mouse leave the edge. Set the EventIsHandeled flag to true
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EdgeControl_MouseLeave(object sender, MouseEventArgs e)
        {
            EventIsHandeled = true;
        }

        /// <summary>
        /// If mouse leaves vertex or mouse leaves edge event has not been handled
        /// then we handle mouse leaves graph layout event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void graphLayout_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!EventIsHandeled)
            {
                logicManager.EstablishGraphCoordinates(ref graphLayout);
            }
        }

        /// <summary>
        /// Handle mouse leave vertex event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            StackPanel stackPanel = (StackPanel)sender;
            logicManager.UpdateVertexInfo((VertexCity)stackPanel.DataContext, ref graphLayout);
        }

        /// <summary>
        /// Set the EventIsHandeled flag to false, which
        /// means that we need to handle mouse leave graphlayout event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void graphLayout_MouseEnter(object sender, MouseEventArgs e)
        {
            EventIsHandeled = false;
        }

        #endregion Mouse events handlers

        // Handles events on the vertex context menu items
        #region Context menu items event handlers

        private void StartCityItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            VertexCity vertex = (VertexCity)menuItem.DataContext;

            logicManager.MarkStartCity(vertex, ref graphLayout);
            
        }

        private void FinalCityItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            VertexCity vertex = (VertexCity)menuItem.DataContext;

            logicManager.MarkFinalCity(vertex, ref graphLayout);
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            logicManager.DeleteCity((VertexCity)menuItem.DataContext, ref graphLayout);
        }

        #endregion Context menu items event handlers
    }
}
