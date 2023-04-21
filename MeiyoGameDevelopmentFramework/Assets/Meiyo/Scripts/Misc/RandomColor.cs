using UnityEngine;

public class RandomColor : MonoBehaviour
{
    private void Awake()
    {
        // This comment is a test to see if updates work as expected via git...
        GetComponent<MeshRenderer>().material.color = Random.ColorHSV(0.0f, 1.0f, 0.75f, 1.0f, 0.5f, 1.0f);
    }
}
