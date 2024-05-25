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
        private void EncondeTextToQR_Code()
        {
            string linkAndHash = finalLink;
            Debug.Log(linkAndHash);

            Color32[] convertPixelToTexture = Encode(linkAndHash, storedEncodedTexture.width, storedEncodedTexture.height);
            storedEncodedTexture.SetPixels32(convertPixelToTexture);
            storedEncodedTexture.Apply();

            QR_CodeImageReceiver.texture = storedEncodedTexture;
        }
        #endregion
    }
}