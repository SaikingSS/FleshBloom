using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item_Usable")]
[System.Serializable]
public class Usable : Item
{
    [Header("Usable")]

    public GameObject effect;
}
