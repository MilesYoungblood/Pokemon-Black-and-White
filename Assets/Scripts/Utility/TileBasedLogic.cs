using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Utility.Math;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Scripts.Utility
{
    public static class TileBasedLogic
    {
        public static (Collider2D collider, int distance) TileCast(
            Vector2 position,
            Vector2Int direction,
            int tiles,
            int layerMask)
        {
            for (var i = 1; i <= tiles; ++i)
            {
                var collider = GetCollider2D(position, direction, i, layerMask);
                if (collider)
                {
                    return (collider, i);
                }
            }

            return default;
        }

        public static bool TileCast(
            Vector2 position,
            Vector2Int direction,
            int tiles,
            int layerMask,
            out (Collider2D collider, int distance) hit)
        {
            hit = TileCast(position, direction, tiles, layerMask);
            return hit.collider;
        }

        public static Collider2D GetCollider2D(Vector2 position, Vector2Int direction, int tiles, int layerMask)
        {
            direction.Clamp(-Vector2Int.one, Vector2Int.one);
            return direction != Vector2Int.zero
                ? Physics2D.OverlapPoint(position + direction * tiles, layerMask)
                : null;
        }

        public static bool GetCollider2D(
            Vector2 position,
            Vector2Int direction,
            int tiles,
            int layerMask,
            out Collider2D collision)
        {
            collision = GetCollider2D(position, direction, tiles, layerMask);
            return collision;
        }

        public static Collider2D GetAdjacentCollider2D(Vector2 position, Vector2Int direction, int layerMask)
        {
            return GetCollider2D(position, direction, 1, layerMask);
        }

        public static IEnumerator Move(this Rigidbody2D rb, Vector2Int direction, int tiles, float time)
        {
            var startPosition = rb.position;
            var targetPosition = startPosition + direction * tiles;

            for (var elapsedTime = 0.0f; elapsedTime < time; elapsedTime += Time.fixedDeltaTime)
            {
                rb.MovePosition(Vector2.Lerp(startPosition, targetPosition, elapsedTime / time));
                yield return new WaitForFixedUpdate();
            }
        }

        private static Vector3Int _staticPosition;

        public static Vector2Int[] BreadthFirstSearch(
            this Tilemap tilemap,
            Vector2Int startPosition,
            Vector2Int targetPosition)
        {
            // If the start and target positions are the same, return an empty path
            if (startPosition == targetPosition)
            {
                return Array.Empty<Vector2Int>();
            }

            // Queue for BFS: stores the current position and the path taken to get there
            var queue = new Queue<(Vector2Int, Vector2Int[])>();
            // Enqueue the starting position with an empty path
            queue.Enqueue((startPosition, new[] { startPosition }));

            var visited = new HashSet<Vector2Int> { startPosition };

            do
            {
                var (currentPosition, path) = queue.Dequeue();

                // Explore each possible direction
                foreach (var direction in EightWindCompassRose.Directions)
                {
                    var nextPosition = currentPosition + direction;

                    // Skip if this position is not a walkable floor tile or if the position as visited
                    if (!tilemap.HasTile((Vector3Int)nextPosition) || !visited.Add(nextPosition))
                    {
                        continue;
                    }

                    // Create a new path including this next position
                    var newPath = new List<Vector2Int>(path) { nextPosition }.ToArray();

                    // If we reached the target, return the path
                    if (nextPosition == targetPosition)
                    {
                        return newPath;
                    }

                    // Enqueue the next position with the updated path
                    queue.Enqueue((nextPosition, newPath));
                }
            } while (queue.Any());

            // If no path is found, return an empty list
            return Array.Empty<Vector2Int>();
        }

        public static Vector3Int[] AStarSearch(this Tilemap[] tilemaps, Vector3Int start, Vector3Int target)
        {
            var openSet = new PriorityQueue<Node>();

            var visited = new HashSet<Vector3Int>();

            var startNode = new Node(start, 0.0f, LinearAlgebra.ManhattanDistance(start, target), 0.0f);
            openSet.Enqueue(startNode, startNode.FScore);

            var allNodes = new Dictionary<Vector3Int, Node> { [start] = startNode };
            do
            {
                // Get the node in openSet with the lowest fScore
                var current = openSet.Dequeue();
                var currentPos = current.Position;

                if (currentPos == target)
                {
                    // Reconstruct the path and return it
                    return ReconstructPath(current);
                }

                visited.Add(currentPos);
                tilemaps.ExpandSearch(openSet, visited, current, allNodes, target);
            } while (openSet.Any());

            // No path found
            return Array.Empty<Vector3Int>();
        }

        public static Vector3Int[] BiDirectionalAStar(this Tilemap[] tilemaps, Vector3Int start, Vector3Int target)
        {
            var forwardOpenSet = new PriorityQueue<Node>();
            var backwardOpenSet = new PriorityQueue<Node>();

            var forwardVisited = new HashSet<Vector3Int>();
            var backwardVisited = new HashSet<Vector3Int>();

            var forwardStart = new Node(start, 0.0f, LinearAlgebra.ManhattanDistance(start, target), 0.0f);
            var backwardStart = new Node(target, 0.0f, LinearAlgebra.ManhattanDistance(target, start), 0.0f);

            forwardOpenSet.Enqueue(forwardStart, forwardStart.FScore);
            backwardOpenSet.Enqueue(backwardStart, backwardStart.FScore);

            var allNodes = new Dictionary<Vector3Int, Node>();

            do
            {
                // Expand forward search
                var forwardCurrent = forwardOpenSet.Dequeue();
                forwardVisited.Add(forwardCurrent.Position);

                if (backwardVisited.Contains(forwardCurrent.Position))
                {
                    return ReconstructBidirectionalPath(forwardCurrent, backwardVisited);
                }

                tilemaps.ExpandSearch(forwardOpenSet, forwardVisited, forwardCurrent, allNodes, target);

                // Expand backward search
                var backwardCurrent = backwardOpenSet.Dequeue();
                backwardVisited.Add(backwardCurrent.Position);

                if (forwardVisited.Contains(backwardCurrent.Position))
                {
                    return ReconstructBidirectionalPath(backwardCurrent, forwardVisited);
                }

                tilemaps.ExpandSearch(backwardOpenSet, backwardVisited, backwardCurrent, allNodes, start);
            } while (forwardOpenSet.Any() && backwardOpenSet.Any());

            return Array.Empty<Vector3Int>();
        }

        private static bool IsTileWalkable(this Tilemap[] tilemaps, Vector3Int position)
        {
            _staticPosition = position;
            return tilemaps.Any(tilemap => tilemap.HasTile(_staticPosition));
        }

        private static void ExpandSearch(
            this Tilemap[] tilemaps,
            PriorityQueue<Node> openSet,
            HashSet<Vector3Int> visited,
            Node currentNode,
            Dictionary<Vector3Int, Node> allNodes,
            Vector3Int target)
        {
            foreach (var direction in EightWindCompassRose.Directions)
            {
                var neighborPos = currentNode.Position + (Vector3Int)direction;

                if (!tilemaps.IsTileWalkable(neighborPos) || visited.Contains(neighborPos))
                {
                    continue;
                }

                var tentativeGScore = currentNode.GScore + LinearAlgebra.ManhattanDistance(currentNode.Position, neighborPos);
                if (allNodes.TryGetValue(neighborPos, out var neighborNode))
                {
                    if (tentativeGScore >= neighborNode.GScore)
                    {
                        continue;
                    }

                    // Found a better path!
                    neighborNode.GScore = tentativeGScore;
                    neighborNode.FScore = tentativeGScore + LinearAlgebra.ManhattanDistance(neighborPos, target);
                    neighborNode.Parent = currentNode;

                    openSet.UpdatePriority(neighborNode, neighborNode.FScore);
                }
                else
                {
                    neighborNode = new Node(
                        neighborPos,
                        tentativeGScore,
                        tentativeGScore + LinearAlgebra.ManhattanDistance(neighborPos, target),
                        0,
                        currentNode
                    );
                    allNodes[neighborPos] = neighborNode;
                    openSet.Enqueue(neighborNode, neighborNode.FScore);
                }
            }
        }

        private static Vector3Int[] ReconstructBidirectionalPath(Node meetingPoint, HashSet<Vector3Int> visited)
        {
            var path = new Stack<Vector3Int>();
            var current = meetingPoint;

            // Forward path reconstruction
            while (current != null)
            {
                path.Push(current.Position);
                current = current.Parent;
            }

            var backtrack = meetingPoint;
            while (backtrack != null)
            {
                path.Push(backtrack.Position);
                backtrack = backtrack.Parent;
            }

            return path.ToArray();
        }

        private static float HeuristicCostEstimate(Vector3Int from, Vector3Int to)
        {
            return (from - to).magnitude;
        }

        // Reconstructs the path from the start to the target by following cameFrom map
        private static Vector3Int[] ReconstructPath(Node target)
        {
            var path = new Stack<Vector3Int>();
            while (target != null)
            {
                path.Push(target.Position);
                target = target.Parent;
            }

            return path.ToArray(); // Convert stack to list (already in correct order)
        }
    }
}
