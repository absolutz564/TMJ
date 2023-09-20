using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NekraliusDevelopmentStudio
{
    public class FlashEffect : MonoBehaviour
    {
        //Code made by Victor Paulo Melo da Silva - Game Developer - GitHub - https://github.com/Necralius
        //FlashEffect - (0.1)
        //State: Functional - (Needs Refactoring, Needs Coments, Needs Improvement)
        //This code represents (Code functionality or code meaning)

        #region - Flash Effect Data
        private Image imgComponent => GetComponent<Image>();
        private bool callFlashEffect = false;
        #endregion

        [SerializeField] private float flashSpeed = 2;
        private float flashDuration = 0.05f;  // Duração de cada flash em segundos
        private float flashInterval = 0.1f; // Intervalo entre os flashes em segundos
        private bool isFlashing = false;

        private float flashTimer = 0f;
        private float intervalTimer = 0f;

        private void FlashEffectUpdate()
        {
            if (callFlashEffect)
            {
                Color currentColor = imgComponent.color;
                currentColor.a -= (Time.deltaTime / flashSpeed);
                imgComponent.color = currentColor;

                if (currentColor.a <= 0) { callFlashEffect = false; gameObject.SetActive(false); }
            }
        }
        public void CallFlashEffect()
        {
            isFlashing = false;
            this.gameObject.SetActive(true);
            Color currentColor = Color.white;
            imgComponent.color = currentColor;
            callFlashEffect = true;
        }

        public void StartFlashing()
        {
            isFlashing = true;
            flashTimer = 0f;
            intervalTimer = 0f;
            this.gameObject.SetActive(true);
        }

        public void StopFlashing()
        {
            isFlashing = false;
            Color currentColor = Color.white;
            imgComponent.color = currentColor;
            this.gameObject.SetActive(false);
        }

        void Update()
        {
            if (isFlashing)
            {
                flashTimer += Time.deltaTime;

                if (flashTimer < flashDuration)
                {
                    // Flashing (fading out)
                    Color currentColor = imgComponent.color;
                    currentColor.a -= Time.deltaTime / flashDuration;
                    imgComponent.color = currentColor;

                    if (currentColor.a <= 0)
                    {
                        // Flash terminado, resetar timer
                        flashTimer = 0f;
                    }
                }
                else if (intervalTimer < flashInterval)
                {
                    // Intervalo entre flashes
                    intervalTimer += Time.deltaTime;
                }
                else
                {
                    // Iniciar novo flash
                    Color currentColor = Color.white;
                    imgComponent.color = currentColor;
                    flashTimer = 0f;
                    intervalTimer = 0f;
                }
            }
            else
            {
                FlashEffectUpdate();
            }
        }
    }
}