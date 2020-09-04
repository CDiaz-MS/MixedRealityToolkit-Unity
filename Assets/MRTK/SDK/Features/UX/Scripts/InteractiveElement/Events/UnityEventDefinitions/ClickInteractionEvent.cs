using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// A Unity event with MixedRealityPointerEventData. This event is used in the event configuration for the 
    /// Click state.
    /// </summary>
    [System.Serializable]
    public class ClickInteractionEvent : UnityEvent<MixedRealityPointerEventData> { }
}