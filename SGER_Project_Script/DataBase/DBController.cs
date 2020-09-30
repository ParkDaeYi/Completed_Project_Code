using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;

namespace DBManager
{
    public abstract class DBController
    {
        public string name = null;

        public abstract void Init(string DB_File, ArrayList arrSqls = null);
        public abstract DataTable SelectData(string sql, DataTable dtParam);
        public abstract void InsertData(string sql, DataTable dtParam);
        public abstract void UpdateData(string sql, DataTable dtParam);
        public abstract void DeleteData(string sql, DataTable dtParam);
        public abstract void DropDB(string DB_File);
        public abstract void PlusPlayed(string sql);
        public abstract void TotalStart(string sql);
        public abstract void DeleteRow(string sql);
        public abstract int TotalGetKey(string sql);
        public abstract void VoiceDirectory_Start(string sql);
        public abstract void VoiceFile_Start(string sql);
        public abstract void VoiceAudio_Start(string sql);
    }
}
