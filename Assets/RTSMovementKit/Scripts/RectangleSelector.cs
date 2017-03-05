using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleSelector : MonoBehaviour {

    [SerializeField, Tooltip("The prefab of the selection plane.")]
    private GameObject selectionPlane;
    [SerializeField, Tooltip("The canvas the selection plane should be rendered on.")]
    private Canvas canvas;

    private GameObject _selectionPlane;
    private Vector2 mouseStart;

	// Use this for initialization
	void Start () {
        _selectionPlane = Instantiate(selectionPlane, canvas.transform) as GameObject;
        
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (Input.GetMouseButtonDown(0))
        {
            mouseStart = new Vector2(Input.mousePosition.x - Screen.width * 0.5f, Input.mousePosition.y - Screen.height * 0.5f);
        }
        if (Input.GetMouseButton(0))
        {
            if(!_selectionPlane.activeSelf)
                _selectionPlane.SetActive(true);
            UpdateSelectionPlane();
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (_selectionPlane.activeSelf)
            {
                SelectAllInSelectionPlane();
                _selectionPlane.SetActive(false);
            }
        }
	}

    void UpdateSelectionPlane()
    {
        var mousePos = new Vector2(Input.mousePosition.x - Screen.width * 0.5f, Input.mousePosition.y - Screen.height * 0.5f);
        var startToNow = mousePos - mouseStart;

        var rectangleScale = new Vector2(Mathf.Abs(startToNow.x), Mathf.Abs(startToNow.y));
        
        _selectionPlane.GetComponent<RectTransform>().sizeDelta = rectangleScale;
        _selectionPlane.GetComponent<RectTransform>().anchoredPosition = mouseStart + (startToNow * 0.5f);
        
    }

    void SelectAllInSelectionPlane()
    {
        Selectable[] allSelectables = SelectableManager.Instance.AllSelectables;
        
        for(int i = 0; i < allSelectables.Length; i++)
        {
            var screenPos = Camera.main.WorldToScreenPoint(allSelectables[i].transform.position);
            Vector2 v = new Vector2(screenPos.x, screenPos.y);
            if (IsInsideSelectionBox(v))
            {
                SelectableManager.Instance.SelectObject(allSelectables[i], false);
            }
        }
    }

    bool IsInsideSelectionBox(Vector2 p)
    {
        var sizeDelta = _selectionPlane.GetComponent<RectTransform>().sizeDelta;
        var point00 = new Vector2(_selectionPlane.GetComponent<RectTransform>().position.x - (sizeDelta.x * 0.5f),
                                  _selectionPlane.GetComponent<RectTransform>().position.y - (sizeDelta.y * 0.5f));
        var point11 = new Vector2(_selectionPlane.GetComponent<RectTransform>().position.x + (sizeDelta.x * 0.5f),
                                  _selectionPlane.GetComponent<RectTransform>().position.y + (sizeDelta.y * 0.5f));
        if (p.x > point00.x && p.x < point11.x && p.y > point00.y && p.y < point11.y)
            return true;
        return false;
    }


}
