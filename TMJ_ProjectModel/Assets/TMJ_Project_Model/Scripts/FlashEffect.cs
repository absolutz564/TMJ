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

        private void Update() => FlashEffectUpdate();
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
            this.gameObject.SetActive(true);
            Color currentColor = Color.white;
            imgComponent.color = currentColor;
            callFlashEffect = true;
        }
    }
}