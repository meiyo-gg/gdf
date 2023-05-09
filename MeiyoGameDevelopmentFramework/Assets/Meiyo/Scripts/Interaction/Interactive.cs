using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive : MonoBehaviour
{
    [Header("References")]
    [SerializeField] InteractiveType interactiveType;
    [SerializeField] Animator animator;
    [SerializeField] AudioClip audioClip;
    
    [Header("Buttons and Levers")]
    [SerializeField] List<GameObject> objectsToBeActivated = new List<GameObject>();

    [Header("Doors and Gates")]
    [SerializeField, Range(-180, 180)] float doorOpenAngle;
    [SerializeField, Range(-10, 10)] float gateOpenDistance;
    [SerializeField] bool verticalGate = false;
    [SerializeField, Range(0, 10)] float timeToComplete = 1;
    bool open = false;

    // Other parameters
    bool canInteract = true;
    public bool CanInteract { get { return canInteract; } set { canInteract = value; } }

    public InteractiveType GetInteractiveType()
    {
        return interactiveType;
    }

    public void Animate()
    {
        if (animator != null)
        {
            // play animation
            if (!animator.GetBool("On"))
                animator.SetBool("On", true);
            else
                animator.SetBool("On", false);
        }
    }

    public void PlayAudio()
    {
        if (audioClip != null)
        {
            // play audio
        }
    }

    public void Door()
    {
        TriggerEvent(this);
    }

    public void ButtonLever()
    {
        // Pressing a button or pulling a lever can trigger an event
        foreach (GameObject go in objectsToBeActivated)
        {
            // The TriggerEvent() method of all components attached to go will be called (if the method exists)
            go.SendMessage("TriggerEvent", this);
        }
    }

    public void Gate()
    {
        TriggerEvent(this);
    }

    public void Portal()
    {
        GetComponent<Portal>().Teleport();
        CanInteract = true;
    }


    // This method may be called by buttons/levers to open and close doors/gates
    public void TriggerEvent(Interactive sender)
    {
        
        if (interactiveType == InteractiveType.Door)
        {
            StartCoroutine(OpenCloseDoor(sender));
        }
        else // interactiveType == InteractiveType.Gate
        {
            StartCoroutine(OpenCloseGate(sender));
        }
    }

    IEnumerator OpenCloseDoor(Interactive sender)
    {
        CanInteract = false;
        Transform parent = transform.parent;
        Vector3 currentRotation = parent.eulerAngles;
        float elapsedTime = 0;
        float elapsedPercentage = 0;

        if (!open)
        {
            open = true;

            while (elapsedPercentage < 1)
            {
                elapsedTime += Time.deltaTime;
                elapsedPercentage = elapsedTime / timeToComplete;
                parent.eulerAngles = Vector3.Slerp(currentRotation, new Vector3(currentRotation.x, currentRotation.y + doorOpenAngle, currentRotation.z), elapsedPercentage);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            open = false;
            
            while (elapsedPercentage < 1)
            {
                elapsedTime += Time.deltaTime;
                elapsedPercentage = elapsedTime / timeToComplete;
                parent.eulerAngles = Vector3.Slerp(currentRotation, new Vector3(currentRotation.x, currentRotation.y - doorOpenAngle, currentRotation.z), elapsedPercentage);
                yield return new WaitForEndOfFrame();
            }
        }

        CanInteract = true;
        sender.CanInteract = true;
    }

    IEnumerator OpenCloseGate(Interactive sender)
    {
        CanInteract = false;
        Vector3 translation;
        float elapsedTime = 0;

        if (!open)
        {
            open = true;

            if (!verticalGate)
                translation = new Vector3(gateOpenDistance, 0, 0);
            else
                translation = new Vector3(0, gateOpenDistance, 0);

            while (elapsedTime < timeToComplete)
            {
                elapsedTime += Time.deltaTime;

                transform.Translate(translation * (Time.deltaTime / timeToComplete));

                yield return new WaitForEndOfFrame();
            }

            if (!verticalGate)
                transform.localPosition = new Vector3(gateOpenDistance, 1, 0);
            else
                transform.localPosition = new Vector3(0, gateOpenDistance + 1, 0);
        }
        else
        {
            open = false;

            if (!verticalGate)
                translation = new Vector3(-gateOpenDistance, 0, 0);
            else
                translation = new Vector3(0, -gateOpenDistance, 0);

            while (elapsedTime < timeToComplete)
            {
                elapsedTime += Time.deltaTime;

                transform.Translate(translation * (Time.deltaTime / timeToComplete));

                yield return new WaitForEndOfFrame();
            }

            transform.localPosition = new Vector3(0, 1, 0);
        }

        CanInteract = true;
        sender.CanInteract = true;
    }
}
