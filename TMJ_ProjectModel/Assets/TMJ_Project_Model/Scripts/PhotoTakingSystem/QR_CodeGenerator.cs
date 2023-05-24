using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Security.Cryptography;
using ZXing;
using ZXing.QrCode;
using Unity.VisualScripting;
using System;
using MySql.Data.MySqlClient;

namespace NekraliusDevelopmentStudio
{
    public class QR_CodeGenerator : MonoBehaviour
    {
        //Code made by Victor Paulo Melo da Silva - Game Developer - GitHub - https://github.com/Necralius
        //CompleteCodeName - (Code Version)
        //Code State - (Needs Refactoring, Needs Coments, Needs Improvement)
        //This code represents (Code functionality or code meaning)

        #region - Singleton Pattern -
        public static QR_CodeGenerator Instance;
        private void Awake() => Instance = this;
        #endregion

        private RawImage QR_CodeImageReceiver;
        private Texture2D storedEncodedTexture;

        private RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
        public string key;
        public string finalHash;

        public string currentLink;
        public bool isActive = true;

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
        }

        private void EncondeTextToQR_Code()
        {
           // string currentLink = "https://teste.com/?id=" + finalHash;
            string newLink = currentLink + finalHash;
            Debug.Log(currentLink);

            Color32[] convertPixelToTexture = Encode(currentLink, storedEncodedTexture.width, storedEncodedTexture.height);
            storedEncodedTexture.SetPixels32(convertPixelToTexture);
            storedEncodedTexture.Apply();

            QR_CodeImageReceiver.texture = storedEncodedTexture;
        }
        private Color32[] Encode(string textForEncode, int width, int height)
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

        public void GenerateQrCode()
        {
            CriarKey();
            CriarHash();
        }
        private void CriarKey()
        {
            var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var Charsarr = new char[8];
            var random = new System.Random();

            for (int i = 0; i < Charsarr.Length; i++) Charsarr[i] = characters[random.Next(characters.Length)];

            key = new String(Charsarr);
        }
        private void CriarHash()
        {
            byte[] salt = new byte[16];
            rngCsp.GetBytes(salt);

            var pbkdf2 = new Rfc2898DeriveBytes(key, salt, 1000);

            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];

            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            finalHash = ValidString(Convert.ToBase64String(hashBytes));

            Debug.Log("Hash corrigido " + finalHash);
        }
        string ValidString(string stringItem)
        {
            string newString = "";

            for (int i = 0; i < stringItem.Length; i++) if (!(stringItem.Substring(i, 1) == "+")) newString += stringItem.Substring(i, 1);

            return newString;
        }

        private void ReadLink()
        {
            MySqlConnection currentConnection = new MySqlConnection("Server=145.14.134.34;Database=DilisTmj;Uid=pedroDev;Pwd=H&QmFT7ax$2q;");
            MySqlCommand getRelatedLink = new MySqlCommand("select linkAtrelado from links where ProjectOrigem = 'TAMO JUNTO - CAMPINENSE'", currentConnection);

            currentConnection.Open();

            getRelatedLink.CommandType = System.Data.CommandType.Text;
            MySqlDataReader consultData = getRelatedLink.ExecuteReader();

            if (consultData.Read()) currentLink = consultData[0].ToString();

            Debug.Log("Link Atrelado " + currentLink);
            currentConnection.Close();
        }
    }
}