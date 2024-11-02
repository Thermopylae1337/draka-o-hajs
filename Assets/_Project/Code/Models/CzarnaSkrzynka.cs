
using System;

namespace Assets
{
    public class CzarnaSkrzynka
    {
        private String nazwa;
        private TypCzarnejSkrzynki typ;

        public CzarnaSkrzynka()
        {
            Array typySkrzynek = Enum.GetValues(typeof(TypCzarnejSkrzynki));
            Random random = new Random();

            //stworzenie czarnej skrzynki o losowym typie
            typ = (TypCzarnejSkrzynki) typySkrzynek.GetValue(random.Next(typySkrzynek.Length));
            nazwa = typ.FriendlyNazwa();
        }
        public void Aktywacja()
        {
            //todo
        }
    }
}
