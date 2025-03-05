using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;

    private void Awake()
    {
        //serverBtn.onClick.AddListener(() =>
        //{
        //    NetworkManager.Singleton.StartServer();
        //});

        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            HideNetworkUI();
        });

        clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            HideNetworkUI();
        });
    }

    private void HideNetworkUI()
    {
        this.gameObject.SetActive(false);
    }
}
