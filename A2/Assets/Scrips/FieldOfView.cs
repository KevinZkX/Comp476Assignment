using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour {

    public float viewRadius;
    public float viewAngle;
    public LayerMask target;
    public LayerMask obs;
    public LayerMask chaser;
    
    public List<Transform> visibleTargets = new List<Transform>();

    private void Start()
    {
        StartCoroutine(FindTargetWithDelay(0.2f));
    }

    IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
            FindTarget();
        }
    }

    void FindVisibleTargets ()
    {
        visibleTargets.Clear();

        Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, target);
        for (int i = 0; i < targetInViewRadius.Length; i++)
        {
            Transform target = targetInViewRadius[i].transform;
            Vector3 direction_to_target = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, direction_to_target) < viewAngle / 2)
            {
                float disToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, direction_to_target, disToTarget, obs))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    void FindTarget()
    {
        Collider[] chasers = Physics.OverlapSphere(transform.position, viewRadius, chaser);
        if (chasers.Length == 0)
        {
            GetComponent<PlayerController>().target = null;
        }
        float distance = 100000;
        foreach (Collider co in chasers)
        {
            if (Vector3.Distance(co.transform.position, transform.position) < distance)
            {
                GetComponent<PlayerController>().target = co.gameObject;
            }
        }
    }

    public Vector3 DirFromAngle (float angle, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angle += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
