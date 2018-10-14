using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Voronoi_Delaunay
{
    class Delaunay
    {
        public static List<DelaunayEdge> DelaunayEdges(Triangle superTriangle, List<Triangle> delaunayTriangles)
        {
            List<DelaunayEdge> delaunayEdges = new List<DelaunayEdge>();

            for (int i = 0; i < delaunayTriangles.Count; i++)
            {
                DelaunayEdge edge1 = new DelaunayEdge(delaunayTriangles[i].vertex1, delaunayTriangles[i].vertex2);
                DelaunayEdge edge2 = new DelaunayEdge(delaunayTriangles[i].vertex2, delaunayTriangles[i].vertex3);
                DelaunayEdge edge3 = new DelaunayEdge(delaunayTriangles[i].vertex3, delaunayTriangles[i].vertex1);
                if (!edge1.ContainsVertex(superTriangle.vertex1) && !edge1.ContainsVertex(superTriangle.vertex2) && !edge1.ContainsVertex(superTriangle.vertex3))
                    delaunayEdges.Add(edge1);
                if (!edge2.ContainsVertex(superTriangle.vertex1) && !edge2.ContainsVertex(superTriangle.vertex2) && !edge2.ContainsVertex(superTriangle.vertex3))
                    delaunayEdges.Add(edge2);
                if (!edge3.ContainsVertex(superTriangle.vertex1) && !edge3.ContainsVertex(superTriangle.vertex2) && !edge3.ContainsVertex(superTriangle.vertex3))
                    delaunayEdges.Add(edge3);
            }

            return delaunayEdges;
        }

        public static List<Triangle> Triangulate(Triangle superTriangle, List<Room> triangulationPoints)
        {
            List<Triangle> delaunayTriangles = new List<Triangle>();

            Collections.allTriangles = new List<Triangle>();

            List<int> open = new List<int>();
            List<int> closed = new List<int>();

            Collections.allTriangles.Add(superTriangle); ;
            open.Add(0);

            for (int i = 0; i < triangulationPoints.Count - 3; i++)
            {

                List<DelaunayEdge> polygon = new List<DelaunayEdge>();

                for (int j = open.Count - 1; j >= 0; j--)
                {
                    double dx = triangulationPoints.ElementAt(i).position.x - Collections.allTriangles[open[j]].center.x;

                    if (dx > 0.0 && dx * dx > Collections.allTriangles[open[j]].radius * Collections.allTriangles[open[j]].radius)
                    {
                        closed.Add(open[j]);
                        open.RemoveAt(j);
                        continue;
                    }

                    if (Collections.allTriangles[open[j]].ContainsInCircumcircle(triangulationPoints.ElementAt(i)))
                    {
                        polygon.Add(new DelaunayEdge(Collections.allTriangles[open[j]].vertex1, Collections.allTriangles[open[j]].vertex2));
                        polygon.Add(new DelaunayEdge(Collections.allTriangles[open[j]].vertex2, Collections.allTriangles[open[j]].vertex3));
                        polygon.Add(new DelaunayEdge(Collections.allTriangles[open[j]].vertex3, Collections.allTriangles[open[j]].vertex1));

                        triangulationPoints[Collections.allTriangles[open[j]].vertex1].adjoinTriangles.Remove(open[j]);
                        triangulationPoints[Collections.allTriangles[open[j]].vertex2].adjoinTriangles.Remove(open[j]);
                        triangulationPoints[Collections.allTriangles[open[j]].vertex3].adjoinTriangles.Remove(open[j]);

                        open.RemoveAt(j);

                    }
                }

                for (int j = polygon.Count - 2; j >= 0; j--)
                {
                    for (int k = polygon.Count - 1; k >= j + 1; k--)
                    {
                        if (polygon[j] == polygon[k])
                        {
                            polygon.RemoveAt(k);
                            polygon.RemoveAt(j);
                            k--;
                            continue;
                        }
                    }
                }

                for (int j = 0; j < polygon.Count; j++)
                {
                    Triangle triangle = new Triangle(polygon[j].start, polygon[j].end, i);

                    int currentTriangle = Collections.allTriangles.Count;

                    Collections.allTriangles.Add(triangle);
                    open.Add(currentTriangle);

                    if (!triangulationPoints[i].adjoinTriangles.Contains(currentTriangle))
                    {
                        triangulationPoints[i].adjoinTriangles.Add(currentTriangle);
                    }
                    if (!triangulationPoints[polygon[j].start].adjoinTriangles.Contains(currentTriangle))
                    {
                        triangulationPoints[polygon[j].start].adjoinTriangles.Add(currentTriangle);
                    }
                    if (!triangulationPoints[polygon[j].end].adjoinTriangles.Contains(currentTriangle))
                    {
                        triangulationPoints[polygon[j].end].adjoinTriangles.Add(currentTriangle);
                    }

                }
            }
            /*
            for (int i = 0; i < Subjects.allRooms.Count - 3; i++)
            {
                for (int j = 0; j < Subjects.allRooms[i].adjoinTriangles.Count; j++)
                {
                    if (!(open.Contains(Subjects.allRooms[i].adjoinTriangles[j]) || closed.Contains(Subjects.allRooms[i].adjoinTriangles[j])))
                    {
                        Subjects.allRooms[i].adjoinTriangles.RemoveAt(j);
                    }
                }
            }
            */

            for (int i = 0; i < open.Count; i++)
            {
                delaunayTriangles.Add(Collections.allTriangles[open[i]]);
            }

            for (int i = 0; i < closed.Count; i++)
            {
                delaunayTriangles.Add(Collections.allTriangles[closed[i]]);
            }

            return delaunayTriangles;
        }

        public static Triangle SuperTriangle(List<Room> triangulationPoints)
        {
            float M = triangulationPoints[0].position.x;

            for (int i = 1; i < triangulationPoints.Count; i++)
            {
                float xAbs = Math.Abs(triangulationPoints[i].position.x);
                float yAbs = Math.Abs(triangulationPoints[i].position.y);
                if (xAbs > M) M = xAbs;
                if (yAbs > M) M = yAbs;
            }

            Room sp1 = new Room(new Vector2(100f * M, 0), Int2.one);
            triangulationPoints.Add(sp1);

            Room sp2 = new Room(new Vector2(0, 100 * M), Int2.one);

            triangulationPoints.Add(sp2);

            Room sp3 = new Room(new Vector2(-100 * M, -100 * M), Int2.one);

            triangulationPoints.Add(sp3);

            return new Triangle(triangulationPoints.Count - 3, triangulationPoints.Count - 2, triangulationPoints.Count - 1);
        }
    }
}
