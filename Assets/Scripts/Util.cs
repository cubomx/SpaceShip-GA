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
}