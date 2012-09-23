using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;
using Path_Finder.DataTypes;

namespace Path_Finder.Source.Managers
{
    /// <summary>
    /// This manager is used to print information and 
    /// messages to the log text box. It is generaly called
    /// from the algorithm thread. Thus, it uses a dispatcher 
    /// to access the text box in the UI thread.
    /// </summary>
    public class LogManager
    {
        #region Data

        /// <summary>
        /// Text box to print to
        /// </summary>
        public TextBox textBox
        {
            get;
            private set;
        }

        #endregion Data

        #region Constructors

        /// <summary>
        /// Null constructor
        /// </summary>
        public LogManager()
        {
            textBox = null;
        }

        /// <summary>
        /// Basic class constructor
        /// </summary>
        /// <param name="_textBox">
        /// Text box to print information to
        /// </param>
        public LogManager(TextBox _textBox)
        {
            textBox = _textBox;
        }

        #endregion Constructors

        #region General methods

        /// <summary>
        /// Clear the text box
        /// </summary>
        public void Clear()
        {
            textBox.Dispatcher.Invoke(
                new Action(delegate()
                    {
                        textBox.Text = "";
                    }),
                    DispatcherPriority.Normal, null);
        }

        /// <summary>
        /// Print some message to the text box
        /// </summary>
        /// <param name="message"></param>
        public void PrintMessage(string message)
        {
            textBox.Dispatcher.Invoke(
                new Action(delegate()
                    {
                        textBox.Text += message;
                        textBox.Text += "\n";
                    }),
                    DispatcherPriority.Normal, null);
        }

        /// <summary>
        /// Print break to the text box. Break is an empty line.
        /// </summary>
        public void PrintBreak()
        {
            textBox.Dispatcher.Invoke(
                new Action(delegate()
                    {
                        textBox.Text += "\n";
                    }),
            DispatcherPriority.Normal, null);
        }

        /// <summary>
        /// Print a line to the text box.
        /// </summary>
        public void PrintLine()
        {
            textBox.Dispatcher.Invoke(
                new Action(delegate()
                {
                    textBox.Text += "---------------------------------------\n";
                }),
            DispatcherPriority.Normal, null);
        }

        /// <summary>
        /// Print a line break which is a break-line-break sequence.
        /// </summary>
        public void PrintLineBreak()
        {
            PrintBreak();
            PrintLine();
            PrintBreak();
        }

        #endregion General methods

        #region Print city information methods

        /// <summary>
        /// Print information about the best city to explore next
        /// </summary>
        /// <param name="city"></param>
        /// <param name="cityInfo"></param>
        public void PrintBestCity(City city, CityInfo cityInfo)
        {
            PrintLineBreak();
            textBox.Dispatcher.Invoke(
                new Action(delegate()
                    {
                        textBox.Text += string.Format("Next city to explore: {0}\n", 
                            city.getName());
                        textBox.Text += string.Format("Estimated path distance: {0:0.##}\n", 
                            cityInfo.PathDistance);
                    }),
                DispatcherPriority.Normal, null);

        }

        /// <summary>
        /// Print information about the city connections
        /// </summary>
        /// <param name="connections"></param>
        public void PrintCityConnections(List<City> connections)
        {
            textBox.Dispatcher.Invoke(
                new Action(delegate()
                    {
                        textBox.Text += "City connections: ";
                        for (int i = 0; i < connections.Count; i++)
                        {
                            if (i == 0)
                            {
                                textBox.Text += connections[i].getName();
                            }
                            else
                            {
                                textBox.Text += string.Format(", {0}", 
                                    connections[i].getName());
                            }
                        }
                        textBox.Text += "\n";
                    }),
                DispatcherPriority.Normal, null);

        }

        /// <summary>
        /// Print information about the new city that was found
        /// </summary>
        /// <param name="city"></param>
        /// <param name="cityInfo"></param>
        public void PrintAddedCity(City city, CityInfo cityInfo)
        {
            PrintBreak();
            textBox.Dispatcher.Invoke(
                new Action(delegate()
                    {
                        textBox.Text += string.Format("Add new city: {0}\n",
                            city.getName());
                        textBox.Text += string.Format("Distance from start: {0:0.##}\n",
                            cityInfo.FromStart);
                        textBox.Text += string.Format("Estimated path length: {0:0.##}\n",
                            cityInfo.PathDistance);
                    }),
                DispatcherPriority.Normal, null);
        }

        /// <summary>
        /// Print information about the city that was updated
        /// </summary>
        /// <param name="city"></param>
        /// <param name="newCityInfo"></param>
        /// <param name="oldCityInfo"></param>
        public void PrintUpdatedCity(City city, CityInfo newCityInfo,
            CityInfo oldCityInfo)
        {
            PrintBreak();
            textBox.Dispatcher.Invoke(
                new Action(delegate()
                {
                    textBox.Text += string.Format("Update city (shorter path): {0}\n",
                        city.getName());
                    textBox.Text += string.Format("Old distance from start: {0:0.##}\n",
                        oldCityInfo.FromStart);
                    textBox.Text += string.Format("New distance from start: {0:0.##}\n",
                        newCityInfo.FromStart);
                    textBox.Text += string.Format("Estimated path length: {0:0.##}\n",
                        newCityInfo.PathDistance);
                }),
                DispatcherPriority.Normal, null);
        }

        /// <summary>
        /// Print information about the city that was rejected
        /// </summary>
        /// <param name="city"></param>
        /// <param name="newCityInfo"></param>
        /// <param name="oldCityInfo"></param>
        public void PrintRejectedCity(City city, CityInfo newCityInfo,
            CityInfo oldCityInfo)
        {
            PrintBreak();
            textBox.Dispatcher.Invoke(
                new Action(delegate()
                {
                    textBox.Text += string.Format("Reject city (already visited): {0}\n",
                        city.getName());
                    textBox.Text += string.Format("Old distance from start: {0:0.##}\n",
                        oldCityInfo.FromStart);
                    textBox.Text += string.Format("New distance from start: {0:0.##}\n",
                        newCityInfo.FromStart);
                }),
                DispatcherPriority.Normal, null);
        }

        /// <summary>
        /// Print information about the path through a list of cities
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pathLength"></param>
        public void PrintPath(List<City> path, double pathLength)
        {
            PrintLineBreak();
            textBox.Dispatcher.Invoke(
                new Action(delegate()
                {
                    textBox.Text += "Path: ";
                    for (int i = 0; i < path.Count; i++)
                    {
                        if (i == 0)
                        {
                            textBox.Text += path[i].getName();
                        }
                        else
                        {
                            textBox.Text += string.Format(" -> {0}",
                                path[i].getName());
                        }
                    }
                    textBox.Text += "\n";
                    textBox.Text += string.Format("Path length: {0:0.##}\n",
                        pathLength);
                }),
                    DispatcherPriority.Normal, null);
        }

        /// <summary>
        /// Print start message
        /// </summary>
        /// <param name="startCity"></param>
        /// <param name="finalCity"></param>
        /// <param name="heuristic"></param>
        public void PrintStartAlg(City startCity, City finalCity,
            Heuristic heuristic)
        {
            textBox.Dispatcher.Invoke(
                new Action(delegate()
                    {
                        textBox.Text += "--== Starting A-star algorithm ==--\n";
                        PrintBreak();
                        if (heuristic == Heuristic.Distance)
                        {
                            textBox.Text += string.Format("Used heuristic: Minimum Distance\n");
                        }
                        else if (heuristic == Heuristic.Hops)
                        {
                            textBox.Text += string.Format("Used heuristic: Minimum Hops\n");
                        }                       
                        textBox.Text += string.Format("Start city: {0}\n",
                            startCity.getName());
                        textBox.Text += string.Format("Final city: {0}\n",
                            finalCity.getName());
                    }),
                DispatcherPriority.Normal, null);
        }

        /// <summary>
        /// Print end message
        /// </summary>
        /// <param name="foundThePath"></param>
        public void PrintEndAlg(bool foundThePath)
        {
            PrintLineBreak();
            textBox.Dispatcher.Invoke(
                new Action(delegate()
                {
                    if (foundThePath)
                    {
                        textBox.Text += "Optimal path has been found.\n";
                    }
                    else
                    {
                        textBox.Text += "Could not find a path.\n";
                    }
                    PrintBreak();
                    textBox.Text += "--== End of the A-star algorithm ==--\n";
                }),
            DispatcherPriority.Normal, null);
        }

        /// <summary>
        /// Print a message that explains how the explored and found cities 
        /// are marked.
        /// </summary>
        public void PrintFoundExploredCities()
        {
            PrintBreak();
            PrintMessage("Found cities are highlighted in yellow.");
            PrintMessage("Cities that have not been explored yet");
            PrintMessage("have green border.");
        }

        #endregion Print city information methods
    }
}
