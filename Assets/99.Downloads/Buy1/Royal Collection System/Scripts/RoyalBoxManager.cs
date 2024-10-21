using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class RoyalBoxManager : MonoBehaviour
{

    /// <summary>
    /// RoyalCollectionSystem Package
    /// </summary>

    [Header("Add_Collect")]
    public int collectAdd_Coin; //Coin Diamaond the amount of
    [Space(5)]
    public int collectAdd_Diamond;//Add Diamaond the amount of


    [Space(10)]
    public GameObject boxes;//The gameobject variable in our box Hierarchy

    [Space(10)]
    [Header("Sttings")]
    public RoyalCollectingController[] royalCollectsItem;//Array 0 Coin && Array 1 diamond

    public float TimeClose = 1.3f;//Time to close the bonus box
    private bool close = false;//Box closure
    private bool closeStart = false;//Start closing the box


    private void Update()
    {
        //if was varibale closeStart active or true 
        if (closeStart == true)
        {
            //And if the closing time of our box is zero
            if (TimeClose <= 0)
            {
                //The hair closure closing box is active...
                close = true;

                //OnClose function is executed with the closing input
                OnClose(close);
            }
            //Other than this is our time
            else
            {
               // Our time will be reduced by Delta frame
                TimeClose -= Time.deltaTime;
            }
        }
    }


    ///Click To Box And Collect Get
    public void clickBoxes()
    {
        //Play aniamtions Open To varibale boxes ...
        boxes.GetComponent<Animator>().Play("Open");

        //Audio Play
        gameObject.GetComponent<AudioSource>().Play();


        //boxes to componnet Button to enabel false
        boxes.GetComponent<Button>().enabled = false;

        ///Use the Inune method to OnOpenComplate function after the second time in the input...
        Invoke("OnOpenComplate", 1);


        // When it was closed, our time returned to the first
        TimeClose = 1.3f;

        // it was Varibale Close true
        if (close)
        {
            closeStart = false;//go Varibale closeStart false 
            close = false;//go Varibale close false 
        }
    }


    ///Function that opens when our door opens
    void OnOpenComplate()
    {
        //Add Coins;
        royalCollectsItem[0].CollectItem(collectAdd_Coin);
        //Add Diamond;
        royalCollectsItem[1].CollectItem(collectAdd_Diamond);

        // was Varibale closeStart true
        closeStart = true;
    }


    ///boxes closing function
    void OnClose(bool complateCheck)
    {
        //if was Complate Check Functions Input true
        if (complateCheck)
        {
            //Play aniamtions Close To varibale boxes ...
            boxes.GetComponent<Animator>().Play("Close");

            //boxes to componnet Button to enabel true
            boxes.GetComponent<Button>().enabled = true;

            //Audio Play
            gameObject.GetComponent<AudioSource>().Play();

            // was Varibale close false
            close = false;

            // was Varibale closeStart false
            closeStart = false;
        }
    }
}