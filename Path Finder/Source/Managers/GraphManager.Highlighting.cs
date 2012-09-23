using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Path_Finder.DataTypes;
using System.Windows;
using System.Windows.Threading;
using GraphSharp.Controls;


namespace Path_Finder.Source.Managers
{
    /// <summary>
    /// This part of the Graph Manager class contains the 
    /// highlighting methods, which are generally used from the 
    /// algorithm thread and, thus, use a dispatcher to change UI.
    /// </summary>
    public partial class GraphManager
    {
        #region General highlighting methods

        /// <summary>
        /// Apply style to the city
        /// </summary>
        /// <param name="city">
        /// Target city
        /// </param>
        /// <param name="style">
        /// The style to apply
        /// </param>
        /// <param name="graphLayout">
        /// Graph layout
        /// </param>
        public void MarkCity(City city, Style style, GraphLayoutCity graphLayout)
        {
            // Set style through the UI windows dispatcher
            window.Dispatcher.Invoke(
                new Action(delegate()
                {
                    SetStyle(new VertexCity(city), graphLayout, style);

                }),
                DispatcherPriority.Normal, null);
        }

        /// <summary>
        /// Apply style to the list of cities
        /// </summary>
        /// <param name="cities">
        /// List of the target cities
        /// </param>
        /// <param name="style">
        /// The style to apply
        /// </param>
        /// <param name="graphLayout">
        /// Graph layout
        /// </param>
        public void MarkCities(List<City> cities, Style style, GraphLayoutCity graphLayout)
        {
            // For each city apply the chosen style
            foreach (City city in cities)
            {
                MarkCity(city, style, graphLayout);
            }
        }

        /// <summary>
        /// Highlight an edge
        /// </summary>
        /// <param name="startNode">
        /// Edge start node
        /// </param>
        /// <param name="endNode">
        /// Edge end node
        /// </param>
        /// <param name="graphLayout"></param>
        public void MarkEdge(City startNode, City endNode, GraphLayoutCity graphLayout)
        {
            // Create an EdgeCity object
            EdgeCity edge = new EdgeCity(
                new VertexCity(startNode), new VertexCity(endNode));

            // Highligh the edge
            graphLayout.Dispatcher.Invoke(
                new Action(delegate()
                {
                    graphLayout.HighlightEdge(edge, null);
                }),
                DispatcherPriority.Normal, null);
        }

        /// <summary>
        /// Highligh the edges from the start node to the list 
        /// of nodes.
        /// </summary>
        /// <param name="startNode">
        /// Start node of the edges
        /// </param>
        /// <param name="endNodes">
        /// List of end nodes for the edges
        /// </param>
        /// <param name="graphLayout"></param>
        public void MarkEdges(City startNode, List<City> endNodes, GraphLayoutCity graphLayout)
        {
            foreach (City endNode in endNodes)
            {
                MarkEdge(startNode, endNode, graphLayout);
            }
        }

        /// <summary>
        /// Unmark city. Set its style to default
        /// </summary>
        /// <param name="city"></param>
        /// <param name="graphLayout"></param>
        public void UnmarkCity(City city, GraphLayoutCity graphLayout)
        {
            Style style = null;
            window.Dispatcher.Invoke(
                new Action(delegate()
                {
                    style = (Style)window.Resources["DefaultCityStyle"];
                    SetStyle(new VertexCity(city), graphLayout, style);                
                }),
                DispatcherPriority.Normal, null);
        }

        /// <summary>
        /// Unmark list of cities
        /// </summary>
        /// <param name="cities"></param>
        /// <param name="graphLayout"></param>
        public void UnmarkCities(List<City> cities, GraphLayoutCity graphLayout)
        {
            foreach (City city in cities)
            {
                UnmarkCity(city, graphLayout);
            }
        }

        /// <summary>
        /// Unmark edge
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="endNode"></param>
        /// <param name="graphLayout"></param>
        public void UnmarkEdge(City startNode, City endNode, GraphLayoutCity graphLayout)
        {
            EdgeCity edge = new EdgeCity(
                new VertexCity(startNode), new VertexCity(endNode));

            graphLayout.Dispatcher.Invoke(
                new Action(delegate()
                {
                    graphLayout.RemoveHighlightFromEdge(edge);
                }),
                DispatcherPriority.Normal, null);
        }

        /// <summary>
        /// Unmark edges
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="endNodes"></param>
        /// <param name="graphLayout"></param>
        public void UnmarkEdges(City startNode, List<City> endNodes, GraphLayoutCity graphLayout)
        {
            foreach (City endNode in endNodes)
            {
                UnmarkEdge(startNode, endNode, graphLayout);
            }
        }

        /// <summary>
        /// Unmark all cities
        /// </summary>
        /// <param name="graphLayout"></param>
        public void UnmarkAllCities(GraphLayoutCity graphLayout)
        {
            Style style = null;
            window.Dispatcher.Invoke(
                new Action(delegate()
                {
                    style = (Style)window.Resources["DefaultCityStyle"];
                    SetStyleToAll(graphLayout, style);  
                }),
                DispatcherPriority.Normal, null);
        }

        /// <summary>
        ///  Unmark all edges
        /// </summary>
        /// <param name="graphLayout"></param>
        public void UnmarkAllEdges(GraphLayoutCity graphLayout)
        {
            EdgeControl edgeControl = new EdgeControl();

            foreach (EdgeCity edge in graphLayout.HighlightedEdges)
            {
                graphLayout.Dispatcher.Invoke(
                new Action(delegate()
                {
                    graphLayout.RemoveHighlightFromEdge(edge);
                }),
                DispatcherPriority.Normal, null);
            }
        }

        #endregion General highlighting methods

        #region Style-specific highlighting methods

        /// <summary>
        /// Apply start city style
        /// </summary>
        /// <param name="city"></param>
        /// <param name="graphLayout"></param>
        public void MarkStartCity(City city, GraphLayoutCity graphLayout)
        {
            Style style = null;
            window.Dispatcher.Invoke(
                new Action(delegate()
                {
                    style = (Style)window.Resources["StartCityStyle"];
                    SetStyle(new VertexCity(city), graphLayout, style);
                }),
                    DispatcherPriority.Normal, null);
        }

        /// <summary>
        /// Apply Final City style
        /// </summary>
        /// <param name="city"></param>
        /// <param name="graphLayout"></param>
        public void MarkFinalCity(City city, GraphLayoutCity graphLayout)
        {
            Style style = null;
            window.Dispatcher.Invoke(
                new Action(delegate()
                {
                    style = (Style)window.Resources["FinalCityStyle"];
                    SetStyle(new VertexCity(city), graphLayout, style);
                }),
                    DispatcherPriority.Normal, null);
        }

        /// <summary>
        /// Apply Best City style
        /// </summary>
        /// <param name="city"></param>
        /// <param name="graphLayout"></param>
        public void MarkBestCity(City city, GraphLayoutCity graphLayout)
        {
            Style style = null;
            window.Dispatcher.Invoke(
                new Action(delegate()
                {
                    style = (Style)window.Resources["BestCityStyle"];
                    SetStyle(new VertexCity(city), graphLayout, style);
                }),
                    DispatcherPriority.Normal, null);
        }

        /// <summary>
        /// Apply Checked City style
        /// </summary>
        /// <param name="city"></param>
        /// <param name="graphLayout"></param>
        public void MarkCheckedCity(City city, GraphLayoutCity graphLayout)
        {
            Style style = null;
            window.Dispatcher.Invoke(
                new Action(delegate()
                {
                    style = (Style)window.Resources["CheckedCityStyle"];
                    SetStyle(new VertexCity(city), graphLayout, style);
                }),
                    DispatcherPriority.Normal, null);
        }

        /// <summary>
        /// Apply City to Explore style
        /// </summary>
        /// <param name="city"></param>
        /// <param name="graphLayout"></param>
        public void MarkCityToExplore(City city, GraphLayoutCity graphLayout)
        {
            Style style = null;
            window.Dispatcher.Invoke(
                new Action(delegate()
                {
                    style = (Style)window.Resources["ExploredCityStyle"];
                    SetStyle(new VertexCity(city), graphLayout, style);
                }),
                    DispatcherPriority.Normal, null);
        }

        /// <summary>
        /// Apply Rejected City style
        /// </summary>
        /// <param name="city"></param>
        /// <param name="graphLayout"></param>
        public void MarkRejectedCity(City city, GraphLayoutCity graphLayout)
        {
            Style style = null;
            window.Dispatcher.Invoke(
                new Action(delegate()
                {
                    style = (Style)window.Resources["RejectedCityStyle"];
                    SetStyle(new VertexCity(city), graphLayout, style);
                }),
                    DispatcherPriority.Normal, null);
        }

        /// <summary>
        /// Apply Updated City style
        /// </summary>
        /// <param name="city"></param>
        /// <param name="graphLayout"></param>
        public void MarkUpdatedCity(City city, GraphLayoutCity graphLayout)
        {
            Style style = null;
            window.Dispatcher.Invoke(
                new Action(delegate()
                {
                    style = (Style)window.Resources["UpdatedCityStyle"];
                    SetStyle(new VertexCity(city), graphLayout, style);
                }),
                    DispatcherPriority.Normal, null);
        }

        /// <summary>
        /// Apply Added City style
        /// </summary>
        /// <param name="city"></param>
        /// <param name="graphLayout"></param>
        public void MarkAddedCity(City city, GraphLayoutCity graphLayout)
        {
            Style style = null;
            window.Dispatcher.Invoke(
                new Action(delegate()
                {
                    style = (Style)window.Resources["AddedCityStyle"];
                    SetStyle(new VertexCity(city), graphLayout, style);
                }),
                    DispatcherPriority.Normal, null);
        }

        /// <summary>
        /// Apply City in the Path style
        /// </summary>
        /// <param name="city"></param>
        /// <param name="graphLayout"></param>
        public void MarkPathCity(City city, GraphLayoutCity graphLayout)
        {
            Style style = null;
            window.Dispatcher.Invoke(
                new Action(delegate()
                {
                    style = (Style)window.Resources["PathCityStyle"];
                    SetStyle(new VertexCity(city), graphLayout, style);
                }),
                    DispatcherPriority.Normal, null);
        }

        /// <summary>
        /// Apply Found City style
        /// </summary>
        /// <param name="city"></param>
        /// <param name="graphLayout"></param>
        public void MarkFoundCity(City city, GraphLayoutCity graphLayout)
        {
            Style style = null;
            window.Dispatcher.Invoke(
                new Action(delegate()
                {
                    style = (Style)window.Resources["FoundCityStyle"];
                    SetStyle(new VertexCity(city), graphLayout, style);
                }),
                    DispatcherPriority.Normal, null);
        }

        // Mark many cities

        /// <summary>
        /// Apply Found City style to many cities
        /// </summary>
        /// <param name="cities"></param>
        /// <param name="graphLayout"></param>
        public void MarkAllFoundCities(List<City> cities, GraphLayoutCity graphLayout)
        {
            foreach (City city in cities)
            {
                MarkFoundCity(city, graphLayout);
            }
        }

        /// <summary>
        /// Apply City to Explore style to many cities
        /// </summary>
        /// <param name="cities"></param>
        /// <param name="graphLayout"></param>
        public void MarkAllCitiesToExplore(List<City> cities, GraphLayoutCity graphLayout)
        {
            foreach (City city in cities)
            {
                MarkCityToExplore(city, graphLayout);
            }
        }

        /// <summary>
        /// Apply Checked City style to many cities
        /// </summary>
        /// <param name="cities"></param>
        /// <param name="graphLayout"></param>
        public void MarkAllCheckedCities(List<City> cities, GraphLayoutCity graphLayout)
        {
            foreach (City city in cities)
            {
                MarkCheckedCity(city, graphLayout);
            }
        }

        // Mark path

        /// <summary>
        /// Mark path that is presented by a sequential list of cities
        /// </summary>
        /// <param name="path"></param>
        /// <param name="graphLayout"></param>
        public void MarkPath(List<City> path, GraphLayoutCity graphLayout)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                MarkPathCity(path[i], graphLayout);
                MarkPathCity(path[i + 1], graphLayout);
                MarkEdge(path[i], path[i + 1], graphLayout);
            }
        }

        #endregion Style-specific highlighting methods
    }
}
