using UnityEngine;

public class Level : MonoBehaviour
{
    //static varible so it doesnt get destroyed
    public static string PreviousLevel { get; private set; }
    //when this script is destroyed
    private void OnDestroy()
    {
        //set previouslevel variable to the current scene name so i can call this varible to get the
        //name of previous scene anywhere as long as that scene has a dummy object with this script running.
        PreviousLevel = gameObject.scene.name;
    }
}
