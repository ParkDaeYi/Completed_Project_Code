using UnityEngine;
using Mono.Data.SqliteClient;
using DBManager;
using System.Data;
using System.IO;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using MySql.Data;

public class GameDBController : DBController
{
    public GameDBController()
    {
    }

    private static string db_name = "";

    //VRDBController에서 DB 경로를 받아옴
    public static string DB_Name
    {
        set
        {
            db_name = value;
        }
        get
        {
            return db_name;
        }

    }

    public static SqliteConnection connection;

    //Connection 은 아직 외부에서 설정해주는 경우는 없음
    public static SqliteConnection Connection
    {
        set
        {
            connection = value;
        }
        get
        {
            return connection;
        }
    }

    private static SqliteConnection Open()
    {
        string localDBPath = "URI=file:" + db_name;
        //string localDBPath = "URI=file:" + db_name.Substring(0, 32);
        //string localDBPath = string.Format("{0}/{1}", Static.STATIC.dir_path + "/DB/", db_name);

        //Sqlite 와 연결
        connection = new SqliteConnection(localDBPath);
        //Open
        connection.Open();

        return connection;
    }

    private static void Close()
    {
        //Close
        connection.Close();
    }

    Dictionary<string, int> keyDic = new Dictionary<string, int>();
    Dictionary<string, int> playedDic = new Dictionary<string, int>();
    Dictionary<string, string> timeDic = new Dictionary<string, string>();

    Dictionary<string, int> directoryString = new Dictionary<string, int>();
    Dictionary<int, string> directoryInteger = new Dictionary<int, string>();
    Dictionary<int, int> directoryPoint = new Dictionary<int, int>();
    Dictionary<int, Dictionary<string, string>> directoryTime = new Dictionary<int, Dictionary<string, string>>();
    List<string> dirName = new List<string>();
    Dictionary<int, List<string>> fileInfo = new Dictionary<int, List<string>>();
    Dictionary<int, Dictionary<string, string>> fileTime = new Dictionary<int, Dictionary<string, string>>();
    List<string> fileName = new List<string>();
    Dictionary<int, Dictionary<string, AudioClip>> audioInfo = new Dictionary<int, Dictionary<string, AudioClip>>();
    List<int> idxInfo = new List<int>();
    public Dictionary<int, List<int>> directoryPathIdx = new Dictionary<int, List<int>>();

    StartDBController _StartDBController;

    /**
     * @date : 2018.01.29
     * @author : Won,Junseok
     * @desc : SQL DB 접근 정보 생성
     *         인터페이스를 사용하기 전에 반드시 실행 필요
     * @params
     *  - DB_File : SQL_DB 파일 이름 (ex) vrdb.db
     *  - arrSqls : 데이터베이스가 없는 경우 생성할 DB SQL 문
     *  
     **/
    public override void DropDB(string DB_File)
    {
        string filepath = string.Format("{0}/{1}", Static.STATIC.dir_path + "/DataBase", DB_File);
        FileInfo _finfo = new FileInfo(filepath);
        Debug.Log(filepath);
        // DB 파일이 존재하지 않는 경우, DB 자동 생성
        if (_finfo.Exists)
        {
            Debug.Log("DB 삭제 완료");
            try
            {
                System.IO.File.Delete(filepath);
                Close();
            }
            catch (System.Exception e)
            {
                Debug.Log("데이터베이스 에러 : " + e.Message);
            }
        }
    }
    public override void Init(string DB_File, ArrayList arrSqls = null)
    {
        string filepath = string.Format("{0}/{1}", Static.STATIC.dir_path + "/DataBase", DB_File);

        FileInfo _finfo = new FileInfo(filepath);

        try
        {
            Debug.Log("DB 정보가 존재하지 않아 DB 재생성");

            Open();

            if (arrSqls != null)
            {
                for (int i = 0; i < arrSqls.Count; i++)
                {
                    string sql = (string)arrSqls[i];

                    using (IDbCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("데이터베이스 에러 : " + e.Message);
        }
        Close();
        Debug.Log("DB 생성 완료");
        // }
    }

    /**
     * @date : 2018.01.29
     * @author : Won,Junseok
     * @desc : DB의 데이터를 조회
     * @params
     *  - sql : 실행시킬 SQL 문
     *  - dtParam : 바인딩된 파라미터 값(첫번째 row의 값만으로 조회)
     *  
     **/
    public override DataTable SelectData(string sql, DataTable dtParam) //셀렉트데이터는 아직 이해 못함 //geun(2019.03.28)
    {
        DataSet ds = new DataSet();

        try
        {
            Open();

            using (var cmd = new SqliteCommand(sql, connection))
            {
                cmd.CommandText = sql;

                if (dtParam != null && dtParam.Rows.Count > 0)
                {
                    DataRow dr = dtParam.Rows[0];

                    foreach (DataColumn dc in dtParam.Columns)
                    {
                        SqliteParameter param = new SqliteParameter();
                        param.Value = dr[dc.ColumnName];

                        cmd.Parameters.Add(param);
                    }
                }


                // SqlDataAdapter 초기화
                SqliteDataAdapter adapter = new SqliteDataAdapter(cmd);

                // Fill 메서드 실행하여 결과 DataSet을 리턴받음
                // 비연결 모드로 데이터 가져오기
                adapter.Fill(ds);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("데이터베이스 에러 : " + e.Message);
        }
        Close();

        return ds.Tables[0];
    }

    /**
     * @date : 2018.01.29
     * @author : Won,Junseok
     * @desc : DB에 데이터를 생성
     * @params
     *  - sql : 실행시킬 SQL 문
     *  - dtParam : 바인딩된 파라미터 값
     * 
     **/
    public override void InsertData(string sql, DataTable dtParam)
    {
        try
        {

            Open();

            using (IDbCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;

                foreach (DataRow dr in dtParam.Rows)
                {
                    cmd.Parameters.Clear();

                    foreach (DataColumn dc in dtParam.Columns)
                    {
                        SqliteParameter param = new SqliteParameter();
                        param.Value = dr[dc.ColumnName];

                        cmd.Parameters.Add(param);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("데이터베이스 에러 : " + e.Message);
        }

        Close();
    }

    /**
     * @date : 2019.11.11
     * @author : Day
     * @desc : TotalTable.sqlite 는 모든 DB파일의
     *         이름과 실행횟수 시나리오번호를 저장.
     **/
    /*
     * HiddenMenu Start(코루틴)에서만 시작
     */
    public override void TotalStart(string sql)
    {
        try
        {
            Open();

            using (IDbCommand cmd = connection.CreateCommand())
            {
                List<string> tmp = new List<string>();

                cmd.CommandText = sql;
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string sfile = reader.GetString(0);
                        int key = reader.GetInt32(1);
                        int played = reader.GetInt32(2);
                        string time = reader.GetString(3);

                        tmp.Add(sfile);
                        if (!keyDic.ContainsKey(sfile))
                        {
                            keyDic.Add(sfile, key);
                        }
                        if (!playedDic.ContainsKey(sfile))
                        {
                            playedDic.Add(sfile, played);
                        }
                        if (!timeDic.ContainsKey(sfile))
                        {
                            timeDic.Add(sfile, time);
                        }

                    }
                }
                /*
                 * temp ==> 파일 이름
                 * playedDic ==> 실행횟수
                 * keyDic ==> 시나리오번호
                 */
                _StartDBController = GameObject.Find("StartDBController").GetComponent<StartDBController>();
                _StartDBController.playedDic = playedDic;
                _StartDBController.keyDic = keyDic;
                _StartDBController.timeDic = timeDic;
                _StartDBController.saveFileName = tmp;
            }

        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }

        Close();
    }
    //시나리오 번호를 가져옴.
    public override int TotalGetKey(string sql)
    {
        int num = -1;
        try
        {

            Open();

            using (IDbCommand cmd = connection.CreateCommand())
            {

                cmd.CommandText = sql;

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    num = reader.GetInt32(0);
                }


            }

        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }

        Close();

        return num;
    }

    public override void DeleteRow(string sql)
    {
        try
        {

            Open();

            using (IDbCommand cmd = connection.CreateCommand())
            {

                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();

            }

        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }

        Close();
    }

    //업데이트 후 Played UI에 갱신
    public override void PlusPlayed(string sql)
    {
        int _key = 0;
        int _played = 0;
        try
        {
            Open();

            using (IDbCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();

                GameObject tmp = GameObject.Find("Canvas/HiddenCanvas/Scroll View/Viewport/SaveContent").transform.Find(Static.STATIC._saveClickButton.name).gameObject;

                cmd.CommandText = "SELECT * From TotalTable WHERE SaveFile='" + Static.STATIC._saveClickButton.name + "'";
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    _key = reader.GetInt32(1);
                    _played = reader.GetInt32(2);
                    tmp.transform.GetChild(0).GetComponent<Text>().text = "<color=#0000ff>" + _key + ". </color>" + Static.STATIC._saveClickButton.name + " : <color=#ff0000>" + _played + "</color>";

                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
        Close();

        MySqlConnection dbConnection = null;
        string strConn = "Server=localhost;database=quest;UserId=root;Password=qwe123";

        try
        {
            dbConnection = new MySqlConnection(strConn);

            dbConnection.Open();

            string query = "Update g5_qstnretn SET play_time = " + _played + " where content_no = " + _key;
            MySqlCommand cmd = new MySqlCommand(query, dbConnection);
            cmd.ExecuteNonQuery();
        }
        catch (System.Exception msg)
        {
            Debug.Log(msg);
        }
        dbConnection.Close();
    }

    public override void VoiceDirectory_Start(string sql)
    {
        try
        {
            Open();

            using (IDbCommand cmd = connection.CreateCommand())
            {
                List<string> tmp = new List<string>();

                cmd.CommandText = sql;
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string dir = reader.GetString(0);
                        int key = reader.GetInt32(1);
                        int prev_key = reader.GetInt32(2);
                        string time = reader.GetString(3);

                        tmp.Add(dir);
                        if (!directoryString.ContainsKey(dir))
                        {
                            directoryString.Add(dir, key);
                        }
                        if (!directoryInteger.ContainsKey(key))
                        {
                            directoryInteger.Add(key, dir);
                            idxInfo.Add(key);
                        }
                        if (!directoryPoint.ContainsKey(key))
                        {
                            directoryPoint.Add(key, prev_key);
                        }
                        if (!directoryTime.ContainsKey(key))
                        {
                            Dictionary<string, string> ss = new Dictionary<string, string>();
                            directoryTime.Add(key, ss);
                        }
                        if (!directoryTime[key].ContainsKey(dir))
                        {
                            directoryTime[key].Add(dir, time);
                        }
                        if (prev_key == 0 && key == 0) continue;
                        if (!directoryPathIdx.ContainsKey(prev_key))
                        {
                            directoryPathIdx.Add(prev_key, new List<int>());
                        }
                        //if (!directoryPathIdx.ContainsKey(key))
                        //{
                        //    directoryPathIdx.Add(key, new List<int>());
                        //}
                        directoryPathIdx[prev_key].Add(key);
                    }
                }

                _StartDBController = GameObject.Find("StartDBController").GetComponent<StartDBController>();
                _StartDBController.directoryString = directoryString;
                _StartDBController.directoryInteger = directoryInteger;
                _StartDBController.directoryPoint = directoryPoint;
                _StartDBController.directoryTime = directoryTime;
                _StartDBController.directoryName = tmp;
                _StartDBController.idxInfo = idxInfo;
                _StartDBController.directoryPathIdx = directoryPathIdx;
            }

        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }

        Close();
    }

    public override void VoiceFile_Start(string sql)
    {
        try
        {
            Open();

            using (IDbCommand cmd = connection.CreateCommand())
            {
                List<string> tmp = new List<string>();

                cmd.CommandText = sql;
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string f = reader.GetString(0);
                        int key = reader.GetInt32(1);
                        string time = reader.GetString(2);

                        tmp.Add(f);

                        if (!fileInfo.ContainsKey(key))
                        {
                            fileInfo[key] = new List<string>();
                        }
                        fileInfo[key].Add(f);
                        if (!fileTime.ContainsKey(key))
                        {
                            Dictionary<string, string> ss = new Dictionary<string, string>();
                            fileTime.Add(key, ss);
                        }
                        if (!fileTime[key].ContainsKey(f))
                        {
                            fileTime[key].Add(f, time);
                        }                      
                    }
                }

                _StartDBController = GameObject.Find("StartDBController").GetComponent<StartDBController>();
                _StartDBController.fileInfo = fileInfo;
                _StartDBController.fileTime = fileTime;
            }

        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }

        Close();
    }

    public override void VoiceAudio_Start(string sql)
    {
        try
        {
            Open();

            using (IDbCommand cmd = connection.CreateCommand())
            {
                List<string> tmp = new List<string>();

                cmd.CommandText = sql;
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string f = reader.GetString(0);
                        int key = reader.GetInt32(1);
                        string p = reader.GetString(2);

                        AudioClip tmp_audio = Resources.Load<AudioClip>(p + f);
                        if (!audioInfo.ContainsKey(key))
                        {
                            Dictionary<string, AudioClip> sa = new Dictionary<string, AudioClip>();
                            audioInfo.Add(key, sa);
                        }
                        if (!audioInfo[key].ContainsKey(f))
                        {
                            audioInfo[key].Add(f, tmp_audio);
                        }
                    }
                }

                _StartDBController = GameObject.Find("StartDBController").GetComponent<StartDBController>();
                _StartDBController.audioInfo = audioInfo;
            }

        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }

        Close();
    }

    /**
     * @date : 2018.01.29
     * @author : Won,Junseok
     * @desc :
     *  
     **/
    public override void UpdateData(string sql, DataTable dtParam)
    {
        try
        {


            Open();

            using (IDbCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;

                foreach (DataRow dr in dtParam.Rows)
                {
                    foreach (DataColumn dc in dtParam.Columns)
                    {
                        //cmd.Parameters.AddWithValue(dc.ColumnName, dr[dc.ColumnName]);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }

        Close();
    }

    /**
     * @date : 2018.01.29
     * @author : Won,Junseok
     * @desc :
     *  
     **/
    public override void DeleteData(string sql, DataTable dtParam)
    {
        try
        {


            Open();

            using (IDbCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;

                foreach (DataRow dr in dtParam.Rows)
                {
                    foreach (DataColumn dc in dtParam.Columns)
                    {
                        //cmd.Parameters.AddWithValue(dc.ColumnName, dr[dc.ColumnName]);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }

        Close();
    }
}
/**
     * @date : 2018.04.07
     * @author : Lee Sang ho
     * @desc : 경로에 있는 .db파일들을 가져옮
     **/

