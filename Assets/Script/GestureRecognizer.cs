using System;
using UnityEngine.UI;
using UnityEngine;

public class GestureRecognizer : MonoBehaviour
{
    private const String NAME_CHAIR = "chair";
    private int count;
    public Text countText;
    public Text debugText;
    public float speed = 0.5F;
    public float safeDistance = 1.2f;


    int pulse;
    static int maxPulse = 400;


    void Start()
    {
        count = 0;
        countText.text = "Count: " + count.ToString();
    }

    /*
    void Update()
    {
        if (Input.touchCount > 0) {
            count = count + 1;
            countText.text = "Count: " + count.ToString ();
            Debug.Log (Input.touchCount);
        }
    }
    */

    void FixedUpdate()
    {
        Debug.Log(GetClosestEnemy().name);


        if (Input.touchCount > 0)
        {
            // The screen has been touched so store the touch
            Touch touch = Input.GetTouch(0);
            debugText.text = touch.deltaTime.ToString();
            // the phase can define the cicle of life of the touch
            if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                // If the finger is on the screen, move the object smoothly to the touch position
                Vector3 touchPosition =
                    Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10));


                ///new Vector3.Lerp(transform.position, touchPosition, Time.deltaTime)
                transform.position = touchPosition;
            }


            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Began)
            {
                modifyPulse(true);
                countText.text = "moving finger , pulse:" + pulse;
            }
            else
            {
                modifyPulse(false);
                countText.text = "isnt moving , pulse:" + pulse;
            }
        }
        else
        {
            modifyPulse(false);
            countText.text = "isnt moving" + pulse;
        }
    }

    private void modifyPulse(bool isAdding)
    {
        if (isAdding)
        {
            pulse = pulse < maxPulse ?  pulse = pulse + 5 :  pulse = maxPulse;
        }
        else
        {
            pulse = pulse > 0 ? pulse - 5 : 0;
        }
    }

    GameObject GetClosestEnemy()
    {
        GameObject[] chairs = GameObject.FindGameObjectsWithTag(NAME_CHAIR);
        Transform[] chairTransformComponents = new Transform[chairs.Length];
        int position = 0;
        foreach (GameObject chairGameObject in chairs)
        {
            chairTransformComponents[position] = chairGameObject.transform;
            position++;
        }

        GameObject nearestGameObject = null;
        float minimumDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        float distance = 0;
        foreach (GameObject gameObject in chairs)
        {
            distance = Vector3.Distance(gameObject.transform.position, currentPosition);
            Debug.Log(distance.ToString());
            if (distance < minimumDistance)
            {
                nearestGameObject = gameObject;
                minimumDistance = distance;
            }
        }
        
        if (minimumDistance >= safeDistance)
        {
            Vector3 newPosition = getNewPosition(nearestGameObject.transform.position,
                currentPosition,
                minimumDistance,
                safeDistance);
            transform.Translate(newPosition * Time.deltaTime);
            Debug.Log("enter wirh distance equal to : " + distance);
        }
        else
        {
            // move arround the chair
            transform.RotateAround(nearestGameObject.transform.position, Vector3.forward, 20 * Time.deltaTime);
        }

        return nearestGameObject;
    }

    public Vector3 getNewPosition(Vector3 nearestPosition, Vector3 currentPosition, float minimumDistance, float safeDistance)
    {
        float reason = safeDistance / minimumDistance;
        // new x position of current object
        float newXPosition = (currentPosition.x + (reason * nearestPosition.x)) / (1 + reason);
        // new y position of current object
        float newYPosition = (currentPosition.y + (reason * nearestPosition.y)) / (1 + reason);
        return new Vector3(newXPosition, newYPosition, 0);
    }
    

    /// we are going to try calcule the "velocity"
}