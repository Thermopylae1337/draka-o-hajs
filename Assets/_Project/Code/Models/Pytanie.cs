

public class Pytanie
{
    public string Tresc { get; private set; }
    private List<string> poprawneOdpowiedzi = new List<string>();

    private List<string> odpowiedzi;


    public Pytanie(string Tresc, List<string> poprawneOdpowiedzi, string odpA, string odpB, string odpC, string odpD)
    {
        this.Tresc = Tresc;
        this.poprawneOdpowiedzi = poprawneOdpowiedzi; // podane jako lista poprawne warianty odpowiedzi

        odpowiedzi = new List<string>();
        odpowiedzi.Append(odpA);
        odpowiedzi.Append(odpB);
        odpowiedzi.Append(odpC);
        odpowiedzi.Append(odpD);
    }

    public bool czyPoprawna(string odpowiedz)
    {
        return poprawneOdpowiedzi.Contains(odpowiedz.Trim().ToLower());
    }

    public string Podpowiedz()
    {
        return $"A: {odpowiedzi[0]}, B: {odpowiedzi[1]}, C: {odpowiedzi[2]}, D: {odpowiedzi[3]}";
    }
}

