using System;
using System.Collections;
using UnityEngine;

namespace GamesKeystoneFramework.MethodSupport
{
    class InvokeSystem : MonoBehaviour
    {
        public void DelaySecondsInvoke(Action action, float delay)
        {
            StartCoroutine(CustomInvoke(action, new WaitForSeconds(delay)));
        }

        private IEnumerator CustomInvoke(Action action, YieldInstruction instruction)
        {
            yield return instruction;
            action?.Invoke();
        }
    }
}

