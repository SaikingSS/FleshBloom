using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public float maxHealth;
    public float maxMana;

    public string characteName;
    public int characteLevel;
    public string characteClass;
    public int learningPoints;

    public int bs_strength;
    public int bs_dexterity;
    public int bs_intelligenc;


    public bool firstOpen;




    public GameData (Progress progress)
    {
        firstOpen = progress.firstOpen;
    }
}
