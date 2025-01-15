using UnityEngine;
using UnityEngine.UI;

namespace Assets.SimpleSpinner
{
    /// <summary>
    /// Klasa odpowiedzialna za tworzenie efektu obracającego się koła z efektem tęczy. Umożliwia dostosowanie prędkości rotacji, animacji oraz efektu kolorowej tęczy w interfejsie użytkownika.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class SimpleSpinner : MonoBehaviour
    {
        /// <summary>
        /// Zmienna przechowująca informacje o rotacji koła. Dodaje nagłówek w edytorze Unity, który pomaga w organizacji kodu.
        /// </summary>
        [Header("Rotation")]
        public bool Rotation = true;

        /// <summary>
        /// Prędkość rotacji obiektu, mierzona w obrotach na sekundę (Hz). 
        /// </summary>
        [Range(-10, 10), Tooltip("Value in Hz (revolutions per second).")]
        public float RotationSpeed = 1;

        /// <summary>
        /// Pole przechowujące referencje do obiektu klasy AnimationCurve, który kontroluje płynność rotacji (animacja koła).
        /// </summary>
        public AnimationCurve RotationAnimationCurve = AnimationCurve.Linear(0, 0, 1, 1);

        /// <summary>
        /// Zmienna określajaca czy ma być zastosowany efekt tęczy.
        /// </summary>
        [Header("Rainbow")]
        public bool Rainbow = true;

        /// <summary>
        /// Prędkość zmiany kolorów w tęczy, określa jak szybko zmieniają się kolory tęczy.
        /// </summary>
        [Range(-10, 10), Tooltip("Value in Hz (revolutions per second).")]
        public float RainbowSpeed = 0.5f;

        /// <summary>
        /// Pole przechowujące nasycenie kolorów tęczy.
        /// </summary>
        [Range(0, 1)]
        public float RainbowSaturation = 1f;

        /// <summary>
        /// Pole przechowujace referencje do obiektu klasy AnimationCurve, który kontroluje płynność zmiany kolorów.
        /// </summary>
        public AnimationCurve RainbowAnimationCurve = AnimationCurve.Linear(0, 0, 1, 1);

        /// <summary>
        /// Pole wskazujące czy okres zmiany kolorów i rotacji ma być losowy.
        /// </summary>
        [Header("Options")]
        public bool RandomPeriod = true;

        /// <summary>
        /// Pole przechowujące obrazek koła.
        /// </summary>
        private Image _image;

        /// <summary>
        /// Pole przechowujace okres dla animacji rotacji.
        /// </summary>
        private float _period;

        /// <summary>
        /// Metoda inicjalizująca, która ustawia komponent Image i generuje losowy okres animacji.
        /// </summary>
        public void Start()
        {
            _image = GetComponent<Image>();
            _period = RandomPeriod ? Random.Range(0f, 1f) : 0;
        }

        /// <summary>
        /// Metoda aktualizująca, która jest wywoływana co klatkę. 
        /// </summary>
        public void Update()
        {
            if (Rotation)
            {
                transform.localEulerAngles = new Vector3(0, 0, -360 * RotationAnimationCurve.Evaluate((RotationSpeed * Time.time + _period) % 1));
            }

            if (Rainbow)
            {
                _image.color = Color.HSVToRGB(RainbowAnimationCurve.Evaluate((RainbowSpeed * Time.time + _period) % 1), RainbowSaturation, 1);
            }
        }
    }
}