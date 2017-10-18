using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClingOns : MonoBehaviour {

    Rigidbody rb;
    bool isSeeking = true;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {         
        
	}


    private void OnCollisionStay(Collision collision) {
        StartCoroutine("Jump");
    }
    private void OnCollisionExit(Collision collision) {
        StopCoroutine("Jump");
    }

    IEnumerator Jump(){
        if(isSeeking){
            yield return new WaitForSeconds(1);
            rb.AddForce(((FindObjectOfType<PlayerController>().gameObject.transform.position + Vector3.up * 2) - gameObject.transform.position).normalized * 7, ForceMode.Impulse);
        }
    }
}
