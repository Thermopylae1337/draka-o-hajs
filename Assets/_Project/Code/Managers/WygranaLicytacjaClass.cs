using Assets;
using System;
using UnityEngine;

public class WygranaLicytacjaClass
{
    
    public void WygranaLicytacja(Druzyna druzyna, Kategoria kategoria)
    {
        if(kategoria == Kategoria.PODPOWIED�)
        {
            druzyna.przyznajPodpowiedz();

        } else if (kategoria == Kategoria.CZARNA_SKRZYNKA)
        {
            druzyna.przyznajCzarnaSkrzynke();

        } else //kategoria pytaniowa
        {
            //etapOdpowiedzi(druzyna, kategoria);
        }
    }

}
