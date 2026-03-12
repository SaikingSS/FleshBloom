using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventorySaveMenager : MonoBehaviour
{
    private Progress progress;

    [Header("Debug")]

    public bool dontLoad;

   /* [Header("Collected")]
    public List<int> inv_weapons = new List<int>();
    public List<int> inv_usable = new List<int>();
    public List<int> inv_runes = new List<int>();
    public List<int> inv_materials = new List<int>();
    public List<int> inv_keys = new List<int>();
    public List<int> inv_rings = new List<int>();

    [Header("Prefabs")]

    public List<GameObject> pref_weapons = new List<GameObject>();
    public List<GameObject> pref_usable = new List<GameObject>();
    public List<GameObject> pref_runes = new List<GameObject>();
    public List<GameObject> pref_materials = new List<GameObject>();
    public List<GameObject> pref_keys = new List<GameObject>();
    public List<GameObject> pref_rings = new List<GameObject>();*/


    private void Start()
    {
        progress = GetComponent<Progress>();
        if (progress.loadOnEnable)
        {
            LoadInventory();
        }
    }

    public void LoadInventory()
    {
        /*if (!progress.firstOpen)
        {
            if (!dontLoad)
            {
                inv_weapons = FileHandler.ReadListFromJSON<int>("iw.sv" + progress.currentSaveFile);
                inv_usable = FileHandler.ReadListFromJSON<int>("iu.sv" + progress.currentSaveFile);
                inv_runes = FileHandler.ReadListFromJSON<int>("ir.sv" + progress.currentSaveFile);
                inv_materials = FileHandler.ReadListFromJSON<int>("im.sv" + progress.currentSaveFile);
                inv_keys = FileHandler.ReadListFromJSON<int>("ik.sv" + progress.currentSaveFile);
                inv_rings = FileHandler.ReadListFromJSON<int>("rg.sv" + progress.currentSaveFile);

                //Compatiblity Test
                if(inv_weapons.Count < pref_weapons.Count)
                {
                    while(inv_weapons.Count != pref_weapons.Count)
                    {
                        inv_weapons.Add(-1);
                    }
                }
            }
        }*/
    }

    public void SaveLevels()
    {
        /*FileHandler.SaveToJSON<int>(inv_weapons, "iw.sv" + progress.currentSaveFile);
        FileHandler.SaveToJSON<int>(inv_usable, "iu.sv" + progress.currentSaveFile);
        FileHandler.SaveToJSON<int>(inv_runes, "ir.sv" + progress.currentSaveFile);
        FileHandler.SaveToJSON<int>(inv_materials, "im.sv" + progress.currentSaveFile);
        FileHandler.SaveToJSON<int>(inv_keys, "ik.sv" + progress.currentSaveFile);
        FileHandler.SaveToJSON<int>(inv_rings, "rg.sv" + progress.currentSaveFile);*/
    }

    private void OnDisable()
    {
        //SaveLevels();
    }
}
