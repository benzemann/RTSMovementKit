using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(Collider) )]
public class Selectable : MonoBehaviour {

    #region private variables
    [Header("Selection plane variables")]
    [SerializeField, Tooltip("This gameobject will be shown below the gameobject when selected")]
    private GameObject selectionProjectorPrefab;
    [SerializeField, Tooltip("The selection projector will be instatiated at local (0,0,0) + this offset")]
    private Vector3 offset;
    [SerializeField, Tooltip("The rotation of the selection projector")]
    private Vector3 rotation;
    private GameObject selectionPlane;
    private bool isSelected;
    #endregion

    #region Properties
    public bool IsSelected { get { return isSelected; } set { isSelected = value; } }
    #endregion

    // Use this for initialization
    void Start () {
        SelectableManager.Instance.AddSelectable(this);
    }

    private void Update()
    {
        if(isSelected && selectionProjectorPrefab != null)
        {
            // If there is no selectionPlane, instantiate one.
            if(selectionPlane == null)
            {
                selectionPlane = Instantiate(selectionProjectorPrefab, this.transform.position + offset, Quaternion.Euler(rotation), this.transform);
            }
            selectionPlane.SetActive(true);

        } else if (!isSelected && selectionPlane != null)
        {
            selectionPlane.SetActive(false);
        }
    }

    /// <summary>
    /// Is called when the mouse is released untop of this object.
    /// </summary>
    private void OnMouseUp()
    {
        SelectableManager.Instance.SelectObject(this);
    }

    private void OnDestroy()
    {
        SelectableManager.Instance.RemoveSelectable(this);
    }

}
