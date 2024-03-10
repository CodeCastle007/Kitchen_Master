using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerCounter : BaseCounter
{
    public static ManagerCounter Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public event Action OnManagerCounterInteraction;


    public override void Interact(IKitchenObjectParent player)
    {
        OnManagerCounterInteraction?.Invoke();
    }
}
