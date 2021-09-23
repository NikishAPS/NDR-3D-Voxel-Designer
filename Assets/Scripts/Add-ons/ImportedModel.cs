using UnityEngine;
using System.Collections;

public class ImportedModel
{
    public readonly Transform Transform;
    public int Transparency
    {
        get
        {
            return _transparency;
        }
        set
        {
            _transparency = value;
            foreach(MeshRenderer meshRenderer in meshRenderers)
            {
                foreach (Material material in meshRenderer.materials)
                {
                    material.SetColor("_Color", new Color(1, 1, 1, value));
                }
            }
        }
    }

    private int _transparency;
    private readonly MeshRenderer[] meshRenderers;

    public ImportedModel(Transform transform, Material material)
    {
        Transform = transform;
        meshRenderers = Transform.GetComponentsInChildren<MeshRenderer>();
        for(int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].transform.tag = "OBJ";
            meshRenderers[i].gameObject.AddComponent<MeshCollider>();


            Material[] materials = meshRenderers[i].materials;
            for(int j = 0; j < materials.Length; j++)
            {
                materials[j] = material;
            }
            meshRenderers[i].materials = materials;

        }
    }

    public void SetTransparency(float value)
    {
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            Material[] materials = meshRenderers[i].materials;
            for (int j = 0; j < materials.Length; j++)
            {
                materials[j].SetColor("_Color", new Color(1, 1, 1, value));
            }
            meshRenderers[i].materials = materials;
        }
    }
}
