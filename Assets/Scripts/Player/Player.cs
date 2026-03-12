using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Progress progress;


    [Header("Debug")]

    public bool DEBUG_SuperSprint;

    [Header("Movement")]

    public float airSpeed = 4.5f;
    public float walkSpeed = 4.5f;
    public float sprintSpeed = 7.5f;
    public float crouchSpeed = 2.5f;
    public float sprint_staminaCost = 0.05f;
    public float stamina_regen = 0.05f;
    private float moveSpeed;

    public float groundDrag;
    public float airDrag;

    public float jumpForce;
    public float additonalGravityForce;
    public float jumpCooldown;
    public float airMultyplayer;
    bool readyToJump = true;

    public bool grabbingObject;

    [Header("Dash")]

    public float dashSpeed;
    public float dashCost;
    public float dashDuration;
    public float dashCdTime;
    private bool dashCd;
    private bool dashing;

    [Header("Combat")]

    public bool combatMode;
    [SerializeField] private Animator weaponAnim;
    private bool hostelAnimation;

    [Header("Grounded")]

    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Movement")]
    public bool playerOnSlop;
    public bool playerOnSteepSlop;
    public float maxSlopeAngle;
    //public float dragSlopeAngle;
    //public float currentSlopeAngel;
    public float steepSlopeDrag = 40f;
    private RaycastHit slopeHit;
    private bool exitingSlope;


    [Header("Crouch")]

    public bool crouch;
    public bool crouchLock;
    public bool crouchCd;


    [Header("LockOn")]
    public float lockOnRaycastDistance;
    public LayerMask lockOnRaycastImpactLayers;
    private bool lockOn;

    [SerializeField] private Transform lockedOnEnmey, lockOnPoint;

    [Header("Map")]
    public GameObject mapWindow;
    private MapControl mapControl;


    [Header("Componets")]

    [SerializeField] private GrabingObjects grabingObjects;
    [SerializeField] private Animator camAnimator;
    [SerializeField] private CameraMovement cameraMovement;
    [SerializeField] private Transform orientation, camPointer;
    [SerializeField] private HUD hud;

    float horizontalInput;
    float veritaclInput;

    Vector3 moveDirection;

    Rigidbody rb;


    [Header("Input")]

    private PlayerControls inputs;

    public Vector2 move, moveController;
    private bool sprinButtonHold;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        dashing,
        air
    }

    #region Controls

    private void Awake()
    {
        inputs = new PlayerControls();

        inputs.Controls.Movement.performed += ctx => move = ctx.ReadValue<Vector2>(); 

        inputs.Controls.MovementController.performed += ctx => moveController = ctx.ReadValue<Vector2>();
        inputs.Controls.MovementController.canceled += ctx => moveController = Vector2.zero;

        inputs.Controls.Attack_Normal.started += ctx => AttackHolster();

        inputs.Controls.Jump.performed += ctx => Jump();


        inputs.Controls.Crouch.performed += ctx => Crouch();

        inputs.Controls.Dodge.performed += ctx => Dash();

        inputs.Controls.LockIn.started += ctx => LockOn();

        inputs.Controls.HolsterWeapon.started += ctx => Holster();

        inputs.Controls.Sprint.started += ctx => sprinButtonHold = true;
        inputs.Controls.Sprint.canceled += ctx => sprinButtonHold = false;

        inputs.Controls.Map.started += ctx => Map();
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

    public void Start()
    {
        progress = GameObject.FindGameObjectWithTag("progress").GetComponent<Progress>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        if (DEBUG_SuperSprint) 
        {
            sprintSpeed = 40f; 
        }
        mapControl = mapWindow.GetComponent<MapControl>();
    }

    public void LockOn()
    {
       /* if (!lockOn)
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, lockOnRaycastDistance, lockOnRaycastImpactLayers))
            {
                if (hit.collider.gameObject.CompareTag("enemy"))
                {
                    lockOn = true;

                    //Debug.LogError("Locked on " + hit.transform.gameObject.name);

                    if (lockedOnEnmey)
                    {
                        lockedOnEnmey.GetComponent<Enemy>().lockOnMark.SetActive(false);
                    }

                    lockedOnEnmey = hit.transform;

                    hit.transform.gameObject.GetComponent<Enemy>().lockOnMark.SetActive(true);
                    lockOnPoint = lockedOnEnmey.GetComponent<Enemy>().lockOnPoint;
                    hud.enemy = lockedOnEnmey.GetComponent<Enemy>();
                }
            }
        }
        else
        {
            lockOn = false;
            lockedOnEnmey.GetComponent<Enemy>().lockOnMark.SetActive(false);
            lockedOnEnmey = null;
            hud.enemy = null;
        }*/
    }

    #region Attack

    public void AttackHolster()
    {
        if (!combatMode && !grabingObjects)
        {
            Invoke("Holster", 0.01f);
        }
    }

    #endregion

    #region Jump
    private void Jump()
    {
        if (ActionLockCheck()) { return; }

        if (readyToJump && grounded && !OnSteepSlope())
        {
            readyToJump = false;
            Invoke("ResetJump", jumpCooldown);

            exitingSlope = true;

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    #endregion

    #region Dash

    private void Dash()
    {
        if (ActionLockCheck()) { return; }

        if ((move.x != 0 || move.y != 0 || moveController.x != 0 || moveController.y != 0) && !dashing && progress.stamina > dashCost && !dashCd && grounded)
        {
            dashing = true;
            dashCd = true;
            progress.stamina -= dashCost;
            Invoke("ResetDash", dashDuration);

            //int currentSide = 0; //0 - Forward, 1 - Backward, 2- Right, 3 - Left

            if(move.x + moveController.x > 0) //Right
            {

                camAnimator.SetTrigger("dash_Right");
            }
            else if (move.x + moveController.x < 0)  //Left
            {

                camAnimator.SetTrigger("dash_Left");
            }
            else if (move.y + moveController.y > 0)  //Forward
            {

                camAnimator.SetTrigger("dash_Forwad");
            }
            else if (move.y + moveController.y < 0)  //Backward
            {

                camAnimator.SetTrigger("dash_Backward");
            }
        }
    }
    private void ResetDash()
    {
        dashing = false;
        Invoke("DashCooldown", dashCdTime);
    }
    private void DashCooldown()
    {
        dashCd = false;
    }


    #endregion

    #region Holster

    private void Holster()
    {
        if (ActionLockCheck()) { return; }

        StartCoroutine(HolsterCo());
    }

    IEnumerator HolsterCo()
    {
        if (!grabbingObject && !hostelAnimation)
        {
            hostelAnimation = true;
            if (combatMode)
            {
                weaponAnim.SetTrigger("HideWeapon");
                hud.SwitchInputHints(0);
            }
            else
            {
                weaponAnim.SetTrigger("ShowWeapon");
                hud.SwitchInputHints(1);
            }
            yield return new WaitForSeconds(0.5f);
            combatMode = !combatMode;
            hostelAnimation = false;
            
        }
    }



    #endregion

    #region Crouch

    public void Crouch()
    {
        if (crouch)
        {
            if (!crouchLock)
            {
                crouch = false;
                crouchCd = true;
                LeanTween.moveLocal(camPointer.gameObject, new Vector3(0f, -0.65f, 0f), 0.25f).setEaseOutQuart().setOnComplete(CrouchCdEnd);
              //  characterChar.height = 2.16f; 
              //  characterChar.center = new Vector3(0f, 0.16f, 0f);
            }
        }
        else if (grounded)
        {
            crouch = true;
            crouchCd = true;
            LeanTween.moveLocal(camPointer.gameObject, new Vector3(0f, -1.55f, 0f), 0.25f).setEaseOutQuart().setOnComplete(CrouchCdEnd);
            //characterChar.height = 1.4f;
            // characterChar.center = new Vector3(0f, -0.22f, 0f);
        }
    }

    void CrouchCdEnd()
    {
        crouchCd = false;
    }
    #endregion

    #region UI

    void Map()
    {
        if (!mapControl.writingMessage)
        {
            if (mapWindow.activeInHierarchy)
            {
                Time.timeScale = 1f;
                mapWindow.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Time.timeScale = 0.0001f;
                mapWindow.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    #endregion


    bool ActionLockCheck()
    {
        if (mapWindow.activeInHierarchy) { return true; }

        return false;
    }

    private void Update()
    {
        StateMachineUpdater();
        SpeedControl();
    }

    private void FixedUpdate()
    {
        GroundCheck();

        moveDirection = orientation.forward * (move.y + moveController.y) + orientation.right * (move.x + moveController.x);

        if (OnSteepSlope())
        {
            SteepSlopeCheck();
        }
        playerOnSlop = OnSlope();
        playerOnSteepSlop = OnSteepSlope();
        MovePlayer();
        FallFixer();
        StaminaRegen();
        LockOnLook();
        /*camAngleAnimator.SetFloat("BlendHorizontal", move.x);
        camAngleAnimator.SetFloat("BlendVertical", move.y);
        if(move.x != 0 || move.y != 0)
        {
            camAngleAnimator.SetBool("moving", true);
        }
        else
        {
            camAngleAnimator.SetBool("moving", false);
        }*/
    }

    #region Update Functions
    private void MovePlayer()
    {
        if (OnSlope())
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if(rb.linearVelocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        else if (grounded)
        {
            float curMoveSpeed = moveSpeed;
            if (crouch) { curMoveSpeed = curMoveSpeed * 0.45f; }

            rb.AddForce(moveDirection.normalized * curMoveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultyplayer, ForceMode.Force);
        }

        if (grounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = airDrag;
        }

        rb.useGravity = !OnSlope();
    }

    private void StateMachineUpdater()
    {
        if (dashing)
        {
            state = MovementState.dashing;
            moveSpeed = dashSpeed;
        }
        else if (grounded && sprinButtonHold && progress.stamina > 0f && !crouch)
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
            progress.stamina -= sprint_staminaCost * Time.deltaTime;
            if(progress.stamina < 0)
            {
                progress.stamina = 0;
            }
            
        }
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }

    private void StaminaRegen()
    {
        if(progress.stamina < progress.maxStamina && (state != MovementState.sprinting || state != MovementState.air) && !dashCd)
        {
            progress.stamina += stamina_regen * Time.deltaTime;
        }
    }

    private void GroundCheck()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
    }

    private void SpeedControl()
    {
        if(OnSlope() && !exitingSlope)
        {
            if(rb.linearVelocity.magnitude > moveSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
            }
        }
        else
        {
            Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (flatVelocity.magnitude > moveSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
            }
        }
    }

    private void FallFixer()
    {
        if (!grounded/* && rb.velocity.y < 0*/)
        {
            rb.AddForce(Vector3.down * additonalGravityForce, ForceMode.Force);
        }
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            //currentSlopeAngel = angle;
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private bool OnSteepSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle > maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private void SteepSlopeCheck()
    {
        Vector3 slopeDirection = Vector3.up - slopeHit.normal * Vector3.Dot(Vector3.up, slopeHit.normal);
        float slideSpeed = (moveSpeed + steepSlopeDrag) * Time.deltaTime;

        moveDirection = slopeDirection * -slideSpeed;
        moveDirection.y = moveDirection.y - slopeHit.point.y;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    private void LockOnLook()
    {
        if (lockOn)
        {
            cameraMovement.transform.LookAt(lockOnPoint);

            var lookPos = lockOnPoint.position - orientation.transform.position;
            lookPos.y = 0;
            if (Quaternion.LookRotation(lookPos) != new Quaternion(0f, 0f, 0f, 0f))
            {
                var rotation = Quaternion.LookRotation(lookPos);
                orientation.transform.rotation = Quaternion.Slerp(orientation.transform.rotation, rotation, Time.deltaTime * 9999f);
            }
            if (cameraMovement.lockOn == false)
            {
                cameraMovement.lockOn = true;
            }
        }
        else
        {
            if (cameraMovement.lockOn == true)
            {
                cameraMovement.lockOn = false;
            }
        }
    }
   
    #endregion
}
