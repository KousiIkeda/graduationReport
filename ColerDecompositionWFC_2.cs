using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColerDecompositionWFC_2 : MonoBehaviour
{
    [System.Serializable] private struct FrontConnectionInformation //前面の接続情報を格納する構造体
    {
        public int x;
        public int y;
        public int z;
        public int part;
        public int connectionX;
        public int connectionY;
        public int connectionZ;
    }
    [System.Serializable] private struct BackConnectionInformation //背面の接続情報を格納する構造体
    {
        public int x;
        public int y;
        public int z;
        public int part;
        public int connectionX;
        public int connectionY;
        public int connectionZ;
    }
    [System.Serializable] private struct RightConnectionInformation //右面の接続情報を格納する構造体
    {
        public int x;
        public int y;
        public int z;
        public int part;
        public int connectionX;
        public int connectionY;
        public int connectionZ;
    }
    [System.Serializable] private struct LeftConnectionInformation //左面の接続情報を格納する構造体
    {
        public int x;
        public int y;
        public int z;
        public int part;
        public int connectionX;
        public int connectionY;
        public int connectionZ;
    }
    [System.Serializable] private struct UpConnectionInformation //上面の接続情報を格納する構造体
    {
        public int x;
        public int y;
        public int z;
        public int part;
        public int connectionX;
        public int connectionY;
        public int connectionZ;
    }
    [System.Serializable] private struct DownConnectionInformation //下面の接続情報を格納する構造体
    {
        public int x;
        public int y;
        public int z;
        public int part;
        public int connectionX;
        public int connectionY;
        public int connectionZ;
    }
    /// <summary>
    /// ///////////////////////////////分解後のデータを格納するクラスを宣言する///////////////////////////////
    /// </summary>
    [System.Serializable] private class DecompositionCubeDate 
    {
        public int partNum;
        public int canConnectionNum;
        public int[,,] decompositionCubeArray;
        public Color color;
        public List<FrontConnectionInformation> frontConnectionInformation = new List<FrontConnectionInformation>();
        public List<BackConnectionInformation> backConnectionInformation = new List<BackConnectionInformation>();
        public List<RightConnectionInformation> rightConnectionInformation = new List<RightConnectionInformation>();
        public List<LeftConnectionInformation> leftConnectionInformation = new List<LeftConnectionInformation>();
        public List<UpConnectionInformation> upConnectionInformation = new List<UpConnectionInformation>();
        public List<DownConnectionInformation> downConnectionInformation = new List<DownConnectionInformation>();
    }
    /// <summary>
    /// ///////////////////////////////分解後のデータを格納する///////////////////////////////
    /// </summary>
    [SerializeField] private List<DecompositionCubeDate> decompositionCubeDateList = new List<DecompositionCubeDate>(); //分解後のデータを格納するリスト
    int[,,] decompositionCubeArray; //分解後の配列データを格納する配列
    private int cubeNum = 0; //生成したCubeの数をカウントする変数
    private int[,,] cubeNumList = new int[100,100,100]; //ボクセルデータのどのボクセルかを格納する配列
    private bool[,,] cubeBoolList = new bool[100, 100, 100]; //ボクセルデータのその座標にボクセルがあるかを格納する配列
    private List<GameObject> partsList = new List<GameObject>(); //パーツを格納するリスト
    private List<GameObject> colorChildList = new List<GameObject>(); //色分けで分解したパーツの子オブジェクトを格納するリスト
    private List<GameObject> cubes = new List<GameObject>(); //生成したCubeを格納するリスト
    private List<GameObject> colorPartsList = new List<GameObject>(); //色分けで分解したパーツを格納するリスト
    private List<Color> colorList = new List<Color>(); //色分けで分解したパーツの色を格納するリスト
    /// <summary>
    /// ///////////////////////////////WFCで生成可能なパーツの情報をまとめるクラス///////////////////////////////
    /// <summary>
    [System.Serializable] private class CanGeneratePartsInformation
    {
        public int x;
        public int y;
        public int z;
        public int connectionX;
        public int connectionY;
        public int connectionZ;
        public int canConnnectionPartsNum; //接続可能なパーツの数
        public int direction; //0:front,1:back,2:right,3:left,4:up,5:down
        public List<int> connectionParts = new List<int>();
    }
    /// <summary>
    /// ///////////////////////////////WFCで生成したパーツを格納する///////////////////////////////
    /// </summary>
    private List<CanGeneratePartsInformation> canGeneratePartsInformationList = new List<CanGeneratePartsInformation>(); //WFCで生成可能なパーツの情報をまとめるリスト
    private CanGeneratePartsInformation generatePartsInformation = new CanGeneratePartsInformation(); //WFCで生成可能なパーツの情報をまとめるクラス
    private bool isFirstGenerate = false; //最初の生成が行われたかを判定する変数
    private int firstHeight = 0; //最初に生成するパーツの高さを格納する変数
    private int generateMinPartListIndex = 0; //最小のインデックスを格納する変数
    private int cubeNumWFC = 0; //WFCで生成したCubeの数をカウントする変数
    private int generatePart = 0; //WFCで生成するパーツの番号
    private List<GameObject> partsListWFC = new List<GameObject>(); //WFCで生成したパーツを格納するリスト
    private List<GameObject> cubesWFC = new List<GameObject>(); //WFCで生成したCubeを格納するリスト
    private List<int> partsNumListWFC = new List<int>(); //WFCで生成したパーツの番号を格納するリスト
    private List<int> canConnectionPartsList = new List<int>(); //WFCで生成する接続可能なパーツの番号を格納するリスト
    private List<int> generateMinPartList = new List<int>(); //最小のインデックスを格納するリスト
    private int[,,] cubeNumListWFC = new int[100,100,100]; //WFCで生成したボクセルデータのどのボクセルかを格納する配列
    private int[,,] cubeTypeListWFC = new int[100, 100, 100]; //WFCで生成したボクセルデータのその座標に何の種類のボクセルがあるかを格納する配列
    /// <summary>
    /// ///////////////////////////////SerializeField//////////////////////////////////////////////////////////
    /// </summary>
    [SerializeField] private int propagationNum; //伝播の回数
    [SerializeField] private GameObject cubePrefab; //生成するCubeのPrefab
    [SerializeField] private string filePath; //読み込むファイルのパス
    [SerializeField] private int N; //N×N×Nに分解する
    [SerializeField] private int maxHeight; //最大の高さ
    void Start()
    {
        decompositionCubeArray = new int[N,N,N];
        ArrayInitialization(cubeNumList);
        LoadAndGenerateCubes();
        ColerDecomposition();
        partsDecomposition();
        ConnectionInformationSaving();
        WFC();
    }

    void WFC() //WFC実行
    {
        for (int i = 0; i < cubeTypeListWFC.GetLength(0); i++)
        {
            for (int j = 0; j < cubeTypeListWFC.GetLength(1); j++)
            {
                for (int k = 0; k < cubeTypeListWFC.GetLength(2); k++)
                {
                    cubeTypeListWFC[i, j, k] = -1;
                }
            }
        }

        int firstPart = Random.Range(0,partsList.Count);
        Debug.Log("firstPart:"+firstPart+"decompositionCubeList:"+decompositionCubeDateList.Count);
        firstHeight = (Random.Range(0,maxHeight-N*3)/N)*N;
        GameObject part = new GameObject("part"+partsListWFC.Count);
        if(CheckCanGenerateCube(30,firstHeight,30,0,0,0,decompositionCubeDateList[firstPart]))
        {
            GenerateCube_ByWFC(decompositionCubeDateList[firstPart],30,firstHeight,31,0,0,0,0,part);
            partsListWFC.Add(part);
            partsNumListWFC.Add(firstPart);
            CheckCanConnectionPart(30,firstHeight,31,0,0,0,0,decompositionCubeDateList[firstPart]);
        }
        else
        {
            Debug.Log("firstPartが生成できない");
            return;
        }
        
        for(int nowPropagation=0;nowPropagation < propagationNum;nowPropagation++)//伝播の回数だけ繰り返すか伝播が終了するまで繰り返す
        {
            FindGenerateMinPartIndex();
            SelectGeneratePartInMinIndex();
            GameObject newPart = new GameObject("part"+partsListWFC.Count);
            Debug.Log("generatePart:"+generatePart);
            GenerateCube_ByWFC(decompositionCubeDateList[generatePart],canGeneratePartsInformationList[generateMinPartListIndex].x,canGeneratePartsInformationList[generateMinPartListIndex].y,canGeneratePartsInformationList[generateMinPartListIndex].z,canGeneratePartsInformationList[generateMinPartListIndex].connectionX,canGeneratePartsInformationList[generateMinPartListIndex].connectionY,canGeneratePartsInformationList[generateMinPartListIndex].connectionZ,canGeneratePartsInformationList[generateMinPartListIndex].direction,newPart);
            partsListWFC.Add(newPart);
            partsNumListWFC.Add(generatePart);
            generatePartsInformation = canGeneratePartsInformationList[generateMinPartListIndex];
            CheckCanConnectionPart(generatePartsInformation.x,generatePartsInformation.y,generatePartsInformation.z,generatePartsInformation.connectionX,generatePartsInformation.connectionY,generatePartsInformation.connectionZ,generatePartsInformation.direction,decompositionCubeDateList[generatePart]);
            RemoveGeneratePartsInformationList(generatePartsInformation);

            if(canGeneratePartsInformationList.Count == 0)
            {
                Debug.Log("リストの中身無し、伝播終了");
                break;
            }
        }
        if(canGeneratePartsInformationList.Count != 0)
        {
            Debug.Log("回数上限を超えた、伝播終了");
        }
    }

    void RemoveGeneratePartsInformationList(CanGeneratePartsInformation generateCube)//生成したパーツと被る座標の接続情報をもつパーツをリストから削除する関数
    {
        for (int index = canGeneratePartsInformationList.Count - 1; index >= 0; index--)
        {
            CanGeneratePartsInformation canGeneratePartsInformation = canGeneratePartsInformationList[index];

            switch(canGeneratePartsInformation.direction)
            {
                case 0:
                    if(cubeTypeListWFC[canGeneratePartsInformation.x,canGeneratePartsInformation.y,canGeneratePartsInformation.z-1] == 1 || cubeTypeListWFC[canGeneratePartsInformation.x,canGeneratePartsInformation.y,canGeneratePartsInformation.z-1] == 0)
                    {
                        canGeneratePartsInformationList.RemoveAt(index);
                        break;
                    }
                    else
                    {
                        break;
                    }
                case 1:
                    if(cubeTypeListWFC[canGeneratePartsInformation.x,canGeneratePartsInformation.y,canGeneratePartsInformation.z+1] == 1 || cubeTypeListWFC[canGeneratePartsInformation.x,canGeneratePartsInformation.y,canGeneratePartsInformation.z+1] == 0)
                    {
                        canGeneratePartsInformationList.RemoveAt(index);
                        break;
                    }
                    else
                    {
                        break;
                    }
                case 2:
                    if(cubeTypeListWFC[canGeneratePartsInformation.x+1,canGeneratePartsInformation.y,canGeneratePartsInformation.z] == 1 || cubeTypeListWFC[canGeneratePartsInformation.x+1,canGeneratePartsInformation.y,canGeneratePartsInformation.z] == 0)
                    {
                        canGeneratePartsInformationList.RemoveAt(index);
                        break;
                    }
                    else
                    {
                        break;
                    }
                case 3:
                    if(cubeTypeListWFC[canGeneratePartsInformation.x-1,canGeneratePartsInformation.y,canGeneratePartsInformation.z] == 1 || cubeTypeListWFC[canGeneratePartsInformation.x-1,canGeneratePartsInformation.y,canGeneratePartsInformation.z] == 0)
                    {
                        canGeneratePartsInformationList.RemoveAt(index);
                        break;
                    }
                    else
                    {
                        break;
                    }
                case 4:
                    if(cubeTypeListWFC[canGeneratePartsInformation.x,canGeneratePartsInformation.y+1,canGeneratePartsInformation.z] == 1 || cubeTypeListWFC[canGeneratePartsInformation.x,canGeneratePartsInformation.y+1,canGeneratePartsInformation.z] == 0)
                    {
                        canGeneratePartsInformationList.RemoveAt(index);
                        break;
                    }
                    else
                    {
                        break;
                    }
                case 5:
                    Debug.Log("canGeneratePartsInformation.x,canGeneratePartsInformation.y-1,canGeneratePartsInformation.z"+canGeneratePartsInformation.x+","+canGeneratePartsInformation.y+","+canGeneratePartsInformation.z);
                    Debug.Log("cubeTypeListWFC[canGeneratePartsInformation.x,canGeneratePartsInformation.y-1,canGeneratePartsInformation.z]"+cubeTypeListWFC[canGeneratePartsInformation.x,canGeneratePartsInformation.y-1,canGeneratePartsInformation.z]);
                    if(cubeTypeListWFC[canGeneratePartsInformation.x,canGeneratePartsInformation.y-1,canGeneratePartsInformation.z] == 1 || cubeTypeListWFC[canGeneratePartsInformation.x,canGeneratePartsInformation.y-1,canGeneratePartsInformation.z] == 0)
                    {
                        canGeneratePartsInformationList.RemoveAt(index);
                        break;
                    }
                    else
                    {
                        break;
                    }
            }
        }
    }

    void SelectGeneratePartInMinIndex()//最小のインデックスの中からランダムでパーツを選ぶ
    {
        int connectionPartsDistinction = Random.Range(0,generateMinPartList.Count);
        generatePart = canConnectionPartsList[connectionPartsDistinction];
        generateMinPartListIndex = generateMinPartList[connectionPartsDistinction];
        Debug.Log("generateMinPartList.Count:"+generateMinPartList.Count);
        Debug.Log("generateMinPartListIndex:"+generateMinPartListIndex);
    }

    void FindGenerateMinPartIndex()//listの中から接続可能パーツ数が最小のインデックスを探す関数
    {
        int nowConnectionPartsNum = 0;
        generateMinPartList.Clear();
        canConnectionPartsList.Clear();

        foreach(CanGeneratePartsInformation canGeneratePartsInformation in canGeneratePartsInformationList)
        {
            if(nowConnectionPartsNum == 0)
            {
                Debug.Log("nowConnectionPartsNum == 0");
                nowConnectionPartsNum = canGeneratePartsInformation.canConnnectionPartsNum;
                generateMinPartList.Add(canGeneratePartsInformationList.IndexOf(canGeneratePartsInformation));
                canConnectionPartsList.Add(canGeneratePartsInformation.connectionParts[0]);
            }

            if(canGeneratePartsInformation.canConnnectionPartsNum < nowConnectionPartsNum)
            {
                nowConnectionPartsNum = canGeneratePartsInformation.canConnnectionPartsNum;
                generateMinPartList.Clear();
                canConnectionPartsList.Clear();
                generateMinPartList.Add(canGeneratePartsInformationList.IndexOf(canGeneratePartsInformation));
                canConnectionPartsList.Add(canGeneratePartsInformation.connectionParts[0]);
            }

            if(nowConnectionPartsNum == canGeneratePartsInformation.canConnnectionPartsNum)
            {
                if(canGeneratePartsInformationList[generateMinPartList[0]].x - canGeneratePartsInformationList[generateMinPartList[0]].connectionX == canGeneratePartsInformation.x - canGeneratePartsInformation.connectionX 
                    && canGeneratePartsInformationList[generateMinPartList[0]].y - canGeneratePartsInformationList[generateMinPartList[0]].connectionY == canGeneratePartsInformation.y - canGeneratePartsInformation.connectionY
                    && canGeneratePartsInformationList[generateMinPartList[0]].z - canGeneratePartsInformationList[generateMinPartList[0]].connectionZ == canGeneratePartsInformation.z - canGeneratePartsInformation.connectionZ
                    && canGeneratePartsInformationList[generateMinPartList[0]].direction == canGeneratePartsInformation.direction)
                {
                    generateMinPartList.Add(canGeneratePartsInformationList.IndexOf(canGeneratePartsInformation));
                    canConnectionPartsList.Add(canGeneratePartsInformation.connectionParts[0]);
                }
            }
        }
    }

    int CheckGeneratePartsNum(int x,int y,int z,int connectionX,int connectionY,int connectionZ,int partConnectionImformationX,int partConnectionImformationY,int partConnectionImformationZ)//生成したパーツの周りに何個のパーツが接続可能かを確認、保存する関数(x,y,zはグローバル座標、connnectionX,Y,Zはローカル座標
    {
        int canGeneratePartsNum = 1;
        foreach(CanGeneratePartsInformation targetInfo in canGeneratePartsInformationList)
        {
            if(x-connectionX+partConnectionImformationX == targetInfo.x && y-connectionY+partConnectionImformationY == targetInfo.y && z-connectionZ+partConnectionImformationZ == targetInfo.z)
            {
                targetInfo.canConnnectionPartsNum++;
                canGeneratePartsNum++;
            }
        }
        return canGeneratePartsNum;
    }

    void CheckCanConnectionPart(int x,int y,int z,int connectionX,int connectionY,int connectionZ,int direction,DecompositionCubeDate checkpart)//生成したパーツの周りに何のパーツが接続可能かを確認、保存する関数(x,y,zはグローバル座標、connnectionX,Y,Zはローカル座標)
    {
        switch(direction)
        {
            case 0:
                z = z - 1;
                break;
            case 1:
                z = z + 1;
                break;
            case 2:
                x = x + 1;
                break;
            case 3:
                x = x - 1;
                break;
            case 4:
                y = y + 1;
                break;
            case 5:
                y = y - 1;
                break;
        }

        foreach(FrontConnectionInformation frontConnectionInformation in checkpart.frontConnectionInformation) //frontの処理
        {
            if(frontConnectionInformation.part != -1)
            {
                if(CheckCanGenerateCube(x-connectionX+frontConnectionInformation.x,y-connectionY+frontConnectionInformation.y,z-connectionZ+frontConnectionInformation.z-1,frontConnectionInformation.connectionX,frontConnectionInformation.connectionY,frontConnectionInformation.connectionZ,decompositionCubeDateList[frontConnectionInformation.part]))
                {
                    CanGeneratePartsInformation canGeneratePartsInformation = new CanGeneratePartsInformation();
                    canGeneratePartsInformation.x = x-connectionX+frontConnectionInformation.x;
                    canGeneratePartsInformation.y = y-connectionY+frontConnectionInformation.y;
                    canGeneratePartsInformation.z = z-connectionZ+frontConnectionInformation.z;
                    canGeneratePartsInformation.connectionX = frontConnectionInformation.connectionX;
                    canGeneratePartsInformation.connectionY = frontConnectionInformation.connectionY;
                    canGeneratePartsInformation.connectionZ = frontConnectionInformation.connectionZ;
                    canGeneratePartsInformation.canConnnectionPartsNum = CheckGeneratePartsNum(x,y,z,connectionX,connectionY,connectionZ,frontConnectionInformation.x,frontConnectionInformation.y,frontConnectionInformation.z);
                    canGeneratePartsInformation.direction = 0;
                    canGeneratePartsInformation.connectionParts.Add(frontConnectionInformation.part);
                    canGeneratePartsInformationList.Add(canGeneratePartsInformation);
                }
            }
        }

        foreach(BackConnectionInformation backConnectionInformation in checkpart.backConnectionInformation) //backの処理
        {
            if(backConnectionInformation.part != -1)
            {
                Debug.Log("backConnectionInformation.part:"+backConnectionInformation.part);
                if(CheckCanGenerateCube(x-connectionX+backConnectionInformation.x,y-connectionY+backConnectionInformation.y,z-connectionZ+backConnectionInformation.z+1,backConnectionInformation.connectionX,backConnectionInformation.connectionY,backConnectionInformation.connectionZ,decompositionCubeDateList[backConnectionInformation.part]))
                {
                    CanGeneratePartsInformation canGeneratePartsInformation = new CanGeneratePartsInformation();
                    canGeneratePartsInformation.x = x-connectionX+backConnectionInformation.x;
                    canGeneratePartsInformation.y = y-connectionY+backConnectionInformation.y;
                    canGeneratePartsInformation.z = z-connectionZ+backConnectionInformation.z;
                    canGeneratePartsInformation.connectionX = backConnectionInformation.connectionX;
                    canGeneratePartsInformation.connectionY = backConnectionInformation.connectionY;
                    canGeneratePartsInformation.connectionZ = backConnectionInformation.connectionZ;
                    canGeneratePartsInformation.canConnnectionPartsNum = CheckGeneratePartsNum(x,y,z,connectionX,connectionY,connectionZ,backConnectionInformation.x,backConnectionInformation.y,backConnectionInformation.z);
                    canGeneratePartsInformation.direction = 1;
                    canGeneratePartsInformation.connectionParts.Add(backConnectionInformation.part);
                    canGeneratePartsInformationList.Add(canGeneratePartsInformation);
                }
            }
        }

        foreach(RightConnectionInformation rightConnectionInformation in checkpart.rightConnectionInformation) //rightの処理
        {
            if(rightConnectionInformation.part != -1)
            {
                Debug.Log("rightConnectionInformation.part:"+rightConnectionInformation.part);
                if(CheckCanGenerateCube(x-connectionX+rightConnectionInformation.x+1,y-connectionY+rightConnectionInformation.y,z-connectionZ+rightConnectionInformation.z,rightConnectionInformation.connectionX,rightConnectionInformation.connectionY,rightConnectionInformation.connectionZ,decompositionCubeDateList[rightConnectionInformation.part]))
                {
                    CanGeneratePartsInformation canGeneratePartsInformation = new CanGeneratePartsInformation();
                    canGeneratePartsInformation.x = x-connectionX+rightConnectionInformation.x;
                    canGeneratePartsInformation.y = y-connectionY+rightConnectionInformation.y;
                    canGeneratePartsInformation.z = z-connectionZ+rightConnectionInformation.z;
                    canGeneratePartsInformation.connectionX = rightConnectionInformation.connectionX;
                    canGeneratePartsInformation.connectionY = rightConnectionInformation.connectionY;
                    canGeneratePartsInformation.connectionZ = rightConnectionInformation.connectionZ;
                    canGeneratePartsInformation.canConnnectionPartsNum = CheckGeneratePartsNum(x,y,z,connectionX,connectionY,connectionZ,rightConnectionInformation.x,rightConnectionInformation.y,rightConnectionInformation.z);
                    canGeneratePartsInformation.direction = 2;
                    canGeneratePartsInformation.connectionParts.Add(rightConnectionInformation.part);
                    canGeneratePartsInformationList.Add(canGeneratePartsInformation);
                }
            }
        }

        foreach(LeftConnectionInformation leftConnectionInformation in checkpart.leftConnectionInformation) //leftの処理
        {
            if(leftConnectionInformation.part != -1)
            {
                Debug.Log("leftConnectionInformation.part:"+leftConnectionInformation.part);
                if(CheckCanGenerateCube(x-connectionX+leftConnectionInformation.x-1,y-connectionY+leftConnectionInformation.y,z-connectionZ+leftConnectionInformation.z,leftConnectionInformation.connectionX,leftConnectionInformation.connectionY,leftConnectionInformation.connectionZ,decompositionCubeDateList[leftConnectionInformation.part]))
                {
                    CanGeneratePartsInformation canGeneratePartsInformation = new CanGeneratePartsInformation();
                    canGeneratePartsInformation.x = x-connectionX+leftConnectionInformation.x;
                    canGeneratePartsInformation.y = y-connectionY+leftConnectionInformation.y;
                    canGeneratePartsInformation.z = z-connectionZ+leftConnectionInformation.z;
                    canGeneratePartsInformation.connectionX = leftConnectionInformation.connectionX;
                    canGeneratePartsInformation.connectionY = leftConnectionInformation.connectionY;
                    canGeneratePartsInformation.connectionZ = leftConnectionInformation.connectionZ;
                    canGeneratePartsInformation.canConnnectionPartsNum = CheckGeneratePartsNum(x,y,z,connectionX,connectionY,connectionZ,leftConnectionInformation.x,leftConnectionInformation.y,leftConnectionInformation.z);
                    canGeneratePartsInformation.direction = 3;
                    canGeneratePartsInformation.connectionParts.Add(leftConnectionInformation.part);
                    canGeneratePartsInformationList.Add(canGeneratePartsInformation);
                }
            }
        }

        foreach(UpConnectionInformation upConnectionInformation in checkpart.upConnectionInformation) //upの処理
        {
            if(upConnectionInformation.part != -1)
            {
                Debug.Log("upConnectionInformation.part:"+upConnectionInformation.part);
                if(CheckCanGenerateCube(x-connectionX+upConnectionInformation.x,y-connectionY+upConnectionInformation.y+1,z-connectionZ+upConnectionInformation.z,upConnectionInformation.connectionX,upConnectionInformation.connectionY,upConnectionInformation.connectionZ,decompositionCubeDateList[upConnectionInformation.part]))
                {
                    CanGeneratePartsInformation canGeneratePartsInformation = new CanGeneratePartsInformation();
                    canGeneratePartsInformation.x = x-connectionX+upConnectionInformation.x;
                    canGeneratePartsInformation.y = y-connectionY+upConnectionInformation.y;
                    canGeneratePartsInformation.z = z-connectionZ+upConnectionInformation.z;
                    canGeneratePartsInformation.connectionX = upConnectionInformation.connectionX;
                    canGeneratePartsInformation.connectionY = upConnectionInformation.connectionY;
                    canGeneratePartsInformation.connectionZ = upConnectionInformation.connectionZ;
                    canGeneratePartsInformation.canConnnectionPartsNum = CheckGeneratePartsNum(x,y,z,connectionX,connectionY,connectionZ,upConnectionInformation.x,upConnectionInformation.y,upConnectionInformation.z);
                    canGeneratePartsInformation.direction = 4;
                    canGeneratePartsInformation.connectionParts.Add(upConnectionInformation.part);
                    canGeneratePartsInformationList.Add(canGeneratePartsInformation);
                }
            }
        }

        foreach(DownConnectionInformation downConnectionInformation in checkpart.downConnectionInformation) //downの処理
        {
            if(y-connectionY+downConnectionInformation.y != 0)
            {
                if(downConnectionInformation.part != -2 && downConnectionInformation.part != -1)
                {
                    Debug.Log("downConnectionInformation.part:"+downConnectionInformation.part);
                    if(CheckCanGenerateCube(x-connectionX+downConnectionInformation.x,y-connectionY+downConnectionInformation.y-1,z-connectionZ+downConnectionInformation.z,downConnectionInformation.connectionX,downConnectionInformation.connectionY,downConnectionInformation.connectionZ,decompositionCubeDateList[downConnectionInformation.part]))
                    {
                        CanGeneratePartsInformation canGeneratePartsInformation = new CanGeneratePartsInformation();
                        canGeneratePartsInformation.x = x-connectionX+downConnectionInformation.x;
                        canGeneratePartsInformation.y = y-connectionY+downConnectionInformation.y;
                        canGeneratePartsInformation.z = z-connectionZ+downConnectionInformation.z;
                        canGeneratePartsInformation.connectionX = downConnectionInformation.connectionX;
                        canGeneratePartsInformation.connectionY = downConnectionInformation.connectionY;
                        canGeneratePartsInformation.connectionZ = downConnectionInformation.connectionZ;
                        canGeneratePartsInformation.canConnnectionPartsNum = CheckGeneratePartsNum(x,y,z,connectionX,connectionY,connectionZ,downConnectionInformation.x,downConnectionInformation.y,downConnectionInformation.z);
                        canGeneratePartsInformation.direction = 5;
                        canGeneratePartsInformation.connectionParts.Add(downConnectionInformation.part);
                        canGeneratePartsInformationList.Add(canGeneratePartsInformation);
                    }
                }
            }
        }
    }

    bool CheckCanGenerateCube(int x,int y,int z,int connectionX,int connectionY,int connectionZ,DecompositionCubeDate decompositionCubeDate) //Cubeを生成できるかを確認する関数
    {
        bool canGenerateCube = true;
        bool canConnection = false;

        if(isFirstGenerate == false)
        {
            Debug.Log("isFirstGenerateCheck");
            isFirstGenerate = true;
            
            foreach(DownConnectionInformation downConnectionInformation in decompositionCubeDate.downConnectionInformation)
            {
                if(downConnectionInformation.part != -2 && y+downConnectionInformation.y-connectionY == 0)
                {
                    canGenerateCube = false;
                    Debug.Log("canGenerateCube = false");
                    return canGenerateCube;
                }

                if(downConnectionInformation.part == -2 && firstHeight != 0)
                {
                    Debug.Log("firstHeight:0");
                    firstHeight = 0;
                }
            }

            return canGenerateCube;
        }
        
        if(y-connectionY < 0)
        {
            canGenerateCube = false;
            Debug.Log("canGenerateCube = false");
            return canGenerateCube;
        }

        for(int k=0; k < N; k++)
        {
            for(int j=0; j < N; j++)
            {
                for(int i=0; i < N; i++)
                {
                    if(decompositionCubeDate.decompositionCubeArray[i,j,k] == 1 && cubeTypeListWFC[x+i-connectionX,y+j-connectionY,z+k-connectionZ] == 1)
                    {
                        canGenerateCube = false;
                        Debug.Log("canGenerateCube = false");
                        return canGenerateCube;
                    }
                    else if(decompositionCubeDate.decompositionCubeArray[i,j,k] == 0)
                    {
                        if(cubeTypeListWFC[x+i-connectionX,y+j-connectionY,z+k-connectionZ] == 1)
                        {
                            canGenerateCube = false;
                            Debug.Log("canGenerateCube = false");
                            return canGenerateCube;
                        }
                    }
                }
            }
        }
        //接続okな部品があるならtrueを返すようにする
        canConnection = false;
        foreach(FrontConnectionInformation frontConnectionInformation in decompositionCubeDate.frontConnectionInformation)
        {
            if(frontConnectionInformation.part == -1)
            {
                if(cubeTypeListWFC[x+frontConnectionInformation.x-connectionX,y+frontConnectionInformation.y-connectionY,z+frontConnectionInformation.z-connectionZ-1] == 1)
                {
                    canGenerateCube = false;
                    Debug.Log("canGenerateCube = false");
                    return canGenerateCube;
                }
            }
            else
            {
                if(cubeTypeListWFC[x+frontConnectionInformation.x-connectionX,y+frontConnectionInformation.y-connectionY,z+frontConnectionInformation.z-connectionZ-1] == 1)
                {
                    if(frontConnectionInformation.part != partsNumListWFC[partsListWFC.IndexOf(cubesWFC[cubeNumListWFC[x+frontConnectionInformation.x-connectionX,y+frontConnectionInformation.y-connectionY,z+frontConnectionInformation.z-connectionZ-1]].transform.parent.gameObject)])
                    {
                        canGenerateCube = false;
                    }

                    if(frontConnectionInformation.part == partsNumListWFC[partsListWFC.IndexOf(cubesWFC[cubeNumListWFC[x+frontConnectionInformation.x-connectionX,y+frontConnectionInformation.y-connectionY,z+frontConnectionInformation.z-connectionZ-1]].transform.parent.gameObject)])
                    {
                        canConnection = true;
                    }
                }
            }
        }

        if(canGenerateCube == false && canConnection == false)
        {
            Debug.Log("canGenerateCube = false");
            return canGenerateCube;
        }
        canConnection = false;
        canGenerateCube = true;

        foreach(BackConnectionInformation backConnectionInformation in decompositionCubeDate.backConnectionInformation)
        {
            if(backConnectionInformation.part == -1)
            {
                if(cubeTypeListWFC[x+backConnectionInformation.x-connectionX,y+backConnectionInformation.y-connectionY,z+backConnectionInformation.z-connectionZ+1] == 1)
                {
                    canGenerateCube = false;
                    Debug.Log("canGenerateCube = false");
                    return canGenerateCube;
                }
            }
            else
            {
                if(cubeTypeListWFC[x+backConnectionInformation.x-connectionX,y+backConnectionInformation.y-connectionY,z+backConnectionInformation.z-connectionZ+1] == 1)
                {
                    if(backConnectionInformation.part != partsNumListWFC[partsListWFC.IndexOf(cubesWFC[cubeNumListWFC[x+backConnectionInformation.x-connectionX,y+backConnectionInformation.y-connectionY,z+backConnectionInformation.z-connectionZ+1]].transform.parent.gameObject)])
                    {
                        canGenerateCube = false;
                    }

                    if(backConnectionInformation.part == partsNumListWFC[partsListWFC.IndexOf(cubesWFC[cubeNumListWFC[x+backConnectionInformation.x-connectionX,y+backConnectionInformation.y-connectionY,z+backConnectionInformation.z-connectionZ+1]].transform.parent.gameObject)])
                    {
                        canConnection = true;
                    }
                }
            }
        }

        if(canGenerateCube == false && canConnection == false)
        {
            Debug.Log("canGenerateCube = false");
            return canGenerateCube;
        }
        canConnection = false;
        canGenerateCube = true;

        foreach(RightConnectionInformation rightConnectionInformation in decompositionCubeDate.rightConnectionInformation)
        {
            if(rightConnectionInformation.part == -1)
            {
                if(cubeTypeListWFC[x+rightConnectionInformation.x-connectionX+1,y+rightConnectionInformation.y-connectionY,z+rightConnectionInformation.z-connectionZ] == 1)
                {
                    canGenerateCube = false;
                    Debug.Log("canGenerateCube = false");
                    return canGenerateCube;
                }
            }
            else
            {
                if(cubeTypeListWFC[x+rightConnectionInformation.x-connectionX+1,y+rightConnectionInformation.y-connectionY,z+rightConnectionInformation.z-connectionZ] == 1)
                {
                    if(rightConnectionInformation.part != partsNumListWFC[partsListWFC.IndexOf(cubesWFC[cubeNumListWFC[x+rightConnectionInformation.x-connectionX+1,y+rightConnectionInformation.y-connectionY,z+rightConnectionInformation.z-connectionZ]].transform.parent.gameObject)])
                    {
                        canGenerateCube = false;
                    }

                    if(rightConnectionInformation.part == partsNumListWFC[partsListWFC.IndexOf(cubesWFC[cubeNumListWFC[x+rightConnectionInformation.x-connectionX+1,y+rightConnectionInformation.y-connectionY,z+rightConnectionInformation.z-connectionZ]].transform.parent.gameObject)])
                    {
                        canConnection = true;
                    }
                }
            }
        }

        if(canGenerateCube == false && canConnection == false)
        {
            Debug.Log("canGenerateCube = false");
            return canGenerateCube;
        }
        canConnection = false;
        canGenerateCube = true;

        foreach(LeftConnectionInformation leftConnectionInformation in decompositionCubeDate.leftConnectionInformation)
        {
            if(leftConnectionInformation.part == -1)
            {
                if(cubeTypeListWFC[x+leftConnectionInformation.x-connectionX-1,y+leftConnectionInformation.y-connectionY,z+leftConnectionInformation.z-connectionZ] == 1)
                {
                    canGenerateCube = false;
                    Debug.Log("canGenerateCube = false");
                    return canGenerateCube;
                }
            }
            else
            {
                if(cubeTypeListWFC[x+leftConnectionInformation.x-connectionX-1,y+leftConnectionInformation.y-connectionY,z+leftConnectionInformation.z-connectionZ] == 1)
                {
                    if(leftConnectionInformation.part != partsNumListWFC[partsListWFC.IndexOf(cubesWFC[cubeNumListWFC[x+leftConnectionInformation.x-connectionX-1,y+leftConnectionInformation.y-connectionY,z+leftConnectionInformation.z-connectionZ]].transform.parent.gameObject)])
                    {
                        canGenerateCube = false;
                    }

                    if(leftConnectionInformation.part == partsNumListWFC[partsListWFC.IndexOf(cubesWFC[cubeNumListWFC[x+leftConnectionInformation.x-connectionX-1,y+leftConnectionInformation.y-connectionY,z+leftConnectionInformation.z-connectionZ]].transform.parent.gameObject)])
                    {
                        canConnection = true;
                    }
                }
            }
        }

        if(canGenerateCube == false && canConnection == false)
        {
            Debug.Log("canGenerateCube = false");
            return canGenerateCube;
        }
        canConnection = false;
        canGenerateCube = true;

        foreach(UpConnectionInformation upConnectionInformation in decompositionCubeDate.upConnectionInformation)
        {
            if(y+upConnectionInformation.y-connectionY+N > maxHeight)
            {
                canGenerateCube = false;
                Debug.Log("canGenerateCube = false");
                return canGenerateCube;
            }

            if(upConnectionInformation.part == -1)
            {
                if(cubeTypeListWFC[x+upConnectionInformation.x-connectionX,y+upConnectionInformation.y-connectionY+1,z+upConnectionInformation.z-connectionZ] == 1)
                {
                    canGenerateCube = false;
                    Debug.Log("canGenerateCube = false");
                    return canGenerateCube;
                }
            }
            else
            {
                if(cubeTypeListWFC[x+upConnectionInformation.x-connectionX,y+upConnectionInformation.y-connectionY+1,z+upConnectionInformation.z-connectionZ] == 1)
                {
                    if(upConnectionInformation.part != partsNumListWFC[partsListWFC.IndexOf(cubesWFC[cubeNumListWFC[x+upConnectionInformation.x-connectionX,y+upConnectionInformation.y-connectionY+1,z+upConnectionInformation.z-connectionZ]].transform.parent.gameObject)])
                    {
                        canGenerateCube = false;
                    }

                    if(upConnectionInformation.part == partsNumListWFC[partsListWFC.IndexOf(cubesWFC[cubeNumListWFC[x+upConnectionInformation.x-connectionX,y+upConnectionInformation.y-connectionY+1,z+upConnectionInformation.z-connectionZ]].transform.parent.gameObject)])
                    {
                        canConnection = true;
                    }
                }
            }
        }

        if(canGenerateCube == false && canConnection == false)
        {
            Debug.Log("canGenerateCube = false");
            return canGenerateCube;
        }
        canConnection = false;
        canGenerateCube = true;

        foreach(DownConnectionInformation downConnectionInformation in decompositionCubeDate.downConnectionInformation)
        {
            if(downConnectionInformation.part == -2)
            {
                if(y+downConnectionInformation.y-connectionY != 0)
                {
                    canGenerateCube = false;
                    Debug.Log("canGenerateCube = false");
                    return canGenerateCube;
                }
            }
            else if(y+downConnectionInformation.y-connectionY == 0 && downConnectionInformation.part != -2)
            {
                canGenerateCube = false;
                Debug.Log("canGenerateCube = false");
                return canGenerateCube;
            }
            else if(downConnectionInformation.part == -1)
            {
                if(cubeTypeListWFC[x+downConnectionInformation.x-connectionX,y+downConnectionInformation.y-connectionY-1,z+downConnectionInformation.z-connectionZ] == 1)
                {
                    canGenerateCube = false;
                    Debug.Log("canGenerateCube = false");
                    return canGenerateCube;
                }
            }
            else
            {
                if(cubeTypeListWFC[x+downConnectionInformation.x-connectionX,y+downConnectionInformation.y-connectionY-1,z+downConnectionInformation.z-connectionZ] == 1)
                {
                    if(downConnectionInformation.part != partsNumListWFC[partsListWFC.IndexOf(cubesWFC[cubeNumListWFC[x+downConnectionInformation.x-connectionX,y+downConnectionInformation.y-connectionY-1,z+downConnectionInformation.z-connectionZ]].transform.parent.gameObject)])
                    {
                        canGenerateCube = false;
                    }

                    if(downConnectionInformation.part == partsNumListWFC[partsListWFC.IndexOf(cubesWFC[cubeNumListWFC[x+downConnectionInformation.x-connectionX,y+downConnectionInformation.y-connectionY-1,z+downConnectionInformation.z-connectionZ]].transform.parent.gameObject)])
                    {
                        canConnection = true;
                    }
                }
            }
        }

        if(canGenerateCube == false && canConnection == false)
        {
            Debug.Log("canGenerateCube = false");
            return canGenerateCube;
        }
        canConnection = false;
        canGenerateCube = true;

        return true;
    }

    void GenerateCube_ByWFC(DecompositionCubeDate decompositionCubeDate,int x,int y,int z,int connectionX,int connectionY,int connectionZ,int direction,GameObject part)//WFCでパーツを生成する関数(x,y,zが生成を行う始点の座標、connectionx,y,zがその部品と接続する部品のローカル座標)
    {
        x = x - connectionX;
        y = y - connectionY;
        z = z - connectionZ;

        switch(direction)
        {
            case 0:
                z = z - 1;
                break;
            case 1:
                z = z + 1;
                break;
            case 2:
                x = x + 1;
                break;
            case 3:
                x = x - 1;
                break;
            case 4:
                y = y + 1;
                break;
            case 5:
                y = y - 1;
                break;
        }

        for(int k=0; k < N; k++)
        {
            for(int j=0; j < N; j++)
            {
                for(int i=0; i < N; i++)
                {
                    if(decompositionCubeDate.decompositionCubeArray[i,j,k] == 1)
                    {
                        GameObject cube = Instantiate(cubePrefab,new Vector3(x+i,y+j,z+k),Quaternion.identity);
                        cube.GetComponent<Renderer>().material.color = decompositionCubeDate.color;
                        cube.transform.parent = part.transform;
                        cubesWFC.Add(cube);
                        cubeNumListWFC[x+i,y+j,z+k] = cubeNumWFC;
                        cubeTypeListWFC[x+i,y+j,z+k] = 1;
                        cubeNumWFC++;
                    }
                    else if(decompositionCubeDate.decompositionCubeArray[i,j,k] == 0)
                    {
                        GameObject air = new GameObject("air");
                        air.transform.parent = part.transform;
                        air.transform.position = new Vector3(x+i,y+j,z+k);
                        cubesWFC.Add(air);
                        cubeNumListWFC[x+i,y+j,z+k] = cubeNumWFC;
                        cubeTypeListWFC[x+i,y+j,z+k] = 0;
                        cubeNumWFC++;
                    }
                    else
                    {
                        GameObject air = new GameObject("ExceptionAir");
                        air.transform.parent = part.transform;
                        air.transform.position = new Vector3(x+i,y+j,z+k);
                        cubesWFC.Add(air);
                        cubeNumListWFC[x+i,y+j,z+k] = cubeNumWFC;
                        cubeNumWFC++;
                    }
                }
            }
        }  
    }

    void CaliculateConnectionCubePos(ref int connectionCubePos_x,ref int connectionCubePos_y,ref int connectionCubePos_z,int connectionCubeNumPos, int N)
    {
        connectionCubePos_x = connectionCubeNumPos % N;
        connectionCubePos_y = connectionCubeNumPos / (N * N);
        connectionCubePos_z = (connectionCubeNumPos - connectionCubePos_x - connectionCubePos_y * N * N) / N;
    }

    // 親オブジェクト(Transform)と子オブジェクト(GameObject)を受け取り、子オブジェクトが親オブジェクトの中で何番目にあるかを返す関数
    int GetChildIndex(Transform parentTransform, GameObject childObject)
    {
        if (parentTransform != null && childObject != null)
        {
            if (parentTransform.childCount > 0)
            {
                for (int i = 0; i < parentTransform.childCount; i++)
                {
                    if (parentTransform.GetChild(i).gameObject == childObject)
                    {
                        return i;
                    }
                }
            }
        }

        // 見つからない場合は -1 を返す
        return -1;
    }

    void ConnectionInformationSaving()//接続情報を保存する関数
    {
        foreach(GameObject parts in partsList)
        {
            int connectionCubePosNum = 0;
            int nowX = 0;
            int nowY = 0;
            int nowZ = 0;
            List<Color> colorList = new List<Color>();
            int connectionNum = 0;
            List<int> connectionNumList = new List<int>();
            DecompositionCubeDate nowDecompositionCubeDate = decompositionCubeDateList[partsList.IndexOf(parts)];
            
            for(nowY=0;nowY < N;nowY++) //forntの処理
            {
                nowZ = 0;
                for(nowX=0;nowX < N;nowX++)
                {
                    if(nowDecompositionCubeDate.decompositionCubeArray[nowX,nowY,nowZ] == 0 || nowDecompositionCubeDate.decompositionCubeArray[nowX,nowY,nowZ] == 1)
                    {
                        GameObject partsCube = parts.transform.GetChild(nowX+nowZ*N+nowY*(N*N)).gameObject;
                        FrontConnectionInformation frontConnectionInformation = new FrontConnectionInformation();
                        frontConnectionInformation.x = nowX;
                        frontConnectionInformation.y = nowY;
                        frontConnectionInformation.z = nowZ;

                        if(cubeBoolList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z-1])
                        {
                            frontConnectionInformation.part = partsList.IndexOf(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z-1]].transform.parent.gameObject);
                            nowDecompositionCubeDate.canConnectionNum++;

                            connectionCubePosNum = GetChildIndex(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z-1]].transform.parent,cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z-1]]);
                            CaliculateConnectionCubePos(ref frontConnectionInformation.connectionX,ref frontConnectionInformation.connectionY,ref frontConnectionInformation.connectionZ,connectionCubePosNum,N);

                            colorList.Add(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z-1]].GetComponent<Renderer>().material.color);
                            connectionNumList.Add(connectionNum);
                            connectionNum++;

                            nowDecompositionCubeDate.frontConnectionInformation.Add(frontConnectionInformation);
                        }
                        else
                        {
                            frontConnectionInformation.part = -1;
                            nowDecompositionCubeDate.frontConnectionInformation.Add(frontConnectionInformation);
                        }
                    }
                    else
                    {
                        for(int i=1; i < N; i++)
                        {
                            if(nowDecompositionCubeDate.decompositionCubeArray[nowX,nowY,nowZ+i] == 0 || nowDecompositionCubeDate.decompositionCubeArray[nowX,nowY,nowZ+i] == 1)
                            {
                                GameObject partsCube = parts.transform.GetChild(nowX+(nowZ+i)*N+nowY*(N*N)).gameObject;
                                FrontConnectionInformation frontConnectionInformation = new FrontConnectionInformation();
                                frontConnectionInformation.x = nowX;
                                frontConnectionInformation.y = nowY;
                                frontConnectionInformation.z = nowZ+i;

                                if(cubeBoolList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z-1])
                                {
                                    frontConnectionInformation.part = partsList.IndexOf(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z-1]].transform.parent.gameObject);
                                    nowDecompositionCubeDate.canConnectionNum++;

                                    connectionCubePosNum = GetChildIndex(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z-1]].transform.parent,cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z-1]]);
                                    CaliculateConnectionCubePos(ref frontConnectionInformation.connectionX,ref frontConnectionInformation.connectionY,ref frontConnectionInformation.connectionZ,connectionCubePosNum,N);

                                    colorList.Add(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z-1]].GetComponent<Renderer>().material.color);
                                    connectionNumList.Add(connectionNum);
                                    connectionNum++;
                                    nowDecompositionCubeDate.frontConnectionInformation.Add(frontConnectionInformation);
                                    break;
                                }
                                else
                                {
                                    frontConnectionInformation.part = -1;
                                    nowDecompositionCubeDate.frontConnectionInformation.Add(frontConnectionInformation);
                                    break;
                                }
                            }
                        }
                    }
                }
                nowX = 0;
            }

            for(nowY=0;nowY < N;nowY++) //backの処理
            {
                nowZ = N-1;
                for(nowX=0;nowX < N;nowX++)
                {
                    if(nowDecompositionCubeDate.decompositionCubeArray[nowX,nowY,nowZ] == 0 || nowDecompositionCubeDate.decompositionCubeArray[nowX,nowY,nowZ] == 1)
                    {
                        GameObject partsCube = parts.transform.GetChild(nowX+nowZ*N+nowY*(N*N)).gameObject;
                        BackConnectionInformation backConnectionInformation = new BackConnectionInformation();
                        backConnectionInformation.x = nowX;
                        backConnectionInformation.y = nowY;
                        backConnectionInformation.z = nowZ;

                        if(cubeBoolList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z+1])
                        {
                            backConnectionInformation.part = partsList.IndexOf(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z+1]].transform.parent.gameObject);
                            nowDecompositionCubeDate.canConnectionNum++;

                            connectionCubePosNum = GetChildIndex(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z+1]].transform.parent,cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z+1]]);
                            CaliculateConnectionCubePos(ref backConnectionInformation.connectionX,ref backConnectionInformation.connectionY,ref backConnectionInformation.connectionZ,connectionCubePosNum,N);

                            nowDecompositionCubeDate.backConnectionInformation.Add(backConnectionInformation);
                            for(int i = 0; i < colorList.Count; i++)
                            {
                                if(colorList[i] == cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z+1]].GetComponent<Renderer>().material.color && nowDecompositionCubeDate.color == cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z+1]].GetComponent<Renderer>().material.color)
                                {
                                    if(nowX == decompositionCubeDateList[partsList.IndexOf(parts)].frontConnectionInformation[connectionNumList[i]].x && nowY == decompositionCubeDateList[partsList.IndexOf(parts)].frontConnectionInformation[connectionNumList[i]].y)
                                    {
                                        FrontConnectionInformation frontReproductionConnectionInformation = new FrontConnectionInformation();
                                        frontReproductionConnectionInformation.x = decompositionCubeDateList[partsList.IndexOf(parts)].frontConnectionInformation[connectionNumList[i]].x;
                                        frontReproductionConnectionInformation.y = decompositionCubeDateList[partsList.IndexOf(parts)].frontConnectionInformation[connectionNumList[i]].y;
                                        frontReproductionConnectionInformation.z = decompositionCubeDateList[partsList.IndexOf(parts)].frontConnectionInformation[connectionNumList[i]].z;
                                        frontReproductionConnectionInformation.part = partsList.IndexOf(parts);
                                        frontReproductionConnectionInformation.connectionX = decompositionCubeDateList[partsList.IndexOf(parts)].frontConnectionInformation[connectionNumList[i]].x;
                                        frontReproductionConnectionInformation.connectionY = decompositionCubeDateList[partsList.IndexOf(parts)].frontConnectionInformation[connectionNumList[i]].y;
                                        frontReproductionConnectionInformation.connectionZ = decompositionCubeDateList[partsList.IndexOf(parts)].frontConnectionInformation[connectionNumList[i]].z+(N-1);
                                        nowDecompositionCubeDate.frontConnectionInformation.Add(frontReproductionConnectionInformation);
                                        nowDecompositionCubeDate.canConnectionNum++;

                                        BackConnectionInformation backReproductionConnectionInformation = new BackConnectionInformation();
                                        backReproductionConnectionInformation.x = decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation.Count - 1].x;
                                        backReproductionConnectionInformation.y = decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation.Count - 1].y;
                                        backReproductionConnectionInformation.z = decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation.Count - 1].z;
                                        backReproductionConnectionInformation.part = partsList.IndexOf(parts);
                                        backReproductionConnectionInformation.connectionX = decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation.Count - 1].x;
                                        backReproductionConnectionInformation.connectionY = decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation.Count - 1].y;
                                        backReproductionConnectionInformation.connectionZ = decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation.Count - 1].z-(N-1);
                                        nowDecompositionCubeDate.backConnectionInformation.Add(backReproductionConnectionInformation);
                                        nowDecompositionCubeDate.canConnectionNum++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            backConnectionInformation.part = -1;
                            nowDecompositionCubeDate.backConnectionInformation.Add(backConnectionInformation);
                        }
                    }
                    else
                    {
                        for(int i=1; i < N; i++)
                        {
                            if(nowDecompositionCubeDate.decompositionCubeArray[nowX,nowY,nowZ-i] == 0 || nowDecompositionCubeDate.decompositionCubeArray[nowX,nowY,nowZ-i] == 1)
                            {
                                GameObject partsCube = parts.transform.GetChild(nowX+(nowZ-i)*N+nowY*(N*N)).gameObject;
                                BackConnectionInformation backConnectionInformation = new BackConnectionInformation();
                                backConnectionInformation.x = nowX;
                                backConnectionInformation.y = nowY;
                                backConnectionInformation.z = nowZ-i;

                                if(cubeBoolList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z+1])
                                {
                                    backConnectionInformation.part = partsList.IndexOf(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z+1]].transform.parent.gameObject);
                                    nowDecompositionCubeDate.canConnectionNum++;

                                    connectionCubePosNum = GetChildIndex(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z+1]].transform.parent,cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z+1]]);
                                    CaliculateConnectionCubePos(ref backConnectionInformation.connectionX,ref backConnectionInformation.connectionY,ref backConnectionInformation.connectionZ,connectionCubePosNum,N);

                                    nowDecompositionCubeDate.backConnectionInformation.Add(backConnectionInformation);
                                    for(int j = 0; j < colorList.Count; j++)
                                    {
                                        if(colorList[j] == cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z+1]].GetComponent<Renderer>().material.color && nowDecompositionCubeDate.color == cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z+1]].GetComponent<Renderer>().material.color)
                                        {
                                            if(nowX == decompositionCubeDateList[partsList.IndexOf(parts)].frontConnectionInformation[connectionNumList[j]].x && nowY == decompositionCubeDateList[partsList.IndexOf(parts)].frontConnectionInformation[connectionNumList[j]].y)
                                            {
                                                FrontConnectionInformation frontReproductionConnectionInformation = new FrontConnectionInformation();
                                                frontReproductionConnectionInformation.x = decompositionCubeDateList[partsList.IndexOf(parts)].frontConnectionInformation[connectionNumList[j]].x;
                                                frontReproductionConnectionInformation.y = decompositionCubeDateList[partsList.IndexOf(parts)].frontConnectionInformation[connectionNumList[j]].y;
                                                frontReproductionConnectionInformation.z = decompositionCubeDateList[partsList.IndexOf(parts)].frontConnectionInformation[connectionNumList[j]].z;
                                                frontReproductionConnectionInformation.part = partsList.IndexOf(parts);
                                                frontReproductionConnectionInformation.connectionX = decompositionCubeDateList[partsList.IndexOf(parts)].frontConnectionInformation[connectionNumList[j]].x;
                                                frontReproductionConnectionInformation.connectionY = decompositionCubeDateList[partsList.IndexOf(parts)].frontConnectionInformation[connectionNumList[j]].y;
                                                frontReproductionConnectionInformation.connectionZ = decompositionCubeDateList[partsList.IndexOf(parts)].frontConnectionInformation[connectionNumList[j]].z+(N-1);
                                                nowDecompositionCubeDate.frontConnectionInformation.Add(frontReproductionConnectionInformation);
                                                nowDecompositionCubeDate.canConnectionNum++;

                                                BackConnectionInformation backReproductionConnectionInformation = new BackConnectionInformation();
                                                backReproductionConnectionInformation.x = decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation.Count - 1].x;
                                                backReproductionConnectionInformation.y = decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation.Count - 1].y;
                                                backReproductionConnectionInformation.z = decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation.Count - 1].z;
                                                backReproductionConnectionInformation.part = partsList.IndexOf(parts);
                                                backReproductionConnectionInformation.connectionX = decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation.Count - 1].x;
                                                backReproductionConnectionInformation.connectionY = decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation.Count - 1].y;
                                                backReproductionConnectionInformation.connectionZ = decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].backConnectionInformation.Count - 1].z-(N-1);
                                                nowDecompositionCubeDate.backConnectionInformation.Add(backReproductionConnectionInformation);
                                                nowDecompositionCubeDate.canConnectionNum++;
                                            }
                                        }
                                    }
                                    break;
                                }
                                else
                                {
                                    backConnectionInformation.part = -1;
                                    nowDecompositionCubeDate.backConnectionInformation.Add(backConnectionInformation);
                                    break;
                                }
                            }
                        }
                    }
                }
                nowX = 0;
            }
            colorList.Clear();
            connectionNumList.Clear();
            connectionNum = 0;

            for(nowY=0; nowY < N; nowY++)//Rightの処理
            {
                nowX = N-1;
                for(nowZ=0; nowZ < N; nowZ++)
                {
                    if(nowDecompositionCubeDate.decompositionCubeArray[nowX,nowY,nowZ] == 0 || nowDecompositionCubeDate.decompositionCubeArray[nowX,nowY,nowZ] == 1)
                    {
                        GameObject partsCube = parts.transform.GetChild(nowX+nowZ*N+nowY*(N*N)).gameObject;
                        RightConnectionInformation rightConnectionInformation = new RightConnectionInformation();
                        rightConnectionInformation.x = nowX;
                        rightConnectionInformation.y = nowY;
                        rightConnectionInformation.z = nowZ;

                        if(cubeBoolList[(int)partsCube.transform.position.x+1,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z])
                        {
                            rightConnectionInformation.part = partsList.IndexOf(cubes[cubeNumList[(int)partsCube.transform.position.x+1,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z]].transform.parent.gameObject);
                            nowDecompositionCubeDate.canConnectionNum++;

                            connectionCubePosNum = GetChildIndex(cubes[cubeNumList[(int)partsCube.transform.position.x+1,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z]].transform.parent,cubes[cubeNumList[(int)partsCube.transform.position.x+1,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z]]);
                            CaliculateConnectionCubePos(ref rightConnectionInformation.connectionX,ref rightConnectionInformation.connectionY,ref rightConnectionInformation.connectionZ,connectionCubePosNum,N);

                            colorList.Add(cubes[cubeNumList[(int)partsCube.transform.position.x+1,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z]].GetComponent<Renderer>().material.color);
                            connectionNumList.Add(connectionNum);
                            connectionNum++;
                            nowDecompositionCubeDate.rightConnectionInformation.Add(rightConnectionInformation);
                        }
                        else
                        {
                            rightConnectionInformation.part = -1;
                            nowDecompositionCubeDate.rightConnectionInformation.Add(rightConnectionInformation);
                        }
                    }
                    else
                    {
                        for(int i=1; i < N; i++)
                        {
                            if(nowDecompositionCubeDate.decompositionCubeArray[nowX-i,nowY,nowZ] == 0 || nowDecompositionCubeDate.decompositionCubeArray[nowX-i,nowY,nowZ] == 1)
                            {
                                GameObject partsCube = parts.transform.GetChild(nowX-i+nowZ*N+nowY*(N*N)).gameObject;
                                RightConnectionInformation rightConnectionInformation = new RightConnectionInformation();
                                rightConnectionInformation.x = nowX-i;
                                rightConnectionInformation.y = nowY;
                                rightConnectionInformation.z = nowZ;

                                if(cubeBoolList[(int)partsCube.transform.position.x+1,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z])
                                {
                                    rightConnectionInformation.part = partsList.IndexOf(cubes[cubeNumList[(int)partsCube.transform.position.x+1,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z]].transform.parent.gameObject);
                                    nowDecompositionCubeDate.canConnectionNum++;

                                    connectionCubePosNum = GetChildIndex(cubes[cubeNumList[(int)partsCube.transform.position.x+1,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z]].transform.parent,cubes[cubeNumList[(int)partsCube.transform.position.x+1,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z]]);
                                    CaliculateConnectionCubePos(ref rightConnectionInformation.connectionX,ref rightConnectionInformation.connectionY,ref rightConnectionInformation.connectionZ,connectionCubePosNum,N);

                                    colorList.Add(cubes[cubeNumList[(int)partsCube.transform.position.x+1,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z]].GetComponent<Renderer>().material.color);
                                    connectionNumList.Add(connectionNum);
                                    connectionNum++;
                                    nowDecompositionCubeDate.rightConnectionInformation.Add(rightConnectionInformation);
                                    break;
                                }
                                else
                                {
                                    rightConnectionInformation.part = -1;
                                    nowDecompositionCubeDate.rightConnectionInformation.Add(rightConnectionInformation);
                                    break;
                                }
                            }
                        }
                    }
                }
                nowZ = 0;
            }

            for(nowY=0; nowY < N; nowY++)//Leftの処理
            {
                nowX = 0;
                for(nowZ=0; nowZ < N; nowZ++)
                {
                    if(nowDecompositionCubeDate.decompositionCubeArray[nowX,nowY,nowZ] == 0 || nowDecompositionCubeDate.decompositionCubeArray[nowX,nowY,nowZ] == 1)
                    {
                        GameObject partsCube = parts.transform.GetChild(nowX+nowZ*N+nowY*(N*N)).gameObject;
                        LeftConnectionInformation leftConnectionInformation = new LeftConnectionInformation();
                        leftConnectionInformation.x = nowX;
                        leftConnectionInformation.y = nowY;
                        leftConnectionInformation.z = nowZ;

                        if(cubeBoolList[(int)partsCube.transform.position.x-1,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z])
                        {
                            leftConnectionInformation.part = partsList.IndexOf(cubes[cubeNumList[(int)partsCube.transform.position.x-1,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z]].transform.parent.gameObject);
                            nowDecompositionCubeDate.canConnectionNum++;

                            connectionCubePosNum = GetChildIndex(cubes[cubeNumList[(int)partsCube.transform.position.x-1,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z]].transform.parent,cubes[cubeNumList[(int)partsCube.transform.position.x-1,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z]]);
                            CaliculateConnectionCubePos(ref leftConnectionInformation.connectionX,ref leftConnectionInformation.connectionY,ref leftConnectionInformation.connectionZ,connectionCubePosNum,N);

                            nowDecompositionCubeDate.leftConnectionInformation.Add(leftConnectionInformation);
                            for(int i = 0; i < colorList.Count; i++)
                            {
                                if(colorList[i] == cubes[cubeNumList[(int)partsCube.transform.position.x-1,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z]].GetComponent<Renderer>().material.color && nowDecompositionCubeDate.color == cubes[cubeNumList[(int)partsCube.transform.position.x-1,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z]].GetComponent<Renderer>().material.color)
                                {
                                    if(nowZ == decompositionCubeDateList[partsList.IndexOf(parts)].rightConnectionInformation[connectionNumList[i]].z && nowY == decompositionCubeDateList[partsList.IndexOf(parts)].rightConnectionInformation[connectionNumList[i]].y)
                                    {
                                        RightConnectionInformation rightReproductionConnectionInformation = new RightConnectionInformation();
                                        rightReproductionConnectionInformation.x = decompositionCubeDateList[partsList.IndexOf(parts)].rightConnectionInformation[connectionNumList[i]].x;
                                        rightReproductionConnectionInformation.y = decompositionCubeDateList[partsList.IndexOf(parts)].rightConnectionInformation[connectionNumList[i]].y;
                                        rightReproductionConnectionInformation.z = decompositionCubeDateList[partsList.IndexOf(parts)].rightConnectionInformation[connectionNumList[i]].z;
                                        rightReproductionConnectionInformation.part = partsList.IndexOf(parts);
                                        rightReproductionConnectionInformation.connectionX = decompositionCubeDateList[partsList.IndexOf(parts)].rightConnectionInformation[connectionNumList[i]].x-(N-1);
                                        rightReproductionConnectionInformation.connectionY = decompositionCubeDateList[partsList.IndexOf(parts)].rightConnectionInformation[connectionNumList[i]].y;
                                        rightReproductionConnectionInformation.connectionZ = decompositionCubeDateList[partsList.IndexOf(parts)].rightConnectionInformation[connectionNumList[i]].z;
                                        nowDecompositionCubeDate.rightConnectionInformation.Add(rightReproductionConnectionInformation);
                                        nowDecompositionCubeDate.canConnectionNum++;

                                        LeftConnectionInformation leftReproductionConnectionInformation = new LeftConnectionInformation();
                                        leftReproductionConnectionInformation.x = decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation.Count - 1].x;
                                        leftReproductionConnectionInformation.y = decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation.Count - 1].y;
                                        leftReproductionConnectionInformation.z = decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation.Count - 1].z;
                                        leftReproductionConnectionInformation.part = partsList.IndexOf(parts);
                                        leftReproductionConnectionInformation.connectionX = decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation.Count - 1].x+(N-1);
                                        leftReproductionConnectionInformation.connectionY = decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation.Count - 1].y;
                                        leftReproductionConnectionInformation.connectionZ = decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation.Count - 1].z;
                                        nowDecompositionCubeDate.leftConnectionInformation.Add(leftReproductionConnectionInformation);
                                        nowDecompositionCubeDate.canConnectionNum++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            leftConnectionInformation.part = -1;
                            nowDecompositionCubeDate.leftConnectionInformation.Add(leftConnectionInformation);
                        }
                    }
                    else
                    {
                        for(int i=1; i < N; i++)
                        {
                            if(nowDecompositionCubeDate.decompositionCubeArray[nowX+i,nowY,nowZ] == 0 || nowDecompositionCubeDate.decompositionCubeArray[nowX+i,nowY,nowZ] == 1)
                            {
                                GameObject partsCube = parts.transform.GetChild(nowX+i+nowZ*N+nowY*(N*N)).gameObject;
                                LeftConnectionInformation leftConnectionInformation = new LeftConnectionInformation();
                                leftConnectionInformation.x = nowX+i;
                                leftConnectionInformation.y = nowY;
                                leftConnectionInformation.z = nowZ;

                                if(cubeBoolList[(int)partsCube.transform.position.x-1,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z])
                                {
                                    leftConnectionInformation.part = partsList.IndexOf(cubes[cubeNumList[(int)partsCube.transform.position.x-1,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z]].transform.parent.gameObject);
                                    nowDecompositionCubeDate.canConnectionNum++;

                                    connectionCubePosNum = GetChildIndex(cubes[cubeNumList[(int)partsCube.transform.position.x-1,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z]].transform.parent,cubes[cubeNumList[(int)partsCube.transform.position.x-1,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z]]);
                                    CaliculateConnectionCubePos(ref leftConnectionInformation.connectionX,ref leftConnectionInformation.connectionY,ref leftConnectionInformation.connectionZ,connectionCubePosNum,N);

                                    nowDecompositionCubeDate.leftConnectionInformation.Add(leftConnectionInformation);
                                    for(int j = 0; j < colorList.Count; j++)
                                    {
                                        if(colorList[j] == cubes[cubeNumList[(int)partsCube.transform.position.x-1,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z]].GetComponent<Renderer>().material.color && nowDecompositionCubeDate.color == cubes[cubeNumList[(int)partsCube.transform.position.x-1,(int)partsCube.transform.position.y,(int)partsCube.transform.position.z]].GetComponent<Renderer>().material.color)
                                        {
                                            if(nowZ == decompositionCubeDateList[partsList.IndexOf(parts)].rightConnectionInformation[connectionNumList[j]].z && nowY == decompositionCubeDateList[partsList.IndexOf(parts)].rightConnectionInformation[connectionNumList[j]].y)
                                            {
                                                RightConnectionInformation rightReproductionConnectionInformation = new RightConnectionInformation();
                                                rightReproductionConnectionInformation.x = decompositionCubeDateList[partsList.IndexOf(parts)].rightConnectionInformation[connectionNumList[j]].x;
                                                rightReproductionConnectionInformation.y = decompositionCubeDateList[partsList.IndexOf(parts)].rightConnectionInformation[connectionNumList[j]].y;
                                                rightReproductionConnectionInformation.z = decompositionCubeDateList[partsList.IndexOf(parts)].rightConnectionInformation[connectionNumList[j]].z;
                                                rightReproductionConnectionInformation.part = partsList.IndexOf(parts);
                                                rightReproductionConnectionInformation.connectionX = decompositionCubeDateList[partsList.IndexOf(parts)].rightConnectionInformation[connectionNumList[j]].x-(N-1);
                                                rightReproductionConnectionInformation.connectionY = decompositionCubeDateList[partsList.IndexOf(parts)].rightConnectionInformation[connectionNumList[j]].y;
                                                rightReproductionConnectionInformation.connectionZ = decompositionCubeDateList[partsList.IndexOf(parts)].rightConnectionInformation[connectionNumList[j]].z;
                                                nowDecompositionCubeDate.rightConnectionInformation.Add(rightReproductionConnectionInformation);
                                                nowDecompositionCubeDate.canConnectionNum++;

                                                LeftConnectionInformation leftReproductionConnectionInformation = new LeftConnectionInformation();
                                                leftReproductionConnectionInformation.x = decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation.Count - 1].x;
                                                leftReproductionConnectionInformation.y = decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation.Count - 1].y;
                                                leftReproductionConnectionInformation.z = decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation.Count - 1].z;
                                                leftReproductionConnectionInformation.part = partsList.IndexOf(parts);
                                                leftReproductionConnectionInformation.connectionX = decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation.Count - 1].x+(N-1);
                                                leftReproductionConnectionInformation.connectionY = decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation.Count - 1].y;
                                                leftReproductionConnectionInformation.connectionZ = decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].leftConnectionInformation.Count - 1].z;
                                                nowDecompositionCubeDate.leftConnectionInformation.Add(leftReproductionConnectionInformation);
                                                nowDecompositionCubeDate.canConnectionNum++;
                                            }
                                        }
                                    }
                                    break;
                                }
                                else
                                {
                                    leftConnectionInformation.part = -1;
                                    nowDecompositionCubeDate.leftConnectionInformation.Add(leftConnectionInformation);
                                    break;
                                }
                            }
                        }
                    }
                }
                nowZ = 0;
            }
            colorList.Clear();
            connectionNumList.Clear();
            connectionNum = 0;

            for(nowZ=0; nowZ<N; nowZ++)//upの処理
            {
                nowY = N-1;
                for(nowX=0; nowX<N; nowX++)
                {
                    if(nowDecompositionCubeDate.decompositionCubeArray[nowX,nowY,nowZ] == 0 || nowDecompositionCubeDate.decompositionCubeArray[nowX,nowY,nowZ] == 1)
                    {
                        GameObject partsCube = parts.transform.GetChild(nowX+nowZ*N+nowY*N*N).gameObject;
                        UpConnectionInformation upConnectionInformation = new UpConnectionInformation();
                        upConnectionInformation.x = nowX;
                        upConnectionInformation.y = nowY;
                        upConnectionInformation.z = nowZ;

                        if(cubeBoolList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y+1,(int)partsCube.transform.position.z])
                        {
                            upConnectionInformation.part = partsList.IndexOf(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y+1,(int)partsCube.transform.position.z]].transform.parent.gameObject);
                            nowDecompositionCubeDate.canConnectionNum++;

                            connectionCubePosNum = GetChildIndex(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y+1,(int)partsCube.transform.position.z]].transform.parent,cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y+1,(int)partsCube.transform.position.z]]);
                            CaliculateConnectionCubePos(ref upConnectionInformation.connectionX,ref upConnectionInformation.connectionY,ref upConnectionInformation.connectionZ,connectionCubePosNum,N);

                            colorList.Add(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y+1,(int)partsCube.transform.position.z]].GetComponent<Renderer>().material.color);
                            connectionNumList.Add(connectionNum);
                            connectionNum++;
                            nowDecompositionCubeDate.upConnectionInformation.Add(upConnectionInformation);
                        }
                        else
                        {
                            upConnectionInformation.part = -1;
                            nowDecompositionCubeDate.upConnectionInformation.Add(upConnectionInformation);
                        }
                    }
                    else
                    {
                        for(int i=1; i < N; i++)
                        {
                            if(nowDecompositionCubeDate.decompositionCubeArray[nowX,nowY-i,nowZ] == 0 || nowDecompositionCubeDate.decompositionCubeArray[nowX,nowY-i,nowZ] == 1)
                            {
                                GameObject partsCube = parts.transform.GetChild(nowX+nowZ*N+(nowY-i)*N*N).gameObject;
                                UpConnectionInformation upConnectionInformation = new UpConnectionInformation();
                                upConnectionInformation.x = nowX;
                                upConnectionInformation.y = nowY-i;
                                upConnectionInformation.z = nowZ;

                                if(cubeBoolList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y+1,(int)partsCube.transform.position.z])
                                {
                                    upConnectionInformation.part = partsList.IndexOf(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y+1,(int)partsCube.transform.position.z]].transform.parent.gameObject);
                                    nowDecompositionCubeDate.canConnectionNum++;

                                    connectionCubePosNum = GetChildIndex(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y+1,(int)partsCube.transform.position.z]].transform.parent,cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y+1,(int)partsCube.transform.position.z]]);
                                    CaliculateConnectionCubePos(ref upConnectionInformation.connectionX,ref upConnectionInformation.connectionY,ref upConnectionInformation.connectionZ,connectionCubePosNum,N);

                                    colorList.Add(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y+1,(int)partsCube.transform.position.z]].GetComponent<Renderer>().material.color);
                                    connectionNumList.Add(connectionNum);
                                    connectionNum++;
                                    nowDecompositionCubeDate.upConnectionInformation.Add(upConnectionInformation);
                                    break;
                                }
                                else
                                {
                                    upConnectionInformation.part = -1;
                                    nowDecompositionCubeDate.upConnectionInformation.Add(upConnectionInformation);
                                    break;
                                }
                            }
                        }
                    }
                }
                nowX = 0;
            }

            for(nowZ=0; nowZ<N; nowZ++)//downの処理
            {
                nowY = 0;
                for(nowX=0; nowX<N; nowX++)
                {
                    if(nowDecompositionCubeDate.decompositionCubeArray[nowX,nowY,nowZ] == 0 || nowDecompositionCubeDate.decompositionCubeArray[nowX,nowY,nowZ] == 1)
                    {
                        GameObject partsCube = parts.transform.GetChild(nowX+nowZ*N+nowY*N*N).gameObject;
                        DownConnectionInformation downConnectionInformation = new DownConnectionInformation();
                        downConnectionInformation.x = nowX;
                        downConnectionInformation.y = nowY;
                        downConnectionInformation.z = nowZ;

                        if(partsCube.transform.position.y == 0)
                        {
                            downConnectionInformation.part = -2;
                            nowDecompositionCubeDate.downConnectionInformation.Add(downConnectionInformation);
                        }
                        else if(cubeBoolList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y-1,(int)partsCube.transform.position.z])
                        {
                            downConnectionInformation.part = partsList.IndexOf(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y-1,(int)partsCube.transform.position.z]].transform.parent.gameObject);
                            nowDecompositionCubeDate.canConnectionNum++;

                            connectionCubePosNum = GetChildIndex(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y-1,(int)partsCube.transform.position.z]].transform.parent,cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y-1,(int)partsCube.transform.position.z]]);
                            CaliculateConnectionCubePos(ref downConnectionInformation.connectionX,ref downConnectionInformation.connectionY,ref downConnectionInformation.connectionZ,connectionCubePosNum,N);

                            nowDecompositionCubeDate.downConnectionInformation.Add(downConnectionInformation);
                            
                            for(int i = 0; i < colorList.Count; i++)
                            {
                                if(cubeBoolList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y+N,(int)partsCube.transform.position.z])
                                {
                                    if(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y+N,(int)partsCube.transform.position.z]].GetComponent<Renderer>().material.color == cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y-1,(int)partsCube.transform.position.z]].GetComponent<Renderer>().material.color && nowDecompositionCubeDate.color == cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y-1,(int)partsCube.transform.position.z]].GetComponent<Renderer>().material.color)
                                    {
                                        if(nowX == decompositionCubeDateList[partsList.IndexOf(parts)].upConnectionInformation[connectionNumList[i]].x && nowZ == decompositionCubeDateList[partsList.IndexOf(parts)].upConnectionInformation[connectionNumList[i]].z)
                                        {
                                            UpConnectionInformation upReproductionConnectionInformation = new UpConnectionInformation();
                                            upReproductionConnectionInformation.x = decompositionCubeDateList[partsList.IndexOf(parts)].upConnectionInformation[connectionNumList[i]].x;
                                            upReproductionConnectionInformation.y = decompositionCubeDateList[partsList.IndexOf(parts)].upConnectionInformation[connectionNumList[i]].y;
                                            upReproductionConnectionInformation.z = decompositionCubeDateList[partsList.IndexOf(parts)].upConnectionInformation[connectionNumList[i]].z;
                                            upReproductionConnectionInformation.part = partsList.IndexOf(parts);
                                            upReproductionConnectionInformation.connectionX = decompositionCubeDateList[partsList.IndexOf(parts)].upConnectionInformation[connectionNumList[i]].x;
                                            upReproductionConnectionInformation.connectionY = decompositionCubeDateList[partsList.IndexOf(parts)].upConnectionInformation[connectionNumList[i]].y-(N-1);
                                            upReproductionConnectionInformation.connectionZ = decompositionCubeDateList[partsList.IndexOf(parts)].upConnectionInformation[connectionNumList[i]].z;
                                            nowDecompositionCubeDate.upConnectionInformation.Add(upReproductionConnectionInformation);
                                            nowDecompositionCubeDate.canConnectionNum++;

                                            DownConnectionInformation downReproductionConnectionInformation = new DownConnectionInformation();
                                            downReproductionConnectionInformation.x = decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation.Count - 1].x;
                                            downReproductionConnectionInformation.y = decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation.Count - 1].y;
                                            downReproductionConnectionInformation.z = decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation.Count - 1].z;
                                            downReproductionConnectionInformation.part = partsList.IndexOf(parts);
                                            downReproductionConnectionInformation.connectionX = decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation.Count - 1].x;
                                            downReproductionConnectionInformation.connectionY = decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation.Count - 1].y+(N-1);
                                            downReproductionConnectionInformation.connectionZ = decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation.Count - 1].z;
                                            nowDecompositionCubeDate.downConnectionInformation.Add(downReproductionConnectionInformation);
                                            nowDecompositionCubeDate.canConnectionNum++;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            downConnectionInformation.part = -1;
                            nowDecompositionCubeDate.downConnectionInformation.Add(downConnectionInformation);
                        }
                    }
                    else
                    {
                        for(int i=1; i < N; i++)
                        {
                            if(nowDecompositionCubeDate.decompositionCubeArray[nowX,nowY+i,nowZ] == 0 || nowDecompositionCubeDate.decompositionCubeArray[nowX,nowY+i,nowZ] == 1)
                            {
                                GameObject partsCube = parts.transform.GetChild(nowX+nowZ*N+(nowY+i)*N*N).gameObject;
                                DownConnectionInformation downConnectionInformation = new DownConnectionInformation();
                                downConnectionInformation.x = nowX;
                                downConnectionInformation.y = nowY+i;
                                downConnectionInformation.z = nowZ;

                                if(cubeBoolList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y-1,(int)partsCube.transform.position.z])
                                {
                                    downConnectionInformation.part = partsList.IndexOf(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y-1,(int)partsCube.transform.position.z]].transform.parent.gameObject);
                                    nowDecompositionCubeDate.canConnectionNum++;

                                    connectionCubePosNum = GetChildIndex(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y-1,(int)partsCube.transform.position.z]].transform.parent,cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y-1,(int)partsCube.transform.position.z]]);
                                    CaliculateConnectionCubePos(ref downConnectionInformation.connectionX,ref downConnectionInformation.connectionY,ref downConnectionInformation.connectionZ,connectionCubePosNum,N);

                                    nowDecompositionCubeDate.downConnectionInformation.Add(downConnectionInformation);

                                    for(int j = 0; j < colorList.Count; j++)
                                    {
                                        if(cubeBoolList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y+N-i,(int)partsCube.transform.position.z])
                                        {
                                            if(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y+N-i,(int)partsCube.transform.position.z]].GetComponent<Renderer>().material.color == cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y-1,(int)partsCube.transform.position.z]].GetComponent<Renderer>().material.color && nowDecompositionCubeDate.color == cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y-1,(int)partsCube.transform.position.z]].GetComponent<Renderer>().material.color)
                                            {
                                                if(nowX == decompositionCubeDateList[partsList.IndexOf(parts)].upConnectionInformation[connectionNumList[j]].x && nowZ == decompositionCubeDateList[partsList.IndexOf(parts)].upConnectionInformation[connectionNumList[j]].z)
                                                {
                                                    UpConnectionInformation upReproductionConnectionInformation = new UpConnectionInformation();
                                                    upReproductionConnectionInformation.x = decompositionCubeDateList[partsList.IndexOf(parts)].upConnectionInformation[connectionNumList[j]].x;
                                                    upReproductionConnectionInformation.y = decompositionCubeDateList[partsList.IndexOf(parts)].upConnectionInformation[connectionNumList[j]].y;
                                                    upReproductionConnectionInformation.z = decompositionCubeDateList[partsList.IndexOf(parts)].upConnectionInformation[connectionNumList[j]].z;
                                                    upReproductionConnectionInformation.part = partsList.IndexOf(parts);
                                                    upReproductionConnectionInformation.connectionX = decompositionCubeDateList[partsList.IndexOf(parts)].upConnectionInformation[connectionNumList[j]].x;
                                                    upReproductionConnectionInformation.connectionY = decompositionCubeDateList[partsList.IndexOf(parts)].upConnectionInformation[connectionNumList[j]].y-(N-1);
                                                    upReproductionConnectionInformation.connectionZ = decompositionCubeDateList[partsList.IndexOf(parts)].upConnectionInformation[connectionNumList[j]].z;
                                                    nowDecompositionCubeDate.upConnectionInformation.Add(upReproductionConnectionInformation);
                                                    nowDecompositionCubeDate.canConnectionNum++;

                                                    DownConnectionInformation downReproductionConnectionInformation = new DownConnectionInformation();
                                                    downReproductionConnectionInformation.x = decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation.Count - 1].x;
                                                    downReproductionConnectionInformation.y = decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation.Count - 1].y;
                                                    downReproductionConnectionInformation.z = decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation.Count - 1].z;
                                                    downReproductionConnectionInformation.part = partsList.IndexOf(parts);
                                                    downReproductionConnectionInformation.connectionX = decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation.Count - 1].x;
                                                    downReproductionConnectionInformation.connectionY = decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation.Count - 1].y+(N-1);
                                                    downReproductionConnectionInformation.connectionZ = decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation[decompositionCubeDateList[partsList.IndexOf(parts)].downConnectionInformation.Count - 1].z;
                                                    nowDecompositionCubeDate.downConnectionInformation.Add(downReproductionConnectionInformation);
                                                    nowDecompositionCubeDate.canConnectionNum++;
                                                }
                                            }
                                        }
                                    }
                                    break;
                                }
                                else
                                {
                                    downConnectionInformation.part = -1;
                                    nowDecompositionCubeDate.downConnectionInformation.Add(downConnectionInformation);
                                    break;
                                }
                            }
                        }
                    }
                }
                nowX = 0;
            }
        }
    }

    void partsDecomposition() //色で分解されたパーツをN×N×Nに分解する関数
    {
        foreach (GameObject colorParts in colorPartsList)//全ての色分けパーツが分解されるまで以下をループ
        {
            foreach(Transform child in colorParts.transform)//親から子を取得
            {
                colorChildList.Add(child.gameObject);
            }
            //子の中から最小値と最大値を探す
            int minx;
            int miny;
            int minz;
            int maxx;
            int maxy;
            int maxz;

            minx = (int)colorChildList[0].transform.position.x;
            miny = (int)colorChildList[0].transform.position.y;
            minz = (int)colorChildList[0].transform.position.z;
            maxx = (int)colorChildList[0].transform.position.x;
            maxy = (int)colorChildList[0].transform.position.y;
            maxz = (int)colorChildList[0].transform.position.z;
            foreach(GameObject colorChild in colorChildList)
            {
                if(minx > colorChild.transform.position.x)
                {
                    minx = (int)colorChild.transform.position.x;
                }
                if(miny > colorChild.transform.position.y)
                {
                    miny = (int)colorChild.transform.position.y;
                }
                if(minz > colorChild.transform.position.z)
                {
                    minz = (int)colorChild.transform.position.z;
                }
                if(maxx < colorChild.transform.position.x)
                {
                    maxx = (int)colorChild.transform.position.x;
                }
                if(maxy < colorChild.transform.position.y)
                {
                    maxy = (int)colorChild.transform.position.y;
                }
                if(maxz < colorChild.transform.position.z)
                {
                    maxz = (int)colorChild.transform.position.z;
                }
            }
            //その座標からN＊N*Nの形に分解していく
            int nowX = minx;
            int nowY = miny;
            int nowZ = minz;
            int decompositionNum_x = (maxx - minx) / N+1;
            int decompositionNum_y = (maxy - miny) / N+1;
            int decompositionNum_z = (maxz - minz) / N+1;
            bool includeCube = false;

            for(int Num_y = 0; Num_y < decompositionNum_y; Num_y++)
            {
                for(int Num_z = 0; Num_z < decompositionNum_z; Num_z++)
                {
                    for(int Num_x = 0; Num_x < decompositionNum_x; Num_x++)
                    {
                        GameObject parts = new GameObject("Parts_"+partsList.Count);
                        includeCube = false;

                        for(int y = 0; y < N; y++)//N*N*Nの形に分解していく
                        {
                            for(int z = 0; z < N; z++)
                            {
                                for(int x = 0; x < N; x++)
                                {
                                    if(nowX<=maxx && nowY<=maxy && nowZ<=maxz)
                                    {
                                        if(cubeBoolList[nowX,nowY,nowZ])
                                        {
                                            if(colorChildList.Contains(cubes[cubeNumList[nowX,nowY,nowZ]])==true)
                                            {
                                                cubes[cubeNumList[nowX,nowY,nowZ]].transform.parent = parts.transform;
                                                decompositionCubeArray[x,y,z] = 1;
                                                includeCube = true;
                                            }
                                            else
                                            {
                                                GameObject air = new GameObject("ExceptionAir");
                                                air.transform.parent = parts.transform;
                                                air.transform.position = new Vector3(nowX,nowY,nowZ);
                                                decompositionCubeArray[x,y,z] = -1;
                                            }
                                        }
                                        else
                                        {
                                            GameObject air = new GameObject("Air");
                                            air.transform.parent = parts.transform;
                                            air.transform.position = new Vector3(nowX,nowY,nowZ);
                                            decompositionCubeArray[x,y,z] = 0;
                                        }
                                    }
                                    else
                                    {
                                        GameObject air = new GameObject("ExceptionAir");
                                        air.transform.parent = parts.transform;
                                        air.transform.position = new Vector3(nowX,nowY,nowZ);
                                        decompositionCubeArray[x,y,z] = -1;
                                    }
                                    nowX++;
                                }
                                nowX = minx;
                                nowZ++;
                            }
                            
                            nowX = minx;
                            nowZ = minz;
                            nowY++;
                        }//N*N*Nの形に分解していくここまで,またずらして次のN*N*Nの形に分解していく
                        if(includeCube)//分解後のデータにパーツが含まれている場合はリストに追加する
                        {
                            DecompositionCubeDate decompositionCubeDate = new DecompositionCubeDate(); //分解後のデータを格納するクラスのインスタンス生成し、リストに追加
                            decompositionCubeDate.color = colorChildList[0].GetComponent<Renderer>().material.color;
                            decompositionCubeDate.decompositionCubeArray = decompositionCubeArray.Clone() as int[,,]; // 新しい配列を生成して代入
                            decompositionCubeDate.partNum = partsList.Count;
                            decompositionCubeDateList.Add(decompositionCubeDate);
                            partsList.Add(parts);
                        }
                        else//分解後のデータにパーツが含まれていない場合はパーツを削除する
                        {
                            Destroy(parts);
                        }
                        minx += N;
                        nowX = minx;
                        nowZ = minz;
                        nowY = miny;
                    }
                    minx = minx - decompositionNum_x*N;
                    nowX = minx;
                    minz += N;
                    nowZ = minz;
                    nowY = miny;
                }
                minz = minz - decompositionNum_z*N;
                nowX = minx;
                nowZ = minz;
                miny += N;
                nowY = miny;
            }
            colorChildList.Clear();
        }
    }
    void ColerDecomposition() //色分けで分解する関数
    {
        foreach (GameObject cube in cubes)
        {
               Color cubeColor = cube.GetComponent<Renderer>().material.color;
               bool isSameColor = false;

               if(colorPartsList== null) //colorPartsListが空の場合は新しく生成する
               {
                    GameObject colorParts = new GameObject("ColorParts_"+colorPartsList.Count);
                    cube.transform.parent = colorParts.transform;
                    colorList.Add(cubeColor);
                    colorPartsList.Add(colorParts);
               }

               foreach(Color color in colorList)
               {
                    if(color == cubeColor) //同じ色のパーツがある場合はそのパーツに追加する
                    {
                         cube.transform.parent = colorPartsList[colorList.IndexOf(color)].transform;
                         isSameColor = true;
                    }
               }

               if(!isSameColor) //同じ色のパーツがない場合は新しく生成する
               {
                    GameObject colorParts = new GameObject("ColorParts_"+colorPartsList.Count);
                    cube.transform.parent = colorParts.transform;
                    colorList.Add(cubeColor);
                    colorPartsList.Add(colorParts);
               }
        }
    }

    void LoadAndGenerateCubes() //.txtから読み込んでCubeを生成する関数の実行
    {
        string[] lines = System.IO.File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            string[] values = line.Trim('[', ']').Split(' ');
            if (values.Length == 6)
            {
                if (float.TryParse(values[0], out float x) &&
                    float.TryParse(values[2], out float y) &&
                    float.TryParse(values[1], out float z) &&
                    float.TryParse(values[3], out float r) &&
                    float.TryParse(values[4], out float g) &&
                    float.TryParse(values[5], out float b))
                {
                    x = Mathf.Clamp(x, -1000f, 1000f); // 任意の範囲で値を制限
                    y = Mathf.Clamp(y, -1000f, 1000f);
                    z = Mathf.Clamp(z, -1000f, 1000f);
                    r = Mathf.Clamp01(r / 255f);
                    g = Mathf.Clamp01(g / 255f);
                    b = Mathf.Clamp01(b / 255f);

                    Vector3 position = new Vector3(x, y, z);
                    GenerateCube(position, new Color(r, g, b));
                }
                else
                {
                    Debug.LogWarning("Invalid line format: " + line);
                }
            }
            else
            {
                Debug.LogWarning("Invalid line format: " + line);
            }
        }
    }

    void GenerateCube(Vector3 position, Color color) //Cubeを生成する関数であり、ボクセルデータの更新も行う
    {
        GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
        cube.GetComponent<Renderer>().material.color = color;
        cubes.Add(cube);

        UpdateVoxelData(cube);
        cubeNum++;
    }

    void UpdateVoxelData(GameObject cube) //ボクセルデータの更新を行う関数
    {
        Vector3 cubePosition = cube.transform.position;

        cubeNumList[(int)cubePosition.x, (int)cubePosition.y, (int)cubePosition.z] = cubeNum;
        cubeBoolList[(int)cubePosition.x, (int)cubePosition.y, (int)cubePosition.z] = true;
    }

    void ArrayInitialization(int[,,] array) //配列の初期化を行う関数
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for(int j = 0; j < array.GetLength(1); j++)
            {
                for(int k = 0; k < array.GetLength(2); k++)
                {
                    array[i, j, k] = -1;
                }
            }
        }
    }
}
