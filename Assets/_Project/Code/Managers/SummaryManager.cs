using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using TMPro;
using UnityEngine.Video;
using System.Collections;
using UnityEngine.UI;
using System.Runtime.InteropServices.WindowsRuntime;

public class SummaryManager : NetworkBehaviour
{
    [SerializeField] private GameObject panelPrefab;
    [SerializeField] private Transform grid;
    public TextMeshProUGUI teamDrawingText;
    public TextMeshProUGUI[] boxesText; // 0 gdy otwarta jest jedna skrzynka ,(1-2)  gdy otwarte są 2 skrzynki 
    public VideoPlayer[] boxOpeningVideoPlayer; //0-otwieranie jednej skrzynki, 1-otwieranie 2 skrzynek
    public RawImage videoCanvas;
    private static readonly System.Random _random = new();

    private void Start()
    {
        StartCoroutine(HandleTeamsWithDelay());
    }

    private IEnumerator HandleTeamsWithDelay()
    {
        foreach (NetworkClient teamClient in NetworkManager.ConnectedClientsList)
        {
            TeamManager team = teamClient.PlayerObject.GetComponent<TeamManager>();

            team.BlackBoxes = _random.Next(1, 2);

            if (team.BlackBoxes > 0 && team != null)
            {

                videoCanvas.gameObject.SetActive(true);
                teamDrawingText.text = $"Czarną skrzynkę losuje drużyna: {team.name}";
                teamDrawingText.color = ColorHelper.ToUnityColor(team.Colour);
                teamDrawingText.gameObject.SetActive(true);

                switch (team.BlackBoxes)
                {
                    case 1:
                        boxesText[0].text = DrawBlackBox(team);
                        yield return StartCoroutine(PlayVideo(1)); // Play video and wait
                        break;

                    case 2:
                        boxesText[1].text = DrawBlackBox(team);
                        boxesText[2].text = DrawBlackBox(team);
                        yield return StartCoroutine(PlayVideo(2)); // Play video and wait
                        break;

                    default:
                        break;
                }  
            }

            DeactivateAll();

            Panel panel = Instantiate(panelPrefab, grid).GetComponent<Panel>();
            panel.Initialize(team);

            // Optional delay between handling teams
            yield return new WaitForSeconds(5f);
        }
    }
    private void DeactivateAll()
    {

        foreach (TextMeshProUGUI text in boxesText)
        {
             text.gameObject.SetActive(false);  
        }
        
        videoCanvas.gameObject.SetActive(false);
        teamDrawingText.gameObject.SetActive(false);
    }

    private IEnumerator PlayVideo(int i)
    {
        VideoPlayer videoPlayer = ( i == 1 ) ? boxOpeningVideoPlayer[0] : boxOpeningVideoPlayer[1];
        bool videoFinished = false;

        // Subscribe to loopPointReached
        videoPlayer.loopPointReached += (vp) => { videoFinished = true; };

        // Play the video
        videoPlayer.Play();

        // Wait for the video to finish
        yield return new WaitUntil(() => videoFinished);

        // After the video ends, display the text
        if (i == 1)
        {
            boxesText[0].gameObject.SetActive(true);
        }
        else
        {
            boxesText[1].gameObject.SetActive(true);
            boxesText[2].gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(7f);
    }

    private string DrawBlackBox(TeamManager team)
    {
        double los = _random.NextDouble();

        if (los < 0.8) // 80% szans na pieniądze
        {
            int money = DrawMoney();
            team.Money += money;
            Debug.Log($"{team.name} wylosowala {money}");
            return money.ToString();
        }
        else // 20% szans na odznakę
        {
            string badge = DrawBadge();
            Debug.Log($"{team.name} wylosowala {badge}");
            //team.Badges.Add?
            return badge;
            
        }
    }

    private static int DrawMoney()
    {
        int[] progi = Enumerable.Range(0, 21).Select(i => i == 0 ? 1 : i * 500).ToArray() ; // progi kwot [1, 500, 1000, .. , 10000]
        double[] szanse = new double[]
        { 0.01 ,0.08, 0.08, 0.08, 0.08, 0.07, 0.07, 0.07, 0.07, 0.06, 0.05, 0.05, 0.05, 0.03, 0.03, 0.03, 0.03, 0.03, 0.01, 0.01, 0.01}; // Szansa na wylosowanie każdego progu

        double los = _random.NextDouble();
        double kumulatywnaSzansa = 0.0;

        for (int i = 0; i < progi.Length; i++)
        {
            kumulatywnaSzansa += szanse[i];
            if (los < kumulatywnaSzansa)
            {
                return progi[i];
            }
        }

        return 1; // Domyślna wartość (nigdy nie powinna wystąpić)
    }

    private  string DrawBadge()
    {
        string[] odznaki = new string[] { "Samochód", "Ogórek" };
        double[] szanse = new double[] { 0.2, 0.8 }; // 20% na samochód, 80% na ogórka

        double los = _random.NextDouble();
        double kumulatywnaSzansa = 0.0;

        for (int i = 0; i < odznaki.Length; i++)
        {
            kumulatywnaSzansa += szanse[i];
            if (los < kumulatywnaSzansa)
            {
                return odznaki[i];
            }
        }

        return "Ogórek"; // Domyślna wartość (nigdy nie powinna wystąpić)
    }

    public void ChangeScene() => SceneManager.LoadScene("MainMenu");   //utils jest statyczne i nie wyswietlaja się w inspektorze w On Click
}
