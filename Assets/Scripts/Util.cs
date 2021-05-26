using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Util {
    
    public List<Rocket> Shuffle  ( List<Rocket> list ){
        for (int i = 0; i < list.Count; i++) {
            Rocket temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }

    public Vector2 changePosition (float x, float z){
         if ( x >= 30.0f) {
             x = -30.0f;
             z += 3.0f;
         }
         else{
             x += 3.0f;
         } 
         return new Vector2(x, z);
    }

    public float changeValue(float min, float max, float value, float ratio, bool isToUp){
        value *= (isToUp) ? (1.0f +ratio) : (1.0f - ratio) ;
        if ( value > max ) value = max;
        else if ( value < min ) value = min;
        return value;
    }
    public bool istUpMutating ( ){
        float randomUpDown = Random.Range(0.0f, 10.0f);
        if ( randomUpDown > 5.0f){
            return true;
        }
        return false;
    }
    public int getBiggestValue(int firstValue, int secondValue ){
        if ( firstValue > secondValue ) return firstValue;
        return secondValue;
    }

    public void deleteRockets( List<Rocket> rockets ){
        for ( int idx = 0; idx < rockets.Count; idx++ ){
            rockets[idx].DestroySelf( );
        }
    }
}