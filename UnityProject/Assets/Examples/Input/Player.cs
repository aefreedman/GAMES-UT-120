using UnityEngine;

public class Player : MonoBehaviour
{
    public int PlayerNumber;
    public float Speed;

    private void Update()
    {
        if (PlayerNumber == 1)
        {
            if (Input.GetKey(KeyCode.UpArrow))
                transform.Translate(Time.deltaTime * Speed * Vector2.up);

            if (Input.GetKey(KeyCode.DownArrow))
                transform.Translate(Time.deltaTime * Speed * Vector2.down);
        }
        else if (PlayerNumber == 0)
        {
            if (Input.GetKey(KeyCode.W))
                transform.Translate(Time.deltaTime * Speed * Vector2.up);

            if (Input.GetKey(KeyCode.S))
                transform.Translate(Time.deltaTime * Speed * Vector2.down);
        }
    }
}