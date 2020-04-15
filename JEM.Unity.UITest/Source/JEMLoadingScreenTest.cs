//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System.Collections;
using JEM.Unity.UI;
using UnityEngine;

namespace JEM.Unity.UITest
{
    public sealed class JEMLoadingScreenTest : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1f);
            
            JEMLoadingScreen.Instance.OnUserContinue += () =>
            {
                Debug.Log("JEMLoadingScenario received user input.");
            };
            
            JEMLoadingScreen.Instance.OnLoadingStateReported += state =>
            {
                if (!state)
                    Debug.Log("Loading UI is now disabled so we can initialize something new?"); // tho you should initialize your initialize during actual loading.
                else
                {
                    Debug.Log("Loading enabled.");
                }
            };
            
            // Activate loading UI.
            JEMLoadingScreen.Instance.ActiveLoading();
            
            // Wait 3 seconds just as we loading something.
            yield return new WaitForSeconds(3f);
            
            // Report loading end.
            JEMLoadingScreen.Instance.TryDisableLoading();
        }
    }
}