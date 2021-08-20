using System;
using UnityEngine.Events;


namespace CustomUnityEvents
{
    [Serializable]
    public class EventInt : UnityEvent<int> { }

    [Serializable]
    public class EventFloat : UnityEvent<float> { }

    [Serializable]
    public class EventString : UnityEvent<string> { }

    [Serializable]
    public class EventObject : UnityEvent<object> { }

    [Serializable]
    public class EventCommand : UnityEvent<Command> { }

}
