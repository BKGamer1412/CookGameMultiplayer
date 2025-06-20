using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private GameObject hasProgressGameObject;
    [SerializeField] private Image barImage;

    private IHasProgress hasProgress;

    private void Awake()
    {
        hasProgress = hasProgressGameObject.GetComponentInParent<IHasProgress>();
        if(hasProgress == null)
        {
            Debug.LogError("Gameobject "+ hasProgressGameObject+" does not implement IHasProgress interface.");
        }
    }
    private void Start()
    {
        hasProgress.OnProgressChanged += HasProgress_OnProgressChanged;
        barImage.fillAmount = 0f; // Initialize the bar to be empty
        Hide(); // Start with the progress bar hidden
    }

    private void HasProgress_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        barImage.fillAmount = e.progressNormalized;
        if (e.progressNormalized >= 1f || e.progressNormalized <= 0f)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
