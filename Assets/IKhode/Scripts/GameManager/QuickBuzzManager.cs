using System;
using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class QuickBuzzManager : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI categoryText;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button buzzButton;
    [SerializeField] private TMP_InputField answerInput;
    [SerializeField] private Button submitAnswerButton;
    [SerializeField] private Image questionImage;

    private List<QuestionData> questions;
    private QuestionData currentQuestion;

    public struct QuestionData
    {
        public string Category;
        public string Question;
        public string Answer;
        public string ImageUrl;
    }

    private bool buzzed = false;
    private ulong buzzedPlayerId;

    private void Start()
    {

        buzzButton.onClick.AddListener(OnBuzzPressed);
        submitAnswerButton.onClick.AddListener(OnSubmitAnswer);
    }
    public override void OnNetworkSpawn() {
        if (IsServer)
        {
            LoadQuestions();
            ShowNextQuestion();
        }
    }

    public void LoadQuestions()
    {
        string path = Path.Combine(Application.dataPath, "Ikhode/Resources/QA.json");
        if (File.Exists(path))
        {
            string jsonContent = File.ReadAllText(path);
            questions = JsonConvert.DeserializeObject<List<QuestionData>>(jsonContent);
        }
        else
        {
            Debug.LogError("QA.json file not found!");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnBuzzPressedServerRpc(ulong playerId)
    {
        if (!buzzed)
        {
            buzzed = true;
            buzzedPlayerId = playerId;

            Debug.Log($"Player {playerId} buzzed first!");
            EnableAnswerInputClientRpc(playerId);
        }
    }

    private void OnBuzzPressed()
    {
        if (IsClient)
        {
            OnBuzzPressedServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    [ClientRpc]
    private void EnableAnswerInputClientRpc(ulong playerId)
    {
        if (NetworkManager.Singleton.LocalClientId == playerId)
        {
            answerInput.gameObject.SetActive(true);
            submitAnswerButton.gameObject.SetActive(true);
        }
    }

    private void OnSubmitAnswer()
    {
        string playerAnswer = answerInput.text;

        if (IsClient)
        {
            SubmitAnswerServerRpc(NetworkManager.Singleton.LocalClientId, playerAnswer);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SubmitAnswerServerRpc(ulong playerId, string answer)
    {
        if (playerId == buzzedPlayerId)
        {
            if (answer.Trim().ToLower() == currentQuestion.Answer.Trim().ToLower())
            {
                Debug.Log($"Player {playerId} answered correctly: {answer}");
                ShowNextQuestion();
            }
            else
            {
                Debug.Log($"Player {playerId} answered incorrectly: {answer}");
                buzzed = false; // Allow others to buzz
            }
        }
    }

    public void ShowNextQuestion()
    {
        if (questions.Count == 0)
        {
            Debug.Log("No more questions!");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, questions.Count);
        currentQuestion = questions[randomIndex];
        questions.RemoveAt(randomIndex);

        UpdateQuestionClientRpc(currentQuestion.Category, currentQuestion.Question, currentQuestion.ImageUrl);
        buzzed = false;
        buzzedPlayerId = ulong.MaxValue;

        ResetAnswerInputClientRpc();
    }

    [ClientRpc]
    private void ResetAnswerInputClientRpc()
    {
        answerInput.text = string.Empty;
        answerInput.gameObject.SetActive(false);
        submitAnswerButton.gameObject.SetActive(false);
    }

    [ClientRpc]
    private void UpdateQuestionClientRpc(string category, string question, string imageUrl)
    {
        categoryText.text = category;
        questionText.text = question;

        StartCoroutine(LoadImageCoroutine(imageUrl));
    }

    private IEnumerator LoadImageCoroutine(string imageUrl)
    {
        // Validate URL using the Uri class
        if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri uriResult) || 
            !(uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
        {
            Debug.LogError("Invalid image URL provided.");
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || 
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Failed to load image: {request.error}");
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                if (texture != null)
                {
                    questionImage.sprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f));
                }
                else
                {
                    Debug.LogError("Failed to create texture from the downloaded image.");
                }
            }
        }
    }

}
