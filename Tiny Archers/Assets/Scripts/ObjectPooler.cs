using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    private class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    [SerializeField] private List<Pool> pools;

    private Dictionary<string, Queue<GameObject>> poolDict;


    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance= this;
    }

    // Start is called before the first frame update
    void Start()
    {
        poolDict= new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> queue = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject g = Instantiate(pool.prefab);
                g.SetActive(false);
                queue.Enqueue(g);
            }
            poolDict[pool.tag] = queue;

        }
        
    }

    public GameObject SpawnPoolObject(string poolTag, Vector3 position, Quaternion rotation, Transform parent=null)
    {
        if (!poolDict.ContainsKey(poolTag))
        {
            Debug.LogWarning("Object Pool with tag " + tag + " doesn't exists!");
            return null;
        }

        GameObject pooledObject = poolDict[poolTag].Dequeue();
        
        pooledObject.transform.position = position;
        pooledObject.transform.rotation = rotation;
        if (parent != null)
        pooledObject.transform.parent = parent;
        pooledObject.SetActive(true);
        poolDict[poolTag].Enqueue(pooledObject);
        return pooledObject;

    }

    
}
