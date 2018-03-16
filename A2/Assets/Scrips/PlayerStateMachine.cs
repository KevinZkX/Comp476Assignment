using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum PlayerState { flee, wonder, idol}

public class PlayerStateMachine:MonoBehaviour{

    public PlayerController player;
    public PlayerState playerState;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        
    }
}
