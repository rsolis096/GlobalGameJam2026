using Unity.VisualScripting;
using UnityEngine;

public class ItemStand : MonoBehaviour
{
    public Grabbable holdingObject = null;

    public void PlaceItem(Grabbable itemToPlace)
    {

        if (itemToPlace && !holdingObject)
        {

            Debug.Log("Plcae Item Called!");

            itemToPlace.transform.SetParent(this.transform);

            itemToPlace.transform.position = this.transform.position;
            itemToPlace.transform.localPosition = Vector3.zero;
            itemToPlace.transform.localRotation = Quaternion.Euler(Vector3.zero);

            holdingObject = itemToPlace;
        }
    }

    public void RemoveItem()
    {
        Debug.Log("Remove Item Called!");
        holdingObject = null;
    }   
}
