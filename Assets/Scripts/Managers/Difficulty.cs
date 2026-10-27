using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[InfoHeaderClass("Put me into the scene. I control difficulty by increasing movespeed")]
public class Difficulty : MonoBehaviour
{
    //Increase difficulty over time
    //Goal: Increase movespeed every set amount of time

    [Header("In what script does the thing to change live?")]
    [SerializeField] private ForwardRunner forwardRunner;
   

    [Header("How much should it increase?")]
    [SerializeField] private float speedIncrease = 2f;
    [Header("How often/fast should it increase? 0.5 means every half second")]
    [SerializeField] private float interval = 1f;

    //What to change to increase difficulty
    private float moveSpeed;
    //Keep track of the current time in the timer
    private float timer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveSpeed = forwardRunner.forwardSpeed;
    }

    // Update is called once per frame
    void Update()
    {        
        Timer();
    }

    private void Timer()
    {
        timer += Time.deltaTime;

        if(timer > interval)
        {
            //Linear increase every second
            moveSpeed = moveSpeed + speedIncrease;
            forwardRunner.forwardSpeed = moveSpeed;
            //Reset the timer
            timer = 0f;
        }
    }
}
