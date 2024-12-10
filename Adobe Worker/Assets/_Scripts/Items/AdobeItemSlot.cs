using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdobeItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int slotIndex; // �� ������ �ε���
    public AdobeItemPack itemPack; // �κ��丮 ���� ��ũ��Ʈ
    private Image slotImage;
    private Transform originalParent; // �巡�� ���� �� ���� �θ�
    private CanvasGroup canvasGroup; // �巡�� �� ��ȣ�ۿ� ����
    private Canvas canvas; // �巡�׸� ���� �ֻ��� ĵ����
    private Vector3 originalPosition; // �������� ���� ��ġ ����

    private void Awake()
    {
        slotImage = GetComponent<Image>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>(); // CanvasGroup �߰�
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slotImage.sprite == null) return; // �� �����̸� �巡�� �Ұ�

        originalParent = transform.parent; // ���� �θ� ����
        originalPosition = transform.position; // ���� ��ġ ����
        transform.SetParent(canvas.transform, true); // ĵ������ �̵�
        canvasGroup.blocksRaycasts = false; // �巡�� �� Raycast ��Ȱ��ȭ
        canvasGroup.alpha = 0.6f; // ���� ����
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (slotImage.sprite == null) return;
        transform.position = eventData.position; // ���콺 ��ġ�� ���� �̵�
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; // Raycast ����
        canvasGroup.alpha = 1.0f; // ���� �������

        // ��ӵ� ��ġ�� �ٸ� �������� Ȯ��
        GameObject dropTarget = eventData.pointerEnter;
        if (dropTarget != null && dropTarget.TryGetComponent(out AdobeItemSlot targetSlot))
        {
            // ���� �� ������ ��ȯ
            itemPack.SwapItems(slotIndex, targetSlot.slotIndex); // ������ ��ȯ
        }

        // �������� ���� ��ġ�� ���ư����� ����
        transform.SetParent(originalParent, true);
        transform.position = originalPosition; // ���� ��ġ�� ����

        // ���� UI ����
        itemPack.ShowInventory();
        itemPack.ShowQuickSlot();
    }
}
