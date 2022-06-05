using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { None = -1, Idle = 0, Wander,Pursuit, Attack,}

public class EnemyFSM : MonoBehaviour
{
    [Header("Pursuit")]
    [SerializeField] float targetRecognitionRange = 8;
    [SerializeField] float pursuitLimitRange = 10;

    [Header("Attack")]
    [SerializeField] GameObject projectilePrefab;      //발사체
    [SerializeField] Transform projectileSpawnPoint; //발사체 생성 위;치
    [SerializeField] float attackRange = 5;     //공격 범위
    [SerializeField] float attackRate = 1; //공격 속도

    EnemyState enemyState = EnemyState.None;
    float lastAttackTime = 0;

    PlayerStatus statue;
    NavMeshAgent navMeshAgent;
    Transform target;
    EnemyMemoryPool enemyMemoryPool;
    PlayerController playerController;

    private void Start()
    {
        
    }

    public void Setup(Transform target, EnemyMemoryPool enemyMemoryPool)
    {
        statue = GetComponent<PlayerStatus>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        this.enemyMemoryPool = enemyMemoryPool;
        this.target = target;

        //navMeshAgent 컴포넌트에서 회전을 업데이트 하지 못하도록 설정
        navMeshAgent.updateRotation = false;
    }

    private void OnEnable()
    {
        ChangerState(EnemyState.Idle);
    }

    private void OnDisable()
    {
        StopCoroutine(enemyState.ToString());

        enemyState = EnemyState.None;
    }

    public void ChangerState(EnemyState newState)
    {
        //현재 진행중인 상태와 바꾸려고 하는 상태가 같으면 바꿀 필요가 없기 때문에 return;
        if (enemyState == newState) return;

        StopCoroutine(enemyState.ToString());

        enemyState = newState;

        StartCoroutine(enemyState.ToString());

    }

    IEnumerator Idle()
    {
        StartCoroutine("AutoChangeFromIdleToWander");

        while (true)
        {
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }

    IEnumerator AutoChangeFromIdleToWander()
    {
        int changeTime = Random.Range(1, 5);

        yield return new WaitForSeconds(changeTime);

        ChangerState(EnemyState.Wander);
    }

    IEnumerator Wander()
    {
        float curTime = 0;
        float maxTime = 10;

        navMeshAgent.speed = statue.WalkSpeed;

        navMeshAgent.SetDestination(CalculateWanderPosition());

        Vector3 to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation = Quaternion.LookRotation(to - from);

        while (true)
        {
            curTime += Time.deltaTime;

            to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
            from = new Vector3(transform.position.x, 0, transform.position.z);
            if ((to - from).sqrMagnitude < 0.01f || curTime >= maxTime)
            {
                ChangerState(EnemyState.Idle);
            }

            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }

    IEnumerator Pursuit()
    {
        while (true)
        {
            navMeshAgent.speed = statue.RunSpeed;

            navMeshAgent.SetDestination(target.position);

            LookRotationToTarget();

            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }

    IEnumerator Attack()
    {
        //공격 할때는 이동을 멈추도록 설정
        
        navMeshAgent.ResetPath();

        while (true)
        {
            LookRotationToTarget();

            CalculateDistanceToTargetAndSelectState();

            if(Time.time - lastAttackTime > attackRate)
            {
                lastAttackTime = Time.time;

                GameObject clone = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                clone.GetComponent<EnemyProjectile>().Setup(target.position);
            }
            yield return null;
        }
    }

    void LookRotationToTarget()
    {
        Vector3 to = new Vector3(target.position.x, 0, target.position.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

        transform.rotation = Quaternion.LookRotation(to - from);
    }

    void CalculateDistanceToTargetAndSelectState()
    {
        if (target == null) return;

        float distance = Vector3.Distance(target.position, transform.position);

        if(distance <= attackRange)
        {
            ChangerState(EnemyState.Attack);
        }
        else if(distance <= targetRecognitionRange)
        {
            ChangerState(EnemyState.Pursuit);
        }
        else if(distance >= pursuitLimitRange)
        {
            ChangerState(EnemyState.Wander);
        }
    }

    Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 10;
        int wanderJitter = 0;
        int wanderJitterMin = 0;
        int wanderJitterMax = 360;

        Vector3 rangePos = Vector3.zero;
        Vector3 rangeScale = Vector3.one * 100.0f;

        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPos = transform.position + SetAngle(wanderRadius,wanderJitter);

        targetPos.x = Mathf.Clamp(targetPos.x, rangePos.x - rangeScale.x * 0.5f, rangePos.x + rangeScale.x * 0.5f);
        targetPos.y = 0.0f;
        targetPos.z = Mathf.Clamp(targetPos.z, rangePos.z - rangeScale.z * 0.5f, rangePos.z + rangeScale.z * 0.5f);

        return targetPos;
    }

    Vector3 SetAngle(float radius, int angle)
    {
        Vector3 pos = Vector3.zero;

        pos.x = Mathf.Cos(angle) * radius;
        pos.z = Mathf.Sin(angle) * radius;

        return pos;
    }

    public void TakeDamage(int damage)
    {
        bool isDie = statue.DecreaseHP(damage);

        if(isDie == true)
        {
            enemyMemoryPool.DeactivateEnemy(gameObject);
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.black;
    //    Gizmos.DrawRay(transform.position, navMeshAgent.destination - transform.position);
    //}
}
