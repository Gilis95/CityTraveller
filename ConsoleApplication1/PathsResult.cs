using System.Collections.Generic;
using Neo4jClient;

namespace ConsoleApplication1
{
    internal class PathsResult<T>
    {
        public IEnumerable<Node<T>> nodes { get; set; }
    }
}