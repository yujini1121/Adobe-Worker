using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeAttackRange : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float destroyTime = 1.5f;

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
        GetComponent<AdobeTagBundle>().AddReceiveAction(Attack);
        GetComponent<BoxCollider>().enabled = true;

#warning TODO: ���� �� ������ ������Ʈ Ǯ������ ǥ���ϱ�. ���Ӹ޴����� �� ������Ʈ���� ���� ��û ��, �ð� ������ ��ȯ
        Destroy(gameObject, destroyTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
