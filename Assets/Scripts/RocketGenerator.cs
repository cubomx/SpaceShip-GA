using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject RocketPrefab;
    public Vector2 quantityRockets = new Vector2(10, 10);
    public int selectionQuantity = 50;
    public float desireHeight = 10000.0f;

    private int [] rangePropulsors =  new int [2]{ 4, 4}; 
    private Vector2 rangeIgnition = new Vector2(0.0f, 0.0f);

    // range [10, 100], * 10
    private Vector2 rangeTurbo = new Vector2(2.0f, 6.0f);
    // range [1000, 10000]
    private Vector2 rangeGas = new Vector2(1.0f, 10.0f);
    private List<Rocket> rockets;

    private bool generatingNewPop = false;
    private bool foundTheBest;

    private Util util;

    void Start()
    {
        util = new Util( );
        rockets = new List<Rocket>( );
        for( int idx = 0; idx < quantityRockets.x; idx++){
            for( int idz = 0; idz < quantityRockets.y; idz++){
                rockets.Add ( generateRocket ( idx, idz ) );
            }
        }
    }

    Rocket generateRocket ( int indexX, int indexZ ){
        Rocket rocket;
        Vector3 pos = new Vector3(3.0f*(indexX - 10), 0.0f, 3.0f*(indexZ- 10));
        GameObject rocketGO = Instantiate( RocketPrefab, pos, Quaternion.identity );
        
        rocket = rocketGO.GetComponent<Rocket>();
        int moduleQuantiy = 3;
        for ( int idx = 0; idx < moduleQuantiy; idx++ ){
            rocket.CreateRandomModule( rangePropulsors[0], rangePropulsors[1], 
            rangeIgnition.x, rangeIgnition.y, rangeTurbo.x, rangeTurbo.y, rangeGas.x, rangeGas.y );
        }
        
        rocket.GenerateModules();
        return rocket;
    }

    // Update is called once per frame
    void Update()
    {
        if ( !generatingNewPop ){

                if ( allRocketDone( rockets ) ) {

                    if ( foundTheBest ){
                        Debug.Log("Found best: " +  rockets[0]._maxHeight  );
                        rockets[0] = GenerateTheBest( rockets[0] );
                    }
                    else{
                        generatingNewPop = true;
                        Debug.Log("las sombras avanzan");
                        rockets.Sort( SortByHeight );
                        rockets = select( rockets );
                        if ( rockets[0]._maxHeight > desireHeight ){
                            
                            foundTheBest = true;
                            rockets = DeleteAllOther( rockets );
                        }
                        else{
                            printFirstFive( rockets );
                            rockets = crossover( rockets );
                            
                        }
                        generatingNewPop = false;
                    }
                    
                    
            }

        }
        
        
    }

    List<Rocket> DeleteAllOther( List<Rocket> rockets ){
        List<Rocket> rockets1 = new List<Rocket>( );
        for ( int i = 1; i < rockets.Count; i++ ){
            rockets[i].DestroySelf( );
        }
        rockets1.Add( rockets[0] );
        return rockets1;
    }

    Rocket GenerateTheBest( Rocket bestRocket ){
        Rocket rocket;
        GameObject rocketGO = Instantiate( RocketPrefab, Vector3.zero, Quaternion.identity );
        rocket = rocketGO.GetComponent<Rocket>();
        for ( int i = 0; i < bestRocket.rocketModules.Count; i++ ){
            Module module = bestRocket.rocketModules[i];
            rocket.CreateModule( module._propulsors, 0.0f, module._turbo, module._maxGas );
        }
        rocket.GenerateModules( );
        bestRocket.DestroySelf( );
        return rocket;
    }

    void printFirstFive(List<Rocket> rockets ){
        for (int i = 0; i < 5; i++ ){
            Debug.Log("Height: " + rockets[i]._maxHeight );
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

    List<Rocket> select ( List<Rocket> rockets ){
        List<Rocket> selectedRockets = new List<Rocket>( );

        for ( int idx = 0; idx < rockets.Count; idx++ ){
            if ( idx < selectionQuantity ){
                selectedRockets.Add( rockets[idx] );
                rockets[idx].ChangeActive( false );
            }
            else{
                rockets[idx].DestroySelf( );
            }
            
        }
        return selectedRockets;
    }

    Rocket combineGenes ( Rocket parentOne, Rocket parentTwo, bool isFirstSon, Vector3 pos ){
        Rocket rocket;
        GameObject rocketGO = Instantiate( RocketPrefab, pos, Quaternion.identity );
        rocket = rocketGO.GetComponent<Rocket>();
        // get largest module number 
        int parentOneModuleNum = parentOne.rocketModules.Count, parentTwoModuleNum = parentTwo.rocketModules.Count;
        int numModules = util.getBiggestValue( parentOneModuleNum, parentTwoModuleNum );
        for ( int idx = 0; idx < numModules; idx++ ){
            Module parentOneModule = parentOne.rocketModules[idx];
            Module parentTwoModule = parentTwo.rocketModules[idx];
            CrossOver cross;
            if ( isFirstSon ){
                cross = getValuesFromModule(parentOneModule, parentTwoModule, idx % 2 == 0 ? true : false );
            }
            else{
                cross = getValuesFromModule(parentOneModule, parentTwoModule, idx % 2 != 0 ? true : false );
            }
            cross = mutate( cross, .10f );
            rocket.CreateCombinedModule( cross );
        }
        
        rocket.GenerateModules();
        return rocket;
    }


    CrossOver getValuesFromModule (Module parentOneMod, Module parentTwoMod, bool mode ) {
        if ( mode ){
            return new CrossOver ( parentOneMod._propulsors, parentTwoMod._ignition, parentOneMod._turbo, parentTwoMod._maxGas);
        }
        else{
            return new CrossOver ( parentTwoMod._propulsors, parentOneMod._ignition, parentTwoMod._turbo, parentOneMod._maxGas);
        }
    }

    CrossOver mutate(CrossOver cross, float ratio ){
        cross.propulsorNo = 4;
        cross.ignitionTime = 0.0f;
        cross.turboPercentage = ( (int) util.changeValue( rangeTurbo.x, rangeTurbo.y, cross.turboPercentage/10.0f, ratio, util.istUpMutating( ) )) * 10.0f;
        cross.gasCapacity = ((int)util.changeValue( rangeGas.x, rangeGas.y, cross.gasCapacity/300.0f, ratio, util.istUpMutating( ) ))*1000.0f + 200;
        return cross;
    }

    


    List<Rocket> crossover ( List<Rocket> rockets ){
        float x = -30, z = -30;
        List<Rocket> newPop = new List<Rocket>( );
        Vector2 pos = new Vector2(0.0f, 0.0f);

        for (int iterations = 0; iterations < 100; iterations++ ){
            rockets = util.Shuffle( rockets );
            for( int idCouple = 0; idCouple < selectionQuantity / 2; idCouple++ ){
                // create two sons per couple
                Rocket parentOne =  rockets[ idCouple ];
                Rocket parentTwo = rockets[idCouple + selectionQuantity / 2 ];
                newPop.Add( combineGenes( parentOne, parentTwo, true, new Vector3 (x, 0.0f, z)  )  );
                pos = util.changePosition( x, z );
                x = pos.x;
                z = pos.y;
                newPop.Add( combineGenes( parentOne, parentTwo, false, new Vector3 (x, 0.0f, z)  ) );
                pos = util.changePosition( x, z );
                x = pos.x;
                z = pos.y;
                
            }
        }
        util.deleteRockets( rockets ); 
        return newPop;
    }



    
}
