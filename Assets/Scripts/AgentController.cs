using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{

    #region SerializedFields
    [Header("Movement variables")]
    [SerializeField, Tooltip("The speed of the agent")]
    float speed;
    [SerializeField, Tooltip("The mass, high mass equals a more 'heavy' agent (slower turn speed)")]
    float mass;
    [SerializeField, Tooltip("The max force applied to the agent. Higher max force will result in more acceleration and turning speed")]
    float maxForce;
    [SerializeField, Tooltip("The max degrees the gameobject can turn each frame.")]
    float maxDeltaAngle;
    [Header("Path variables")]
    [SerializeField, Tooltip("The time between each time the agent will try to calculate a new path, low repathRate may affect performance")]
    float repathRate;
    [SerializeField, Tooltip("The distance the agent will pick a new waypoint.")]
    float pickNextWaypointDistance;
    [Header("Push force variables")]
    [SerializeField, Tooltip("Whether this agent is pushed by other agents")]
    bool applyPushingForce;
    [SerializeField, Tooltip("The radius of influence of the pushing force")]
    float pushRadius;
    [SerializeField, Tooltip("How much the agent is pushed")]
    float pushWeight;
    #endregion
    #region Private
    NavMeshPath path;
    Vector3[] vPath;
    int currentWaypoint;
    bool targetReached = true;
    Vector3 targetPos;
    Vector3 velocity;
    float timeSinceLastPath;
    bool pushesToStandingAgents = false;
    int numberOfPushesOnStandingAgent;
    int targetDistanceMultiplier;
    bool cannotBePushed = false;
    #endregion
    #region Properties
    public float MaxForce { get { return maxForce; } set { maxForce = value; } }
    public float Speed { get { return speed; } set { speed = value; } }
    public float Mass { get { return mass; } set { mass = value; } }
    public Vector3 PushingForce { get; set; }
    public int NoOfPushers { get; set; }
    public float CurrentSpeed { get { return velocity.magnitude; } }
    public float PushRadius { get { return pushRadius; } }
    public bool ApplyPushingForces { get { return applyPushingForce; } }
    public bool TargetReached { get { return targetReached; } }
    #endregion

    // Use this for initialization
    void Start()
    {
        // Add this to the agent manager
        AgentManager.Instance.AddAgent(this);
        path = new NavMeshPath();
    }


    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Order the agent to go to specific position.
    /// </summary>
    /// <param name="tp">The target position</param>
    public void GoToPos(Vector3 tp)
    {
        Stop();
        targetPos = tp;
        StartCoroutine(TryToSearchPath());
    }

    private IEnumerator TryToSearchPath()
    {
        while (true)
        {
            if (NavMesh.CalculatePath(this.transform.position, targetPos, NavMesh.AllAreas, path))
            {
                targetReached = false;
                vPath = path.corners;
                Debug.Log("Calculated new path " + vPath.Length);
                yield break;
            }
            yield return new WaitForSeconds(repathRate);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            var p = (this.transform.position - other.transform.position).normalized * 0.025f;
            
            p = new Vector3(p.x, 0.0f, p.z);
            transform.position += p;
            cannotBePushed = true;
            PushingForce = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            cannotBePushed = true;
        }
    }

    /// <summary>
    /// Moves the agent with the current velocity
    /// </summary>
    public void ApplyVelocity()
    {
        if (NoOfPushers > 0)
            PushingForce /= NoOfPushers;

        NoOfPushers = 0;
        //transform.Translate(PushingForce, Space.World);
        

        if (velocity != Vector3.zero)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(velocity);
            Quaternion restrictedRotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, maxDeltaAngle);
            transform.rotation = restrictedRotation;
        }
        Vector3 finalVelocity = transform.forward * velocity.magnitude * Time.deltaTime * speed;
        
        finalVelocity += PushingForce;
        PushingForce = Vector3.zero;

        if (finalVelocity.magnitude > speed)
            finalVelocity = finalVelocity.normalized * speed;
        
        if (finalVelocity.magnitude > 0f)
        {
            transform.Translate(finalVelocity, Space.World);
        }

        if (!targetReached)
        {
            if (!pushesToStandingAgents)
            {
                targetDistanceMultiplier = 1;
                numberOfPushesOnStandingAgent = 0;
            }
            else if(numberOfPushesOnStandingAgent > 40 && targetDistanceMultiplier < 5)
            {
                targetDistanceMultiplier++;
            }
            pushesToStandingAgents = false;
            if (Vector3.Distance(this.transform.position, targetPos) < 0.5f * targetDistanceMultiplier)
            {
                Stop();
            }
        }

    }

    /// <summary>
    /// Calculate the velocity of the agent.
    /// </summary>
    public void CalculateVelocity()
    {
        if (targetReached)
        {
            velocity = Vector3.zero;
            return;
        }
        // Get the current xz position of the agent
        Vector3 currentPosition = this.transform.position;

        // Check if we need to go to next waypoint
        if (currentWaypoint <= vPath.Length - 1)
        {
            var dis = XZDistance(currentPosition, vPath[currentWaypoint]);
            // Check if agent is close enough to waypoint
            if (dis <= pickNextWaypointDistance)
            {
                currentWaypoint++;
                if (currentWaypoint >= vPath.Length)
                {
                    Stop();
                    return;
                }
            }
        }

        // Calculate steering
        Vector3 desiredVelocity = Vector3.Normalize(vPath[currentWaypoint] - currentPosition) * speed;
        Vector3 steering = desiredVelocity - velocity;
        if (steering.sqrMagnitude > maxForce)
            steering = Vector3.Normalize(steering) * maxForce;
        
        steering = steering / mass;

        // Calculate velocity
        velocity = velocity + steering;
    }

    /// <summary>
    /// Calculate push forcing based on close agents
    /// </summary>
    /// <param name="neighborAgents">A list of close neighborhood agents</param>
    public void CalculatePushForces(List<AgentController> neighborAgents)
    {
        foreach (AgentController agent in neighborAgents)
        {
            if (agent != this)
            {
                var d = Vector3.Distance(this.transform.position, agent.transform.position);
                if (!cannotBePushed)
                {
                    Vector3 pushForce = this.transform.position - agent.transform.position;
                    pushForce = pushForce.normalized;
                    pushForce *= (pushRadius - d);
                    pushForce = new Vector3(pushForce.x, 0f, pushForce.z);
                    PushingForce += pushForce * pushWeight * Time.deltaTime;
                    NoOfPushers++;
                }
                cannotBePushed = false;

                if (agent.TargetReached)
                {
                    pushesToStandingAgents = true;
                    numberOfPushesOnStandingAgent++;
                }
            }
        }
    }

    /// <summary>
    /// Forces the agent to stop
    /// </summary>
    public void Stop()
    {
        currentWaypoint = 0;
        targetReached = true;
        targetDistanceMultiplier = 1;
        numberOfPushesOnStandingAgent = 0;
    }

    /// <summary>
    /// Returns the distance from a to b only in the X-Z plane.
    /// </summary>
    /// <param name="a">Start point</param>
    /// <param name="b">End point</param>
    /// <returns></returns>
    float XZDistance(Vector3 a, Vector3 b)
    {
        float dx = b.x - a.x;
        float dz = b.z - a.z;

        return dx * dx + dz * dz;
    }

    private void OnDestroy()
    {
        AgentManager.Instance.RemoveAgent(this);
    }

}
