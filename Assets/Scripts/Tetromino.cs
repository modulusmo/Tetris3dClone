using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TetrominoType
{
    I, J, L, O, S, T, Z
};

public class Tetromino : MonoBehaviour
{
    /*void OnEnable()
    {
        InputManager.OnMoveTetr += InputManager_OnMoveTetr;
        InputManager.OnRotateTetr += InputManager_OnRotateTetr;
    }

    void OnDisable()
    {
        InputManager.OnMoveTetr -= InputManager_OnMoveTetr;
        InputManager.OnRotateTetr -= InputManager_OnRotateTetr;
    }

    void InputManager_OnMoveTetr(int input)
    {
        transform.position = new Vector3(
            transform.position.x + input, 
            transform.position.y,
            transform.position.z
        );
    }

    void InputManager_OnRotateTetr()
    {
        transform.Rotate(new Vector3(0,1,0),90);
    }
    
    public delegate void HitCubes();
    public static event HitCubes OnHitCubes;
    */


    public TetrominoType Type;
 
    [Tooltip("Used for translation")]
    public GameObject Root;
 
    [Tooltip("Used for rotation")]
    public GameObject Pivot;
    
    bool moveLeft = true;
    bool moveRight =true;
 
    [SerializeField]
    protected GameObject[] cubes = new GameObject[4];

    /*public delegate void MoveLeft();
    public delegate void MoveRight();

    public static event MoveLeft OnMoveLeft;
    public static event MoveRight OnMoveRight; */
    
 
    // Start is called before the first frame update
    void Start()
    {
        
    }
 
    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(OnMoveLeft != null)
                OnMoveLeft();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(OnMoveRight != null)
                OnMoveRight();
        }
 
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Pivot.transform.Rotate(Vector3.forward, -90);
        }
        */
    }

    public int[] GetCubeCoords()//returns x and y values of coordinates of cubes
    {
        // find x and y coordinates of the cubes
        int [] cubePos = new int [8];
        Transform tempCube;

        for(int i=0; i<8; i=i+2){
            tempCube = transform.Find(string.Format("axis/Cube{0}",(i/2)));
            cubePos[i] = Mathf.RoundToInt(tempCube.position.x);
            cubePos[i+1]=Mathf.RoundToInt(tempCube.position.y);
            //Debug.Log(tempCube.name +": "+cubePos[i]+", "+cubePos[i+1]);
        }
        //Debug.Log("-----------------------");
        return cubePos;
    }

    public int[] GetRotationData()
    {
        float xPivot = Pivot.transform.position.x;
        float yPivot = Pivot.transform.position.y;
        int [] rotationCoords = new int[8];
        float x,y;
        rotationCoords = GetCubeCoords();//inherited from Tetromino
        for(int i=0; i<8; i=i+2){
            //do vector manip
            x = (float) rotationCoords[i];
            y = (float) rotationCoords[i+1];
            Debug.Log("RotationCoordsBefore: "+rotationCoords[i]+", "+rotationCoords[i+1]);
            x = x - xPivot;
            y = y - yPivot;
            x =  y + xPivot;// normally, = xcos(-90) + ysin(-90)
            y = -x + yPivot;// but cos(-90) = 0 and sin(-90) = -1
            rotationCoords[i] = Mathf.RoundToInt(x);
            rotationCoords[i+1]= Mathf.RoundToInt(y);
            Debug.Log("RotationCoordsAfter: "+rotationCoords[i]+", "+rotationCoords[i+1]);
        }
        Debug.Log("------------------------------");
        return rotationCoords;
    }

    public void RotateTetr()
    {
        Pivot.transform.Rotate(Vector3.forward, -90);
    }

/*   void OnCollisionEnter(Collision coll)
    {
        Debug.Log("Tetromino has collided");
        if(coll.gameObject.name == "LeftSide"){
            moveLeft=false;
        }
        else if(coll.gameObject.name == "RightSide"){
            moveRight=false;
        }

        //when rotating near a wall
        if(coll.gameObject.name == "LeftPush"){
            transform.position = new Vector3(
                    transform.position.x + 1,
                    transform.position.y,
                    transform.position.z
                );
        }
        else if(coll.gameObject.name == "RightPush"){
            transform.position = new Vector3(
                    transform.position.x - 1,
                    transform.position.y,
                    transform.position.z
                );
        }

        if(coll.gameObject.CompareTag("Cube"))
        {
            if(OnHitCubes != null)
            {
                transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y + 1,
                    transform.position.z
                );
                OnHitCubes();
            }
        }
    }
    void OnCollisionExit(Collision coll)
    {
        if(coll.gameObject.name == "LeftSide"){
            moveLeft=true;
        }
        else if(coll.gameObject.name == "RightSide"){
            moveRight=true;
        }
    }*/
}
