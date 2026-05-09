using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType { Basic, Fast, Heavy, Swarm,Stealth,Flying, BossSpider, None}

public class Enemy : MonoBehaviour , IDamagable
{

    public Enemy_Visuals visuals {  get; private set; }

    protected ObjectPoolManager objectPool;
    protected NavMeshAgent agent;
    protected Rigidbody rb;
    protected EnemyPortal myPortal;
    protected GameManager gameManager;

    [SerializeField] private EnemyType enemyType;
    [SerializeField] private Transform centerPoint;
    public float maxHp = 100;
    public float currentHp = 4;
    protected bool isDead;

    [Header("Movement")]
    [SerializeField] private float turnSpeed = 10;
    [SerializeField] protected Vector3[] myWaypoints;

    protected int nextWaypointIndex;
    protected int currentWaypointIndex;
    protected float totalDistance;
    protected float originalSpeed;

    protected bool canBeHidden = true;
    protected bool isHidden;
    private Coroutine hideCo;
    private Coroutine disableHideCo;
    private int originalLayerIndex;
    protected IEnemyEffectController effectController;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // NavMeshAgent may not be present on some prefabs (pooled or special-case enemies).
        // Use TryGetComponent and guard agent usage everywhere.
        if (TryGetComponent<NavMeshAgent>(out var foundAgent))
        {
            agent = foundAgent;
            agent.updateRotation = false;
            agent.avoidancePriority = Mathf.RoundToInt(agent.speed * 10);
        }

        visuals = GetComponent<Enemy_Visuals>();
        originalLayerIndex = gameObject.layer;

        gameManager = FindFirstObjectByType<GameManager>();
        originalSpeed = agent != null ? agent.speed : 0f;

        objectPool = ObjectPoolManager.instance;
        effectController = new EnemyEffectController(this);
    }

    protected virtual void Start()
    {

    }


    public void SetupEnemy(EnemyPortal myNewPortal)
    {
        myPortal = myNewPortal;
        // Guard against portals that haven't collected waypoints yet or have none
        if (myPortal == null || myPortal.currentWaypints == null || myPortal.currentWaypints.Length == 0)
        {
            myWaypoints = new Vector3[] { transform.position };
            CollectTotalDistance();
            ResetEnemy();
            BeginMovement();
            return;
        }

        UpdateWaypoints(myPortal.currentWaypints);
        CollectTotalDistance();
        ResetEnemy();
        BeginMovement();
    }

    private void UpdateWaypoints(Vector3[] newWaypoints)
    {
        if (newWaypoints == null || newWaypoints.Length == 0)
        {
            myWaypoints = new Vector3[] { transform.position };
            return;
        }

        myWaypoints = new Vector3[newWaypoints.Length];

        for (int i = 0; i < myWaypoints.Length; i++)
            myWaypoints[i] = newWaypoints[i];
    }

    private void BeginMovement()
    {
        currentWaypointIndex = 0;
        nextWaypointIndex = 0;
        ChangeWaypoint();
    }

    protected void ResetEnemy()
    {
        gameObject.layer = originalLayerIndex;

        if (visuals != null)
            visuals.MakeTransperent(false);

        currentHp = maxHp;
        isDead = false;

        if (agent != null)
        {
            agent.speed = originalSpeed;
            agent.enabled = true;
        }
    }


    protected virtual void Update()
    {
        if (agent != null)
            FaceTarget(agent.steeringTarget);

        // Check if the agent is close to current target point
        if (ShouldChangeWaypoint())
            ChangeWaypoint();
    }
                                    
    public void SlowEnemy(float slowMultiplier,float duration) => effectController.SlowEnemy(slowMultiplier, duration);
    internal void ApplySlowEnemy(float slowMultiplier, float duration) => StartCoroutine(SlowEnemyCo(slowMultiplier, duration));
    private IEnumerator SlowEnemyCo(float slowMultiplier, float duration)
    {
        if (agent != null)
        {
            agent.speed = originalSpeed;
            agent.speed = agent.speed * slowMultiplier;
        }

        yield return new WaitForSeconds(duration);

        if (agent != null)
            agent.speed = originalSpeed;
    }
    public void DisableHide(float duration) => effectController.DisableHide(duration);
    internal void ApplyDisableHide(float duration)
    {
        if(disableHideCo != null)
            StopCoroutine(disableHideCo);

        disableHideCo = StartCoroutine(DisableHideCo(duration));
    }
    protected virtual IEnumerator DisableHideCo(float duration)
    {
        canBeHidden = false;

        yield return new WaitForSeconds(duration);
        canBeHidden = true;
    }
    public void HideEnemy(float duration) => effectController.HideEnemy(duration);
    internal void ApplyHideEnemy(float duration)
    {
        if (canBeHidden == false)
            return;

        if(hideCo != null)
            StopCoroutine(hideCo);

        hideCo = StartCoroutine(HideEnemyCo(duration));
    }
    private IEnumerator HideEnemyCo(float duration)
    {
        gameObject.layer = LayerMask.NameToLayer("Untargetable");
        if (visuals != null)
            visuals.MakeTransperent(true);
        isHidden = true;

        yield return new WaitForSeconds(duration);

        gameObject.layer = originalLayerIndex;
        if (visuals != null)
            visuals.MakeTransperent(false);
        isHidden = false;
    }

    protected virtual void ChangeWaypoint()
    {
        if (agent != null)
            agent.SetDestination(GetNextWaypoint());
    }
    protected virtual bool ShouldChangeWaypoint()
    {
        if (nextWaypointIndex >= myWaypoints.Length)
            return false;

        if (agent != null)
        {
            if (agent.remainingDistance < .5f)
                return true;
        }

        Vector3 currentWaypoint = myWaypoints[currentWaypointIndex];
        Vector3 nextWaypoint = myWaypoints[nextWaypointIndex];

        float distanceToNextWaypoint = Vector3.Distance(transform.position, nextWaypoint);
        float distnaceBeetwenPoints = Vector3.Distance(currentWaypoint, nextWaypoint);

        // If no NavMeshAgent is present, fall back to a simple proximity check
        if (agent == null)
        {
            if (distanceToNextWaypoint < .5f)
                return true;
        }

        return distnaceBeetwenPoints > distanceToNextWaypoint;
    }

    public virtual float DistanceToFinishLine()
    {
        if (agent != null)
            return totalDistance + agent.remainingDistance;

        return totalDistance;
    }
    private void CollectTotalDistance()
    {
        for (int i = 0; i < myWaypoints.Length - 1; i++)
        {
            float distance = Vector3.Distance(myWaypoints[i], myWaypoints[i + 1]);
            totalDistance = totalDistance + distance;
        }
    }
    private void FaceTarget(Vector3 newTarget)
    {
        // Calculate the direction from current position to the new target
        Vector3 directionToTarget = newTarget - transform.position;
        directionToTarget.y = 0; // Ignore any diffrence in the vertical position // Removes vertical component

        Quaternion newRotation = Quaternion.LookRotation(directionToTarget);

        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, turnSpeed * Time.deltaTime);
    }

    protected Vector3 GetFinalWaypoint()
    {
        if(myWaypoints.Length == 0)
            return transform.position;

        return myWaypoints[myWaypoints.Length - 1];
    }

    private Vector3 GetNextWaypoint()
    {
        // Check if the waypoint index is beyond the last waypoint
        if (nextWaypointIndex >= myWaypoints.Length)
        {
            // If true, return the agent's current position, effectively stopping it
            // Uncomment the line below to loop the waypoints
            // waypointIndex = 0;
            return transform.position;
        }

        // Get the current target point from the waypoints array
        Vector3 targetPoint = myWaypoints[nextWaypointIndex];

        // If this is not the first waypoint, calculate the distance from the previous waypoint
        if (nextWaypointIndex > 0)
        {
            float distance = Vector3.Distance(myWaypoints[nextWaypointIndex], myWaypoints[nextWaypointIndex - 1]);
            // Subtract this distance from the total distance
            totalDistance = totalDistance - distance;
        }

        // Increment the waypoint index to move to the next waypoint on the next call
        nextWaypointIndex = nextWaypointIndex + 1;
        currentWaypointIndex = nextWaypointIndex - 1; // Assign current waypoint index

        // Return the current target point
        return targetPoint;
    }

    public Vector3 CenterPoint() => centerPoint != null ? centerPoint.position : transform.position;
    public EnemyType GetEnemyType() => enemyType;
    
    public virtual void TakeDamage(float damage)
    {
        currentHp = currentHp - damage;

        if (currentHp <= 0 && isDead == false)
        {
            // Is dead is needed, because I had situatuions 
            // where Die() method would be called twice
            isDead = true;
            Die();
        }
    }

    public virtual void Die()
    {
        gameManager.UpdateCurrency(1);
        RemoveEnemy();
    }

    public virtual void RemoveEnemy()
    {
        if (visuals != null)
            visuals.CreateOnDeathVFX();

        if (objectPool != null)
            objectPool.Remove(gameObject);
        else
            Destroy(gameObject);

        if (agent != null)
            agent.enabled = false;

        if (myPortal != null)
            myPortal.RemoveActiveEnemy(gameObject);
    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {
        StopAllCoroutines();
        CancelInvoke();
    }
}
