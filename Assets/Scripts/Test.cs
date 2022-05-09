using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Test : MonoBehaviour
{



    PlayerInputActions inputAction;
    private Grid tileGrid;

    LevelGenerator level;


    private void Awake()
    {
        inputAction = new PlayerInputActions();

        
    }

    // Start is called before the first frame update
    void Start()
    {


        uint randomSeed = GetRandomSeed();//2896306688;// GetRandomSeed();4073934336,1617799936,2814833152
        inputAction.Mouse.Enable();
        inputAction.Mouse.LeftClick.started += LeftClicked;


    }

    private void LeftClicked(InputAction.CallbackContext obj)
    {

    }

    private uint GetRandomSeed()
    {
        return (uint)Random.Range(1, uint.MaxValue); // 3363111936; 3687977984;//
    }

   


    



}
