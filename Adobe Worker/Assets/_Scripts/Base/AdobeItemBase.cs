using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AdobeItemBase : MonoBehaviour
{
    public static AdobeItemBase instance;

    [TextArea]
    [Tooltip("�� ��Ʈ���� �ڵ� �󿡼� �ƹ� ¦���� ���� ������, ��� �ν����Ϳ��� �ּ� ������ �մϴ�.")]
    public string memo;

    public Sprite sprite;
    public int id;
    public int amount;

    protected AdobeItemPack itemPack;


    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    public void Start()
    {
        itemPack = GetComponent<AdobeItemPack>();
    }

    public virtual void Use(AdobeItemUseArguments arguments)
    {
        Debug.Log($"{gameObject.name} : �������� ����߽��ϴ�.");
    }

    /// <summary>
    ///     �����ۿ� ������Ʈ ������ ���� ��, MonoBehaviour �ν��Ͻ��� �ٷ� �����Ͽ� ���̴� ���� �Ұ����ϱ� ������, Ÿ�� ������ �̿��Ͽ� ���� �߰��ϴ� ������� �ذ��ؾ� �մϴ�.
    ///     ���� ������Ʈ�� Ư���� �ʱⰪ�� ������ �ִٸ�, �߰� �Ŀ� �� ���� �����ϴ� �߰� ������ �־�� �� �� �ֽ��ϴ�.
    /// </summary>
    /// <param name="others"></param>
    public virtual void Paste(AdobeItemBase others)
    {
        this.id = others.id;
        this.amount = others.amount;
    }
}
