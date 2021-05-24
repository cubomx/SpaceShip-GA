using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject RocketPrefab;
    public Vector2 quantityRockets = new Vector2(10, 10);
    private List<Rocket> rockets;

    private bool generatingNewPop = false;

    void Start()
    {
        rockets = new List<Rocket>( );
        for( int idx = 0; idx < quantityRockets.x; idx++){
            
            for( int idz = 0; idz < quantityRockets.y; idz++){
                rockets.Add ( generateRocket ( idx, idz ) );
            }
        }
    }

    Rocket generateRocket ( int indexX, int indexZ ){
        Rocket rocket;
        Vector3 pos = new Vector3(3.0f*(indexX - 1), 0.0f, 3.0f*(indexZ- 1));
        GameObject rocketGO = Instantiate( RocketPrefab, pos, Quaternion.identity );
        rocket = rocketGO.GetComponent<Rocket>();
        int moduleQuantiy = Random.Range(1, 8 );
        for ( int idx = 0; idx < moduleQuantiy; idx++ ){
            rocket.CreateRandomModule( 1, 5, 0.0f, 5.0f, 10.0f, 30.0f, 1000.0f, 3000.0f );
        }
        
        rocket.GenerateModules();
        return rocket;
    }

    // Update is called once per frame
    void Update()
    {
        if ( !generatingNewPop ){
            
                if ( allRocketDone( rockets ) ) {
                    Debug.Log("las sombras avanzan");
                    rockets.Sort( SortByHeight );
                    for ( int idx = 0; idx < rockets.Count; idx++ ){
                        Debug.Log( "Height: " + rockets[idx]._maxHeight );
                    }
                    generatingNewPop = true;
            }

            
        }
        else{
             
        }
        
    }

    int  SortByHeight ( Rocket rocket1, Rocket rocket2 ) {
        return rocket2._maxHeight.CompareTo( rocket1._maxHeight );
    }

    bool allRocketDone ( List<Rocket> rockets ){
        for ( int idx = 0; idx < rockets.Count; idx++ ){
            if ( !rockets[idx].itFlew ) return false;
        }
        return true;
    }
}
