using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeAttackRange : MonoBehaviour
{
    static bool isDebugging = true;

    [SerializeField] float damage;
    [SerializeField] float destroyTime = 1.5f;

    public void SetDamage(float damage)
    {
        this.damage = damage;
        if (isDebugging)
        {
            Debug.Log($"AdobeAttackRange.SetDamage(float damage) 데미지 설정함 : {damage}");
        }
    }

    void Attack(AdobeTagActionArguments arguments)
    {
        GameObject attackTarget = arguments.other.gameObject;

        AdobePlayerController player = attackTarget.GetComponent<AdobePlayerController>();
        if (player != null)
        {
            player.GetHurt(damage);
        }

        AdobeEnemyBase enemy = attackTarget.GetComponent<AdobeEnemyBase>();
        if (enemy != null)
        {
            enemy.DoWhenDamaged(damage);
        }

        
    }

    // Start is called before the first frame update
    void Start()
    {
        AdobeTagBundle tagBundle = GetComponent<AdobeTagBundle>();

        tagBundle.AddReceiveAction(Attack);
        AdobeAttackRangeFlying attackRangeFlying = GetComponent<AdobeAttackRangeFlying>();
        if (attackRangeFlying != null)
        {
            tagBundle.AddReceiveAction(attackRangeFlying.End);
        }


        GetComponent<BoxCollider>().enabled = true;

#warning TODO: 생성 및 삭제를 오브젝트 풀링으로 표현하기. 게임메니저의 한 컴포넌트에서 생성 요청 후, 시간 지나면 반환
        Destroy(gameObject, destroyTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
