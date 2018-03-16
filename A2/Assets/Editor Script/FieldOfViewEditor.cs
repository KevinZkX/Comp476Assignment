using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FieldOfView))]
public class NewBehaviourScript : Editor {

    private void OnSceneGUI()
    {
        FieldOfView fow = (FieldOfView)target;
        Handles.color = Color.red;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);
        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);


        //Handles.color = Color.blue;
        //if (fow.GetComponent<PlayerController>().target != null)
        //{
        //    Handles.DrawLine(fow.transform.position, fow.GetComponent<PlayerController>().target.transform.position);
        //}

        //Handles.color = Color.yellow;
        //Handles.DrawLine(fow.transform.position, fow.transform.position + fow.GetComponent<Rigidbody>().velocity.normalized * 3);
    }
}
