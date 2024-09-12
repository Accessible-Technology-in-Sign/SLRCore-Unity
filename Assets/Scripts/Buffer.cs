using System.Collections;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using System.Collections.Generic;


class Buffer<T>{
    LinkedList<T> internalBuffer = new LinkedList<T>();

    //Dictionary<T> callbacks = new Dictionary<String, SLREventHandler<List<T>>>();
    //commented out because we don't have SLREventHandler yet

    IBufferTriggerStrategy<T> trigger = new CapacityFullTrigger<T>(60);
    IBufferFillStrategy<T> filler = new CapacityFill<T>();

    public void AddElement(T elem) {

        List<T> temp = internalBuffer.ToList();
        bool triggered = trigger.Check(temp);
        filler.Fill(temp, elem, triggered);
        //can't use linked list internalBuffer for methods that accept List<T>
        //made temp so that I could still used these methods with internalBuffer

        if (triggered) {
            //triggerCallBacks();
        }
        
    }

    public void triggerCallBacks() {
    }
}