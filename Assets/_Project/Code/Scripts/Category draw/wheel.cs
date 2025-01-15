using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utils;

/// <summary>
/// Klasa odpowiedzialna za zarządzanie kołem fortuny w grze. Zawiera logiką obracania koła, generowania segmentów oraz wykrywania momentu, w którym koło zatrzyma się, aby ogłosić wynik.
/// </summary>
public class Wheel : MonoBehaviour
{
    /// <summary>
    /// Zmienne przechowujące wartości delty, kąta oraż konwersje kąta radianów.
    /// </summary>
    private float delta, angleStep, angleStepRad;

    /// <summary>
    /// Zmienna przechowująca stan koła (czy jest w trakcie obracania).
    /// </summary>
    [SerializeField] private bool spinning = false;
    /// <summary>
    /// Zmienna przechowująca ilość segmentów na kole.
    /// </summary>
    private readonly int numberOfSegments = 29;       // Ilosc segmentow kola

    /// <summary>
    /// Prefab pojedynczego segmentu koła.
    /// </summary>
    [SerializeField] private GameObject segmentPrefab;              // Prefab pojedynczego segmentu

    /// <summary>
    /// Delegat wywoływany po zatrzymaniu koła.
    /// </summary>
    /// <param name="wynik">Wynik losowania (np. numer segmentu, na którym zatrzymało się koło).</param>
    public delegate void WheelStoppedHandler(int wynik);
    /// <summary>
    /// Event wywoływany po zatrzymaniu koła. 
    /// </summary>
    public event WheelStoppedHandler OnWheelStopped;

    /// <summary>
    /// Zmienna przechowująca docelowy kąt, do którego koło ma się obrócić.
    /// </summary>
    private float targetAngle;
    /// <summary>
    /// Prywatna zmienna przechowująca aktualną wartość kąta. 
    /// </summary>
    private float angle = 0.0f;

    /// <summary>
    /// Inicjalizuje zmienne koła, oblicza kąt obrotu oraz wywołuje metode generującą koło.
    /// </summary>
    private void Start()
    {
        angleStep = 360f / numberOfSegments;
        angleStepRad = angleStep * Mathf.Deg2Rad;

        GenerateWheel();
    }

    /// <summary>
    /// Aktualizuje stan koła fortuny, obracając je do docelowego kąta. Po zakończeniu obrotu, wyzwala zdarzenie z wynikiem losowania.
    /// </summary>
    private void Update()
    {
        delta = Time.deltaTime;

        if (spinning)
        {
            angle = Mathf.Lerp(angle, targetAngle, 1 * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            if (Mathf.Abs(angle - targetAngle) < 0.1f)
            {
                spinning = false;

                // Indeks wylosowanej kategorii
                float correctedAngle = targetAngle + ( angleStep * 0.5f );
                int wynik = (int)( Mathf.Round(correctedAngle / angleStep) % numberOfSegments ) - 1;
                if (wynik < 0)
                {
                    wynik = numberOfSegments - 1;    // dla ostatniej kategorii wynik = 0 - 1
                }

                Debug.Log(wynik.ToString());
                Debug.Log("Kategoria: " + categoryNames[wynik]);

                OnWheelStopped?.Invoke(wynik);
            }
        }
    }

    /// <summary>
    /// Metoda umożlwiająca obracanie się koła.
    /// </summary>
    /// <param name="angle">Zmienna reprezentująca kąt obrotu.</param>
    public void SpinWheel(float angle)
    {
        if (!spinning)
        {
            spinning = true;
            targetAngle = angle + this.angle;
        }
    }

    /// <summary>
    /// Metoda generująca koło fortuny na którym znajdują sie różne kategorie.
    /// </summary>
    private void GenerateWheel()
    {
        for (int i = 0; i < numberOfSegments; i++)
        {
            // Nowa instancja segmentu
            GameObject segment = Instantiate(segmentPrefab, transform);
            segment.transform.localPosition = Vector3.zero;
            segment.transform.localRotation = Quaternion.Euler(0, 0, ( i * -angleStep ) + 180f);

            // Ustawienie k�ta wycinka
            Image segmentImage = segment.GetComponent<Image>();
            segmentImage.fillAmount = 1f / numberOfSegments; // Rozmiar wycinka

            if (i % 2 == 0)
            {
                segmentImage.color = new Color(48f / 255f, 152f / 255f, 223f / 255f, 1f);
            }

            // Obr�t tekstu na wycinku
            TextMeshProUGUI textComponent = segment.GetComponentInChildren<TextMeshProUGUI>(); // Pobranie komponentu tekstowego
            textComponent.rectTransform.localRotation = Quaternion.Euler(0, 0, ( -angleStep * 0.5f ) - 90f);

            // Pozycja tekstu na wycinku
            float d = 4 * 36 * Mathf.Sin(angleStepRad / 2) / ( 3 * angleStepRad );    // odległość środka koła
            // Ewentualnie d = r / 2 -> lepiej wykorzystuje miejsce
            float x = -d * Mathf.Sin(angleStepRad * 0.5f);
            float y = -d * Mathf.Cos(angleStepRad * 0.5f);
            textComponent.rectTransform.localPosition = new Vector2(x, y);

            if (categoryNames[i] == "Czarna skrzynka")
            {
                segmentImage.color = Color.black;
                textComponent.color = Color.white;
            }
            else if (categoryNames[i] == "Podpowiedź")
            {
                segmentImage.color = new Color(1f, 231f / 255f, 13f / 255f, 1f);  //yellow
            }

            textComponent.text = categoryNames[i];
            textComponent.text += i;
        }
    }
}
