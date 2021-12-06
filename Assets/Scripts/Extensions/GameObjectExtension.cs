using UnityEngine;
using UnityEngine.Rendering;

public static class GameObjectExtension
{
    public static GameObject Create(this GameObject gameObject, string name, Transform parent, Vector3 position, Quaternion rotation)
    {
        gameObject.name = name;
        gameObject.transform.SetParent(parent);
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;

        return gameObject;
    }

    public static void  AddMeshRenderer(this GameObject gameObject, Material material, bool castShadow)
    {
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
        meshRenderer.shadowCastingMode = castShadow ? ShadowCastingMode.On : ShadowCastingMode.Off;
    }
}
