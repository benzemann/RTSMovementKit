using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    GameObject target;
    float speed;
    float damage;

    public void Seek(GameObject t, float d, float s)
    {
        target = t;
        damage = d;
        speed = s;
    }

	// Update is called once per frame
	void Update () {
        if (target == null)
        {
            this.gameObject.SetActive(false);
            return;
        }

        Vector3 dir = target.transform.position - transform.position;
        float disThisFrame = speed * Time.deltaTime;

        if(dir.magnitude <= disThisFrame)
        {
            HitTarget();
            this.gameObject.SetActive(false);
            return;
        }

        transform.Translate(dir.normalized * disThisFrame, Space.World);
        transform.LookAt(target.transform.position, Vector3.up);
    }

    void HitTarget()
    {
        if (target.GetComponent<Health>() == null)
            return;

        target.GetComponent<Health>().Damage(damage);
    }


}
