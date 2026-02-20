using UnityEngine;
using UnityEngine.InputSystem;
public class playerBehavior : MonoBehaviour
{
    public float speed;
    public float min; 
    public float max;
    public int move;

    public Rigidbody2D rb;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody2D found on " + gameObject.name);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        float offset = 0.0f;
        bool left = (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed) && move != 1;
        if(left == true){
        offset = -speed;
        }

        if(Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed){
        offset = speed;
        }

        Vector3 newPos = transform.position;
        newPos.x = newPos.x + offset;

         //float startTime = 0.0f;
        if(transform.position.x > max){
            //startTime  = Time.time;
            newPos.x = max;
        }
        transform.position = newPos;


        if(transform.position.x < min){
            newPos.x = min;
        }
        transform.position = newPos;
    }
}
