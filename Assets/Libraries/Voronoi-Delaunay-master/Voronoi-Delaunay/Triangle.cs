using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Voronoi_Delaunay
{
    class Triangle
    {
        public int vertex1, vertex2, vertex3;
        public Vector2 center;
        public double radius;

        public Triangle()
        {
            this.vertex1 = -1;
            this.vertex2 = -1;
            this.vertex3 = -1;
        }

        public Triangle(int vertex1, int vertex2, int vertex3)
        {
            float x1, x2, x3, y1, y2, y3;
            float x, y;

            this.vertex1 = vertex1;
            this.vertex2 = vertex2;
            this.vertex3 = vertex3;

            x1 = Collections.allRooms[vertex1].position.x;
            x2 = Collections.allRooms[vertex2].position.x;
            x3 = Collections.allRooms[vertex3].position.x;
            y1 = Collections.allRooms[vertex1].position.y;
            y2 = Collections.allRooms[vertex2].position.y;
            y3 = Collections.allRooms[vertex3].position.y;

            x = ((y2 - y1) * (y3 * y3 - y1 * y1 + x3 * x3 - x1 * x1) - (y3 - y1) * (y2 * y2 - y1 * y1 + x2 * x2 - x1 * x1)) / (2 * (x3 - x1) * (y2 - y1) - 2 * ((x2 - x1) * (y3 - y1)));
            y = ((x2 - x1) * (x3 * x3 - x1 * x1 + y3 * y3 - y1 * y1) - (x3 - x1) * (x2 * x2 - x1 * x1 + y2 * y2 - y1 * y1)) / (2 * (y3 - y1) * (x2 - x1) - 2 * ((y2 - y1) * (x3 - x1)));

            this.center = new Vector2(x, y);
            this.radius = Math.Sqrt(Math.Abs(Collections.allRooms[vertex1].position.x - x) * Math.Abs(Collections.allRooms[vertex1].position.x - x) + Math.Abs(Collections.allRooms[vertex1].position.y - y) * Math.Abs(Collections.allRooms[vertex1].position.y - y));
        }

        public bool ContainsInCircumcircle(Room room)
        {
            double d_squared = (room.position.x - this.center.x) * (room.position.x - this.center.x) + (room.position.y - this.center.y) * (room.position.y - this.center.y);
            double radius_squared = this.radius * this.radius;

            return d_squared < radius_squared;
        }

        public bool SharesVertexWith(Triangle triangle)
        {
            if (this.vertex1 == triangle.vertex1) return true;
            if (this.vertex1 == triangle.vertex2) return true;
            if (this.vertex1 == triangle.vertex3) return true;

            if (this.vertex2 == triangle.vertex1) return true;
            if (this.vertex2 == triangle.vertex2) return true;
            if (this.vertex2 == triangle.vertex3) return true;

            if (this.vertex3 == triangle.vertex1) return true;
            if (this.vertex3 == triangle.vertex2) return true;
            if (this.vertex3 == triangle.vertex3) return true;

            return false;
        }

        public DelaunayEdge FindCommonEdgeWith(Triangle triangle)
        {
            DelaunayEdge commonEdge;
            List<int> commonVertices = new List<int>();

            if (this.vertex1 == triangle.vertex1 || this.vertex1 == triangle.vertex2 || this.vertex1 == triangle.vertex3) commonVertices.Add(this.vertex1);
            if (this.vertex2 == triangle.vertex1 || this.vertex2 == triangle.vertex2 || this.vertex2 == triangle.vertex3) commonVertices.Add(this.vertex2);
            if (this.vertex3 == triangle.vertex1 || this.vertex3 == triangle.vertex2 || this.vertex3 == triangle.vertex3) commonVertices.Add(this.vertex3);

            if (commonVertices.Count == 2)
            {
                commonEdge = new DelaunayEdge(commonVertices[0], commonVertices[1]);
                return commonEdge;
            }

            return null;
        }

        public override bool Equals(object obj)
        {
            return this == (Triangle)obj;
        }

        public static bool operator ==(Triangle a, Triangle b)
        {
            if (((object)a) == ((object)b))
            {
                return true;
            }

            if ((((object)a) == null) || (((object)b) == null))
            {
                return false;
            }

            return ((a.vertex1 == b.vertex1 && a.vertex2 == b.vertex2 && a.vertex3 == b.vertex3) ||
                     (a.vertex1 == b.vertex1 && a.vertex2 == b.vertex3 && a.vertex3 == b.vertex2) ||
                     (a.vertex1 == b.vertex2 && a.vertex2 == b.vertex1 && a.vertex3 == b.vertex3) ||
                     (a.vertex1 == b.vertex2 && a.vertex2 == b.vertex3 && a.vertex3 == b.vertex1) ||
                     (a.vertex1 == b.vertex3 && a.vertex2 == b.vertex1 && a.vertex3 == b.vertex2) ||
                     (a.vertex1 == b.vertex3 && a.vertex2 == b.vertex2 && a.vertex3 == b.vertex1));
        }

        public static bool operator !=(Triangle a, Triangle b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return this.vertex1.GetHashCode() ^ this.vertex2.GetHashCode() ^ this.vertex3.GetHashCode();
        }
    }
}
