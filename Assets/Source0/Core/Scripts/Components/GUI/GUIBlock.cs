namespace Source.Components.GUI
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    [AddComponentMenu("Source/GUI/GUI Block"), RequireComponent(typeof(EventTrigger)), DisallowMultipleComponent]
    public sealed class GUIBlock : MonoBehaviour
    {
        public static bool isBlocked;

        private EventTrigger eventTrigger;

        private void Awake()
        {
            eventTrigger = GetComponent<EventTrigger>();
            if(eventTrigger != null)
            {
                var enter = new EventTrigger.Entry();
                enter.eventID = EventTriggerType.PointerEnter;
                enter.callback.AddListener((x) => isBlocked = true);
                eventTrigger.triggers.Add(enter);

                var exit = new EventTrigger.Entry();
                exit.eventID = EventTriggerType.PointerExit;
                exit.callback.AddListener((x) => isBlocked = false);
                eventTrigger.triggers.Add(exit);
            }
        }
    }
}