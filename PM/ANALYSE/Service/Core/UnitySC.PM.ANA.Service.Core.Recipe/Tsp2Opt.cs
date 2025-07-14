using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Core.Recipe
{
    public class MeasurePoints
    {
        public XYPosition Position;
        public List<MeasurePoint> MeasurePointsList;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]  // kind of force inlining for perf
        public double DistSqrTo(MeasurePoints target)
        {
            return (Position.X - target.Position.X) * (Position.X - target.Position.X) + (Position.Y - target.Position.Y) * (Position.Y - target.Position.Y);
        }
    }

    public static class Tsp2Opt
    {
        public static List<MeasurePoints> MakeShortestLoopPath(List<MeasurePoints> points)
        {
            var path = new List<MeasurePoints>(points);
            double curLength = PathLengthSqr(path);
            int n = path.Count;
            bool foundImprovement = true;
            while (foundImprovement)
            {
                foundImprovement = false;
                for (int i = 0; i < n - 1; i++)
                {
                    for (int j = i + 1; j < n; j++)
                    {
                        double lengthDelta =
                            -path[i].DistSqrTo(path[(i + 1) % n])
                            - path[j].DistSqrTo(path[(j + 1) % n])
                            + path[i].DistSqrTo(path[j])
                            + path[(i + 1) % n].DistSqrTo(path[(j + 1) % n]);
                        // If the length of the path is reduced, do a 2-opt swap
                        if (lengthDelta < -(1e-5))
                        {
                            do2Opt(ref path, i, j);
                            curLength += lengthDelta;
                            foundImprovement = true;
                        }
                    }
                }
            }

            return path;
        }

        private static double PathLengthSqr(List<MeasurePoints> path)
        {
            if (path == null || path.Count < 2)
                return 0.0;

            double length = 0.0;
            for (int i = 0; i < path.Count - 1; i++)
            {
                length += path[i].DistSqrTo(path[(i + 1)]);
            }
            length += path[path.Count - 1].DistSqrTo(path[0]);
            return length;
        }

        // Perform a 2-opt swap
        private static void do2Opt(ref List<MeasurePoints> path, int i, int j)
        {
            int count = j - i;

            //reverse(begin(path) + i + 1, begin(path) + j + 1);
            path.Reverse(i + 1, count);
        }
    }
}

/*
    public class testtsp
    {
        private int V = 6;

        List<int> final_ans = new List<int>();

        int minimum_key(int[] key, bool[] mstSet)
        {
            int min = int.MaxValue;
            int min_index = default;

            for (int v = 0; v < V; v++)
            {
                if (mstSet[v] == false && key[v] < min)
                {
                    min = key[v];
                    min_index = v;
                }
            }

            return min_index;
        }

        List<List<int>> MST(int[] parent, int[][] graph)
        {
            List<List<int>> v = new List<List<int>>();
            for (int i = 1; i < V; i++)
            {
                List<int> p = new List<int>();
                p.Add(parent[i]);
                p.Add(i);
                v.Add(p);
            }
            return v;
        }

        // getting the Minimum Spanning Tree from the given graph
        // using Prim's Algorithm
        List<List<int>> primMST(int[][] graph)
        {
            int[] parent = new int[V];
            int[] key = new int[V];

            // to keep track of vertices already in MST 
            bool[] mstSet = new bool[V];

            // initializing key value to INFINITE & false for all mstSet
            for (int i = 0; i < V; i++)
            {
                key[i] = int.MaxValue;
                mstSet[i] = false;
            }

            // picking up the first vertex and assigning it to 0
            key[0] = 0;
            parent[0] = -1;

            // The Loop
            for (int count = 0; count < V - 1; count++)
            {
                // checking and updating values wrt minimum key
                int i = minimum_key(key, mstSet);
                mstSet[i] = true;
                for (int j = 0; j < V; j++)
                {
                    if (graph[i][j] != 0 && mstSet[j] == false && graph[i][j] < key[j])
                    {
                        parent[j] = i;
                        key[j] = graph[i][j];
                    }
                }
            }
            List<List<int>> v;
            v = MST(parent, graph);
            return v;
        }

        // getting the preorder walk of the MST using DFS
        void DFS(int[][] edges_list, int num_nodes, int starting_vertex, bool[] visited_nodes)
        {
            // adding the node to final answer
            final_ans.Add(starting_vertex);

            // checking the visited status 
            visited_nodes[starting_vertex] = true;

            // using a recursive call
            for (int i = 0; i < num_nodes; i++)
            {
                if (i == starting_vertex)
                {
                    continue;
                }
                if (edges_list[starting_vertex][i] == 1)
                {
                    if (visited_nodes[i])
                    {
                        continue;
                    }
                    DFS(edges_list, num_nodes, i, visited_nodes);
                }
            }
        }

        public List<int> Main(int[][] graph)
        {
            V = graph.Length;

            // getting the output as MST 
            var v = primMST(graph);

            // creating a dynamic matrix
            int[][] edges_list = new int[V][];
            for (int i = 0; i < V; i++)
            {
                edges_list[i] = new int[V];
                for (int j = 0; j < V; j++)
                {
                    edges_list[i][j] = 0;
                }
            }

            // setting up MST as adjacency matrix
            for (int i = 0; i < v.Count; i++)
            {
                int first_node = v[i][0];
                int second_node = v[i][1];
                edges_list[first_node][second_node] = 1;
                edges_list[second_node][first_node] = 1;
            }

            // a checker function for the DFS
            bool[] visited_nodes = new bool[V];
            for (int i = 0; i < V; i++)
            {
                bool visited_node;
                visited_nodes[i] = false;
            }

            //performing DFS
            DFS(edges_list, V, 0, visited_nodes);

            // adding the source node to the path
            final_ans.Add(final_ans[0]);

            return final_ans;
        }
    }
*/
