using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeEnemyBase : MonoBehaviour, AdobeIDamageable
{
    public float health;
    public float speed;
    [SerializeField] protected float detectRange;
    [SerializeField] protected float attackTryRange;
    [SerializeField] protected float attacTermTime;
    protected float nextAttackTime;
    [SerializeField] protected float offsetAttackPosition;
    private Transform playerTransform;
    private bool isDead = false;


    Rigidbody myRigidbody;
    EnemyMovementBase myEnemyMovementBase;

    public virtual void DoWhenDamaged(float damage)
    {
        if (isDead) return;

        health -= damage;
        print("Hello");

        if (health < 0)
        {
            DoWhenDead();
            myEnemyMovementBase.Die();
        }
        else
        {
            myEnemyMovementBase.GotHit();
        }
    }

    protected virtual void DoWhenDead()
    {
        isDead = true;

        AdobeItemPack drops = GetComponent<AdobeItemPack>();
        drops.DropItemsAll();
        Destroy(gameObject, myEnemyMovementBase.DieTime);
    }

    protected virtual bool IsCanReach(float range)
    {
        return (transform.position - playerTransform.position).sqrMagnitude < range * range;
    }

    protected virtual bool IsDetectPlayer() => IsCanReach(detectRange);
    protected virtual bool IsCanAttackReach() => IsCanReach(attackTryRange);

    protected virtual void LinearChase()
    {
        if (isDead) return;

        // 플레이어를 향해 바라보게 만듦
        Vector3 target = playerTransform.position;
        target.y = transform.position.y;
        transform.LookAt(target, Vector3.up);

        // 플레이어를 향해 접근
        myRigidbody.velocity = transform.forward * speed;
    }

    

    protected virtual void Attack()
    {
        if (IsCanAttackReach() == false) return;
        if (Time.time < nextAttackTime) return;

        Debug.Log($"AdobeEnemyBase : 공격!");

        myEnemyMovementBase.Attack();
        GameObject instantiated = Instantiate(
            AdobePrefabManager.enemyAttackRange,
            transform.position + AdobePrefabManager.enemyAttackRange.transform.localScale.z / 2 * transform.forward,
            transform.rotation
            );

        nextAttackTime = Time.time + attacTermTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        myEnemyMovementBase = GetComponent<EnemyMovementBase>();

        playerTransform = AdobePlayerReference.playerInstance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) { return; }

        if (IsDetectPlayer())
        {
            LinearChase();
            myEnemyMovementBase.Move();
            Attack();
        }
        else
        {
            myEnemyMovementBase.Stop();
        }
    }
}
