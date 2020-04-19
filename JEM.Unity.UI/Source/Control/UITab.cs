//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections;
using JEM.Unity.UI.Animation;
using UnityEngine;
using UnityEngine.UI;

namespace JEM.Unity.UI.Control
{
    /// <inheritdoc />
    /// <summary>
    ///     A simple script that binds selected array of buttons with selected panels
    ///      so the only one panel could be active at the time.
    /// </summary>
    [AddComponentMenu("JEM/UI/Control/Tab Controller")]
    [DisallowMultipleComponent]
    internal class UITab : MonoBehaviour
    {
        [Serializable]
        public class Item
        {
            public UIFadeAnimation Panel;
            public Button Activator;
        }

        /// <summary>
        ///     Our list of items to bind.
        /// </summary>
        [Header("Settings")]
        [SerializeField] internal Item[] Items = new Item[0];

        /// <summary>
        ///     Amount of time to wait between panel activation.
        /// </summary>
        [SerializeField] internal float ActivationWait = 0.5f;

        /// <summary>
        ///     If true, the first item in list will be activated at Start.
        /// </summary>
        [SerializeField] internal bool ActivateOnStart = true;

        /// <summary>
        ///     When true, button of selected panel will be disabled.
        /// </summary>
        [SerializeField] internal bool DisableSelected;

        /// <summary>
        ///     Panels will be restarted on component disable.
        /// </summary>
        [SerializeField] internal bool ResetOnDisable;

        private Coroutine _coroutine;
        private void Start()
        {
            if (Items.Length == 0)
                return;

            foreach (var i in Items)
            {
                i.Activator.onClick.AddListener(() =>
                {
                    if (_coroutine != null)
                        StopCoroutine(_coroutine);
                    _coroutine = StartCoroutine(Activate(i));
                });
            }

            if (ActivateOnStart)
                Items[0].Activator.onClick.Invoke();
        }

        private void OnDisable()
        {
            if (ResetOnDisable)
            {
                DisableAll();
            }
        }

        public void DisableAll()
        {
            for (var index = 0; index < Items.Length; index++)
            {
                var i = Items[index];
                i.Panel.SetActive(false);
                if (DisableSelected)
                    i.Activator.interactable = true;
            }
        }

        private IEnumerator Activate(Item i)
        {
            foreach (var i2 in Items)
            {
                if (i == i2) continue;
                i2.Panel.SetActive(false);
                if (DisableSelected)
                    i2.Activator.interactable = true;
            }

            yield return new WaitForSeconds(ActivationWait);
            if (i.Panel.IsActive)
            {
                i.Panel.SetActive(false);
                if (DisableSelected)
                    i.Activator.interactable = true;
            }
            else
            {
                i.Panel.SetActive(true);
                if (DisableSelected)
                    i.Activator.interactable = false;
            }

            _coroutine = null;
        }
    }
}
