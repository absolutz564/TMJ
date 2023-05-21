using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NekraliusDevelopmentStudio
{
    public class GameManager : MonoBehaviour
    {
        //Code made by Victor Paulo Melo da Silva - Game Developer - GitHub - https://github.com/Necralius
        //GameManager - (Code Version)
        //Code State - (Needs Refactoring, Needs Coments, Needs Improvement)
        //This code represents (Code functionality or code meaning)

        #region - Singleton Pattern -
        public static GameManager Instance;
        void Awake()
        {
            if (Instance != null) DontDestroyOnLoad(Instance);
            else Instance = this;
        }
        #endregion      
    }
}