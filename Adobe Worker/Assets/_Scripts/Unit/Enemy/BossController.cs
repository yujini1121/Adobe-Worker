using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public bool IsDead { get => isDead; }

    [SerializeField] private Renderer objectRenderer;
    [SerializeField] GameObject AttackPrefab;
    [SerializeField] Transform AttackPosition;
    [SerializeField] float attackRange = 20f;
    [SerializeField] float maxHealth = 15f;
    [SerializeField] float timeTermForAttack = 2.0f;
    [SerializeField] float destroyTime = 3.0f;
    private Transform playerTransform;
    float currentHealth;
    float nextAttackTime = 0.0f;
    bool isDead = false;

    public void DoWhenDamaged(float damage)
    {

        Debug.Log($"BossController : 데미지 {damage}");

        if (isDead) return;

        currentHealth -= damage;

        Debug.Log($"BossController : 체력 {currentHealth}");

        if (currentHealth <= 0)
        {
            isDead = true;
            StartCoroutine(FadeIn(destroyTime));
        }
    }

    protected virtual bool IsCanReach(float range)
    {
        return (transform.position - playerTransform.position).sqrMagnitude < range * range;
    }

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = AdobePlayerReference.playerInstance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        if (IsCanReach(attackRange) && (Time.time >= nextAttackTime))
        {
            GameObject m_bullet = Instantiate(AttackPrefab, AttackPosition.transform.position, Quaternion.identity);
            m_bullet.transform.LookAt(playerTransform.position);
            nextAttackTime = Time.time + timeTermForAttack;
        }
    }

    private IEnumerator FadeIn(float duration)
    {

        // 초기 알파값 (0)과 목표 알파값 (1) 
        float startAlpha = 1f;
        float endAlpha = 0f;

        // 경과 시간 초기화
        float elapsedTime = 0f;

        // 경과 시간이 duration보다 작을 때까지 반복합니다.
        while (elapsedTime < duration)
        {


            // 경과 시간 갱신
            elapsedTime += Time.deltaTime;

            // 보간된 알파값을 계산 (0에서 1 사이의 값으로 보간)
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);

            Debug.Log($"AA {newAlpha}");


            // 렌더러의 색상을 가져와서 알파값을 수정
            if (objectRenderer != null)
            {
                Color color = objectRenderer.material.color;
                color.r = newAlpha; // 값 설정
                color.g = newAlpha; // 값 설정
                color.b = newAlpha; // 값 설정
                objectRenderer.material.color = color; // 색상 적용
            }

            // 다음 프레임까지 대기
            yield return null;
        }

        // 마지막으로 목표 알파값을 명확히 설정 (누락 방지)
        if (objectRenderer != null)
        {
            Color color = objectRenderer.material.color;
            color.a = endAlpha; // 알파값 1로 설정 (완전히 불투명)
            objectRenderer.material.color = color; // 색상 적용
        }
    }
}
