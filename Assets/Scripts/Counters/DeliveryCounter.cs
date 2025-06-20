using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using System.Diagnostics;

public class DeliveryCounter : BaseCounter
{
    public static DeliveryCounter Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    public override void Interact(PlayerController player)
    {
        if (player.HasKitchenObject())
        {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                //only accepts Plates

                DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);


                player.GetKitchenObject().DestroySelf();

            }
        }
    }
}
