using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     32개짜리 태그를 비트마스킹으로 관리하고 충돌 처리를 해주는 컴포넌트입니다.
///     태그는 유니티의 태그가 아닙니다.
/// </summary>
public class AdobeTagBundle : MonoBehaviour
{
    [Header("충돌 시 외부에서 받을 정보 : 영향을 받을 태그 필터(자신 호출)")]
    [SerializeField] bool fromPlayer;
    [SerializeField] bool fromEnemy;
    [SerializeField] bool fromAttack;
    [SerializeField] bool fromStructure;
    [SerializeField] bool fromItem;

    [Header("충돌시 외부에게 보낼 정보 : 자신을 설명하는 태그(외부 호출)")]
    [SerializeField] bool player;
    [SerializeField] bool enemy;
    [SerializeField] bool attack;
    [SerializeField] bool structure;
    [SerializeField] bool item;
    [SerializeField] List<bool> inspectorInputTags;
    [SerializeField] List<bool> inspectorOutputTags;

    const int INSPECTOR_TAGS_COUNT = 5;
    int inputTagValue; // receive tag from other
    int outputTagValue; // insert tag to other
    System.Action<AdobeTagActionArguments> tagActionReceiving;
    System.Action<AdobeTagActionArguments> tagActionSending;

    public int GetInputTagID()
    {
        return inputTagValue;
    }
    public void AddInputTag(int tagIndex)
    {
        inputTagValue = inputTagValue | (1 << tagIndex);
    }
    public void RemoveInputTag(int tagIndex)
    {
        inputTagValue &= ~(1 << tagIndex);
    }
    public bool HasInputTag(int tagIndex)
    {
        return (inputTagValue & (1 << tagIndex)) == (1 << tagIndex);
    }
    public int GetOutputTagID()
    {
        return outputTagValue;
    }
    public void AddOutputTag(int tagIndex)
    {
        outputTagValue = outputTagValue | (1 << tagIndex);
    }
    public void RemoveOutputTag(int tagIndex)
    {
        outputTagValue &= ~(1 << tagIndex);
    }
    public bool HasOutputTag(int tagIndex)
    {
        return (outputTagValue & (1 << tagIndex)) == (1 << tagIndex);
    }
    [System.Obsolete]
    public bool IsSame(AdobeTagBundle other)
    {
        return (this.inputTagValue & other.outputTagValue) != 0;
    }
    public bool HasTagSent(AdobeTagBundle other)
    {
        return (this.outputTagValue & other.inputTagValue) != 0;
    }
    public bool HasTagReceived(AdobeTagBundle other)
    {
        return (this.inputTagValue & other.outputTagValue) != 0;
    }

    public void AddReceiveAction(System.Action<AdobeTagActionArguments> action)
    {
        tagActionReceiving += action;
    }
    public void AddSendAction(System.Action<AdobeTagActionArguments> action)
    {
        tagActionSending += action;
    }

    public void WhenChildCollide(AdobeTagBundle other, AdobeTagActionArguments arguments)
    {
        if (HasTagSent(other))
        {
            tagActionSending(arguments);
        }
        if (HasTagReceived(other))
        {
            tagActionReceiving(arguments);
        }
    }

    void OnTriggerEnter(Collider other)
    {
		AdobeTagBundle otherTag = other.gameObject.GetComponent<AdobeTagBundle>();
        if (otherTag == null)
        {
            return;
        }

        AdobeTagActionArguments arguments = new AdobeTagActionArguments();
        arguments.other = other;

        if (HasTagSent(otherTag))
        {
            tagActionSending(arguments);
        }
        if (HasTagReceived(otherTag))
        {
            tagActionReceiving(arguments);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
		AdobeTagBundle otherTag = collision.gameObject.GetComponent<AdobeTagBundle>();
        if (otherTag == null)
        {
            return;
        }
        //Debug.Log($"{gameObject.name} : 충돌!");
        //Debug.Log($"{gameObject.name} : i {AdobeUtility.ShowBitArray(inputTagValue)} o : {AdobeUtility.ShowBitArray(outputTagValue)}");
        AdobeTagActionArguments arguments = new AdobeTagActionArguments();
        arguments.SetCollision(collision);

        if (HasTagSent(otherTag))
        {
            tagActionSending(arguments);
        }
        if (HasTagReceived(otherTag))
        {
            tagActionReceiving(arguments);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        tagActionSending += (AdobeTagActionArguments arguments) => { };
        tagActionReceiving += (AdobeTagActionArguments arguments) => { };
        if (fromPlayer == true) AddInputTag(0);
        if (fromEnemy == true) AddInputTag(1);
        if (fromAttack == true) AddInputTag(2);
        if (fromStructure == true) AddInputTag(3);
        if (fromItem == true) AddInputTag(4);

        if (player == true) AddOutputTag(0);
        if (enemy == true) AddOutputTag(1);
        if (attack == true) AddOutputTag(2);
        if (structure == true) AddOutputTag(3);
        if (item == true) AddOutputTag(4);

        for (int index = 0; index < 32 - INSPECTOR_TAGS_COUNT && index < inspectorInputTags.Count; ++index)
        {
            if (inspectorInputTags[index]) AddInputTag(index + INSPECTOR_TAGS_COUNT);
        }
        for (int index = 0; index < 32 - INSPECTOR_TAGS_COUNT && index < inspectorOutputTags.Count; ++index)
        {
            if (inspectorOutputTags[index]) AddOutputTag(index + INSPECTOR_TAGS_COUNT);
        }
    }
}
