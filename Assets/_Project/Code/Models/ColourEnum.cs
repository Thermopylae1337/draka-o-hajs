using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Wyliczenie reprezentujące dostępne kolory drużyn.
/// </summary>
public enum ColourEnum
{
    /// <summary>
    /// Kolor reprezentujący drużynę żółtą. 
    /// </summary>
    YELLOW,
    /// <summary>
    /// Kolor reprezentujący drużynę zieloną.
    /// </summary>
    GREEN,
    /// <summary>
    /// Kolor reprezentujący drużynę niebieską.
    /// </summary>
    BLUE

}
/// <summary>
/// Klasa pomocnicza do konwersji wyliczeń kolorów na kolory w Unity.
/// </summary>
public static class ColorHelper
{
    /// <summary>
    /// Konwertuje kolor z wyliczenia na obiekt typu <see cref="Color"/> Unity.
    /// </summary>
    /// <param name="kolor">Kolor w wyliczeniu</param>
    /// <returns>Kolor Unity odpowiadający wybranemu kolorowi z wyliczenia.</returns>
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
