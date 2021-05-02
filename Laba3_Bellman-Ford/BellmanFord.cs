using System;
using System.IO;
using System.Collections.Generic;
using RBTree;

namespace Bellman_ford
{
    public class BellmanFord
    {
        private const int inf = int.MaxValue;

        private readonly List<(string, string, int, int)> flights;

        private readonly List<List<int>> adjacencyMatrix;

        private readonly Map<string, int> CitiesTree;

        private static int countOfCities = 0;

        private readonly List<City> routsGraph;

        private readonly List<string> citiesName;

        public readonly List<string> pathByCities;

        public readonly List<int> pathByPrice;

        public int totalPricePath = 0;


        public BellmanFord(string filePath)
        {
            CitiesTree = new Map<string, int>();

            adjacencyMatrix = new List<List<int>>();

            flights = new List<(string, string, int, int)>();

            routsGraph = new List<City>();

            citiesName = new List<string>();

            pathByCities = new List<string>();

            pathByPrice = new List<int>();

            FileParse(filePath);
        }

        private void FileParse(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception("No such file exists");
            }

            using (StreamReader sr = new StreamReader(filePath, System.Text.Encoding.Default))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {

                    flights.Add(GetFlights(line));
                }

            }

            CreateCitiesTree();

            CreateMatrix();

            CreateFlightGraph();

        }

        private void CreateFlightGraph()
        {
            for (int i = 0; i < countOfCities; i++)
            {
                var edges = new List<Edge>();

                var city = new City();

                for (int j = 0; j < countOfCities; j++)
                {
                    var edge = new Edge();

                    edge.City = citiesName[j];

                    edge.Weight = adjacencyMatrix[j][i];

                    edges.Add(edge);
                }

                city.Edges = edges;

                city.Id = i;

                city.Name = citiesName[i];

                routsGraph.Add(city);

            }
        }

        private void CreateCitiesTree()
        {
            foreach (var cur in flights)
            {
                try
                {
                    CitiesTree.Insert(cur.Item1, countOfCities++);

                    citiesName.Add(cur.Item1);
                }
                catch
                {
                    countOfCities--;
                }

                try
                {
                    CitiesTree.Insert(cur.Item2, countOfCities++);

                    citiesName.Add(cur.Item2);
                }
                catch
                {
                    countOfCities--;
                }
            }

        }

        private void CreateMatrix()
        {
            for (int i = 0; i < countOfCities; i++)
            {
                adjacencyMatrix.Add(new List<int>());

                for (int j = 0; j < countOfCities; j++)
                {
                    adjacencyMatrix[i].Add(inf);

                }
            }

            foreach (var cur in flights)
            {
                try
                {
                    adjacencyMatrix[CitiesTree.Find(cur.Item1).Data][CitiesTree.Find(cur.Item2).Data] = cur.Item4;

                    adjacencyMatrix[CitiesTree.Find(cur.Item2).Data][CitiesTree.Find(cur.Item1).Data] = cur.Item3;
                }
                catch
                {
                    // skip repetitive city
                }

            }
        }

        public void ShowAdjacencyMatrix()
        {
            Console.WriteLine("Adjacency matrix for these flights:\n");

            foreach (var curCity in citiesName)
            {
                Console.Write($"{curCity}\t\t");
            }

            Console.WriteLine("\n");

            for (int i = 0; i < countOfCities; i++)
            {

                for (int j = 0; j < countOfCities; j++)
                {
                    Console.Write($"{adjacencyMatrix[i][j]}\t\t");
                }

                Console.Write("\n");
            }

            Console.Write("\n");
        }

        private static (string, string, int, int) GetFlights(string line)
        {

            var parameters = line.Split(';', StringSplitOptions.RemoveEmptyEntries);

            if (parameters.Length != 4)
            {
                throw new Exception("Invalid data format");
            }

            string city1 = parameters[0];

            string city2 = parameters[1];

            int price1;

            int price2;

            if (parameters[2] == "N/A")
            {
                price1 = inf;
            }
            else
            {
                price1 = Int32.Parse(parameters[2]);
            }

            if (parameters[3] == "N/A")
            {
                price2 = inf;
            }
            else
            {
                price2 = Int32.Parse(parameters[3]);
            }

            return (city1, city2, price1, price2);
        }

        private void BellmanFordAlg(string startCity, string endCity)
        {

            var price = new int[countOfCities];

            var path = new int[countOfCities];

            for (var i = 0; i < countOfCities; i++)
            {
                price[i] = inf;

                path[i] = -1;
            }

            price[CitiesTree.Find(startCity).Data] = 0;

            for (var i = 0; i < countOfCities - 1; i++)
            {
                for (var j = 0; j < countOfCities; j++)
                {
                    if (price[j] < inf)
                    {
                        for (var q = 0; q < countOfCities; q++)
                        {
                            if (routsGraph[j].Edges[q].Weight < inf)
                            {
                                if (price[q] > price[j] + routsGraph[j].Edges[q].Weight)
                                {
                                    price[q] = price[j] + routsGraph[j].Edges[q].Weight;

                                    path[q] = j;
                                }
                            }
                        }
                    }
                }
            }

            GetCitiesAndPrice(path, CitiesTree.Find(startCity).Data, CitiesTree.Find(endCity).Data);

        }

        private void GetCitiesAndPrice(int[] path, int startIndex, int endIndex)
        {
            if (path[endIndex] == -1 && startIndex != endIndex)
            {
                throw new Exception("There is no way");
            }

            var curCity = endIndex;

            while (path[curCity] != -1)
            {
                pathByCities.Add(routsGraph[curCity].Name);

                var coast = routsGraph[path[curCity]].Edges[curCity].Weight;

                pathByPrice.Add(coast);

                curCity = path[curCity];
            }

        }

        public void GetPath(string startCity, string endCity)
        {
            BellmanFordAlg(startCity, endCity);

            var currentCity = startCity;

            Console.Write("Path: ");

            for (var i = pathByCities.Count - 1; i >= 0; i--)
            {
                Console.Write($"{currentCity} --{pathByPrice[i]}--> {pathByCities[i]};  ");

                totalPricePath += pathByPrice[i];

                currentCity = pathByCities[i];
            }

            Console.WriteLine($"\n\nTotal price: {totalPricePath}");

        }

    }
}