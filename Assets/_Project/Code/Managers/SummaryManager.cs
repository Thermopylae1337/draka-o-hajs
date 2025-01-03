using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using TMPro;
using UnityEngine.Video;
using System.Collections;
using UnityEngine.UI;

public class SummaryManager : NetworkBehaviour
{
    [SerializeField] private GameObject panelPrefab;
    [SerializeField] private Transform grid;
    public TextMeshProUGUI teamDrawingText;
    public TextMeshProUGUI[] boxesText; // 0 gdy otwarta jest jedna skrzynka ,(1-2)  gdy otwarte są 2 skrzynki 
    public VideoPlayer[] boxOpeningVideoPlayer; //0-otwieranie jednej skrzynki, 1-otwieranie 2 skrzynek
    public RawImage videoCanvas;
    private static readonly System.Random _random = new();

    [Serializable]
    public struct PrizeData : INetworkSerializable
    {
        public string teamName;
        public int money;
        public string badge;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            teamName ??= string.Empty;
            badge ??= string.Empty;

            serializer.SerializeValue(ref teamName);
            serializer.SerializeValue(ref money);
            serializer.SerializeValue(ref badge);
        }
    }

    [Serializable]
    public struct PrizeDataList : INetworkSerializable
    {
        public PrizeData[] prizes;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            int count = prizes?.Length ?? 0;
            serializer.SerializeValue(ref count);

            if (serializer.IsReader)
            {
                prizes = new PrizeData[count];
            }

            for (int i = 0; i < count; i++)
            {
                prizes[i].NetworkSerialize(serializer);
            }
        }
    }

    private void Start()
    {
       
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost)
        {
            StartCoroutine(HandleTeams());
        }
    }

    private IEnumerator HandleTeams()
    {
        foreach (NetworkClient teamClient in NetworkManager.ConnectedClientsList)
        {
            ulong clientId = teamClient.ClientId;
     
            CalculatePrizeServerRpc(clientId);

            yield return new WaitUntil(() => videoCanvas.gameObject.activeSelf == false);

            CreatePanelClientRpc(clientId);

            yield return new WaitForSeconds(3f);
        }
    }

    [ClientRpc]
    private void CreatePanelClientRpc(ulong clientId)
    {

        TeamManager team = NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<TeamManager>();

        GameObject panelObject = Instantiate(panelPrefab, grid);
        Panel panel = panelObject.GetComponent<Panel>();
        panel.Initialize(team);

    }

    [ServerRpc(RequireOwnership = false)]
    private void CalculatePrizeServerRpc(ulong clientId)
    {
        TeamManager team = NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<TeamManager>();
        team.BlackBoxes = _random.Next(2, 3); // Randomize between 1 and 2
        Debug.Log($"Team {team.name} has {team.BlackBoxes} black boxes.");

        if (team.BlackBoxes > 0)
        {
            PrizeData[] prizeDatas = new PrizeData[team.BlackBoxes];
            for (int i = 0; i < team.BlackBoxes; i++)
            {
                prizeDatas[i] = DrawBlackBox(team);
            }

            PrizeDataList prizeDataList = new() { prizes = prizeDatas };
            DisplayPrizeClientRpc(prizeDataList);
        }
    }

    [ClientRpc]
    private void DisplayPrizeClientRpc(PrizeDataList prizeDataList)
    {
        PrizeData[] prizeList = prizeDataList.prizes;
        teamDrawingText.text = $"Drużyna {prizeList[0].teamName} losuje czarną skrzynkę:";

        if (prizeList.Length == 1)
        {
            boxesText[0].text = prizeList[0].money > 0 ? prizeList[0].money.ToString() : prizeList[0].badge;
            StartCoroutine(PlayVideo(1));
        }
        else
        {
            boxesText[1].text = prizeList[0].money > 0 ? prizeList[0].money.ToString() : prizeList[0].badge;
            boxesText[2].text = prizeList[1].money > 0 ? prizeList[1].money.ToString() : prizeList[1].badge;
            StartCoroutine(PlayVideo(2));
        }
    }

    private IEnumerator PlayVideo(int i)
    {

        VideoPlayer videoPlayer = ( i == 1 ) ? boxOpeningVideoPlayer[0] : boxOpeningVideoPlayer[1];
        bool videoFinished = false;

        // Clear the RenderTexture
        
        if (videoPlayer.targetTexture != null)
        {
            RenderTexture renderTexture = videoPlayer.targetTexture;
            RenderTexture.active = renderTexture;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = null;
        }

        videoCanvas.gameObject.SetActive(true);
        teamDrawingText.gameObject.SetActive(true);



        videoPlayer.loopPointReached += (vp) => { videoFinished = true; };
        videoPlayer.Play();

        yield return new WaitUntil(() => videoFinished);

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
        DeactivateAll(); 

        videoPlayer.targetTexture.Release();
    
    }

    private void DeactivateAll()
    {

        foreach (TextMeshProUGUI text in boxesText)
        {
            text.gameObject.SetActive(false);
        }

        teamDrawingText.gameObject.SetActive(false);
        videoCanvas.gameObject.SetActive(false);
    }

    private PrizeData DrawBlackBox(TeamManager team)
    {
        double los = _random.NextDouble();
        PrizeData prize = new()
        {
            teamName = team.name ?? string.Empty,
            money = 0,
            badge = string.Empty 
        };

        if (los < 0.8) 
        {
            int money = DrawMoney();
            team.Money += money;
            prize.money = money;
            prize.badge = string.Empty;
            Debug.Log($"{team.name} won {money}");
        }
        else 
        {
            string badge = DrawBadge();
            prize.money = 0; 
            prize.badge = badge ?? string.Empty;
            Debug.Log($"{team.name} won {badge}");
        }

        return prize;
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
