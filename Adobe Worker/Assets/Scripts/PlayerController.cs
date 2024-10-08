using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Header("Cinemachine Camera")]
	[SerializeField] private GameObject TP_virtualCam;
    [SerializeField] private GameObject FP_virtualCam;
    [SerializeField] private GameObject virtualCamTarget;
    private Rigidbody rb;

    private float keyHorizontalAxisValue;
    private float keyVerticalAxisValue;

	[SerializeField] private float virtualCamPitchTop = 70.0f;
	[SerializeField] private float virtualCamPitchBottom = -30.0f;
	private float virtualCamYaw;
    private float virtualCamPitch;


    [Header("Move Value")]
	[SerializeField] private float rotationSmoothTime = 0.12f;
	[SerializeField] private float moveSpeed = 10.0f;
	private float targetRotation = 0f;
    private float rotationVelocity = 0f;


    [Header("Dash Value")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashUpwardForce;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooltime;
    [SerializeField] private Vector3 delayedForceToApply;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        Debug.LogWarning("Test : Mouse cursor is locked");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
		CameraSwitch();

		PlayerMove();
        PlayerDash();
    }

    private void LateUpdate()
    {
        PlayerRotation();
    }


    void CameraSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            FP_virtualCam.SetActive(!FP_virtualCam.active);
            TP_virtualCam.SetActive(!TP_virtualCam.active);
        }
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

        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        rb.velocity = targetDirection.normalized * (inputDir != Vector3.zero ? moveSpeed : 0.0f)
                        + new Vector3(0.0f, rb.velocity.y, 0.0f);
    }

    void PlayerDash()
    {
        Debug.Log(dashCooltime);
		//if (dashCooltime > 0) return;

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            dashCooltime -= Time.deltaTime;
            Vector3 forceToApply = rb.transform.forward * dashForce + rb.transform.up * dashUpwardForce;

            delayedForceToApply = forceToApply;
            Invoke("DelayedDashForce", 0.025f);
            Invoke("ResetDash", dashDuration);
            dashCooltime = 1.5f;
        }
    }

    private void DelayedDashForce()
    {
		rb.AddForce(delayedForceToApply, ForceMode.Impulse);
	}

	void ResetDash()
    {

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
}