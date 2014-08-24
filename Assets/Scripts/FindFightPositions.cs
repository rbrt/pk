using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FindFightPositions : MonoBehaviour {

    [SerializeField] int positionRange;
    [SerializeField] float scalingValue;

    public List<Vector3> GetEnemyPositions(int enemyCount){
        float offset = GetOffsetValue(enemyCount);
        float angle = 0 - (positionRange / enemyCount);

        if (offset!= 0){
            angle = 0 - (positionRange / enemyCount / offset);
        }

        List<Vector3> enemyPositions = new List<Vector3>();
        for (int i = 0; i < enemyCount; i++){

            enemyPositions.Add(transform.position + Quaternion.Euler(0, 0, angle) * ( Vector3.right) * scalingValue);

            if (i % 2 == 0){
                angle += 180;
            }
            else{
                angle -= 180;
                angle += (positionRange / enemyCount);
            }
        }

        return enemyPositions;
    }

    void Update(){
        GetEnemyPositions(8).ForEach(pos => Debug.DrawLine(transform.position, pos, Color.red));
    }

    float GetOffsetValue(int enemyCount){
        if (enemyCount > 8 || enemyCount < 0){
            Debug.LogError("More than 8 enemies or less than 1 enemy not supported");
            return -2;
        }
        else{
            if (enemyCount == 1){
                return -1;
            }
            else if (enemyCount == 2){
                return .05f;
            }
            else if (enemyCount < 5){
                return 2;
            }
            else if (enemyCount < 7){
                return 0;
            }
            else if (enemyCount == 7){
                return .65f;
            }
            else if (enemyCount == 8){
                return .75f;
            }
        }

        return -2;
    }
}
