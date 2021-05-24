using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public bool itFlew;
    private float initialHeight;
    [Header("Properties")]
    public float _maxHeight = 0.0f;
    [SerializeField] private float _flightTime = 0.0f;
    [SerializeField] private float _weight = 0.0f;
    [SerializeField] private float _trust = 0.0f;
    public float forceMultiplier = 1000.0f;
    public Material igniteColor;
    
    [Header("Weights")]
    public float baseRocketWeight = 10.0f;
    public float propulsorsWeight = 1.0f;
    public float modulesWeight = 5.0f;

    [Header("Objects")]
    public GameObject Module;
    public GameObject Fire;

    private List<GameObject> moduleObjs = new List<GameObject>();
    [SerializeField] private List<Module> rocketModules = new List<Module>();
    private Rigidbody rocketBody;

    void Start()
    {    
        /*CreateModule(4, 0.0f, 0.0f, 1000.0f);
        CreateModule(4, 1.0f, 50.0f, 1000.0f);
        CreateModule(4, 1.0f, 50.0f, 1000.0f);
        GenerateModules();*/
    }

    void FixedUpdate() {
        if ( !itFlew ){
            _flightTime += Time.deltaTime;
            RocketFly( );
            GetHeight( );
        }
        
        
    }

    float GetHeight(){

        float height = transform.position.y;
        
        if( height > _maxHeight ){
            _maxHeight = height;
        }
        else if ( height > initialHeight + 1.0f ){
            Debug.Log("hiho");
            itFlew = true;
        }
       
        return _maxHeight;
    }

    void RocketFly(){
        float rocketThrust = 0.0f;
        float rocketWeight = baseRocketWeight;
        int moduleCount = rocketModules.Count;

        //Base Module
        if( rocketModules[0].IsIgnite(_flightTime) ){
            rocketThrust += rocketModules[0].Propel();
        }else{
            Fire.SetActive(false);
        }
        rocketWeight += rocketModules[0].GetWeight();

        //Extra Modules
        for( int m=1; m<moduleCount; m++ ){
            
            if( rocketModules[m].IsIgnite(_flightTime) ){
                
                rocketThrust += rocketModules[m].Propel();
                moduleObjs[m-1].GetComponent<MeshRenderer>().material = igniteColor;
                moduleObjs[m-1].transform.GetChild(0).gameObject.SetActive(true);
            }
            
            if( rocketModules[m].IsEmpty() ){
                moduleObjs[m-1].SetActive(false);
            }else{
                rocketWeight += rocketModules[m].GetWeight();
            }
            
        }

        //Set mass and Add force
        _trust = rocketThrust;
        _weight =rocketWeight;
        rocketBody.mass = rocketWeight;
        rocketThrust *= forceMultiplier;
        rocketBody.AddForce(transform.up * rocketThrust);
    }

    public void GenerateModules(){
        rocketBody = GetComponent<Rigidbody>();
        int moduleCount = rocketModules.Count - 1;
        float angle = 360.0f / (float)moduleCount;

        GameObject mObj;
        for( int m=0; m<moduleCount; m++ ){
            mObj = Instantiate( Module, transform.position, Quaternion.identity );
            mObj.transform.Rotate( 0.0f, (angle*(m+1)), 0.0f, Space.Self );
            mObj.transform.parent = transform;
            moduleObjs.Add( mObj );
        }
    }

    public void CreateRandomModule( int minPropulsors, int maxPropulsors, float minIgnition, float maxIgnition, float minTurbo, float maxTurbo, float minGas, float maxGas ){
        int P = Random.Range(minPropulsors, maxPropulsors);
        float I = Random.Range(minIgnition, maxIgnition);
        float T = Random.Range(minTurbo, maxTurbo);
        float G = Random.Range(minGas, maxGas);
        CreateModule( P, I, T, G );
    }

    public void CreateModule( int propulsorNo, float ignitionTime, float turboPercentage, float gasCapacity){
        Module newModule = new Module( propulsorNo, ignitionTime, turboPercentage, 
                                        gasCapacity, modulesWeight, propulsorsWeight );
        //gameObject.AddComponent<Module>();
        rocketModules.Add( newModule );
        initialHeight = transform.position.y;
    }
}
