using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Path_Finder.DataTypes;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using Path_Finder.Source.Managers;


namespace Path_Finder.Source.Algorithms
{
    /// <summary>
    /// This part of the class contains general methods used 
    /// in the algorithm
    /// </summary>
    public partial class Algorithm
    {  
        #region Data

        private CitiesLocations citiesLocations;             // Cities' locations
        private CitiesConnections citiesConnecitons;         // Cities' connections
        private Dictionary<City, CityInfo> foundCities;      // List of cities that have been found so far
        private Dictionary<City, CityInfo> citiesToExplore;  // List of cities that must be explored during the 
                                                             // next step of the algorithm
        // Start and Final cities
        public City StartCity
        {
            get;
            private set;
        }
        public City FinalCity
        {
            get;
            private set;
        }

        // Used speed and heuristic
        public Alg_Speed AlgSpeed
        {
            get;
            set;
        }
        public Heuristic AlgHeuristic
        {
            get;
            private set;
        }

        private GraphLayoutCity graphLayout;            // Layout

        private TextBox textBoxLog;                     // Textbox for log

        private ResourceDictionary resourceDictionary;  // Resources (templates and styles)

        // Flag that shows is algorithm now running or not
        private bool isRunning;
        public bool IsRunning
        {
            get
            {
                return (isRunning);
            }
            set
            {
                isRunning = value;
            }
        }

        public MainWindow Window
        {
            get;
            set;
        }          // Link to the main window in the UI thread

        public GraphManager graphManager;    // Manager for the graph

        #endregion Data

        #region Constructors

        // Null constructor
        public Algorithm()
        {
            citiesLocations = null;
            citiesConnecitons = null;

            StartCity = null;
            FinalCity = null;

            AlgSpeed = Alg_Speed.Fast;
            AlgHeuristic = Heuristic.Distance;

            graphLayout = null;
            textBoxLog = null;
            resourceDictionary = null;

            IsRunning = false;

            Window = null;

            CanContinue = false;
        }

        // Main constructor that initializes everything except window
        public Algorithm(CitiesLocations _citiesLocations, CitiesConnections _citiesConnectios, 
            City _startCity, City _finalCity, Alg_Speed _algSpeed, Heuristic _algHeuristic,
            ref GraphLayoutCity _graphLayout, ref TextBox _textBoxLog,
            ResourceDictionary _resourceDictionary, GraphManager _graphManager)
        {
            citiesLocations = _citiesLocations;
            citiesConnecitons = _citiesConnectios;

            StartCity = _startCity;
            FinalCity = _finalCity;

            AlgSpeed = _algSpeed;
            AlgHeuristic = _algHeuristic;

            graphLayout = _graphLayout;
            textBoxLog = _textBoxLog;
            resourceDictionary = _resourceDictionary;

            IsRunning = false;

            Window = null;

            CanContinue = false;

            graphManager = _graphManager;
        }

        #endregion Constructors

        #region Static methods

        /// <summary>
        /// Find Euclidian distance between two points
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static double FindDistance(Coordinates point1, Coordinates point2)
        {
            double distance;

            // Calculate the distance
            distance = Math.Sqrt(Math.Pow(point1.getX() - point2.getX(), 2) +
                                 Math.Pow(point1.getY() - point2.getY(), 2));

            return (distance);            
        }
        
        /// <summary>
        /// Calculates the path between the sequence of cities.
        /// </summary>
        public static double CalculateTotalPath(List<City> cities, CitiesLocations citiesLocations)
        {
            if (cities.Count < 2)
            {
                return (-1);
            }

            double pathLength = 0;
            Coordinates tempCityCoordinates_1, tempCityCoordinates_2;

            citiesLocations.locations.TryGetValue(cities[0], out tempCityCoordinates_1);
            for (int i = 1; i < cities.Count; i++)
            {
                citiesLocations.locations.TryGetValue(cities[i], out tempCityCoordinates_2);
                pathLength += Algorithm.FindDistance(tempCityCoordinates_1, tempCityCoordinates_2);

                tempCityCoordinates_1 = tempCityCoordinates_2;
            }

            return (pathLength);
        }

        #endregion Static methods

        #region Private methods

        /// <summary>
        /// Find the best city to explore next in cities.
        /// The best city is a city with the least approximate
        /// pathDistance.
        /// </summary>
        /// <returns></returns>
        private City FindBestCity(Dictionary<City, CityInfo> cities)
        {
            City bestCity = null;     // Best city to explore next
            CityInfo bestCityInfo;    // Information about the best city
            CityInfo nextCityInfo;    // Informations about the currently examined city
            double minimalDist = -1;  // Minimal total path distance found so far
            double nextCityDist;      // Total path distance for the currently examined city
            int i = 0;                // Iterative variable in the foreach loop

            // Check all the cities in citiesToExplore
            foreach (City nextCity in cities.Keys)
            {
                // If we just started, then set that best city is the first examined city
                if (i == 0)
                {
                    bestCity = nextCity;                             // Set the best city
                    cities.TryGetValue(bestCity, out bestCityInfo);  // Set the info
                    minimalDist = bestCityInfo.PathDistance;         // Set the min distance
                }
                // If it is not the first iteration of the loop
                else
                {
                    // Get the information about the city
                    cities.TryGetValue(nextCity, out nextCityInfo);
                    nextCityDist = nextCityInfo.PathDistance;

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

        /// <summary>
        /// Get city coordinates from the location file
        /// </summary>
        /// <param name="city">
        /// City which coordinates we want to get
        /// </param>
        /// <returns></returns>
        private Coordinates GetCityCoordinates(City city)
        {
            Coordinates cityCoordinates = null;

            if (citiesLocations.locations.ContainsKey(city))
            {
                citiesLocations.locations.TryGetValue(city, out cityCoordinates);
            }

            return (cityCoordinates);
        }

        /// <summary>
        /// Revers the order of the cities in the list
        /// </summary>
        /// <param name="cities">
        /// List of cities which order we want to reverse
        /// </param>
        private void ReverseListOrder(ref List<City> cities)
        {
            City tempCity = null;
            int numElements = cities.Count;

            // Reverse the order in place
            for (int i = 0; i < (int)(numElements / 2); i++)
            {
                tempCity = cities[i];

                cities[i] = cities[numElements - i - 1];
                cities[numElements - i - 1] = tempCity;
            }
        }

        #endregion Private methods
    }
}
