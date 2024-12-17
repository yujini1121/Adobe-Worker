using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AdobeItemBase : MonoBehaviour
{
    [TextArea]
    [Tooltip("�� ��Ʈ���� �ڵ� �󿡼� �ƹ� ¦���� ���� ������, ��� �ν����Ϳ��� �ּ� ������ �մϴ�.")]
    public string memo;

    public Sprite sprite;    
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
        Debug.Log($"{gameObject.name} : �������� ����߽��ϴ�.");
    }

    void Start()
    {
        itemPack = GetComponent<AdobeItemPack>();
    }

    /// <summary>
    ///     �����ۿ� ������Ʈ ������ ���� ��, MonoBehaviour �ν��Ͻ��� �ٷ� �����Ͽ� ���̴� ���� �Ұ����ϱ� ������, Ÿ�� ������ �̿��Ͽ� ���� �߰��ϴ� ������� �ذ��ؾ� �մϴ�.
    ///     ���� ������Ʈ�� Ư���� �ʱⰪ�� ������ �ִٸ�, �߰� �Ŀ� �� ���� �����ϴ� �߰� ������ �־�� �� �� �ֽ��ϴ�.
    /// </summary>
    /// <param name="others"></param>
    public virtual void Paste(AdobeItemBase others)
    {
        this.m_id = others.m_id;
        this.amount = others.amount;
    }
}
