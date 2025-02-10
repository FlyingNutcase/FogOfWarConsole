using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json; // Install Newtonsoft.Json NuGet package

namespace FogOfWarConsole
{
    public class WeatherSettings
    {
        [JsonProperty("CloudTypeWeights")]
        public Dictionary<int, int> CloudTypeWeights { get; set; } // CloudType -> Weight

        [JsonProperty("CloudHeightWeights")]
        public Dictionary<double, int> CloudHeightWeights { get; set; } // CloudHeight -> Weight

        private Random random;

        public WeatherSettings()
        {
            random = new Random();
            CloudTypeWeights = new Dictionary<int, int>();
            CloudHeightWeights = new Dictionary<double, int>();
        }

        public void LoadFromFile(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                Console.WriteLine("JSON content:");
                Console.WriteLine(json);

                // Directly deserialize into the WeatherSettings object
                JsonConvert.PopulateObject(json, this);

                Console.WriteLine("Deserialized CloudTypeWeights:");
                foreach (var kvp in CloudTypeWeights)
                {
                    Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
                }

                Console.WriteLine("Deserialized CloudHeightWeights:");
                foreach (var kvp in CloudHeightWeights)
                {
                    Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Error: File not found at {filePath}");
                HandleLoadingError();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error: Invalid JSON format in {filePath}: {ex.Message}");
                HandleLoadingError();
            }
        }

        private void HandleLoadingError()
        {
            // Handle the error appropriately (e.g., use default weights)
            CloudTypeWeights.Add(1, 1);
            CloudTypeWeights.Add(2, 4);
            CloudTypeWeights.Add(3, 2);
            CloudTypeWeights.Add(4, 0);

            CloudHeightWeights.Add(1000.0, 1);
            CloudHeightWeights.Add(2000.0, 2);
            CloudHeightWeights.Add(3000.0, 1);
            CloudHeightWeights.Add(4000.0, 0);
        }

        public int GetRandomCloudType()
        {
            return GetWeightedRandomValue(CloudTypeWeights);
        }

        public double GetRandomCloudHeight()
        {
            return GetWeightedRandomValue(CloudHeightWeights);
        }

        private T GetWeightedRandomValue<T>(Dictionary<T, int> weights) where T : notnull
        {
            if (weights == null || weights.Count == 0)
            {
                throw new InvalidOperationException("Weights must be loaded from a file first.");
            }

            List<T> values = weights.Keys.ToList();
            List<int> weightValues = weights.Values.ToList();

            int totalWeight = weightValues.Sum();
            if (totalWeight <= 0)
            {
                throw new InvalidOperationException("Total weight must be greater than zero.");
            }

            int randomNumber = random.Next(0, totalWeight);
            int cumulativeWeight = 0;

            for (int i = 0; i < values.Count; i++)
            {
                cumulativeWeight += weightValues[i];
                if (randomNumber < cumulativeWeight)
                {
                    return values[i];
                }
            }

            return values.Last(); // Should not happen, but included for safety
        }
    }
}
