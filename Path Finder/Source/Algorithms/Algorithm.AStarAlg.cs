using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Path_Finder.DataTypes;
using Path_Finder.Source.Managers;
using System.Threading;
using System.Windows;

namespace Path_Finder.Source.Algorithms
{
    /// <summary>
    /// This part of the class has the implementation 
    /// of the A-star algorithm
    /// </summary>
    public partial class Algorithm
    {
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
        public List<City> FindOptimalPath()
        {
            City nextCity;                      // Next city to explore
            CityInfo nextCityInfo;              // Next city information
            List<City> nextCityConnections;     // Connections of the nextCity

            CityInfo nextConnectedCityInfo;     // Information about the currently examined city connected to nextCity
            CityInfo nextConnectedCityOldInfo;  // Old information about the currently examined city
           
            Coordinates tempStartPoint, tempEndPoint; // Help variables. Store points on the map.
            City tempCity, tempPrevCity;        // Help variables. Store consecutive sities in the optimal path
            CityInfo tempCityInfo;              // Help variable. Stores a city's info

            LogManager logManager = new LogManager(textBoxLog); // Log manager

            bool finalCityIsFound = false;      // Flag that we found the final city
            double distanceToFinal = -1;        // Length of the found path to the final city

            // Print log - algorithm start
            logManager.Clear();
            logManager.PrintStartAlg(StartCity, FinalCity, AlgHeuristic);

            // Start with the start city:))
            nextCity = StartCity;
            nextCityInfo = new CityInfo(StartCity, null, FinalCity,
                GetCityCoordinates(StartCity), null, GetCityCoordinates(FinalCity), 
                0, AlgHeuristic);

            // Initialize
            foundCities = new Dictionary<City, CityInfo>();
            citiesToExplore = new Dictionary<City, CityInfo>();
            
            // Add the start city
            foundCities.Add(nextCity, nextCityInfo);
            citiesToExplore.Add(nextCity, nextCityInfo);

            // While we have cities to explore
            while (citiesToExplore.Count != 0)
            {
                // Print log
                logManager.PrintFoundExploredCities();

                // Clean the layout
                graphManager.UnmarkAllCities(graphLayout);
                graphManager.UnmarkAllEdges(graphLayout);

                // Highlighting
                graphManager.MarkAllFoundCities(new List<City>(foundCities.Keys), graphLayout);
                graphManager.MarkAllCitiesToExplore(new List<City>(citiesToExplore.Keys), graphLayout);
                graphManager.MarkStartCity(StartCity, graphLayout);
                graphManager.MarkFinalCity(FinalCity, graphLayout);

                // Wait
                Wait();


                // Find the next best city among citiesToExplore
                nextCity = FindBestCity(citiesToExplore);
                // Get its info
                citiesToExplore.TryGetValue(nextCity, out nextCityInfo);

                // Stop if all the estimates are greater than the found path length
                if (finalCityIsFound)
                {
                    if (nextCityInfo.PathDistance >= distanceToFinal)
                    {
                        break;
                    }
                }
                
                // Get the nextCity connections
                citiesConnecitons.connections.TryGetValue(nextCity, out nextCityConnections);

                // Print log - next city to explore
                logManager.PrintBestCity(nextCity, nextCityInfo);

                // Highlighting - next city to explore
                graphManager.UnmarkAllCities(graphLayout);
                graphManager.UnmarkAllEdges(graphLayout);
                graphManager.MarkBestCity(nextCity, graphLayout);

                // Print log - city connections
                logManager.PrintCityConnections(nextCityConnections);

                // Highlighting - city connections
                graphManager.MarkEdges(nextCity, nextCityConnections, graphLayout);
                graphManager.MarkAllCheckedCities(nextCityConnections, graphLayout);
                Wait();

                // Examine all the connections of the nextCity
                foreach (City nextConnectedCity in nextCityConnections)
                {
                    // Get the examined city location and the nextCity location
                    citiesLocations.locations.TryGetValue(nextCity, out tempStartPoint);
                    citiesLocations.locations.TryGetValue(nextConnectedCity, out tempEndPoint);

                    // Create information about the currently examined city
                    if (AlgHeuristic == Heuristic.Distance)
                    {
                        nextConnectedCityInfo = new CityInfo(nextConnectedCity, nextCity, FinalCity,
                            tempEndPoint, tempStartPoint, GetCityCoordinates(FinalCity),
                            FindDistance(tempStartPoint, tempEndPoint) + nextCityInfo.FromStart,
                            AlgHeuristic);
                    }
                    // If we use number of hops as a heuristic
                    else
                    {
                        nextConnectedCityInfo = new CityInfo(nextConnectedCity, nextCity, FinalCity,
                            tempEndPoint, tempStartPoint, GetCityCoordinates(FinalCity),
                            1 + nextCityInfo.FromStart, AlgHeuristic);
                    }


                    // If the examined city has already been found.
                    // If the current path is better, then update the city's info
                    if (foundCities.ContainsKey(nextConnectedCity))
                    {
                        // Get the city's old info from foundCities
                        foundCities.TryGetValue(nextConnectedCity, out nextConnectedCityOldInfo);

                        // Compare its old path distance to the new path distance
                        if (nextConnectedCityInfo.PathDistance <
                            nextConnectedCityOldInfo.PathDistance)
                        {
                            // Print log - updated city
                            logManager.PrintUpdatedCity(nextConnectedCity, nextConnectedCityInfo,
                                nextConnectedCityOldInfo);

                            // Highlighting - updated city
                            graphManager.MarkUpdatedCity(nextConnectedCity, graphLayout);

                            // Update the info if the new path is shorter
                            nextConnectedCityOldInfo.PrevCity = nextConnectedCityInfo.PrevCity;
                            nextConnectedCityOldInfo.FromStart = nextConnectedCityInfo.FromStart;                           
                            
                            // If we updated the final city (found a better path)
                            if (nextConnectedCity.Equals(FinalCity))
                            {
                                distanceToFinal = nextConnectedCityInfo.FromStart;
                            }
                        }
                        else
                        {
                            // Print log - rejected city
                            logManager.PrintRejectedCity(nextConnectedCity, nextConnectedCityInfo,
                                nextConnectedCityOldInfo);

                            // Highlighting - rejected city
                            graphManager.MarkRejectedCity(nextConnectedCity, graphLayout);
                        }
                    }
                    // If we have not found this city so far.
                    // Add it to foundCities and citiesToExplore.
                    else
                    {
                        // Add the city to foundCities and citiesToExplore.
                        foundCities.Add(nextConnectedCity, nextConnectedCityInfo);
                        citiesToExplore.Add(nextConnectedCity, nextConnectedCityInfo);

                        // Print log - added city
                        logManager.PrintAddedCity(nextConnectedCity, nextConnectedCityInfo);

                        // Highlighting - added city
                        graphManager.MarkAddedCity(nextConnectedCity, graphLayout);

                        // Check wether we added the desired final city
                        if (!finalCityIsFound)
                        {
                            if (nextConnectedCity.Equals(FinalCity))
                            {
                                finalCityIsFound = true;
                                distanceToFinal = nextConnectedCityInfo.FromStart;
                            }
                        }
                    }
                }

                // Wait
                Wait();

                // Mark nextCity as explored.
                // Remove it from citiesToExplore
                nextCityInfo.IsExplored = true;
                citiesToExplore.Remove(nextCity);               
            }

            // If we were able to reach the destination,
            // then construct the optimal path.
            if (foundCities.ContainsKey(FinalCity))
            {
                // Start with the end city
                tempCity = FinalCity;
                // Get the end city info
                foundCities.TryGetValue(tempCity, out tempCityInfo);

                // Initialize
                List<City> optimalPath = new List<City>();

                // Add the end city to the optimal path
                optimalPath.Add(tempCity);

                // Set the city that preceds the end city in the optimal path
                tempPrevCity = tempCityInfo.PrevCity;

                // While we have not reached the start city
                while (tempPrevCity != null)
                {
                    // Add the city that preceds the current city in the optimal path
                    tempCity = tempPrevCity;
                    optimalPath.Add(tempCity);

                    // Move to the previous city in the path in order to 
                    // add it in the next loop iteration.
                    foundCities.TryGetValue(tempCity, out tempCityInfo);
                    tempPrevCity = tempCityInfo.PrevCity;
                }

                // Reverse the list
                ReverseListOrder(ref optimalPath);

                // Print log - optimal path
                logManager.PrintPath(optimalPath,
                    Algorithm.CalculateTotalPath(optimalPath, citiesLocations));

                // Highlighting - optimal path
                graphManager.UnmarkAllCities(graphLayout);
                graphManager.UnmarkAllEdges(graphLayout);
                graphManager.MarkPath(optimalPath, graphLayout);

                // Print log - end of the algorithm
                logManager.PrintEndAlg(true);

                return (optimalPath);
            }
            // Output an error if the destination could not be reached.
            else
            {
                // Highlighting 
                graphManager.UnmarkAllCities(graphLayout);
                graphManager.UnmarkAllEdges(graphLayout);
                graphManager.MarkStartCity(StartCity, graphLayout);
                graphManager.MarkFinalCity(FinalCity, graphLayout);

                // Print log - end of the algorithm
                logManager.PrintEndAlg(false);

                // Output an error that the path was not found
                MessageBox.Show("Cannot find a path between the chosen cities.",
                    "Path Finder", MessageBoxButton.OK, MessageBoxImage.Warning);
                return (null);
            }
        }
    }
}
