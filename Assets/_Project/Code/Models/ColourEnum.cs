using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum ColourEnum
{
    BLUE,
    GREEN,
    YELLOW
}
public static class ColorHelper
{
    public static Color ToUnityColor(this ColourEnum kolor)
    {
        return kolor switch
        {
            ColourEnum.YELLOW => Color.yellow,
            ColourEnum.GREEN => Color.green,
            ColourEnum.BLUE => Color.blue,
            _ => Color.white // default
        };
    }
}
