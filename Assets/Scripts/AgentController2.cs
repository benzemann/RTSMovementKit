using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class AgentController2 : MonoBehaviour
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
    [SerializeField, Tooltip("Define the layers of the ground")]
    LayerMask layerMask;
    #endregion
    #region Private
    NavMeshPath path;
    Vector3[] vPath;
    int currentWaypoint;
    bool targetReached;
    Vector3 targetPos;
    Vector3 velocity;
    Vector3 flockingForces;
    bool isStandingGround = false;
    float timeSinceLastPath;
    bool canWalkThrough = false;
    #endregion
    #region Properties
    public float MaxForce { get { return maxForce; } set { maxForce = value; } }
    public float Speed { get { return speed; } set { speed = value; } }
    public float Mass { get { return mass; } set { mass = value; } }
    public Vector3 PushingForce { get; set; }
    public bool IsStandingGround
    {
        get { return isStandingGround; }
        set { isStandingGround = value; }
    }
    public bool CanWalkThrough
    {
        get { return canWalkThrough; }
        set { canWalkThrough = value; }
    }
    public int NoOfPushers { get; set; }
    public float CurrentSpeed { get { return velocity.magnitude; } }
    public bool IsWalking {
        get {
            if (path != null)
                return true;
            return false;
        }
    }
    public float PushRadius { get { return pushRadius; } }
    public bool ApplyPushingForces { get { return applyPushingForce; } }
    #endregion

    // Use this for initialization
    void Start()
    {
        // Add this to the agent manager
        //AgentManager.Instance.AddAgent(this);
    }

    void Awake()
    {

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
        targetPos = tp;
        //seeker.StartPath(transform.position, targetPos);
        StartCoroutine(TryToSearchPath());
    }

    /// <summary>
    /// Check if the agentcontroller is searching for a path.
    /// </summary>
    /// <returns></returns>
    public bool IsSearchingForPath()
    {
        return false;
    }

    private IEnumerator TryToSearchPath()
    {
        while (true)
        {
            if (NavMesh.CalculatePath(this.transform.position, targetPos, layerMask, path))
            {
                targetReached = false;
                // Get the vector path
                vPath = path.corners;
                yield break;
            }
            yield return new WaitForSeconds(repathRate);
        }
    }

    /// <summary>
    /// Moves the agent with the current velocity
    /// </summary>
    public void ApplyVelocity()
    {
        if (path == null) return;
        if (NoOfPushers > 0)
            PushingForce /= NoOfPushers;
        NoOfPushers = 0;
        transform.Translate(PushingForce, Space.World);
        PushingForce = Vector3.zero;
        if (velocity != Vector3.zero)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(velocity);
            Quaternion restrictedRotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, maxDeltaAngle);
            transform.rotation = restrictedRotation;
        }

        Vector3 finalVelocity = transform.forward * velocity.magnitude * Time.deltaTime * speed;

        if (finalVelocity.magnitude > speed)
            finalVelocity = finalVelocity.normalized * speed;

        if (finalVelocity.magnitude > 0f)
        {
            transform.Translate(finalVelocity, Space.World);
        }
        
    }

    /// <summary>
    /// Calculate the velocity of the agent.
    /// </summary>
    public void CalculateVelocity()
    {
        // Ensures that there is a path
        if (path == null || targetReached == true)
        {
            velocity = Vector3.zero;
            return;
        }

        // Get the current xz position of the agent
        Vector3 currentPosition = this.transform.position;
        //currentPosition.y = 0.0f;

        // Check if further ahead nodes are closer, if so increment currentwaypoint
        /*while (currentWaypoint < vPath.Count - 2)
        {
            float disToCurrent = Vector3.Distance(transform.position, vPath[currentWaypoint]);
            float disToNext = Vector3.Distance(transform.position, vPath[currentWaypoint + 1]);
            if (disToNext < disToCurrent)
            {
                currentWaypoint++;
            }
            else
            {
                break;
            }
        }*/

        // Check if we need to go to next waypoint
        if (currentWaypoint <= vPath.Length - 1)
        {
            var dis = XZDistance(currentPosition, vPath[currentWaypoint]);
            // Check if agent is close enough to waypoint
            if (dis <= pickNextWaypointDistance)
            {
                currentWaypoint++;
                // Check if path is complete
                if (currentWaypoint >= vPath.Length)
                {
                    Stop();
                    return;
                }
            }
            /*else if (dis > 2f && Time.time - timeSinceLastPath >= 1f && GetComponent<Seeker>().IsDone())
            {
                GetComponent<Seeker>().StartPath(transform.position, targetPos);
                timeSinceLastPath = Time.time;
            }*/
        }

        // Calculate steering
        Vector3 desiredVelocity = Vector3.Normalize(vPath[currentWaypoint] - currentPosition) * speed;
        Vector3 steering = desiredVelocity - velocity;
        if (steering.sqrMagnitude > maxForce)
            steering = Vector3.Normalize(steering) * maxForce;
        
        steering = steering / mass;

        // Calculate velocity
        velocity = velocity + steering;
        velocity += flockingForces;

        // Make sure to mark area under the agent as walkable
        if (isStandingGround && velocity != Vector3.zero)
        {
            //ReleaseGroundNodes();
        }
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
                if (!isStandingGround)
                {
                    Vector3 pushForce = this.transform.position - agent.transform.position;
                    pushForce = pushForce.normalized;
                    pushForce *= (pushRadius - d);
                    pushForce = new Vector3(pushForce.x, 0f, pushForce.z);
                    PushingForce += pushForce * Time.deltaTime * pushWeight;
                    NoOfPushers++;
                    
                }
            }
        }
    }

    /// <summary>
    /// Forces the agent to stop
    /// </summary>
    public void Stop()
    {
        targetReached = true;
        path = null;
    }

    /// <summary>
    /// Marks the area under this agent as unwalkable, and cannot be pushed
    /// </summary>
    public void AvoidPushing()
    {
        if (isStandingGround)
            return;

        isStandingGround = true;
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
        //AgentManager.Instance.RemoveAgent(this);
    }

}
