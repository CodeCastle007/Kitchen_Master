using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIReset : MonoBehaviour
{
    
    private void Start(){

        GetComponent<RectTransform>().anchoredPosition = Vector3.zero; 
    }
}
