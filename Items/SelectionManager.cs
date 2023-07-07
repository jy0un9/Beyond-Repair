using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    private Transform _selection;
    private Outline2 _outline;
    public LayerMask Interactable;
    
    private void Start()
    {
        _outline = GetComponent<Outline2>();
    }
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (_selection != null)
            {
                _outline.enabled = false;
                _selection = null;
            } 

            if (Physics.Raycast(ray, out hit, 4, Interactable))
            {
                _selection = hit.transform;
                _outline = _selection.gameObject.GetComponent<Outline2>();
                
                if (_outline == null)
                {
                    _outline = _selection.gameObject.AddComponent<Outline2>();
                }
                _outline.enabled = true;
            }
        }  
    }
}