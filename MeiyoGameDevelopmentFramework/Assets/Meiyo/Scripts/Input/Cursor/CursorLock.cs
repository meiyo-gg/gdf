using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLock : MonoBehaviour
{
    [SerializeField] private List<GameObject> UIPanels;

    private void Start()
    {
        SetCursorLockState();
    }

    public void SetCursorLockState()
    {
        foreach (GameObject panel in UIPanels)
        {
            if (panel.activeInHierarchy)
            {
                Cursor.lockState = CursorLockMode.None;
                return;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                return;
            }
        }
        // Can perform other checks here e.g. if cursor is locked/unlocked for a different reason to UI
    }
}