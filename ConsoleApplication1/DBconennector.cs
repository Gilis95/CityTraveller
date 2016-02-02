using Neo4jClient;
using Neo4jClient.Cypher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class DBconennector
    {
        private GraphClient client;
        private List<string> cities;

        public DBconennector()
        {
            client = new GraphClient(new Uri("http://neo4j:neo4j@localhost:7474/db/data"));
            client.Connect();

            cities = new List<string>();
        }



        public void PrintCityList()
        {

            var query = client.Cypher.Match("(city:City)").Return((city) => new { a = city.As<City>() });

            foreach (var res in query.Results)
            {
                Console.WriteLine(res.a.name + " ");
                cities.Add(res.a.name);
            }

            if (!cities.Any()) {
                Console.WriteLine("DataBase missing, it will take us few seconds to load it. Please wait!");
                this.loadNewDB();
            }

        }

        public void loadNewDB() {
            client.Cypher.Create(@"(Pleven:City{name:'Pleven'}) , (Mezdra:City{name:'Mezdra'}) , (Vraca:City{name:'Vraca'}) , 
                                        (Kneja: City{ name: 'Kneja'}), (Borovan:City{ name: 'Borovan'}) , (Lukovit:City{ name: 'Lukovit'}) ,
                                        (Botevgrad:City{ name: 'Botevgrad'}) , (PolskiTrambesh:City{ name: 'Polski Trambesh'}) , 
                                        (VelikoTarnovo:City{ name: 'Veliko Tarnovo'}) , (Lovech:City{ name: 'Lovech'}) , 
                                        (Pleven) -[:Distance{ km: 47}]->(Kneja) , (Kneja) -[:Distance{ km: 29}]->(Borovan) ,
                                        (Borovan) -[:Distance{ km: 32}]->(Vraca) , (Vraca) -[:Distance{ km: 17}]->(Mezdra) , 
                                        (Mezdra) -[:Distance{ km: 36}]->(Botevgrad) , (Botevgrad) -[:Distance{ km: 56}]->(Lukovit) ,
                                        (Lukovit) -[:Distance{ km: 51}]->(Pleven) ,(Pleven) -[:Distance{ km: 92}]->(PolskiTrambesh) ,
                                        (Pleven) -[:Distance{ km: 30}]->(Lovech) , (Lovech) -[:Distance{ km: 35}]->(VelikoTarnovo) , 
                                        (VelikoTarnovo) -[:Distance{ km: 38}]->(PolskiTrambesh) , (Kneja) -[:Distance{ km: 47}]->(Pleven) , 
                                        (Borovan) -[:Distance{ km: 29}]->(Kneja) , (Vraca) -[:Distance{ km: 32}]->(Borovan) , 
                                        (Mezdra) -[:Distance{ km: 17}]->(Vraca) , (Botevgrad) -[:Distance{ km: 36}]->(Mezdra) ,
                                        (Lukovit) -[:Distance{ km: 56}]->(Botevgrad) , (Pleven) -[:Distance{ km: 51}]->(Lukovit) ,
                                        (PolskiTrambesh) -[:Distance{ km: 92}]->(Pleven) ,(Lovech) -[:Distance{ km: 30}]->(Pleven) , 
                                        (VelikoTarnovo) -[:Distance{ km: 35}]->(Lovech) , (PolskiTrambesh) -[:Distance{ km: 38}]->(VelikoTarnovo)").ExecuteWithoutResults();


            this.PrintCityList();

                                                }


        public bool FindPathBetweenTwoTowns(string from, string to)
        {
            if (!this.CheckMarcherute(from,to)) {
                Console.WriteLine("No such city");
                return false;
            }
            /*MATCH  p=(a:City{name:"Pleven"})-[*]->(b:City{name:"Mezdra"})
            RETURN p AS shortestPath,
                   reduce(km = 0, r in relationships(p) | km + r.km) AS totalDistance
                   ORDER BY totalDistance ASC
                   LIMIT 1*/

            //var 1
            var query = client.Cypher
                       .Match("p=(a:City{name:{from}})-[*]->(b:City{name:{to}})")
                       .WithParam("from", from)
                       .WithParam("to", to)
                       .Return((p,order) => new
                       {
                           shortestPath = Return.As<IEnumerable<Node<City>>>("nodes(p)"),
                           order= Return.As<int>("reduce(km = 0, r in relationships(p) | km + r.km)")


                       }).OrderBy("order ASC")
                       .Limit(1);

            var result = query.Results;
            Console.WriteLine("Fastest Line:");
            foreach (var res in result) {
                foreach (var city in res.shortestPath.ToList()) {
                    Console.WriteLine(city.Data.name);
                };

            }

            return true;
            

           /* 
            //var 2

            CypherQuery query1 = new CypherQuery("MATCH  p=(a:City{name:'" + from + "'})-[*]->(b:City{name:'" + to + "'}) RETURN p AS shortestPath, reduce(km = 0, r in relationships(p) | km + r.km) AS totalDistance ORDER BY totalDistance ASC LIMIT 1", new Dictionary<string, object>(), CypherResultMode.Set);
            var paths = ((IRawGraphClient)client).ExecuteGetCypherResults<List<string>>(query1);
            */    
        }


        public bool FindEasiestPath(String from, String to) {
            //Match p=shortestPath((a:City{name:"Pleven"})-[*]->(b:City{name:"Mezdra"})) Return p

            if (!this.CheckMarcherute(from, to))
            {
                Console.WriteLine("No such city");
                return false;
            }

            

            var query = client.Cypher.Match("p=shortestPath((a:City{name:{from}})-[*]->(b:City{name:{to}}))")
                                   .WithParam("from", from)
                                   .WithParam("to", to)
                                   .Return(p => new
                                   {
                                       name = Return.As<IEnumerable<Node<City>>>("nodes(p)")
                                   });
            Console.WriteLine("Easiest Line:");
            foreach (var res in query.Results)
            {
                foreach (var city in res.name.ToList())
                {
                    Console.WriteLine(city.Data.name);
                };

            }

            return true;

        }


        public bool CheckMarcherute(string from,string to) {
            return (cities.Contains(from) ? ((cities.Contains(to))? true : false) : false);
        }

    }
}
