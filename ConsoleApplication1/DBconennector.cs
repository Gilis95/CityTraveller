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

        public DBconennector()
        {
            client = new GraphClient(new Uri("http://neo4j:neo4j@localhost:7474/db/data"));
            client.Connect();
        }



        public void PrintCityList()
        {

            var query = client.Cypher.Match("(city:City)").Return((city) => new { a = city.As<City>() });

            foreach (var res in query.Results)
            {
                Console.WriteLine(res.a.name + " ");
            }

        }


        public void FindPathBetweenTwoTowns(string from, string to)
        {
            //var 1
            var query = client.Cypher
                       .Match("p=(a:City{name:{from}})-[*]->(b:City{name:{to}})")
                       .WithParam("from", from)
                       .WithParam("to", to)
                       .Return((p,order) => new
                       {
                           shortestPath = Return.As<City>("p"),
                           order= Return.As<int>("reduce(km = 0, r in relationships(p) | km + r.km)")


                       }).OrderBy("order ASC")
                       .Limit(1);

            var result = query.Results;

            foreach(var res in result) {
                Console.WriteLine(res.order);
            }

           /* 
            //var 2

            CypherQuery query1 = new CypherQuery("MATCH  p=(a:City{name:'" + from + "'})-[*]->(b:City{name:'" + to + "'}) RETURN p AS shortestPath, reduce(km = 0, r in relationships(p) | km + r.km) AS totalDistance ORDER BY totalDistance ASC LIMIT 1", new Dictionary<string, object>(), CypherResultMode.Set);
            var paths = ((IRawGraphClient)client).ExecuteGetCypherResults<List<string>>(query1);
            */    
        }




    }
}
