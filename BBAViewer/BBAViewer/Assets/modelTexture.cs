using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach this script to an object to detect wether the mouse is hovering the object.
/// Here, we use it to allow zooming in and out of the model view only if the mouse is in the view.
/// </summary>
public class modelTexture : MonoBehaviour
{
    private static bool canScroll = false;

    /// <summary>
    /// Create events and add listeners to other methods
    /// </summary>
    void Start()
    {
        EventTrigger triggerBegin = GetComponent<EventTrigger>();
        EventTrigger triggerEnd = GetComponent<EventTrigger>();
        EventTrigger.Entry entryBegin = new EventTrigger.Entry();
        EventTrigger.Entry entryEnd = new EventTrigger.Entry();
        entryBegin.eventID = EventTriggerType.PointerEnter;
        entryEnd.eventID = EventTriggerType.PointerExit;
        entryBegin.callback.AddListener((data) => { OnPointerEnterDelegate((PointerEventData)data); });
        entryEnd.callback.AddListener((data) => { OnPointerExitDelegate((PointerEventData)data); });
        triggerBegin.triggers.Add(entryBegin);
        triggerEnd.triggers.Add(entryEnd);
    }

    /// <summary>
    /// Event called when the user's mouse is begining to hover the object
    /// </summary>
    /// <param name="data"></param>
    public void OnPointerEnterDelegate(PointerEventData data)
    {
        canScroll = true;
    }

    /// <summary>
    /// Event called when the user's mouse is exiting the object's view
    /// </summary>
    /// <param name="data"></param>
    public void OnPointerExitDelegate(PointerEventData data)
    {
        canScroll = false;
    }

    public static bool CanScroll()
    {
        return canScroll;
    }
}
