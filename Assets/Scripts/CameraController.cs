using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera frontCamera;
    public Camera rearCamera;

    void Start()
    {
        ActivateFront();
    }

    void ActivateFront()
    {
        frontCamera.enabled = true;
        rearCamera.enabled = false;
    }

    void ActivateRear()
    {
        frontCamera.enabled = false;
        rearCamera.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("TopPlayer"))
        {
            Debug.Log("Entered Trigger Region");
            if (!rearCamera.enabled)
                ActivateRear();
            else if (rearCamera.enabled)
                ActivateFront();
        }

    }

}