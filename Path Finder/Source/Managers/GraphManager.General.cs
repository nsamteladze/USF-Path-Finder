using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphSharp.Controls;
using Path_Finder.DataTypes;
using System.Windows.Media;
using System.Windows.Threading;
using QuickGraph;
using System.Windows;

namespace Path_Finder.Source.Managers
{
    /// <summary>
    /// This part of the class contains general methods
    /// which are generally used from the Logic Manager.
    /// </summary>
    public partial class GraphManager
    {
        #region Data

        public MainWindow window;  // link to the main window in the UI thread

        #endregion Data

        #region Constructors

        /// <summary>
        /// Null constructor
        /// </summary>
        public GraphManager()
        {
            window = null;
        }

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="_window">
        /// Link to the main window in the UI thread
        /// </param>
        public GraphManager(MainWindow _window)
        {
            window = _window;
        }

        #endregion Constructors

        #region Graph manipulation methods

        public GraphCity CreateGraph(
            CitiesLocations citiesLocations, CitiesConnections citiesConnections)
        {
            var graph = new GraphCity();
            Coordinates tempStartCityCoordinates, tempEndCityCoordinates;

            foreach (City city in citiesLocations.locations.Keys)
            {
                citiesLocations.locations.TryGetValue(city, out tempStartCityCoordinates);
                graph.AddVertex(new VertexCity(new City(city), 
                    new Coordinates(tempStartCityCoordinates)));
            }

            List<City> tempCityConnections = new List<City>();
            foreach (City startCity in citiesConnections.connections.Keys)
            {
                citiesLocations.locations.TryGetValue(startCity, out tempStartCityCoordinates);
                citiesConnections.connections.TryGetValue(startCity, out tempCityConnections);

                foreach (City endCity in tempCityConnections)
                {
                    citiesLocations.locations.TryGetValue(endCity, out tempEndCityCoordinates);
                    graph.AddEdge(new EdgeCity(
                        new VertexCity(new City(startCity), new Coordinates(tempStartCityCoordinates)), 
                        new VertexCity(new City(endCity), new Coordinates(tempEndCityCoordinates))));
                }
            }

            return (graph);
        }

        /// <summary>
        /// Get the data about the current graph's
        /// node, edges and their coordinates. Save this
        /// data to citiesLocations and citiesConnections.
        /// </summary>
        public void GetCurrentGraph(GraphLayoutCity graphLayout,
            ref CitiesLocations citiesLocations)
        {
            VertexControl vertexControl = new VertexControl();
            Coordinates tempVertexCoordinates;
            int tempXCoord;
            int tempYCoord;

            foreach (VertexCity vertex in graphLayout.Graph.Vertices)
            {
                // Get the vertex data and a vertex control
                citiesLocations.locations.TryGetValue(vertex.City, out tempVertexCoordinates);
                vertexControl = graphLayout.GetVertexControl(vertex);

                // Get current coordinates
                tempXCoord = (int)GraphCanvas.GetX(vertexControl);
                tempYCoord = (int)GraphCanvas.GetY(vertexControl);

                // Update the information
                tempVertexCoordinates.setX(tempXCoord);
                tempVertexCoordinates.setY(tempYCoord);
            }
        }

        /// <summary>
        /// Set the nodes coordinates to the coordinates from the 
        /// locations file.
        /// </summary>
        /// <param name="graphLayout">
        /// Graph layout
        /// </param>
        /// <param name="citiesLocations">
        /// Locations file that has the desired coordinates for the graph's vertices
        /// </param>
        public void EstablishCoordinates(ref GraphLayoutCity graphLayout,
            CitiesLocations citiesLocations)
        {
            VertexControl vertexControl = new VertexControl();
            Coordinates tempVertexCoordinates;

            if (citiesLocations != null)
            {
                foreach (VertexCity vertex in graphLayout.Graph.Vertices)
                {
                    // Get the coordinates from the locations file
                    citiesLocations.locations.TryGetValue(vertex.City, out tempVertexCoordinates);

                    // Update the vertex data
                    vertex.CityCoordinates.setX(tempVertexCoordinates.getX());
                    vertex.CityCoordinates.setY(tempVertexCoordinates.getY());

                    // Update the vertex position on the screen
                    vertexControl = graphLayout.GetVertexControl(vertex);
                    GraphCanvas.SetX(vertexControl, vertex.CityCoordinates.getX());
                    GraphCanvas.SetY(vertexControl, vertex.CityCoordinates.getY());
                }
            }
        }

        /// <summary>
        /// Updates the vertex internal information.
        /// Maintains consistency between the vertex's position on the
        /// screen and ins internal information.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="graphLayout"></param>
        public void UpdateVertexInfo(ref VertexCity vertex, ref GraphLayoutCity graphLayout)
        {
            if (vertex != null)
            {
                // Get the vertex control
                VertexControl vertexControl = graphLayout.GetVertexControl(vertex);

                // If we could find the verex control
                if (vertexControl != null)
                {
                    double tempXCoord, tempYCoord;

                    // Get the vertex screen coordinates
                    tempXCoord = GraphCanvas.GetX(vertexControl);
                    tempYCoord = GraphCanvas.GetY(vertexControl);

                    // Update the internal information if the coordinates 
                    // are in range from 0 to 800
                    if ((tempXCoord > 0) && (tempXCoord < 800) &&
                        (tempYCoord > 0) && (tempYCoord < 800))
                    {
                        vertex.CityCoordinates.setX((int)tempXCoord);
                        vertex.CityCoordinates.setY((int)tempYCoord);
                    }

                    // Set the new screen coordinates
                    GraphCanvas.SetX(vertexControl, vertex.CityCoordinates.getX());
                    GraphCanvas.SetY(vertexControl, vertex.CityCoordinates.getY());
                }
            }            
        }

        /// <summary>
        /// Delete the city and all connected edges
        /// </summary>
        /// <param name="cityToDelete">
        /// City we want to delete
        /// </param>
        /// <param name="graphLayout">
        /// Graph layout
        /// </param>
        public void DeleteCity(VertexCity cityToDelete, ref GraphLayoutCity graphLayout)
        {
            graphLayout.Graph.RemoveVertex(cityToDelete);
        }

        #endregion Graph manipulation methods

        #region Graph coloring methods
      
        /// <summary>
        /// Apply the style to the vertex. Must be called from the UI 
        /// thread because it does not use a deispatcher.
        /// </summary>
        /// <param name="vertex">
        /// Tartget vertex for the style
        /// </param>
        /// <param name="graphLayout">
        /// Graph layout
        /// </param>
        /// <param name="style">
        /// The style to apply
        /// </param>
        public void SetStyle(VertexCity vertex, GraphLayoutCity graphLayout,
            Style style)
        {
            // Get vertex control and update style if possible
            VertexControl vertexControl = graphLayout.GetVertexControl(vertex);
            if (vertexControl != null)
            {
                vertexControl.Style = style;
            }
        }

        /// <summary>
        /// Apply the style to all vertices in the graph. Must be called from the UI 
        /// thread because it does not use a deispatcher.
        /// </summary>
        /// <param name="graphLayout">
        /// Graph layout
        /// </param>
        /// <param name="style">
        /// The style to apply
        /// </param>
        public void SetStyleToAll(GraphLayoutCity graphLayout, Style style)
        {
            VertexControl vertexControl;
            
            // Apply style for each vertex in the graph
            foreach (VertexCity vertex in graphLayout.Graph.Vertices)
            {
                vertexControl = graphLayout.GetVertexControl(vertex);
                if (vertexControl != null)
                {
                    vertexControl.Style = style;
                }
            }
        }

        #endregion Graph coloring methods
    }
}