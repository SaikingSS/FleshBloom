using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    private Progress progress;

    public Enemy enemy;
    [SerializeField] private GameObject enemyBar;
    [SerializeField] private GameObject[] hintModes;
    [SerializeField] private Image hpFill, staminaFill, manaFill, enemyFill;
    [SerializeField] private TextMeshProUGUI enemyName;

    public void Start()
    {
        progress = GameObject.FindGameObjectWithTag("progress").GetComponent<Progress>();
    }

    public void SwitchInputHints(int currentMode)
    {
        for (int i = 0; i < hintModes.Length; i++)
        {
            hintModes[i].SetActive(false);
        }
        //0 - Normal, 1 - Combat, 2 - Grab, 3 - Magic
        hintModes[currentMode].SetActive(true);
    }

    private void LateUpdate()
    {
        hpFill.fillAmount = progress.health / progress.maxHealth;
        staminaFill.fillAmount = progress.stamina / progress.maxStamina;
        manaFill.fillAmount = progress.mana / progress.maxMana;

        if (enemy)
        {
            if(enemyBar.activeInHierarchy == false)
            {
                enemyBar.SetActive(true);
                enemyName.text = enemy.enemyName;
            }
            enemyFill.fillAmount = enemy.health / enemy.maxHealth;
        }
        else if (enemyBar.activeInHierarchy == true)
        {
            enemyBar.SetActive(false);
        }

    }

}
