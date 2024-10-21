using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using FMODUnity;
using FMOD.Studio;
using System;

public class RoyalCollectingController : MonoBehaviour {

    /// <summary>
    /// RoyalCollectionSystem Package
    /// </summary>



    // Play collecting sound at begining of the animation or at the end
    public RoyalCollectingAnimation.PLAY_SOUND_MODE _playSoundMode;
    public RoyalCollectingAnimation.EXPANSION_MODE _expansionMode = RoyalCollectingAnimation.EXPANSION_MODE.Going_Up;
    // The emission rate in seconds
	public float emissionRate = 0.2f;
	// The tranform component of the item displayer
	public Transform itemDisplayer;
	// The position where to pop the items
	public Transform popPosition;
	// The prefab of the items to instanciate
	public GameObject itemPrefab { get; set; }
	// Instance of this class
	[HideInInspector]
	public static RoyalCollectingController _instance;

	// This is a list of instanciated _itemPrefab 
	private List<RoyalCollectingAnimation> _itemList = new List<RoyalCollectingAnimation>();
    // Reference to the AudioSource component

    [Header("------------------ SFX Area -----------------")]
    [SerializeField]
    private EventReference _collectingSfx;
    private EventInstance _collectingSfxInstance;

    void Awake() {
		// Setting instance
		_instance = this;
        //_audioSource = GetComponent<AudioSource> ();
        _collectingSfxInstance = RuntimeManager.CreateInstance(_collectingSfx);
    }

    private void Start()
    {
        if (GamePlayData.Instance != null)
        {
            _collectingSfxInstance.setVolume(GamePlayData.Instance.isCommonSFXOn ? 1f : 0f);
        }
    }

    private void OnDestroy()
    {
        _collectingSfxInstance.setUserData(IntPtr.Zero);
        _collectingSfxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _collectingSfxInstance.release();
    }

    // Collect some items with animation
    public void CollectItem(int quantity)
    {
		StartCoroutine (PopItems(quantity));
	}

    public void CollectItem(int quantity, RectTransform itemDisplayer, RectTransform popPos, GameObject itemPrefab, UnityAction done)
    {
        StartCoroutine(PopItems(quantity, itemDisplayer, popPos, itemPrefab, done));
    }

    public void UseItem(int quantity, RectTransform destPos, RectTransform itemDisplayer, RectTransform popPos, GameObject itemPrefab, UnityAction done)
    {
        StartCoroutine(UsePopItems(quantity, destPos, itemDisplayer, popPos, itemPrefab, done));
    }

    // Collect some items with animation at a fixed position
    public void CollectItemAtPosition(int quantity, Vector3 position) {
        // Set the position
        popPosition.position = position;
		StartCoroutine (PopItems(quantity));
	}

	// Here we pop all the necessary items
	IEnumerator PopItems(int quantity) {
		WaitForSeconds delay = new WaitForSeconds (emissionRate);
		for (int i = 0; i < quantity; i++) {
            RoyalCollectingAnimation animation = null;
			if(i < _itemList.Count) {
				if(!_itemList[i]._animationRunning) {
					// A free object has been found in pool, so we reuse it
					animation = _itemList[i];
				}
			} 
			if(animation == null) {
				// No free object has been found in pool, so we instantiate a new one
				GameObject go = Instantiate (itemPrefab) as GameObject;
				animation = go.GetComponent<RoyalCollectingAnimation>();
				_itemList.Add(animation);
                Destroy(go , 2.2f);//Remove go Object
            }

			// Initialize object
			animation.Initialize(itemDisplayer, popPosition, Vector3.zero, Vector3.one, _playSoundMode, _expansionMode, this);
			// Start animation
			animation.StartAnimation(null);
			yield return delay;
		}
	}

    IEnumerator PopItems(int quantity, RectTransform itemDisplayer, RectTransform popPos, GameObject itemPrefab, UnityAction done)
    {
        WaitForSeconds delay = new WaitForSeconds(emissionRate);
        for (int i = 0; i < quantity; i++)
        {
            RoyalCollectingAnimation animation = null;
            if (i < _itemList.Count)
            {
                if (!_itemList[i]._animationRunning)
                {
                    // A free object has been found in pool, so we reuse it
                    animation = _itemList[i];
                }
            }
            if (animation == null)
            {
                // No free object has been found in pool, so we instantiate a new one
                GameObject go = Instantiate(itemPrefab) as GameObject;
                animation = go.GetComponent<RoyalCollectingAnimation>();
                _itemList.Add(animation);
                Destroy(go, 2.2f);//Remove go Object
            }

            // Initialize object
            animation.Initialize(itemDisplayer, popPos, Vector3.zero, Vector3.one, _playSoundMode, _expansionMode, this);
            // Start animation
            animation.StartAnimation(null);
            yield return delay;
        }
        if (done != null)
        {
            done.Invoke();
        }
    }

    IEnumerator UsePopItems(int quantity, RectTransform destPos, RectTransform itemDisplayer, RectTransform popPos, GameObject itemPrefab, UnityAction done)
    {
        WaitForSeconds delay = new WaitForSeconds(emissionRate);
        for (int i = 0; i < quantity; i++)
        {
            RoyalCollectingAnimation animation = null;
            if (i < _itemList.Count)
            {
                if (!_itemList[i]._animationRunning)
                {
                    // A free object has been found in pool, so we reuse it
                    animation = _itemList[i];
                }
            }
            if (animation == null)
            {
                // No free object has been found in pool, so we instantiate a new one
                GameObject go = Instantiate(itemPrefab) as GameObject;
                animation = go.GetComponent<RoyalCollectingAnimation>();
                _itemList.Add(animation);
                Destroy(go, 2.2f);//Remove go Object
            }

            // Initialize object
            animation.Initialize(destPos, itemDisplayer, popPos, Vector3.zero, Vector3.one, _playSoundMode, _expansionMode, this);
            // Start animation
            animation.StartAnimation(done);
            yield return delay;
        }
    }

    // Play the collecting sound
    public void PlayCollectingSound()
    {
        _collectingSfxInstance.start();

    }
}
