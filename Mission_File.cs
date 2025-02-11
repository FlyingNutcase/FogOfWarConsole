using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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

        // Add the Wings collection
        public List<Wing> Wings { get; set; } = new List<Wing>();
        private List<string> wingIdentifiers = new List<string>();

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
            ParseWingIdentifiers();
            ParseWings();
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

        private void ParseWingIdentifiers()
        {
            if (sections.ContainsKey("Wing"))
            {
                foreach (string line in sections["Wing"])
                {
                    wingIdentifiers.Add(line.Trim());
                }
            }
        }

        private void ParseWings()
        {
            foreach (var wingIdentifier in wingIdentifiers)
            {
                if (sections.ContainsKey(wingIdentifier))
                {
                    Wing wing = new Wing { Name = wingIdentifier };
                    foreach (string line in sections[wingIdentifier])
                    {
                        string[] parts = line.Split(new[] { ' ' }, 2);
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            string value = parts[1].Trim();

                            switch (key)
                            {
                                case "Planes":
                                    wing.Planes = int.Parse(value);
                                    break;
                                case "Skill":
                                    wing.Skill0 = wing.Skill1 = wing.Skill2 = wing.Skill3 = int.Parse(value);
                                    break;
                                case "Skill0":
                                    wing.Skill0 = int.Parse(value);
                                    break;
                                case "Skill1":
                                    wing.Skill1 = int.Parse(value);
                                    break;
                                case "Skill2":
                                    wing.Skill2 = int.Parse(value);
                                    break;
                                case "Skill3":
                                    wing.Skill3 = int.Parse(value);
                                    break;
                                case "skin0":
                                    wing.Skin0 = value;
                                    break;
                                case "skin1":
                                    wing.Skin1 = value;
                                    break;
                                case "skin2":
                                    wing.Skin2 = value;
                                    break;
                                case "skin3":
                                    wing.Skin3 = value;
                                    break;
                                case "pilot0":
                                    wing.Pilot0 = value;
                                    break;
                                case "pilot1":
                                    wing.Pilot1 = value;
                                    break;
                                case "pilot2":
                                    wing.Pilot2 = value;
                                    break;
                                case "pilot3":
                                    wing.Pilot3 = value;
                                    break;
                                case "numberOn0":
                                    wing.NumberOn0 = int.Parse(value);
                                    break;
                                case "numberOn1":
                                    wing.NumberOn1 = int.Parse(value);
                                    break;
                                case "numberOn2":
                                    wing.NumberOn2 = int.Parse(value);
                                    break;
                                case "numberOn3":
                                    wing.NumberOn3 = int.Parse(value);
                                    break;
                                case "Class":
                                    wing.Class = value;
                                    break;
                                case "Fuel":
                                    wing.Fuel = int.Parse(value);
                                    break;
                                case "weapons":
                                    wing.Weapons = value;
                                    break;
                                case "StartTime":
                                    wing.StartTime = int.Parse(value);
                                    break;
                            }
                        }
                    }
                    Wings.Add(wing);
                }

                if (sections.ContainsKey(wingIdentifier + "_Way"))
                {
                    Wing wing = Wings.FirstOrDefault(w => w.Name == wingIdentifier);
                    if (wing != null)
                    {
                        foreach (string line in sections[wingIdentifier + "_Way"])
                        {
                            string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length >= 5)
                            {
                                Waypoint waypoint = new Waypoint
                                {
                                    Wing = wing, // Set the Wing reference
                                    Type = parts[0],
                                    X = ParseDouble(parts[1]),
                                    Y = ParseDouble(parts[2]),
                                    Altitude = ParseDouble(parts[3]),
                                    Speed = ParseDouble(parts[4]),
                                    Target = parts.Length > 5 ? string.Join(" ", parts.Skip(5)) : null
                                };
                                wing.Waypoints.Add(waypoint);
                            }
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
    }

    public class Wing
    {
        public string Name { get; set; }
        public int Planes { get; set; }
        public int Skill0 { get; set; }
        public int Skill1 { get; set; }
        public int Skill2 { get; set; }
        public int Skill3 { get; set; }
        public string Skin0 { get; set; }
        public string Skin1 { get; set; }
        public string Skin2 { get; set; }
        public string Skin3 { get; set; }
        public string Pilot0 { get; set; }
        public string Pilot1 { get; set; }
        public string Pilot2 { get; set; }
        public string Pilot3 { get; set; }
        public int NumberOn0 { get; set; }
        public int NumberOn1 { get; set; }
        public int NumberOn2 { get; set; }
        public int NumberOn3 { get; set; }
        public string Class { get; set; }
        public int Fuel { get; set; }
        public string Weapons { get; set; }
        public int StartTime { get; set; }
        public List<Waypoint> Waypoints { get; set; } = new List<Waypoint>();
    }

    public class Waypoint
    {
        public Wing Wing { get; set; }
        public string Type { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Altitude { get; set; }
        public double Speed { get; set; }
        public string Target { get; set; }
    }
}
