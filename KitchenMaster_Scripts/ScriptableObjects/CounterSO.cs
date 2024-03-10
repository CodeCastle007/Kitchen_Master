using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CounterSO : ScriptableObject
{
    public Sprite icon;
    public string counterName;
    public string displayUIName;
    public string tutorialMessage;

    public float cost;
}
