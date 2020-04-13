//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;
using UnityEngine.UI;

namespace JEM.Unity.UI.Control
{
    [RequireComponent(typeof(Text)), DisallowMultipleComponent]
    public class JEMInterfaceProgressiveInt : MonoBehaviour
    {
        // TODO: Move from update to coroutine

        [Header("Settings")]
        public int Value;
        public string EndText;
        public float Smooth = 5f;

        public Text Text { get; private set; }

        private void Awake()
        {
            Text = GetComponent<Text>();
        }

        private float _value;
        private void Update()
        {
            _value = Mathf.Lerp(_value, Value, Time.deltaTime * Smooth);
            Text.text = $"{_value:0}{EndText}";
        }
    }
}
