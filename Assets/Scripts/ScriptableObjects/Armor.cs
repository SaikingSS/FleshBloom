using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item_Armor")]
[System.Serializable]
public class Armor : Item
{
    [Header("Armor")]

    public ArmorType armorType;
}
   
