using System.Numerics;
using static WarehouseDemoBackend.Models.BotEnums;

namespace WarehouseDemoBackend.Models
{
    public class BoundingBoxHelpers
    {
        public BoundingBox StepBBFromDirection(BoundingBox BoxToMove, Direction direction, Vector2 currentSpeed)
        {
            Vector2 moveVector = new Vector2(0, 0);

            switch (direction)
            {
                case Direction.Up:
                    moveVector.Y = -(currentSpeed.Y);
                    break;
                case Direction.Down:
                    moveVector.Y = currentSpeed.Y;
                    break;
                case Direction.Left:
                    moveVector.X = -(currentSpeed.X);
                    break;
                case Direction.Right:
                    moveVector.X = currentSpeed.X;
                    break;
            }
            BoxToMove.UpdatePosition(moveVector);
            return BoxToMove;
        }

        public BoundingBox UnStepBBFromDirection(BoundingBox BoxToMove, Direction direction, Vector2 currentSpeed)
        {
            Vector2 moveVector = new Vector2(0, 0);

            switch (direction)
            {
                case Direction.Up:
                    moveVector.Y = currentSpeed.Y;
                    break;
                case Direction.Down:
                    moveVector.Y = -(currentSpeed.Y);
                    break;
                case Direction.Left:
                    moveVector.X = currentSpeed.X;
                    break;
                case Direction.Right:
                    moveVector.X = -(currentSpeed.X);
                    break;
            }
            BoxToMove.UpdatePosition(moveVector);
            return BoxToMove;
        }

        public class GJKImplementation
        {
            private const float EPA_TOLERANCE = 1e-4f;
            private const float MIN_PENETRATION_THRESHOLD = 1e-3f; // Treat touching as non-collision if depth is below this

            public static bool DetectCollision(IBoundingBox shapeA, IBoundingBox shapeB)
            {
                Vector2 direction = new Vector2(1, 0); // Arbitrary initial direction
                List<Vector2> simplex = new List<Vector2>();

                Vector2 support = Support(shapeA, shapeB, direction);
                simplex.Add(support);
                direction = -support;

                while (true)
                {
                    support = Support(shapeA, shapeB, direction);

                    if (Vector2.Dot(support, direction) <= 1e-6f)
                    {
                        return false; // No collision
                    }

                    simplex.Add(support);

                    if (DoSimplex(ref simplex, ref direction))
                    {
                        if (EPA(shapeA, shapeB, simplex, out float depth, out Vector2 normal))
                        {
                            return depth > MIN_PENETRATION_THRESHOLD;
                        }
                        return false; // EPA failed to converge
                    }
                }
            }

            private static Vector2 Support(IBoundingBox shapeA, IBoundingBox shapeB, Vector2 direction)
            {
                Vector2 pointA = GetFarthestPoint(shapeA, direction);
                Vector2 pointB = GetFarthestPoint(shapeB, -direction);
                return pointA - pointB;
            }

            private static Vector2 GetFarthestPoint(IBoundingBox shape, Vector2 direction)
            {
                Vector2 farthest = shape.Coords[0];
                float maxDot = Vector2.Dot(farthest, direction);

                foreach (var point in shape.Coords)
                {
                    float dot = Vector2.Dot(point, direction);
                    if (dot > maxDot)
                    {
                        maxDot = dot;
                        farthest = point;
                    }
                }

                return farthest;
            }

            private static bool DoSimplex(ref List<Vector2> simplex, ref Vector2 direction)
            {
                if (simplex.Count == 2)
                {
                    // Line segment: B (last), A (second last)
                    Vector2 B = simplex[0];
                    Vector2 A = simplex[1];
                    Vector2 AB = B - A;
                    Vector2 AO = -A;

                    direction = TripleProduct(AB, AO, AB);
                }
                else if (simplex.Count == 3)
                {
                    // Triangle: C (last), B, A
                    Vector2 C = simplex[0];
                    Vector2 B = simplex[1];
                    Vector2 A = simplex[2];

                    Vector2 AB = B - A;
                    Vector2 AC = C - A;
                    Vector2 AO = -A;

                    Vector2 ABPerp = TripleProduct(AC, AB, AB);
                    Vector2 ACPerp = TripleProduct(AB, AC, AC);

                    if (Vector2.Dot(ABPerp, AO) > 0)
                    {
                        // Remove point C
                        simplex.RemoveAt(0);
                        direction = ABPerp;
                    }
                    else if (Vector2.Dot(ACPerp, AO) > 0)
                    {
                        // Remove point B
                        simplex.RemoveAt(1);
                        direction = ACPerp;
                    }
                    else
                    {
                        return true; // Origin is inside triangle
                    }
                }

                return false;
            }

            private static Vector2 TripleProduct(Vector2 a, Vector2 b, Vector2 c)
            {
                float ac = Vector2.Dot(a, c);
                float bc = Vector2.Dot(b, c);
                return b * ac - a * bc;
            }
            private static bool EPA(IBoundingBox shapeA, IBoundingBox shapeB, List<Vector2> simplex, out float depth, out Vector2 normal)
            {
                depth = 0;
                normal = Vector2.Zero;

                const int maxIterations = 64;

                // Initialize edge list from triangle
                List<(Vector2 a, Vector2 b)> edges = new List<(Vector2, Vector2)>
        {
            (simplex[0], simplex[1]),
            (simplex[1], simplex[2]),
            (simplex[2], simplex[0])
        };

                for (int iteration = 0; iteration < maxIterations; iteration++)
                {
                    float minDistance = float.MaxValue;
                    int closestEdgeIndex = -1;
                    Vector2 closestNormal = Vector2.Zero;

                    // Find edge closest to origin
                    for (int i = 0; i < edges.Count; i++)
                    {
                        Vector2 a = edges[i].a;
                        Vector2 b = edges[i].b;
                        Vector2 edge = b - a;

                        // Outward normal
                        Vector2 perp = new Vector2(edge.Y, -edge.X);
                        perp = Vector2.Normalize(perp);

                        float distance = Vector2.Dot(perp, a);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestEdgeIndex = i;
                            closestNormal = perp;
                        }
                    }

                    // Support point in direction of closest normal
                    Vector2 support = Support(shapeA, shapeB, closestNormal);
                    float supportDistance = Vector2.Dot(support, closestNormal);
                    float diff = supportDistance - minDistance;

                    if (diff < EPA_TOLERANCE)
                    {
                        // Converged
                        depth = supportDistance;
                        normal = closestNormal;
                        return true;
                    }

                    // Insert new point into edge list
                    var closestEdge = edges[closestEdgeIndex];
                    edges.RemoveAt(closestEdgeIndex);
                    edges.Add((closestEdge.a, support));
                    edges.Add((support, closestEdge.b));
                }

                return false; // Failed to converge
            }
        }
    }
}
