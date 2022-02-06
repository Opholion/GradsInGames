using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    #region CameraRef_Singleton

    public static CameraMovement instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion


    Transform playerRef;
    bool isUpdatingPosition;
    static float CameraCurrentZ;
    bool isOutsideTurns = false;
    private void Start()
    {
        CameraCurrentZ = transform.position.z;
    }
    public void forcePositionUpdate()
    {
        if(playerRef == null && worldManager.instance.GetPlayer() != null)
            playerRef = worldManager.instance.GetPlayer().transform;

        if (playerRef != null)
            isOutsideTurns = true;


    }

    void Update()
    {
        if (!Board.instance.isGameActive()) return;
        if (isOutsideTurns)
        {
            if (playerRef != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, playerRef.transform.position, Time.deltaTime * 8.0f);
                transform.position = new Vector3(transform.position.x, transform.position.y, CameraCurrentZ);

                if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(playerRef.position.x, playerRef.position.y)) < .0001f)
                    isOutsideTurns = false;
            }
        }



    }
}
