using UnityEngine;

public class Stuck : MonoBehaviour
{
    public bool playerTouchingPoisonPlatform = false;
    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.name == "SM_Left_Ivy_Low Variant" || collision.gameObject.name == "SM_Right_Ivy_Low Variant")
        {
            playerTouchingPoisonPlatform = true;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.name == "SM_Left_Ivy_Low Variant" || collision.gameObject.name == "SM_Right_Ivy_Low Variant")
        {
            playerTouchingPoisonPlatform = false;
        }
    }
}
