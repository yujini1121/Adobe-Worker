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
    private TextMeshProUGUI m_textItemName;
    private Dictionary<int, string> m_itemName = new Dictionary<int, string>()
    {
        { 0, "None" },
        { 100, "None" },
        { 101, "나무" },
        { 102, "돌" },
        { 103, "철광석" },
        { 105, "밧줄" },
        { 106, "종이" },
        { 108, "빈 유리병" },
        { 109, "물병" },
        { 111, "깃털" },
        { 114, "거미줄" },
        { 116, "당근" },
        { 117, "감자" },
        { 118, "베리" },
        { 119, "양배추" },
        { 120, "버섯" },
        { 121, "사과" },
        { 122, "고기" },
        { 123, "밀가루" },
        { 124, "양파" },
        { 125, "계랸" },
        { 126, "적옥" },
        { 127, "벽옥" },
        { 128, "청옥" },
        { 200, "None" },
        { 201, "나무 검" },
        { 202, "돌 검" },
        { 203, "철 검" },
        { 204, "나무 창" },
        { 205, "돌 창" },
        { 206, "철 창" },
        { 207, "활" },
        { 208, "돌 화살" },
        { 209, "철 화살" },
        { 211, "부적" },
        { 400, "None" },
        { 401, "과일 샐러드" },
        { 402, "스튜" },
        { 403, "전" },
        { 404, "만두" },
        { 405, "꼬치" },
        { 406, "계란 후라이" },
        { 500, "None" },
        { 501, "회복의 물약" },
        { 502, "염력의 물약" },
        { 503, "붕대" },
        { 600, "None" },
        { 601, "가마솥" },
        { 603, "모닥불" },
        { 604, "가시 바리게이트" },
        { 605, "둔화 트랩" },
        { 606, "조합대" },
        { 700, "None" },
        { 701, "나무 곡괭이" },
        { 702, "돌 곡괭이" },
        { 703, "철 곡괭이" },
        { 704, "나무 도끼" },
        { 705, "돌 도끼" },
        { 706, "철 도끼" },
    };


    // Start is called before the first frame update
    void Start()
    {
        m_slider = GetComponent<Slider>();
        m_text = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        m_textItemName = transform.Find("ItemName").GetComponent<TextMeshProUGUI>();

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

        m_textItemName.text = m_itemName[playerInventory.inventory[playerInventory.InventoryIndex].Id];

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
