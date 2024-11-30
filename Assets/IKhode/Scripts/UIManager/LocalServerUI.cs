using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class LocalServerUI : MonoBehaviour
{
    public NetworkManager _networkManager;  // Reference to the NetworkManager
    public Button HostButton;              // Button for starting the host
    public Button ClientButton;            // Button for joining as a client
    public Button DisconnectButton;        // Button for disconnecting from the game

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Assign button click events
        HostButton.onClick.AddListener(OnHostButtonClicked);
        ClientButton.onClick.AddListener(OnClientButtonClicked);
        DisconnectButton.onClick.AddListener(OnDisconnectButtonClicked);
        
        // Ensure DisconnectButton is initially inactive (you can make it active when connected)
        DisconnectButton.gameObject.SetActive(false);
    }

    // Handler for the Host button click
    private void OnHostButtonClicked()
    {
        if (!_networkManager.IsServer && !_networkManager.IsClient) // Check if we're not already connected
        {
            _networkManager.StartHost(); // Start the server and host the game
            Debug.Log("Hosting game as server");

            // Disable Host and Client buttons, enable Disconnect button
            HostButton.gameObject.SetActive(false);
            ClientButton.gameObject.SetActive(false);
            DisconnectButton.gameObject.SetActive(true);
        }
    }

    // Handler for the Client button click
    private void OnClientButtonClicked()
    {
        if (!_networkManager.IsServer && !_networkManager.IsClient) // Ensure we're not already connected
        {
            _networkManager.StartClient(); // Start the client and connect to the server
            Debug.Log("Connecting to server as client");

            // Disable Host and Client buttons, enable Disconnect button
            HostButton.gameObject.SetActive(false);
            ClientButton.gameObject.SetActive(false);
            DisconnectButton.gameObject.SetActive(true);
        }
    }

    // Handler for the Disconnect button click
    private void OnDisconnectButtonClicked()
    {
        if (_networkManager.IsServer)
        {
            _networkManager.Shutdown(); // Shutdown the server (host)
            Debug.Log("Server has disconnected.");
        }
        else if (_networkManager.IsClient)
        {
            _networkManager.Shutdown(); // Shutdown the client connection
            Debug.Log("Client has disconnected.");
        }

        // After disconnecting, reset UI buttons
        HostButton.gameObject.SetActive(true);
        ClientButton.gameObject.SetActive(true);
        DisconnectButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Any logic you want to update every frame can be added here
    }
}
