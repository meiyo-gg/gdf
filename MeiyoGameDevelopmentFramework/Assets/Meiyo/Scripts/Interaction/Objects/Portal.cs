using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    GameObject playerController;
    [SerializeField] Transform destination;

    private void OnEnable()
    {
        playerController = GameObject.FindGameObjectWithTag("PlayerController");
    }

    public void Teleport()
    {
        /*if (playerController.TryGetComponent<Rigidbody>(out Rigidbody r))
        {
            r.position = destination.position;
            r.rotation = destination.rotation;
        }
        else
        {
            CharacterController controller = playerController.GetComponent<CharacterController>();
            controller.enabled = false;
            playerController.transform.position = destination.position;
            playerController.transform.rotation = destination.rotation;
            controller.enabled = true;
        }*/

        playerController.GetComponent<CharacterController>().enabled = false;
        playerController.transform.position = destination.position;
        playerController.transform.rotation = destination.rotation;
        playerController.GetComponent<CharacterController>().enabled = true;
    }
}
