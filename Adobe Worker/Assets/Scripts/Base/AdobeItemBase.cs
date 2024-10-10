using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AdobeItemBase : MonoBehaviour
{
    [TextArea]
    [Tooltip("이 스트링은 코드 상에서 아무 짝에도 쓸모가 없지만, 적어도 인스펙터에서 주석 역할을 합니다.")]
    public string memo;

    public int id;
    public int amount;

    public virtual void Use(AdobeItemUseArguments arguments)
    {
        Debug.Log($"{gameObject.name} : 아이템을 사용했습니다.");
    }
}
