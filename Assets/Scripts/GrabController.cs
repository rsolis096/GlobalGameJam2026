using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabController : MonoBehaviour
{
    public Transform GrabHolder;
    public PlayerController OwningPlayer;

    // Update is called once per frame
    void Update()
    {
        if (!OwningPlayer) return;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.isPressed) AttemptGrab();
        }        
    }

    void AttemptGrab()
    {
        if (nearbyGrabbable)
        {
            Debug.Log("Grab Called");

            nearbyGrabbable.Pickup();

            nearbyGrabbable.transform.SetParent(GrabHolder);
            nearbyGrabbable.transform.position = GrabHolder.position;
        }
    }

    Grabbable nearbyGrabbable = null;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
        {
            nearbyGrabbable = other.gameObject.GetComponent<Grabbable>();
            Debug.Log("Entered trigger: " + other.name);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
        {
            nearbyGrabbable = null;
            Debug.Log("Exited trigger: " + other.name);
        }
    }

}
