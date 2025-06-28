using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGameObjectArray;
    private void Start()
    {
        if (PlayerController.LocalInstance != null)
        {
            PlayerController.LocalInstance.OnSelectedCounterChanged += PlayerController_OnSelectedCounterChanged;
        }
        else
        {
            PlayerController.OnAnyPlayerSpawned += PlayerController_OnAnyPlayerSpawned;
        }

    }

    private void PlayerController_OnAnyPlayerSpawned(object sender, EventArgs e)
    {
        if (PlayerController.LocalInstance != null)
        {
            //may duplicate base on event so need to unsubcribe first befor subcribe again
            PlayerController.LocalInstance.OnSelectedCounterChanged -= PlayerController_OnSelectedCounterChanged;
            PlayerController.LocalInstance.OnSelectedCounterChanged += PlayerController_OnSelectedCounterChanged;
        }
    }

    private void PlayerController_OnSelectedCounterChanged(object sender, PlayerController.OnSelectedCounterChangedeventArgs e)
    {
        //hightlight the selected counter
        if (e.seletectedCounter == baseCounter)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        foreach (GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(true);
        }
    }

    private void Hide()
    {
        foreach (GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(false);
        }
    }
}
