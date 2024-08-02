using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    public int numFloors = 15;
    public int minPathsPerFloor = 3;
    public int maxPathsPerFloor = 4;

    [SerializeField] private float floorHeight = 100f;  // New variable to control height between floors
    [SerializeField] private float nodeSpacing = 100f;  // Horizontal spacing between nodes

    [SerializeField] private GameObject startNodePrefab;
    [SerializeField] private GameObject enemyNodePrefab;
    [SerializeField] private GameObject restSiteNodePrefab;
    [SerializeField] private GameObject eventNodePrefab;
    [SerializeField] private GameObject bossNodePrefab;

    [SerializeField] private RectTransform playerIcon;
    [SerializeField] private Transform mapContainer;
    [SerializeField] private Canvas mapCanvas;

    [SerializeField] private GameObject lineContainer;
    [SerializeField] private GameObject nodeContainer;


    [SerializeField] private Color lineColor = Color.white;
    [SerializeField] private float lineWidth = 2f;


    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float scrollPadding = 50f;
    private Node currentNode;

    

    private List<List<Node>> map;

    void Start()
    {
        SetupScrolling();
        GenerateMap();
        VisualizeMap();
        SetupContainers();
        PlacePlayerAtStart();
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

            ConnectNodes(map[floor - 1], currentFloor, false);
            map.Add(currentFloor);
        }

        // Create boss node
        List<Node> lastFloor = new List<Node> { new Node(NodeType.Boss, new Vector2(0, numFloors - 1)) };
        ConnectNodes(map[numFloors - 2], lastFloor, true);
        map.Add(lastFloor);

        RemoveIsolatedNodes();
    }


    void ConnectNodes(List<Node> previousFloor, List<Node> currentFloor, bool isBossFloor = false)
    {
        foreach (Node currentNode in currentFloor)
        {
            if (isBossFloor || previousFloor == map[0])
            {
                // Connect to all nodes of the previous floor if it's the max or first floor
                foreach (Node previousNode in previousFloor)
                {
                    currentNode.AddConnection(previousNode);
                    previousNode.AddConnection(currentNode);
                }
            }
            else
            {


                int connections = UnityEngine.Random.Range(1, 3); // 1 or 2 connections
                int currentNodeIndex = currentFloor.IndexOf(currentNode); // Get index of current node

                // Create a list of possible connections based on adjacency
                //List<Node> possibleConnections = new List<Node>(previousFloor);
                
                List<Node> possibleConnections = new List<Node>();
                if (currentNodeIndex > 0 && currentNodeIndex - 1 < previousFloor.Count)
                {
                    possibleConnections.Add(previousFloor[currentNodeIndex - 1]);
                }
                if (currentNodeIndex < previousFloor.Count)
                {
                    possibleConnections.Add(previousFloor[currentNodeIndex]);
                }
                if (currentNodeIndex + 1 < previousFloor.Count)
                {
                    possibleConnections.Add(previousFloor[currentNodeIndex + 1]);
                }
                
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
    }


    NodeType GetRandomNodeType()
    {
        float random = UnityEngine.Random.value;
        if (random < 0.6f) return NodeType.Enemy;
        if (random < 0.8f) return NodeType.Event;
        return NodeType.RestSite;
    }

    void SetupContainers()
    {
        // Create separate containers for lines and nodes if they don't exist
        if (lineContainer == null)
        {
            lineContainer = new GameObject("LineContainer");
            lineContainer.transform.SetParent(mapContainer, false);
            lineContainer.AddComponent<RectTransform>();
        }

        if (nodeContainer == null)
        {
            nodeContainer = new GameObject("NodeContainer");
            nodeContainer.transform.SetParent(mapContainer, false);
            nodeContainer.AddComponent<RectTransform>();
        }

        
        lineContainer.transform.SetSiblingIndex(1);
        nodeContainer.transform.SetSiblingIndex(2);
        lineContainer.transform.SetParent(scrollRect.content, false);
        nodeContainer.transform.SetParent(scrollRect.content, false);
    }

    void RemoveIsolatedNodes()
    {
        bool removedNodes;
        do
        {
            removedNodes = false;
            foreach (var floor in map)
            {
                if (floor == map[0]) continue;

                List<Node> nodesToRemove = new List<Node>();

                foreach (var node in floor)
                {
                    if (node.Connections.Count <= 1)
                    {
                        nodesToRemove.Add(node);
                    }
                }

                // Remove the isolated nodes and update connections
                foreach (var nodeToRemove in nodesToRemove)
                {
                    foreach (var connectedNode in nodeToRemove.Connections)
                    {
                        connectedNode.Connections.Remove(nodeToRemove);
                    }
                    floor.Remove(nodeToRemove);

                    removedNodes = true;
                }
            }
        } while (removedNodes); // Continue removing nodes as long as any nodes were removed



        // remove all nodes that does not have a continuation to the next floor
        for(int floorIndex = 1; floorIndex < map.Count - 2; floorIndex++) 
        {
            var currentFloor = map[floorIndex];
            var nextFloor = map[floorIndex + 1];

            foreach (var node in currentFloor)
            {
                bool modifyNode = true;
                int nodeIndex = currentFloor.IndexOf(node);

                Node closestNode = null;
                int closestIndexDifference = int.MaxValue;

                // Ensure the index exists on the next floor
                if (nodeIndex < nextFloor.Count)
                {
                    closestNode = nextFloor[nodeIndex];
                    closestIndexDifference = 0;
                }
                else
                {
                    // Find the closest valid node if index doesn't exist
                    for (int i = 0; i < nextFloor.Count; i++)
                    {
                        int indexDifference = Mathf.Abs(nodeIndex - i);
                        if (indexDifference < closestIndexDifference)
                        {
                            closestIndexDifference = indexDifference;
                            closestNode = nextFloor[i];
                        }
                    }
                }

                foreach (var connection in node.Connections)
                {
                    if(connection.Position.y > node.Position.y)
                    {
                        modifyNode = false;
                        break;
                    }
                }
                if (modifyNode)
                {
                    node.AddConnection(closestNode);
                    closestNode.AddConnection(node);
                }
            }
        }
    }

    void VisualizeMap()
    {
        foreach (var floor in map)
        {
            foreach (var node in floor)
            {
                Vector2 startPos = new Vector2(node.Position.x * nodeSpacing, node.Position.y * floorHeight);

                GameObject prefab = GetPrefabForNodeType(node.Type);
                GameObject nodeObject = Instantiate(prefab, nodeContainer.transform);
                RectTransform rectTransform = nodeObject.GetComponent<RectTransform>();

                rectTransform.anchoredPosition = new Vector2(node.Position.x * nodeSpacing, node.Position.y * floorHeight);

                NodeReference nodeRef = nodeObject.GetComponent<NodeReference>();
                if (nodeRef == null)
                {
                    nodeRef = nodeObject.AddComponent<NodeReference>();
                }
                nodeRef.node = node;

                Button button = nodeObject.GetComponent<Button>();
                button.onClick.AddListener(() => OnNodeClicked(node));


                //Draw the lines
                foreach (var connection in node.Connections)
                {
                    if (connection.Position.y > node.Position.y)
                    {
                        Vector2 endPos = new Vector2(connection.Position.x * nodeSpacing, connection.Position.y * floorHeight);
                        DrawUILine(startPos, endPos);
                    }
                }
            }
        }
    }

    void PlacePlayerAtStart()
    {
        currentNode = map[0][0]; // Assuming the start node is always the first node of the first floor
        UpdatePlayerPosition();
    }

    void UpdatePlayerPosition()
    {
        playerIcon.anchoredPosition = new Vector2(currentNode.Position.x * nodeSpacing, currentNode.Position.y * floorHeight);
    }

    void SetupScrolling()
    {
        // Calculate the total height of the map
        float totalHeight = (numFloors - 1) * floorHeight + (2 * scrollPadding);

        // Set the height of the ScrollRect's content
        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, totalHeight);

        // Adjust the position of the map within the scroll area
        Vector2 contentPosition = scrollRect.content.anchoredPosition;
        contentPosition.y = -scrollPadding;
        scrollRect.content.anchoredPosition = contentPosition;

        // Start the scroll view at the bottom
        scrollRect.verticalNormalizedPosition = 0f;
    }

    void OnNodeClicked(Node node)
    {
        if (currentNode.Connections.Contains(node) && node.Position.y > currentNode.Position.y)
        {
            currentNode = node;
            UpdatePlayerPosition();

            MapManager.Instance.SetMapDisplay(false);

            switch (node.Type)
            {
                case NodeType.Enemy:
                    battlescript.Instance.sceneChanger("battle1");
                    break;
                case NodeType.Boss:
                    battlescript.Instance.sceneChanger("boss");
                    break;
                case NodeType.Event:
                    battlescript.Instance.activatequiz();
                    break;
                case NodeType.RestSite:
                    battlescript.Instance.sceneChanger("restsite");
                    break;
                default:
                    Debug.LogWarning("Unhandled node type: " + node.Type);
                    break;
            }
        }
        else
        {
            Debug.Log("Cannot move to this node. It's not connected or not on the next floor.");
        }
    }

    GameObject GetPrefabForNodeType(NodeType type)
    {
        switch (type)
        {
            case NodeType.Start: return startNodePrefab;
            case NodeType.Enemy: return enemyNodePrefab;
            case NodeType.RestSite: return restSiteNodePrefab;
            case NodeType.Event: return eventNodePrefab;
            case NodeType.Boss: return bossNodePrefab;
            default: return enemyNodePrefab; // Default case
        }
    }

    void DrawUILine(Vector2 start, Vector2 end)
    {
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.SetParent(lineContainer.transform, false);
        Image lineImage = lineObj.AddComponent<Image>();
        lineImage.color = lineColor;

        RectTransform rectTransform = lineObj.GetComponent<RectTransform>();
        Vector2 dir = (end - start).normalized;
        float distance = Vector2.Distance(start, end);

        rectTransform.anchorMin = rectTransform.anchorMax = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(distance, lineWidth);
        rectTransform.anchoredPosition = start + dir * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }
}

public enum NodeType
{
    Start,
    Enemy,
    RestSite,
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

// Add this new class to store Node reference in GameObjects
public class NodeReference : MonoBehaviour
{
    public Node node;
}