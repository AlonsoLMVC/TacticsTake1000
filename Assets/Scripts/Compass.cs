using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    // Start is called before the first frame update

    public Vector2 blendTreeNE;
    public Vector2 blendTreeSE;
    public Vector2 blendTreeSW;
    public Vector2 blendTreeNW;

    public Vector2 NE;
    public Vector2 SE;
    public Vector2 SW;
    public Vector2 NW;


    void Start()
    {   
        blendTreeNE = new Vector2(1, 0);
        blendTreeSE = new Vector2(0, -1);
        blendTreeSW = new Vector2(-1, 0);
        blendTreeNW = new Vector2(0, 1);

        NE = new Vector2(1, 0);
        SE = new Vector2(0, -1);
        SW = new Vector2(-1, 0);
        NW = new Vector2(0, 1);
    }

    
    public void compassShiftClockwise()
    {
        Debug.Log("compass shift clockwise");
        Vector2 oldNE = NE;
        Vector2 oldSE = SE;
        Vector2 oldSW = SW;
        Vector2 oldNW = NW;

        NE = oldNW;
        SW = oldSE;
        NW = oldSW;
        SE = oldNE;
    }

    public void compassShiftCounterClockwise()
    {
        Debug.Log("compass shift counter clockwise");

        Vector2 oldNE = NE;
        Vector2 oldSE = SE;
        Vector2 oldSW = SW;
        Vector2 oldNW = NW;

        NE = oldSE;
        SW = oldNW;
        NW = oldNE;
        SE = oldSW;
    }

    public Vector2 convertDirectionToBlendTreeDirection(Vector2 direction)
    {
        Debug.Log("The direction being sent is " + direction);
        if (direction == NE)
        {
            Debug.Log("Right now that corresponds to NE");
            return blendTreeNE;
        }
        else if (direction == NW)
        {
            Debug.Log("Right now that corresponds to NW");
            return blendTreeNW;
        }
        else if (direction == SW)
        {
            Debug.Log("Right now that corresponds to SW");
            return blendTreeSW;
        }
        else if (direction == SE)
        {
            Debug.Log("Right now that corresponds to SE");
            return blendTreeSE;
        }
        else {
            Debug.Log("No match! This should never happen.");
            return Vector2.zero;
        }


    }


}
