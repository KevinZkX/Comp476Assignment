using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public void ChangeHeuristic(int i)
    {
        if (i == 0)
        {
            PathGeneraor.heuristic = HeuristicType.Dijkstra;
        }
        else if (i == 1)
        {
            PathGeneraor.heuristic = HeuristicType.Euclidean;
        }
        else if (i == 2)
        {
            PathGeneraor.heuristic = HeuristicType.Cluster;
        }
    }
}
