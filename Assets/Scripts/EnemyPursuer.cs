using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TargetFinder))]
public class EnemyPursuer : MonoBehaviour {

    [SerializeField, Tooltip("How far away it attacker can attack")]
    private float attackRange;
    [SerializeField, Tooltip("Define which object should rotate towards the enemy")]
    private GameObject objectToBeRotated;
    [SerializeField, Tooltip("How fast the object should rotate toward the enemy")]
    private float rotationSpeed;
    [SerializeField]
    private LayerMask ignoreLayer;
    GameObject target;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(target == null)
        {
            target = GetComponent<TargetFinder>().Target;
            
            if (target != null && GetComponent<AgentController>() != null)
                GetComponent<AgentController>().GoToPos(target.transform.position);
        }

        if(target != null)
        {
            
            if (target != null && GetComponent<BulletShooter>() != null)
            {
                if (Vector3.Distance(target.transform.position, this.transform.position) <= attackRange)
                {
                    if (GetComponent<AgentController>() != null)
                        GetComponent<AgentController>().Stop();
                    if (objectToBeRotated != null)
                    {
                        float step = rotationSpeed * Time.deltaTime;
                        Vector3 targetDir = target.transform.position - objectToBeRotated.transform.position;
                        targetDir = new Vector3(targetDir.x, 0f, targetDir.z);
                        Vector3 newDir = Vector3.RotateTowards(objectToBeRotated.transform.forward, targetDir, step, 0.0f);
                        objectToBeRotated.transform.rotation = Quaternion.LookRotation(newDir);
                    }

                    RaycastHit hit;
                    var dir = (target.transform.position - transform.position).normalized;
                    Ray ray = new Ray(this.transform.position, dir);
                    //var layerMask = ~(1 << this.gameObject.layer);
                    if (Physics.Raycast(ray, out hit, 1000f, ignoreLayer))
                    {
                        if (hit.transform.gameObject == target)
                        {
                            GetComponent<BulletShooter>().Shoot(target);
                        } else if (hit.transform.gameObject.tag == target.tag)
                        {
                            target = hit.transform.gameObject;
                            GetComponent<BulletShooter>().Shoot(target);

                        }
                    }

                } else if (GetComponent<AgentController>() != null && GetComponent<AgentController>().TargetReached)
                {
                    GetComponent<AgentController>().GoToPos(target.transform.position);
                }
            }
        }
	}

    public void ClearTarget()
    {
        target = null;
    }
}
