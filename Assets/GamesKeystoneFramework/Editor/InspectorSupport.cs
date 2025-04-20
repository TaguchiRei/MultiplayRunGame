using UnityEditor;
using UnityEngine;

namespace GamesKeystoneFramework.Editor
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class InspectorSupport : UnityEditor.Editor
    {
        private void OnEnable()
        {

        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
