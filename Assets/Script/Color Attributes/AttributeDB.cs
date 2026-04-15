using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttributeDatabase", menuName = "ColorAttributes/AttributeDatabase")]
public class AttributeDatabase : ScriptableObject
{
    private static AttributeDatabase _instance;
    public static AttributeDatabase Instance
    {
        get
        {
            if (_instance == null)
            {
                // Resources 폴더 안에 파일 이름이 "AttributeDatabase"여야 합니다.
                _instance = Resources.Load<AttributeDatabase>("AttributeDatabase");

                if (_instance == null)
                {
                    Debug.LogError("Resources 폴더에서 AttributeDatabase 파일을 찾을 수 없습니다!");
                }
            }
            return _instance;
        }
    }

    public List<ColorAttributeData> attributes;

    public ColorAttributeData Get(ColorMode mode)
    {
        return attributes.Find(attr => attr.colorMode == mode);
    }
}
