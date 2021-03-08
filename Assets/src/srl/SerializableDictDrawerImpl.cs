using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

// -------------------------------------------
//  string => int
// -------------------------------------------
[UnityEditor.CustomPropertyDrawer(typeof(DictStringInt))]
public class StringIntDictionaryDrawer : SerializableDictDrawer<string, int>
{
    protected override SerializableKeyValueTemplate<string, int> GetTemplate()
    {
        return GetGenericTemplate<SerializableStringIntTemplate>();
    }
}
internal class SerializableStringIntTemplate : SerializableKeyValueTemplate<string, int> { }

// -------------------------------------------
//  GameObject => float
// -------------------------------------------
[UnityEditor.CustomPropertyDrawer(typeof(DictGameObjectFloat))]
public class GameObjectFloatDictionaryDrawer : SerializableDictDrawer<GameObject, float>
{
    protected override SerializableKeyValueTemplate<GameObject, float> GetTemplate()
    {
        return GetGenericTemplate<SerializableGameObjectFloatTemplate>();
    }
}
internal class SerializableGameObjectFloatTemplate : SerializableKeyValueTemplate<GameObject, float> { }