using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCNeeds : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Sprite foodSprite;
    [SerializeField] Sprite drinkSprite;
    [SerializeField] Sprite pillsSprite;
    [SerializeField] Sprite moneySprite;
    [SerializeField] Sprite bubbleSprite;
    [SerializeField] GameObject destroyParticlePrefab;

    private Sprite selectedSprite;
    private PickupTypes neededPickup;
    private bool isFollowing;

    private void Start()
    {
        neededPickup = (PickupTypes) UnityEngine.Random.Range(0, Enum.GetValues(typeof(PickupTypes)).Length);
        
        switch (neededPickup)
        {
            case PickupTypes.Food:
                selectedSprite = foodSprite;
                break;
            case PickupTypes.Money:
                selectedSprite = moneySprite;
                break;
            case PickupTypes.Pills:
                selectedSprite = pillsSprite;
                break;
            case PickupTypes.Drinks:
                selectedSprite = drinkSprite;
                break;
            default:
                break;
        }

        icon.sprite = bubbleSprite;
    }

    private void OnTriggerEnter(Collider _other)
    {
        if(isFollowing)
        {
            return;
        }

        if (_other.GetComponent<Collector>() != null)
        {
            icon.sprite = selectedSprite;
        }
    }

    private void OnTriggerExit(Collider _other)
    {
        if (isFollowing)
        {
            return;
        }

        if (_other.GetComponent<Collector>() != null)
        {
            icon.sprite = bubbleSprite;
        }
    }

    public void Following()
    {
        isFollowing = true;
        icon.color = Color.clear;
    }

    public void Caught()
    {
        if (!isFollowing)
        {
            return;
        }

        if(destroyParticlePrefab != null)
        {
            Instantiate(destroyParticlePrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    public PickupTypes NeededPickup { get { return neededPickup; } }
}
