using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AdobeItemBase : MonoBehaviour
{
    [TextArea]
    [Tooltip("이 스트링은 코드 상에서 아무 짝에도 쓸모가 없지만, 적어도 인스펙터에서 주석 역할을 합니다.")]
    public string memo;

    public int amount;
    public int Id { get => m_id; }

    [SerializeField] protected int m_id;
    private bool m_inited = false;

    protected AdobeItemPack itemPack;

    public void Init(int _id, int _amount)
    {
        if (m_inited) return;
        m_id = _id;
        amount = _amount;
    }

    public virtual void Use(AdobeItemUseArguments arguments)
    {
        Debug.Log($"{gameObject.name} : 아이템을 사용했습니다.");
    }

    void Start()
    {
        itemPack = GetComponent<AdobeItemPack>();
    }

    /// <summary>
    ///     아이템에 컴포넌트 정보를 넣을 때, MonoBehaviour 인스턴스를 바로 복사하여 붙이는 것은 불가능하기 때문에, 타입 정보를 이용하여 새로 추가하는 방식으로 해결해야 합니다.
    ///     만약 컴포넌트가 특별한 초기값을 가지고 있다면, 추가 후에 그 값을 설정하는 추가 로직을 넣어야 할 수 있습니다.
    /// </summary>
    /// <param name="others"></param>
    public virtual void Paste(AdobeItemBase others)
    {
        this.m_id = others.m_id;
        this.amount = others.amount;
    }
}
