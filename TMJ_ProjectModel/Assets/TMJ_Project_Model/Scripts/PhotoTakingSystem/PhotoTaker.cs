using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using TMPro;
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
                if (i == 1) flashEffect.CallFlashEffect();

                countDownText.gameObject.GetComponent<RescaleEffect>().StartEffect();
                countDownText.gameObject.GetComponent<RescaleEffect>().ResetScale();

                countDownText.text = i.ToString();
                yield return new WaitForSeconds(1);
            }
            countDownText.gameObject.SetActive(false);

            yield return new WaitForSeconds(1);
            yield return new WaitForEndOfFrame();
            int width = Screen.width;
            int height = Screen.height;

            Texture2D screenShotTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, width, height);
            screenShotTexture.ReadPixels(rect, 0, 0);
            screenShotTexture.Apply();
            screenShotTexture.EncodeToPNG();

            photoTexture = screenShotTexture;

            yield return new WaitForSeconds(1);            

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
            ConvertAndSend();
            objectsToDesapear.SetActive(false);
            photoShower.SetActive(true);
            sceneLoaderAsset.canLoad = true;
        }
        private void ConvertAndSend()
        {
            byte[] bytes = photoTexture.EncodeToPNG();

            string base64string = Convert.ToBase64String(bytes);

            try
            {
                MySqlConnection currentConnection = new MySqlConnection(ConnectionManager.Instance.FindMyConnection("Dilis Main Database").GetConnectionString());

                MySqlCommand currentComand = new MySqlCommand("insert into imgLibrary(hash, base64, projectName) values('" + QR_CodeGenerator.Instance.finalHash + "','" + base64string + "', 'TAMO JUNTO BARRA - NATAL')", currentConnection);

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