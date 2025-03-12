using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    public Transform player;
    public float triggerDistance = 1f;

    private void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        Debug.Log("Khoảng cách đến trap: " + distance);

        if (distance <= triggerDistance)
        {
            Debug.Log("Trap được bật!");
            GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
