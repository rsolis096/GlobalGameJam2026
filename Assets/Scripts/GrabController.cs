using UnityEngine;
using UnityEngine.InputSystem;

public class GrabController : MonoBehaviour
{
    public Transform GrabHolder;

    Grabbable nearbyGrabbable = null;
    Grabbable holdingGrabbable = null;
    ItemStand nearbyItemStand = null;

    [Header("Input")]
    [SerializeField] float actionCooldown = 0.15f;
    float nextActionTime = 0f;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
        {
            nearbyGrabbable = other.GetComponent<Grabbable>();
            nearbyGrabbable.HighlightOn();
            Debug.Log("Entered trigger: " + other.name);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("ItemStand"))
        {
            nearbyItemStand = other.GetComponent<ItemStand>();
            Debug.Log("Entered trigger: " + other.name);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
        {
            nearbyGrabbable?.HighlightOff();
            if (other.GetComponent<Grabbable>() == nearbyGrabbable)
                nearbyGrabbable = null;

            Debug.Log("Exited trigger: " + other.name);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("ItemStand"))
        {
            if (other.GetComponent<ItemStand>() == nearbyItemStand)
                nearbyItemStand = null;

            Debug.Log("Exited trigger: " + other.name);
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (!Keyboard.current.eKey.wasPressedThisFrame) return;

        if (Time.time < nextActionTime) return;
        nextActionTime = Time.time + actionCooldown;

        if (holdingGrabbable != null && nearbyItemStand != null)
        {
            AttemptPlace();
        }
        else if (holdingGrabbable == null && nearbyGrabbable != null)
        {
            AttemptGrab();
        }
    }

    void AttemptPlace()
    {
        if (holdingGrabbable != null && nearbyItemStand != null)
        {
            Debug.Log("Place Called");
            holdingGrabbable.transform.SetParent(null);
            nearbyItemStand.PlaceItem(holdingGrabbable);
            holdingGrabbable = null;
        }
    }

    void AttemptGrab()
    {
        if (nearbyGrabbable != null)
        {
            Debug.Log("Grab Called");

            nearbyGrabbable.GetComponentInParent<ItemStand>()?.RemoveItem();

            nearbyGrabbable.transform.SetParent(GrabHolder);
            nearbyGrabbable.transform.position = GrabHolder.position;
            //nearbyGrabbable.transform.localPosition = GrabHolder.localPosition;
            nearbyGrabbable.transform.localRotation = Quaternion.Euler(Vector3.zero);

            holdingGrabbable = nearbyGrabbable;
        }
    }
}
