using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmbrellaController : MonoBehaviour
{

    public UmbrellaType type;

    public enum UmbrellaType
    {
        None,
        Short,
        Long,
        Shooter
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void ShortNeutral()
    {
        //A SIMPLE SWIPE OF THE UMBRELLA USED AS A SWORD AS IN HOLLOW KNIGHT
    }

    void ShortUp()
    {
        //A SIMPLE SWIPE OF THE UMBRELLA IN THE UPWARDS DIRECTION AS IN HOLLOW KNIGHT
    }

    void ShortDownAir()
    {
        //A SIMPLE SWIPE OF THE UMBRELLA DOWNWARDS USED TO POGO AS IN HOLLOW KNIGHT
    }

    void LongNeutral()
    {
        //A SHORT THRUST USING THE POINTY LONG UMBRELLA
    }

    void LongUp()
    {
        //AN UPWARD MOVING IN A DIAGONAL DIRECTION THAT LIFTS THE PLAYER
    }

    void LongDash()
    {
        //DASHES FORWARD WITH THE WEAPON IN HAND TO HURT ENEMIES
    }

    void LongDown()
    {
        //POGO BOUNCE AS IN DUCKTALES OR SHOVEL KNIGHT
    }

    void Grapple()
    {
        //ALL UMBRELLA TYPES CAN GRAPPLE IN SOME WAY
    }

    void ShooterNeutral()
    {
        //NOT PRIORITY

        //SHOTS THROUGH THE POINT OF THE UMBRELLA ALLOW FOR 8 DIRECTIONS
    }
}