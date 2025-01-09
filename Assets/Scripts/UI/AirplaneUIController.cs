using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AirplaneUIController : MonoBehaviour
{
    public List<IAirplaneUI> airplaneUIs = new List<IAirplaneUI>();
    void Start()
    {
        airplaneUIs = transform.GetComponentsInChildren<IAirplaneUI>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        if(airplaneUIs.Count > 0)
        {
            foreach(IAirplaneUI airplaneUI in airplaneUIs)
            {
                airplaneUI.HandleAirplaneUI();
            }
        }
    }
}
