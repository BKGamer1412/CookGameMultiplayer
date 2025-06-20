using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCounterVisual : MonoBehaviour
{
    [SerializeField] private PlateCounter plateCounter;
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private Transform plateVisualPrefab;
    private float plateOffsetY = .1f;

    private List<GameObject> plateVisualGameObjectList;

    private void Awake()
    {
        plateVisualGameObjectList = new List<GameObject>();
    }
    private void Start()
    {
        plateCounter.OnPlateSpawned += PlateCounter_OnPlateSpawned;
        plateCounter.OnPlateTaken += PlateCounter_OnPlateTaken;
    }

    private void PlateCounter_OnPlateTaken(object sender, EventArgs e)
    {
        GameObject plateGameObject = plateVisualGameObjectList[plateVisualGameObjectList.Count - 1];
        plateVisualGameObjectList.Remove(plateGameObject);
        Destroy(plateGameObject);   
    }

    private void PlateCounter_OnPlateSpawned(object sender, EventArgs e)
    {
        Transform plateVisualTransform = Instantiate(plateVisualPrefab, counterTopPoint);

        plateVisualTransform.localPosition = new Vector3(0f, plateOffsetY * plateVisualGameObjectList.Count, 0f);

        plateVisualGameObjectList.Add(plateVisualTransform.gameObject);
        
    }
}
