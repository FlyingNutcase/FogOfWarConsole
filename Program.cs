﻿// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FogOfWarConsole
{
    class Program
    {
        static void Main(string[] args) 
        {
            //  User greeting
            Console.WriteLine("Welcome to Fog of War Console App!");
            Console.WriteLine("Press and key to continue...");

            //  File path to the 'base' .mis mission (this will be via the WinForms UI)
            string filePath = "C:\\_My\\CODE\\Missions\\Fighter_Sweep\\Bf-109G205.mis";

            //  Parse the file into a MissionFile object
            MissionFile mission = new MissionFile(filePath); 

            // Display existing properties of interest
            Console.WriteLine($"Cloud Type: {mission.CloudType}");
            Console.WriteLine($"Cloud Height: {mission.CloudHeight}");

            // Load the settings file
            string settingsFilePath = "C:\\_My\\CODE\\Missions\\Fighter_Sweep\\Bf-109G205.fow";
            WeatherSettings randSettings = new WeatherSettings();
            randSettings.LoadFromFile(settingsFilePath);

            //  Assign randomized values
            mission.CloudType = randSettings.GetRandomCloudType();
            mission.CloudHeight = randSettings.GetRandomCloudHeight();

            //  Display the randomized values
            Console.WriteLine($"Cloud Type: {mission.CloudType}");
            Console.WriteLine($"Cloud Height: {mission.CloudHeight}");
            mission.Save();

            Console.ReadKey();
        }
    }

}

