using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetterMaterial : MonoBehaviour
{
    public Material material;

    void OnEnable()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        for(int i = 0; i < meshRenderer.materials.Length;i++)
        {
            meshRenderer.materials[i] = material;
        }

        meshRenderer.materials[1] = material;
        print(meshRenderer.materials[1].name);
    }

    void Update()
    {
        
    }
}
