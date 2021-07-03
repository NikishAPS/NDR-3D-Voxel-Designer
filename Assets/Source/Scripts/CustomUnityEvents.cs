using System;
using UnityEngine.Events;


namespace CustomUnityEvents
{
    [Serializable]
    public class EventInt : UnityEvent<int> { }

    [Serializable]
    public class EventFloat : UnityEvent<float> { }
}
