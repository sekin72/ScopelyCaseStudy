using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CerberusFramework.Core.UI.Components
{
    public class CFText : MonoBehaviour
    {
        private string _textValue = "";
        public List<TMP_Text> TextFields = new List<TMP_Text>();

        public string Text
        {
            get => _textValue;
            set => UpdateInnerText(value);
        }

        private void UpdateInnerText(string value)
        {
            _textValue = value;
            foreach (var textField in TextFields)
            {
                textField.text = value;
            }
        }
    }
}