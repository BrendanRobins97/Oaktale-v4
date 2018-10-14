using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph<T>
{
    private List<Node> nodes_ = new List<Node>();

    public class Node
    {
        private List<Edge> edges_ = new List<Edge>();
        private T context_;

        public T context
        {
            get
            {
                return context_;
            }
        }

        public List<Edge> edges
        {
            get
            {
                return edges_;
            }
        }

        public Node(T context)
        {
            context_ = context;
        }
    }

    public class Edge
    {
        Node to_;
        int weight_;

        public Node to
        {
            get
            {
                return to_;
            }
        }
        public int weight
        {
            get
            {
                return weight_;
            }
        }

        public Edge(Node to, int weight)
        {
            to_ = to;
            weight_ = weight;
        }
    }

    public int n
    {
        get
        {
            return nodes_.Count;
        }
    }

    public List<Node> nodes
    {
        get
        {
            return nodes_;
        }
    }

    public Graph(IEnumerable<T> initialize_list)
    {
        foreach (var i in initialize_list)
        {
            AddVertex(i);
        }
    }

    public Graph(Graph<T> othr)
    {
        CopyFrom(othr);
    }

    public Node AddVertex(T context)
    {
        Node a = new Node(context);
        nodes_.Add(a);
        return a;
    }

    public Node FindVertex(T context)
    {
        return nodes_.Find(n => n.context.Equals(context));
    }

    public void CopyFrom(Graph<T> othr)
    {
        nodes_.Clear();
        nodes_.AddRange(othr.nodes_);
    }

    public bool Contains(Node node)
    {
        return nodes_.Find(n => n.Equals(node)) != null;
    }

    public void Remove(Node node)
    {
        nodes_.Remove(node);
    }
}

public static class Controller
{
    public static void SetEdge<T>(this Graph<T> self, T a, T b, int weight)
    {
        var a_node = self.FindVertex(a);
        var b_node = self.FindVertex(b);
        a_node.AddEdge(b_node, weight);
        b_node.AddEdge(a_node, weight);
    }

    public static void AddEdge<T>(this Graph<T>.Node self, Graph<T>.Node to, int weight)
    {
        self.edges.Add(new Graph<T>.Edge(to, weight));
    }

    public static List<Graph<T>.Node> GetNeighbors<T>(this Graph<T>.Node self)
    {
        List<Graph<T>.Node> nodes = new List<Graph<T>.Node>();
        foreach (Graph<T>.Edge edge in self.edges)
        {
            if (!nodes.Contains(edge.to))
            {
                nodes.Add(edge.to);

            }
        }
        return nodes;
    }

    public static int GetWeight<T>(this Graph<T>.Node self, Graph<T>.Node b)
    {
        foreach (var e in self.edges)
        {
            if (e.to == b)
            {
                return e.weight;
            }
        }
        return int.MaxValue - 1;
    }
}
class Prim<T>
{
    readonly int kInfinite = int.MaxValue - 1;

    public void prim(Graph<T> g, Graph<T>.Node r, ref Dictionary<Graph<T>.Node, Graph<T>.Node> tree)
    {
        Graph<T> Q = new Graph<T>(g);
        var d = new Dictionary<Graph<T>.Node, int>();
        foreach (var u in Q.nodes)
        {
            d[u] = kInfinite;
        }
        d[r] = 0;

        while (Q.n > 0)
        {
            var u = deleteMin(Q, d);
            foreach (var v in u.GetNeighbors())
            {
                if (Q.Contains(v) && (u.GetWeight(v) < d[v]))
                {
                    d[v] = u.GetWeight(v);
                    tree[v] = u;
                }
            }
        }
    }

    private Graph<T>.Node deleteMin(Graph<T> Q, Dictionary<Graph<T>.Node, int> d)
    {
        Graph<T>.Node min_node = null;
        int min_weight = kInfinite;
        foreach (var n in Q.nodes)
        {
            if (d[n] <= min_weight)
            {
                min_weight = d[n];
                min_node = n;
            }
        }
        Q.Remove(min_node);
        return min_node;
    }

}
