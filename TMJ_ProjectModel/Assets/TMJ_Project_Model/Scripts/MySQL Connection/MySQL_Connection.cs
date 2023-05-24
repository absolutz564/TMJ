using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql;
using MySql.Data.MySqlClient;

namespace NekraliusDevelopmentStudio
{
    [CreateAssetMenu(fileName = "New Database Asset", menuName = "TMJ - Project/Database/New Database Asset")]
    public class MySQL_Connection : ScriptableObject
    {
        //Code made by Victor Paulo Melo da Silva - Game Developer - GitHub - https://github.com/Necralius
        //MySQL_Connection - (0.1)
        //Code State - (Needs Refactoring, Needs Coments, Needs Improvement)
        //This code represents (Code functionality or code meaning)

        #region - Connection Data -
        [Header("Connection Data")]
        public string connectionName = "New Connection";
        public string Server = "";
        public string Database = "";
        public string UId = "";
        public string Password = "";
        public MySqlConnection ObjectConnection;
        #endregion

        #region - Connection Managment -
        public string GetConnectionString() => string.Format("Server={0};Database={1};Uid={2};Pwd={3};", Server, Database, UId, Password);
        public MySqlConnection GetConnection() { ObjectConnection = new MySqlConnection(GetConnectionString()); return ObjectConnection; }
        #endregion
    }
}