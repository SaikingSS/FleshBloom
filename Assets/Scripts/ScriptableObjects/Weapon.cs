using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Item_Weapon")]
[System.Serializable]
public class Weapon : Item
{
    [Header("Weapon")]

    public WeaponType weaponType;
}
