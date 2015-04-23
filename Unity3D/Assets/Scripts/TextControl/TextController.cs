using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.TextControl
{
    class TextController : MonoBehaviour
    {
        private Text _text;
        private RectTransform _rectTransform;

        void Start()
        {
            _text = GetComponent<Text>();
            _rectTransform = GetComponent<RectTransform>();
        }

        public void AddMessage(string message)
        {
            _text.text += Environment.NewLine + message;
            _rectTransform.sizeDelta += new Vector2(0, 21);
        }
    }
}
