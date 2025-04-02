using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosswalk : MonoBehaviour
{
    public delegate void StateChange(bool crossing);
    public StateChange stateChange;
    public bool PedestriansAreCrossing = false;
    private int numberOfPedestians = 0;
    public bool CanCross = true;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
      
    }
    private void OnTriggerExit(Collider other)
    {
      
    }
}
