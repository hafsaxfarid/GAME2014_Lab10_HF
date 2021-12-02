using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Transform player;
    public Transform spawnPoint;

    void Start()
    {
        player.position = spawnPoint.position;
    }
}