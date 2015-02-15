using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    protected float minY = -.25f,
                    maxY = 1.31f,
                    minX = 0,
                    maxX = 15,
                    cameraScrollAmount = 1f,
                    horizontalScrollAmount = 1.75f,
                    currentMinX = 0,
                    currentMaxX = 0;

    [SerializeField] protected GameObject player;

    bool cameraScrollLocked = false;

    Camera thisCamera;
    SafeCoroutine cameraMovementOverride;

    static CameraControl instance;

    public static CameraControl Instance{
        get { return instance; }
    }

    public void LockMinAndMaxX(float targetX){
        cameraScrollLocked = true;
        cameraMovementOverride = this.StartSafeCoroutine(CenterCameraOnFight(targetX));
    }

    public void UnlockMinAndMaxX(){
        cameraScrollLocked = false;
        if (cameraMovementOverride.IsRunning){
            cameraMovementOverride.Stop();
        }
    }

    void Awake(){
        instance = this;
    }

	void Start () {
        minX = GameObject.Find("MinXBorder").transform.position.x;
        maxX = GameObject.Find("MaxXBorder").transform.position.x;
        currentMinX = minX;
        currentMaxX = maxX;
        GameObject.Destroy(GameObject.Find("MinXBorder"));
        GameObject.Destroy(GameObject.Find("MaxXBorder"));

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

        if (!cameraScrollLocked){
            // Move camera right
            if (thisCamera.WorldToScreenPoint(pos).x > (Screen.width * .8f) && thisPos.x < currentMaxX){
                thisPos.x += Time.deltaTime * horizontalScrollAmount;
            }
            // Move camera left
            else if (thisCamera.WorldToScreenPoint(pos).x < (Screen.width * .2f) && thisPos.x > currentMinX){
                thisPos.x -= Time.deltaTime * horizontalScrollAmount;
            }
        }

        transform.position = thisPos;
	}

    IEnumerator CenterCameraOnFight(float targetX){
        float scroll = 2f;
        var pos = transform.position;

        if (targetX < transform.position.x){
            while (targetX < transform.position.x){
                pos.x -= scroll * Time.deltaTime;
                transform.position = pos;
                yield return null;
            }
        }
        else if (targetX > transform.position.x){
            while (targetX > transform.position.x){
                pos.x += scroll * Time.deltaTime;
                transform.position = pos;
                yield return null;
            }
        }
    }

}
