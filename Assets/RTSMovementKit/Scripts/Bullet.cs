using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    GameObject target;
    float speed;
    float damage;
    Vector3 targetPos;
    Vector3 dir;
    bool noTarget = false;
    public void Seek(GameObject t, float d, float s)
    {
        target = t;
        damage = d;
        speed = s;
        targetPos = t.transform.position;
    }

	// Update is called once per frame
	void Update () {
        if(target != null)
        {
            targetPos = target.transform.position;
            dir = targetPos - transform.position;
        } else if(target == null && noTarget == false)
        {
            noTarget = true;
            dir = dir.normalized * 10.0f;
        }
        
        float disThisFrame = speed * Time.deltaTime;

        if(dir.magnitude <= disThisFrame)
        {
            HitTarget();
            this.gameObject.SetActive(false);
            return;
        }

        transform.Translate(dir.normalized * disThisFrame, Space.World);
        transform.LookAt(targetPos, Vector3.up);
    }

    void HitTarget()
    {
        if (target == null || target.GetComponent<Health>() == null)
            return;

        target.GetComponent<Health>().Damage(damage);
    }


}
