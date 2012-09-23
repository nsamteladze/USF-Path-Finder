using System;                      // For standard functions
using System.Collections.Generic;
using System.Windows.Media;        // For different collections
using GraphSharp.Controls;
using QuickGraph;
using System.Diagnostics;
using System.ComponentModel;
using Path_Finder.Source.Algorithms;

namespace Path_Finder
{
    /* DESCRIPTION:
     * Contains all the common data types used in the project.
     */
    namespace DataTypes
    {
        /* DESCRIPTION:
         * Point on the map.
         * Has two coordinates: X and Y.
         */
        public class Coordinates
        {
            // Data
            private int xCoord, yCoord;  // X and Y coordinates respectively

            // Constructors
            public Coordinates()
            {
                xCoord = -1;
                yCoord = -1;
            }

            /* DESCRIPTION:
             * Construct an object and check that coordinates
             * are in the [0, 800] range.
             */
            /* TODO:
             * Replace console output with exceptions.
             */
            public Coordinates(int newXCoord, int newYCoord)
            {
                // Check does the coordinate fit in the range
                if ((newXCoord >= 0) && (newXCoord <= 800))
                {
                    xCoord = newXCoord;
                }
                // If it does not fit: initialize the coordinate with -1
                // and output an error.
                else
                {
                    xCoord = -1;
                    Console.WriteLine("ERROR! Coordinate X must be in the range [0, 800]. " +
                                      "X coordinate is initialized with value -1.");
                }
                // Check does the coordinate fit in the range
                if ((newYCoord >= 0) && (newYCoord <= 800))
                {
                    yCoord = newYCoord;
                }
                // If it does not fit: initialize the coordinate with -1
                // and output an error.
                else
                {
                    yCoord = -1;
                    Console.WriteLine("ERROR! Coordinate Y must be in the range [0, 800]. " +
                                      "Y coordinate is initialized with value -1.");
                }
            }

            public Coordinates(Coordinates coordinates)
            {
                xCoord = coordinates.getX();
                yCoord = coordinates.getY();
            }

            // Get Data Methods
            public int getX()
            {
                return (xCoord);
            }

            public int getY()
            {
                return (yCoord);
            }

            // Set Data Methods
            /* DESCRIPTION:
             * Set a new value to the X coordinate.
             * Check does the new value fit in the [0, 800] range.
             * Does NOT change the coordinate's value if the new value
             * is incorrect.
             */
            public void setX(int newXCoord)
            {
                // Check the range
                if ((newXCoord >= 0) && (newXCoord <= 800))
                {
                    xCoord = newXCoord;
                }
                // Output an error if the new coordinate does not fit in the range.
                // Do NOT change the coordinate's value.
                else
                {
                    Console.WriteLine("ERROR! Coordinate X must be in the range [0, 800].");
                }
            }

            /* DESCRIPTION:
             * Set a new value to the Y coordinate.
             * Check does the new value fit in the [0, 800] range.
             * Does NOT change the coordinate's value if the new
             * value is incorrect.
             */
            public void setY(int newYCoord)
            {
                // Check the [0, 800] range
                if ((newYCoord >= 0) && (newYCoord <= 800))
                {
                    yCoord = newYCoord;
                }
                // Output an error if it does not fit.
                // Do NOT change the coordinate's value.
                else
                {
                    Console.WriteLine("ERROR! Coordinate Y must be in the range [0, 800].");
                }
            }

            public override int GetHashCode()
            {
                return (xCoord.GetHashCode() ^ yCoord.GetHashCode());
            }

            public override bool Equals(object obj)
            {
                return (Equals(obj as Coordinates));
            }

            public bool Equals(Coordinates coord)
            {
                return ((coord != null) &&
                        (xCoord == coord.xCoord) &&
                        (yCoord == coord.yCoord));
            }

            public override string ToString()
            {
                return (string.Format("X = {0}, Y = {1}", xCoord, yCoord));
            }
        }

        /* DESCRIPTION:
         * City on the map. Contains only the city's name.
         */
        public class City
        {
            // Data
            private string cityName;  // Name of the city

            // Constructors
            public City()
            {
                cityName = "";
            }

            /* DESCRIPTION:
             * Construct an object and check that the city name's
             * length is from 1 to 80 symbols.
             */
            public City(string newCityName)
            {
                // Check the number of symbols in the city's name
                if ((newCityName.Length > 0) && (newCityName.Length <= 80))
                {
                    cityName = newCityName;
                }
                // Output an error if the number of symbols is incorrect.
                // Initialize the city's name with an empty string ("").
                else
                {
                    cityName = "";
                    Console.WriteLine("ERROR! The length of the city name must be in the range [0, 80]. " +
                                      "The city's name is initialized with an empty string.");
                }
            }

            public City(City city)
            {
                cityName = city.getName();
            }

            // Get Data Methods
            public string getName()
            {
                return (cityName);
            }

            // Set Data Methods
            /* DESCRIPTION:
             * Set a new value to the city's name.
             * Check that the city name's length is from 1 to 80 symbols.
             * Does NOT change the city's name if the new name is incorrect.
             */
            public void setName(string newCityName)
            {
                // Check the number of symbols in the new city's name
                if ((newCityName.Length > 0) && (newCityName.Length <= 80))
                {
                    cityName = newCityName;
                }
                // Output an error if the number of symbols in the new city's name 
                // is incorrect. Do NOT change the city's name.
                else
                {
                    Console.WriteLine("ERROR! The length of the city name must be in the range [0, 80].");
                }
            }

            // Override functions to implement proper comparasion.
            /* DESCRIPTIONS:
             * Overrides the GetHashCode() function.
             * Hash code of a City object is computed based on its cityName value.
             */
            public override int GetHashCode()
            {
                // Hash code is the city name's hash code
                return (cityName.GetHashCode());
            }

            /* DESCRIPTION:
             * Overrides the Equal() method when City is compared
             * with object instance.
             * Invokes Equals(City) to decide if the objects are equal.
             */
            public override bool Equals(object obj)
            {
                // Invoke Equal(City)
                return (Equals(obj as City));
            }

            /* DESCRIPTION:
             * Used to compare two City instances.
             * Cities are equal if they have the same name.
             */
            public bool Equals(City city)
            {
                // Check the the compared with city is not null.
                // Equal if they have the same name.
                return ((city != null) && (city.cityName == this.cityName));
            }

            public override string ToString()
            {
                return (cityName);
            }
        }

        /// <summary>
        /// Represents a pair of cities
        /// </summary>
        public class CityPair
        {
            public City FromCity { get; set; }
            public City ToCity { get; set; }

            public CityPair(City _fromCity, City _toCity)
            {
                FromCity = new City(_fromCity);
                ToCity = new City(_toCity);
            }

            public override int GetHashCode()
            {
                return (FromCity.GetHashCode() ^ ToCity.GetHashCode());
            }

            public override bool Equals(object obj)
            {
                return (Equals(obj as CityPair));
            }

            public bool Equals(CityPair cityPair)
            {
                return ((cityPair != null) &&
                        (FromCity.Equals(cityPair.FromCity)) &&
                        (ToCity.Equals(cityPair.ToCity)));
            }
        }

        /* DESCRIPTION:
         * Information about all cities' locations on the map.
         * Determines how this data must be structured.
         */
        public class CitiesLocations
        {
            // Data
            public Dictionary<City, Coordinates> locations;  // Cities' locations

            // Constructors
            public CitiesLocations()
            {
                locations = new Dictionary<City, Coordinates>();
            }

            private void Reset()
            {
                locations = null;
            }

            public void Copy(CitiesLocations newCitiesLocations)
            {
                Reset();
                locations = new Dictionary<City, Coordinates>();
                Coordinates tempCoordinates;

                foreach (City city in newCitiesLocations.locations.Keys)
                {
                    newCitiesLocations.locations.TryGetValue(city, out tempCoordinates);
                    locations.Add(new City(city), new Coordinates(tempCoordinates));
                }
            }

            public override int GetHashCode()
            {
                int globalHashCode = 0;
                int unitHashCode;

                Coordinates tempCityCoordinates;

                foreach (City city in locations.Keys)
                {
                    locations.TryGetValue(city, out tempCityCoordinates);
                    unitHashCode = city.GetHashCode() ^ tempCityCoordinates.GetHashCode();

                    if (globalHashCode == 0)
                    {
                        globalHashCode = unitHashCode;
                    }
                    else
                    {
                        globalHashCode = globalHashCode ^ unitHashCode;
                    }
                }

                return (globalHashCode);
            }

            public override bool Equals(object obj)
            {
                return (Equals(obj as CitiesLocations));
            }

            public bool Equals(CitiesLocations toCompare)
            {
                if (toCompare == null)
                {
                    return (false);
                }
                if (locations.Count != toCompare.locations.Count)
                {
                    return (false);
                }

                foreach (KeyValuePair<City, Coordinates> dictPair in locations)
                {
                    if (!toCompare.locations.ContainsKey(dictPair.Key))
                    {
                        return (false);
                    }
                    if (!dictPair.Value.Equals(toCompare.locations[dictPair.Key]))
                    {
                        return (false);
                    }
                }

                return (true);
            }
        }

        /* DESCRIPTION:
         * Information about the connections each city has.
         * Determines how this data must be structured.
         */
        public class CitiesConnections
        {
            // Data
            /* DESCRIPTION:
             * Hash table.
             * Key - City
             * Value - List of City instances. Represents connections.
             */
            public Dictionary<City, List<City>> connections;

            // Constructors
            public CitiesConnections()
            {
                connections = new Dictionary<City, List<City>>();
            }

            private void Reset()
            {
                connections = null;
            }

            public void Copy(CitiesConnections newCitiesConnections)
            {
                Reset();
                connections = new Dictionary<City, List<City>>();
                List<City> tempCityConnections;

                foreach (City city in newCitiesConnections.connections.Keys)
                {
                    newCitiesConnections.connections.TryGetValue(city, out tempCityConnections);
                    connections.Add(new City(city), new List<City>(tempCityConnections));
                }
            }


        }

        /// <summary>
        /// Different algorithm's speeds
        /// </summary>
        public enum Alg_Speed
        {
            Steps = 0,
            Slow = 1,
            Fast = 2
        }

        /// <summary>
        /// Different algorithm's heuristics
        /// </summary>
        public enum Heuristic
        {
            Distance = 0,
            Hops = 1
        }

        /// <summary>
        /// Node of a graph
        /// </summary>
        public class VertexCity
        {
            public City City
            {
                get;
                set;
            }
            public Coordinates CityCoordinates
            {
                get;
                set;
            }

            /* WARNING:
             * Constructor does not invoke new method
             */
            public VertexCity(City _city, Coordinates _cityCoordinates)
            {
                City = _city;
                CityCoordinates = _cityCoordinates;
            }

            public VertexCity()
            {
                City = null;
                CityCoordinates = null;
            }

            public VertexCity(City _city)
            {
                City = _city;
                CityCoordinates = null;
            }

            public override int GetHashCode()
            {
                return (City.GetHashCode());
            }

            public override bool Equals(object obj)
            {
                return (Equals(obj as VertexCity));
            }

            public bool Equals(VertexCity toCompare)
            {
                return ((toCompare != null) &&
                        (City.Equals(toCompare.City)));
            }

            public override string ToString()
            {
                return (string.Format("Name: {0} {1}", City, CityCoordinates));
            }


        }

        /// <summary>
        /// Edge of the graph
        /// </summary>
        public class EdgeCity : Edge<VertexCity>
        {
            public EdgeCity(VertexCity _source, VertexCity _target)
                : base(_source, _target)
            {
            }

            public override string ToString()
            {
                return (string.Format("{0}-->{1}", Source, Target));
            }

            public override int GetHashCode()
            {
                return (Source.GetHashCode() ^ Target.GetHashCode());
            }

            public override bool Equals(object obj)
            {
                return (Equals(obj as EdgeCity));
            }

            public bool Equals(EdgeCity toCompare)
            {
                return ((toCompare != null) &&
                        (Source.Equals(toCompare.Source)) &&
                        (Target.Equals(toCompare.Target)));
            }
        }

        /// <summary>
        /// Graph with custom nodes and edges
        /// </summary>
        public class GraphCity : BidirectionalGraph<VertexCity, EdgeCity>
        {
            public GraphCity() 
            { 
            }
        }

        /// <summary>
        /// Layout for the custom graph
        /// </summary>
        public class GraphLayoutCity : GraphLayout<VertexCity, EdgeCity, GraphCity> 
        { 
        }

        /// <summary>
        /// Data about a city used in the A-star algorithm
        /// </summary>
        public class CityInfo
        {
            #region Data

            public City City         // The city this info is about
            {
                get;
                private set;
            }             
            public City PrevCity     // Previous city in the path from the start city
            {
                get;
                set;
            }
            public City FinalCity    // The desired final city
            {
                get;
                private set;
            }

            public Coordinates CityCoordinates
            {
                get;
                private set;
            }
            public Coordinates PrevCityCoordinates
            {
                get;
                set;
            }
            public Coordinates FinalCityCoordinates
            {
                get;
                private set;
            }

            private double fromStart;
            public double FromStart // Distance from start (precise)
            {
                get
                {
                    return (fromStart);
                }
                set
                {
                    // Check that >= 0
                    if (value >= 0)
                    {
                        fromStart = value;
                        // Calculate a new path distance
                        CalculatePathDist();
                    }
                }
            }
            public double ToEnd         // Distance to the end city (approximate)
            {
                get;
                private set;
            }
            public double PathDistance  // Total path distance = toEnd + from Start
            {
                get;
                private set;
            }

            public bool IsExplored      // Has this city been explored or not so far
            {
                get;
                set;
            }

            public Heuristic usedHeuristic
            {
                get;
                private set;
            }

            #endregion Data

            #region Constructors

            public CityInfo()
            {
                City = null;
                PrevCity = null;
                FinalCity = null;

                CityCoordinates = null;
                PrevCityCoordinates = null;
                FinalCityCoordinates = null;

                FromStart = -1;
                ToEnd = -1;
                PathDistance = -1;

                IsExplored = false;

                usedHeuristic = Heuristic.Distance;
            }

            public CityInfo(City _city, City _prevCity, City _finalCity,
                Coordinates _cityCoordinates, Coordinates _prevCityCoordinates,
                Coordinates _finalCityCoordinates, double _fromStart,
                Heuristic _usedHeuristic)
            {
                City = _city;
                PrevCity = _prevCity;
                FinalCity = _finalCity;

                CityCoordinates = _cityCoordinates;
                PrevCityCoordinates = _prevCityCoordinates;
                FinalCityCoordinates = _finalCityCoordinates;

                // ToEnd and PathDistance are calculated automatically when
                // FromStart is set
                FromStart = _fromStart;

                IsExplored = false;

                usedHeuristic = _usedHeuristic;
            }

            #endregion Constructors

            #region Private Methods

            /* DESCRIPTION:
             * Calculate the distance to the final city
             */
            private void CalculateDistToEnd()
            {
                // Calculate the distance
                if (usedHeuristic == Heuristic.Distance)
                {
                    ToEnd = Algorithm.FindDistance(CityCoordinates, FinalCityCoordinates);
                }
                else if (usedHeuristic == Heuristic.Hops)
                {
                    ToEnd = 1;
                }
            }

            /* DESCRIPTION:
             * Calculate the total path distance.
             * Automatically calculates the distance to the 
             * end city if it is not set.
             */
            private void CalculatePathDist()
            {
                // Calculate if the distance from start is known
                if (FromStart != -1)
                {
                    // If the distance to the destinations is not
                    // known, then calculate it.
                    if (ToEnd == -1)
                    {
                        CalculateDistToEnd();
                    }

                    // Calculate the total path distance
                        PathDistance = FromStart + ToEnd;
                }
            }

            #endregion Private Methods
        }
    }
}
