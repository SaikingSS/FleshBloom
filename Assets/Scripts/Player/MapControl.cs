using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MapControl : MonoBehaviour
{
    private Progress progress;
    private PlayerControls inputs; 
    private Transform miniMapCamera, player;
    private Camera miniMapCameraSettings;

    public float zoomInSpeed;
    public float zoomOutSpeed;
    public float moveSpeed;
    public Vector2 zoomBoundries = new Vector2(15f, 125f);
    public int mapRot = 0;

    [Space]
    public float centerOfMapOrthographicSize;
    public Vector3 centerOfMapPos;
    public Vector3 centerOfMapZoom;
    private Vector3 move, moveController;


    private bool zoomIn;
    private bool zoomOut;
    private float startZoom = 50f;

    [SerializeField] private GameObject mapMarker, mapNoteMakerker;
    [SerializeField] private GameObject[] markersInUI;
    [SerializeField] private Transform markersParent;
    [SerializeField] private RawImage mapRawImage;
    [SerializeField] private Sprite[] markerSprites;
    [SerializeField] private Compas compas;
    [SerializeField] private EventSystem eventSystem;
    public LayerMask mapIconLayer;
    public bool[] markerInUse = new bool[5];
    private bool noteMarker, deleteMarker;
    public bool writingMessage;
    private TMP_InputField writingHighlight;
    private bool writingCd;

    #region Controls

    private void Awake()
    {
        inputs = new PlayerControls();

        inputs.Controls.Movement.performed += ctx => move = ctx.ReadValue<Vector2>();
        inputs.Controls.Movement.canceled += ctx => move = Vector2.zero;

        inputs.Controls.CameraMoveController.performed += ctx => moveController = ctx.ReadValue<Vector2>();
        inputs.Controls.CameraMoveController.canceled += ctx => moveController = Vector2.zero;

        inputs.Controls.MapPlayer.started += ctx => SetOnPlayer();
        inputs.Controls.Attack_Stab.started += ctx => MakeMarker();


        inputs.Controls.Sprint.started += ctx => noteMarker = true;
        inputs.Controls.Sprint.canceled += ctx => noteMarker = false;


        inputs.Controls.Jump.started += ctx => deleteMarker = true;
        inputs.Controls.Jump.canceled += ctx => deleteMarker = false;
        //inputs.Controls.MapCenter.started += ctx => CenterMap();

        inputs.Controls.MapZoomIn.started += ctx => zoomIn = true;
        inputs.Controls.MapZoomIn.canceled += ctx => zoomIn = false;

        inputs.Controls.MapZoomOut.started += ctx => zoomOut = true;
        inputs.Controls.MapZoomOut.canceled += ctx => zoomOut = false;

    }

    private void OnEnable()
    {
        inputs.Controls.Enable();
    }

    private void OnDisable()
    {
        inputs.Controls.Disable();
    }

    #endregion

    private void Start()
    {
        progress = GameObject.FindGameObjectWithTag("progress").GetComponent<Progress>();
        miniMapCamera = GameObject.FindGameObjectWithTag("mapCamera").transform;
        miniMapCameraSettings = miniMapCamera.GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //Invoke("SetStartZoom", 0.3f);
    }

    public void SetStartZoom()
    {
        //miniMapCameraSettings.orthographicSize = startZoom;
        if(writingMessage) { return; }

        if (!miniMapCamera)
        {
            miniMapCamera = GameObject.FindGameObjectWithTag("mapCamera").transform;
        }
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        miniMapCamera.transform.position = new Vector3(player.transform.position.x, miniMapCamera.transform.position.y, player.transform.position.z);
    }

    void SetOnPlayer()
    {
        if (writingMessage) { return; }
        if (gameObject.activeInHierarchy)
        {
            miniMapCamera.transform.position = new Vector3(player.transform.position.x, miniMapCamera.transform.position.y, player.transform.position.z);
        }
    }

    /*void CenterMap()
    {
        if (gameObject.activeInHierarchy)
        {
            miniMapCamera.transform.position = centerOfMapPos;
            miniMapCameraSettings.orthographicSize = centerOfMapOrthographicSize;
        }
    }*/

    #region Markers

    public void MakeMarker()
    {
        if (writingMessage) { return; }

        Vector2 mousePos = Input.mousePosition;

        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(mapRawImage.rectTransform, mousePos, null, out localPoint))
            return;

        Rect rect = mapRawImage.rectTransform.rect;
        float u = (localPoint.x - rect.x) / rect.width;
        float v = (localPoint.y - rect.y) / rect.height;

        RenderTexture rt = miniMapCameraSettings.targetTexture;
        Vector3 pixelPos = new Vector3(u * rt.width, v * rt.height, 0f);

        Ray ray = miniMapCameraSettings.ScreenPointToRay(pixelPos);
        RaycastHit hit;


        if (deleteMarker)
        {
            if (Physics.Raycast(ray, out hit, mapIconLayer))
            {
                //Delete from Compas list if its not Note Marker
                if (hit.transform.GetComponent<PlayerMarker>())
                {
                    PlayerMarker findedMaker = hit.transform.GetComponent<PlayerMarker>();

                    for (int i = 0; i < markerInUse.Length; i++)
                    {
                        if (compas.questMarker[i] != null)
                        {
                            if (compas.questMarker[i] == findedMaker)
                            {
                                compas.questMarker[i] = null;
                                markerInUse[i] = false;
                            }
                        }
                    }

                    //Delete from save file too

                    if (findedMaker.image)
                    {
                        Destroy(findedMaker.image.gameObject);
                    }
                    Destroy(hit.transform.gameObject);
                }               
            }

            UpdateMarkersInHud();
            return;
        }
     
        if (Physics.Raycast(ray, out hit))
        {
            int firstNonUseMarker = FindFirstFreeMarker();

            if (!noteMarker && firstNonUseMarker != -1)
            {
                GameObject curMarker = Instantiate(mapMarker, hit.point, Quaternion.identity);
                curMarker.GetComponent<PlayerMarker>().SetIcon(markerSprites[firstNonUseMarker]);
                compas.AddQuestMarker(curMarker.GetComponent<PlayerMarker>());
                curMarker.transform.SetParent(markersParent);
                markerInUse[firstNonUseMarker] = true;
            }
            else if(noteMarker)
            {
                GameObject curMarker = Instantiate(mapNoteMakerker, hit.point, Quaternion.identity);
                curMarker.transform.SetParent(markersParent);

                eventSystem.SetSelectedGameObject(curMarker.GetComponent<PlayerMarker>().message.gameObject);
                writingCd = true;
                StartCoroutine(WrttingCdEnd());
                writingHighlight = eventSystem.currentSelectedGameObject.GetComponent<TMP_InputField>();
                writingMessage = true;
               // curMarker.GetComponent<PlayerMarker>().message.
            }
        }
        UpdateMarkersInHud();
    }

    IEnumerator WrttingCdEnd()
    {
        yield return new WaitForSecondsRealtime(0.1f);

        writingCd = false;
    }

    public int FindFirstFreeMarker()
    {
        for (int i = 0; i < markerInUse.Length; i++)
        {
            if (!markerInUse[i])
            {
                return i;
            }
        }

        return -1;
    }

    void UpdateMarkersInHud()
    {
        for (int i = 0; i < markerInUse.Length; i++)
        {
            markersInUI[i].SetActive(!markerInUse[i]);
        }
    }

    #endregion


    private void LateUpdate()
    {
        if (gameObject.activeInHierarchy)
        {
            if (writingHighlight)
            {
                if (!writingHighlight.isFocused && writingMessage && !writingCd) { Debug.LogError("Remove writing highlight"); writingHighlight = null; writingMessage = false; }
            }
          
        }
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            if (writingMessage) { return; }
            if (move != Vector3.zero || moveController != Vector3.zero)
            {
                Vector3 newPos = Vector3.zero;
                switch (mapRot)
                {
                    case 0:
                        newPos = new Vector3(move.x, 0, move.y) + new Vector3(moveController.x, 0, moveController.y);
                        break;
                    case 1:
                        newPos = new Vector3(move.y, 0, -move.x) + new Vector3(moveController.y, 0, -moveController.x);
                        break;
                    case 2:
                        newPos = new Vector3(-move.x, 0, -move.y) + new Vector3(-moveController.x, 0, -moveController.y);
                        break;
                    case 3:
                        newPos = new Vector3(-move.y, 0, move.x) + new Vector3(-moveController.y, 0,moveController.x);
                        break;
                }
                newPos = newPos.normalized;
                miniMapCamera.transform.position += newPos *  (moveSpeed * miniMapCameraSettings.orthographicSize);
            }


            //Zoom with Keyborad/Gamepad
            if (zoomIn)
            {
                if(zoomBoundries.x < miniMapCameraSettings.orthographicSize)
                {
                    miniMapCameraSettings.orthographicSize -= zoomInSpeed;
                }
            }
            else if (zoomOut)
            {
                if (zoomBoundries.y > miniMapCameraSettings.orthographicSize)
                {
                    miniMapCameraSettings.orthographicSize += zoomInSpeed;
                }
            }

            //Zoom with Mouse
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f)
            {
                if (zoomBoundries.x < miniMapCameraSettings.orthographicSize)
                {
                    miniMapCameraSettings.orthographicSize -= zoomInSpeed * 12f;
                }
            }
            else if (scroll < 0f)
            {
                if (zoomBoundries.y > miniMapCameraSettings.orthographicSize)
                {
                    miniMapCameraSettings.orthographicSize += zoomInSpeed * 12f;
                }
            }
        }
    }
}
