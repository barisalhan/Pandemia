using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class MTPScriptableObject : ScriptableObject
{
    /*
     * Gets the value of an independent
     * parameter of an instance of a class.
     */
    public virtual object GetParameter(string parameterName)
    {
        // Holds the metadata of the class.
        Type type = this.GetType();

        string methodName = "Get" + parameterName;

        // Holds kind of a pointer to the method that we want to reach.
        MethodInfo method = type.GetMethod(methodName);

        if (method != null)
        {
            Object instance = this;
            var value = method.Invoke(instance, null);
            return value.ToString();
        }
        else
        {
            Debug.Log("Method:" + methodName + " does not exits and it is tried to be called.");
        }

        return null;
    }


    /*
     * Sets the value of an independent
     * parameter of an instance of a class.
     */
    public virtual void SetParameter(string parameterName,
                                     int effectType,
                                     double effectValue)
    {
        // Holds the metadata of the class.
        Type type = this.GetType();

        FieldInfo fieldInfo = type.GetField(parameterName);

        // TODO: error-prone area.
        if (fieldInfo != null)
        {
            Object instance = this;

            if (effectType == 0)
            {
                if (fieldInfo.FieldType == typeof(double))
                {
                    double currentValue = (double)fieldInfo.GetValue(instance);
                    fieldInfo.SetValue(instance, currentValue + effectValue);
                }
                else if (fieldInfo.FieldType == typeof(int))
                {
                    int currentValue = (int)fieldInfo.GetValue(instance);
                    fieldInfo.SetValue(instance, (int)(currentValue + effectValue));
                }
                else
                {
                    Debug.Log("EffectType and EffectValue do not match.");
                }
            }
            else if (effectType == 1)
            {
                if (fieldInfo.FieldType == typeof(double))
                {
                    double currentValue = (double)fieldInfo.GetValue(instance);
                    fieldInfo.SetValue(instance, currentValue * effectValue);
                }
                else if(fieldInfo.FieldType == typeof(int))
                {
                    int currentValue = (int)fieldInfo.GetValue(instance);
                    fieldInfo.SetValue(instance, (int)(currentValue * effectValue));
                }
                else
                {
                    Debug.Log("EffectType and EffectValue do not match.");
                }
            }
            else if (effectType == 2)
            {
                if (fieldInfo.FieldType == typeof(bool))
                {
                    bool currentValue = (bool)fieldInfo.GetValue(instance);
                    fieldInfo.SetValue(instance, effectValue);
                }
                else
                {
                    Debug.Log("EffectType and EffectValue do not match.");
                }
            }
            else
            {
                Debug.Log("Unknown effectType is requested.");
            }
        }
        else
        {
            Debug.Log("Parameter:" + parameterName + " does not exits and it is tried to be reached.");
        }
    }
}
