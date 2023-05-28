using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

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
        [SerializeField] private VideoPlayer videoPlayer;
        public FlashEffect flashEffect;
        #endregion

        #region - Photo Show System
        public Sprite currentPhotoSprite;
        public GameObject photoShower;
        public Image photoReceiver;
        public GameObject objectsToDesapear;
        public GameObject frameObject;
        public int waitTimeToShow = 4;
        #endregion

        #region - Countdown System -
        public int timeToTakePhoto = 3;
        public TextMeshProUGUI countDownText;
        public SceneLoader sceneLoaderAsset;
        #endregion

        public GameObject photoTaker;

        #region - Photo Take System Data -
        private string path;
        private Texture2D photoTexture;
        #endregion

        private void Start()
        {
            path = Application.dataPath + "/Interaction Photos/";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            Screen.autorotateToPortrait = true;
        }
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

            countDownText.gameObject.SetActive(false);

            yield return new WaitForSeconds(1f);

            yield return new WaitForEndOfFrame();
            int width = Screen.width;
            int height = Screen.height;

            frameObject.SetActive(true);

            //yield return new WaitForSeconds(0.1f);

            Texture2D screenShotTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, width, height);

            yield return new WaitForEndOfFrame();
            screenShotTexture.ReadPixels(rect, 0, 0);
            screenShotTexture.Apply();
            screenShotTexture.EncodeToPNG();

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
            ConvertAndSend();
            objectsToDesapear.SetActive(false);
            photoShower.SetActive(true);
            sceneLoaderAsset.canLoad = true;
        }
        private void ConvertAndSend()
        {
            QR_CodeGenerator.Instance.isActive = true;

            byte[] bytes = photoTexture.EncodeToPNG();

            string base64string = Convert.ToBase64String(bytes);

            try
            {
                MySQL_Connection dillisBase = (MySQL_Connection)AssetDatabase.LoadAssetAtPath("Assets/TMJ_Project_Model/Scriptable Objects/Dilis Bases.asset", typeof(MySQL_Connection));

                MySqlConnection currentConnection = new MySqlConnection(dillisBase.GetConnectionString());

                MySqlCommand currentComand = new MySqlCommand("insert into imgLibrary(hash, base64, projectName) values('" + QR_CodeGenerator.Instance.generatedHash + "','" + base64string + "', 'TAMO JUNTO BARRA - BOTICARIO')", currentConnection);

                currentConnection.Open();

                currentComand.CommandType = System.Data.CommandType.Text;
                currentComand.ExecuteNonQuery();

                currentConnection.Close();
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
            QR_CodeGenerator.Instance.isActive = true;
        }
    }
}