using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public delegate void MoveSideways(int lr);
    public static event MoveSideways OnMoveSideways;

    public delegate void rotateTetr();
    public static event rotateTetr OnRotateTetr;

    public delegate void IncreaseSpeed(bool s);
    public static event IncreaseSpeed OnIncreaseSpeed;

    public delegate void Harddrop();
    public static event Harddrop OnHarddrop;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(OnMoveTetr != null){
                OnMoveTetr(-1);
            }
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(OnMoveTetr != null){
                OnMoveTetr(1);
            }
        }*/

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(OnMoveSideways != null)
                OnMoveSideways(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(OnMoveSideways != null)
                OnMoveSideways(1);
        }

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(OnRotateTetr != null){
                OnRotateTetr();
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(OnHarddrop != null){
                OnHarddrop();
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(OnIncreaseSpeed != null) 
                OnIncreaseSpeed(true);
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if(OnIncreaseSpeed != null) 
                OnIncreaseSpeed(false);
        }
    }
}
