using System;
using System.Collections;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;

class Buffer<T>{
    List<T> internalBuffer = new();

    Dictionary<string, Action<List<T>>> callbacks = new ();
    //commented out because we don't have SLREventHandler yet

    IBufferTriggerStrategy<T> trigger = new CapacityFullTrigger<T>(60);
    IBufferFillStrategy<T> filler = new CapacityFill<T>();

    public void AddElement(T elem) {

        List<T> temp = internalBuffer.ToList();
        bool triggered = trigger.Check(temp);
        filler.Fill(internalBuffer, elem, triggered);
        //can't use linked list internalBuffer for methods that accept List<T>
        //made temp so that I could still used these methods with internalBuffer

        if (triggered)
        {
            TriggerCallbacks();
        }
        
        Debug.Log(internalBuffer.Count);
        
    }

    public void AddCallback(string name, Action<List<T>> callback)
    {
        callbacks.Add(name, callback);
    }

    public void RemoveCallback(string name)
    {
        callbacks.Remove(name);
    }

    public void TriggerCallbacks() {
        var copy = internalBuffer.ToList();
        foreach (var callback in callbacks) {
            callback.Value(copy);
        }
    }
}