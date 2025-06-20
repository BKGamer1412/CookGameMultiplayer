using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//SO = Scriptable Object
// KitchenObjectSO is a base class for kitchen objects in the game.
[CreateAssetMenu()]
public class KitchenObjectSO : ScriptableObject
{
    public Transform prefab; // The prefab associated with this kitchen object.
    public Sprite sprite; // The sprite representing this kitchen object in the UI.
    public string objectName; // The name of the kitchen object.



}
