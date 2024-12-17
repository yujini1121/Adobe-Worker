using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdobeItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private int slotIndex;
    [SerializeField] private AdobeItemPack itemPack;

    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Image slotImage;
    private Transform originalParent;
    private Vector3 originalPosition;

    private void Awake()
    {
        slotImage = GetComponent<Image>();

        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = transform.position;
        transform.SetParent(canvas.transform, true);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1.0f;

        GameObject dropTarget = eventData.pointerEnter;
        if (dropTarget != null && dropTarget.TryGetComponent(out AdobeItemSlot targetSlot))
        {
            //Debug.Log("Slot Index: " + slotIndex);
            //Debug.Log("Target Slot Index: " + targetSlot.slotIndex);

            itemPack.SwapItems(slotIndex, targetSlot.slotIndex);
        }
        else
        {
            Debug.Log("Drop target is not valid or not a valid slot.");
        }

        // 원래 위치로 돌아가도록 설정
        transform.SetParent(originalParent, true);
        transform.position = originalPosition;

        // 슬롯 UI 갱신
        itemPack.ShowInventory();
        itemPack.ShowQuickSlot();
    }

}
