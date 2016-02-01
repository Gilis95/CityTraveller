using Neo4jClient;
using Neo4jClient.Cypher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {

        private static DBconennector db;

        static void Main(string[] args)
        {

            db = new DBconennector();

            string from;
            string to;

            Console.WriteLine("Choose marcherute: ");

            db.PrintCityList();

            Console.Write("Choose the town where you beggin: ");
            from = Console.ReadLine();
            Console.Write("Choose the town where you are going to: ");
            to = Console.ReadLine();

            db.FindPathBetweenTwoTowns(from, to);

            Console.Read();
        }
    }
}
