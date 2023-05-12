using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;

public class UserInterfaceInput : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private CursorLock _cursorLock;

    private GameObject characterInputHandler;

    public bool InventoryIsPressed { get; private set; } = false;

    public bool RemoveItemIsPressed { get; private set; } = false;

    InputActions _input = null;

    private void OnEnable()
    {
        _input = new InputActions();
        _input.UI.Enable();

        _input.UI.Inventory.started += SetInventory;
        _input.UI.Inventory.canceled += SetInventory; // ?

        _input.UI.RemoveItem.started += RemoveSelectedItemFromInventory;

        characterInputHandler = GameObject.FindWithTag("InputHandler");
    }

    private void OnDisable()
    {
        _input.UI.Inventory.started -= SetInventory;
        _input.UI.Inventory.canceled -= SetInventory;
        _input.UI.RemoveItem.started -= RemoveSelectedItemFromInventory;
        _input.UI.Disable();
    }

    private void SetInventory(InputAction.CallbackContext ctx)
    {
        InventoryIsPressed = ctx.started;

        if (InventoryIsPressed && !_inventoryPanel.activeInHierarchy)
        {
            _inventoryPanel.SetActive(true);
            _cursorLock.SetCursorLockState();
            characterInputHandler.GetComponent<HumanoidLandInput>().ResetInputs();
            characterInputHandler.SetActive(false);
        }
        else if (InventoryIsPressed && _inventoryPanel.activeInHierarchy)
        {
            _inventoryPanel.SetActive(false);

            List<CustomPass> customPasses = GameObject.FindWithTag("ObjectUICustomPassVolume").GetComponent<CustomPassVolume>().customPasses;
            CustomPass RenderObjectOnUI = customPasses.Find(x => x.name == "RenderObjectOnUI");
            RenderObjectOnUI.enabled = false;

            _cursorLock.SetCursorLockState();
            characterInputHandler.SetActive(true);
        }
    }

    private void RemoveSelectedItemFromInventory(InputAction.CallbackContext ctx)
    {
        GameObject selectedItemButton = InventoryManager.Instance.SelectedItemButton;
        // If pressed, remove selected item from inventory list, remove selectedItem reference, reset list in UI
        //if (RemoveItemIsPressed && _inventoryManager)
    }
}
