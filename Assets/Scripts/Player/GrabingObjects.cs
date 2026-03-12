using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabingObjects : MonoBehaviour
{
    private PlayerControls inputs;

    [SerializeField] private GameObject player;
    [SerializeField] private Player playerScript;
    [SerializeField] private Transform holdPos;
    [SerializeField] private CameraMovement cameraMovement;
    [SerializeField] private HUD hud;

    //if you copy from below this point, you are legally required to like the video
    public float throwForce = 500f; //force at which the object is thrown at
    public float pickUpRange = 5f; //how far the player can pickup the object from
    private float rotationSensitivity = 1f; //how fast/slow the object is rotated in relation to mouse movement
    private GameObject heldObj; //object which we pick up
    private Rigidbody heldObjRb; //rigidbody of object we pick up
    private bool canDrop = true; //this is needed so we don't throw/drop object when rotating the object
    private int LayerNumber; //layer index
    private int prewLayerOfHoldedObject;

    private bool rotatingButtonHold;
    private Vector2 camMove, camMoveController;

    #region Controls

    private void Awake()
    {
        inputs = new PlayerControls();

        //Throw
        inputs.Controls.Attack_Heavy.performed += ctx => Throw();

        //Grab
        inputs.Controls.Attack_Normal.performed += ctx => GrabObject();

        //RotateObjects
        inputs.Controls.Block.started += ctx => rotatingButtonHold = true;
        inputs.Controls.Block.canceled += ctx => rotatingButtonHold = false;


        inputs.Controls.CameraMove.performed += ctx => camMove = ctx.ReadValue<Vector2>();
        inputs.Controls.CameraMove.canceled += ctx => camMove = Vector2.zero;

        inputs.Controls.CameraMoveController.performed += ctx => camMoveController = ctx.ReadValue<Vector2>();
        inputs.Controls.CameraMoveController.canceled += ctx => camMoveController = Vector2.zero;
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


    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("HoldLayer"); 
    }

    void GrabObject()
    {
        if (heldObj == null && !playerScript.combatMode) //if currently not holding anything
        {
            //perform raycast to check if player is looking at object within pickuprange
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
            {
                //make sure pickup tag is attached
                if (hit.transform.gameObject.tag == "grabable")
                {
                    //pass in object hit into the PickUpObject function
                    PickUpObject(hit.transform.gameObject);
                }
            }
        }
        else
        {
            if (canDrop == true && !playerScript.combatMode)
            {
                StopClipping(); //prevents object from clipping through walls
                DropObject();
            }
        }
    }

    void Throw()
    {
        if(heldObj != null && canDrop == true)
        {
            StopClipping();
            ThrowObject();
        }
    }

    void Update()
    {
    
        if (heldObj != null) //if player is holding object
        {
            MoveObject(); //keep object position at holdPos
            RotateObject();         
        }
    }
    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>()) //make sure the object has a RigidBody
        {
            heldObj = pickUpObj; //assign heldObj to the object that was hit by the raycast (no longer == null)
            heldObjRb = pickUpObj.GetComponent<Rigidbody>(); //assign Rigidbody
            heldObjRb.isKinematic = true;
            heldObjRb.transform.parent = holdPos.transform; //parent object to holdposition
            prewLayerOfHoldedObject = heldObj.layer;
            heldObj.layer = LayerNumber; //change the object layer to the holdLayer
            //make sure object doesnt collide with player, it can cause weird bugs
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
            hud.SwitchInputHints(2);
            playerScript.grabbingObject = true;
        }
    }
    void DropObject()
    {
        //re-enable collision with player
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);

        heldObj.layer = prewLayerOfHoldedObject;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null; //unparent object
        heldObj = null; //undefine game object
        hud.SwitchInputHints(0);
        Invoke("noGrabing", 0.1f);
    }
    void MoveObject()
    {
        //keep object position the same as the holdPosition position
        heldObj.transform.position = holdPos.transform.position;
    }
    void RotateObject()
    {
        if (rotatingButtonHold)//hold R key to rotate, change this to whatever key you want
        {
            canDrop = false; //make sure throwing can't occur during rotating

            //disable player being able to look around
            //mouseLookScript.verticalSensitivity = 0f;
            //mouseLookScript.lateralSensitivity = 0f;

            float XaxisRotation = (camMove.x + camMoveController.x) * rotationSensitivity;
            float YaxisRotation = (camMove.y + camMoveController.y) * rotationSensitivity;
            //rotate the object depending on mouse X-Y Axis
            heldObj.transform.Rotate(Vector3.down, XaxisRotation);
            heldObj.transform.Rotate(Vector3.right, YaxisRotation);
            cameraMovement.rotatingGrabbedObject = true;
        }
        else
        {
            //re-enable player being able to look around
            //mouseLookScript.verticalSensitivity = originalvalue;
            //mouseLookScript.lateralSensitivity = originalvalue;
            cameraMovement.rotatingGrabbedObject = false;
            canDrop = true;
        }
    }
    void ThrowObject()
    {
        //same as drop function, but add force to object before undefining it
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = prewLayerOfHoldedObject;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObjRb.AddForce(transform.forward * throwForce);
        heldObj = null;
        hud.SwitchInputHints(0);
        Invoke("noGrabing", 0.1f);
    }
    void StopClipping() //function only called when dropping/throwing
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position); //distance from holdPos to the camera
        //have to use RaycastAll as object blocks raycast in center screen
        //RaycastAll returns array of all colliders hit within the cliprange
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        //if the array length is greater than 1, meaning it has hit more than just the object we are carrying
        if (hits.Length > 1)
        {
            //change object position to camera position 
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f); //offset slightly downward to stop object dropping above player 
            //if your player is small, change the -0.5f to a smaller number (in magnitude) ie: -0.1f
        }
    }

    void noGrabing()
    {
        playerScript.grabbingObject = false;
    }
}