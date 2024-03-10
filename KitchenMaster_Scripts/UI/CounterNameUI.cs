using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CounterNameUI : MonoBehaviour
{
    private BaseCounter baseCounter;
    [SerializeField] private TextMeshProUGUI nameText;

    private void Start(){
        baseCounter = GetComponentInParent<BaseCounter>();

        nameText.text = baseCounter.GetDisplayName();
    }
    
}
