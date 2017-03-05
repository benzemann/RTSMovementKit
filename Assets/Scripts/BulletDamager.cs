using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamager : Damager
{

    [SerializeField, Tooltip("The bullet prefab")]
    private GameObject bullet;
    [SerializeField, Tooltip("The transform of the object where the bullet shall spawn")]
    private Transform barrelExit;
    [SerializeField, Tooltip("The speed of the bullets")]
    private float bulletSpeed;

    private float timeAtLastShot;
    private ObjectPool bulletPool;

    private void Start()
    {
        // Uses an objectpool for bullets.
        bulletPool = new ObjectPool(bullet, 5, true);
    }

    private void Update()
    {
        if(GetComponent<TargetFinder>().Target != null)
        {
            var target = GetComponent<TargetFinder>().Target;
            var distance = Vector3.Distance(target.transform.position, transform.position);
            if(distance <= attackRange)
            {
                // Stop moving if walking
                if(GetComponent<AgentController>() != null && !GetComponent<AgentController>().TargetReached)
                {
                    GetComponent<AgentController>().Stop();
                }
                // Try to shoot at target
                Shoot(target);
            }
        }
    }

    /// <summary>
    /// Shoot a bullet towards the target
    /// </summary>
    /// <param name="target">The target</param>
    public void Shoot(GameObject target)
    {
        if(!RotateTowardsTarget() || Time.time - timeAtLastShot < attackSpeed)
            return;

        timeAtLastShot = Time.time;

        GameObject bulletInstance = bulletPool.GetPooledObject();
        bulletInstance.SetActive(true);
        bulletInstance.transform.position = barrelExit.transform.position;
        bulletInstance.GetComponent<Bullet>().Seek(target, damage, bulletSpeed);
        
    }

}
