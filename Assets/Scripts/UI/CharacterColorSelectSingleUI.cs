using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectSingleUI : MonoBehaviour
{
    [SerializeField] private int colorId;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedGameObject;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.Instance.ChangePlayerColor(colorId);
        });
    }

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkChanged += KitchenGameMultiplayer_OnPlayerDataNetworkChanged;
        image.color = KitchenGameMultiplayer.Instance.GetPlayerColor(colorId);
        UpdateIsSelected();
    }

    private void KitchenGameMultiplayer_OnPlayerDataNetworkChanged(object sender, EventArgs e)
    {
        UpdateIsSelected();
    }

    private void UpdateIsSelected()
    {
        if (KitchenGameMultiplayer.Instance.GetPlayerData().colorId == colorId)
        {
            ShowSelected();
        }
        else
        {
            HideSelected();
        }
    }

    private void ShowSelected()
    {
        selectedGameObject.SetActive(true);
    }

    private void HideSelected()
    {
        selectedGameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkChanged -= KitchenGameMultiplayer_OnPlayerDataNetworkChanged;
    }
}
