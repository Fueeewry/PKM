using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Node currentNode;
    private bool isMoving = false;
    private Vector3 targetPosition;

    void Start()
    {
        // Find the start node and position the player there
        GameObject startNode = GameObject.FindGameObjectWithTag("StartNode");
        if (startNode != null)
        {
            currentNode = startNode.GetComponent<NodeReference>().node;
            transform.position = startNode.transform.position;
        }
        else
        {
            Debug.LogError("Start node not found!");
        }
    }

    void Update()
    {
        if (isMoving)
        {
            // Move towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Check if we've reached the target
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
            }
        }
        else
        {
            // Check for player input to move to connected nodes
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    NodeReference nodeRef = hit.collider.GetComponent<NodeReference>();
                    if (nodeRef != null && currentNode.Connections.Contains(nodeRef.node))
                    {
                        MoveToNode(nodeRef.node);
                    }
                }
            }
        }
    }

    void MoveToNode(Node targetNode)
    {
        currentNode = targetNode;
        targetPosition = new Vector3(targetNode.Position.x, targetNode.Position.y, 0) * 100; // Match the scaling in MapGenerator
        isMoving = true;

        // Here you can trigger events based on the node type
        switch (targetNode.Type)
        {
            case NodeType.Enemy:
                Debug.Log("Entered combat!");
                // Trigger combat event
                break;
            case NodeType.RestSite:
                Debug.Log("Resting...");
                // Trigger rest event
                break;
            case NodeType.Event:
                Debug.Log("Random event occurred!");
                // Trigger random event
                break;
            case NodeType.Boss:
                Debug.Log("Boss fight!");
                // Trigger boss fight
                break;
        }
    }
}