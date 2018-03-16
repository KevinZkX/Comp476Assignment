using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;

public class PlayerController : MonoBehaviour {

    [HideInInspector]public GameObject fleeingNode;
    public GameObject pathGenerator;
    public GameObject start;
    public GameObject end;
    //public List<GameObject> target;
    public float max_speed;
    public float maixum_angular_velocity_magnitude;
    //PlayerStateMachine playerStateMachine;
    protected List<GameObject> path;
    protected Rigidbody rigidbody;
    bool calculated = false;
    int path_index = 0;
    public GameObject target;
    public float notice_range;

    // Use this for initialization
    void Start () {
        Init();
    }

    public void Init()
    {
        path = new List<GameObject>();
        rigidbody = GetComponent<Rigidbody>();
        //target = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        //playerStateMachine = GetComponent<PlayerStateMachine>();
        //foreach (GameObject go in PathGeneraor.pathNode)
        //{
        //    Physics.IgnoreCollision(GetComponent<CapsuleCollider>(), go.GetComponent<SphereCollider>());
        //}
    }

    protected void ClosestNode()
    {
        RaycastHit hit;
        float dis = 10000;
        foreach (GameObject go in PathGeneraor.pathNode)
        {
            Vector3 direction = (go.transform.position - transform.position).normalized;
            float distance = Vector3.Distance(go.transform.position, transform.position);
            if (Physics.Raycast(transform.position, direction, out hit, distance))
            {
                if (hit.collider.gameObject != null && hit.collider.tag == "PathNode")
                {
                    if (dis > Vector3.Distance(hit.collider.transform.position, transform.position))
                    {
                        dis = Vector3.Distance(hit.collider.transform.position, transform.position);
                        start = hit.collider.gameObject;
                    }
                }
            }
        }
    }

    private void CreatePath()
    {
        if (!calculated)
        {
            switch (PathGeneraor.heuristic)
            {
                case HeuristicType.Dijkstra:
                case HeuristicType.Euclidean:
                    path = PathGeneraor.AlgorithmA(start, end);
                    break;
                case HeuristicType.Cluster:
                    path = PathGeneraor.Cluster(start, end);
                    break;
            }
            
            calculated = true;
        }
        //for (int i = 0; i < path.Count - 1; i++)
        //{
        //    path[i].GetComponent<LineRenderer>().enabled = true;
        //    path[i].GetComponent<LineRenderer>().SetPosition(0, path[i].transform.position);
        //    path[i].GetComponent<LineRenderer>().SetPosition(1, path[i + 1].transform.position);
        //}
    }

    protected void Move()
    {
        //Debug.Log("Path index: " + path_index);
        //Debug.Log("Path count: " + path.Count);
        if (path_index < path.Count)
        {
            if (Vector3.Distance(transform.position, path[path_index].transform.position) < 0.5f)
            {
                //if (path_index > 0)
                //{
                //    path[path_index - 1].GetComponent<LineRenderer>().enabled = false;
                //}
                //Debug.Log("Next");
                path_index++;
            }
            else
            {
                Vector3 direction = (path[path_index].transform.position - transform.position).normalized;
                transform.LookAt(direction);
                rigidbody.velocity = direction * max_speed;
                Align(path[path_index]);
            }
        }
        else
        {
            rigidbody.velocity = Vector3.zero;
            path.Clear();
            path_index = 0;
            start = null;
            end = null;
            calculated = false;
        }
    }

    protected void PickUpAnEnd()
    {
        if (end == null)
        {
            int index = Random.Range(0, PathGeneraor.pathNode.Count);
            if (PathGeneraor.pathNode[index] != start)
            {
                end = PathGeneraor.pathNode[index];
            }
        }
        else
        {
            return;
        }
    }

    protected void Wonder()
    {
        if (path.Count == 0)
        { 
            ClosestNode();
            PickUpAnEnd();
            if (end != null)
            {
                CreatePath();
            }
        }
    }

    protected void Align(GameObject target)
    {
        Quaternion look_where_going;
        if (target != null)
        {
            Vector3 direciton = (target.transform.position - transform.position).normalized;
            look_where_going = Quaternion.LookRotation(direciton);
        }
        else
        {
            look_where_going = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity.normalized);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, look_where_going, maixum_angular_velocity_magnitude);
    }
}
