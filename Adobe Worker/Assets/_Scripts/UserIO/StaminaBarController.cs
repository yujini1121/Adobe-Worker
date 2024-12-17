using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBarController : MonoBehaviour
{
    private TextMeshProUGUI m_mpText;
    private Slider m_slider;
    private AdobePlayerController m_playerController;

    // Start is called before the first frame update
    void Start()
    {
        m_slider = GetComponent<Slider>();
        m_mpText = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        m_playerController = AdobePlayerReference.playerInstance.GetComponent<AdobePlayerController>();
    }


    // Update is called once per frame
    void Update()
    {
        if (m_playerController != null)
        {
            m_slider.value = m_playerController.Stamina / m_playerController.MaxStamina;
            m_mpText.text = $"{m_playerController.Stamina} / {m_playerController.MaxStamina}";
        }
    }
}
