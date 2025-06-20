using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;


    public override void Interact(PlayerController player)
    {
        if (!HasKitchenObject())
        {
            //there is no KitchenObject here
            if (player.HasKitchenObject())
            {
                //player is carrying something

                //get the kitchen object from the player and set it to this counter
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                //player is not carrying anything

            }
        }
        else
        {
            //there is a KitchenObject here
            if (player.HasKitchenObject())
            {
                //player is carrying something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //player hold a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                    }
                }
                else
                {
                    //player holding something
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        //there a plate on counter
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            player.GetKitchenObject().DestroySelf();
                        }

                    }
                }
            }
            else
            {
                //player is not carrying anything

                //give the kitchen object to the player
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}
