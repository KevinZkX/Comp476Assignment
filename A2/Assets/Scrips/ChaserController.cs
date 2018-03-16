using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;

public class ChaserController : PlayerController {

    public bool Win { get; set; }
    public bool Visiable { get; set; }
    public bool ItFlees { get; set; }
    BehaviorTree bt;
    GameObject[] chasers;

    private void Awake()
    {
        Win = false;
        Visiable = false;
        ItFlees = false;

        bt = new BehaviorTree(Application.dataPath + "/Scrips/a.xml", this);
        bt.Start();
        foreach (GameObject go in PathGeneraor.pathNode)
        {
            Physics.IgnoreCollision(GetComponent<CapsuleCollider>(), go.GetComponent<SphereCollider>());
        }
        
    }

    private void Start()
    {
        Init();
        chasers = GameObject.FindGameObjectsWithTag("Chaser");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            Win = true;
        }
    }

    private void FixedUpdate()
    {
        CheckTheClosetTarget();
        if (Visiable)
        {
            foreach (GameObject go in chasers)
            {
                if (go != this.gameObject)
                {
                    go.GetComponent<ChaserController>().ItFlees = false;
                }
            }
        }
        else if (ItFlees)
        {

        }
        if (path.Count > 0)
        {
            Move();
        }

    }

    public void CheckTheClosetTarget()
    {
        if (Vector3.Distance(target.transform.position, transform.position) < notice_range)
        {
            RaycastHit hit;
            Vector3 direction = (target.transform.position - transform.position).normalized;
            if (Physics.Raycast(transform.position, direction, out hit))
            {
                if (hit.collider.gameObject == target)
                {
                    Visiable = true;
                }
            }
        }
        else
        {
            Visiable = false;
        }
    }

    public void SeekIt()
    {
        if (target != null)
        {
            Debug.Log("Seek");
            if (true)
            {
                Vector3 direction = (target.transform.position - transform.position).normalized;
                rigidbody.velocity = direction * max_speed;
                Align(target);
            }
            //float distanceOfTarget = 100000;
            //float distanceOfSelf = 100000;
            //foreach (GameObject go in PathGeneraor.pathNode)
            //{
            //    if (distanceOfTarget > Vector3.Distance(go.transform.position, target.transform.position))
            //    {
            //        distanceOfTarget = Vector3.Distance(go.transform.position, target.transform.position);
            //        closetNodeToTarge = go;
            //    }
            //    if (distanceOfSelf > Vector3.Distance(go.transform.position, transform.position))
            //    {
            //        distanceOfTarget = Vector3.Distance(go.transform.position, transform.position);
            //        closetNodeToSelf = go;
            //    }
            //}
            //path = PathGeneraor.Cluster(closetNodeToSelf, closetNodeToTarge);
        }
    }

    public void FlankIt()
    {
        if (target != null)
        {
            GameObject go = target.GetComponent<PlayerController>().fleeingNode;
            ClosestNode();
            path = PathGeneraor.Cluster(start, go);
        }
    }

    [BTLeaf("target-catched")]
    public bool WinTheGame()
    {
        return Win;
    }

    [BTLeaf("target-in-vision")]
    public bool IsInVision()
    {
        return Visiable;
    }

    [BTLeaf("seek-it")]
    public BTCoroutine Seek()
    {
        Debug.Log("seek");
        if (!IsInVision())
        {
            yield return BTNodeResult.Failure;
            yield break;
        }
        SeekIt();
        foreach (GameObject go in chasers)
        {
            Debug.Log("flank guys");
            if (go != this.gameObject)
            {
                go.GetComponent<ChaserController>().ItFlees = true;
            }
        }   
        while (true)
        {
            if (WinTheGame())
            {
                yield return BTNodeResult.Success;
                break;
            }
            if (!IsInVision())
            {
                yield return BTNodeResult.Failure;
                yield break;
            }
            yield return BTNodeResult.NotFinished;
        }
    }

    [BTLeaf("target-fleeing")]
    public bool IsFlee()
    {
        return ItFlees;
    }

    [BTLeaf("flank-it")]
    public BTCoroutine Flank()
    {
        Debug.Log("flank");
        FlankIt();
        while (true)
        {
            if (WinTheGame())
            {
                yield return BTNodeResult.Success;
                yield break;
            }
            else if (!IsFlee())
            {
                yield return BTNodeResult.Failure;
                yield break;
            }
            yield return BTNodeResult.NotFinished;
        }
        
    }

    [BTLeaf("wander")]
    public BTCoroutine Wander()
    {
        Debug.Log("wonder");
        Wonder();
        yield return BTNodeResult.Success;
    }
}
