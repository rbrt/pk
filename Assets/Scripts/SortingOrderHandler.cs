using UnityEngine;
using System.Collections;
using System.Linq;

public class SortingOrderHandler : MonoBehaviour {

    int sortNumber = 100;

	void Update () {
        var renderers = GameObject.FindObjectsOfType<SpriteRenderer>().ToList();
        renderers.Sort((renderer1, renderer2) => (renderer2.transform.position.y.CompareTo(renderer1.transform.position.y)));
        int index = 0;
        renderers.ForEach(x => {
            x.sortingOrder = sortNumber + index;
            index++;
        });
	}
}
