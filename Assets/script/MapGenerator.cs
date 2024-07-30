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

    [SerializeField] private Transform mapContainer;
    [SerializeField] private Canvas mapCanvas;

    [SerializeField] private GameObject lineContainer;
    [SerializeField] private GameObject nodeContainer;


    [SerializeField] private Color lineColor = Color.white;
    [SerializeField] private float lineWidth = 2f;

    

    private List<List<Node>> map;

    void Start()
    {
        
        GenerateMap();
        VisualizeMap();
        SetupContainers();
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

        
        lineContainer.transform.SetSiblingIndex(0);
        nodeContainer.transform.SetSiblingIndex(1);
    }

    void VisualizeMap()
    {
        
        foreach (var floor in map)
        {
            foreach (var node in floor)
            {
                Vector2 startPos = new Vector2(node.Position.x * nodeSpacing, node.Position.y * floorHeight);
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

        
        foreach (var floor in map)
        {
            foreach (var node in floor)
            {
                GameObject prefab = GetPrefabForNodeType(node.Type);
                GameObject nodeObject = Instantiate(prefab, nodeContainer.transform);
                RectTransform rectTransform = nodeObject.GetComponent<RectTransform>();
                
                rectTransform.anchoredPosition = new Vector2(node.Position.x * nodeSpacing, node.Position.y * floorHeight);
                
                NodeReference nodeRef = nodeObject.AddComponent<NodeReference>();
                nodeRef.node = node;
                
                
                Button button = nodeObject.GetComponent<Button>();
                if (button == null)
                {
                    button = nodeObject.AddComponent<Button>();
                }
                
                
                button.onClick.AddListener(() => OnNodeClicked(node));
            }
        }
    }

    void OnNodeClicked(Node node)
    {
        
        string sceneToLoad = "";
        switch (node.Type)
        {
            case NodeType.Enemy:
                sceneToLoad = "enemy";  
                break;
            case NodeType.Boss:
                sceneToLoad = "boss";  
                break;
            case NodeType.Event:
                sceneToLoad = "event";
                break;
            case NodeType.RestSite:
                sceneToLoad = "rest"; 
                break;
            default:
                Debug.LogWarning("Unhandled node type: " + node.Type);
                return;
        }

        
        battlescript.Instance.sceneChanger(sceneToLoad);
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