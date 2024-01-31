using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ColerDecompositionWFC_1 : MonoBehaviour
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
    private int[,,] cubeNumList; //ボクセルデータのどのボクセルかを格納する配列
    private bool[,,] cubeBoolList; //ボクセルデータのその座標にボクセルがあるかを格納する配列
    private List<GameObject> partsList = new List<GameObject>(); //パーツを格納するリスト
    private List<GameObject> colorChildList = new List<GameObject>(); //色分けで分解したパーツの子オブジェクトを格納するリスト
    private List<GameObject> cubes = new List<GameObject>(); //生成したCubeを格納するリスト
    private List<GameObject> colorPartsList = new List<GameObject>(); //色分けで分解したパーツを格納するリスト
    private List<Color> colorList = new List<Color>(); //色分けで分解したパーツの色を格納するリスト
    /// <summary>
    /// ///////////////////////////////WFCで生成したパーツを格納する///////////////////////////////
    /// </summary>
    private int generatePart = 0; //WFCで生成するパーツの番号
    private List<GameObject> partsListWFC = new List<GameObject>(); //WFCで生成したパーツを格納するリスト
    private List<GameObject> cubesWFC = new List<GameObject>(); //WFCで生成したCubeを格納するリスト
    private int[,,] cubeNumListWFC; //WFCで生成したボクセルデータのどのボクセルかを格納する配列
    private int[,,] cubeTypeListWFC; //WFCで生成したボクセルデータのその座標に何の種類のボクセルがあるかを格納する配列
    ///
    /// ///////////////////////////////エントロピーの計算に使うクラスを宣言する///////////////////////////////
    /// 
    private class ConnectionParts
    {
        public Vector3Int position;
        public List<int> partNum = new List<int>();
    }
    /// <summary>
    /// ///////////////////////////////エントロピーを格納する///////////////////////////////
    /// </summary>
    private List<ConnectionParts> connectionPartsList = new List<ConnectionParts>(); //エントロピーを計算するための接続情報を格納するリスト
    private DecompositionCubeDate airDate = new DecompositionCubeDate(); //空気のデータを格納するクラス
    private List<bool[]> entropyBoolList = new List<bool[]>(); //エントロピーを格納するリスト
    private float minEntropy = 0; //最小のエントロピーを格納する変数
    private int minEntropyIndex = 0; //最小のエントロピーのインデックスを格納する変数
    private bool isError = false; //矛盾が起きたかを判定する変数
    /// <summary>
    /// ///////////////////////////////SerializeField//////////////////////////////////////////////////////////
    /// </summary>
    [SerializeField] private GameObject cubePrefab; //生成するCubeのPrefab
    [SerializeField] private string filePath; //読み込むファイルのパス
    [SerializeField] private int N; //N×N×Nに分解する
    [SerializeField] private int outputSize_x; //出力するボクセルデータのxのサイズ
    [SerializeField] private int outputSize_y; //出力するボクセルデータのyのサイズ
    [SerializeField] private int outputSize_z; //出力するボクセルデータのzのサイズ
    void Start()
    {
        cubeNumListWFC = new int[100,100,100];
        cubeBoolList = new bool[100,100,100];
        cubeNumListWFC = new int[outputSize_x,outputSize_y,outputSize_z];
        cubeTypeListWFC = new int[outputSize_x,outputSize_y,outputSize_z];
        decompositionCubeArray = new int[N,N,N];
        cubeNumList = new int[outputSize_x,outputSize_y,outputSize_z];
        ArrayInitialization(cubeNumList);
        ArrayInitialization(cubeTypeListWFC);
        LoadAndGenerateCubes(); //.txtファイルを読み込みCubeを生成する
        ColerDecomposition();
        partsDecomposition();
        ConnectionInformationSaving();
        WFC();
    }

    void WFC()//WFC実行
    {
        bool isContinue;
        EntropyBoolListAdd();
        int firstPlace_x = Random.Range(0,outputSize_x);
        int firstPlace_y = Random.Range(0,outputSize_y);
        int firstPlace_z = Random.Range(0,outputSize_z);

        minEntropyIndex = firstPlace_x+firstPlace_z*outputSize_x+firstPlace_y*outputSize_x*outputSize_z;
        SelectGeneratePartInMinEntropy();
        GameObject firstPart = new GameObject("part");
        partsListWFC.Add(firstPart);
        generatePart = 4;
        GeneratePartsWFC(decompositionCubeDateList[generatePart],minEntropyIndex,firstPart);
        Debug.Log("generatePart:"+generatePart);
        EntropyBoolListUpdate(firstPlace_x,firstPlace_y,firstPlace_z,decompositionCubeDateList[generatePart]);
        CaliculateEntropy();

        while(true)
        {
            isContinue = false;
            GameObject part = new GameObject("part"+partsListWFC.Count);
            partsListWFC.Add(part);
            GeneratePartsWFC(decompositionCubeDateList[generatePart],minEntropyIndex,part);
            EntropyBoolListUpdate(minEntropyIndex%outputSize_x,(minEntropyIndex - minEntropyIndex%outputSize_x)/outputSize_x/outputSize_z,(minEntropyIndex - minEntropyIndex%outputSize_x)/outputSize_x%outputSize_z,decompositionCubeDateList[generatePart]);

            for (int i = 0; i < cubeTypeListWFC.GetLength(0); i++)
            {
                for (int j = 0; j < cubeTypeListWFC.GetLength(1); j++)
                {
                    for (int k = 0; k < cubeTypeListWFC.GetLength(2); k++)
                    {
                        if (cubeTypeListWFC[i, j, k] == -1)
                        {
                            isContinue = true;
                            break;
                        }
                    }

                    if (isContinue)
                    {
                        break;
                    }
                }

                if (isContinue)
                {
                    break;
                }
            }
            if(isContinue == false)
            {
                Debug.Log("全ての空間にパーツを配置しました");
                break;
            }

            CaliculateEntropy();
            if(isError == true)
            {
                Debug.Log("矛盾が起きて終了");
                break;
            }
        }
    }
    void GeneratePartsWFC(DecompositionCubeDate generateParts,int minEntropyIndex,GameObject part)
    {
        int x = minEntropyIndex % outputSize_x;
        int y = minEntropyIndex / (outputSize_x*outputSize_z);
        int z = (minEntropyIndex - x - y*outputSize_x*outputSize_z) / outputSize_x;

        if(decompositionCubeDateList.IndexOf(generateParts) == decompositionCubeDateList.Count-1)
        {
            for (int l = 0; l < decompositionCubeDateList.Count; l++)
            {
                entropyBoolList[x+z*outputSize_x+y*outputSize_x*outputSize_z][l] = false;
            }

            GameObject air = new GameObject("air");
            air.transform.parent = part.transform;
            air.transform.position = new Vector3(x,y,z);
            cubesWFC.Add(air);
            cubeTypeListWFC[x,y,z] = 0;
            return;
        }

        for(int k=0; k < N; k++)
        {
            for(int j=0; j < N; j++)
            {
                for(int i=0; i < N; i++)
                {
                    if(generateParts.decompositionCubeArray[i,j,k] == 1)
                    {
                        for (int l = 0; l < decompositionCubeDateList.Count; l++)
                        {
                            entropyBoolList[x+i+(z+k)*outputSize_x+(y+j)*outputSize_x*outputSize_z][l] = false;
                        }

                        GameObject cube = Instantiate(cubePrefab,new Vector3(x+i,y+j,z+k),Quaternion.identity);
                        cube.GetComponent<Renderer>().material.color = generateParts.color;
                        cube.transform.parent = part.transform;
                        cubesWFC.Add(cube);
                        cubeTypeListWFC[x+i,y+j,z+k] = 1;
                    }
                    else if(generateParts.decompositionCubeArray[i,j,k] == 0)
                    {
                        for (int l = 0; l < decompositionCubeDateList.Count; l++)
                        {
                            entropyBoolList[x+i+(z+k)*outputSize_x+(y+j)*outputSize_x*outputSize_z][l] = false;   
                        }

                        GameObject air = new GameObject("air");
                        air.transform.parent = part.transform;
                        air.transform.position = new Vector3(x+i,y+j,z+k);
                        cubesWFC.Add(air);
                        cubeTypeListWFC[x+i,y+j,z+k] = 0;
                    }
                    else
                    {
                        GameObject air = new GameObject("ExceptionAir");
                        air.transform.parent = part.transform;
                        air.transform.position = new Vector3(x+i,y+j,z+k);
                        cubesWFC.Add(air);
                    }
                }
            }
        }
    }
    void SelectGeneratePartInMinEntropy()//最小のエントロピーの中からランダムでパーツを選ぶ
    {
        List<int> canGeneratePartsList = new List<int>();
        int nowCanGeneratePartsNum = 0;
        foreach(bool canGenerateParts in entropyBoolList[minEntropyIndex])
        {

            if(canGenerateParts == true)
            {
                canGeneratePartsList.Add(nowCanGeneratePartsNum);
            }
            nowCanGeneratePartsNum++;
        }
        if(canGeneratePartsList.Count == 0)
        {
            isError = true;
            Debug.Log("矛盾が起きた");
            return;
        }
        generatePart = canGeneratePartsList[Random.Range(0,canGeneratePartsList.Count)];

        int x = minEntropyIndex % outputSize_x;
        int y = minEntropyIndex / (outputSize_x*outputSize_z);
        int z = (minEntropyIndex - x - y*outputSize_x*outputSize_z) / outputSize_x;

        if(generatePart == decompositionCubeDateList.Count-1)
        {
            if(cubeTypeListWFC[x,y,z] != -1)
            {
                Debug.Log("x:"+x+" y:"+y+" z:"+z);
                entropyBoolList[minEntropyIndex][generatePart] = false;
                CaliculateEntropy();
            }
        }
        else
        {
            for(int k=0;k < N;k++)//そのパーツがおけるか確認する
            {
                for(int j=0;j < N;j++)
                {
                    for(int i=0;i < N;i++)
                    {
                        if(generatePart != decompositionCubeDateList.Count-1)
                        {
                            if(decompositionCubeDateList[generatePart].decompositionCubeArray[i,j,k] == 0 || decompositionCubeDateList[generatePart].decompositionCubeArray[i,j,k] == 1)
                            {
                                if(x+i >= outputSize_x || y+j >= outputSize_y || z+k >= outputSize_z)
                                {
                                    generatePart = decompositionCubeDateList.Count-1;
                                }
                                else if(cubeTypeListWFC[x+i,y+j,z+k] != -1 || entropyBoolList[x+i+(z+k)*outputSize_x+(y+j)*outputSize_x*outputSize_z][generatePart] == false)
                                {
                                    entropyBoolList[minEntropyIndex][generatePart] = false;
                                    CaliculateEntropy();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    void CaliculateEntropy()//エントロピーを計算する関数
    {
        bool isFirstCaliculate = false;
        minEntropy = 0;
        foreach(bool[] entropyBool in entropyBoolList)
        {
            float nowEntropy = 0;
            float sumEntropyWeight = 0;
            foreach(bool canConnection in entropyBool)
            {
                if(canConnection == true)
                {
                    sumEntropyWeight++;
                }
            }

            if(sumEntropyWeight == 0)
            {
                continue;
            }

            foreach(bool canConnection in entropyBool)
            {
                if(canConnection == true)
                {
                    nowEntropy += -1*(1/sumEntropyWeight)*Mathf.Log(1/sumEntropyWeight);
                }
            }
            if(isFirstCaliculate == false)
            {
                minEntropy = nowEntropy;
                minEntropyIndex = entropyBoolList.IndexOf(entropyBool);
                isFirstCaliculate = true;
            }
            else if(isFirstCaliculate == true)
            {
                if(nowEntropy < minEntropy)
                {
                    minEntropy = nowEntropy;
                    minEntropyIndex = entropyBoolList.IndexOf(entropyBool);
                }
            }
        }

        if(isFirstCaliculate == false)
        {
            isError = true;
            Debug.Log("矛盾が起きた");
            return;
        }
        SelectGeneratePartInMinEntropy();
    }
    void EntropyBoolListUpdate(int x, int y, int z, DecompositionCubeDate generateParts) //エントロピーを格納するリストを更新する関数
    {
        foreach(FrontConnectionInformation frontConnectionInformation in generateParts.frontConnectionInformation)//エントロピーを更新するための接続情報を格納するリストを更新する
        {
            ConnectionParts newConnectionParts = new ConnectionParts();
            Vector3Int position = new Vector3Int(x+frontConnectionInformation.x-frontConnectionInformation.connectionX,y+frontConnectionInformation.y-frontConnectionInformation.connectionY,z+frontConnectionInformation.z-frontConnectionInformation.connectionZ-1);
            if(position.x >= 0 && position.x < outputSize_x && position.y >= 0 && position.y < outputSize_y && position.z >= 0 && position.z < outputSize_z)
            {
                if(connectionPartsList==null || connectionPartsList.Any(connectionParts => connectionParts.position == position) == false)
                {
                    newConnectionParts.position = position;
                    newConnectionParts.partNum.Add(frontConnectionInformation.part);
                    connectionPartsList.Add(newConnectionParts);
                }
                else if(connectionPartsList.Any(connectionParts => connectionParts.position == position) == true)
                {
                    connectionPartsList.Find(connectionParts => connectionParts.position == position).partNum.Add(frontConnectionInformation.part);
                }
            }
        }

        foreach(BackConnectionInformation backConnectionInformation in generateParts.backConnectionInformation)//backの処理
        {
            ConnectionParts newConnectionParts = new ConnectionParts();
            Vector3Int position = new Vector3Int(x+backConnectionInformation.x-backConnectionInformation.connectionX,y+backConnectionInformation.y-backConnectionInformation.connectionY,z+backConnectionInformation.z-backConnectionInformation.connectionZ+1);
            if(x+backConnectionInformation.x-backConnectionInformation.connectionX >= 0 && x+backConnectionInformation.x-backConnectionInformation.connectionX < outputSize_x && y+backConnectionInformation.y-backConnectionInformation.connectionY >= 0 && y+backConnectionInformation.y-backConnectionInformation.connectionY < outputSize_y && z+backConnectionInformation.z-backConnectionInformation.connectionZ+1 >= 0 && z+backConnectionInformation.z-backConnectionInformation.connectionZ+1 < outputSize_z)
            {
                if(connectionPartsList==null || connectionPartsList.Any(connectionParts => connectionParts.position == position) == false)
                {
                    newConnectionParts.position = position;
                    newConnectionParts.partNum.Add(backConnectionInformation.part);
                    connectionPartsList.Add(newConnectionParts);
                }
                else if(connectionPartsList.Any(connectionParts => connectionParts.position == position) == true)
                {
                    connectionPartsList.Find(connectionParts => connectionParts.position == position).partNum.Add(backConnectionInformation.part);
                }
            }
        }

        foreach(RightConnectionInformation rightConnectionInformation in generateParts.rightConnectionInformation)//rightの処理
        {
            ConnectionParts newConnectionParts = new ConnectionParts();
            Vector3Int position = new Vector3Int(x+rightConnectionInformation.x+1-rightConnectionInformation.connectionX,y+rightConnectionInformation.y-rightConnectionInformation.connectionY,z+rightConnectionInformation.z-rightConnectionInformation.connectionZ);
            if(x+rightConnectionInformation.x+1-rightConnectionInformation.connectionX >= 0 && x+rightConnectionInformation.x+1-rightConnectionInformation.connectionX < outputSize_x && y+rightConnectionInformation.y-rightConnectionInformation.connectionY >= 0 && y+rightConnectionInformation.y-rightConnectionInformation.connectionY < outputSize_y && z+rightConnectionInformation.z-rightConnectionInformation.connectionZ >= 0 && z+rightConnectionInformation.z-rightConnectionInformation.connectionZ < outputSize_z)
            {
                if(connectionPartsList==null || connectionPartsList.Any(connectionParts => connectionParts.position == position) == false)
                {
                    newConnectionParts.position = position;
                    newConnectionParts.partNum.Add(rightConnectionInformation.part);
                    connectionPartsList.Add(newConnectionParts);
                }
                else if(connectionPartsList.Any(connectionParts => connectionParts.position == position) == true)
                {
                    connectionPartsList.Find(connectionParts => connectionParts.position == position).partNum.Add(rightConnectionInformation.part);
                }
            }
        }

        foreach(LeftConnectionInformation leftConnectionInformation in generateParts.leftConnectionInformation)//leftの処理
        {
            ConnectionParts newConnectionParts = new ConnectionParts();
            Vector3Int position = new Vector3Int(x+leftConnectionInformation.x-1-leftConnectionInformation.connectionX,y+leftConnectionInformation.y-leftConnectionInformation.connectionY,z+leftConnectionInformation.z-leftConnectionInformation.connectionZ);
            if(x+leftConnectionInformation.x-1-leftConnectionInformation.connectionX >= 0 && x+leftConnectionInformation.x-1-leftConnectionInformation.connectionX < outputSize_x && y+leftConnectionInformation.y-leftConnectionInformation.connectionY >= 0 && y+leftConnectionInformation.y-leftConnectionInformation.connectionY < outputSize_y && z+leftConnectionInformation.z-leftConnectionInformation.connectionZ >= 0 && z+leftConnectionInformation.z-leftConnectionInformation.connectionZ < outputSize_z)
            {
                if(connectionPartsList==null || connectionPartsList.Any(connectionParts => connectionParts.position == position) == false)
                {
                    newConnectionParts.position = position;
                    newConnectionParts.partNum.Add(leftConnectionInformation.part);
                    connectionPartsList.Add(newConnectionParts);
                }
                else if(connectionPartsList.Any(connectionParts => connectionParts.position == position) == true)
                {
                    connectionPartsList.Find(connectionParts => connectionParts.position == position).partNum.Add(leftConnectionInformation.part);
                }
            }
        }

        foreach(UpConnectionInformation upConnectionInformation in generateParts.upConnectionInformation)//upの処理
        {
            ConnectionParts newConnectionParts = new ConnectionParts();
            Vector3Int position = new Vector3Int(x+upConnectionInformation.x-upConnectionInformation.connectionX,y+upConnectionInformation.y+1-upConnectionInformation.connectionY,z+upConnectionInformation.z-upConnectionInformation.connectionZ);
            if(x+upConnectionInformation.x-upConnectionInformation.connectionX >= 0 && x+upConnectionInformation.x-upConnectionInformation.connectionX < outputSize_x && y+upConnectionInformation.y+1-upConnectionInformation.connectionY >= 0 && y+upConnectionInformation.y+1-upConnectionInformation.connectionY < outputSize_y && z+upConnectionInformation.z-upConnectionInformation.connectionZ >= 0 && z+upConnectionInformation.z-upConnectionInformation.connectionZ < outputSize_z)
            {
                if(connectionPartsList==null || connectionPartsList.Any(connectionParts => connectionParts.position == position) == false)
                {
                    newConnectionParts.position = position;
                    newConnectionParts.partNum.Add(upConnectionInformation.part);
                    connectionPartsList.Add(newConnectionParts);
                }
                else if(connectionPartsList.Any(connectionParts => connectionParts.position == position) == true)
                {
                    connectionPartsList.Find(connectionParts => connectionParts.position == position).partNum.Add(upConnectionInformation.part);
                }
            }
        }

        foreach(DownConnectionInformation downConnectionInformation in generateParts.downConnectionInformation)//downの処理
        {
            ConnectionParts newConnectionParts = new ConnectionParts();
            Vector3Int position = new Vector3Int(x+downConnectionInformation.x-downConnectionInformation.connectionX,y+downConnectionInformation.y-1-downConnectionInformation.connectionY,z+downConnectionInformation.z-downConnectionInformation.connectionZ);
            if(x+downConnectionInformation.x-downConnectionInformation.connectionX >= 0 && x+downConnectionInformation.x-downConnectionInformation.connectionX < outputSize_x && y+downConnectionInformation.y-1-downConnectionInformation.connectionY >= 0 && y+downConnectionInformation.y-1-downConnectionInformation.connectionY < outputSize_y && z+downConnectionInformation.z-downConnectionInformation.connectionZ >= 0 && z+downConnectionInformation.z-downConnectionInformation.connectionZ < outputSize_z)
            {
                if(connectionPartsList==null || connectionPartsList.Any(connectionParts => connectionParts.position == position) == false)
                {
                    newConnectionParts.position = position;
                    newConnectionParts.partNum.Add(downConnectionInformation.part);
                    connectionPartsList.Add(newConnectionParts);
                }
                else if(connectionPartsList.Any(connectionParts => connectionParts.position == position) == true)
                {
                    connectionPartsList.Find(connectionParts => connectionParts.position == position).partNum.Add(downConnectionInformation.part);
                }
            }
        }

        foreach(ConnectionParts connectionParts in connectionPartsList)//エントロピーを格納するリストを更新する
        {
            int j = 0;
            bool[] previousEntropyBool = entropyBoolList[connectionParts.position.x+connectionParts.position.z*outputSize_x+connectionParts.position.y*outputSize_x*outputSize_z];
            bool[] connectionEntropyBool = new bool[decompositionCubeDateList.Count];
            bool[] updateEntropyBool = new bool[decompositionCubeDateList.Count];
            foreach(int partNum in connectionParts.partNum)
            {
                connectionEntropyBool[partNum] = true;
                j = partNum;
            }
            if(previousEntropyBool.Contains(true) == false)
            {
                // Debug.Log("生成済み")
                continue;
            }
            else if(previousEntropyBool.Contains(false))
            {
                // Debug.Log("bool配列の合成");
                for(int i=0; i < decompositionCubeDateList.Count; i++)
                {
                    if(previousEntropyBool[i] == true && connectionEntropyBool[i] == true)
                    {
                        updateEntropyBool[i] = true;
                    }
                    else
                    {
                        updateEntropyBool[i] = false;
                    }
                }
            }
            else
            {
                updateEntropyBool = connectionEntropyBool;
            }

            if(updateEntropyBool.Contains(true) == false)
            {
                if(cubeNumListWFC[connectionParts.position.x,connectionParts.position.y,connectionParts.position.z] == -1)
                {
                    isError = true;
                    return;
                }
            }
            entropyBoolList[connectionParts.position.x+connectionParts.position.z*outputSize_x+connectionParts.position.y*outputSize_x*outputSize_z] = updateEntropyBool;
        }
    }
    void EntropyBoolListAdd() //エントロピーを格納するリストを初期化する関数
    {
        for(int j=0; j < outputSize_x*outputSize_y*outputSize_z; j++)
        {
            bool[] entropyBool = new bool[decompositionCubeDateList.Count];
            for(int i=0; i < decompositionCubeDateList.Count; i++)
            {
                entropyBool[i] = true;
            }
            entropyBoolList.Add(entropyBool);
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
            bool isAirDateInsert = false;
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
                            frontConnectionInformation.part = partsList.Count;
                            nowDecompositionCubeDate.frontConnectionInformation.Add(frontConnectionInformation);

                            if(isAirDateInsert == false)
                            {
                                BackConnectionInformation airBackConnectionInformation = new BackConnectionInformation();
                                airBackConnectionInformation.part = partsList.IndexOf(parts);
                                airDate.backConnectionInformation.Add(airBackConnectionInformation);
                                isAirDateInsert = true;
                            }
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
                                    frontConnectionInformation.part = partsList.Count;
                                    nowDecompositionCubeDate.frontConnectionInformation.Add(frontConnectionInformation);
                                    if(isAirDateInsert==false)
                                    {
                                        BackConnectionInformation airBackConnectionInformation = new BackConnectionInformation();
                                        airBackConnectionInformation.part = partsList.IndexOf(parts);
                                        airDate.backConnectionInformation.Add(airBackConnectionInformation);
                                        isAirDateInsert = true;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                nowX = 0;
            }
            isAirDateInsert = false;
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
                        }
                        else
                        {
                            backConnectionInformation.part = partsList.Count;
                            nowDecompositionCubeDate.backConnectionInformation.Add(backConnectionInformation);
                            if(isAirDateInsert == false)
                            {
                                FrontConnectionInformation airFrontConnectionInformation = new FrontConnectionInformation();
                                airFrontConnectionInformation.part = partsList.IndexOf(parts);
                                airDate.frontConnectionInformation.Add(airFrontConnectionInformation);
                                isAirDateInsert = true;
                            }
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
                                    break;
                                }
                                else
                                {
                                    backConnectionInformation.part = partsList.Count;
                                    nowDecompositionCubeDate.backConnectionInformation.Add(backConnectionInformation);
                                    if(isAirDateInsert == false)
                                    {
                                        FrontConnectionInformation airFrontConnectionInformation = new FrontConnectionInformation();
                                        airFrontConnectionInformation.part = partsList.IndexOf(parts);
                                        airDate.frontConnectionInformation.Add(airFrontConnectionInformation);
                                        isAirDateInsert = true;
                                    }
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
            isAirDateInsert = false;
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
                            rightConnectionInformation.part = partsList.Count;
                            nowDecompositionCubeDate.rightConnectionInformation.Add(rightConnectionInformation);
                            if(isAirDateInsert == false)
                            {
                                LeftConnectionInformation airLeftConnectionInformation = new LeftConnectionInformation();
                                airLeftConnectionInformation.part = partsList.IndexOf(parts);
                                airDate.leftConnectionInformation.Add(airLeftConnectionInformation);
                                isAirDateInsert = true;
                            }
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
                                    rightConnectionInformation.part = partsList.Count;
                                    nowDecompositionCubeDate.rightConnectionInformation.Add(rightConnectionInformation);
                                    if(isAirDateInsert == false)
                                    {
                                        LeftConnectionInformation airLeftConnectionInformation = new LeftConnectionInformation();
                                        airLeftConnectionInformation.part = partsList.IndexOf(parts);
                                        airDate.leftConnectionInformation.Add(airLeftConnectionInformation);
                                        isAirDateInsert = true;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                nowZ = 0;
            }
            isAirDateInsert = false;
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
                        }
                        else
                        {
                            leftConnectionInformation.part = partsList.Count;
                            nowDecompositionCubeDate.leftConnectionInformation.Add(leftConnectionInformation);
                            if(isAirDateInsert == false)
                            {
                                RightConnectionInformation airRightConnectionInformation = new RightConnectionInformation();
                                airRightConnectionInformation.part = partsList.IndexOf(parts);
                                airDate.rightConnectionInformation.Add(airRightConnectionInformation);
                                isAirDateInsert = true;
                            }
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
                                    break;
                                }
                                else
                                {
                                    leftConnectionInformation.part = partsList.Count;
                                    nowDecompositionCubeDate.leftConnectionInformation.Add(leftConnectionInformation);
                                    if(isAirDateInsert == false)
                                    {
                                        RightConnectionInformation airRightConnectionInformation = new RightConnectionInformation();
                                        airRightConnectionInformation.part = partsList.IndexOf(parts);
                                        airDate.rightConnectionInformation.Add(airRightConnectionInformation);
                                        isAirDateInsert = true;
                                    }
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
            isAirDateInsert = false;
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
                            upConnectionInformation.part = partsList.Count;
                            nowDecompositionCubeDate.upConnectionInformation.Add(upConnectionInformation);
                            if(isAirDateInsert == false)
                            {
                                DownConnectionInformation airDownConnectionInformation = new DownConnectionInformation();
                                airDownConnectionInformation.part = partsList.IndexOf(parts);
                                airDate.downConnectionInformation.Add(airDownConnectionInformation);
                                isAirDateInsert = true;
                            }
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
                                    upConnectionInformation.part = partsList.Count;
                                    nowDecompositionCubeDate.upConnectionInformation.Add(upConnectionInformation);
                                    if(isAirDateInsert == false)
                                    {
                                        DownConnectionInformation airDownConnectionInformation = new DownConnectionInformation();
                                        airDownConnectionInformation.part = partsList.IndexOf(parts);
                                        airDate.downConnectionInformation.Add(airDownConnectionInformation);
                                        isAirDateInsert = true;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                nowX = 0;
            }
            isAirDateInsert = false;
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
                            break;
                        }
                        else if(cubeBoolList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y-1,(int)partsCube.transform.position.z])
                        {
                            downConnectionInformation.part = partsList.IndexOf(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y-1,(int)partsCube.transform.position.z]].transform.parent.gameObject);
                            nowDecompositionCubeDate.canConnectionNum++;

                            connectionCubePosNum = GetChildIndex(cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y-1,(int)partsCube.transform.position.z]].transform.parent,cubes[cubeNumList[(int)partsCube.transform.position.x,(int)partsCube.transform.position.y-1,(int)partsCube.transform.position.z]]);
                            CaliculateConnectionCubePos(ref downConnectionInformation.connectionX,ref downConnectionInformation.connectionY,ref downConnectionInformation.connectionZ,connectionCubePosNum,N);

                            nowDecompositionCubeDate.downConnectionInformation.Add(downConnectionInformation);
                        }
                        else
                        {
                            downConnectionInformation.part = partsList.Count;
                            nowDecompositionCubeDate.downConnectionInformation.Add(downConnectionInformation);
                            if(isAirDateInsert==false)
                            {
                                UpConnectionInformation airUpConnectionInformation = new UpConnectionInformation();
                                airUpConnectionInformation.part = partsList.IndexOf(parts);
                                airDate.upConnectionInformation.Add(airUpConnectionInformation);
                                isAirDateInsert = true;
                            }
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
                                    break;
                                }
                                else
                                {
                                    downConnectionInformation.part = partsList.Count;
                                    nowDecompositionCubeDate.downConnectionInformation.Add(downConnectionInformation);
                                    if(isAirDateInsert == false)
                                    {
                                        UpConnectionInformation airUpConnectionInformation = new UpConnectionInformation();
                                        airUpConnectionInformation.part = partsList.IndexOf(parts);
                                        airDate.upConnectionInformation.Add(airUpConnectionInformation);
                                        isAirDateInsert = true;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                nowX = 0;
            }
            isAirDateInsert = false;
        }

        FrontConnectionInformation airFrontAirConnectionInformation = new FrontConnectionInformation();
        airFrontAirConnectionInformation.part = partsList.Count;
        airDate.frontConnectionInformation.Add(airFrontAirConnectionInformation);
        BackConnectionInformation airBackAirConnectionInformation = new BackConnectionInformation();
        airBackAirConnectionInformation.part = partsList.Count;
        airDate.backConnectionInformation.Add(airBackAirConnectionInformation);
        RightConnectionInformation airRightAirConnectionInformation = new RightConnectionInformation();
        airRightAirConnectionInformation.part = partsList.Count;
        airDate.rightConnectionInformation.Add(airRightAirConnectionInformation);
        LeftConnectionInformation airLeftAirConnectionInformation = new LeftConnectionInformation();
        airLeftAirConnectionInformation.part = partsList.Count;
        airDate.leftConnectionInformation.Add(airLeftAirConnectionInformation);
        UpConnectionInformation airUpAirConnectionInformation = new UpConnectionInformation();
        airUpAirConnectionInformation.part = partsList.Count;
        airDate.upConnectionInformation.Add(airUpAirConnectionInformation);
        DownConnectionInformation airDownAirConnectionInformation = new DownConnectionInformation();
        airDownAirConnectionInformation.part = partsList.Count;
        airDate.downConnectionInformation.Add(airDownAirConnectionInformation);
        decompositionCubeDateList.Add(airDate);
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
