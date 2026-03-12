using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosTracker : MonoBehaviour
{
    [SerializeField] private Transform cameraPos;

    private void Update()
    {
        transform.position = cameraPos.position;
    }
}
