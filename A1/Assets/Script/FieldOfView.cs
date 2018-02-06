using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour {

    public float viewRadius;
    public float viewAngle;

    public LayerMask targetLayer;
    public LayerMask obstacleLayer;

    
    [HideInInspector]public List<Transform> visibleTargets = new List<Transform>();

    private void Start()
    {
        viewAngle = GetComponent<HumanBehavior>().field_of_view_angle;
        StartCoroutine(FindTargetWithDelay(0.2f));
    }

    IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets(gameObject.layer);

        }
    }

    void FindVisibleTargets (int layer)
    {
        visibleTargets.Clear();
        if (layer == 9)
        {
            Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetLayer);

            for (int i = 0; i < targetInViewRadius.Length; i++)
            {
                Transform target = targetInViewRadius[i].transform;
                Vector3 direction_to_target = (target.position - transform.position).normalized;
                //if (Vector3.Angle(transform.forward, direction_to_target) < viewAngle / 2)
                //{
                    float disToTarget = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, direction_to_target, disToTarget, obstacleLayer))
                    {
                        visibleTargets.Add(target);
                    }
                //}
            }
        }
        else if (layer == 8)
        {
            Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetLayer);

            for (int i = 0; i < targetInViewRadius.Length; i++)
            {
                Transform target = targetInViewRadius[i].transform;
                Vector3 direction_to_target = (target.position - transform.position).normalized;

                float disToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, direction_to_target, disToTarget, obstacleLayer))
                {
                    visibleTargets.Add(target);
                }

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
