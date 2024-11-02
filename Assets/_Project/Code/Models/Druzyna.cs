using System;
using System.Collections.Generic;

namespace Assets
{
    public class Druzyna
    {
        public String nazwa;
        public int podpowiedzi;
        public List<CzarnaSkrzynka> czarneSkrzynki = new List<CzarnaSkrzynka>();

        public void przyznajPodpowiedz()
        {
            podpowiedzi++;
            Console.WriteLine("Drużyna " + nazwa + " otrzymuje podpowiedź!"); //todo wyświetlenie w grze na UI
        }
        public void przyznajCzarnaSkrzynke()
        {
            CzarnaSkrzynka skrzynka = new CzarnaSkrzynka();
            czarneSkrzynki.Add(skrzynka);
            Console.WriteLine("Drużyna " + nazwa + " otrzymuje czarną skrzynkę!"); //todo wyświetlenie w grze na UI
        }
    }
}
