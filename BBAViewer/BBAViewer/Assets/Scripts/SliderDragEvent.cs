using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// The slider with this script will capture drag events and relay them to the audio control
/// </summary>
public class SliderDragEvent : MonoBehaviour
{

    private bool dragging = false;

    /// <summary>
    /// Create events and add listeners to other methods
    /// </summary>
    void Start()
    {
        EventTrigger triggerBegin = GetComponent<EventTrigger>();
        EventTrigger triggerEnd = GetComponent<EventTrigger>();
        EventTrigger.Entry entryBegin = new EventTrigger.Entry();
        EventTrigger.Entry entryEnd = new EventTrigger.Entry();
        entryBegin.eventID = EventTriggerType.BeginDrag;
        entryEnd.eventID = EventTriggerType.EndDrag;
        entryBegin.callback.AddListener((data) => { OnBeginDragDelegate((PointerEventData)data); });
        entryEnd.callback.AddListener((data) => { OnEndDragDelegate((PointerEventData)data); });
        triggerBegin.triggers.Add(entryBegin);
        triggerEnd.triggers.Add(entryEnd);
    }

    /// <summary>
    /// Event called when the user start seeking
    /// </summary>
    /// <param name="data"></param>
    public void OnBeginDragDelegate(PointerEventData data)
    {
        Debug.Log("Dragging.");
        dragging = true;
        // Pause audio to avoid scratching sounds while seeking
        LoadZip.AudioPause();
    }

    /// <summary>
    /// Event called when the user ends seeking
    /// </summary>
    /// <param name="data"></param>
    public void OnEndDragDelegate(PointerEventData data)
    {
        Debug.Log("Dragging.");
        dragging = false;
        // Start audio again
        LoadZip.AudioPlay();
    }

    public bool IsBeingDragged()
    {
        return dragging;
    }
}