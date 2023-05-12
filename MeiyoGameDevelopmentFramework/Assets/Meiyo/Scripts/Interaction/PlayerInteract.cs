using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System;

public class PlayerInteract : MonoBehaviour
{
    InputActions _input = null;
    //public bool InteractWasPressedThisFrame { get; private set; } = false;
    private bool interactWasPressedThisFrame = false;

    private List<GameObject> interactiveObjectsInRange;
    private SphereCollider _collider;
    private Transform _mainCamera;

    [SerializeField] CinemachineVirtualCamera cinemachine3rdPerson;
    CinemachineFramingTransposer _cinemachine3rdPersonTransposer;

    private void Start()
    {
        _collider = GetComponent<SphereCollider>();
        _cinemachine3rdPersonTransposer = cinemachine3rdPerson.GetCinemachineComponent<CinemachineFramingTransposer>();
        _mainCamera = GameObject.FindWithTag("MainCamera").transform;
        interactiveObjectsInRange = new List<GameObject>();
    }

    private void Update()
    {
        interactWasPressedThisFrame = _input.InteractionMap.Interact.WasPerformedThisFrame();

        if (interactiveObjectsInRange.Count != 0)
        {
            // Cast a ray that can only interact with objects in the "Interactive" layer
            int layerMask = 1 << LayerMask.NameToLayer("Interactive");
            RaycastHit hit;
            if (Physics.Raycast(_mainCamera.position, _mainCamera.forward, out hit, _cinemachine3rdPersonTransposer.m_CameraDistance + _collider.radius, layerMask))
            {
                Interactive interactive = hit.collider.gameObject.GetComponent<Interactive>();

                if (interactive != null)
                {
                    if (interactWasPressedThisFrame && interactive.CanInteract)
                    {
                        interactive.Animate();
                        interactive.PlayAudio();
                    }


                    InteractiveType type = interactive.GetInteractiveType();
                    switch (type)
                    {
                        case InteractiveType.NPC:
                            Debug.Log("Talk [E]"); // will be replaced by UI
                                                   // do something
                            TalkToNpc(interactive);
                            break;

                        case InteractiveType.Door:
                            Debug.Log("Open [E]"); // will be replaced by UI
                                                   // do something
                            if (interactWasPressedThisFrame && interactive.CanInteract) { interactive.CanInteract = false; interactive.Door(); }
                            break;

                        case InteractiveType.Gate:
                            Debug.Log("Open [E]"); // will be replaced by UI
                                                   // do something
                            if (interactWasPressedThisFrame && interactive.CanInteract) { interactive.CanInteract = false; interactive.Gate(); }
                            break;

                        case InteractiveType.Button:
                            Debug.Log("Press [E]"); // will be replaced by UI
                                                    // do something
                            if (interactWasPressedThisFrame && interactive.CanInteract) { interactive.CanInteract = false; interactive.ButtonLever(); }
                            break;

                        case InteractiveType.Lever:
                            Debug.Log("Pull [E]"); // will be replaced by UI
                                                   // do something
                            if (interactWasPressedThisFrame && interactive.CanInteract) { interactive.CanInteract = false; interactive.ButtonLever(); }
                            break;

                        case InteractiveType.Portal:
                            Debug.Log("Teleport [E]"); // will be replaced by UI
                                                       // do something
                            if (interactWasPressedThisFrame && interactive.CanInteract) { interactive.CanInteract = false; interactive.Portal(); }
                            break;

                        default:
                            throw new Exception("Unknown interactive type.");
                    }
                }

                CollectableItem collectable = hit.collider.gameObject.GetComponent<CollectableItem>();

                if (collectable != null)
                {
                    Debug.Log("Take " + collectable.item.itemName + " " + "[E]");
                    if (interactWasPressedThisFrame) { collectable.Pickup(); }
                }
            }
            else
            {
                // close the Interaction UI by setting it to false
            }
        }
    }

    private void TalkToNpc(Interactive interactive)
    {
        if (interactWasPressedThisFrame)
        {
            // We need to prevent any further interaction while in dialogue
            // We could do this by changing the layer of this object from Interactive
            // And then change it back when dialogue ends
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        interactiveObjectsInRange.Add(other.gameObject);
    }

    public void OnTriggerExit(Collider other)
    {
        interactiveObjectsInRange.Remove(other.gameObject);
    }

    private void OnEnable()
    {
        _input = new InputActions();
        _input.InteractionMap.Enable();
    }

    private void OnDisable()
    {
        _input.InteractionMap.Disable();
    }
}
