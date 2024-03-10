using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class EmployeeSO : ScriptableObject
{
    public Sprite icon;
    public string employeeName;
    public float hireCost;

    public Transform employeePrefab;
}
