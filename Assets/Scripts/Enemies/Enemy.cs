using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;

    public float health;
    public float maxHealth;

    public GameObject lockOnMark;
    public Transform lockOnPoint;
}
