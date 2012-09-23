using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;
using Path_Finder.DataTypes;
using GraphSharp.Controls;
using Path_Finder.Source.UI;
using System.Windows;
using System.Windows.Controls;
using Path_Finder.Source.Algorithms;
using System.Threading;
using System.IO;

namespace Path_Finder.Source.Managers
{
    /// <summary>
    /// Logic manager contains all the programs logic. It is generally used
    /// from the UI event handlers. It uses graph manager for the graph
    /// related operations.
    /// </summary>
    class LogicManager
    {
        #region Data

        // Current graph
        public GraphCity graphToVisualize;
        public CitiesLocations citiesLocations;
        public CitiesConnections citiesConnections;

        // Original graph downloaded from a file
        public CitiesLocations defaultCitiesLocations;
        public CitiesConnections defaultCitiesConnections;

        // Data which was used to start the A-star algorithm
        private CitiesLocations algCitiesLocations;

        // End cities
        public VertexCity startCity;
        public VertexCity finalCity;

        // Algorithm variables
        public Alg_Speed algSpeed;
        public Heuristic algHeuristic;

        // Graph manager
        public GraphManager graphManager;

        // Windows resources
        ResourceDictionary resources;

        // Data about the algorithm
        Algorithm algorithm;
        Thread algThread;

        // Determines is algorithm thread running now
        private bool isRunningAlg;
        public bool IsRunningAlg
        {
            get
            {
                if (algorithm != null)
                {
                    return (algorithm.IsRunning);
                }
                else
                {
                    return (isRunningAlg);
                }
            }
            set
            {
                isRunningAlg = value;
            }
        }

        // Link to the main UI window
        MainWindow window;

        #endregion Data

        #region Constructors

        public LogicManager(MainWindow _window)
        {
            graphToVisualize = null;

            citiesLocations = null;
            citiesConnections = null;

            defaultCitiesLocations = new CitiesLocations();
            defaultCitiesConnections = new CitiesConnections();

            algCitiesLocations = new CitiesLocations();

            startCity = null;
            finalCity = null;

            algSpeed = Alg_Speed.Fast;
            algHeuristic = Heuristic.Distance;
         
            resources = _window.Resources;
            window = _window;

            graphManager = new GraphManager(window);

            algorithm = null;
            algThread = null;
            IsRunningAlg = false;
        }

        #endregion Constructors

        #region Private methods

        /// <summary>
        /// Recover the default graph data, which was downloaded from the initial
        /// file.
        /// </summary>
        private void RecoverGraphData()
        {
            citiesLocations.Copy(defaultCitiesLocations);
            citiesConnections.Copy(defaultCitiesConnections);
        }

        /// <summary>
        /// Determines wether some vertices have been deleted from
        /// the initial graph.
        /// </summary>
        /// <returns></returns>
        private bool NoDelete()
        {
            foreach (City city in defaultCitiesLocations.locations.Keys)
            {
                if (!citiesLocations.locations.ContainsKey(city))
                {
                    return (false);
                }
            }

            return (true);
        }

        /// <summary>
        /// Reset cities location to default locations
        /// </summary>
        private void ResetCitiesLocations()
        {
            Coordinates tempCityCoordinates, defaultCityCoordinates;

            if ((citiesLocations != null) && (defaultCitiesLocations != null))
            {
                foreach (City city in citiesLocations.locations.Keys)
                {
                    citiesLocations.locations.TryGetValue(city, out tempCityCoordinates);
                    defaultCitiesLocations.locations.TryGetValue(city, out defaultCityCoordinates);
                    tempCityCoordinates.setX(defaultCityCoordinates.getX());
                    tempCityCoordinates.setY(defaultCityCoordinates.getY());
                }
            }
        }

        /// <summary>
        /// Verify that the data passed into the algorithm has not been changed.
        /// Is not used. UI is disabled instead.
        /// </summary>
        /// <param name="graphLayout"></param>
        /// <returns></returns>
        private bool VerifyAlgorithmDataConsistency(GraphLayoutCity graphLayout)
        {
            CaptureCurrentGraph(graphLayout);
            if (!citiesLocations.Equals(algCitiesLocations))
            {
                return (false);
            }
            else
            {
                return (true);
            }
        }

        /// <summary>
        /// Delete the city from the locations and the connections files.
        /// </summary>
        /// <param name="cityToDelete"></param>
        private void DeleteCityFromData(City cityToDelete)
        {
            List<City> tempCityConnections;

            if (citiesLocations.locations.ContainsKey(cityToDelete))
            {
                citiesLocations.locations.Remove(cityToDelete);
            }
            if (citiesConnections.connections.ContainsKey(cityToDelete))
            {
                citiesConnections.connections.Remove(cityToDelete);
            }

            foreach (City connectedCity in citiesConnections.connections.Keys)
            {
                citiesConnections.connections.TryGetValue(connectedCity, out tempCityConnections);

                if (tempCityConnections.Contains(cityToDelete))
                {
                    tempCityConnections.Remove(cityToDelete);
                }
            }
        }
     
        #endregion Private methods

        #region Public methods

        /// <summary>
        /// Reset all the style to the default style
        /// </summary>
        /// <param name="graphLayout"></param>
        public void CleanGraph(ref GraphLayoutCity graphLayout)
        {
            Style style = (Style)resources["DefaultCityStyle"];
            graphManager.SetStyleToAll(graphLayout, style);
        }

        /// <summary>
        /// Start A-star algorithm in a separate thread
        /// </summary>
        /// <param name="graphLayout"></param>
        /// <param name="richTextBoxLog"></param>
        /// <param name="window"></param>
        public void StartAlgorithm(ref GraphLayoutCity graphLayout, ref TextBox richTextBoxLog,
            MainWindow window)
        {
            // If already running
            if (IsRunningAlg)
            {
                algorithm.WaitMutex.WaitOne();
                algorithm.CanContinue = true;
                algorithm.WaitMutex.ReleaseMutex();
            }
            // If not, then start a new thread
            else
            {

                algorithm = new Algorithm(citiesLocations, citiesConnections,
                    startCity.City, finalCity.City, algSpeed, algHeuristic,
                    ref graphLayout, ref richTextBoxLog, resources, graphManager);
                // Set window and create a mutex
                algorithm.Window = window;
                algorithm.WaitMutex = new Mutex();

                // Null the start and end cities
                startCity = null;
                finalCity = null;

                // Create and start the thread
                algThread = new Thread(new ThreadStart(algorithm.RunAlgThread));
                algThread.SetApartmentState(ApartmentState.STA);
                algThread.Start();

                // Allow the algorithm to continue
                algorithm.WaitMutex.WaitOne();
                algorithm.CanContinue = true;
                algorithm.WaitMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Stop the algorithm execution and kill the algorithm thread
        /// </summary>
        public void StopAlgorithm()
        {
            if (algThread != null)
            {
                algThread.Abort();
            }

            if (algorithm != null)
            {
                algorithm.IsRunning = false;
                algorithm = null;
            }
        }

        /// <summary>
        /// Exit the program. Kill all the threads.
        /// </summary>
        public void Exit()
        {
            if (algThread != null)
            {
                algThread.Abort();
            }

            Application.Current.Shutdown();
        }

        /// <summary>
        /// Update the vertex information in the locations file and also update the 
        /// vertex internal information
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="graphLayout"></param>
        public void UpdateVertexInfo(VertexCity vertex, ref GraphLayoutCity graphLayout)
        {
            graphManager.UpdateVertexInfo(ref vertex, ref graphLayout);

            if (vertex != null)
            {
                Coordinates tempCityCoordinates;
                if (citiesLocations.locations.TryGetValue(vertex.City, out tempCityCoordinates))
                {
                    tempCityCoordinates.setX(vertex.CityCoordinates.getX());
                    tempCityCoordinates.setY(vertex.CityCoordinates.getY());
                }
            }

        }

        /// <summary>
        /// Delete city from the locations and the connections files. Also
        /// delete it from the screen.
        /// </summary>
        /// <param name="cityToDelete"></param>
        /// <param name="graphLayout"></param>
        public void DeleteCity(VertexCity cityToDelete, ref GraphLayoutCity graphLayout)
        {
            graphManager.DeleteCity(cityToDelete, ref graphLayout);
            DeleteCityFromData(cityToDelete.City);
        }

        /// <summary>
        /// Mark start city on the screen
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="graphLayout"></param>
        public void MarkStartCity(VertexCity vertex, ref GraphLayoutCity graphLayout)
        {
            if (startCity != null)
            {
                Style oldStyle = (Style)resources["DefaultCityStyle"];
                graphManager.SetStyle(startCity, graphLayout, oldStyle);
            }

            if ((finalCity != null) && (finalCity.Equals(vertex)))
            {
                finalCity = null;
            }

            startCity = vertex;

            Style newStyle = (Style)resources["StartCityStyle"];
            graphManager.SetStyle(vertex, graphLayout, newStyle);
        }

        /// <summary>
        /// Mark the city as Final City on the screen
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="graphLayout"></param>
        public void MarkFinalCity(VertexCity vertex, ref GraphLayoutCity graphLayout)
        {
            if (finalCity != null)
            {
                Style oldStyle = (Style)resources["DefaultCityStyle"];
                graphManager.SetStyle(finalCity, graphLayout, oldStyle);
            }

            if ((startCity != null) && (startCity.Equals(vertex)))
            {
                startCity = null;
            }

            finalCity = vertex;

            Style newStyle = (Style)resources["FinalCityStyle"];
            graphManager.SetStyle(vertex, graphLayout, newStyle);
        }

        /// <summary>
        /// Open the help file
        /// </summary>
        public void OpenHelp()
        {
            if (File.Exists("Path Finder.chm"))
            {
                System.Diagnostics.Process.Start("Path Finder.chm");
            }
            else
            {
                MessageBox.Show("Help file not found! Check that file Path Finder.chm is in the program's directory.",
                    "Path Finder", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Reset the graph to the default graph, which was originally downloaded from the
        /// file.
        /// </summary>
        /// <param name="graphLayout"></param>
        public void ResetGraphToDefault(ref GraphLayoutCity graphLayout)
        {
            if (NoDelete())
            {
                ResetGraph(ref graphLayout);
            }
            else
            {
                RecoverGraphData();
                CreateGraph();
                RefreshGraph(ref graphLayout);
                EstablishGraphCoordinates(ref graphLayout);
            }
        }

        /// <summary>
        /// Reset the vertecies locations to the default locations.
        /// </summary>
        /// <param name="graphLayout"></param>
        public void ResetGraph(ref GraphLayoutCity graphLayout)
        {
            ResetCitiesLocations();
            EstablishGraphCoordinates(ref graphLayout);
        }

        /// <summary>
        /// Create a new graph based on locations and connections data
        /// </summary>
        public void CreateGraph()
        {
            graphToVisualize = graphManager.CreateGraph(citiesLocations, citiesConnections);
        }

        /// <summary>
        /// Returns true if we have all the data to find the 
        /// optimal path between two nodes. Returns false elsewhen.
        /// </summary>
        /// <returns></returns>
        public bool ReadyToStart()
        {
            if (algorithm != null)
            {
                if (algorithm.IsRunning)
                {
                    return (true);
                }
            }
            if ((citiesLocations != null) && (citiesConnections != null) &&
                (startCity != null) && (finalCity != null))
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }

        /// <summary>
        /// Get current nodes screen coordinates. Is not used. Coordinates are updated 
        /// automatically.
        /// </summary>
        /// <param name="graphLayout"></param>
        public void CaptureCurrentGraph(GraphLayoutCity graphLayout)
        {
            graphManager.GetCurrentGraph(graphLayout, ref citiesLocations);
        }

        /// <summary>
        /// Set the vertices coordinates according to the locations file
        /// </summary>
        /// <param name="graphLayout"></param>
        public void EstablishGraphCoordinates(ref GraphLayoutCity graphLayout)
        {
            graphManager.EstablishCoordinates(ref graphLayout, citiesLocations);
        }

        /// <summary>
        /// Refresh the graph in the layout
        /// </summary>
        /// <param name="graphLayout"></param>
        public void RefreshGraph(ref GraphLayoutCity graphLayout)
        {
            graphLayout.Graph = graphToVisualize;
        }

        /// <summary>
        /// Load graph from the file
        /// </summary>
        /// <param name="graphLayout"></param>
        public void LoadGraphFromFile(ref GraphLayoutCity graphLayout)
        {
            var openGraphWindow = new OpenGraphWindow();
            openGraphWindow.ShowDialog();

            if (openGraphWindow.getResult())
            {
                citiesLocations = openGraphWindow.getLocations();
                citiesConnections = openGraphWindow.getConnections();

                defaultCitiesLocations.Copy(citiesLocations);
                defaultCitiesConnections.Copy(citiesConnections);

                CreateGraph();
                RefreshGraph(ref graphLayout);
                EstablishGraphCoordinates(ref graphLayout);
            }
        }

        #endregion Public methods
    }
}
