using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(FlightController))]

public class UI_ItemPlacement : MonoBehaviour {

    FlightController flightController;
    PlayerController playerController;
    public int itemSelectionToggle = 0;
    GameObject flyUI;

    public GameObject[] items;
    public int objectIndex = -1;
    public LayerMask itemSpawnLocation;

    bool hasSpawned = false;
    GameObject heldObject;


	// Use this for initialization
	void Start () {
		playerController = FindObjectOfType<PlayerController>();
        flightController = FindObjectOfType<FlightController>();
        flyUI = GameObject.Find("FlyUI");
        if(itemSelectionToggle == 0){
            flyUI.SetActive(false);
        }
	}
	
	void LateUpdate () {
        if(playerController.flyMode && itemSelectionToggle == 1){
		    if(Input.GetMouseButton(0)){

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		        RaycastHit hit;

		        if(Physics.SphereCast(ray, .5f, out hit, Mathf.Infinity, itemSpawnLocation) && heldObject != null){
                    heldObject.transform.position = hit.point + hit.normal * .5f;
                } if(!Physics.Raycast(ray, out hit, Mathf.Infinity, itemSpawnLocation) && heldObject != null){
                        heldObject.transform.position = ray.GetPoint(10);

                }
            }

            if(Input.GetMouseButtonUp(0)){
                ReleaseItem();
            }
        }
	}

    public void ItemSelectionToggle(){
        itemSelectionToggle = 1 - itemSelectionToggle;

        //on
        if(itemSelectionToggle == 1){
            flightController.selectingItem = true;
            flyUI.SetActive(true);
            flyUI.transform.Find("ItemMenu").GetComponent<Animator>().Play("Open");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        //off
        if(itemSelectionToggle == 0){
            flightController.selectingItem = false;
            Cursor.visible = false;        
            Cursor.lockState = CursorLockMode.Locked;
            flyUI.transform.Find("ItemMenu").GetComponent<Animator>().Play("Close");
        }
    }

    public void SpawnItem(){
        if(objectIndex > -1 && Input.GetMouseButton(0)){

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		    RaycastHit hit;

		    if(Physics.SphereCast(ray, .5f, out hit, Mathf.Infinity, itemSpawnLocation)){
                if(!hasSpawned){
                    heldObject = Instantiate(items[objectIndex], hit.point + hit.normal * .5f, Quaternion.identity) as GameObject;
                    heldObject.GetComponent<Rigidbody>().isKinematic = true;
                }
            } else {
                heldObject = Instantiate(items[objectIndex], ray.GetPoint(10), Quaternion.identity) as GameObject;
                heldObject.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }

    void ReleaseItem(){
        if(heldObject != null){
            heldObject.GetComponent<Rigidbody>().isKinematic = false;
        }
        hasSpawned = false;
        heldObject = null;
        objectIndex = -1;
    }
}
