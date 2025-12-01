using System;
using UnityEngine;
[Serializable]
public class InteractCases : MonoBehaviour
{
    public virtual void Successful()
    {

    }
    
    public virtual bool AdditionalCheck()
    {
        return true;
    }
    
    public virtual void Failed()
    {
        
    }
}

