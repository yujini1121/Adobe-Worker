using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdobeItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int slotIndex; // 이 슬롯의 인덱스
    public AdobeItemPack itemPack; // 인벤토리 관리 스크립트
    private Image slotImage;
    private Transform originalParent; // 드래그 시작 시 원래 부모
    private CanvasGroup canvasGroup; // 드래그 시 상호작용 제어
    private Canvas canvas; // 드래그를 위한 최상위 캔버스
    private Vector3 originalPosition; // 아이템의 원래 위치 저장

    private void Awake()
    {
        slotImage = GetComponent<Image>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>(); // CanvasGroup 추가
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slotImage.sprite == null) return; // 빈 슬롯이면 드래그 불가

        originalParent = transform.parent; // 원래 부모 저장
        originalPosition = transform.position; // 원래 위치 저장
        transform.SetParent(canvas.transform, true); // 캔버스로 이동
        canvasGroup.blocksRaycasts = false; // 드래그 중 Raycast 비활성화
        canvasGroup.alpha = 0.6f; // 투명도 변경
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (slotImage.sprite == null) return;
        transform.position = eventData.position; // 마우스 위치에 따라 이동
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; // Raycast 복원
        canvasGroup.alpha = 1.0f; // 투명도 원래대로

        // 드롭된 위치가 다른 슬롯인지 확인
        GameObject dropTarget = eventData.pointerEnter;
        if (dropTarget != null && dropTarget.TryGetComponent(out AdobeItemSlot targetSlot))
        {
            // 슬롯 간 아이템 교환
            itemPack.SwapItems(slotIndex, targetSlot.slotIndex); // 아이템 교환
        }

        // 아이템이 원래 위치로 돌아가도록 설정
        transform.SetParent(originalParent, true);
        transform.position = originalPosition; // 원래 위치로 복귀

        // 슬롯 UI 갱신
        itemPack.ShowInventory();
        itemPack.ShowQuickSlot();
    }
}
