using UnityEngine;

public class Grabbable : MonoBehaviour
{
    MaterialPropertyBlock mpb;

    public Renderer HighlightedObject;

    static readonly int ScaleID = Shader.PropertyToID("_Scale");

    [Header("Highlight Scale")]
    public float offScale = 0f;
    public float onScale = 1.05f;

    public bool canBeGrabbed = true;

    void Awake()
    {
        if (!HighlightedObject) return;

        if (HighlightedObject.sharedMaterials == null || HighlightedObject.sharedMaterials.Length < 2)
        {
            Debug.LogWarning($"{name} does not have 2 materials");
            return;
        }

        mpb = new MaterialPropertyBlock();
        HighlightOff(); 
    }

    public void HighlightOn()
    {
        if (!HighlightedObject || !canBeGrabbed) return;

        HighlightedObject.GetPropertyBlock(mpb, 3);     
        mpb.SetFloat(ScaleID, onScale);
        HighlightedObject.SetPropertyBlock(mpb, 3);
    }

    public void HighlightOff()
    {
        if (!HighlightedObject) return;

        HighlightedObject.GetPropertyBlock(mpb, 3);     
        mpb.SetFloat(ScaleID, offScale);
        HighlightedObject.SetPropertyBlock(mpb, 3);
    }
}
