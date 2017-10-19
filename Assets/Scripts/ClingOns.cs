using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClingOns : MonoBehaviour {

    Rigidbody rb;
    bool isSeeking = true;
    bool isStuck;

    PlayerController playerController;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
        playerController = FindObjectOfType<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {         
        if(FindObjectOfType<UI_SubmitPlayerInfo>().submittedPlayerInfo && !isStuck){
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
            StartCoroutine(Stick(collision.collider.gameObject));
        }
        if(collision.collider.gameObject.name == "Body"){
            playerController.DamageBody();
            StartCoroutine(Stick(collision.collider.gameObject));
        }
        //if(collision.collider.gameObject.tag == "StickyPad"){
        //    isStuck = true;
        //    //gameObject.transform.parent = collision.collider.gameObject.transform;
        //    StopCoroutine("Jump");
        //    rb.isKinematic = true;
        //    //gameObject.GetComponent<SphereCollider>().enabled = false;
        //}
    }

    IEnumerator Jump(){
        if(isSeeking){
            yield return new WaitForSeconds(.5f);
            rb.AddForce(((FindObjectOfType<PlayerController>().gameObject.transform.position + FindObjectOfType<PlayerController>().gameObject.transform.up* 2.5f) - gameObject.transform.position).normalized * 6.5f, ForceMode.Impulse);
        }
    }

    IEnumerator Stick(GameObject surface){
        isStuck = true;
        gameObject.transform.parent = surface.transform;
        StopCoroutine("Jump");
        rb.isKinematic = true;
        gameObject.GetComponent<SphereCollider>().enabled = false;
        yield return new WaitForSeconds(3);
        //gameObject.GetComponent<SphereCollider>().enabled = true;    
        //gameObject.transform.parent = null;
        //rb.isKinematic = false;
        //isStuck = false;
        
        float percent = 0;
        float time = 1;
        float speed = 1 / time;

        Vector3 currentScale = gameObject.transform.localScale;

        while(percent < 1){
            percent += Time.deltaTime * speed;
            gameObject.transform.localScale = Vector3.Lerp(currentScale, Vector3.zero, percent);
            yield return null;
        }
        Destroy(gameObject);
    }

}
