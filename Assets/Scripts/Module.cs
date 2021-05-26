using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module
{

    public int _propulsors;
    public float _ignition; //
    public float _turbo; //Percentage
    public float _gas;
    
    private float _gasWeight = 2.0f;
    private float _baseWeight = 5.0f;
    private float _propulsorWeight = 1.0f;
    private float _turboBoost = 1.0f;
    public float _maxGas;

    public Module(int p, float i, float t, float g){
        _propulsors = p;
        _ignition = i;
        _turbo = t;
        _maxGas = g;
        _gas = g;
    }

    public Module(int p, float i, float t, float g, float bW, float pW){
        _baseWeight = bW;
        _propulsorWeight = pW;

        _propulsors = p;
        _ignition = i;
        _turbo = t;
        _maxGas = g;
        _gas = g;
    }

    public float GetWeight(){
        float moduleWeight = _baseWeight + (_propulsorWeight * _propulsors) + (_gas*_gasWeight);
        return moduleWeight;
    }

    public bool IsEmpty(){
        if(_gas <= 0) return true;
        return false;
    }

    public bool IsIgnite(float Time){
        if(Time > _ignition) return true;
        return false;
    }

    public float Propel(){

        float gasPercentage = (_gas/_maxGas) * 100.0f;
        if(gasPercentage < _turbo){
            _turboBoost *= ( 1+(_turbo*0.5f) );
        }

        float force;
        float cost = (float)_propulsors * (_turboBoost*1.5f);
        if( cost > _gas ){
            force = _gas;
            _gas = 0;
        }else{
            force = cost;
            _gas -= cost;
        }

        return force;
    }


}
