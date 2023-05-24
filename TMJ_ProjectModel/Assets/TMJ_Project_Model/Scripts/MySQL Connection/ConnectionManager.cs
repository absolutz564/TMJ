using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NekraliusDevelopmentStudio
{
    public class ConnectionManager : MonoBehaviour
    {
        //Code made by Victor Paulo Melo da Silva - Game Developer - GitHub - https://github.com/Necralius
        //ConnectionManager - (0.1)
        //State - (Needs Refactoring, Needs Coments, Needs Improvement)
        //This code represents (Code functionality or code meaning)

        #region - Singleton Pattern -
        public static ConnectionManager Instance;
        void Awake() => Instance = this;
        #endregion

        public List<MySQL_Connection> connections;

        public MySQL_Connection FindMyConnection(string connectionName) => connections.First(e => e.connectionName == connectionName);
    }
}