using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeHarvestBase : MonoBehaviour
{
    public float durability = 100f;

    public virtual void TakeDamage(float damage)
    {
        durability -= damage;
        print($"현재 내구도 : {durability}");


        // 자원 파괴 후, 재생성 얘기도 고려하여 우선은 SetActive로 관리해놨어요.
        if (durability <= 0f)
        {
            gameObject.SetActive(false);
            print("오브젝트가 파괴되었습니다.");
        }
    }
}
