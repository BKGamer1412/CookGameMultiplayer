using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutterCounterVisual : MonoBehaviour
{
    private const string CUT = "Cut";
    [SerializeField] private CuttingCounter cuttingCounter;
    private Animator animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        cuttingCounter.OnCuttingObject += CuttingCounter_OnPlayerGrabbedObject;
    }

    private void CuttingCounter_OnPlayerGrabbedObject(object sender, EventArgs e)
    {
        animator.SetTrigger(CUT);
    }
}
