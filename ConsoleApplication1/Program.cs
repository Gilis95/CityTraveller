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

            int res=0;

            Console.WriteLine("Choose marcherute: ");

            db.PrintCityList();
            do
            {
                Console.Write("Choose the town where you beggin: ");
                from = Console.ReadLine();
                Console.Write("Choose the town where you are going to: ");
                to = Console.ReadLine();

                string LineReaded;
                
                do
                {
                    bool isDigit = true;
                    do
                    {
                        Console.WriteLine("Choose Between properties(1,2 or 3): \n 1.Fastest way \n 2.Easiest way \n 3.New Marcherute");

                        LineReaded = Console.ReadLine();
                        foreach (char c in LineReaded.ToCharArray())
                        {
                            if (!Char.IsDigit(c))
                            {
                                isDigit = false;
                                break;
                            }
                        }
                        if (isDigit == true)
                        {
                            res = Convert.ToInt32(LineReaded);                            
                            if (res != 1 && res != 2 && res != 3)
                                isDigit = false;
                        }
                    } while (!(isDigit == true));


                    if (res == 1)
                    {
                        
                        if(db.FindPathBetweenTwoTowns(from, to)==false)
                            break;
                    }
                    else if (res == 2)
                    {
                        if(db.FindEasiestPath(from, to)== false)
                            break;
                    }

                }
                while (res != 3);

            } while (true);

        }
    }
}
