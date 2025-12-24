using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttributeDatabase", menuName = "ColorAttributes/AttributeDatabase")]
public class AttributeDatabase : ScriptableObject
{
    public List<ColorAttributeData> attributes;

    public ColorAttributeData Get(ColorMode mode)
    {
        return attributes.Find(attr => attr.colorMode == mode);
    }
}
