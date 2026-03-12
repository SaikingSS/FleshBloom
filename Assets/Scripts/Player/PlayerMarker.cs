using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMarker : MonoBehaviour
{
    public Sprite icon;
    public Image image;
    public TMP_InputField message;

    public Vector2 position { get { return new Vector2(transform.position.x, transform.position.z); } }

    public void SetIcon(Sprite newIcon)
    {
        icon = newIcon;

        if (transform.childCount > 0)
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = icon;
        }
    }
}
