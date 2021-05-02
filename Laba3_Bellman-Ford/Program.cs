
using System;

namespace Bellman_ford
{
    class Program
    {
        static void Main(string[] args)
        {
            BellmanFord bellmanFord = new BellmanFord(@"D:\Yaropolk.txt");

            bellmanFord.ShowAdjacencyMatrix();

            bellmanFord.GetPath("Владивосток", "Санкт-Петербург");
        }
    }
}