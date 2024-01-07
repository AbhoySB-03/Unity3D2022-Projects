using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Object pooler script for the cards.
/// Normally creating and destroying gameobject during runtime is an expensive operation
/// Object pooling optimizes the Gameplay as the Objects are Created in the starting of the Scene,
/// and in runtime they are just activated or deactivated instead of creating and destroying
/// </summary>
public class CardObjectPooler : MonoBehaviour
{
    [SerializeField] private GameObject poolObject; // The card object that is pooled
    [SerializeField] private int poolSize = 52; // pool size for the card
    Queue<GameObject> cardPool; // Queue DS for storing the pooled instances
    
    // Awake is called in the begining of the Game
    void Awake()
    {
        // create the Pool
        cardPool = createPool();
    }

    // Creating the pool of the objects
    Queue<GameObject> createPool()
    {
        // initialize an empty Queue
        Queue<GameObject> pool = new Queue<GameObject>();
        // create a certain number(=poolSize) of instances and add them in the Queue
        for (int i = 0; i < poolSize; i++)
        {
            GameObject g=Instantiate(poolObject);
            g.SetActive(false); // Make sure the Instances are not active
            pool.Enqueue(g);
        }
        // return this Queue
        return pool;
    }

    /// <summary>
    /// Used to get an instance from the pool
    /// </summary>
    /// <param name="position">Postion at which the object instance will be placed</param>
    /// <param name="rotation">Rotation for the Instance</param>
    /// <param name="parent">Parent transform for the Instance. Default is null</param>
    /// <returns></returns>
    public GameObject GetInstance(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject g=cardPool.Dequeue();    // Dequeue an object from the poolQueue
        g.SetActive (true); // Activate the object

        // Set the desired transforms for the object
        g.transform.position = position;
        g.transform.rotation = rotation;
        g.transform.parent = parent;

        // Return the gameobject
        return g;
    }

    /// <summary>
    /// Remove the instance from the scene
    /// </summary>
    /// <param name="g">The instance to be Removed</param>
    public void RemoveInstance(GameObject g)
    {
        // Deactivate the instance and add it to the Queue for reuse
        g.SetActive(false);
        cardPool.Enqueue(g);

    }

}
