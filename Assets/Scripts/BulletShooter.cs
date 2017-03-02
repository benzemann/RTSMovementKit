using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShooter : MonoBehaviour {
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private Transform barrelExit;
    [SerializeField]
    private float damage;
    [SerializeField]
    private float shootingRate;
    [SerializeField]
    private float bulletSpeed;

    private float timeAtLastShot;
    private ObjectPool bulletPool;

    private void Start()
    {
        bulletPool = new ObjectPool(bullet, 5, true);
    }

    public void Shoot(GameObject target)
    {
        if (Time.time - timeAtLastShot < shootingRate)
            return;
        timeAtLastShot = Time.time;

        GameObject bulletInstance = bulletPool.GetPooledObject();
        bulletInstance.SetActive(true);
        bulletInstance.transform.position = barrelExit.transform.position;
        bulletInstance.GetComponent<Bullet>().Seek(target, damage, bulletSpeed);
    }
}
