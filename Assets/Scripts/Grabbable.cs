using UnityEngine;

public class Grabbable : MonoBehaviour
{
    Renderer r;
    MaterialPropertyBlock mpb;

    static readonly int ScaleID = Shader.PropertyToID("_Scale");

    [Header("Highlight Scale")]
    public float offScale = 0f;
    public float onScale = 1.05f;

    void Awake()
    {
        r = GetComponent<Renderer>();
        if (!r) return;

        if (r.sharedMaterials == null || r.sharedMaterials.Length < 2)
        {
            Debug.LogWarning($"{name} does not have 2 materials");
            return;
        }

        mpb = new MaterialPropertyBlock();
        HighlightOff(); 
    }

    public void HighlightOn()
    {
        if (!r) return;

        r.GetPropertyBlock(mpb, 1);     
        mpb.SetFloat(ScaleID, onScale);
        r.SetPropertyBlock(mpb, 1);
    }

    public void HighlightOff()
    {
        if (!r) return;

        r.GetPropertyBlock(mpb, 1);     
        mpb.SetFloat(ScaleID, offScale);
        r.SetPropertyBlock(mpb, 1);
    }
}
