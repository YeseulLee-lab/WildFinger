using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolingKeys
{
    BaseNote,
    Bar,
    NoteHIT,
    NoteSHY,
    NoteSCT,
    NoteHTG,
    NoteJMP,
    NoteDSG,
    MainQuaver,
    MainCoin,
    NoteSCF, //시크릿플릭 1회
    NoteBRD, //빵반죽 4번
    NoteSCH, //시크릿홀드 4박자
    NoteILU, //알러뷰
    NotePAL, //단짝
    NotePCK, //공작새
    NoteGenerator,
}

[System.Serializable]
public class ObjectPool
{
    public GameObject pooling;
    public int Count;
}

public class BaseObjectPool : MonoBehaviour
{
    public static BaseObjectPool Instance { get; private set; }

    public ObjectPool[] _data;
    //수정 필요
    private Dictionary<PoolingKeys, Queue<GameObject>> _poolingChain = new Dictionary<PoolingKeys, Queue<GameObject>>();

    private void Awake()
    {
        Instance = this;
        Initialize();
    }

    private void Initialize()
    {
        for (int i = 0; i < _data.Length; ++i)
        {
            Queue<GameObject> temp = new Queue<GameObject>();
            for (int j = 0; j < _data[i].Count; ++j)
            {
                GameObject obj = Instantiate(_data[i].pooling);
                obj.transform.SetParent(this.transform);
                obj.SetActive(false);
                temp.Enqueue(obj);
            }

            _poolingChain.Add(_data[i].pooling.GetComponent<BaseObjectPoolUnit>().key, temp);
        }
    }

    public void ReturnObject(GameObject obj, GameObject parent = null)
    {
        //DebugX.Log("Return");
        if (obj == null)
        {
            return;
        }

        if (parent != null)
        {
            obj.SetActive(false);
            obj.transform.SetParent(parent.transform);
        }
        else
        {
            obj.SetActive(false);
            obj.transform.SetParent(this.transform);
        }

        //_poolingChain[key].Enqueue(obj);
    }

    public void ClearPool(GameObject parent)
    {
        int count = parent.transform.childCount;
        for (int i = 0; i < count; ++i)
        {
            if(parent.transform.GetChild(0).gameObject.GetComponent<BaseObjectPoolUnit>() == null)
            {
                continue;
            }

            ReturnObject(parent.transform.GetChild(0).gameObject);
        }
    }

    public GameObject Spawn(PoolingKeys key, GameObject parent = null)
    {
        if (_poolingChain.Count > 0)
        {
            if (_poolingChain[key].Count > 0)
            {
                GameObject pool = _poolingChain[key].Dequeue();
                pool.SetActive(true);
                pool.transform.SetParent(parent == null? null : parent.transform);
                _poolingChain[key].Enqueue(pool);
                return pool;
            }
            else
            {
                for (int i = 0; i < _data.Length; ++i)
                {
                    if (_data[i].pooling.GetComponent<BaseObjectPoolUnit>().key.Equals(key))
                    {
                        GameObject pool = Instantiate(_data[i].pooling);
                        pool.SetActive(true);
                        return pool;
                    }
                }
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    //1일 1회 들어가야하는 이유
    //3. RecycleSpawn() : 일정 갯수가 넘어가면 최초의 pool을 다시 enqueue 해서 사용
    public GameObject RecycleSpawn(GameObject parent, PoolingKeys key)
    {
        if (_poolingChain.Count > 0)
        {
            if (_poolingChain[key].Count > 0)
            {
                GameObject pool = _poolingChain[key].Dequeue();
                pool.SetActive(true);
                if (parent != null)
                    pool.transform.SetParent(parent.transform);
                else
                    pool.transform.SetParent(null);
                return pool;
            }
            else
            {
                if (parent.transform.childCount > 0)
                {
                    GameObject pool = parent?.transform.GetChild(0).gameObject;
                    pool.SetActive(true);

                    ReturnObject(pool);
                    Spawn(key, parent);
                    return pool;
                }
                else
                {
                    return null;
                }
            }
        }
        else
        {
            return null;
        }
    }
}
