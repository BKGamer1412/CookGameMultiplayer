using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;

    public static DeliveryManager Instance { get; private set; }
    [SerializeField] private RecipeListSO recipeListSO;
    private List<RecipeSO> waitingRecipeSOList;

    private float spawnRecipeTimer = 4f;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipeSOMax = 4;
    private int recipeDeliveredCount;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one Delivery Manager instance in the scene!");
        }
        Instance = this;

        waitingRecipeSOList = new List<RecipeSO>();
    }
    private void Update()
    {
        //only server can generate recipes
        if (!IsServer)
        {
            return;
        }
        //waiting recipe algorithm
        spawnRecipeTimer -= Time.deltaTime;

        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipeSOMax)
            {
                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
                
                //netcode dont know Scriptable Object
                SpawnWaitingRecipeClientRpc(waitingRecipeSOIndex);

                // waitingRecipeSOList.Add(waitingRecipeSO);

                // OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    [ClientRpc]
    private void SpawnWaitingRecipeClientRpc(int waitingRecipeSOIndex)
    {
        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];
        waitingRecipeSOList.Add(waitingRecipeSO);

        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO recipeSO = waitingRecipeSOList[i];

            if (recipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                //match ingredients amount
                bool plateContainMatchedRecipe = true;

                foreach (KitchenObjectSO recipeKitchenObjectSO in recipeSO.kitchenObjectSOList)
                {
                    bool ingredientFound = false;
                    //cycling through ingredients in recipe
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        //cycling through ingredients in plate
                        if (recipeKitchenObjectSO == plateKitchenObjectSO)
                        {
                            //match ingredient
                            ingredientFound = true;
                            break;
                        }

                    }

                    if (!ingredientFound)
                    {
                        //this recipe ingredients not found on plate
                        plateContainMatchedRecipe = false;
                    }
                }

                if (plateContainMatchedRecipe)
                {
                    //match recipe

                    //host run this code
                    DeliveryCorrectRecipeServerRpc(i);

                    return;
                }

            }
        }

        //no matched recipes
        //player did not deliver the correct recipe
        DeliveryFailedRecipeServerRpc();
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliveryFailedRecipeServerRpc()
    {
        DeliveryFailedRecipeClientRpc();
    }

    [ClientRpc]
    private void DeliveryFailedRecipeClientRpc()
    {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    //sent success request to server
    [ServerRpc(RequireOwnership = false)]
    private void DeliveryCorrectRecipeServerRpc(int waitingRecipeSOIndex)
    {
        DeliveryCorrectRecipeClientRpc(waitingRecipeSOIndex);
    }

    //sync to all client
    [ClientRpc]
    private void DeliveryCorrectRecipeClientRpc(int waitingRecipeSOIndex)
    {
        recipeDeliveredCount++;
        waitingRecipeSOList.RemoveAt(waitingRecipeSOIndex);

        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetRecipeDeliveredCount()
    {
        return recipeDeliveredCount;
    }
}
