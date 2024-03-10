using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour,IKitchenObjectParent
{
    private InputHandler inputHandler;
    private bool isWalking;
    private Vector3 lastInteractDirection;
    private BaseCounter selectedCounter;

    private KitchenObject kitchenObject;

    [SerializeField] private Transform objectHoldPoint;
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private float rotationSpeed = 3f;

    public event Action<BaseCounter> OnSelectedCounterChange;
    public event Action<Transform> OnPickSomething;

    public static Player Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        inputHandler = InputHandler.Instance;

        inputHandler.OnInteractPerformed += InputHandler_OnInteractPerformed;
        inputHandler.OnInteractAlternatePerformed += InputHandler_OnInteractAlternatePerformed;
    }

    private void InputHandler_OnInteractAlternatePerformed()
    {
        //If game is not playing then return
        if (!GameHandler.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void InputHandler_OnInteractPerformed()
    {
        //If game is not playing then return
        if (!GameHandler.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleInteraction();
    }

    private void HandleInteraction()
    {
        #region Old Code
        Vector2 movementVector = inputHandler.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(movementVector.x, 0, movementVector.y);

        if (moveDir != Vector3.zero)
        {
            lastInteractDirection = moveDir;
        }

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDirection, out RaycastHit hit, interactDistance))
        {
            if (hit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                SetSelectedCounter(baseCounter);
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
        #endregion

        #region New Code
        //float interactDistance = 1f;
        //Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactDistance);
        //if (colliderArray.Length > 0)
        //{
        //    foreach (Collider collider in colliderArray)
        //    {
        //        if (collider.TryGetComponent(out BaseCounter baseCounter))
        //        {
        //            SetSelectedCounter(baseCounter);
        //            break;
        //        }
        //        else
        //        {
        //            SetSelectedCounter(null);
        //        }
        //    }
        //}
        //else
        //{
        //    SetSelectedCounter(null);
        //}
        #endregion
    }

    private void HandleMovement()
    {
        Vector2 movementVector = inputHandler.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(movementVector.x, 0, movementVector.y);

        float playerHeight = 2f;
        float playerRadius = .3f;
        float distance = movementSpeed * Time.deltaTime;
        Vector3 offset = new Vector3(0, 0.5f, 0);

        bool canMove = !Physics.CapsuleCast(transform.position + offset, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, distance);

        if (!canMove)
        {
            //We cannot move on Move Dir
            //We try X movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = moveDirX!=Vector3.zero && !Physics.CapsuleCast(transform.position + offset, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, distance);

            if (canMove)
            {
                //We can move on X
                moveDir = moveDirX;
            }
            else
            {
                //We cannot move on X
                //Try moving on Z
                //Vector3 offset = new Vector3(0, 0.2f, 0);
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = moveDirZ != Vector3.zero && !Physics.CapsuleCast(transform.position + offset, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, distance);

                if (canMove)
                {
                    //We can move on Z
                    moveDir = moveDirZ;
                }
                else
                {
                    //We cannot move in any direction
                }
            }
        }


        if (canMove)
        {
            transform.position += moveDir * distance;
        }
        

        isWalking = moveDir != Vector3.zero;

        transform.forward = Vector3.Slerp(transform.forward, moveDir, rotationSpeed * Time.deltaTime);

    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        if (this.selectedCounter != selectedCounter)
        {
            this.selectedCounter = selectedCounter;

            OnSelectedCounterChange?.Invoke(this.selectedCounter);
        }
    }



    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject ? true : false;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if (this.kitchenObject != null)
        {
            OnPickSomething?.Invoke(transform);
        }
    }
    public Transform GetKitchenObjectFollowTransform()
    {
        return objectHoldPoint;
    }
}
