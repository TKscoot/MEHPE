using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collector : MonoBehaviour
{
    [SerializeField] private Text foodValue;
    [SerializeField] private Text drinksValue;
    [SerializeField] private Text pillsValue;
    [SerializeField] private Text moneyValue;
    [SerializeField] private Text followerValue;
    [SerializeField] private Text houseValue;

    private Dictionary<PickupTypes, int> inventory;
    private List<NPCController> follower;

    private int score;
    private int totalFollower;

    private void Awake()
    {
        follower = new List<NPCController>();
        inventory = new Dictionary<PickupTypes, int>();

        inventory.Add(PickupTypes.Food, 0);
        inventory.Add(PickupTypes.Drinks, 0);
        inventory.Add(PickupTypes.Pills, 0);
        inventory.Add(PickupTypes.Money, 0);
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.GetComponent<PickupController>() != null)
        {
            inventory[_other.GetComponent<PickupController>().Pickup()]++;

            foodValue.text = inventory[PickupTypes.Food].ToString();
            drinksValue.text = inventory[PickupTypes.Drinks].ToString();
            pillsValue.text = inventory[PickupTypes.Pills].ToString();
            moneyValue.text = inventory[PickupTypes.Money].ToString();
        }

        if (_other.GetComponent<NPCController>() != null)
        {
            if (inventory[_other.GetComponent<NPCNeeds>().NeededPickup] > 0)
            {
                if (follower.Contains(_other.GetComponent<NPCController>()))
                {
                    return;
                }

                follower.Add(_other.GetComponent<NPCController>());

                inventory[_other.GetComponent<NPCNeeds>().NeededPickup]--;

                foodValue.text = inventory[PickupTypes.Food].ToString();
                drinksValue.text = inventory[PickupTypes.Drinks].ToString();
                pillsValue.text = inventory[PickupTypes.Pills].ToString();
                moneyValue.text = inventory[PickupTypes.Money].ToString();

                _other.GetComponent<NPCController>().SetFollowing();

                AudioController.Instance.AddPartyMember(1);
                totalFollower++;


                followerValue.text = follower.Count.ToString();

				_other.GetComponent<NPCController>().SetCollectorScript(this);
            }
        }
    }

    public int Score { get { return score; } }

	private void OnCollisionEnter(Collision collision)
	{
		if(collision.collider.tag == "PlayerHouse")
		{
			foreach(NPCController npc in follower)
			{
				npc.SetHouseGameObject(collision.collider.gameObject);
			}

			AudioController.Instance.RemovePartyMember(totalFollower);
			follower.Clear();
			followerValue.text = follower.Count.ToString();	
			totalFollower = follower.Count;

		}
	}

	public void IncreaseScore()
	{
		score++; //               SCCCCCCCCCCCCCCCCCCCCCOOOOOOOOOOOOOOOOOOOOOOOOOOORRRRRRRRRRRRRRRRRRRRRRRRRRRRREEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE
		houseValue.text = score.ToString();
	}

	public void RemoveFollower(NPCController _npcToRemove)
	{
		follower.Remove(_npcToRemove);
		followerValue.text = follower.Count.ToString();
		totalFollower = follower.Count;
	}

	public int GetFollowerCount()
	{
		return follower.Count;
	}
}
