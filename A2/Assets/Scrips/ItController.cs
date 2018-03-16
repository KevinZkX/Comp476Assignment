using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;

public class ItController : PlayerController {

    public GameObject[] targets;
    

    private void FixedUpdate()
    {
        CheckClosetTarget();
        StateMachine();
    }

    public void CheckClosetTarget()
    {
        GameObject temp = new GameObject();
        foreach (GameObject go in targets)
        {
            float distance = Vector3.Distance(go.transform.position, transform.position);
            if (distance < notice_range)
            {
                temp = go;
            }
            Vector3 direction = (temp.transform.position - transform.position).normalized;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit))
            {
                if (hit.collider.gameObject == temp)
                {
                    target = temp;
                }
            }
        }
        if (target != null && Vector3.Distance(target.transform.position, transform.position) > notice_range)
        {
            target = null;
        }
    }

    public void StateMachine()
    {
        if (target == null)
        {
            Wonder();
            
        }
        else
        {
            Debug.Log("in flee");
            FleeIt();
        }
        Move();
    }

	public bool flee()
    {
        float max = 0;
        if (target != null)
        {
            Vector3 direction = target.transform.forward + transform.forward;
            if (Mathf.Approximately(direction.magnitude, 0))
            {
                direction = target.transform.forward;
            }
            foreach (GameObject go in PathGeneraor.pathNode)
            {
                Vector3 d = (go.transform.position - transform.position).normalized;
                if ((direction.x > 0 && direction.z > 0) && (d.x > 0 && d.z > 0))
                {
                    if (Vector3.Distance(go.transform.position, transform.position) > max)
                    {
                        max = Vector3.Distance(go.transform.position, transform.position);
                        end = go;
                    }
                }
                else if ((direction.x < 0 && direction.z > 0) && (d.x < 0 && d.z > 0))
                {
                    if (Vector3.Distance(go.transform.position, transform.position) > max)
                    {
                        max = Vector3.Distance(go.transform.position, transform.position);
                        end = go;
                    }
                }
                else if ((direction.x < 0 && direction.z < 0) && (d.x < 0 && d.z < 0))
                {
                    if (Vector3.Distance(go.transform.position, transform.position) > max)
                    {
                        max = Vector3.Distance(go.transform.position, transform.position);
                        end = go;
                    }
                }
                else if ((direction.x > 0 && direction.z < 0) && (d.x > 0 && d.z < 0))
                {
                    if (Vector3.Distance(go.transform.position, transform.position) > max)
                    {
                        max = Vector3.Distance(go.transform.position, transform.position);
                        end = go;
                    }
                }
            }
            return true;
        }
        return false;
    }

    public void FleeIt()
    {
        if (flee())
        {
            ClosestNode();
        }
    }
}
