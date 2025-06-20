using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI keyMoveUpText;
    [SerializeField] TextMeshProUGUI keyMoveDownText;
    [SerializeField] TextMeshProUGUI keyMoveLeftText;
    [SerializeField] TextMeshProUGUI keyMoveRightText;
    [SerializeField] TextMeshProUGUI keyInteractText;
    [SerializeField] TextMeshProUGUI keyInteractAltText;
    [SerializeField] TextMeshProUGUI keyPauseText;
    [SerializeField] TextMeshProUGUI keyGamepad_InteractText;
    [SerializeField] TextMeshProUGUI keyGamepad_InteractAltText;
    [SerializeField] TextMeshProUGUI keyGamepad_PauseText;

    private void Start()
    {
        GameInput.Instance.OnBindingRebind += GameInput_OnBindingRebind;
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        Show();
        UpdateVisual();
    }

    private void KitchenGameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (KitchenGameManager.Instance.IsCountdownStartActive())
        {
            Hide();
        }
    }

    private void GameInput_OnBindingRebind(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        keyMoveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        keyMoveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
        keyMoveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
        keyMoveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
        keyInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        keyInteractAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlt);
        keyPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
        keyGamepad_InteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamePad_Interact);
        keyGamepad_InteractAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamePad_InteractAlt);
        keyGamepad_PauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamePad_Pause);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
