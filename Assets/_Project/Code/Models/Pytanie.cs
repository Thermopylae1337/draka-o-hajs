public class Pytanie
{
    public Pytanie()
    {

        public string Tresc { get; private set; }
    private List<string> poprawneOdpowiedzi = new List<string>();
    private Dictionary<char, string> odpowiedzi;


    public Pytanie(string Tresc, List<string> poprawneOdpowiedzi, string odpA, string odpB, string odpC, string odpD)
    {
        this.Tresc = Tresc;
        this.poprawneOdpowiedzi = poprawneOdpowiedzi; // podane jako lista poprawne warianty odpowiedzi

        odpowiedzi = new Dictionary<char, string>
                {
                    {'A', odpA},
                    {'B', odpB},
                    {'C', odpC},
                    {'D', odpD}
                };
    }

    public bool czyPoprawna(string odpowiedz)
    {
        return poprawneOdpowiedzi.Contains(odpowiedz.Trim().ToLower());
    }

    public string Podpowiedz()
    {
        return $"A: {odpowiedzi['A']}, B: {odpowiedzi['B']}, C: {odpowiedzi['C']}, D: {odpowiedzi['D']}";
    }
}
}
