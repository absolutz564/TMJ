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
        [SerializeField] private VideoPlayer videoPlayer2;
        public FlashEffect flashEffect;
        #endregion

        #region - Photo Show System
        [Header("Photo Shower")]
        public Sprite currentPhotoSprite;
        public GameObject photoShower;
        public Image photoReceiver;
        public GameObject objectsToDesapear;
        //public GameObject frameObject;
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
        public VideoClip currentClip2;
        public GameObject videoRenderer;
        public GameObject videoRenderer2;

        public GameObject ArrowObject;
        public bool IsPhotoSolo = true;
        #endregion

        [Serializable]
        public class ResponseData
        {
            public string link;
        }

        public WebCamStream cameraStream;

        public GameObject Border;
        public RectTransform ObjectRect;

        private void Start()
        {
            currentClip = clips[0].clip;
            videoPlayer.clip = currentClip;
            currentClip2 = clips[0].clip2;
            videoPlayer2.clip = currentClip2;
            cameraStream.Play();
        }
        public void StartPhotoTakeAction()
        {
            videoPlayer.Play();
            videoPlayer2.Play();
            StartCoroutine(TakeScreenShot());
            if (!IsPhotoSolo)
            {
                StartCoroutine(WaitToShow());

            }
        }

        IEnumerator WaitToShow()
        {
            Debug.Log("Exibindo renderes");
            yield return new WaitForSeconds(0.6f);
            videoRenderer.SetActive(true);
            videoRenderer2.SetActive(true);
        }
        public void SetTime(int currTime)
        {
            timeToTakePhoto = currTime;
        }
        IEnumerator TakeScreenShot()
        {
            for (int i = timeToTakePhoto; i > 0; i--)
            {
                countDownText.gameObject.GetComponent<RescaleEffect>().StartEffect();
                countDownText.gameObject.GetComponent<RescaleEffect>().ResetScale();

                countDownText.text = i.ToString();
                if(i == 1)
                {
                    ArrowObject.SetActive(false);
                }
                yield return new WaitForSeconds(1);
            }
            textCircle.gameObject.SetActive(false);
            countDownText.gameObject.SetActive(false);

            yield return new WaitForEndOfFrame();
            int width = Screen.width;
            int height = Screen.height;

            //frameObject.SetActive(true);
            Border.SetActive(true);
            ObjectRect.localPosition = new Vector3(5.3f, 140, 0.40f);
            ObjectRect.localScale = new Vector3(0.95f, 0.95f, 0.95f);


            yield return new WaitForEndOfFrame();

            int newWidth = width; 
            int newHeight = height;
            //int newWidth = width / 2;
            //int newHeight = height / 2;

            Texture2D screenShotTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, width, height);

            yield return new WaitForEndOfFrame();
            screenShotTexture.ReadPixels(rect, 0, 0);
            screenShotTexture.Apply();

            Texture2D resizedTexture = ScaleTexture(screenShotTexture, newWidth, newHeight);

            StartCoroutine(PhotoSend(resizedTexture));

            photoTexture = screenShotTexture;
            Border.SetActive(false);
            ObjectRect.localPosition = new Vector3(0, 0, 0);
            ObjectRect.localScale = new Vector3(1, 1, 1);

            //frameObject.SetActive(false);        
            flashEffect.CallFlashEffect();

            ConvertPhoto(photoTexture);
            StartCoroutine(ShowPhoto());            
        }

        private Texture2D ScaleTexture(Texture2D source, int newWidth, int newHeight)
        {
            Texture2D resizedTexture = new Texture2D(newWidth, newHeight);
            Color[] pixels = new Color[newWidth * newHeight];

            float xRatio = (float)source.width / newWidth;
            float yRatio = (float)source.height / newHeight;


            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    int sourceX = Mathf.FloorToInt(x * xRatio);
                    int sourceY = Mathf.FloorToInt(y * yRatio);

                    pixels[y * newWidth + x] = source.GetPixel(sourceX, sourceY);
                }
            }

            resizedTexture.SetPixels(pixels);
            resizedTexture.Apply();

            return resizedTexture;
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
            byte[] currentData = photo.EncodeToPNG();
            //currentPhotoLink = "https://tmj-boticario-api.herokuapp.com/api/users/download/image.png";

            WWWForm form = new WWWForm();
            form.AddBinaryData("upload", currentData, "image.png", "image/png");

            UnityWebRequest request = UnityWebRequest.Post("https://tmj-boticario-backend-555df95a9060.herokuapp.com/API/users/upload-file", form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                ResponseData responseData = JsonUtility.FromJson<ResponseData>(jsonResponse);

                Debug.Log("Imagem enviada com sucesso!");

                currentPhotoLink = responseData.link;
            }
            else
            {
                Debug.Log("Erro ao enviar a imagem. Status: " + request.responseCode);
            }

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
            IsPhotoSolo = videoIndex < 0;
            if(videoIndex >= 0)
            {
                currentClip = clips[videoIndex].clip;
                //videoRenderer.transform.localPosition = clips[videoIndex].videoPosition;
                videoPlayer.clip = currentClip;

                currentClip2 = clips[videoIndex].clip2;
                ////videoRenderer2.transform.localPosition = clips[videoIndex].videoPosition;
                videoPlayer2.clip = currentClip2;
            }
            else
            {
                videoRenderer.SetActive(false);
                videoRenderer2.SetActive(false);
                currentClip = null;
                videoPlayer.clip = null;

                currentClip2 = null;
                videoPlayer2.clip = null;
            }
        }
        #endregion
    }

    [Serializable]
    public struct VideoPos
    {
        public Vector3 videoPosition;
        public VideoClip clip;
        public VideoClip clip2;
    }
}