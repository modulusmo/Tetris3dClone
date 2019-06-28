using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[] TetrominoPrefabs;
 
    public GameObject currentTetrominoFalling = null;
    public GameObject nextTetromino = null;

    public Text txtOccupiedTable;
 
    public float waitTime = 1.0f;
    public float coolOffTime;

    bool isMoving, gameIsRunning, hardDrop;
    int dropY = 0; //for hard Drop
    int gameScore = 0;
    int levelThreshhold = 200;
    int level=1;
    bool [] newArr;//for filling occupied list
    List<bool []> occupied = new List<bool[]>();
    List<GameObject> cubeRow = new List<GameObject>();

    public delegate void LevelChange(int l);
    public static event LevelChange OnLevelChange;
    public delegate void ScoreChange(int s);
    public static event ScoreChange OnScoreChange;


    void OnEnable()
    {
        InputManager.OnIncreaseSpeed += InputManager_OnIncreaseSpeed;
        InputManager.OnMoveSideways += InputManager_OnMoveSideways;
        InputManager.OnRotateTetr += InputManager_OnRotateTetr;
        InputManager.OnHarddrop += InputManager_OnHarddrop;
        UIManager.OnResetGame += UIManager_OnResetGame;
        //Tetromino.OnHitCubes += Tetromino_OnHitCubes;
    }

    void OnDisable()
    {
        InputManager.OnIncreaseSpeed -= InputManager_OnIncreaseSpeed;
        InputManager.OnMoveSideways -= InputManager_OnMoveSideways;
        InputManager.OnRotateTetr -= InputManager_OnRotateTetr;
        InputManager.OnHarddrop -= InputManager_OnHarddrop;
        UIManager.OnResetGame -= UIManager_OnResetGame;
        //Tetromino.OnHitCubes -= Tetromino_OnHitCubes;
    }
    // Start is called before the first frame update
    void Start()
    {
        gameIsRunning=false;//prevent tetromino from being created
        hardDrop=false;
        nextTetromino =
            GameObject.Instantiate(
                TetrominoPrefabs[Random.Range(0, TetrominoPrefabs.Length)],
                new Vector3(15, 10, 0),
                Quaternion.identity) as GameObject;
        PrimeGame();
    }
 
    // Update is called once per frame
    void Update()
    {
        if(coolOffTime < Time.time && gameIsRunning)
        {
            // instantiate a new Tetromino and move it down ...
            if (currentTetrominoFalling == null)
            {
                currentTetrominoFalling = nextTetromino;
                currentTetrominoFalling.transform.position = new Vector3(5,20,0);
                currentTetrominoFalling.transform.rotation = Quaternion.identity; // set straight
                nextTetromino =
                    GameObject.Instantiate(
                        TetrominoPrefabs[Random.Range(0, TetrominoPrefabs.Length)],
                        new Vector3(17, 8, 0),
                        Quaternion.identity) as GameObject;
                nextTetromino.transform.Rotate(new Vector3(0,1,0), -15);
                //Debug.Log(currentTetrominoFalling.GetComponent<ITetromino>().GetType());
            }
            else //if (currentTetrominoFalling != null)
            {
                if(hardDrop){
                    while(SpaceIsFree(dropY+1)){//check row below
                        dropY++;
                    }
                    currentTetrominoFalling.transform.position = new Vector3(
                        currentTetrominoFalling.transform.position.x,
                        currentTetrominoFalling.transform.position.y - dropY,
                        currentTetrominoFalling.transform.position.z
                    );
                    dropY = 0;
                    hardDrop = false;
                }
                else if (SpaceIsFree(1)){// check if space is free one row below
                    //currentTetrominoFalling.transform.Translate(Vector3.down);//move down 1
                    currentTetrominoFalling.transform.position = new Vector3(
                        currentTetrominoFalling.transform.position.x,
                        currentTetrominoFalling.transform.position.y - 1,
                        currentTetrominoFalling.transform.position.z
                    );
                }
                else{//reached bottom
                    UpdateOccupiedTable(); // uses coordinates of children
                    //remove cubes from parent before deleting
                    ReleaseCubes();
                    Destroy(currentTetrominoFalling);
                    currentTetrominoFalling = null;
                    ClearCompleteLines();
                    printTable();
                    //isMoving = true;
                }
            }
            coolOffTime = Time.time + waitTime;
        }


    }

    void InputManager_OnIncreaseSpeed(bool increaseSpeed)
    {
        if(increaseSpeed){
            waitTime = waitTime/ 20;
        }
        else{
            waitTime = waitTime * 20;
        }
        //Debug.Log("GameManager.cs : Speed is "+ speed);
    }

    public bool SpaceIsFree(int rowsBelow)
    {
        int [] cubeCoords = new int [8]; // x and y values of 4 cubes: i == x and i+1 == y 
        bool [] tempArr = new bool [12];
        cubeCoords = currentTetrominoFalling.GetComponent<Tetromino>().GetCubeCoords();//find where the cubes are
        for(int i=0; i<8; i=i+2){
            tempArr = occupied[cubeCoords[i+1] - rowsBelow]; // current_row - 1 == row_below
            if(tempArr[cubeCoords[i]]){ // x value (column)
                return false;
            }
        }
        return true;
        
    }

    void UpdateOccupiedTable()
    {
        int [] cubeCoords = new int [8]; // x and y values of 4 cubes: i == x and i+1 == y 
        bool [] tempArr = new bool [12];
        int x,y;
        cubeCoords = currentTetrominoFalling.GetComponent<Tetromino>().GetCubeCoords();
        for(int i=0; i<8; i=i+2){
            x = cubeCoords[i];
            y = cubeCoords[i+1];
            //tempArr = new bool [12]; // create new instance
            tempArr = occupied[y]; // find y index
            tempArr[x] = true; //find x index, set to true (occupied)
            occupied[y] = tempArr; // assign new instance to occupied table
        }
    }

    void ClearCompleteLines()//checks for complete lines and deletes them
    {
        bool [] tempArr = new bool [12];
        int cubesInLineCount = 0;//number of cubes in a given line
        int linesClearedCount= 0;//number of lines cleared at once
        for(int i=1; i<25 && gameIsRunning; i++){ // don't need to check the boundary
            tempArr = occupied[i]; 
            for(int j=1; j<11 && gameIsRunning; j++){ // again, not checking the boundaries
                if(tempArr[j] && i==20){
                    gameIsRunning = false;//end of game if occupied has something at the top
                }
                else if (tempArr[j]){
                    cubesInLineCount++;
                }
            }
            if(cubesInLineCount==10 && gameIsRunning){ // remove cubes at this location
                occupied.RemoveAt(i);
                Destroy(cubeRow[i]); // delete gameobjects in row
                cubeRow.RemoveAt(i); // remove entry from 
                GameObject emptyObj = new GameObject();
                emptyObj.transform.position = new Vector3(13, 24, 0);
                cubeRow.Add(emptyObj);//added to top of board
                tempArr = new bool [12];
                for(int j=0; j<12; j++){ // create a new empty line
                    if(j==0 || j==11){
                        tempArr[j]=true;
                    }else{
                        tempArr[j]=false;
                    }
                }
                occupied.Add(tempArr);
                linesClearedCount++;

                //shift lines down
                for(int k=i; k<25; k++){//start at where the line was cleared
                    cubeRow[k].transform.position = new Vector3(
                        cubeRow[k].transform.position.x,
                        cubeRow[k].transform.position.y - 1,
                        cubeRow[k].transform.position.z
                    );
                }
                i = i-1;
            }
            cubesInLineCount=0;//reset to check next line
        }
        //TODO: calculate points earned using lineClearCount <---- <---- <---- <---- <---- <---- <---- <---- <---- <---- <----
        if (linesClearedCount > 0){
            gameScore += scoreReward(linesClearedCount);
            if(OnScoreChange != null){
                OnScoreChange(gameScore);
            }
        }
        if (gameScore >= levelThreshhold){
            level++;
            levelThreshhold += (levelThreshhold * level);
            waitTime = waitTime * 0.80f;
            if(OnLevelChange!=null)
                OnLevelChange(level);
        }
    }

    int scoreReward(int linesCleared)
    {
        int reward = 0;
        if (linesCleared == 4){
            reward += 200 * level;
        }
        else if(linesCleared == 3){
            reward += 125 * level;
        }
        else if(linesCleared == 2){
            reward += 75 * level;
        }
        else {
            reward += 25 * level;
        }
        return reward;
    }

    void InputManager_OnMoveSideways(int lr)
    {
        if(currentTetrominoFalling!=null){
            int [] cubeCoords = new int [8]; // x and y values of 4 cubes: i == x and i+1 == y 
            bool [] tempArr = new bool [12];
            bool okToMove = true;
            cubeCoords = currentTetrominoFalling.GetComponent<Tetromino>().GetCubeCoords();//find where the cubes are
            for(int i=0; i<8 && okToMove; i=i+2){
                tempArr = occupied[ cubeCoords[i+1] ]; // current_row - 1 == row_below
                if(tempArr[cubeCoords[i] + lr]){ // x value (column)
                    //if the space is occupied
                    okToMove = false;
                }
            }
            if(okToMove){
                currentTetrominoFalling.transform.position = new Vector3(
                    currentTetrominoFalling.transform.position.x + lr,
                    currentTetrominoFalling.transform.position.y,
                    currentTetrominoFalling.transform.position.z
                );
            }
        }
    }

    void printTable()
    {
        txtOccupiedTable.text = "";
        bool [] tempArr = new bool [12];
        for(int i=21; i>-1; i--){
            tempArr = occupied[i]; // find y index
            txtOccupiedTable.text = txtOccupiedTable.text + 
                makeNum(tempArr[0])+ makeNum(tempArr[1])+makeNum(tempArr[2]) +
                makeNum(tempArr[3])+ makeNum(tempArr[4])+makeNum(tempArr[5]) +
                makeNum(tempArr[6])+ makeNum(tempArr[7])+makeNum(tempArr[8]) +
                makeNum(tempArr[9])+makeNum(tempArr[10])+makeNum(tempArr[11]) + "\n";
        }
    }
    string makeNum(bool arg)
    {
        if(arg)
            return "0";
        else
            return "_";
    }

    void InputManager_OnRotateTetr()
    {
        if(currentTetrominoFalling!=null){
            //coolOffTime -= .0002f;//give enough time to test the rotation
            int [] cubeCoords = new int [8]; // x and y values of 4 cubes: i == x and i+1 == y 
            bool [] tempArr = new bool [12];
            bool okToRotate = true;
            cubeCoords = currentTetrominoFalling.GetComponent<Tetromino>().GetRotationData();//find where the cubes are
            for(int i=0; i<8 && okToRotate; i=i+2){
                tempArr = occupied[ cubeCoords[i+1] ]; // row
                if(tempArr[ cubeCoords[i] ]){ // column
                    okToRotate = false;//will break out of for loop if false
                }
                //TODO: try shifting +-1 both x and y <---- <---- <---- <---- <---- <---- <---- <---- <---- <---- <---- <---- <----
            }
            if (okToRotate){
                currentTetrominoFalling.GetComponent<Tetromino>().RotateTetr();
            }
        }
    }

    void InputManager_OnHarddrop()
    {
        hardDrop=true;
    }

    void UIManager_OnResetGame() // reset from pressing button
    {
        RestartGame();
    }

    void ReleaseCubes()
    {
        int row;
        GameObject [] tempCube = new GameObject[4];
        for(int i=0; i<4; i++){
            tempCube[i] = currentTetrominoFalling.transform.Find(string.Format("axis/Cube{0}", i)).gameObject;
        }
        currentTetrominoFalling.transform.Find("axis").transform.DetachChildren();
        for(int i=0; i<4; i++){
            row = Mathf.RoundToInt(tempCube[i].transform.position.y);
            tempCube[i].transform.SetParent(cubeRow[row].transform);//make each cube a child of row
        }

    }

    void RestartGame()
    {
        //destroy all objects
        for(int i=0; i<25; i++){ // always zero because placement shifts when an object is deleted
            occupied.RemoveAt(0);// clear occupied data
            Destroy(cubeRow[0]); // delete gameobjects in row
            cubeRow.RemoveAt(0); // remove entry
        }
        Destroy(currentTetrominoFalling);
        PrimeGame();
    }

    void PrimeGame()
    {
        coolOffTime = 0.0f;
        //isMoving = true; //allows cubes to move
        levelThreshhold = 200;
        gameScore = 0;
        level = 1;
        if(OnLevelChange!=null)
            OnLevelChange(level);
        if(OnScoreChange!=null)
            OnScoreChange(gameScore);

        for(int i=0; i<25; i++){//to avoid out of bounds warnings when spawning tetrominos
            newArr = new bool[12];
            GameObject row = new GameObject();
            row.transform.position = new Vector3(13, i, 0);
            for(int j=0; j<12; j++){
                if(i==0){ // bottom boundary
                    newArr[j] = true;
                }
                else if (j==0 || j==11){ // side boundaries
                    newArr[j]=true;
                }
                else{ // within boundary
                    newArr[j]=false;
                }
            }
            occupied.Add(newArr);//add the line to occupied List
            cubeRow.Add(row);//for actual cubes
        }
        printTable();
        gameIsRunning = true;//start game
    }
}
