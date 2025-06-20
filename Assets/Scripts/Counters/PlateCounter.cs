using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlateCounter : BaseCounter
{
    public event EventHandler OnPlateTaken;
    public event EventHandler OnPlateSpawned;
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int plateSpawnAmount = 0;
    private int plateSpawnAmountMax = 5;

    private void Update()
    {
        spawnPlateTimer += Time.deltaTime;

        if (spawnPlateTimer > spawnPlateTimerMax)
        {
            spawnPlateTimer = 0f;
            // KitchenObject.SpawnKitchenObject(kitchenObjectSO, this);
            if (KitchenGameManager.Instance.IsGamePlaying() && plateSpawnAmount < plateSpawnAmountMax)
            {
                plateSpawnAmount++;

                OnPlateSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    
    public override void Interact(PlayerController player)
    {
        if (!player.HasKitchenObject())
        {
            //player is not carrying anything

            if (plateSpawnAmount > 0)
            {
                //there at least one plate to take
                plateSpawnAmount--;
                KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);

                OnPlateTaken?.Invoke(this, EventArgs.Empty);

            }
        }
    }
}
