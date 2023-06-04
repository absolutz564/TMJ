using Nexweron.WebCamPlayer;
using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using Debug = UnityEngine.Debug;

namespace NekraliusDevelopmentStudio
{
    public class PhotoTaker : MonoBehaviour
    {
        //Code made by Victor Paulo Melo da Silva - Game Developer - GitHub - https://github.com/Necralius
        //PhotoTaker - (0.1)
        //State: Functional - (Needs Refactoring, Needs Coments, Needs Improvement)
        //This code represents (Code functionality or code meaning)

        #region - Singleton Pattern -
        public static PhotoTaker Instance;
        private void Awake() => Instance = this;
        #endregion

        #region - Main Dependecies -
        [Header("System Dependencies")]
        [SerializeField] private VideoPlayer videoPlayer;
        public FlashEffect flashEffect;
        #endregion

        #region - Photo Show System
        [Header("Photo Shower")]
        public Sprite currentPhotoSprite;
        public GameObject photoShower;
        public Image photoReceiver;
        public GameObject objectsToDesapear;
        public GameObject frameObject;
        public int waitTimeToShow = 4;
        #endregion

        #region - Countdown System -
        [Header("Countdown System")]
        public int timeToTakePhoto = 3;
        public TextMeshProUGUI countDownText;
        public SceneLoader sceneLoaderAsset;
        #endregion

        public GameObject photoTaker;
        public GameObject textCircle;

        #region - Photo Data -
        private Texture2D photoTexture;
        #endregion

        #region - DB Post -
        [Header("Photo Send DB")]
        public string currentPhotoLink;
        #endregion

        #region - Video Selector -
        [Header("Video Selection")]
        public VideoPos[] clips;
        public VideoClip currentClip;
        public GameObject videoRenderer;
        #endregion

        public WebCamStream cameraStream;

        private void Start()
        {
            currentClip = clips[0].clip;
            videoPlayer.clip = currentClip;
            cameraStream.Play();
        }
        public void StartPhotoTakeAction()
        {
            videoPlayer.Play();
            StartCoroutine(TakeScreenShot());
        }
        IEnumerator TakeScreenShot()
        {
            for (int i = timeToTakePhoto; i > 0; i--)
            {
                countDownText.gameObject.GetComponent<RescaleEffect>().StartEffect();
                countDownText.gameObject.GetComponent<RescaleEffect>().ResetScale();

                countDownText.text = i.ToString();
                yield return new WaitForSeconds(1);
            }
            textCircle.gameObject.SetActive(false);
            countDownText.gameObject.SetActive(false);

            yield return new WaitForEndOfFrame();
            int width = Screen.width;
            int height = Screen.height;

            frameObject.SetActive(true);

            Texture2D screenShotTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, width, height);

            yield return new WaitForEndOfFrame();
            screenShotTexture.ReadPixels(rect, 0, 0);
            screenShotTexture.Apply();

            StartCoroutine(PhotoSend(screenShotTexture));

            photoTexture = screenShotTexture;
            frameObject.SetActive(false);        
            flashEffect.CallFlashEffect();

            ConvertPhoto(photoTexture);
            StartCoroutine(ShowPhoto());            
        }
        void ConvertPhoto(Texture2D textureData)
        {
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            Rect rect = new Rect(0,0, textureData.width, textureData.height);
            currentPhotoSprite = Sprite.Create(textureData, rect, pivot);
            currentPhotoSprite.name = DateTime.Now.Ticks.ToString();

            photoReceiver.sprite = currentPhotoSprite;
        }
        IEnumerator ShowPhoto()
        {
            yield return new WaitForSeconds(waitTimeToShow);      

            objectsToDesapear.SetActive(false);
            photoShower.SetActive(true);
            sceneLoaderAsset.canLoad = true;
            cameraStream.Stop();
        }

        #region - Photo Sending to DB -
        private IEnumerator PhotoSend(Texture2D photo)
        {
            Debug.Log("Sending Photo!");
            byte[] currentData = photo.EncodeToPNG();
            string base64Photo = Convert.ToBase64String(currentData);

            var dataToPost = new PostData() { image = base64Photo };
            var postRequest = CreateRequest("http://145.14.134.34:3022/api/users/upload", RequestType.POST, dataToPost);
            yield return postRequest.SendWebRequest();

            if (postRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(postRequest.error);
                Debug.Log("Not sended!");
            }
            else Debug.Log("Sucefully uploaded!");

            currentPhotoLink = ValidateString(postRequest.downloadHandler.text);
            Debug.Log("validating string!");

            QR_CodeGenerator.Instance.finalLink = currentPhotoLink;
            QR_CodeGenerator.Instance.isActive = true;
        }
        #endregion
        private string ValidateString(string textToValidade)
        {
            string link = "";
            for (int i = 0; i < textToValidade.Length; i++)
            {
                if (i > 8)
                {
                    if (i >= textToValidade.Length - 2) continue;
                    link += textToValidade.Substring(i, 1);
                }
            }
            return link;
        }

        #region - Unity Web Request Data Model - 
        private UnityWebRequest CreateRequest(string path, RequestType type = RequestType.GET, object data = null)
        {
            UnityWebRequest request = new UnityWebRequest(path, type.ToString());

            if (data != null)
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }

            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            return request;
        }

        [Serializable]
        public class PostData
        {
            public string image;
        }
        public enum RequestType
        {
            GET = 0,
            POST = 1,
            PUT = 2
        }
        #endregion

        #region - Video Clip Selection -
        public void SetCurrentClip(int videoIndex)
        {
            currentClip = clips[videoIndex].clip;
            videoRenderer.transform.localPosition = clips[videoIndex].videoPosition;
            videoPlayer.clip = currentClip;
        }
        #endregion
    }

    [Serializable]
    public struct VideoPos
    {
        public Vector3 videoPosition;
        public VideoClip clip;
    }
}