using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderCallback : MonoBehaviour
{

    //this class to make sure everything is done loading and it will do callback on First Update frame
    private bool isFirstUpdate = true;

    private void Update()
    {
        if (isFirstUpdate)
        {
            isFirstUpdate = false;

            Loader.LoaderCallback();
        }
    }
}
