using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    [SerializeField] protected float minY = -.035f,
                                     maxY = 1.31f,
                                     minX = 0,
                                     maxX = 15,
                                     cameraScrollAmount = 5f,
                                     horizontalScrollAmount = 20f;

    [SerializeField] protected GameObject player;
    [SerializeField] protected GameObject street;

    Camera thisCamera;

	void Start () {
        thisCamera = GetComponent<Camera>();
	}

	void Update () {
        var pos = player.transform.position;
        var thisPos = transform.position;

        if (thisCamera.WorldToScreenPoint(pos).y < (Screen.height * .3f) && thisPos.y > minY){
            thisPos.y -= Time.deltaTime * cameraScrollAmount;
        }
        else if (thisCamera.WorldToScreenPoint(pos).y > (Screen.height * .35f) && thisPos.y < maxY){
            thisPos.y += Time.deltaTime * cameraScrollAmount;
        }

        if (thisCamera.WorldToScreenPoint(pos).x > (Screen.width * .8f) && thisPos.x < maxX){
            thisPos.x += Time.deltaTime * horizontalScrollAmount;
        }
        else if (thisCamera.WorldToScreenPoint(pos).x < (Screen.width * .2f) && thisPos.x > minX){
            thisPos.x -= Time.deltaTime * horizontalScrollAmount;
        }

        transform.position = thisPos;
	}

}
