using System;
using System.Reflection;
using UnityEngine;

public abstract class MTPScriptableObject : ScriptableObject
{
    /*
     * Gets the value of an independent
     * parameter of an instance of a class.
     */
    public virtual T GetParameterValue<T>(string parameterName)
    {
        // Holds the metadata of the class.
        Type type = this.GetType();

        PropertyInfo property = type.GetProperty(parameterName);

        Debug.Log(property.Name);

        if (property != null)
        {
            var value = property.GetValue(this);
            return (T)value;

            //return (T)Convert.ChangeType(value, typeof(T));
        }
        else
        {
            /*
            Debug.Log("Field: " + parameterName  + " does not exits" +
                      " and it is tried to be reached. 0 value is returned.");
            */
            return (T)Convert.ChangeType(null, typeof(T));
        }
    }


    public Double[] GetParameterLimits(string parameterName)
    {
        // Holds the metadata of the class.
        Type type = this.GetType();

        parameterName = "LIMITS_" + parameterName;
        FieldInfo fieldInfo = type.GetField(parameterName);

        if (fieldInfo != null)
        {
            Double[] limits = (Double[])fieldInfo.GetValue(this);
            
            if (limits == null)
            {
                Debug.Log("Limits is assigned but it is null.");
            }

            return limits;
        }
        else
        {
            Debug.Log("Limits: " + parameterName + " does not exits" +
                      " and it is tried to be reached. Null value is returned.");
            return null;
        }
    }


    /*
     * Sets the value of an independent
     * parameter of an instance of a class.
     */
    public virtual void SetParameterValue<T>(string parameterName, T t)
    {
        // Holds the metadata of the class.
        Type type = this.GetType();

        PropertyInfo property = type.GetProperty(parameterName);

        if (property != null)
        {
            property.SetValue(this, t);
        }
        else
        {
            Debug.Log("Field: " + parameterName + " does not exits" +
                      " and it is tried to be set.");
        }
    }

    public double Sigmoid(double input, double lowerLimit, double upperLimit, double temperature = 1)
    {
        double exp = Math.Exp(input/temperature);
        return lowerLimit + (exp / (1 + exp))*(upperLimit-lowerLimit);
    }
}

     
