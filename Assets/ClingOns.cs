using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClingOns : MonoBehaviour {

    Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {

	}


    private void OnCollisionEnter(Collision collision) {
        StartCoroutine(Jump());
    }

    IEnumerator Jump(){
        yield return new WaitForSeconds(1);
        rb.AddForce(((Camera.main.transform.position + Vector3.up * 2) - gameObject.transform.position).normalized * 7, ForceMode.Impulse);
    }
}
