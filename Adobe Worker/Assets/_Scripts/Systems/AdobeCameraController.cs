using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeCameraController : MonoBehaviour
{
	[Header("Cinemachine Camera")]
	[SerializeField] private GameObject FP_virtualCam;
	[SerializeField] private GameObject TP_virtualCam;

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	void Update()
    {
		CameraSwitching();
    }

	void CameraSwitching()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			FP_virtualCam.SetActive(!FP_virtualCam.active);
			TP_virtualCam.SetActive(!TP_virtualCam.active);
		}
	}
}
