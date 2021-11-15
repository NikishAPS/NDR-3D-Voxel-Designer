using System;
using UnityEngine;

public static class ResourcesLoader
{
    public static T Load<T>(string path) where T : UnityEngine.Object
    {
        T resource = Resources.Load<T>(path);

        if (resource == null)
            throw new NullReferenceException($"Resource loading error: { path }");

        return resource;
    }
}