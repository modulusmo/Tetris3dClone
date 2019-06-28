using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L : Tetromino, ITetromino
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    TetrominoType ITetromino.GetType()
    {
        return Type;
    }

GameObject ITetromino.GetRoot()
    {
        return Root;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}