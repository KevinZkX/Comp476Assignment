using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]public enum HeuristicType { Dijkstra, Euclidean, Cluster};

public class PathGeneraor : MonoBehaviour {

    static public List<GameObject> pathNode;
    GameObject starting_node;
    GameObject end_node;
    static public HeuristicType heuristic;
    [SerializeField]static List<GameObject> connectorLsit;

    //Please note that connectorCostTable and connectorTable and connectorList are "share" the same index
    //which means that they have the same order
    static List<List<float>> connectorCostTable;
    static List<List<List<GameObject>>> connectorTable;

    public void Setup(GameObject start, GameObject end)
    {
        starting_node = start;
        end_node = end;
    }

	public void FindAllNodes()
    {
        GameObject[] path = GameObject.FindGameObjectsWithTag("PathNode");
        foreach (GameObject go in path)
        {
            pathNode.Add(go);
        }
        //Debug.Log(pathNode.Count);
    }

    public void FindPathForEachNode()
    {
        foreach (GameObject go1 in pathNode)
        {
            foreach (GameObject go2 in pathNode)
            {
                if (go1.transform.position != go2.transform.position)
                {
                    Vector3 direction = go2.transform.position - go1.transform.position;
                    direction.Normalize();
                    float distance = Vector3.Distance(go2.transform.position, go1.transform.position);
                    RaycastHit hit;
                    if (Physics.Raycast(go1.transform.position, direction, out hit, distance))
                    { 
                        if (hit.collider.gameObject != null && hit.collider.tag == "PathNode")
                        {
                            go1.GetComponent<PathNode>().AddNeighbour(hit.collider.gameObject);
                        }
                    }
                }
            }
            if (go1.GetComponent<PathNode>().connector)
            {
                connectorLsit.Add(go1);
            }
            go1.GetComponent<PathNode>().neighbour = go1.GetComponent<PathNode>().neighbour.OrderBy(x => x.cost).ToList();
            //Debug.Log("connectorList.Count: " + connectorLsit.Count);
            //if (go1.GetComponent<PathNode>().connector)
            //{
            //    Debug.Log(go1.name + " " + go1.GetComponent<PathNode>().neighbourConnector.Count);
            //}   
        }
        
    }

    public void CreateClusterTable()
    {
        foreach (GameObject go1 in connectorLsit)
        {
            List<List<GameObject>> tempGo = new List<List<GameObject>>();
            List<float> tempCo = new List<float>();
            foreach (GameObject go2 in connectorLsit)
            {
                if (go1 != go2)
                {
                    List<GameObject> inner_tempGo = new List<GameObject>();
                    float inner_tempCo = 0;
                    inner_tempGo = AlgorithmA(go1, go2);
                    tempGo.Add(inner_tempGo);
                    inner_tempCo = go2.GetComponent<PathNode>().totalCost;
                    tempCo.Add(inner_tempCo);
                }
                else
                {
                    List<GameObject> inner_tempGo = new List<GameObject>();
                    float inner_tempCo = 0;
                    tempGo.Add(inner_tempGo);
                    tempCo.Add(inner_tempCo);
                }
            }
            connectorTable.Add(tempGo);
            connectorCostTable.Add(tempCo);
        }
    }

    static public void Euclidean(GameObject node, GameObject goal)
    {
        node.GetComponent<PathNode>().heuristicValue = Vector3.Distance(goal.transform.position, node.transform.position);
    }

    static public List<GameObject> Cluster(GameObject node, GameObject nextNode)
    {
        List<int> startClusterIndex = new List<int>();
        List<int> endClusterIndex = new List<int>();
        List<float> startCost = new List<float>();
        List<float> endCost = new List<float>();
        Zone startZone = node.GetComponent<PathNode>().nodeZone;
        Zone endZone = nextNode.GetComponent<PathNode>().nodeZone;
        List<GameObject> startClusterConnector = new List<GameObject>();
        List<GameObject> endClusterConnector = new List<GameObject>();
        List<GameObject> final_path = new List<GameObject>();

        int startIndex = 0;
        int startClIndex = 0;
        int endClIndex = 0;
        int endIndex = 0;
        float temp_total_cost = 100000f;

        if (startZone == endZone)
        {
            return AlgorithmA(node, nextNode);
        }

        for (int i = 0; i < connectorLsit.Count; i++)
        {
            if (connectorLsit[i].GetComponent<PathNode>().nodeZone == startZone)
            {
                startClusterConnector.Add(connectorLsit[i]);
                startClusterIndex.Add(i);
            }
            else if (connectorLsit[i].GetComponent<PathNode>().nodeZone == endZone)
            {
                endClusterConnector.Add(connectorLsit[i]);
                endClusterIndex.Add(i);
            }
        }

        List<List<GameObject>> startClusterPath = new List<List<GameObject>>();
        List<List<GameObject>> endClusterPath = new List<List<GameObject>>();

        foreach (GameObject go in startClusterConnector)
        {
            List<GameObject> startPath = new List<GameObject>();
            startPath = AlgorithmA(node, go);
            startClusterPath.Add(startPath);
            float cost = go.GetComponent<PathNode>().totalCost;
            startCost.Add(cost);
        }
        
        foreach (GameObject go in endClusterConnector)
        {
            List<GameObject> endPath = new List<GameObject>();
            endPath = AlgorithmA(go, nextNode);
            endClusterPath.Add(endPath);
            float cost = go.GetComponent<PathNode>().totalCost;
            endCost.Add(cost);
        }
        //problems here
        for (int start = 0; start < startCost.Count; start++)
        {
            foreach (int startC in startClusterIndex)
            {
                foreach (int endC in endClusterIndex)
                {
                    if (startClusterPath[start][startClusterPath[start].Count - 1] == connectorTable[startC][endC][0])
                    {
                        for (int end = 0; end < endCost.Count; end++)
                        {
                            if (endClusterPath[end][0] == connectorTable[startC][endC][connectorTable[startC][endC].Count - 1])
                            {
                                float temp = startCost[start] + connectorCostTable[startC][endC] + endCost[end];
                                if (temp < temp_total_cost)
                                {
                                    temp_total_cost = temp;
                                    startIndex = start;
                                    startClIndex = startC;
                                    endClIndex = endC;
                                    endIndex = end;
                                }
                            }
                        }
                    }
                }
            }
        }

        //Debug.Log(startIndex + " " + startClIndex + " " + endClIndex + " " + endIndex);
        //Debug.Log("total cost: " + temp_total_cost);
        //Debug.Log(connectorLsit[startIndex].name);
        //Debug.Log(connectorLsit[endIndex].name);
        //Debug.Log(connectorTable[startClIndex][endClIndex].Count);
        //foreach (GameObject go in connectorTable[startClIndex][endClIndex])
        //{
        //    Debug.Log(go.name + "!!!!!!!!");
        //}

        for (int i = 1; i < startClusterPath[startIndex].Count; i++)
        {
            final_path.Add(startClusterPath[startIndex][i]);
        }
        for (int i = 1; i < startClusterPath[startIndex].Count; i++)
        {
            final_path.Add(connectorTable[startClIndex][endClIndex][i]);
        }
        for (int i = 1; i < startClusterPath[startIndex].Count; i++)
        {
            final_path.Add(endClusterPath[endIndex][i]);
        }
        
        return final_path;
    }

    static public List<GameObject> AlgorithmA(GameObject start, GameObject end)
    {
        List<GameObject> closeList = new List<GameObject>();
        List<GameObject> openList = new List<GameObject>();
        List<GameObject> finalPath = new List<GameObject>();
        foreach (GameObject go in pathNode)
        {
            go.GetComponent<PathNode>().totalCost = 0;
            go.GetComponent<PathNode>().ResetPath();
        }
        RecursiveAlgorithmACall(start, end, closeList, openList);
        foreach (GameObject go in end.GetComponent<PathNode>().path)
        {
            finalPath.Add(go);
        }
        return finalPath;
    }

    static private void RecursiveAlgorithmACall(GameObject start, GameObject end, List<GameObject> close, List<GameObject> open)
    {
        //If it is not the goal
        if (start != end)
        {
            //Take the current node out of the open list
            open.Remove(start);
            //Add the current node to close list
            close.Add(start);
            //Make sure that the firt node's heuristic value is calculated
            if (heuristic != 0)
            {
                switch (heuristic)
                {
                    case HeuristicType.Dijkstra:
                        start.GetComponent<PathNode>().heuristicValue = 0;
                        break;
                    case HeuristicType.Euclidean:
                        Euclidean(start, end);
                        break;
                }
            }
            //Iterate all its neighbour
            foreach (neighbourNode nb in start.GetComponent<PathNode>().neighbour)
            {
                switch (heuristic)
                {
                    case HeuristicType.Dijkstra:
                        nb.node.GetComponent<PathNode>().heuristicValue = 0;
                        break;
                    case HeuristicType.Euclidean:
                        Euclidean(nb.node, end);
                        break;
                }
                float temp_total_cost = start.GetComponent<PathNode>().totalCost + nb.cost + nb.node.GetComponent<PathNode>().heuristicValue;
                //If the neighbour is in the close list
                if (close.Contains(nb.node))
                {
                    //If the total cost is smaller than the previous
                    //then do-->
                    if (close.Find(x => x == nb.node).GetComponent<PathNode>().totalCost > temp_total_cost)
                    {
                        close.Remove(nb.node);
                        open.Add(nb.node);
                        nb.node.GetComponent<PathNode>().totalCost = temp_total_cost;
                        nb.node.GetComponent<PathNode>().UpdatePath(start.GetComponent<PathNode>().path);
                    }
                }
                //If the neighbour has already been in the open list
                else if (open.Contains(nb.node))
                {
                    //Check if the total cost is smaller than the previous one
                    //If yes, do-->
                    if (open.Find(x => x == nb.node).GetComponent<PathNode>().totalCost > temp_total_cost )
                    {
                        nb.node.GetComponent<PathNode>().totalCost = temp_total_cost;
                        nb.node.GetComponent<PathNode>().UpdatePath(start.GetComponent<PathNode>().path);
                    }
                }
                //If the neighbour has not been added to the open list
                //Add it to the open list
                else
                {
                    open.Add(nb.node);
                    nb.node.GetComponent<PathNode>().totalCost = temp_total_cost;
                    nb.node.GetComponent<PathNode>().UpdatePath(start.GetComponent<PathNode>().path);
                }
            }
            open = open.OrderBy(x => x.GetComponent<PathNode>().totalCost).ToList();
            RecursiveAlgorithmACall(open[0], end, close, open);
        }
    }

    private void Awake()
    {
        pathNode = new List<GameObject>();
        connectorLsit = new List<GameObject>();
        connectorCostTable = new List<List<float>>();
        connectorTable = new List<List<List<GameObject>>>();
        FindAllNodes();
        FindPathForEachNode();
        connectorLsit = connectorLsit.OrderBy(x => x.GetComponent<PathNode>().nodeZone).ToList();
        CreateClusterTable();
        heuristic = HeuristicType.Euclidean;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
