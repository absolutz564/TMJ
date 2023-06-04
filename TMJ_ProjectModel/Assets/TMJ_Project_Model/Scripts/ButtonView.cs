using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NekraliusDevelopmentStudio
{
    public class ButtonView : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        //Code made by Victor Paulo Melo da Silva - Game Developer - GitHub - https://github.com/Necralius
        //CompleteCodeName - (Code Version)
        //Code State - (Needs Refactoring, Needs Coments, Needs Improvement)
        //This code represents (Code functionality or code meaning)

        [SerializeField] private Image buttonSelection;


        public void OnSelect(BaseEventData eventData)
        {
            buttonSelection.color = new Color(255, 255, 255, 255);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            buttonSelection.color = new Color(255, 255, 255, 0);
        }
    }
}