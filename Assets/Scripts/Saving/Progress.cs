using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Progress : MonoBehaviour
{
    private PlayerControls inputs;

    [Header("Base Stats")]

    public string characteName;
    public string characteClass;
    public int characteLevel;
    public int learningPoints;

    public int bs_strength;
    public int bs_dexterity;
    public int bs_constitution;
    public int bs_intelligenc;
    public int bs_force;
    public int bs_resistance;

    [Header("Character Stats")]

    public float health;
    public float maxHealth;
    public float stamina;
    public float maxStamina;
    public float mana;
    public float maxMana;

    public float loadOut;
    public float maxLoadOut;


    public int gold;

    [Header("Others")]

    public bool firstOpen = true;
    public bool lastClickWithGamepad = false;
    public bool loadOnEnable;
    public int currentSaveFile;


    [Header("Componets Bank")]

    public GameObject defaultDestrucionParticles;


    public static Progress Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        /*inputs = new Controls();

        inputs.PlayerMovement.ChangeInputHintsController.performed += ctx => ChangeInputs(true);
        inputs.PlayerMovement.ChangeInputHintsKeyboard.performed += ctx => ChangeInputs(false);*/
    }

    public void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        //Invoke("FirstOpenDealy", 0.1f);

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        //Time.timeScale = 0.25f;

        #region LevelCupMaker
        //Level Tests
        /*int power = 300;
        float powerCupModiffier = 1.15f;
        int collectiveExp=0;
        levelCaps.Add(0);
        levelCaps.Add(power);

        for (int i=1; i < 588; i++)
        {
            switch (i)
            {
                case 20:
                    powerCupModiffier = 1.06f;
                    break;
                case 40:
                    powerCupModiffier = 1.05f;
                    break;
                case 60:
                    powerCupModiffier = 1.04f;
                    break;
                case 80:
                    powerCupModiffier = 1.03f;
                    break;
                case 100:
                    powerCupModiffier = 1.015f;
                    break;
                case 120:
                    powerCupModiffier = 1.012f;
                    break;
                case 140:
                    powerCupModiffier = 1.008f;
                    break;
                case 160:
                    powerCupModiffier = 1.005f;
                    break;
                case 200:
                    powerCupModiffier = 1.003f;
                    break;
            }
            float nextPowerCup = power * powerCupModiffier;
            power = (int)nextPowerCup;
            levelCaps.Add(power);
            collectiveExp += power;
        }
        Debug.Log("CollectiveExp" + collectiveExp);*/
        #endregion

        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
    }

    private void OnDisable()
    {
        //SaveSytem.SaveGame(this,"/mgs.save");
    }

    private void OnEnable()
    {
        /*if (loadOnEnable)
        {
            LoadGame();
        } */     
    }

    public void LoadGame()
    {
        /*if (SaveSytem.LoadGame(currentSaveFile) != null)
        {

            GameData data = SaveSytem.LoadGame(currentSaveFile);

            firstOpen = data.firstOpen;

            maxHealth = data.maxHealth;
            maxMana = data.maxMana;

            characteName = data.characteName;
            characteClass = data.characteClass;
            characteLevel = data.characteLevel;
            learningPoints = data.learningPoints;
            currentExp = data.currentExp;

            bs_strength = data.bs_strength;
            bs_dexterity = data.bs_dexterity;
            bs_intelligenc = data.bs_intelligenc;

        }*/
    }



    /*void ChangeInputs(bool useGamepad)
    {

        usingUIwithMouse = false;
        if (useGamepad)
        {
            lastClickWithGamepad = true;
            if (inUI)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        else
        {
            lastClickWithGamepad = false;
        }
    }

    private void LateUpdate()
    {
        if((Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) && inUI)
        {
            usingUIwithMouse = true; 
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }*/

}
public enum ItemType
{
    Weapon,
    Armor,
    Usable,
    Material,
    Key,
    Rune
}
public enum WeaponType
{
    OneHand,
    TwoHand,
    OffHand
}
public enum ArmorType
{
    Helmeth,
    Torso,
    Gloves,
    Boots,
    Amulet,
    Ring
}
