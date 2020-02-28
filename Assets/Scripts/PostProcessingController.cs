using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace PhotoMode
{
    [RequireComponent(typeof(PostProcessVolume))]
    public class PostProcessingController : MonoBehaviour
    {
        #region Variables & references
        static readonly float TEMPERATURE = 0f;
        static readonly float SATURATION = 0f;
        static readonly float BLOOM = 8f;
        static readonly float FOCUS = 5f;
        static readonly float VIGNETTE = 0.2f;

        [Header("PostProcessing")]
        public PostProcessProfile gamePostProcess, photoPostProcess;

        private PostProcessVolume _postProcessVolume;
        private ColorGrading _colorGradingPhoto;
        private Bloom _bloomPhoto;
        private DepthOfField _depthOfFieldPhoto;
        private Vignette _vignettePhoto;
        #endregion

        private void Awake()
        {
            _postProcessVolume = GetComponent<PostProcessVolume>();
        }

        private void Start()
        {
            photoPostProcess.TryGetSettings(out _colorGradingPhoto);
            photoPostProcess.TryGetSettings(out _bloomPhoto);
            photoPostProcess.TryGetSettings(out _depthOfFieldPhoto);
            photoPostProcess.TryGetSettings(out _vignettePhoto);
        }

        public void PhotoMode(bool state)
        {
            _postProcessVolume.profile = (state) ? photoPostProcess : gamePostProcess;
        }

        public void Restart(ref PostProcessingOption[] options)
        {
            _colorGradingPhoto.temperature.value = TEMPERATURE;
            _colorGradingPhoto.saturation.value = SATURATION;
            _bloomPhoto.intensity.value = BLOOM;
            _depthOfFieldPhoto.focusDistance.value = FOCUS;
            _vignettePhoto.intensity.value = VIGNETTE;

            foreach (PostProcessingOption option in options)
            {
                switch (option.optionPhotoMode)
                {
                    case Option.Temperature:
                        option.value.text = "" + TEMPERATURE;
                        break;
                    case Option.Saturation:
                        option.value.text = "" + SATURATION;
                        break;
                    case Option.Bloom:
                        option.value.text = "" + BLOOM;
                        break;
                    case Option.Focus:
                        option.value.text = "" + FOCUS;
                        break;
                    case Option.Vignette:
                        option.value.text = "" + VIGNETTE;
                        break;
                    default:return;
                }
            }
        }

        public void Temperature(float value) { _colorGradingPhoto.temperature.value += value; }
        public void Saturation(float value) { _colorGradingPhoto.saturation.value += value; }
        public void Bloom(float value) { _bloomPhoto.intensity.value += value; }
        public void Focus(float value) { _depthOfFieldPhoto.focusDistance.value += value; }
        public void Vignette(float value) { _vignettePhoto.intensity.value += value; }

        public float GetTemperature() { return _colorGradingPhoto.temperature.value; }
        public float GetSaturation() { return _colorGradingPhoto.saturation.value; }
        public float GetBloom() { return _bloomPhoto.intensity.value; }
        public float GetFocus() { return _depthOfFieldPhoto.focusDistance.value; }
        public float GetVignette() { return _vignettePhoto.intensity.value; }
    }
}