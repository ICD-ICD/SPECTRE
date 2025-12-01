using System.Collections.Generic;


public class Pool : IPool
{
    List<IRecyclableGameObject> objectPool = new List<IRecyclableGameObject>();
    
    public void Disable(IRecyclableGameObject poolableObject)
    {
        if (objectPool.Contains(poolableObject) == false)
        {
            AddToPool(poolableObject);
        }
        poolableObject.ResetState();
    }

    public (IRecyclableGameObject objOrNull, int idx) PickFromPool(int startIdx = 0)
    {
        for (int i = startIdx; i < objectPool.Count; i++)
        {
            if (objectPool[i].IsActive == false)
                return (objectPool[i], i);
        }
        
        return (null, int.MinValue);
    }
    
    public void AddToPool(IRecyclableGameObject poolableObject) => objectPool.Add(poolableObject);
}
