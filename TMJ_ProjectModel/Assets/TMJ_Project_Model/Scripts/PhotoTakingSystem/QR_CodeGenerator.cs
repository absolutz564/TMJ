using MySql.Data.MySqlClient;
using System;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

namespace NekraliusDevelopmentStudio
{
    public class QR_CodeGenerator : MonoBehaviour
    {
        //Code made by Victor Paulo Melo da Silva - Game Developer - GitHub - https://github.com/Necralius
        //QR_CodeGenerator - (0.1)
        //Code State - (Needs Refactoring, Needs Coments, Needs Improvement)
        //This code represents (Code functionality or code meaning)

        #region - Singleton Pattern -
        public static QR_CodeGenerator Instance;
        private void Awake() => Instance = this;
        #endregion

        [SerializeField] private RawImage QR_CodeImageReceiver;
        public Texture2D storedEncodedTexture;

        private RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
        public string generatedHash;

        public string finalLink;
        public bool isActive = false;

        #region - BuiltIn Methods -
        private void Start()
        {
            storedEncodedTexture = new Texture2D(256, 256);
        }
        private void Update()
        {
            if (isActive)
            {
                EncondeTextToQR_Code();
                ReadLink();
            }
            else return;
        }
        #endregion

        #region - QrCode Generation -
        private Color32[] Encode(string textForEncode, int width, int height) //This method return an array of pixels that 
        {
            BarcodeWriter writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width,
                }
            };
            return writer.Write(textForEncode);
        }
        private void EncondeTextToQR_Code() //This method gets an string and enconde it on a 2D Texture  using the ZXing plugin.
        {
            string linkAndHash = finalLink + generatedHash;

            Color32[] convertPixelToTexture = Encode(linkAndHash, storedEncodedTexture.width, storedEncodedTexture.height);
            storedEncodedTexture.SetPixels32(convertPixelToTexture);
            storedEncodedTexture.Apply();

            QR_CodeImageReceiver.texture = storedEncodedTexture;
        }
        #endregion

        #region - Identification Hash Generation -
        public void GenerateFinalHash() //This method uses cryptography to generates an random image hash to identify the current image on an extern API.
        {
            var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var Charsarr = new char[8];
            var random = new System.Random();

            for (int i = 0; i < Charsarr.Length; i++) Charsarr[i] = characters[random.Next(characters.Length)];

            string key = new String(Charsarr);

            byte[] salt = new byte[16];
            rngCsp.GetBytes(salt);

            var pbkdf2 = new Rfc2898DeriveBytes(key, salt, 1000);

            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];

            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            generatedHash = ValidString(Convert.ToBase64String(hashBytes));

            //Debug.Log("Final Link -> " + finalLink + generatedHash); -> Debug option visualization
        }

        string ValidString(string stringItem) //This method removes every "+" symbol from a string for prevent API utilization errors.
        {
            string newString = "";

            for (int i = 0; i < stringItem.Length; i++) if (!(stringItem.Substring(i, 1) == "+")) newString += stringItem.Substring(i, 1);

            return newString;
        }

        #endregion

        #region - Application Link Get -
        private void ReadLink() //This method uses an try catch block to get an link from and MySQL Database.
        {
            try
            {
                MySQL_Connection dillisBase = (MySQL_Connection)AssetDatabase.LoadAssetAtPath("Assets/TMJ_Project_Model/Scriptable Objects/Dilis Bases.asset", typeof(MySQL_Connection));

                MySqlConnection currentConnection = new MySqlConnection(dillisBase.GetConnectionString());
                MySqlCommand getRelatedLink = new MySqlCommand("select linkAtrelado from links where ProjectOrigem = 'TAMO JUNTO - CAMPINENSE'", currentConnection);

                currentConnection.Open();

                getRelatedLink.CommandType = System.Data.CommandType.Text;
                MySqlDataReader consultData = getRelatedLink.ExecuteReader();

                if (consultData.Read()) finalLink = consultData[0].ToString();

                currentConnection.Close();        
            }
            catch(Exception ex)
            {
                Debug.Log(ex);
            }
        }
        #endregion
    }
}