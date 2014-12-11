using UnityEngine;
using System.Collections;

public class DisplayFPS : MonoBehaviour
{
	[SerializeField] protected float updateInterval = 0.5F;
	[SerializeField] protected bool enableCounter;

	private float accum   = 0; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval

	void Start()
	{
		if (!enableCounter){
			this.gameObject.SetActive(false);
		}
		timeleft = updateInterval;
	}

	void Update()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		++frames;

		if( timeleft <= 0.0 )
		{
			float fps = accum/frames;
			string format = System.String.Format("{0:F2} FPS",fps);
			GetComponent<UnityEngine.UI.Text>().text = format;

			if(fps < 30)
			GetComponent<UnityEngine.UI.Text>().material.color = Color.yellow;
			else
			if(fps < 10)
			GetComponent<UnityEngine.UI.Text>().material.color = Color.red;
			else
			GetComponent<UnityEngine.UI.Text>().material.color = Color.green;
			timeleft = updateInterval;
			accum = 0.0F;
			frames = 0;
		}
	}
}
