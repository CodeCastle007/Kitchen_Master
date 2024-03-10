using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";

    private Animator animator;
    private Player player;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        player = Player.Instance;
    }

    private void Update()
    {
        animator.SetBool(IS_WALKING, player.IsWalking());
    }
}
