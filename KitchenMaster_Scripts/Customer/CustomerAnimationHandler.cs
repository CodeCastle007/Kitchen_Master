using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerAnimationHandler : MonoBehaviour
{
    private Animator animator;
    private Customer customer;

    private const string IS_WALKING = "IsWalking";
    private const string SITTING = "Sitting";

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        customer = GetComponent<Customer>();
    }

    private void Start()
    {
        customer.OnStateChange += Customer_OnStateChange;
    }

    private void Customer_OnStateChange(Customer.CustomerState state)
    {
        switch (state)
        {
            case Customer.CustomerState.Idle:
                animator.SetBool(IS_WALKING, false);
                animator.SetBool(SITTING, false);
                break;

            case Customer.CustomerState.Walking:
                animator.SetBool(IS_WALKING, true);
                break;

            case Customer.CustomerState.Sitting:
                animator.SetBool(IS_WALKING, false);
                animator.SetBool(SITTING, true);
                break;

            case Customer.CustomerState.Standing:
                animator.SetBool(SITTING, false);
                animator.SetBool(IS_WALKING, true);
                break;
        }

    }
}
