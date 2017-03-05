using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TargetFinder : MonoBehaviour {
    [SerializeField, Tooltip("You can search for potential targets using the custom team mechanics."+
        "Just leave target tag empty and set this to the target team, then it will search for all targets"+
        "assigned to a team using the team component. This is more perfomance friendly than using the targetTag.")]
    private Teams targetTeam;
    [SerializeField, Tooltip("The tag of the target.")]
    private string targetTag;
    [SerializeField, Tooltip("The radius it will look for enemies.")]
    private float searchRadius;

    private GameObject _currentTarget;
    // Restrict the search for targets to one pr frame.
    private bool hasSearchedInThisFrame;

    public bool GoToAndAttack { get; set; }

    private void Start()
    {
        GoToAndAttack = false;
    }

    public GameObject Target {
        get
        {
            return _currentTarget;
        }
        set
        {
            _currentTarget = value;
        }
    }

    public void Update()
    {
        if(_currentTarget == null && !hasSearchedInThisFrame)
        {
            if((GetComponent<AgentController>() != null ? GetComponent<AgentController>().IsReady : true) ||
                GoToAndAttack)
                FindTarget();
        } else
        {
            if (GetComponent<Vision>() != null && !GetComponent<Vision>().CanISeeIt(_currentTarget))
            {
                if (GetComponent<AgentController>() != null)
                {
                    if(GetComponent<AgentController>().IsReady)
                        GetComponent<AgentController>().GoToPos(_currentTarget.transform.position);
                } else
                {
                    _currentTarget = null;
                }
                    
            }
        }
    }

    public void LateUpdate()
    {
        hasSearchedInThisFrame = false;
    }

    /// <summary>
    /// Find the closest target
    /// </summary>
    public void FindTarget()
    {
        hasSearchedInThisFrame = true;
        // Get all potential targets
        GameObject[] potentialTargets;

        // Get all potential targets either by tag or the teammanager
        if(targetTag != "")
        {
            potentialTargets = GameObject.FindGameObjectsWithTag(targetTag);
        } else
        {
            potentialTargets = TeamManager.Instance.GetAllObjectsInTeam(targetTeam);
        }
        
        // Find the closest target
        float closestDistance = float.MaxValue;
        for(int i = 0; i < potentialTargets.Length; i++)
        {
            float distance = Vector3.Distance(this.transform.position, potentialTargets[i].transform.position);
            if(distance < closestDistance && distance <= searchRadius)
            {
                // If this object has a vision component, then check if the potential target is visible
                if(GetComponent<Vision>() != null)
                {
                    if (GetComponent<Vision>().CanISeeIt(potentialTargets[i]))
                    {
                        _currentTarget = potentialTargets[i];
                        closestDistance = distance;
                    } 
                } else
                {
                    _currentTarget = potentialTargets[i];
                    closestDistance = distance;
                }
            } 
        }
    }

}
