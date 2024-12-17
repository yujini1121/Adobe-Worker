using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RandomWeight
{
    public float weight;
    public int value;
}

public class EnemyMovementBase : MonoBehaviour
{
    public float DieTime { get => deadTime;}

    [SerializeField] Animator animator;
    [SerializeField] float deadTime;

    [SerializeField] RandomWeight[] randomWeights;

    public virtual void Move()
    {
        if (animator != null)
        {
            animator.SetBool("IsMoving", true);
        }
    }

    public virtual void Stop()
    {
        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }
    }

    public virtual void GotHit()
    {
        if (animator != null)
        {
            animator.SetTrigger("GotHitTrigger");
        }
    }

    public virtual void Die()
    {
        if (animator != null)
        {
            animator.SetTrigger("DeadTrigger");
        }
    }

    public virtual void Attack()
    {
        if (animator != null)
        {
            animator.SetTrigger("AttackTrigger");

            float sum = 0;
            int index = 0;
            for (; index < randomWeights.Length; index++)
            {
                sum += randomWeights[index].weight;
            }
            float random = Random.Range(0, sum);
            for (index = 0; index < randomWeights.Length - 1; index++)
            {
                if (random <= randomWeights[index].weight) break;
                random -= randomWeights[index].weight;
            }

            animator.SetInteger("Attack", randomWeights[index].value);
        }
    }
}
