using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Drop()
    {
        if (rb)
        {
            rb.useGravity = true;
            rb.isKinematic = false;

            Collider grabbableCollider = GetComponent<Collider>();
            if (grabbableCollider) grabbableCollider.enabled = true;
        }
    }

    public void Pickup()
    {
        if (rb)
        {
            rb.useGravity = false;
            rb.isKinematic = true;

            Collider grabbableCollider = GetComponent<Collider>();
            if (grabbableCollider) grabbableCollider.enabled = false;
        }
    }
}
