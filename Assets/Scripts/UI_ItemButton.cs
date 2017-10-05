using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ItemButton : MonoBehaviour {

    public void AssignObjectIndex(int index){
        if(Input.GetMouseButtonDown(0)){
            FindObjectOfType<UI_ItemPlacement>().objectIndex = index;
        }
    }
}
