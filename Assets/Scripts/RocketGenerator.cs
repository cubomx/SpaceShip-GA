using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject RocketPrefab;
    public Vector2 quantityRockets = new Vector2(10, 10);
    public int selectionQuantity = 50;

    private int [] rangePropulsors =  new int [2]{ 1, 5}; 
    private Vector2 rangeIgnition = new Vector2(0.0f, 5.0f);
    private Vector2 rangeTurbo = new Vector2(10.0f, 30.0f);
    private Vector2 rangeGas = new Vector2(1000.0f, 3000.0f);
    private List<Rocket> rockets;

    private bool generatingNewPop = false;

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
        Vector3 pos = new Vector3(3.0f*(indexX - 2), 0.0f, 3.0f*(indexZ- 2));
        GameObject rocketGO = Instantiate( RocketPrefab, pos, Quaternion.identity );
        
        rocket = rocketGO.GetComponent<Rocket>();
        int moduleQuantiy = Random.Range(1, 8 );
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
                    Debug.Log("las sombras avanzan");
                    rockets.Sort( SortByHeight );
                    rockets = select( rockets );
                    rockets = crossover( rockets );
                    generatingNewPop = true;
            }

            
        }
        else{
             if ( allRocketDone( rockets ) ){
                 Debug.Log( "finish new pop");
             }
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
            Module parentOneModule = null, parentTwoModule = null;
            if ( idx < parentOneModuleNum){
                parentOneModule = parentOne.rocketModules[idx];
            }
            if ( idx < parentTwoModuleNum ){
                parentTwoModule = parentTwo.rocketModules[idx];
            }
            CrossOver cross;
            if ( isFirstSon ){
                cross = getValuesFromModule(parentOneModule, parentTwoModule, idx % 2 == 0 ? true : false );
            }
            else{
                cross = getValuesFromModule(parentOneModule, parentTwoModule, idx % 2 != 0 ? true : false );
            }
            cross = mutate( cross, .15f );
            rocket.CreateCombinedModule( cross );
        }
        
        rocket.GenerateModules();
        return rocket;
    }


    CrossOver getValuesFromModule (Module parentOneMod, Module parentTwoMod, bool mode ) {
        if ( parentOneMod == null ){
            return new CrossOver( parentTwoMod._propulsors, parentTwoMod._ignition, parentTwoMod._turbo, parentTwoMod._gas );
        }
        else if ( parentTwoMod == null){
            return new CrossOver ( parentOneMod._propulsors, parentOneMod._ignition, parentOneMod._turbo, parentOneMod._gas);
        }
        else if ( mode ){
            return new CrossOver ( parentOneMod._propulsors, parentTwoMod._ignition, parentOneMod._turbo, parentTwoMod._gas);
        }
        else{
            return new CrossOver ( parentTwoMod._propulsors, parentOneMod._ignition, parentTwoMod._turbo, parentOneMod._gas);
        }
    }

    CrossOver mutate(CrossOver cross, float ratio ){
        cross.propulsorNo = (int) util.changeValue( (float) rangePropulsors[0],  (float) rangePropulsors[1], (float) cross.propulsorNo, ratio, util.istUpMutating() );
        cross.ignitionTime = util.changeValue(rangeIgnition.x, rangeIgnition.y, cross.ignitionTime, ratio, util.istUpMutating( ) );
        cross.turboPercentage = util.changeValue( rangeTurbo.x, rangeTurbo.y, cross.turboPercentage, ratio, util.istUpMutating( ) );
        cross.gasCapacity = util.changeValue( rangeGas.x, rangeGas.y, cross.gasCapacity, ratio, util.istUpMutating( ) );
        return cross;
    }

    
    List<Rocket> crossover ( List<Rocket> rockets ){
        float x = -6, z = -6;
        List<Rocket> newPop = new List<Rocket>( );
        Vector2 pos = new Vector2(0.0f, 0.0f);
        for ( int iterations = 0; iterations < 2; iterations++ ){
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
                if ( iterations != 0){
                    parentOne.DestroySelf( );
                    parentTwo.DestroySelf( );
                }
            }
        }
        
        return newPop;
    }

    
}
