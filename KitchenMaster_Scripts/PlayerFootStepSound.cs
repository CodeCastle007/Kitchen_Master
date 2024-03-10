using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootStepSound : MonoBehaviour
{
    [SerializeField] private Player player;
    private float footStepTimer;
    private float footStepTimerMax = .3f;

    private void Update()
    {
        footStepTimer -= Time.deltaTime;
        if (footStepTimer <= 0)
        {
            footStepTimer = footStepTimerMax;

            if (player.IsWalking())
            {
                float volume = .3f;
                SoundManager.Instance.PlayFootStepSound(player.transform.position, volume);
            }
        }
    }
}
