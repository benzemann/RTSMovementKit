using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignToGround : MonoBehaviour
{
    [SerializeField, Tooltip("Tick this to let the object rotate to align to the ground normal")]
    private bool adjustForGroundNormal;
    [SerializeField, Tooltip("The layer of the ground")]
    private LayerMask groundLayer;
    [SerializeField, Tooltip("The offset to the ground (on the y axis)")]
    private float heightOffset;
    void LateUpdate()
    {
        //make platform adjust terrain rotation
        RaycastHit hit;
        //Make raycast direction down
        Vector3 dir = transform.TransformDirection(Vector3.down);

        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 10, groundLayer))
        {
            if (adjustForGroundNormal)
            {
                transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            }


            transform.position = hit.point + new Vector3(0f, heightOffset, 0f);

        }
    }
}
