﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClingOns : MonoBehaviour {

    Rigidbody rb;
    bool isSeeking = true;

    PlayerController playerController;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
        playerController = FindObjectOfType<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {         
        if(FindObjectOfType<UI_SubmitPlayerInfo>().submittedPlayerInfo){
            isSeeking = true;
            rb.isKinematic = false;
        } 
        
        if (!FindObjectOfType<UI_SubmitPlayerInfo>().submittedPlayerInfo || playerController.flyMode){
            isSeeking = false;
            rb.isKinematic = true;
        }

	}


    private void OnCollisionStay(Collision collision) {
        StartCoroutine("Jump");
    }
    private void OnCollisionExit(Collision collision) {
        StopCoroutine("Jump");
    }


    private void OnCollisionEnter(Collision collision) {
        if(collision.collider.gameObject.name == "Head"){
            playerController.DamageHead();
        }
        if(collision.collider.gameObject.name == "Body"){
            playerController.DamageBody();
        }
    }

    IEnumerator Jump(){
        if(isSeeking){
            yield return new WaitForSeconds(1);
            rb.AddForce(((FindObjectOfType<PlayerController>().gameObject.transform.position + Vector3.up * 2) - gameObject.transform.position).normalized * 7, ForceMode.Impulse);
        }
    }
}
