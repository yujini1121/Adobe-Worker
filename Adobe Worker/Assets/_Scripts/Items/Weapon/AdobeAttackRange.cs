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

        AdobeHarvestBase harvest = attackTarget.GetComponent<AdobeHarvestBase>();
        if (harvest != null)
        {
            harvest.TakeDamage(damage);
        }
    }

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

        Destroy(gameObject, destroyTime);
    }
}
