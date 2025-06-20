using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    private const string NUMBER_POPUP = "NumberPopUp";
    [SerializeField] private TextMeshProUGUI countdownText;
    private Animator countdownAnimator;
    private int previousNumber;

    private void Awake()
    {
        countdownAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;

        Hide();
    }

    private void Update()
    {
        // countdownText.text = KitchenGameManager.Instance.GetCountdownToStartTimer().ToString("#");  //Stringformat: F2, #.###
        int countdownNumber = Mathf.CeilToInt(KitchenGameManager.Instance.GetCountdownToStartTimer());
        countdownText.text = countdownNumber.ToString();

        if (previousNumber != countdownNumber)
        {
            previousNumber = countdownNumber;
            countdownAnimator.SetTrigger(NUMBER_POPUP);
            SoundManager.Instance.PlayCountdownSound();
        }
    }

    private void KitchenGameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (KitchenGameManager.Instance.IsCountdownStartActive())
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
        countdownText.gameObject.SetActive(true);
    }

    private void Hide()
    {
        countdownText.gameObject.SetActive(false);
    }


}
