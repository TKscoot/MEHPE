using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupController : MonoBehaviour
{
    [SerializeField] PickupTypes pickupType;
    [SerializeField] GameObject destroyPrefab;

    public PickupTypes Pickup()
    {
        if (destroyPrefab != null)
        {
            Instantiate(destroyPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject, 0.1f);
        return pickupType;
    }
}

public enum PickupTypes
{
    Food,
    Money,
    Pills,
    Drinks
}
