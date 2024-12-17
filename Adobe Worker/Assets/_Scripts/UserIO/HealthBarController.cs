using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    private TextMeshProUGUI m_hpText;
    private Slider m_slider;
    private AdobePlayerController m_playerController;

    // Start is called before the first frame update
    void Start()
    {
        m_slider = GetComponent<Slider>();
        m_hpText = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        m_playerController = AdobePlayerReference.playerInstance.GetComponent<AdobePlayerController>();
    }


    // Update is called once per frame
    void Update()
    {
        if (m_playerController != null)
        {
            m_slider.value = m_playerController.Health / m_playerController.MaxHealth;
            m_hpText.text = $"{m_playerController.Health} / {m_playerController.MaxHealth}";
        }
    }
}
