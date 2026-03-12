using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Item")]
[System.Serializable]
public class Item : ScriptableObject
{
    [Header("General")]

    public int itemID;
    public string itemName;
    public string itemDesctiption;

    public ItemType itemType;


    public float weight;

    public Sprite itemIcon;
    public GameObject objectWhenDroped;

}
