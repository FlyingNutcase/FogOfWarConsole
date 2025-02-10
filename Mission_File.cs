using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace FogOfWarConsole
{
    public class MissionFile
    {
        private Dictionary<string, List<string>> sections = new Dictionary<string, List<string>>();

        private string fowFilePath; // Path for the -fow.mis file

        public string Map { get; set; }
        public double Time { get; set; }
        public int CloudType { get; set; }
        public double CloudHeight { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public double WindDirection { get; set; }
        public double WindSpeed { get; set; }
        public int Gust { get; set; }
        public int Turbulence { get; set; }


        public MissionFile(string filePath)
        {
            Load(filePath);

            fowFilePath = filePath.Substring(0, filePath.Length - 4) + "-fow.mis";

        }

        public void Load(string filePath)
        {
            sections.Clear();
            using (StreamReader reader = new StreamReader(filePath))
            {
                string currentSection = null;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();
                    if (string.IsNullOrEmpty(line) || line.StartsWith(";")) continue; // Skip empty lines and comments

                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        currentSection = line.Substring(1, line.Length - 2);
                        if (!sections.ContainsKey(currentSection))
                        {
                            sections[currentSection] = new List<string>();
                        }
                    }
                    else if (currentSection != null)
                    {
                        sections[currentSection].Add(line);
                    }
                }
            }

            ParseMainSection();
            ParseSeasonSection();
            ParseWeatherSection();

        }

        private void ParseMainSection()
        {
            if (sections.ContainsKey("MAIN"))
            {
                foreach (string line in sections["MAIN"])
                {
                    string[] parts = line.Split(new[] { ' ' }, 2);
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();

                        switch (key)
                        {
                            case "MAP":
                                Map = value;
                                break;
                            case "TIME":
                                Time = ParseDouble(value);
                                break;
                            case "CloudType":
                                CloudType = int.Parse(value);
                                break;
                            case "CloudHeight":
                                CloudHeight = ParseDouble(value);
                                break;
                                // ... other MAIN properties
                        }
                    }
                }
            }
        }

        private void ParseSeasonSection()
        {
            if (sections.ContainsKey("SEASON"))
            {
                foreach (string line in sections["SEASON"])
                {
                    string[] parts = line.Split(new[] { ' ' }, 2);
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();

                        switch (key)
                        {
                            case "Year":
                                Year = int.Parse(value);
                                break;
                            case "Month":
                                Month = int.Parse(value);
                                break;
                            case "Day":
                                Day = int.Parse(value);
                                break;
                        }
                    }
                }
            }
        }

        private void ParseWeatherSection()
        {
            if (sections.ContainsKey("WEATHER"))
            {
                foreach (string line in sections["WEATHER"])
                {
                    string[] parts = line.Split(new[] { ' ' }, 2);
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();

                        switch (key)
                        {
                            case "WindDirection":
                                WindDirection = ParseDouble(value);
                                break;
                            case "WindSpeed":
                                WindSpeed = ParseDouble(value);
                                break;
                            case "Gust":
                                Gust = int.Parse(value);
                                break;
                            case "Turbulence":
                                Turbulence = int.Parse(value);
                                break;
                        }
                    }
                }
            }
        }

        //Helper to parse doubles accounting for different culture settings
        private double ParseDouble(string value)
        {
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }
            else
            {
                // Handle parsing failure (e.g., throw an exception or return a default value)
                throw new FormatException($"Could not parse double: {value}");
            }
        }


        // Example of accessing and modifying data (you'll need to implement more of these)
        public List<string> GetWaypoints(string wingName)
        {
            if (sections.ContainsKey(wingName + "_Way"))
            {
                return sections[wingName + "_Way"];
            }
            return new List<string>();
        }

        public void SetTime(double newTime)
        {
            Time = newTime;
            // You'll need to update the "MAIN" section string in the sections dictionary
            // to reflect this change if you intend to save the file later.
        }

        public void Save()
        {
            // Important: Update the corresponding line in the sections dictionary
            if (sections.ContainsKey("MAIN"))
            {
                for (int i = 0; i < sections["MAIN"].Count; i++)
                {
                    string line = sections["MAIN"][i];
                    if (line.StartsWith("CloudType"))
                    {
                        sections["MAIN"][i] = $"CloudType {CloudType}";
                        break; // Found and updated the line, exit the loop
                    }
                }
            }



            using (StreamWriter writer = new StreamWriter(fowFilePath))
            {
                foreach (var section in sections)
                {
                    writer.WriteLine($"[{section.Key}]");
                    foreach (var line in section.Value)
                    {
                        writer.WriteLine(line);
                    }
                }
            }
        }

        // ... Add more methods to access and modify other sections (Wings, Static objects, Buildings, etc.)
    }
}
