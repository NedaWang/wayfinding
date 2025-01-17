﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using System.IO;

public class Motion : MonoBehaviour
{
    GameObject Player;
    Transform Transform;

    public GameObject Cross; // notification for block
    public GameObject Next;
    float Distance = 0.8f;
    bool Forward = true;
    bool Backward = true;
    bool isHeld = false;
    float Timer = 0.5f;
    float SpeedController = 0f;
    public SteamVR_Action_Vector2 touchPadAction;
    void Start()
    {
        Player = GameObject.Find("Player");
        Transform = Player.transform;
    }

    void Update()
    {
        Vector3 Direction = new Vector3(Camera.main.transform.forward.x * Distance,
                                          0f, Camera.main.transform.forward.z * Distance);
        Vector3 Level1 = Player.transform.position + new Vector3(0F, 0.1F, 0F);
        Vector3 Level2 = Player.transform.position + new Vector3(0F, 0.2F, 0F);

        Forward = !Physics.Raycast(Level1, Direction, Direction.magnitude) // Mathf.Sqrt(Distance * Distance * 2)
                 && !Physics.Raycast(Level2, Direction, Direction.magnitude);
        Backward = !Physics.Raycast(Player.transform.position, -Direction, Direction.magnitude)
                 &&!Physics.Raycast(Level1, Direction, -Direction.magnitude) // Mathf.Sqrt(Distance * Distance * 2)
                 && !Physics.Raycast(Level2, Direction, -Direction.magnitude);
        if (!Next.activeSelf)
        {
            Cross.SetActive(!Forward);
        }
        if (Next.activeSelf)
        {
            Cross.SetActive(false);
        }

        Vector2 touchpadValue = touchPadAction.GetAxis(SteamVR_Input_Sources.Any);

        SpeedController += Time.deltaTime;
        if (SteamVR_Actions.default_Teleport.GetStateDown(SteamVR_Input_Sources.Any) && SpeedController >= 0.5f)
        //if (SteamVR_Actions.default_Teleport.GetStateDown(SteamVR_Input_Sources.Any))
        {
            if (touchpadValue.y > 0)
            {
                if (Forward)
                {
                    transform.position += Direction;
                    SpeedController = 0f; //speed control
                    File.AppendAllText(PlayerPrefs.GetString("FilePath"), transform.position.ToString() +" " + System.DateTime.Now.ToString() + "\n");
                }
                else
                {
                    File.AppendAllText(PlayerPrefs.GetString("FilePath"), "forward collision " + " " + System.DateTime.Now.ToString() + "\n");
                    PlayerPrefs.SetInt("Collision", PlayerPrefs.GetInt("Collision") + 1);
                }

                isHeld = true;
            }
            else
            {
                if (Backward)
                {
                    Player.transform.position -= Direction;
                    SpeedController = 0f;
                    File.AppendAllText(PlayerPrefs.GetString("FilePath"), transform.position.ToString() + " " + System.DateTime.Now.ToString() + "\n");
                }
                else
                {
                    File.AppendAllText(PlayerPrefs.GetString("FilePath"), "backward collision" + " " + System.DateTime.Now.ToString() + "\n");
                    PlayerPrefs.SetInt("Collision", PlayerPrefs.GetInt("Collision") + 1);
                }
            }
        }

        if (SteamVR_Actions.default_Teleport.GetStateUp(SteamVR_Input_Sources.Any))
        {
            isHeld = false;
        }

        Timer -= Time.deltaTime;
        if (Timer < 0)
        {
            if (isHeld && Forward)
            {
                transform.position += Direction;

            }
            else if (isHeld)
            {
                print("Collision");
            }
            Timer = 0.5f;
        }
    }
}