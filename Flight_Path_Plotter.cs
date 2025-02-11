using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace FogOfWarConsole
{
    public class FlightRoutePlotter
    {
        public static void GenerateFlightPathImage(string filePath, string outputFile)
        {
            List<List<PointF>> flightRoutes = LoadRoutes(filePath);
            CreateImage(flightRoutes, outputFile);
            Console.WriteLine($"Flight path image generated: {outputFile}");
        }

        private static List<List<PointF>> LoadRoutes(string filePath)
        {
            List<List<PointF>> routes = new List<List<PointF>>();
            List<PointF> currentRoute = new List<PointF>();

            foreach (string line in File.ReadLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] parts = line.Split();
                if (parts[0] == "TAKEOFF" || parts[0] == "NORMFLY" || parts[0] == "LANDING")
                {
                    float x = float.Parse(parts[1]);
                    float y = float.Parse(parts[2]);
                    currentRoute.Add(new PointF(x, y));
                }
                else if (parts[0] == "#") // Use "#" to separate multiple routes in the file
                {
                    if (currentRoute.Count > 0)
                    {
                        routes.Add(new List<PointF>(currentRoute));
                        currentRoute.Clear();
                    }
                }
            }
            if (currentRoute.Count > 0) routes.Add(currentRoute);
            return routes;
        }

        private static void CreateImage(List<List<PointF>> routes, string outputFile)
        {
            int width = 800, height = 600;
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                Random rand = new Random();
                Font font = new Font("Arial", 8);
                Brush textBrush = Brushes.Black;

                int routeIndex = 1;
                foreach (var route in routes)
                {
                    if (route.Count < 2) continue;
                    Color routeColor = Color.FromArgb(rand.Next(100, 255), rand.Next(100, 255), rand.Next(100, 255));
                    Pen pen = new Pen(routeColor, 2);

                    for (int i = 0; i < route.Count; i++)
                    {
                        PointF scaledPoint = ScalePoint(route[i], width, height);
                        g.FillEllipse(Brushes.Red, scaledPoint.X - 2, scaledPoint.Y - 2, 4, 4); // Mark waypoints
                        g.DrawString($"{routeIndex}-{i + 1}", font, textBrush, scaledPoint); // Label waypoints

                        if (i < route.Count - 1)
                        {
                            g.DrawLine(pen, scaledPoint, ScalePoint(route[i + 1], width, height));
                        }
                    }
                    routeIndex++;
                }
            }
            bmp.Save(outputFile);
        }

        private static PointF ScalePoint(PointF point, int width, int height)
        {
            float scaleX = width / 52000f;  // Adjust based on expected max values
            float scaleY = height / 52000f;
            return new PointF(point.X * scaleX, height - (point.Y * scaleY));
        }
    }
}
