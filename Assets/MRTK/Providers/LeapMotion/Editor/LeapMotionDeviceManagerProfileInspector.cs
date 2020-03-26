
using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.LeapMotion.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.LeapMotion.Inspectors
{
#if LEAPMOTIONCORE_PRESENT

    [CustomEditor(typeof(LeapMotionDeviceManagerProfile))]
    public class LeapMotionDeviceManagerProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private const string ProfileTitle = "Leap Motion Settings";
        private const string ProfileDescription = "";
        private SerializedProperty leapControllerLocation;

        protected override void OnEnable()
        {
            base.OnEnable();

            leapControllerLocation = serializedObject.FindProperty("leapControllerLocation");
        }

        public override void OnInspectorGUI()
        {
            if (!RenderProfileHeader(ProfileTitle, ProfileDescription, target, true, BackProfileType.Input))
            {
                return;
            }

            RenderCustomInspector();
        }

        public virtual void RenderCustomInspector()
        {
            serializedObject.Update();

            // Disable ability to edit ToggleList through the inspector if in play mode 
            bool isPlayMode = EditorApplication.isPlaying || EditorApplication.isPaused;
            using (new EditorGUI.DisabledScope(isPlayMode))
            {
                EditorGUILayout.PropertyField(leapControllerLocation);
            }
            serializedObject.ApplyModifiedProperties();
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            return MixedRealityToolkit.IsInitialized && profile != null &&
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile != null &&
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.DataProviderConfigurations != null &&
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.DataProviderConfigurations.Any(s => profile == s.DeviceManagerProfile);
        }

    }
#endif
}
