using System;
using UnityEngine;
using UnityEngine.Events;


namespace CustomUnityEvents
{
    [Serializable]
    public class EventObject : UnityEvent<object> { }

    [Serializable]
    public class EventInt : UnityEvent<int> { }

    [Serializable]
    public class EventFloat : UnityEvent<float> { }

    [Serializable]
    public class EventString : UnityEvent<string> { }

    [Serializable]
    public class EventVector3 : UnityEvent<Vector3> { }

    [Serializable]
    public class EventCommand : UnityEvent<Command> { }

    [Serializable]
    public class EventColor : UnityEvent<Color> { }

    [Serializable]
    public class EventBool : UnityEvent<bool> { }

}
