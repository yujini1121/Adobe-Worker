using Cinemachine;
using MalbersAnimations.Controller;
using System;
using System.Text;
using Unity.Mathematics;
using UnityEngine;

public class AdobePlayerController : MonoBehaviour
{
    [Header("Cinemachine Camera")]
	private float keyHorizontalAxisValue;
	private float keyVerticalAxisValue;

	[SerializeField] private GameObject virtualCamTarget;
	[SerializeField] private float virtualCamPitchTop = 70.0f;
	[SerializeField] private float virtualCamPitchBottom = -30.0f;
	private float virtualCamYaw;
    private float virtualCamPitch;


    [Header("Move Value")]
	[SerializeField] private float rotationSmoothTime = 0.12f;
	[SerializeField] private float moveSpeed = 10.0f;
	private float targetRotation = 0f;
    private float rotationVelocity = 0f;
	private Rigidbody rb;
    Vector3 targetDirection;

    [Header("Dash Value")]
    [SerializeField] private bool isDash = false;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooltime;
    [SerializeField] private Vector3 delayedForceToApply;
    [SerializeField] private float afterDashPower = 0;
    [SerializeField] private float dashDecelete = 0;
    private bool dashInput;

    [Header("Dash Value")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float maxStamina;
    [SerializeField] private float healthRegainPerSecondWhenStaminaFull;
    private float health;
    private float stamina;

    [Header("Debugging")]
    [SerializeField] private bool isDebugging;
    [SerializeField] private bool isDebuggingDash;
    [SerializeField] private bool isDebuggingHeal;

    // other component
    AdobeItemPack inventory;
    PlayerMovement playerMovement;

    public void GetHurt(float damage)
    {
        health -= damage;

        if (health < 0)
        {
            DoWhenDead();
        }
    }

    public void Heal(float addHealth, float addStamina)
    {
        health = Mathf.Min(health + addHealth, maxHealth);
        stamina = Mathf.Min(stamina + addStamina, maxStamina);

        if (isDebuggingHeal)
        {
            Debug.Log($"Heal({addHealth}, {addStamina}) : {health} {stamina}");
        }
    }

    public void HealStamina(float addStamina, float staminaLimit)
    {
        stamina = Mathf.Min(stamina + addStamina, staminaLimit);
        if (isDebuggingHeal)
        {
            Debug.Log($"HealStamina({addStamina}, {addStamina}) : {health} {stamina}");
        }
    }

    public bool DemandStamina(float demand)
    {
        if (demand > stamina)
        {
            return false;
        }
        stamina -= demand;
        return true;
    }

    private void Awake()
    {
        health = maxHealth;
        stamina = maxStamina;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        inventory = GetComponent<AdobeItemPack>();
        playerMovement = GetComponent<PlayerMovement>();

        //Debug.LogWarning("Mouse cursor is locked");
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
		PlayerMove();

        UseItems();
        SwitchItems();
        ShowInventory();
        PlayerStatusManager();
        m_HurtPlayer();
    }

    private void LateUpdate()
    {
        PlayerRotation();
    }

    private void FixedUpdate()
    {
        PlayerDash();
    }

    void PlayerMove()
    {
        if (playerMovement.IsActionable() == false)
        {
            rb.velocity *= 0.95f;
            return;
        }
        playerMovement.DoMove();

        keyHorizontalAxisValue = Input.GetAxisRaw("Horizontal");
        keyVerticalAxisValue = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(keyHorizontalAxisValue, 0.0f, keyVerticalAxisValue).normalized;

        if (inputDir.sqrMagnitude > 0)
        {
            targetRotation = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);

            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward + transform.TransformDirection(new Vector3(0f, 0f, afterDashPower)).normalized;

        rb.velocity = targetDirection * (inputDir != Vector3.zero ? moveSpeed : 0.0f)
                        + new Vector3(0.0f, rb.velocity.y, 0.0f);
    }

    void PlayerDash()
    {
        if (isDebuggingDash)
        {
            Debug.Log(dashCooltime);
        }

        //if (dashCooltime > 0) return;
        if (isDash = false && Input.GetKeyDown(KeyCode.LeftShift))
        {
            isDash = true;
            
            rb.velocity = targetDirection * dashForce * 0.98f;
        }
    }

    void PlayerRotation()
    {
        Vector2 inputMousePos = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        if (inputMousePos.sqrMagnitude >= 0.01f) //지터링 방지
        {
            virtualCamYaw += inputMousePos.x;
            virtualCamPitch += inputMousePos.y;
        }

        virtualCamYaw = ClampAngle(virtualCamYaw, float.MinValue, float.MaxValue);
        virtualCamPitch = ClampAngle(virtualCamPitch, virtualCamPitchBottom, virtualCamPitchTop);

        virtualCamTarget.transform.rotation = Quaternion.Euler(-virtualCamPitch, virtualCamYaw, 0.0f);
    }

    float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    void UseItems()
    {
        if (playerMovement.IsActionable() == false)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0) == false)
        {
            return;
        }

        playerMovement.DoAction();

        AdobeItemUseArguments args = new AdobeItemUseArguments();
        args.itemUser = gameObject;
        args.direction = transform.forward;
        args.rotation = transform.rotation;
        inventory.Use(args);
    }

    void SwitchItems()
    {
        if (isDebugging)
        {
            Debug.Log($">> {Input.mouseScrollDelta}");
        }

        if (Input.mouseScrollDelta.y > 0.5f)
        {
            inventory.SwitchItem(-1);
            Debug.Log($"아이템을 바꾸었습니다. 순서 : {inventory.InventoryIndex} {inventory.inventory[inventory.InventoryIndex].id}");
        }
        if (Input.mouseScrollDelta.y < -0.5f)
        {
            inventory.SwitchItem(1);
            Debug.Log($"아이템을 바꾸었습니다. 순서 : {inventory.InventoryIndex} {inventory.inventory[inventory.InventoryIndex].id}");
        }

    }

    void ShowInventory()
    {
        if (Input.GetKeyDown(KeyCode.I) == false)
        {
            return;
        }

        StringBuilder answer = new StringBuilder();
        foreach (AdobeItemBase item in inventory.inventory)
        {
            answer.AppendLine($"[아이템 아이디 {item.id}, 아이템 갯수 {item.amount}]");
        }

        Debug.Log(answer.ToString());
    }

    void DoWhenDead()
    {
        Debug.Log("플레이어가 사망했습니다!");
    }

    void PlayerStatusManager()
    {
        if (maxStamina - stamina <= float.Epsilon)
        {
            health += healthRegainPerSecondWhenStaminaFull * Time.deltaTime;
            health = Mathf.Min(health, maxHealth);
        }
    }

    void m_HurtPlayer()
    {
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            health = Mathf.Max(health - 10, 0);
            stamina = Mathf.Max(stamina - 10, 0);

            Debug.Log($"m_HurtPlayer() : {health} {stamina}");
        }
    }
}