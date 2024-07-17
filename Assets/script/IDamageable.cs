using System;
   using System.Collections.Generic;
   using UnityEngine;

   public class MapGenerator : MonoBehaviour
   {
       public int numFloors = 15;
       public int minPathsPerFloor = 3;
       public int maxPathsPerFloor = 4;

       private List<List<Node>> map;

       void Start()
       {
           GenerateMap();
       }

       void GenerateMap()
       {
           map = new List<List<Node>>();

           // Create start node
           List<Node> firstFloor = new List<Node> { new Node(NodeType.Start, new Vector2(0, 0)) };
           map.Add(firstFloor);

           // Generate middle floors
           for (int floor = 1; floor < numFloors - 1; floor++)
           {
               List<Node> currentFloor = new List<Node>();
               int numPaths = UnityEngine.Random.Range(minPathsPerFloor, maxPathsPerFloor + 1);

               for (int path = 0; path < numPaths; path++)
               {
                   NodeType type = GetRandomNodeType();
                   Node node = new Node(type, new Vector2(path, floor));
                   currentFloor.Add(node);
               }

               ConnectNodes(map[floor - 1], currentFloor);
               map.Add(currentFloor);
           }

           // Create boss node
           List<Node> lastFloor = new List<Node> { new Node(NodeType.Boss, new Vector2(0, numFloors - 1)) };
           ConnectNodes(map[numFloors - 2], lastFloor);
           map.Add(lastFloor);
       }

       void ConnectNodes(List<Node> previousFloor, List<Node> currentFloor)
       {
           foreach (Node currentNode in currentFloor)
           {
               int connections = UnityEngine.Random.Range(1, 3); // 1 or 2 connections
               List<Node> possibleConnections = new List<Node>(previousFloor);

               for (int i = 0; i < connections && possibleConnections.Count > 0; i++)
               {
                   int index = UnityEngine.Random.Range(0, possibleConnections.Count);
                   Node previousNode = possibleConnections[index];

                   currentNode.AddConnection(previousNode);
                   previousNode.AddConnection(currentNode);

                   possibleConnections.RemoveAt(index);
               }
           }
       }

       NodeType GetRandomNodeType()
       {
           // Adjust these probabilities as needed
           float random = UnityEngine.Random.value;
           if (random < 0.6f) return NodeType.Enemy;
           if (random < 0.8f) return NodeType.Event;
           if (random < 0.9f) return NodeType.RestSite;
           return NodeType.Merchant;
       }
   }

   public enum NodeType
   {
       Start,
       Enemy,
       Elite,
       RestSite,
       Merchant,
       Event,
       Boss
   }

   public class Node
   {
       public NodeType Type { get; private set; }
       public Vector2 Position { get; private set; }
       public List<Node> Connections { get; private set; }

       public Node(NodeType type, Vector2 position)
       {
           Type = type;
           Position = position;
           Connections = new List<Node>();
       }

       public void AddConnection(Node node)
       {
           if (!Connections.Contains(node))
           {
               Connections.Add(node);
           }
       }
   }
