using UnityEngine;

public class Licytacja : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //check if this works without setup
/*[Rpc(SendTo.Everyone)]
void BroadcastPlayerReadyRpc(bool ready, ulong playerId)
{
playersReadyStatus[playerId] = ready;
RefreshPlayerList();
}*/
}
}
