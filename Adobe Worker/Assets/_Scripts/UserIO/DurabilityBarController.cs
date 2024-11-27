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
        { 101, "����" },
        { 102, "��" },
        { 103, "ö����" },
        { 105, "����" },
        { 106, "����" },
        { 108, "�� ������" },
        { 109, "����" },
        { 111, "����" },
        { 114, "�Ź���" },
        { 116, "���" },
        { 117, "����" },
        { 118, "����" },
        { 119, "�����" },
        { 120, "����" },
        { 121, "���" },
        { 122, "���" },
        { 123, "�а���" },
        { 124, "����" },
        { 125, "�跬" },
        { 126, "����" },
        { 127, "����" },
        { 128, "û��" },
        { 200, "None" },
        { 201, "���� ��" },
        { 202, "�� ��" },
        { 203, "ö ��" },
        { 204, "���� â" },
        { 205, "�� â" },
        { 206, "ö â" },
        { 207, "Ȱ" },
        { 208, "�� ȭ��" },
        { 209, "ö ȭ��" },
        { 211, "����" },
        { 400, "None" },
        { 401, "���� ������" },
        { 402, "��Ʃ" },
        { 403, "��" },
        { 404, "����" },
        { 405, "��ġ" },
        { 406, "��� �Ķ���" },
        { 500, "None" },
        { 501, "ȸ���� ����" },
        { 502, "������ ����" },
        { 503, "�ش�" },
        { 600, "None" },
        { 601, "������" },
        { 603, "��ں�" },
        { 604, "���� �ٸ�����Ʈ" },
        { 605, "��ȭ Ʈ��" },
        { 606, "���մ�" },
        { 700, "None" },
        { 701, "���� ���" },
        { 702, "�� ���" },
        { 703, "ö ���" },
        { 704, "���� ����" },
        { 705, "�� ����" },
        { 706, "ö ����" },
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
            Debug.Log("<!>DurabilityBarController.Start() : PlayerReference���� playerInstance�� ã�� �� �����ϴ�!");
            return;
        }
        playerInventory = player.GetComponent<AdobeItemPack>();
        if (playerInventory == null)
        {
            Debug.Log("<!>DurabilityBarController.Start() : �÷��̾� ���ӿ�����Ʈ���� ItemPack�� ã�� �� �����ϴ�!");
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
