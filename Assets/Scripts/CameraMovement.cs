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

    // Update is called once per frame
    public void forcePositionUpdate()
    {
        if(playerRef == null)
            playerRef = worldManager.instance.GetPlayer().transform;

        if(playerRef != null)
        transform.position = new Vector3(playerRef.transform.position.x, playerRef.transform.position.y, transform.position.z);
    }
}
