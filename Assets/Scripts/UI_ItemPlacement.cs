using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//namespace of the outline script on each item that allows an outline to be rendered onscreen
using cakeslice;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(FlightController))]

public class UI_ItemPlacement : MonoBehaviour {

    FlightController flightController;
    PlayerController playerController;
    public int itemSelectionToggle = 0;
    GameObject flyUI;

    public GameObject[] items;
    public int objectIndex = -1;
    
    int duplicateObjIndex = -1;
    bool duplicateObjSpawned = false;

    public LayerMask itemSpawnLocation;
    public LayerMask itemLayer;

    bool hasSpawned = false;
    GameObject heldObject;

    [HideInInspector]
    public GameObject objCurrentlyHoveringOver;


	// Use this for initialization
	void Start () {
		playerController = FindObjectOfType<PlayerController>();
        flightController = FindObjectOfType<FlightController>();
        flyUI = GameObject.Find("FlyUI");
        if(itemSelectionToggle == 0){
            flyUI.SetActive(false);
        }
	}

    void Update() {

    //highlight objects when hovering mouse over them
        if(flightController.selectingItem) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		    RaycastHit hit;

		    if(Physics.Raycast(ray, out hit, Mathf.Infinity, itemLayer)){    
                if(objCurrentlyHoveringOver != null) {
                    objCurrentlyHoveringOver.GetComponent<Outline>().eraseRenderer = true;
                }

                objCurrentlyHoveringOver = hit.collider.gameObject;
                objCurrentlyHoveringOver.GetComponent<Outline>().eraseRenderer = false;

            } else if(objCurrentlyHoveringOver != null){
                objCurrentlyHoveringOver.GetComponent<Outline>().eraseRenderer = true;
                objCurrentlyHoveringOver = null;
            }
        }
    }
	
	void LateUpdate () {
        if(playerController.flyMode && itemSelectionToggle == 1){
		    if(Input.GetMouseButton(0)){
                ObjFollowMouseCursor();
            }

            if(Input.GetMouseButtonUp(0)){
                ReleaseItem();
            }
            if(Input.GetMouseButton(1)){
                if(!duplicateObjSpawned && duplicateObjIndex > -1){
                    DuplicateLastObj();
                    duplicateObjSpawned = true;
                }
                ObjFollowMouseCursor();
            }
            if(Input.GetMouseButtonUp(1)){
                ReleaseDuplicateItem();
                duplicateObjSpawned = false;
            }
        }
	}

    //make objects follow the mouse position
    void ObjFollowMouseCursor(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if(Physics.SphereCast(ray, .5f, out hit, Mathf.Infinity, itemSpawnLocation) && heldObject != null){
            heldObject.transform.position = hit.point + hit.normal * .5f;
        } if(!Physics.Raycast(ray, out hit, Mathf.Infinity, itemSpawnLocation) && heldObject != null){
            heldObject.transform.position = ray.GetPoint(10);

        }
    }

    public void ItemSelectionToggle(){
        itemSelectionToggle = 1 - itemSelectionToggle;

        //on
        if(itemSelectionToggle == 1){
            flightController.selectingItem = true;
            flyUI.SetActive(true);
            flyUI.transform.Find("ItemMenu").GetComponent<Animator>().Play("Open");
            flyUI.transform.Find("Borders").GetComponent<Animator>().Play("FadeIn");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        //off
        if(itemSelectionToggle == 0){
            flightController.selectingItem = false;
            Cursor.visible = false;        
            Cursor.lockState = CursorLockMode.Locked;
            flyUI.transform.Find("ItemMenu").GetComponent<Animator>().Play("Close");
            flyUI.transform.Find("Borders").GetComponent<Animator>().Play("FadeOut");

        }
    }

    //spawning items
    public void SpawnItem(){
        if(objectIndex > -1 && Input.GetMouseButton(0) && heldObject == null){

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

            heldObject.transform.parent = GameObject.Find("Level Objects").transform;
            //StartCoroutine("ExpandWhenSpawned");
        }
    }

    void ReleaseItem(){
        if(heldObject != null){
            heldObject.GetComponent<Rigidbody>().isKinematic = false;
        }
        hasSpawned = false;
        heldObject = null;
        if(objectIndex > -1){
            duplicateObjIndex = objectIndex;
        }
        objectIndex = -1;
    }

    //duplicating already selected items
    void DuplicateLastObj(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if(Physics.SphereCast(ray, .5f, out hit, Mathf.Infinity, itemSpawnLocation)){
            if(!hasSpawned){
                heldObject = Instantiate(items[duplicateObjIndex], hit.point + hit.normal * .5f, Quaternion.identity) as GameObject;
                heldObject.GetComponent<Rigidbody>().isKinematic = true;
            }
        } else {
            heldObject = Instantiate(items[duplicateObjIndex], ray.GetPoint(10), Quaternion.identity) as GameObject;
            heldObject.GetComponent<Rigidbody>().isKinematic = true;
        }    
        heldObject.transform.parent = GameObject.Find("Level Objects").transform;
        //StartCoroutine("ExpandWhenSpawned");
    }
    
    void ReleaseDuplicateItem(){
        if(heldObject != null){
            heldObject.GetComponent<Rigidbody>().isKinematic = false;
        }
        hasSpawned = false;
        heldObject = null;
    }

    IEnumerator ExpandWhenSpawned(){
        float percent = 0;
        float time = .5f;
        float speed = 1/time;
        
        while(percent < 1){
            percent += Time.deltaTime * speed;
            heldObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, percent);
            yield return null;
        }
    }

    //moving existing items

}
