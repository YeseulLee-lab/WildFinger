using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public static class JsonHelper
{
    public static string ObjectToJson(object obj)
    {
        Newtonsoft.Json.JsonSerializerSettings s = new Newtonsoft.Json.JsonSerializerSettings();
        s.Formatting = Formatting.Indented;
        s.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        return JsonConvert.SerializeObject(obj, s);
    }

    public static T JsonToObject<T>(string jsonData)
    {
        return JsonConvert.DeserializeObject<T>(jsonData);
    }

    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    public static string fixJson(string value)
    {
        string result = "{\"Items\":" + value + "}";
        return result;
    }

    public static string[] FixTest(string value)
    {
        value = "{\"Items\":" + value + "}";

        int startIndex = 0;
        int subStart = 0;
        int removeStart = 0;
        int finishIndex = value.Length - 1;
        char[] c = value.ToCharArray();
        string sub = "";

        for (int i = startIndex; i < c.Length; i++)
        {
            if (c[i] == '[')
            {
                bool quoteStart = false;
                for (int ii = i; ii >= 0; ii--)
                {
                    if (!quoteStart && c[ii] == '\"')
                    {
                        quoteStart = true;
                    }
                    else if (quoteStart && c[ii] == '\"')
                    {
                        subStart = ii;
                    }
                    else if(c[ii] == ',' || c[ii] == '{')
                    {
                        removeStart = ii;
                        break;
                    }
                }
                startIndex = i;
                break;
            }
        }

        for (int i = finishIndex; i >= 0 || finishIndex > startIndex; i--)
        {
            if (c[i] == ']')
            {
                finishIndex = i;
                break;
            }
        }

        sub = value;
        sub = sub.Insert(startIndex, "{ \"Items\": ");
        sub = sub.Insert(finishIndex + 1, "}");
        sub = sub.Substring(subStart, finishIndex + "{ \"Items\": ".Length + 1 - subStart);

        value = value.Remove(removeStart, finishIndex - removeStart+1);

        string[] result = new string[] { sub, value };

        return result;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}