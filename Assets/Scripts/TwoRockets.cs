using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoRockets : MonoBehaviour
{
    public GameObject RocketPrefab;

    void Start()
    {

        Rocket rocketScript1;
        GameObject Rocket1 = Instantiate( RocketPrefab, Vector3.zero, Quaternion.identity );
        rocketScript1 = Rocket1.GetComponent<Rocket>();
        rocketScript1.CreateModule(4, 0.0f, 0.0f, 2000.0f);
        rocketScript1.CreateModule(4, 0.0f, 0.0f, 2000.0f);
        rocketScript1.CreateModule(4, 0.0f, 0.0f, 2000.0f);
        rocketScript1.GenerateModules();

        Rocket rocketScript2;
        Vector3 r2Vector = new Vector3(3.0f, 0.0f, 0.0f);
        GameObject Rocket2 = Instantiate( RocketPrefab, r2Vector, Quaternion.identity );
        rocketScript2 = Rocket2.GetComponent<Rocket>();
        rocketScript2.CreateModule(8, 0.0f, 20.0f, 2000.0f);
        rocketScript2.CreateModule(8, 0.0f, 20.0f, 2000.0f);
        rocketScript2.CreateModule(8, 0.0f, 20.0f, 2000.0f);
        rocketScript2.GenerateModules();
        
    }

}
