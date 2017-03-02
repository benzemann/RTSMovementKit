using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class TargetFinder : MonoBehaviour {

    [SerializeField, Tooltip("The tag of the target.")]
    private string targetTag;
    [SerializeField, Tooltip("The radius it will look for enemies.")]
    private float searchRadius;
    [SerializeField, Tooltip("Whether it should always look for enemies. If false, some other scripts must use this component.")]
    private bool alwaysSearch;


    private GameObject _currentTarget;

    private bool hasSearchedInThisFrame;

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
        if ((alwaysSearch || (GetComponent<AgentController>() != null ? (!GetComponent<AgentController>().IsSearchingForPath && GetComponent<AgentController>().TargetReached) : false) ) && _currentTarget == null && !hasSearchedInThisFrame)
        {
            FindTarget();
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
        GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag(targetTag);
        
        // Find the closest target
        float closestDistance = float.MaxValue;
        for(int i = 0; i < potentialTargets.Length; i++)
        {
            float distance = Vector3.Distance(this.transform.position, potentialTargets[i].transform.position);
            if(distance < closestDistance && distance <= searchRadius)
            {

                RaycastHit hit;
                var dir = (potentialTargets[i].transform.position - transform.position).normalized;
                Ray ray = new Ray(this.transform.position, dir);
                var layerMask = ~(1 << this.gameObject.layer);
                if (Physics.Raycast(ray, out hit, 1000f, layerMask))
                {
                    if(hit.transform.gameObject == potentialTargets[i])
                    {
                        _currentTarget = potentialTargets[i];
                        closestDistance = distance;
                    }
                } 
            } 
        }
    }

}
