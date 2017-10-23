using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour {

    bool isStuck = false;
    PlayerController playerController;

    public float headDamage;
    public float bodyDamage;

	// Use this for initialization
	void Start () {
		playerController = FindObjectOfType<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision) {
        if(collision.collider.gameObject.name == "Head"){
            playerController.DamageHead(headDamage);
        }
        if(collision.collider.gameObject.name == "Body"){
            playerController.DamageBody(bodyDamage);
        }
        if (collision.collider.gameObject.tag == "StickyPad") {
            isStuck = true;
        }
    }
}
