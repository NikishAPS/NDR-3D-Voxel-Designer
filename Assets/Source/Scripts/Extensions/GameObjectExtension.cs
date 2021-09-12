using UnityEngine;

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
}
