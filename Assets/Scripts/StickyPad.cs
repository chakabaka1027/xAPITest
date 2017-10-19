using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyPad : MonoBehaviour {

    bool isStuck = false;
    PlayerController playerController;
    public LayerMask stickyPad;

    

	// Use this for initialization
	void Start () {
		playerController = FindObjectOfType<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
	    Ray ray = new Ray(transform.position, -transform.up);

        if(Physics.SphereCast(ray, .5f, playerController.height, stickyPad)) {
            Slow();
        }  else {
            isStuck = false;
        }

        if(isStuck){
            playerController.walkSpeed = 1f;
            playerController.runSpeed = 2f;

        } else {
            playerController.walkSpeed = playerController.maxWalkSpeed;
            playerController.runSpeed = playerController.maxRunSpeed;
        }
	}


    void Slow() {
        isStuck = true;
    }
}
