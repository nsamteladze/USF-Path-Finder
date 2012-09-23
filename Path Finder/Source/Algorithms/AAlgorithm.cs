using System;
using System.Collections.Generic;
using Path_Finder.DataTypes;
using GraphSharp.Controls;
using System.Windows.Media;
using System.Threading;
using Path_Finder.Source.Managers;


namespace Path_Finder
{
    /* DESCRIPTION:
     * Implemets the A Algorithm and the corresponding stuff
     */
    public class AAlgorithm
    {
        private const int SLEEP_TIME = 100;

        // Data
        private CitiesLocations citiesLocations;             // Cities' locations
        private CitiesConnections citiesConnecitons;         // Cities' connections
        private Dictionary<City, CityInfo> foundCities;      // List of cities that have been found so far
        private Dictionary<City, CityInfo> citiesToExplore;  // List of cities that must be explored during the 
                                                             // next step of the algorithm
        private int algSpeed;
        private GraphLayout graphLayout;

        // Constructors
        public AAlgorithm(CitiesLocations newCitiesLocations, CitiesConnections newCitiesConnectios, 
            GraphLayout _graphLayout, int _algSpeed)
        {
            citiesLocations = newCitiesLocations;
            citiesConnecitons = newCitiesConnectios;
            algSpeed = _algSpeed;
            graphLayout = _graphLayout;
        }

        public void RunAAlgThread(object threadData)
        {
            List<City> optimalPath;
            CityPair pathEnds = (CityPair) threadData;

            optimalPath = this.FindOptimalPath(pathEnds.FromCity, pathEnds.ToCity);
        }

        #region Work Methods

        /* DESCRIPTION:
         * Finds ditect distance between two cities
         */
        private static double FindDistance(Coordinates point1, Coordinates point2)
        {
            double distance;

            // Calculate the distance
            distance = Math.Sqrt(Math.Pow(point1.getX() - point2.getX(), 2) +
                                 Math.Pow(point1.getY() - point2.getY(), 2));

            return (distance);
        }

        /* DESCRIPTION:
         * Find the best city to explore next in citiesToExplore.
         * The best city is a city with the least approximate
         * pathDistance.
         */
        private City FindBest()
        {
            City bestCity = null;     // Best city to explore next
            CityInfo bestCityInfo;    // Information about the best city
            CityInfo nextCityInfo;    // Informations about the currently examined city
            double minimalDist = -1;  // Minimal total path distance found so far
            double nextCityDist;      // Total path distance for the currently examined city
            int i = 0;                // Iterative variable in the foreach loop

            // Check all the cities in citiesToExplore
            foreach (City nextCity in citiesToExplore.Keys)
            {
                // If we just started, then set that best city is the first examined city
                if (i == 0)
                { 
                    bestCity = nextCity;                                      // Set the best city
                    citiesToExplore.TryGetValue(bestCity, out bestCityInfo);  // Set the info
                    minimalDist = bestCityInfo.getPathDistance();             // Set the min distance
                }
                // If it is not the first iteration of the loop
                else
                {
                    // Get the information about the city
                    citiesToExplore.TryGetValue(nextCity, out nextCityInfo);
                    nextCityDist = nextCityInfo.getPathDistance();

                    // Compare its total path distance to the minimal
                    // If it is less, then this city becomes the best city to explore nextg
                    if (nextCityDist < minimalDist)
                    {
                        bestCity = nextCity;
                        bestCityInfo = nextCityInfo;
                        minimalDist = nextCityDist;
                    }
                }

                // Next loop iteration
                i++;
            }

            return (bestCity);
        }

        /* DESCRIPTION:
         * This method implements the A Algorithm.
         * Uses direct distance to the final city as a heuristic.
         * At each step finds the best next city to explore from citiesToExplore.
         * Finds the cities connected to it and examines them.
         * If the connected city is already found, then it examines is the new path to
         * it shorter than the already known one. If it is, than it renew the information
         * about the city in foundCities and citiesToExplore.
         * If the city has not been found yet, then it add it to citiesToExplore and
         * foundCities. 
         * When all cities have been explored it finds the minimal path using the idea
         * that if the city B preceds the destination city A in the minimum path,
         * then the minimum path also includes the minimum path from the start city 
         * to B. Thus, it goes from city to city in the reverse order and adds them
         * to the minimum path.
         */
        public List<City> FindOptimalPath(City startCity, City endCity)
        {
            City nextCity;                      // Next city to explore
            CityInfo nextCityInfo;              // Next city information
            CityInfo nextConnectedCityInfo;     // Information about the currently examined city connected to nextCity
            CityInfo nextConnectedCityOldInfo;  // Old information about the currently examined city
            List<City> nextCityConnections;     // Connections of the nextCity
            Coordinates tempStartPoint, tempEndPoint; // Help variables. Store points on the map.
            City tempCity, tempPrevCity;        // Help variables. Store consecutive sities in the optimal path
            CityInfo tempCityInfo;              // Help variable. Stores a city's info
            HighlightManager highlightManager;

            // Start with the start city:))
            nextCity = startCity;
            nextCityInfo = new CityInfo(this, startCity, null, endCity, 0);

            // Initialize
            foundCities = new Dictionary<City, CityInfo>();
            citiesToExplore = new Dictionary<City, CityInfo>();
            highlightManager = new HighlightManager(graphLayout);

            // Add the start city
            foundCities.Add(nextCity, nextCityInfo);
            citiesToExplore.Add(nextCity, nextCityInfo);

            // While we have cities to explore
            while (citiesToExplore.Count != 0)
            {
                // Find the next best city among citiesToExplore
                nextCity = FindBest();

                highlightManager.MarkNode(nextCity, Graph_Colors.BestNode);
                Thread.Sleep(SLEEP_TIME);

                // Get its info and connections
                citiesToExplore.TryGetValue(nextCity, out nextCityInfo);
                citiesConnecitons.connections.TryGetValue(nextCity, out nextCityConnections);
                
                // Examine all the connections of the nextCity
                foreach (City nextConnectedCity in nextCityConnections)
                {
                    highlightManager.MarkNode(nextConnectedCity, Graph_Colors.CheckedNode);
                    highlightManager.MarkEdge(nextCity, nextConnectedCity, Graph_Colors.CheckedEdge);
                    Thread.Sleep(SLEEP_TIME);

                    // Get the examined city location and the nextCity location
                    citiesLocations.locations.TryGetValue(nextCity, out tempStartPoint);
                    citiesLocations.locations.TryGetValue(nextConnectedCity, out tempEndPoint);
                    // Create information about the currently examined city
                    nextConnectedCityInfo = new CityInfo(this, nextConnectedCity, nextCity, endCity,
                                                         FindDistance(tempStartPoint, tempEndPoint) +
                                                         nextCityInfo.getFromStart());

                    // If the examined city has already been found.
                    // If the current path is better, then update the city's info
                    if (foundCities.ContainsKey(nextConnectedCity))
                    {
                        // Get the city's old info from foundCities
                        foundCities.TryGetValue(nextConnectedCity, out nextConnectedCityOldInfo);

                        // Compare its old path distance to the new path distance
                        if (nextConnectedCityInfo.getPathDistance() <
                            nextConnectedCityOldInfo.getPathDistance())
                        {
                            // Update the info if the new path is shorter
                            nextConnectedCityOldInfo.prevCity = nextConnectedCityInfo.prevCity;
                            nextConnectedCityOldInfo.setFromStart(nextConnectedCityInfo.getFromStart());

                            highlightManager.MarkNode(nextConnectedCity, Graph_Colors.UpdatedNode);
                            Thread.Sleep(SLEEP_TIME);
                        }
                        else
                        {
                            highlightManager.MarkNode(nextConnectedCity, Graph_Colors.RejectedNode);
                            Thread.Sleep(SLEEP_TIME);
                        }
                    }
                    // If we have not found this city so far.
                    // Add it to foundCities and citiesToExplore.
                    else
                    {
                        // Add the city to foundCities and citiesToExplore.
                        foundCities.Add(nextConnectedCity, nextConnectedCityInfo);
                        citiesToExplore.Add(nextConnectedCity, nextConnectedCityInfo);

                        highlightManager.MarkNode(nextConnectedCity, Graph_Colors.AddedNode);
                        Thread.Sleep(SLEEP_TIME);
                    }
                }
                
                // Mark nextCity as explored.
                // Remove it from citiesToExplore
                nextCityInfo.IsExplored = true;
                citiesToExplore.Remove(nextCity);

                highlightManager.UnmarkAllNodes();
                highlightManager.UnmarkAllEdges();

                foreach (City city in foundCities.Keys)
                {
                    foundCities.TryGetValue(city, out tempCityInfo);
                    if (tempCityInfo.IsExplored)
                    {
                        highlightManager.MarkNode(city, Graph_Colors.ExploredNode);
                    }
                }
                Thread.Sleep(SLEEP_TIME);
            }

            // If we were able to reach the destination,
            // then construct the optimal path.
            if (foundCities.ContainsKey(endCity))
            {
                // Start with the end city
                tempCity = endCity;
                // Get the end city info
                foundCities.TryGetValue(tempCity, out tempCityInfo);
                
                // Initialize
                List<City> optimalPath = new List<City>();

                // Add the end city to the optimal path
                optimalPath.Add(tempCity); 

                // Set the city that preceds the end city in the optimal path
                tempPrevCity = tempCityInfo.prevCity;
                
                // While we have not reached the start city
                while (tempPrevCity != null)
                {
                    // Add the city that preceds the current city in the optimal path
                    tempCity = tempPrevCity;
                    optimalPath.Add(tempCity);

                    // Move to the previous city in the path in order to 
                    // add it in the next loop iteration.
                    foundCities.TryGetValue(tempCity, out tempCityInfo);
                    tempPrevCity = tempCityInfo.prevCity;
                }

                return (optimalPath);
            }
            // Output an error if the destination could not be reached.
            else
            {
                Console.WriteLine("ERROR! Cannot find any path.");
                return (null);
            }
        }

        /* DESCRIPTION:
         * Calculates the path between the sequence of cities.
         */
        public void CalculateTotalPath()
        {
            int numCities;                        // Number of cities in the path
            double totalPath = 0.0;               // Total path distance
            City curCity, nextCity;               // Help variables. Help to move through the cities.
            Coordinates tempStartPoint, tempEndPoint;   // Help variables. Store the points on the map.

            // Read the number of cities on the path
            Console.Write("Number of cities in the path: ");
            numCities = Int32.Parse(Console.ReadLine());

            // If the nubmer of cities is more than 1
            if (numCities > 1)
            {
                // Read the first city in the path
                Console.Write("1 of {0}: ", numCities);
                curCity = new City(Console.ReadLine());
                
                // Read the sequence of cities that makes a path.
                // Update the total path distance at each iteration.
                for (int i = 0; i < numCities - 1; i++)
                {
                    // Read the next city in the path
                    Console.Write("{0} if {1}: ", i + 2, numCities);
                    nextCity = new City(Console.ReadLine());

                    // Get the data about cities' locations
                    citiesLocations.locations.TryGetValue(curCity, out tempStartPoint);
                    citiesLocations.locations.TryGetValue(nextCity, out tempEndPoint);

                    // Add the distance between cities to the total path distance
                    totalPath += FindDistance(tempStartPoint, tempEndPoint);

                    // Move to the next city
                    curCity = nextCity;
                }

                // Print the total path
                Console.WriteLine("Total path: {0}", totalPath);
                Console.ReadKey();
            }
            // If we have < 2 cities in the path, then output an error.
            else
            {
                Console.WriteLine("ERROR! Less than 2 cities in the path.");
            }
        }

        #endregion Work Methods
    }
}
