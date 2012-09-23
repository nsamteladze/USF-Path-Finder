using System;
using System.IO;
using System.Collections.Generic;
using Path_Finder.DataTypes;
using System.Windows;

namespace Path_Finder
{
    /* DESCRIPTION:
     * Incorporates all the operations with files
     */
    public static class FileManager
    {
        /* DESCRIPTION:
         * Reads the cities' locations from the file
         */
        public static CitiesLocations ReadLocations(string filePath)
        {
            // Read the data of file exists
            if (File.Exists(filePath))
            {
                string nextLine;                       // Read line from the file
                string[] lineData;                     // Data that the read line contains
                char[] delimeter = " ".ToCharArray();  // Delimiter that separates data in the file
                string tempCityName;                   // City's name during each iteration
                int tempXCoord, tempYCoord;            // City's coordinates for each city
                StreamReader inputFile = null;         // File to read from
                CitiesLocations citiesLocations;       // Locations of the cities 

                // Exceptions may arise while reading from the file
                try
                {
                    // Initialize
                    inputFile = new StreamReader(filePath);
                    citiesLocations = new CitiesLocations();

                    // Read till the end of the file
                    while ((nextLine = inputFile.ReadLine()) != null)
                    {
                        // If the END flag is reached, then all data has been read
                        if (nextLine.Equals("END"))
                        {
                            return (citiesLocations);
                        }
                        // If it is not the end, then read the next line from the file
                        else
                        {
                            lineData = nextLine.Split(delimeter, 3);  // Read the next line
                            tempCityName = lineData[0];               // First data - city's name
                            tempXCoord = Int32.Parse(lineData[1]);    // Second - X coordinate
                            tempYCoord = Int32.Parse(lineData[2]);    // Third - 

                            // Add a new city to the list
                            citiesLocations.locations.Add(new City(tempCityName), 
                                                          new Coordinates(tempXCoord, tempYCoord));
                        }
                    }

                    // No END flag. Output an error. File is corrupted.
                    MessageBox.Show("ERROR! File is corrupted.", "Path Finder", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return (null);
                }
                // In case of exception, output its message
                catch (Exception exc)
                {
                    MessageBox.Show(exc.ToString(), "Path Finder", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return (null);
                }
                // Close the input file in the end
                finally
                {
                    if (inputFile != null)
                    {
                        inputFile.Close();
                    }              
                }
            }
            // If the input file does not exist, output an error.
            else
            {
                MessageBox.Show("ERROR! File not found.", "Path Finder", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                return (null);
            }
        }

        /// <summary>
        /// Checks that the file contains graph locations information
        /// </summary>
        /// <param name="filePath">
        /// Path to the checked file
        /// </param>
        /// <returns></returns>
        public static bool VerifyLocationsFile(string filePath)
        {
            // Read the data of file exists
            if (File.Exists(filePath))
            {
                string nextLine;                       // Read line from the file
                string[] lineData;                     // Data that the read line contains
                char[] delimeter = " ".ToCharArray();  // Delimiter that separates data in the file
                int tempXCoord, tempYCoord;            // City's coordinates for each city
                StreamReader inputFile = null;         // File to read from

                // Exceptions may arise while reading from the file
                try
                {
                    // Initialize
                    inputFile = new StreamReader(filePath);

                    // Read till the end of the file
                    while ((nextLine = inputFile.ReadLine()) != null)
                    {
                        // If the END flag is reached, then all data has been read
                        if (nextLine.Equals("END"))
                        {
                            return (true);
                        }
                        // If it is not the end, then read the next line from the file
                        else
                        {
                            lineData = nextLine.Split(delimeter);  // Read the next line
                            if (lineData.Length != 3)
                            {
                                return (false);
                            }
                            else if (!Int32.TryParse(lineData[1], out tempXCoord) || !Int32.TryParse(lineData[2], out tempYCoord))
                            {
                                return (false);
                            }
                        }
                    }

                    // No END flag. Output an error. File is corrupted.
                    return (false);
                }
                // In case of exception, output its message
                catch (Exception exc)
                {
                    MessageBox.Show(exc.ToString(), "Path Finder", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return (false);
                }
                // Close the input file in the end
                finally
                {
                    if (inputFile != null)
                    {
                        inputFile.Close();
                    }
                }
            }
            // If the input file does not exist, output an error.
            else
            {
                return (false);
            }
        }

        /* DESCRIPTION:
         * Read the cities' connections from the file
         */
        public static CitiesConnections ReadConnections(string filePath)
        {
            // Read if the input file exists
            if (File.Exists(filePath))
            {
                CitiesConnections citiesConnections;   // Cities' connections
                StreamReader inputFile = null;         // Input file
                char[] delimeter = " ".ToCharArray();  // Delimeter that separates the data in the input file
                string nextLine;                       // Currently read line from the input file
                string[] lineData;                     // Data the contains in the read line
                int tempNumConnections;                // Number of connections a city has at each iteration
                List<City> tempConnection;             // List of connected cities at each iteration

                // Exceptions may arise during the file usage
                try
                {
                    // Initialize
                    inputFile = new StreamReader(filePath);                    
                    citiesConnections = new CitiesConnections();
                    
                    // Read till the end of the file
                    while ((nextLine = inputFile.ReadLine()) != null)
                    {
                        // If END flag has discovered that we are done
                        if (nextLine.Equals("END"))
                        {
                            return (citiesConnections);
                        }
                        // If no END flag so far, then read the next line
                        else
                        {
                            // Read the next line
                            lineData = nextLine.Split(delimeter);

                            // Number of connections goes second in the line
                            tempNumConnections = Int32.Parse(lineData[1]);  
                            // Read the list of connected cities
                            tempConnection = new List<City>();
                            for (int i = 0; i < tempNumConnections; i++)
                            {
                                // Add city to the connections list
                                tempConnection.Add(new City(lineData[i + 2]));
                            }

                            // Add the city and its connection to cities' connections
                            citiesConnections.connections.Add(new City(lineData[0]), tempConnection);
                        }
                    }

                    // Error if no END flag has been discovered. The input file is corrupted.
                    MessageBox.Show("ERROR! File is corrupted.", "Path Finder", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return (null);
                }
                // In case of exception, output its message
                catch (Exception exc)
                {
                    MessageBox.Show(exc.ToString(), "Path Finder", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return (null);
                }
                // Close the input file in the end
                finally
                {
                    if (inputFile != null)
                    {
                        inputFile.Close();
                    }
                }
            }
            // Error if the input file not found
            else
            {
                MessageBox.Show("ERROR! File not found.", "Path Finder", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                return (null);
            }
        }

        /// <summary>
        /// Checks that the file contains graph connections information
        /// </summary>
        /// <param name="filePath">
        /// Path to the checked file
        /// </param>
        /// <returns></returns>
        public static bool VerifyConnectionsFile(string filePath)
        {
            // Read if the input file exists
            if (File.Exists(filePath))
            {
                StreamReader inputFile = null;         // Input file
                char[] delimeter = " ".ToCharArray();  // Delimeter that separates the data in the input file
                string nextLine;                       // Currently read line from the input file
                string[] lineData;                     // Data the contains in the read line
                int tempNumConnections;                // Number of connections a city has at each iteration

                // Exceptions may arise during the file usage
                try
                {
                    // Initialize
                    inputFile = new StreamReader(filePath);

                    // Read till the end of the file
                    while ((nextLine = inputFile.ReadLine()) != null)
                    {
                        // If END flag has discovered that we are done
                        if (nextLine.Equals("END"))
                        {
                            return (true);
                        }
                        // If no END flag so far, then read the next line
                        else
                        {
                            // Read the next line
                            lineData = nextLine.Split(delimeter);

                            if (lineData.Length < 2)
                            {
                                return (false);
                            }

                            // Number of connections goes second in the line
                            tempNumConnections = Int32.Parse(lineData[1]);
                            if (!Int32.TryParse(lineData[1], out tempNumConnections))
                            {
                                return (false);
                            }

                            if (lineData.Length < tempNumConnections + 2)
                            {
                                return (false);
                            }
                        }
                    }

                    // Error if no END flag has been discovered. The input file is corrupted.
                    return (false);
                }
                // In case of exception, output its message
                catch (Exception exc)
                {
                    MessageBox.Show(exc.ToString(), "Path Finder", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return (false);
                }
                // Close the input file in the end
                finally
                {
                    if (inputFile != null)
                    {
                        inputFile.Close();
                    }
                }
            }
            // Error if the input file not found
            else
            {
                return (false);
            }
        }
    }
}
