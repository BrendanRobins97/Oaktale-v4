using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kruskal
{

    public struct KruskalEdge
    {
        public int Source;
        public int Destination;
        public int Weight;

    }

    public struct Graph
    {
        public int VerticesCount;
        public int EdgesCount;
        public KruskalEdge[] edge;
    }

    public struct Subset
    {
        public int Parent;
        public int Rank;
    }

    public static Graph CreateGraph(int verticesCount, int edgesCoun)
    {
        Graph graph = new Graph();
        graph.VerticesCount = verticesCount;
        graph.EdgesCount = edgesCoun;
        graph.edge = new KruskalEdge[graph.EdgesCount];

        return graph;
    }

    private static int Find(Subset[] subsets, int i)
    {
        if (subsets[i].Parent != i)
            subsets[i].Parent = Find(subsets, subsets[i].Parent);

        return subsets[i].Parent;
    }

    private static void Union(Subset[] subsets, int x, int y)
    {
        int xroot = Find(subsets, x);
        int yroot = Find(subsets, y);

        if (subsets[xroot].Rank < subsets[yroot].Rank)
            subsets[xroot].Parent = yroot;
        else if (subsets[xroot].Rank > subsets[yroot].Rank)
            subsets[yroot].Parent = xroot;
        else
        {
            subsets[yroot].Parent = xroot;
            ++subsets[xroot].Rank;
        }
    }


    public static KruskalEdge[] KruskalAlg(Graph graph)
    {
        int verticesCount = graph.VerticesCount;
        Debug.Log("Vertices Count: " + verticesCount);

        KruskalEdge[] result = new KruskalEdge[verticesCount];
        int i = 0;
        int e = 0;

        Array.Sort(graph.edge, delegate (KruskalEdge a, KruskalEdge b)
        {
            return a.Weight.CompareTo(b.Weight);
        });

        Subset[] subsets = new Subset[verticesCount];

        for (int v = 0; v < verticesCount; ++v)
        {
            subsets[v].Parent = v;
            subsets[v].Rank = 0;
        }

        while (e < verticesCount - 1)
        {
            Debug.Log("e : " + e + "i :" + i);
            KruskalEdge nextEdge = graph.edge[i++];
            int x = Find(subsets, nextEdge.Source);
            int y = Find(subsets, nextEdge.Destination);

            if (x != y)
            {
                result[e++] = nextEdge;
                Union(subsets, x, y);
            }
        }

        return result;
    }
}
