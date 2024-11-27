using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DurabilityBarController : MonoBehaviour
{
    private bool m_isReady = false;
    private AdobeItemPack playerInventory;
    private Slider m_slider;
    private TextMeshProUGUI m_text;

    // Start is called before the first frame update
    void Start()
    {
        m_slider = GetComponent<Slider>();
        m_text = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        GameObject player = AdobePlayerReference.playerInstance;
        if (player == null)
        {
            Debug.Log("<!>DurabilityBarController.Start() : PlayerReference에서 playerInstance를 찾을 수 없습니다!");
            return;
        }
        playerInventory = player.GetComponent<AdobeItemPack>();
        if (playerInventory == null)
        {
            Debug.Log("<!>DurabilityBarController.Start() : 플레이어 게임오브젝트에서 ItemPack를 찾을 수 없습니다!");
            return;
        }
        m_isReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isReady == false)
        {
            M_ShowDefault();
            return;
        }

        AdobeItemConsumable tool = playerInventory.inventory[playerInventory.InventoryIndex] as AdobeItemConsumable;
        if (tool == null)
        {
            M_ShowDefault();
            return;
        }

        m_slider.value = (float)tool.Duability / 30.0f;
        m_text.text = $"{tool.Duability}/30";
    }

    void M_ShowDefault()
    {
        m_slider.value = 1.0f;
        m_text.text = "---";
    }
}
