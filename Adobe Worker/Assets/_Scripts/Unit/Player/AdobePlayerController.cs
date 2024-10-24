using Cinemachine;
using System.Text;
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


	[Header("Dash Value")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashUpwardForce;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooltime;
    [SerializeField] private Vector3 delayedForceToApply;
    [SerializeField] private float afterDashPower = 0;
    [SerializeField] private float dashDecelete = 0;
    private bool dashInput;

    AdobeItemPack inventory;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        inventory = GetComponent<AdobeItemPack>();

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

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            dashInput = true;
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
        keyHorizontalAxisValue = Input.GetAxisRaw("Horizontal");
        keyVerticalAxisValue = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(keyHorizontalAxisValue, 0.0f, keyVerticalAxisValue).normalized;

        if (inputDir.sqrMagnitude > 0)
        {
            targetRotation = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);

            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward + transform.TransformDirection(new Vector3(0f, 0f, afterDashPower));

        rb.velocity = targetDirection * (inputDir != Vector3.zero ? moveSpeed : 0.0f)
                        + new Vector3(0.0f, rb.velocity.y, 0.0f);
    }

    void PlayerDash()
    {
        Debug.Log(dashCooltime);
		//if (dashCooltime > 0) return;

        if (dashInput)
        {
            dashCooltime -= Time.deltaTime;
            Vector3 forceToApply = rb.transform.forward * dashForce + rb.transform.up * dashUpwardForce;

            delayedForceToApply = forceToApply;
            Invoke("DelayedDashForce", 0.025f);
            Invoke("ResetDash", dashDuration);
            dashCooltime = 1.5f;

            dashInput = false;
        }
        else if (afterDashPower >= 0)
        {
            afterDashPower *= dashDecelete ;
            //afterDashPower -= Time.deltaTime;

            if ( afterDashPower < 0)
            {
                afterDashPower = 0;
            }
        }
    }

    private void DelayedDashForce()
    {
        afterDashPower = 10f;
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
        if (Input.GetKeyDown(KeyCode.U) == false)
        {
            return;
        }

        AdobeItemUseArguments args = new AdobeItemUseArguments();
        args.itemUser = gameObject;
        inventory.Use(args);
    }

    void SwitchItems()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            inventory.SwitchItem(-1);
            Debug.Log($"아이템을 바꾸었습니다. 순서 : {inventory.inventoryIndex} {inventory.inventory[inventory.inventoryIndex].id}");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            inventory.SwitchItem(1);
            Debug.Log($"아이템을 바꾸었습니다. 순서 : {inventory.inventoryIndex} {inventory.inventory[inventory.inventoryIndex].id}");
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
}