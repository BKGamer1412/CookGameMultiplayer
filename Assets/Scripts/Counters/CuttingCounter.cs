using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public static event EventHandler OnAnyCut;
    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCuttingObject;

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    private int cuttingProgress;

    public override void Interact(PlayerController player)
    {
        if (!HasKitchenObject())
        {
            //there is no KitchenObject here
            if (player.HasKitchenObject() && HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
            {
                //player is carrying something and it is cuttable

                //get the kitchen object from the player and set it to this counter
                player.GetKitchenObject().SetKitchenObjectParent(this);
                cuttingProgress = 0; // Reset cutting progress when a new object is placed
                ProgressChanged(this);
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
            }
            else
            {
                //player is not carrying anything

                //if the object still cuttable and player want to pick up, reset the cutting progress
                cuttingProgress = 0;
                ProgressChanged(this);

                //give the kitchen object to the player
                GetKitchenObject().SetKitchenObjectParent(player);

            }
        }
    }

    public override void InteractAlternate(PlayerController player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            cuttingProgress++;

            OnCuttingObject?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);

            //there is a KitchenObject here and is cuttable
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
            ProgressChanged(this);

            //Invoke cutting event for animation

            if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
            {
                KitchenObjectSO kitchenObjectOutput = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(kitchenObjectOutput, this);
            }
        }
        else
        {
            //there is no KitchenObject here
        }
    }

    public void ProgressChanged(CuttingCounter cuttingCounter)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(cuttingCounter.GetKitchenObject().GetKitchenObjectSO());

        if (cuttingRecipeSO != null)
        {
            //Invoke the progress changed event for UI
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = (float)cuttingCounter.cuttingProgress / cuttingRecipeSO.cuttingProgressMax
            });
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        return cuttingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        if (cuttingRecipeSO != null)
        {
            return cuttingRecipeSO.output;
        }
        return null;
    }

    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipeSO;
            }
        }
        return null;
    }
}
