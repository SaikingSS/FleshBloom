using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compas : MonoBehaviour
{
    public float maxDistance = 1000f;
    public float compasPosFix = 0.5f;
    [SerializeField] private Transform player;
    [SerializeField] private RawImage compasImage;
    [SerializeField] private MapControl map;

    [SerializeField] private GameObject questMarkerPrefab;
    public List<PlayerMarker> questMarker = new List<PlayerMarker>();

   // public PlayerMarker[] autoQuestMarkers;


    float compasUnit;

    private void Start()
    {
        compasUnit = compasImage.rectTransform.rect.width / 360f;

        /*for(int i = 0; autoQuestMarkers.Length > i; i++)
        {
            AddQuestMarker(autoQuestMarkers[i]);
        }*/
    }

    private void Update()
    {
        compasImage.uvRect = new Rect((player.localEulerAngles.y / 360) + compasPosFix, 0f, 1f, 1f);


        for (int i = 0; i < questMarker.Count; i++)
        {
            if(questMarker[i] != null)
            {
                questMarker[i].image.rectTransform.anchoredPosition = GetPosOnCompas(questMarker[i]);

                float dst = Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z), questMarker[i].position);
                float scale = 0f;

                if (dst < maxDistance)
                {
                    scale = 1f - (dst / maxDistance);
                    if (scale < 0.4f)
                    {
                        scale = 0.4f;
                    }
                }
                else
                {
                    scale = 0.4f;
                }

                questMarker[i].image.rectTransform.localScale = Vector3.one * scale;
            }
        }
    }

    public void AddQuestMarker(PlayerMarker marker)
    {
        GameObject newMarker = Instantiate(questMarkerPrefab, compasImage.transform);
        marker.image = newMarker.GetComponent<Image>();
        marker.image.sprite = marker.icon;

        questMarker[map.FindFirstFreeMarker()] = marker;
    }

    Vector2 GetPosOnCompas(PlayerMarker marker)
    {
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 playerFow = new Vector2(player.transform.forward.x, player.transform.forward.z);

        float angle = Vector2.SignedAngle(marker.position - playerPos, playerFow);

        return new Vector2(compasUnit * angle, 0f);
    }
}
