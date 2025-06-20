using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField] float volume = 1f;
    private PlayerController playerController;
    private float footstepTimer;
    private float footstepTimerMax = 4f;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        footstepTimer -= Time.deltaTime;
        if (footstepTimer < 0f)
        {
            footstepTimer = footstepTimerMax;

            if (playerController.IsWalking())
            {
                SoundManager.Instance.PlayFootstepSound(playerController.transform.position, volume);
            }
        }
    }
}
