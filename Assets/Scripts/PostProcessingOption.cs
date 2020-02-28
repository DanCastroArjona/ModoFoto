using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;

namespace PhotoMode
{
    public enum Option { Temperature, Saturation, Bloom, Focus, Vignette, Filter}

    public class PostProcessingOption : MonoBehaviour
    {
        #region Variables
        public Option optionPhotoMode;

        public float newValue;
        [ReadOnly]
        public TextMeshProUGUI value;
        #endregion

        private void Awake()
        {
            TextMeshProUGUI[] texts = this.gameObject.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI child in texts)
            {
                if (child.gameObject == this)
                    continue;
                value = child;
            }
        }
    }
}