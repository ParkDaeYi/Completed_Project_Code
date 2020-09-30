using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class VRDBController
{

    static GameDBController db = new GameDBController();
    /**
     * @date : 2019.11.11
     * @author : Day
     * @desc : Total~ 함수들은 오로지 TotalTable.sqlite에만 접근
     *         .sqlite가 두개 이상 열려있으면 안되므로
     *         따로 작업하기 위해 만듦
     */
    /*
     *  TotalTable.sqlite 파일이 없으므로 생성
     */
    public static void Total_Init()
    {
        GameDBController.DB_Name = Static.STATIC.dir_path + "/DataBase/TotalTable.sqlite";

        ArrayList arrSqls = new ArrayList();

        arrSqls.Add("CREATE TABLE TotalTable ( `SaveFile` TEXT, `Key` INTEGER PRIMARY KEY AUTOINCREMENT, `Played` INTEGER NOT NULL, `Data` TEXT)");

        db.Init(GameDBController.DB_Name, arrSqls);
    }
    /*
     *  TotalTable.sqlite 파일에 값을 추가
     */
    public static void Total_add(string s, string lw)
    {

        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("SaveFile", typeof(string)));
        dt.Columns.Add(new DataColumn("Key", typeof(int)));
        dt.Columns.Add(new DataColumn("Played", typeof(int)));
        dt.Columns.Add(new DataColumn("Data", typeof(string)));

        DataRow row = dt.NewRow();

        row["SaveFile"] = s;
        row["Played"] = 0;
        row["Data"] = lw;

        dt.Rows.Add(row);

        db.InsertData("INSERT INTO TotalTable(SaveFile,Key,Played,Data) VALUES(?,?,?,?)", dt);
    }

    //HiddenMenuControl Start에서 사용
    public static void Total_GetKey_Start()
    {
        db.TotalStart("SELECT * FROM TotalTable");
    }

    //Save 파일 삭제 시
    public static void Total_Delete(string s)
    {
        GameDBController.DB_Name = Static.STATIC.dir_path + "/DataBase/TotalTable.sqlite";

        db.DeleteRow("DELETE FROM TotalTable WHERE SaveFile='" + s + "'");
    }

    //ContentNumber 를 가져옴
    public static int Total_GetKey(string s)
    {
        return db.TotalGetKey("SELECT Key From TotalTable WHERE SaveFile='" + s + "'");
    }

    public static void VoiceDirectory_Init()
    {
        GameDBController.DB_Name = Static.STATIC.dir_path + "/DataBase/VoiceDirectory.sqlite";

        ArrayList arrSqls = new ArrayList();

        arrSqls.Add("CREATE TABLE VoiceDirectory( `DirectoryName` TEXT NOT NULL, `Key` INTEGER NOT NULL, `Prev_Key` INTEGER NOT NULL, `Data` TEXT NOT NULL)");

        db.Init(GameDBController.DB_Name, arrSqls);
    }

    public static void VoiceFile_Init()
    {
        GameDBController.DB_Name = Static.STATIC.dir_path + "/DataBase/VoiceFile.sqlite";

        ArrayList arrSqls = new ArrayList();

        arrSqls.Add("CREATE TABLE VoiceFile( `FileName` TEXT NOT NULL, `Key` INTEGER NOT NULL, `Data` TEXT NOT NULL)");
        arrSqls.Add("CREATE TABLE VoiceAudio( `FileName` TEXT NOT NULL, `Key` INTEGER NOT NULL, `FilePath` TEXT NOT NULL)");

        db.Init(GameDBController.DB_Name, arrSqls);
    }

    public static void VoiceDirectory_add(string s, string lw,int key,int prev_key)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("DirectoryName", typeof(string)));
        dt.Columns.Add(new DataColumn("Key", typeof(int)));
        dt.Columns.Add(new DataColumn("Prev_Key", typeof(int)));
        dt.Columns.Add(new DataColumn("Data", typeof(string)));

        DataRow row = dt.NewRow();

        row["DirectoryName"] = s;
        row["Key"] = key;
        row["Prev_Key"] = prev_key;
        row["Data"] = lw;

        dt.Rows.Add(row);

        db.InsertData("INSERT INTO VoiceDirectory(DirectoryName,Key,Prev_Key,Data) VALUES(?,?,?,?)", dt);
    }

    public static void VoiceFile_add(string s, string lw, int key)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("FileName", typeof(string)));
        dt.Columns.Add(new DataColumn("Key", typeof(int)));
        dt.Columns.Add(new DataColumn("Data", typeof(string)));

        DataRow row = dt.NewRow();

        row["FileName"] = s;
        row["Key"] = key;
        row["Data"] = lw;

        dt.Rows.Add(row);

        db.InsertData("INSERT INTO VoiceFile(FileName,Key,Data) VALUES(?,?,?)", dt);
    }

    public static void VoiceAudio_add(string s, string fp, int key)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("FileName", typeof(string)));
        dt.Columns.Add(new DataColumn("Key", typeof(int)));
        dt.Columns.Add(new DataColumn("FilePath", typeof(string)));

        DataRow row = dt.NewRow();

        row["FileName"] = s;
        row["Key"] = key;
        row["FilePath"] = fp;

        dt.Rows.Add(row);

        db.InsertData("INSERT INTO VoiceAudio(FileName,Key,FilePath) VALUES(?,?,?)", dt);
    }

    public static void VoiceDirectory_Start()
    {
        db.VoiceDirectory_Start("SELECT * FROM VoiceDirectory");
    }

    public static void VoiceFile_Start()
    {
        db.VoiceFile_Start("SELECT * FROM VoiceFile");
    }

    public static void VoiceAudio_Start()
    {
        db.VoiceAudio_Start("SELECT * FROM VoiceAudio");
    }

    public static void VoiceDirectory_DeleteKey(int key)
    {
        GameDBController.DB_Name = Static.STATIC.dir_path + "/DataBase/VoiceDirectory.sqlite";

        db.DeleteRow("DELETE FROM VoiceDirectory WHERE Key=" + key);
    }

    public static void VoiceFile_DeleteKey(int key)
    {
        GameDBController.DB_Name = Static.STATIC.dir_path + "/DataBase/VoiceFile.sqlite";

        db.DeleteRow("DELETE FROM VoiceFile WHERE Key=" + key);
        db.DeleteRow("DELETE FROM VoiceAudio WHERE Key=" + key);
    }

    public static void VoiceFile_DeleteName(int key, string name)
    {
        GameDBController.DB_Name = Static.STATIC.dir_path + "/DataBase/VoiceFile.sqlite";

        db.DeleteRow("DELETE FROM VoiceFile WHERE Key IN (SELECT Key FROM VoiceFile WHERE Key=" + key + " AND FileName='" + name + "')");
        string[] name2 = name.Split('.');
        db.DeleteRow("DELETE FROM VoiceAudio WHERE Key IN (SELECT Key FROM VoiceAudio WHERE Key=" + key + " AND FileName='" + name2[0] + "')");
    }

    // VR DB 초기화
    /**
     * @date : 2018.04.01
     * @author : Won,Junseok
     * @desc : VR DB 초기화
     *         사용하기 전에 반드시 실행 필요
     *  
     **/
    public static void Init(string s)
    {
        //날짜 추가
        // System.DateTime time = System.DateTime.Now;
        // string DB_File = time.ToString("MM/dd/HH/mm")+".db";
        //string DB_File = "vr.db";
        string DB_File = s + ".sqlite";

        GameDBController.DB_Name = DB_File;


        // DB 생성 정보 설정
        ArrayList arrSqls = new ArrayList();

        // DB 생성 SQL 추가하여 생성
        arrSqls.Add("CREATE TABLE Object ( `ObjectNumber` INTEGER, `OriginNumber` INTEGER NOT NULL, `ObjectName` TEXT NOT NULL, `positionX` REAL NOT NULL, `positionY` REAL NOT NULL, `positionZ` REAL NOT NULL, `rotationX` REAL NOT NULL, `rotationY` REAL NOT NULL, `rotationZ` REAL NOT NULL, `scaleX` REAL NOT NULL, `scaleY` REAL NOT NULL, `scaleZ` REAL NOT NULL, PRIMARY KEY(`ObjectNumber`) )");
        arrSqls.Add("CREATE TABLE Wall ( `ObjectNumber` INTEGER, `OriginNumber` INTEGER NOT NULL, `ObjectName` TEXT NOT NULL, `positionX` REAL NOT NULL, `positionY` REAL NOT NULL, `positionZ` REAL NOT NULL, `rotationX` REAL NOT NULL, `rotationY` REAL NOT NULL, `rotationZ` REAL NOT NULL, `scaleX` REAL NOT NULL, `scaleY` REAL NOT NULL, `scaleZ` REAL NOT NULL,`PlaceNumber` INTEGER NOT NULL,`TilingX` REAL NOT NULL,`TilingY` REAL NOT NULL, PRIMARY KEY(`ObjectNumber`) )");
        arrSqls.Add("CREATE TABLE Human ( `ObjectNumber` INTEGER, `OriginNumber` INTEGER NOT NULL, `ObjectName` TEXT NOT NULL, `positionX` REAL NOT NULL, `positionY` REAL NOT NULL, `positionZ` REAL NOT NULL, `rotationX` REAL NOT NULL, `rotationY` REAL NOT NULL, `rotationZ` REAL NOT NULL, `scaleX` REAL NOT NULL, `scaleY` REAL NOT NULL, `scaleZ` REAL NOT NULL, 'HumanNumber' INTEGER NOT NULL, PRIMARY KEY(`ObjectNumber`) )");
        arrSqls.Add("CREATE TABLE BigAnimation ( `Number` INTEGER , `BarX` REAL NOT NULL, `BarWidth` REAL NOT NULL, `AnimationName` TEXT NOT NULL, `AnibarName` TEXT NOT NULL, `HumanNumber` INTEGER NOT NULL,'AnimationText' TEXT NOT NULL, PRIMARY KEY(`Number`) )");
        arrSqls.Add("CREATE TABLE SmallAnimation ( `Number` INTEGER, `HumanNumber` INTEGER NOT NULL, `MoveOrState` INTEGER NOT NULL, `ActionOrFace` INTEGER NOT NULL, `LayerNumber` INTEGER NOT NULL, `BarX` REAL NOT NULL, `BarWidth` REAL NOT NULL, `ArriveX` REAL NOT NULL, `ArriveY` REAL NOT NULL, `ArriveZ` REAL NOT NULL, 'OriginNumber' INTEGER NOT NULL ,'Rotation' INTEGER NOT NULL, PRIMARY KEY(`Number`))");
        arrSqls.Add("CREATE TABLE VoiceBig ( `Number` INTEGER , `BarX` REAL NOT NULL, `BarWidth` REAL NOT NULL, `VoiceName` TEXT NOT NULL ,`HumanNumber` INTEGER NOT NULL,'VoiceText' TEXT NOT NULL, PRIMARY KEY(`Number`) )");
        arrSqls.Add("CREATE TABLE VoiceSmall('Number' INTEGER, 'HumanNumber' INTEGER NOT NULL,'Key' INTEGER NOT NULL,'VoiceName' TEXT NOT NULL, `BarX` REAL NOT NULL, `BarWidth` REAL NOT NULL, 'OriginNumber' INTEGER NOT NULL , PRIMARY KEY(`Number`))");
        arrSqls.Add("CREATE TABLE HandItem(`OriginNumber` INTEGER NOT NULL, 'HumanNumber' INTEGER NOT NULL,`isLeft` INTEGER NOT NULL)");
        arrSqls.Add("CREATE TABLE Dress(`HumanNumber` INTEGER NOT NULL, `ShirtName` TEXT, `Shirt_R` REAL, `Shirt_G` REAL, `Shirt_B` REAL,`PantName` TEXT, `Pant_R` REAL, `Pant_G` REAL, `Pant_B` REAL,`ShoesName` TEXT, `Shoes_R` REAL, `Shoes_G` REAL, `Shoes_B` REAL)");
        arrSqls.Add("CREATE TABLE House(`isSimpleHouse` INTEGER NOT NULL,`isHouse` INTEGER NOT NULL,`isCar` INTEGER NOT NULL)");
        db.Init(GameDBController.DB_Name, arrSqls);
    }
    //그냥 Sqlite에 연결해주기 위해 사용.
    public static void ConIn(string s)
    {
        string DB_File = s + ".sqlite";

        GameDBController.DB_Name = DB_File;
    }
    public static void Drop(string s)
    {
        string DB_File = s + ".sqlite";
        GameDBController.DB_Name = DB_File;
        Debug.Log("Drop 실행 DB_Name = " + DB_File);
        db.DropDB(GameDBController.DB_Name);
    }
    //실행횟수를 증가시켜 줌
    public static void plusPlayed(string s)
    {
        string[] name = s.Split('/');
        GameDBController.DB_Name = Static.STATIC.dir_path + "/DataBase/TotalTable.sqlite";

        db.PlusPlayed("UPDATE TotalTable SET Played=Played+1 WHERE SaveFile='" + name[2] + "'");
    }

    /**
     * @date : 2018.04.01
     * @author : Won,Junseok
     * @desc : 아이템 추가
     * @params
     *  - obj_id : 객체 값(KEY 값, 중복 저장될 수 없음)
     *  - obj_grp_id : 고유 ID, 해당 객체의 소속 그룹
     *  - pos_x : 대상 x 좌표
     *  - pos_y : 대상 y 좌표
     *  - pos_z : 대상 z 좌표
     *  
     **/
    public static void addHouseInfo(int _isSimpleHouse, int _isHouse, int _isCar)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("isSimpleHouse", typeof(int)));
        dt.Columns.Add(new DataColumn("isHouse", typeof(int)));
        dt.Columns.Add(new DataColumn("isCar", typeof(int)));
        DataRow row = dt.NewRow();

        row["isSimpleHouse"] = _isSimpleHouse;
        row["isHouse"] = _isHouse;
        row["isCar"] = _isCar;

        dt.Rows.Add(row);

        db.InsertData("INSERT INTO House(isSimpleHouse,isHouse,isCar) VALUES(?, ?, ?)", dt);
    }
    public static void addItemInfo(int _objectNumber, int _originNumber, string _objectName, float _positionX, float _positionY, float _positionZ, float _rotationX, float _rotationY, float _rotationZ, float _scaleX, float _scaleY, float _scaleZ)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("ObjectNumber", typeof(int)));
        dt.Columns.Add(new DataColumn("OriginNumber", typeof(int)));
        dt.Columns.Add(new DataColumn("ObjectName", typeof(string)));
        dt.Columns.Add(new DataColumn("positionX", typeof(float)));
        dt.Columns.Add(new DataColumn("positionY", typeof(float)));
        dt.Columns.Add(new DataColumn("positionZ", typeof(float)));
        dt.Columns.Add(new DataColumn("rotationX", typeof(float)));
        dt.Columns.Add(new DataColumn("rotationY", typeof(float)));
        dt.Columns.Add(new DataColumn("rotationZ", typeof(float)));
        dt.Columns.Add(new DataColumn("scaleX", typeof(float)));
        dt.Columns.Add(new DataColumn("scaleY", typeof(float)));
        dt.Columns.Add(new DataColumn("scaleZ", typeof(float)));
        dt.Columns.Add(new DataColumn("ObjectText", typeof(string)));
        DataRow row = dt.NewRow();

        row["ObjectNumber"] = _objectNumber;
        row["OriginNumber"] = _originNumber;
        row["ObjectName"] = _objectName;
        row["positionX"] = _positionX;
        row["positionY"] = _positionY;
        row["positionZ"] = _positionZ;
        row["rotationX"] = _rotationX;
        row["rotationY"] = _rotationY;
        row["rotationZ"] = _rotationZ;
        row["scaleX"] = _scaleX;
        row["scaleY"] = _scaleY;
        row["scaleZ"] = _scaleZ;


        dt.Rows.Add(row);

        db.InsertData("INSERT INTO Object(ObjectNumber,OriginNumber,ObjectName,positionX,positionY,positionZ,rotationX,rotationY,rotationZ,scaleX,scaleY,scaleZ) VALUES(?, ?, ?, ?, ?, ?, ?, ?,?,?,?,?)", dt);
    }
    public static void addWallInfo(int _objectNumber, int _originNumber, string _objectName, float _positionX, float _positionY, float _positionZ, float _rotationX, float _rotationY, float _rotationZ, float _scaleX, float _scaleY, float _scaleZ, int _placeNumber, float _tilingX, float _tilingY)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("ObjectNumber", typeof(int)));
        dt.Columns.Add(new DataColumn("OriginNumber", typeof(int)));
        dt.Columns.Add(new DataColumn("ObjectName", typeof(string)));
        dt.Columns.Add(new DataColumn("positionX", typeof(float)));
        dt.Columns.Add(new DataColumn("positionY", typeof(float)));
        dt.Columns.Add(new DataColumn("positionZ", typeof(float)));
        dt.Columns.Add(new DataColumn("rotationX", typeof(float)));
        dt.Columns.Add(new DataColumn("rotationY", typeof(float)));
        dt.Columns.Add(new DataColumn("rotationZ", typeof(float)));
        dt.Columns.Add(new DataColumn("scaleX", typeof(float)));
        dt.Columns.Add(new DataColumn("scaleY", typeof(float)));
        dt.Columns.Add(new DataColumn("scaleZ", typeof(float)));
        dt.Columns.Add(new DataColumn("PlaceNumber", typeof(int)));
        dt.Columns.Add(new DataColumn("TilingX", typeof(float)));
        dt.Columns.Add(new DataColumn("TilingY", typeof(float)));
        DataRow row = dt.NewRow();

        row["ObjectNumber"] = _objectNumber;
        row["OriginNumber"] = _originNumber;
        row["ObjectName"] = _objectName;
        row["positionX"] = _positionX;
        row["positionY"] = _positionY;
        row["positionZ"] = _positionZ;
        row["rotationX"] = _rotationX;
        row["rotationY"] = _rotationY;
        row["rotationZ"] = _rotationZ;
        row["scaleX"] = _scaleX;
        row["scaleY"] = _scaleY;
        row["scaleZ"] = _scaleZ;
        row["PlaceNumber"] = _placeNumber;
        row["TilingX"] = _tilingX;
        row["TilingY"] = _tilingY;

        dt.Rows.Add(row);

        db.InsertData("INSERT INTO Wall(ObjectNumber,OriginNumber,ObjectName,positionX,positionY,positionZ,rotationX,rotationY,rotationZ,scaleX,scaleY,scaleZ,PlaceNumber,TilingX,TilingY) VALUES(?, ?, ?, ?, ?, ?, ?, ?,?,?,?,?,?,?,?)", dt);
    }
    public static void addHumanInfo(int _objectNumber, int _originNumber, string _objectName, float _positionX, float _positionY, float _positionZ, float _rotationX, float _rotationY, float _rotationZ, float _scaleX, float _scaleY, float _scaleZ, int _humanNumber)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("ObjectNumber", typeof(int)));
        dt.Columns.Add(new DataColumn("OriginNumber", typeof(int)));
        dt.Columns.Add(new DataColumn("ObjectName", typeof(string)));
        dt.Columns.Add(new DataColumn("positionX", typeof(float)));
        dt.Columns.Add(new DataColumn("positionY", typeof(float)));
        dt.Columns.Add(new DataColumn("positionZ", typeof(float)));
        dt.Columns.Add(new DataColumn("rotationX", typeof(float)));
        dt.Columns.Add(new DataColumn("rotationY", typeof(float)));
        dt.Columns.Add(new DataColumn("rotationZ", typeof(float)));
        dt.Columns.Add(new DataColumn("scaleX", typeof(float)));
        dt.Columns.Add(new DataColumn("scaleY", typeof(float)));
        dt.Columns.Add(new DataColumn("scaleZ", typeof(float)));
        dt.Columns.Add(new DataColumn("HumanNumber", typeof(int)));
        DataRow row = dt.NewRow();

        row["ObjectNumber"] = _objectNumber;
        row["OriginNumber"] = _originNumber;
        row["ObjectName"] = _objectName;
        row["positionX"] = _positionX;
        row["positionY"] = _positionY;
        row["positionZ"] = _positionZ;
        row["rotationX"] = _rotationX;
        row["rotationY"] = _rotationY;
        row["rotationZ"] = _rotationZ;
        row["scaleX"] = _scaleX;
        row["scaleY"] = _scaleY;
        row["scaleZ"] = _scaleZ;
        row["HumanNumber"] = _humanNumber; //휴먼넘버라고 지정 => 스케줄러와 애니메이션의 연결


        dt.Rows.Add(row);

        db.InsertData("INSERT INTO Human(ObjectNumber,OriginNumber,ObjectName,positionX,positionY,positionZ,rotationX,rotationY,rotationZ,scaleX,scaleY,scaleZ,HumanNumber) VALUES(?, ?, ?, ?, ?, ?, ?, ?,?,?,?,?,?)", dt);
    }

    public static void addHandItemInfo(int _originNumber, int _humanNumber, int _isLeft)
    {

        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("OriginNumber", typeof(int)));
        dt.Columns.Add(new DataColumn("HumanNumber", typeof(int)));
        dt.Columns.Add(new DataColumn("isLeft", typeof(int)));
        DataRow row = dt.NewRow();

        row["OriginNumber"] = _originNumber;
        row["HumanNumber"] = _humanNumber;
        row["isLeft"] = _isLeft;

        dt.Rows.Add(row);

        db.InsertData("INSERT INTO HandItem(OriginNumber,HumanNumber,isLeft) VALUES(?, ?, ?)", dt);
    }

    public static void addDressInfo(int _humanNumber, string _shirtName, float _shirt_R, float _shirt_G, float _shirt_B, string _pantName, float _pant_R, float _pant_G, float _pant_B, string _shoesName, float _shoes_R, float _shoes_G, float _shoes_B)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("HumanNumber", typeof(int)));
        dt.Columns.Add(new DataColumn("ShirtName", typeof(string)));
        dt.Columns.Add(new DataColumn("Shirt_R", typeof(float)));
        dt.Columns.Add(new DataColumn("Shirt_G", typeof(float)));
        dt.Columns.Add(new DataColumn("Shirt_B", typeof(float)));
        dt.Columns.Add(new DataColumn("PantName", typeof(string)));
        dt.Columns.Add(new DataColumn("Pant_R", typeof(float)));
        dt.Columns.Add(new DataColumn("Pant_G", typeof(float)));
        dt.Columns.Add(new DataColumn("Pant_B", typeof(float)));
        dt.Columns.Add(new DataColumn("ShoesName", typeof(string)));
        dt.Columns.Add(new DataColumn("Shoes_R", typeof(float)));
        dt.Columns.Add(new DataColumn("Shoes_G", typeof(float)));
        dt.Columns.Add(new DataColumn("Shoes_B", typeof(float)));

        DataRow row = dt.NewRow();
        row["HumanNumber"] = _humanNumber;
        row["ShirtName"] = _shirtName;
        row["Shirt_R"] = _shirt_R;
        row["Shirt_G"] = _shirt_G;
        row["Shirt_B"] = _shirt_B;
        row["PantName"] = _pantName;
        row["Pant_R"] = _pant_R;
        row["Pant_G"] = _pant_G;
        row["Pant_B"] = _pant_B;
        row["ShoesName"] = _shoesName;
        row["Shoes_R"] = _shoes_R;
        row["Shoes_G"] = _shoes_G;
        row["Shoes_B"] = _shoes_B;
        dt.Rows.Add(row);

        db.InsertData("INSERT INTO Dress(HumanNumber, ShirtName, Shirt_R, Shirt_G, Shirt_B, PantName, Pant_R, Pant_G, Pant_B, ShoesName, Shoes_R, Shoes_G, Shoes_B) VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)", dt);
    }
    /**
     * @date : 2018.04.01
     * @author : Won,Junseok
     * @desc : 액션 추가
     * @params
     *  - obj_id : 객체값
     *  - strt_time : 시작시간(HH24MISS)
     *  - end_time : 종료시간(HH24MISS)
     *  - mov_pos_x : 이동된 대상 x 좌표
     *  - mov_pos_y : 이동된 대상 y 좌표
     *  - mov_pos_z : 이동된 대상 z 좌표
     *  
     **/
    public static void addBigActionInfo(int i, float _barX, float _barWidth, string _animationName, string _anibarName, int _humanNumber, string _animationText)
    {
        DataTable dt = new DataTable();

        dt.Columns.Add(new DataColumn("Number", typeof(int)));
        dt.Columns.Add(new DataColumn("BarX", typeof(float)));
        dt.Columns.Add(new DataColumn("BarWidth", typeof(float)));
        dt.Columns.Add(new DataColumn("AnimationName", typeof(string)));
        dt.Columns.Add(new DataColumn("AnibarName", typeof(string)));
        dt.Columns.Add(new DataColumn("HumanNumber", typeof(int)));
        dt.Columns.Add(new DataColumn("AnimationText", typeof(string)));
        DataRow row = dt.NewRow();

        row["Number"] = i;
        row["BarX"] = _barX;
        row["BarWidth"] = _barWidth;
        row["AnimationName"] = _animationName;
        row["AnibarName"] = _anibarName;
        row["HumanNumber"] = _humanNumber;
        row["AnimationText"] = _animationText;


        dt.Rows.Add(row);

        db.InsertData("INSERT INTO BigAnimation (Number,BarX,BarWidth,AnimationName,AnibarName,HumanNumber,AnimationText) VALUES(?,?, ?,?, ?, ?,?)", dt);
    }
    public static void addSmallActionInfo(int i, int _humanNumber, int _moveOrState, int _actionOrFace, int _layerNumber, float _barX, float _barWidth, float _arriveX, float _arriveY, float _arriveZ, int _originNumber, int _rotation)
    {
        DataTable dt = new DataTable();

        dt.Columns.Add(new DataColumn("Number", typeof(int)));
        dt.Columns.Add(new DataColumn("HumanNumber", typeof(int)));
        dt.Columns.Add(new DataColumn("MoveOrState", typeof(int)));
        dt.Columns.Add(new DataColumn("ActionOrFace", typeof(int)));
        dt.Columns.Add(new DataColumn("LayerNumber", typeof(int)));
        dt.Columns.Add(new DataColumn("BarX", typeof(float)));
        dt.Columns.Add(new DataColumn("BarWidth", typeof(float)));
        dt.Columns.Add(new DataColumn("ArriveX", typeof(float)));
        dt.Columns.Add(new DataColumn("ArriveY", typeof(float)));
        dt.Columns.Add(new DataColumn("ArriveZ", typeof(float)));
        dt.Columns.Add(new DataColumn("OriginNumber", typeof(int)));
        dt.Columns.Add(new DataColumn("Rotation", typeof(int)));

        DataRow row = dt.NewRow();

        row["Number"] = i;
        row["HumanNumber"] = _humanNumber;
        row["MoveOrState"] = _moveOrState;
        row["ActionOrFace"] = _actionOrFace;
        row["LayerNumber"] = _layerNumber;
        row["BarX"] = _barX;
        row["BarWidth"] = _barWidth;
        row["ArriveX"] = _arriveX;
        row["ArriveY"] = _arriveY;
        row["ArriveZ"] = _arriveZ;
        row["OriginNumber"] = _originNumber;
        row["Rotation"] = _rotation;

        dt.Rows.Add(row);

        db.InsertData("INSERT INTO SmallAnimation (Number,HumanNumber,MoveOrState,ActionOrFace,LayerNumber,BarX,BarWidth,ArriveX,ArriveY,ArriveZ,OriginNumber,Rotation) VALUES(?,?,?,?,?,?,?,?,?,?,?,?)", dt);
    }

    public static void AddBigVoiceInfo(int i, float _barX, float _barWidth, string _voiceName, int _humanNumber, string _voiceText)
    {
        DataTable dt = new DataTable();

        dt.Columns.Add(new DataColumn("Number", typeof(int)));
        dt.Columns.Add(new DataColumn("BarX", typeof(float)));
        dt.Columns.Add(new DataColumn("BarWidth", typeof(float)));
        dt.Columns.Add(new DataColumn("VoiceName", typeof(string)));
        dt.Columns.Add(new DataColumn("HumanNumber", typeof(int)));
        dt.Columns.Add(new DataColumn("VoiceText", typeof(string)));
        DataRow row = dt.NewRow();

        row["Number"] = i;
        row["BarX"] = _barX;
        row["BarWidth"] = _barWidth;
        row["VoiceName"] = _voiceName;
        row["HumanNumber"] = _humanNumber;
        row["VoiceText"] = _voiceText;


        dt.Rows.Add(row);

        db.InsertData("INSERT INTO VoiceBig (Number,BarX,BarWidth,VoiceName,HumanNumber,VoiceText) VALUES(?,?, ?, ?, ?,?)", dt);
    }
    public static void AddSmallVoiceInfo(int i, int _humanNumber, int _dir_key, string _voiceName, float _barX, float _barWidth, int _originNumber)
    {
        DataTable dt = new DataTable();

        dt.Columns.Add(new DataColumn("Number", typeof(int)));
        dt.Columns.Add(new DataColumn("HumanNumber", typeof(int)));
        dt.Columns.Add(new DataColumn("Key", typeof(int)));
        dt.Columns.Add(new DataColumn("VoiceName", typeof(string)));
        dt.Columns.Add(new DataColumn("BarX", typeof(float)));
        dt.Columns.Add(new DataColumn("BarWidth", typeof(float)));
        dt.Columns.Add(new DataColumn("OriginNumber", typeof(int)));

        DataRow row = dt.NewRow();

        row["Number"] = i;
        row["HumanNumber"] = _humanNumber;
        row["Key"] = _dir_key;
        row["VoiceName"] = _voiceName;
        row["BarX"] = _barX;
        row["BarWidth"] = _barWidth;
        row["OriginNumber"] = _originNumber;


        dt.Rows.Add(row);

        db.InsertData("INSERT INTO VoiceSmall (Number,HumanNumber,Key,VoiceName,BarX,BarWidth,OriginNumber) VALUES(?,?,?,?,?,?,?)", dt);
    }

    /**
     * @date : 2018.04.01
     * @author : Won,Junseok
     * @desc : 아이템 조회
     * @params
     *  - obj_id : 객체값(obj_id 값이 없으면 전체 조회)
     * @return
     *  + 아이템 정보 전체
     *  
     **/
    public static DataTable getHouseInfo()
    {
        string sql = "";

        DataTable dt = new DataTable();

        sql = "SELECT * FROM House";

        DataTable retDT = db.SelectData(sql, dt);
        return retDT;
    }
    public static DataTable getItemInfo()
    {

        string sql = "";

        DataTable dt = new DataTable();

        sql = "SELECT * FROM Object";

        DataTable retDT = db.SelectData(sql, dt);
        return retDT;
    }
    public static DataTable getHumanInfo()
    {

        string sql = "";

        DataTable dt = new DataTable();

        sql = "SELECT * FROM Human";

        DataTable retDT = db.SelectData(sql, dt);
        return retDT;
    }
    public static DataTable getHandItemInfo()
    {
        string sql = "";

        DataTable dt = new DataTable();

        sql = "SELECT * FROM HandItem";

        DataTable retDT = db.SelectData(sql, dt);
        return retDT;
    }
    public static DataTable getDressInfo()
    {
        string sql = "";

        DataTable dt = new DataTable();

        sql = "SELECT * FROM Dress";

        DataTable retDT = db.SelectData(sql, dt);
        return retDT;
    }
    public static DataTable getWallInfo()
    {
        string sql = "";

        DataTable dt = new DataTable();

        sql = "SELECT * FROM Wall";

        DataTable retDT = db.SelectData(sql, dt);
        return retDT;
    }
    /**
     * @date : 2018.04.01
     * @author : Won,Junseok
     * @desc : 액션 조회
     * @params
     *  - obj_id : 객체값(obj_id 값이 없으면 전체 조회)
     * @return
     *  + 액션 정보 전체
     *  
     **/
    public static DataTable getSmallAnimationInfo()
    {
        string sql = "";

        DataTable dt = new DataTable();

        sql = "SELECT * FROM SmallAnimation";

        DataTable retDT = db.SelectData(sql, dt);
        return retDT;
    }
    public static DataTable getBigAnimationInfo()
    {
        string sql = "";

        DataTable dt = new DataTable();

        sql = "SELECT * FROM BigAnimation";

        DataTable retDT = db.SelectData(sql, dt);
        return retDT;
    }
    public static DataTable getSmallVoiceInfo()
    {
        string sql = "";

        DataTable dt = new DataTable();

        sql = "SELECT * FROM VoiceSmall";

        DataTable retDT = db.SelectData(sql, dt);
        return retDT;
    }
    public static DataTable getBigVoiceInfo()
    {
        string sql = "";

        DataTable dt = new DataTable();

        sql = "SELECT * FROM VoiceBig";

        DataTable retDT = db.SelectData(sql, dt);
        return retDT;
    }
}
