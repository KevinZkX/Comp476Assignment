using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum Zone
{
    zone0, zone1, zone2, zone3, zone4
}

[System.Serializable]
public struct neighbourNode
{
    public GameObject node;
    public float cost;

    public neighbourNode(GameObject node, float cost)
    {
        this.node = node;
        this.cost = cost;
    }
}

public class PathNode : MonoBehaviour {

    public float heuristicValue;
    public List<neighbourNode> neighbour;
    public bool connector = false;
    public Zone nodeZone;
    public List<neighbourNode> neighbourConnector;
    public List<GameObject> path;
    public float totalCost;

    private void Awake()
    {
        neighbour = new List<neighbourNode>();
        neighbourConnector = new List<neighbourNode>();
        path = new List<GameObject>();
        path.Add(gameObject);
    }

    public void AddNeighbour (GameObject node)
    {
        if (!neighbour.Exists(x => x.node.name.Contains(node.name)))
        {
            if (this.nodeZone == node.GetComponent<PathNode>().nodeZone)
            {
                float cost = Vector3.Distance(node.transform.position, transform.position);
                neighbourNode neigh = new neighbourNode(node, cost);
                neighbour.Add(neigh);
                if (!node.GetComponent<PathNode>().neighbour.Exists(x => x.node == gameObject))
                {
                    Debug.DrawLine(transform.position, node.transform.position, Color.red, 100);
                }
            }
            else
            {
                if (this.connector && node.GetComponent<PathNode>().connector)
                {
                    float cost = Vector3.Distance(node.transform.position, transform.position);
                    neighbourNode neigh = new neighbourNode(node, cost);
                    neighbour.Add(neigh);
                    neighbourConnector.Add(neigh);
                    if (!node.GetComponent<PathNode>().neighbour.Exists(x => x.node == gameObject))
                    {
                        Debug.DrawLine(transform.position, node.transform.position, Color.red, 100);
                    }
                }
            }
        }
    }

    public void UpdatePath (List<GameObject> new_path)
    {
        path.RemoveRange(0, path.Count);
        path.AddRange(new_path);
        path.Add(gameObject);
    }

    public void ResetPath()
    {
        path.Clear();
        path.Add(gameObject);
    }
}
