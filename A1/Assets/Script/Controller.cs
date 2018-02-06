using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum GameState
{
    Kenimatic, Steering
}

public class Controller : MonoBehaviour {

    Camera viewCamera;
    [SerializeField]Text stateText;

	// Use this for initialization
	void Start () {
        viewCamera = Camera.main;
        stateText.text = "Kenimatic";
    }
	
	// Update is called once per frame
	void Update () {
        
        Vector3 mousePos = viewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, viewCamera.transform.position.y));
	}

    public void TriggerKenimatic()
    {
        HumanBehavior.SetState(0);
        stateText.text = "Kenimatic";
    }

    public void TriggerSteering()
    {
        HumanBehavior.SetState(1);
        stateText.text = "Steering";
    }
}
