using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeHarvestBase : MonoBehaviour
{
    public float durability = 100f;

    public virtual void TakeDamage(float damage)
    {
        durability -= damage;
        print($"���� ������ : {durability}");


        // �ڿ� �ı� ��, ����� ��⵵ ����Ͽ� �켱�� SetActive�� �����س����.
        if (durability <= 0f)
        {
            gameObject.SetActive(false);
            print("������Ʈ�� �ı��Ǿ����ϴ�.");
        }
    }
}
