using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeEnemyBase : MonoBehaviour, AdobeIDamageable
{
    public float health;
    public float speed;
    [SerializeField] protected float detectRange;
    private Transform playerTransform;

    Rigidbody myRigidbody;

    public virtual void DoWhenDamaged(float damage)
    {
        health -= damage;
        print("Hello");

        if (health < 0)
        {
            DoWhenDead();
        }
    }

    protected virtual void DoWhenDead()
    {
        AdobeItemPack drops = GetComponent<AdobeItemPack>();
        drops.DropItemsAll();
        Destroy(gameObject);
    }

    protected virtual bool IsDetectPlayer()
    {
        return (transform.position - playerTransform.position).sqrMagnitude < detectRange * detectRange;
    }

    protected virtual void LinearChase()
    {
        // 플레이어를 향해 바라보게 만듦
        Vector3 target = playerTransform.position;
        target.y = transform.position.y;
        transform.LookAt(target, Vector3.up);

        // 플레이어를 향해 접근
        myRigidbody.velocity = transform.forward * speed;
    }

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();

        playerTransform = AdobePlayerReference.playerInstance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDetectPlayer())
        {
            LinearChase();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            DoWhenDead();
        }
    }
}
