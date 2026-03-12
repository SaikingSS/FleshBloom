using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestoryer : MonoBehaviour
{
    public float destroyTime;

    void Start()
    {
        Destroy(this.gameObject, destroyTime);
    }
}
