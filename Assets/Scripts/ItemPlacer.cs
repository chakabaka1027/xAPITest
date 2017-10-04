using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(FlightController))]

public class ItemPlacer : MonoBehaviour {

    FlightController flightController;
    //PlayerController playerController;
    int itemSelectionToggle = 0;
    GameObject flyUI;

	// Use this for initialization
	void Start () {
		//playerController = FindObjectOfType<PlayerController>();
        flightController = FindObjectOfType<FlightController>();
        flyUI = GameObject.Find("FlyUI");
        if(itemSelectionToggle == 0){
            flyUI.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
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
}
