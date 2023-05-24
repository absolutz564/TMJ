using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NekraliusDevelopmentStudio
{
    public class RescaleEffect : MonoBehaviour
    {
        //Code made by Victor Paulo Melo da Silva - Game Developer - GitHub - https://github.com/Necralius
        //CompleteCodeName - (Code Version)
        //Code State - (Needs Refactoring, Needs Coments, Needs Improvement)
        //This code represents (Code functionality or code meaning)

        [SerializeField] bool applyEffect;
        [SerializeField] private float speed = 4f;

        [SerializeField] float startSize;
        [SerializeField] float finalSize;
        [SerializeField] float currentSize;

        private void Start()
        {
            //startSize = transform.localScale.magnitude;
        }

        private void Update() => SetScale();
        private void SetScale()
        {
            if (applyEffect)
            {
                currentSize = Mathf.Lerp(currentSize, finalSize, speed * Time.deltaTime);

                transform.localScale = new Vector3(currentSize, currentSize, currentSize);

                if (currentSize >= finalSize - 1) applyEffect = false;
            }
        }
        public void ResetScale() { currentSize = startSize; transform.localScale = new Vector3 (startSize, startSize, startSize); }
        public void StartEffect() => applyEffect = true;
    }
}