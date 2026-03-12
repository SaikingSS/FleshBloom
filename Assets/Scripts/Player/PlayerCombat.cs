using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Player playerScript;
    [SerializeField] private Animator animator;

    public float heavyThreshold = 0.4f;

    public AttackState currentState = AttackState.Idle;

    public float holdTime;
    public bool isHolding;

    public  bool inputBuffered;

    void Update()
    {
        if (playerScript.combatMode)
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            holdTime = 0;
            isHolding = true;

            if (currentState != AttackState.Idle)
                inputBuffered = true;
        }

        if (isHolding)
        {
            holdTime += Time.deltaTime;

            if (holdTime > heavyThreshold && currentState == AttackState.Idle)
            {
                StartHeavyCharge();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isHolding = false;

            if (currentState == AttackState.HeavyCharge)
            {
                ReleaseHeavy();
            }
            else if (currentState == AttackState.Idle)
            {
                LightAttack1();
            }
        }
    }

    void LightAttack1()
    {
        currentState = AttackState.Light1;
        animator.SetTrigger("LightAttack");
    }

    public void ComboWindow()
    {
        if (inputBuffered)
        {
            inputBuffered = false;
            LightAttack2();
        }
    }

    void LightAttack2()
    {
        currentState = AttackState.Light2;
        animator.SetBool("NextLightAttack", true);
    }

    void StartHeavyCharge()
    {
        currentState = AttackState.HeavyCharge;
        animator.Play("weaponHeavyStart");
    }

    void ReleaseHeavy()
    {
        currentState = AttackState.HeavyRelease;
        animator.SetTrigger("HeavyRelease");
    }

    public void EndAttack()
    {
        currentState = AttackState.Idle;
        animator.SetBool("NextLightAttack", false);
    }
}

public enum AttackState
{
    Idle,
    Light1,
    Light2,
    HeavyCharge,
    HeavyRelease
}