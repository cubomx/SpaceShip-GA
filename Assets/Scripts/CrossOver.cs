using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossOver {
    public int propulsorNo;
    public float ignitionTime; 
    public float turboPercentage; 
    public float gasCapacity;

    public CrossOver ( int propulsorNo, float ignitionTime, float turboPercentage, float gasCapacity){
        this.propulsorNo = propulsorNo;
        this.ignitionTime = ignitionTime;
        this.turboPercentage = turboPercentage;
        this.gasCapacity = gasCapacity;
    }
}