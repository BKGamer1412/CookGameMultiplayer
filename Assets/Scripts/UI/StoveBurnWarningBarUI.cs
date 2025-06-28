using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveBurnWarningBarUI : MonoBehaviour
{
    private const string IS_FLASHING = "IsFlashing";
    [SerializeField] private StoveCounter stoveCounter;
    private Animator stoveBurnWarningAnim;

    private void Awake()
    {
        stoveBurnWarningAnim = GetComponent<Animator>();
    }

    private void Start()
    {
        stoveCounter.OnProgressChanged += StoveCounter_Onprogresschanged;
    }

    private void StoveCounter_Onprogresschanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        float burnProgressAmount = .5f;
        bool show = stoveCounter.IsFried() && e.progressNormalized >= burnProgressAmount;

        stoveBurnWarningAnim.SetBool(IS_FLASHING, show);
    }
}
