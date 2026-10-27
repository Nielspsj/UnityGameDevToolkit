using UnityEngine;

public class HeightTracker : MonoBehaviour
{
    //Show this as part of scoring:
    public float travelledHeight;
    //How high you actually got
    [SerializeField] private float peakHeight;
    [SerializeField] private float currentHeight;
    [SerializeField] private float coolnessMultiplier = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        peakHeight = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        currentHeight = transform.position.y;
        if(peakHeight < currentHeight)
        {
            peakHeight = currentHeight;
            //Show this as part of scoring
            travelledHeight = peakHeight * coolnessMultiplier;
            //Cut away the decimal numbers
            travelledHeight = Mathf.Round(travelledHeight);
        }
    }
}
