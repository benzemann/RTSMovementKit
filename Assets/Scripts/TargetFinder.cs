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
            if (_currentTarget == null && !hasSearchedInThisFrame)
                FindTarget();
            return _currentTarget;
        }
    }

    public void Update()
    {
        if (alwaysSearch && _currentTarget == null && !hasSearchedInThisFrame)
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
    private void FindTarget()
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
                _currentTarget = potentialTargets[i];
                closestDistance = distance;
            }
        }
    }

}
