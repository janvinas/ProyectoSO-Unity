using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GameCameraMovement : MonoBehaviour
{

    [SerializeField] Transform player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            this.transform.position = new Vector3(player.position.x, player.position.y, this.transform.position.z);
        }
    }
}
