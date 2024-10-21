using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class RoyalStarsManager : MonoBehaviour {

    /// <summary>
    /// RoyalCollectionSystem Package
    /// </summary>



    [Header("Stars_Image")]
    public Image[] stars;//UI Image Star Items

    [Space(10)]
    public int StarsNumber;//Stars Get


    [Space(7)]
    public bool[] foulStars;//Active or not active stars
    private int numberStars = 0; //Count of stars

     [Space(15)]
    [Header("PrefabsStars")]
    public GameObject startStarPrefabs;//Prefab Star Created
    public GameObject endStarPrefabs;//Prefab End Star Created


    [Space(10)]
    public GameObject[] popPostions;//Buttons Click Item



    [Space(10)]
    [Header("Setting")]
    public Ease EaseEffect = Ease.Flash;//EffectStras Move
    public Color[] colorStars;//Color Image Star Items



    // Start Game 
    private void Start()
    {
        for (int i = 0; i < foulStars.Length; i++)
        {
            //if was foulstrs hint (i) is Active false 
            if (!foulStars[i])
            {
                //Coler Alpha Go To 150f or colorStars Elemet 1
                stars[i].color = colorStars[1];
            }
        }
    }


    public void On3Stars()//Click Auto 3 Stars
    {
        ///If the foulstars referred to by the num are disabled
        if (!foulStars[numberStars])
        {

            if (numberStars < StarsNumber)
            {
                //Starter Prefabs starts and moves to the star variable using the DG.Tweenm animation system
                GameObject starsCreate = Instantiate(startStarPrefabs, popPostions[0].gameObject.transform.position, Quaternion.identity, popPostions[0].gameObject.transform) as GameObject;

                //starsCreate Created Created Names In Hierarchy
                starsCreate.name = "stars " + numberStars;

                //            if was popPostions[0] Component Button
                //The null variable array bts that actually clicks on the click trigger activates the three stars is disabled
                if (popPostions[0].GetComponent<Button>())
                {
                    popPostions[0].GetComponent<Button>().interactable = false;
                }




                //Using Tween and the Move method, the star object is created, or StarsCreate moves towards the star in which num refers to it and, if it happens, it ends.
                starsCreate.transform.DOMove(stars[numberStars].gameObject.transform.position, 1).SetEase(EaseEffect).Play().OnComplete(() =>
                {
                //The starsCreate object will be deleted
                Destroy(starsCreate.gameObject);

                    //The endStarPrefabs variable is created in the star named numberStars by starsEndCreate
                    GameObject starsEndCreate = Instantiate(endStarPrefabs, stars[numberStars].gameObject.transform.position, Quaternion.identity, stars[numberStars].gameObject.transform) as GameObject;


                    //The variable variable numberStars refers to using the powerful DoShakeScale method.
                    stars[numberStars].transform.DOShakeScale(1.1f, 1).Play().OnComplete(() =>
                    {
                        stars[numberStars].transform.localScale = Vector3.one;//After the end it returns to the first state



                        //Remove Object 1.5f Seceond float
                        Destroy(starsEndCreate, 1.5f);
                    });

                    //  stars[numberStars] Change Color Alpha To Varibale colorStars Array zero
                    stars[numberStars].color = colorStars[0];


                    //The variable numberStars foulStars will be activated
                    foulStars[numberStars] = true;


                //The variable numberStars Add ++
                numberStars++;
  
                //Debug To Console numberStars
                Debug.Log(numberStars);


                //Play The function of checking the activation of all three stars 
                CheckStarsComplate();

                    //After the star is over, our current function will be executed
                    On3Stars();

                });
            }
        }

    }


  //  The function of checking the activation of all three stars
    void CheckStarsComplate()
    {
        //If the numberStars variable is greater than the size of the foulStars variable
        if (numberStars >= StarsNumber)
        {
            //            if was popPostions[1] Component Button
            //Activate the array of the btns variable, which actually refers to the button to delete all the stars
            if (popPostions[0].GetComponent<Button>())
            {
                popPostions[1].GetComponent<Button>().interactable = true;
            }

            //print comolate Stars to Console
            print("ComplateStars");
        }
    }


    //Received stars
    public void ResetStars()
    {
        //for All Array stars Ui Image
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].color = colorStars[1];/// The color of all our stars varies in color To colorStars Array One
            foulStars[i] = false;//All Varibale foulStars To false

        }
        numberStars = 0;//Varibale numberStars zero

        //            if was popPostions[0] Component Button
        if (popPostions[0].GetComponent<Button>() && popPostions[1].GetComponent<Button>())
        {
            popPostions[1].GetComponent<Button>().interactable = false;//btn_Reset Varibale btns Array One Intractabale To false
            popPostions[0].GetComponent<Button>().interactable = true;////btn_On3Stars Varibale btns Array zero Intractabale To true
        }
    }
}
