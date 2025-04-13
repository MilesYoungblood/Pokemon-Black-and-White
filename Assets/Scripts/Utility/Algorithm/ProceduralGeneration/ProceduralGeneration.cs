using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Utility.Algorithm
{
    public static class ProceduralGeneration
    {
        public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
        {
            var path = new HashSet<Vector2Int> { startPosition };
            var previousPosition = startPosition;

            for (var i = 0; i < walkLength; ++i)
            {
                var newPosition = previousPosition + Compass.GetRandomDirection();
                path.Add(newPosition);
                previousPosition = newPosition;
            }

            return path;
        }

        public static Vector2Int[] RandomWalkCorridor(Vector2Int startPosition, int corridorLength)
        {
            var corridor = new Vector2Int[corridorLength];
            corridor[0] = startPosition;
            var direction = Compass.GetRandomDirection();

            var currentPosition = startPosition;
            for (var i = 1; i < corridorLength; ++i) // Changed index to start at 1 to avoid overriding start position
            {
                currentPosition += direction;
                corridor[i] = currentPosition;
            }

            return corridor;
        }

        public static HashSet<Vector2Int> RandomMomentumWalk(Vector2Int startPosition, int walkLength,
            float momentumProbability = 0.75f)
        {
            var path = new HashSet<Vector2Int> { startPosition };
            var previousDirection = Compass.GetRandomDirection();
            var currentPosition = startPosition;

            for (var i = 1; i < walkLength; ++i)
            {
                // Decide whether to continue in the same direction based on momentum probability
                var newDirection = Random.value < momentumProbability
                    ? previousDirection
                    : Compass.GetRandomDirection();

                // Move to the new position
                currentPosition += newDirection;
                path.Add(currentPosition);

                // Update the previous direction
                previousDirection = newDirection;
            }

            return path;
        }

        public static HashSet<BoundsInt> BinarySpacePartitioning(Vector2Int area, Vector2Int minRoomSize, int numRooms)
        {
            while (true)
            {
                var roomsList = new HashSet<BoundsInt>();

                var roomQueue = new Queue<BoundsInt>();
                roomQueue.Enqueue(new BoundsInt(Vector3Int.zero, (Vector3Int)area));

                while (roomsList.Count < numRooms && roomQueue.Count > 0)
                {
                    var room = roomQueue.Dequeue();

                    if (room.size.x >= minRoomSize.x * 2 || room.size.y >= minRoomSize.y * 2)
                    {
                        if (room.size.x >= minRoomSize.x * 2 && room.size.y >= minRoomSize.y * 2)
                        {
                            // Randomly choose to split horizontally or vertically
                            if (Math.Statistics.BernoulliTrial())
                            {
                                SplitHorizontally(roomQueue, room, minRoomSize);
                            }
                            else
                            {
                                SplitVertically(roomQueue, room, minRoomSize);
                            }
                        }
                        else if (room.size.x >= minRoomSize.x * 2)
                        {
                            SplitVertically(roomQueue, room, minRoomSize);
                        }
                        else
                        {
                            SplitHorizontally(roomQueue, room, minRoomSize);
                        }
                    }
                    else if (room.size.x >= minRoomSize.x && room.size.y >= minRoomSize.y)
                    {
                        roomsList.Add(room);
                    }
                }

                // **Ensure the exact number of rooms**
                while (roomsList.Count < numRooms && roomQueue.Count > 0)
                {
                    var room = roomQueue.Dequeue();
                    if (room.size.x >= minRoomSize.x && room.size.y >= minRoomSize.y)
                    {
                        roomsList.Add(room);
                    }
                }

                // Fallback: If we still don't have enough rooms, force adds remaining partitions
                while (roomsList.Count < numRooms && roomQueue.Count > 0)
                {
                    roomsList.Add(roomQueue.Dequeue());
                }

                if (roomsList.Count < numRooms)
                {
                    continue;
                }

                return roomsList;
            }
        }

        private static void SplitHorizontally(Queue<BoundsInt> roomQueue, BoundsInt room, Vector2Int minRoomSize)
        {
            var minSplit = minRoomSize.y;
            var maxSplit = room.size.y - minRoomSize.y;

            if (maxSplit <= minSplit)
            {
                return; // Prevent invalid splits
            }

            var ySplit = Random.Range(minSplit, maxSplit);
            var room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
            var room2 = new BoundsInt(
                new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
                new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z)
            );

            roomQueue.Enqueue(room1);
            roomQueue.Enqueue(room2);
        }

        private static void SplitVertically(Queue<BoundsInt> roomQueue, BoundsInt room, Vector2Int minRoomSize)
        {
            var minSplit = minRoomSize.x;
            var maxSplit = room.size.x - minRoomSize.x;

            if (maxSplit <= minSplit)
            {
                return; // Prevent invalid splits
            }

            var xSplit = Random.Range(minSplit, maxSplit);
            var room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
            var room2 = new BoundsInt(
                new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
                new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z)
            );

            roomQueue.Enqueue(room1);
            roomQueue.Enqueue(room2);
        }
    }
}
