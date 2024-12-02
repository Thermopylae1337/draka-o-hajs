using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static Utils;

public class Wheel : MonoBehaviour
{
    private float delta, angleStep, angleStepRad;

    [SerializeField] private bool spinning = false;
    private readonly int numberOfSegments = 29;       // Ilosc segmentow kola

    [SerializeField] private GameObject segmentPrefab;              // Prefab pojedynczego segmentu

    public delegate void WheelStoppedHandler(int wynik);
    public event WheelStoppedHandler OnWheelStopped;

    private float targetAngle;
    private float angle = 0.0f;


    private void Start()
    {
        angleStep = 360f / numberOfSegments;
        angleStepRad = angleStep * Mathf.Deg2Rad;

        GenerateWheel();
    }

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
                    wynik = numberOfSegments - 1;    // dla ostatniej kategorii wynik = 0 - 1

                Debug.Log(wynik.ToString());
                Debug.Log("Kategoria: " + CATEGORY_NAMES[wynik]);

                OnWheelStopped?.Invoke(wynik);
            }
        }
    }

    public void SpinWheel(float angle)
    {
        if (!spinning)
        {
            spinning = true;
            targetAngle = angle+this.angle;
        }
    }

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

            if (i%2 == 0)
                segmentImage.color = new Color(48f / 255f, 152f / 255f, 223f / 255f, 1f);

            // Obr�t tekstu na wycinku
            TextMeshProUGUI textComponent = segment.GetComponentInChildren<TextMeshProUGUI>(); // Pobranie komponentu tekstowego
            textComponent.rectTransform.localRotation = Quaternion.Euler(0, 0, ( -angleStep * 0.5f ) - 90f);

            // Pozycja tekstu na wycinku
            float d = 4 * 36 * Mathf.Sin(angleStepRad / 2) / ( 3 * angleStepRad );    // odległość środka koła
            // Ewentualnie d = r / 2 -> lepiej wykorzystuje miejsce
            float x = -d * Mathf.Sin(angleStepRad * 0.5f);
            float y = -d * Mathf.Cos(angleStepRad * 0.5f);
            textComponent.rectTransform.localPosition = new Vector2(x, y);

            if (CATEGORY_NAMES[i] == "Czarna skrzynka")
            {
                segmentImage.color = Color.black;
                textComponent.color = Color.white;
            }
            else if (CATEGORY_NAMES[i] == "Podpowiedź")
            {
                segmentImage.color = new Color(1f, 231f / 255f, 13f / 255f, 1f);  //yellow
            }

            textComponent.text = CATEGORY_NAMES[i];
            textComponent.text += i;
        }
    }
}
