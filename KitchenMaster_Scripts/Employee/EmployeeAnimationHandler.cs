using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EmployeeAnimationHandler : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";

    private Animator animator;
    private Employee employee;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        employee = GetComponent<Employee>();
    }

    private void Update()
    {
        animator.SetBool(IS_WALKING, employee.IsWalking());
    }
}
