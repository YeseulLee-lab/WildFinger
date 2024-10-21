using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using UnityEngine.Events;


public class RoyalItemDisplayer : MonoBehaviour
{

    /// <summary>
    /// RoyalCollectionSystem Package
    /// </summary>


    // Reference to the Text component displaying the item collected quantity
    public Text _itemDisplay;

    // The current item collected quantity
    private int _itemQuantity = 0;



    //Checking our item name to display in _itemDisplay's variable
    public string nameItem;

    // Add 'quantity' items and update the UI text
    public void AddItem(int quantity)
    {
        _itemQuantity = int.Parse(_itemDisplay.text);
        _itemQuantity += quantity;
 
        _itemDisplay.text = nameItem + _itemQuantity.ToString();
        gameObject.transform.DOScale(1.1f, 1).SetEase(Ease.OutElastic).Play().OnComplete(() =>
        {
            gameObject.transform.DOScale(Vector3.one, 1).SetEase(Ease.OutBack).Play();
        });




        //If was find Scene RoyalBoxManager Script Or   If was Item BoxsCollect 
        if (FindObjectOfType<RoyalBoxManager>())
        {
            //Each time the coin is added to our variable, we return the time to the original value, and when it is finished, the time box manager decreases and the door...
            FindObjectOfType<RoyalBoxManager>().TimeClose = 1.3f;

        }
    }

    public void AddItem(int quantity, UnityAction done)
    {
        _itemQuantity = int.Parse(_itemDisplay.text);
        _itemQuantity += quantity;

        _itemDisplay.text = nameItem + _itemQuantity.ToString();
        gameObject.transform.DOScale(1.1f, 1).SetEase(Ease.OutElastic).Play().OnComplete(() =>
        {
            gameObject.transform.DOScale(Vector3.one, 1).SetEase(Ease.OutBack).Play();
        });

        if (_itemQuantity <= 0)
        {
            if(done != null)
                done.Invoke();
        }


        //If was find Scene RoyalBoxManager Script Or   If was Item BoxsCollect 
        if (FindObjectOfType<RoyalBoxManager>())
        {
            //Each time the coin is added to our variable, we return the time to the original value, and when it is finished, the time box manager decreases and the door...
            FindObjectOfType<RoyalBoxManager>().TimeClose = 1.3f;

        }
    }
}