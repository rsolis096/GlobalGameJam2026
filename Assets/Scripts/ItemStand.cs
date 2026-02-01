using Unity.VisualScripting;
using UnityEngine;

public class ItemStand : MonoBehaviour
{
    public Grabbable holdingObject = null;
    public GameObject ItemShowcase = null;

    Renderer r;
    MaterialPropertyBlock mpb;

    static readonly int ScaleID = Shader.PropertyToID("_Scale");

    [Header("Highlight Scale")]
    public float offScale = 0f;
    public float onScale = 1.05f;

    public void PlaceItem(Grabbable itemToPlace)
    {

        if (itemToPlace && !holdingObject)
        {

            Debug.Log("Plcae Item Called!");

            itemToPlace.transform.SetParent(this.transform);

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

    void Awake()
    {
        if (!ItemShowcase) return;

        r = ItemShowcase.GetComponent<Renderer>();
        if (!r) return;

        if (r.sharedMaterials == null || r.sharedMaterials.Length < 1)
        {
            Debug.LogWarning($"{name} does not have 1 material");
            return;
        }

        mpb = new MaterialPropertyBlock();
        HighlightOff();
    }

    public void HighlightOn()
    {
        if (!r) return;

        r.GetPropertyBlock(mpb, 0);
        mpb.SetFloat(ScaleID, onScale);
        r.SetPropertyBlock(mpb, 0);
    }

    public void HighlightOff()
    {
        if (!r) return;

        r.GetPropertyBlock(mpb, 0);
        mpb.SetFloat(ScaleID, offScale);
        r.SetPropertyBlock(mpb, 0);
    }
}
