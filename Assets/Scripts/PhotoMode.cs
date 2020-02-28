using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

using UnityEngine.UI;

namespace PhotoMode
{
    [RequireComponent(typeof(PostProcessingController))]
    public class PhotoMode : MonoBehaviour
    {
        #region Variables & references
        static readonly float MAXZOOM = 15f;
        static readonly float MINZOOM = 50f;
        static readonly Vector3 PHOTOCAMERARELATIVEPOSITION = new Vector3(0f, 10f, -10f);
        static readonly Color NORMALCOLOR = Color.white;
        static readonly Color READCOLOR = Color.red;

        [Header("CameraManager")]
        public CinemachineBrain cinemachineBrain;
        public CinemachineBlendDefinition cinemachineBlendDefinition;
        public GameObject gameCameraObject, photoCameraObject;
        private bool _photoModeState = false, _photoTransitionActive= false;

        [Header("Options/UI")]
        public PostProcessingOption[] optionsUI;
        public GameObject photoPanel;
        public GameObject photoPopUpBlock;
        public RawImage imagePhotoBlock;
        private int _selector;

        private PostProcessingController _postProcessingController;

        #endregion


        //Puede ser utilizado por el personaje si queremos que realice alguna pose
        public delegate void ModeAnimation(bool state);
        public event ModeAnimation AnimationPose;
        

        private void Awake()
        {
            _postProcessingController = GetComponent<PostProcessingController>();
        }

        private void Update()//Controles para manejar el modo foto
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                _photoModeState = !_photoModeState;
                PhotoModeAction();
            }

            if (_photoModeState  && !_photoTransitionActive)
            {
                if (Input.GetKeyDown(KeyCode.Z))
                    Zoom(true);
                else if (Input.GetKeyDown(KeyCode.X))
                    Zoom(false);

                if (Input.GetKeyDown(KeyCode.W))
                    NewSelector(false);
                else if (Input.GetKeyDown(KeyCode.S))
                    NewSelector(true);
                else if (Input.GetKeyDown(KeyCode.D))
                    NewValue(true);
                else if (Input.GetKeyDown(KeyCode.A))
                    NewValue(false);
                else if (Input.GetKeyDown(KeyCode.R))
                    _postProcessingController.Restart(ref optionsUI);
                else if (Input.GetKeyDown(KeyCode.Space))
                    NewPhoto();
                
            }
        }

        private void PhotoModeAction()//Activar/Desactivar modo foto
        {
            cinemachineBrain.m_DefaultBlend = cinemachineBlendDefinition;

            if (_photoModeState)
            {
                gameCameraObject.SetActive(false);
                photoCameraObject.SetActive(true);
                photoCameraObject.transform.position = gameCameraObject.GetComponent<CinemachineVirtualCamera>().LookAt.position
                    + PHOTOCAMERARELATIVEPOSITION;
                photoPanel.SetActive(true);
                _postProcessingController.Restart(ref optionsUI);
                RestartSelector();
            }
            else
            {
                photoCameraObject.SetActive(false);
                gameCameraObject.SetActive(true);

                photoPanel.SetActive(false);
                PhotoTransition(false);

                AnimationPose?.Invoke(false);
            }

            _postProcessingController.PhotoMode(_photoModeState);
        }

        private void NewPhoto()
        {
            StartCoroutine(CapturePhoto());
        }

        private IEnumerator CapturePhoto()//Corrutina que se encarga de realizar la fotografía, codificando en un archivo png la textura creada a partir del render de la camara principal
        {
            AnimationPose?.Invoke(true);
            yield return new WaitForEndOfFrame();
            Texture2D screenCap = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            Camera.main.Render();
            RenderTexture.active = Camera.main.targetTexture;
            screenCap.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            byte[] bytes = screenCap.EncodeToPNG();
            Destroy(screenCap);

            string date = System.DateTime.UtcNow.Day + "_" + System.DateTime.UtcNow.Hour + "_"
                + System.DateTime.UtcNow.Minute + "_" + System.DateTime.UtcNow.Second;
            string path = Application.persistentDataPath + "/photoImages/";
            System.IO.Directory.CreateDirectory(path);
            System.IO.File.WriteAllBytes(path + date + ".png", bytes);

            Texture2D currentImage = new Texture2D((int)imagePhotoBlock.rectTransform.rect.width, (int)imagePhotoBlock.rectTransform.rect.height);
            currentImage.LoadImage(bytes);
            imagePhotoBlock.texture = currentImage;

            PhotoTransition(true);
        }

        private void PhotoTransition(bool state)//Utilización de Itween para crear una animación fluida y de forma sencilla
        {
            _photoTransitionActive = state;

            iTween.ScaleTo(photoPopUpBlock, iTween.Hash(
                "scale", (state)? Vector3.one : Vector3.zero,
                "time", cinemachineBlendDefinition.m_Time * 2,
                "easytype", cinemachineBlendDefinition.m_Style
                ));
        }

        private void RestartSelector()
        {
            optionsUI[_selector].GetComponent<TextMeshProUGUI>().color = NORMALCOLOR;
            _selector = 0;
            optionsUI[_selector].GetComponent<TextMeshProUGUI>().color = READCOLOR;
        }

        private void NewSelector(bool state)
        {
            optionsUI[_selector].GetComponent<TextMeshProUGUI>().color = NORMALCOLOR;
            _selector = (state) ? _selector + 1 : _selector - 1;

            if (_selector >= optionsUI.Length) _selector = 0;
            else if (_selector < 0) _selector = optionsUI.Length - 1;

            optionsUI[_selector].GetComponent<TextMeshProUGUI>().color = READCOLOR;
        }

        private void NewValue(bool state)
        {
            PostProcessingOption option = optionsUI[_selector];
            float newValue = (state) ? option.newValue : -option.newValue;

            switch (option.optionPhotoMode)
            {
                case Option.Temperature:
                    _postProcessingController.Temperature(newValue);
                    option.value.text = "" + _postProcessingController.GetTemperature();
                    break;
                case Option.Saturation:
                    _postProcessingController.Saturation(newValue);
                    option.value.text = "" + _postProcessingController.GetSaturation();
                    break;
                case Option.Bloom:
                    _postProcessingController.Bloom(newValue);
                    option.value.text = "" + _postProcessingController.GetBloom();
                    break;
                case Option.Focus:
                    _postProcessingController.Focus(newValue);
                    option.value.text = "" + _postProcessingController.GetFocus();
                    break;
                case Option.Vignette:
                    _postProcessingController.Vignette(newValue);
                    option.value.text = "" + _postProcessingController.GetVignette();
                    break;
                default: return;
            }
        }

        private void Zoom(bool state)
        {
            CinemachineVirtualCamera cameraVirtual = photoCameraObject.GetComponent<CinemachineVirtualCamera>();

            cameraVirtual.m_Lens.FieldOfView = (state) ? cameraVirtual.m_Lens.FieldOfView - 5f : cameraVirtual.m_Lens.FieldOfView + 5f;

            if (cameraVirtual.m_Lens.FieldOfView < MAXZOOM) cameraVirtual.m_Lens.FieldOfView = MAXZOOM;
            else if (cameraVirtual.m_Lens.FieldOfView > MINZOOM) cameraVirtual.m_Lens.FieldOfView = MINZOOM;
        }
    }
}