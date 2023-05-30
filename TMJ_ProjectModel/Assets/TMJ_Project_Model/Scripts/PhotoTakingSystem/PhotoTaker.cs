
using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
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
        public string postURL;
        public string postField;

        public string currentPhotoLink;
        #endregion

        #region - Video Selector -
        [Header("Video Selection")]
        public VideoClip[] clips;
        public VideoClip currentClip;
        #endregion

        public string base64Test;

        public void StartPhotoTakeAction() => StartCoroutine(TakeScreenShot());
        IEnumerator TakeScreenShot()
        {
            videoPlayer.Play();

            for (int i = timeToTakePhoto; i > 0; i--)
            {
                countDownText.gameObject.GetComponent<RescaleEffect>().StartEffect();
                countDownText.gameObject.GetComponent<RescaleEffect>().ResetScale();

                countDownText.text = i.ToString();
                yield return new WaitForSeconds(1);
            }
            textCircle.gameObject.SetActive(false);
            countDownText.gameObject.SetActive(false);

            yield return new WaitForSeconds(1f);

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
            QR_CodeGenerator.Instance.GenerateFinalHash();
            yield return new WaitForSeconds(waitTimeToShow);

            //ConvertAndSend(); -> Decrapted 

            objectsToDesapear.SetActive(false);
            photoShower.SetActive(true);
            sceneLoaderAsset.canLoad = true;
        }
        private IEnumerator PhotoSend(Texture2D photo)
        {
            byte[] currentData = photo.EncodeToPNG();
            string base64Photo = Convert.ToBase64String(currentData);

            #region - Text File Creation -
            string path = "Assets/TMJ_Project_Model/Interaction Photos/photo" + DateTime.Now.ToString("yyyy_MM_dd-hh-mm-ss");
            string fullPhotoPath = path;

            using (StreamWriter sw = File.CreateText(path + ".txt"))
            {
                sw.Write(base64Photo);
                fullPhotoPath = path + ".txt";
            }
            AssetDatabase.Refresh();
            #endregion

            WWWForm form = new WWWForm();
            form.AddField("image", base64Photo);

            using (UnityWebRequest www = UnityWebRequest.Post(postURL, form))
            {
                www.SetRequestHeader("Content-Type", "application/json");
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success) Debug.Log(www.error);
                else Debug.Log("Sucefully uploaded!");

                currentPhotoLink = www.downloadHandler.text;
            }
            QR_CodeGenerator.Instance.finalLink = currentPhotoLink;
            QR_CodeGenerator.Instance.isActive = true;
        }
        public void SetCurrentClip(int videoIndex)
        {
            currentClip = clips[videoIndex];
            videoPlayer.clip = currentClip;
        }

        #region - Decrapted Send to DB Version -
        //private void ConvertAndSend()
        //{
        //    QR_CodeGenerator.Instance.isActive = true;

        //    byte[] bytes = photoTexture.EncodeToPNG();

        //    string base64string = Convert.ToBase64String(bytes);

        //    try
        //    {
        //        MySQL_Connection dillisBase = (MySQL_Connection)AssetDatabase.LoadAssetAtPath("Assets/TMJ_Project_Model/Scriptable Objects/Dilis Bases.asset", typeof(MySQL_Connection));

        //        MySqlConnection currentConnection = new MySqlConnection(dillisBase.GetConnectionString());

        //        MySqlCommand currentComand = new MySqlCommand("insert into imgLibrary(hash, base64, projectName) values('" + QR_CodeGenerator.Instance.generatedHash + "','" + base64string + "', 'TAMO JUNTO BARRA - BOTICARIO')", currentConnection);

        //        currentConnection.Open();

        //        currentComand.CommandType = System.Data.CommandType.Text;
        //        currentComand.ExecuteNonQuery();

        //        currentConnection.Close();
        //    }
        //    catch(Exception ex)
        //    {
        //        Debug.LogError(ex.ToString());
        //    }
        //    QR_CodeGenerator.Instance.isActive = true;
        //}

        #endregion
    }
}