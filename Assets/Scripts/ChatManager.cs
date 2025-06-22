using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public static ChatManager instance;
    private TMP_InputField input;
    private TMP_Text received;
    private bool isEnabled;
    private bool isCapsActive;
    private bool isMicrophoneActive;
    public Sprite microphoneEnabledTexture;
    public Sprite microphoneDisabledTexture;
    public Text fpsField;
    private int numberOfMessages;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }
    
    void Start()
    {
        GameObject panel = this.transform.GetChild(0).gameObject;
        GameObject inputObject = panel.transform.GetChild(0).gameObject;
        GameObject receiveObject = panel.transform.GetChild(1).gameObject;
        
        input = inputObject.GetComponent<TMP_InputField>();
        received = receiveObject.GetComponent<TMP_Text>();
        isEnabled = true;
        isCapsActive = false;
        isMicrophoneActive = true;

        numberOfMessages = 0;
        OnButtonStart(null);
    }
    
    void Update()
    {
        fpsField.text = "FPS: " + ((int)(1 / Time.deltaTime)).ToString();
    }
    
    public void OnButtonClicked(Button btn)
    {
        string text = btn.GetComponent<Button>().GetComponentInChildren<Text>().text;

        switch (text)
        {
            case "ENTER":
                Client.instance.SendChatMessage(input.text);
                input.text = "";
                break;
            case "CAPS":
                if (isCapsActive)
                {
                    isCapsActive = false;
                    btn.GetComponent<Image>().color = Color.white;
                    return;
                }
                else
                {
                    isCapsActive = true;
                    btn.GetComponent<Image>().color = Color.green;
                    return;
                }
                break;
            case "MICROPHONE":
                if (isMicrophoneActive)
                {
                    btn.GetComponent<Image>().sprite = microphoneDisabledTexture;
                    AudioManager.instance.MicrophoneDisable();
                    isMicrophoneActive = false;
                }
                else
                {
                    btn.GetComponent<Image>().sprite = microphoneEnabledTexture;
                    AudioManager.instance.MicrophoneEnable();
                    isMicrophoneActive = true; ;
                }
                break;
            case "DEL":
                input.text = input.text.Substring(0, input.text.Length - 1);
                break;
            case "CLEAR":
                input.text = "";
                break;
            case "SPACE":
                input.text = input.text +  " ";
                break;
            default:
                if(isCapsActive == false)
                    input.text = input.text + text.ToLower();
                else
                    input.text = input.text + text;
                break;
        }

        StartCoroutine(FadeColor(btn));
    }
    
    IEnumerator FadeColor(Button btn)
    {
        float t = 0;
        float timeToChange = 1;
        while (t < timeToChange)
        {
            t += Time.deltaTime;
            btn.GetComponent<Image>().color = Color.Lerp(Color.green, Color.white, t / timeToChange);
            yield return null;
        }
    }

    public void OnReceiveMessage(string msg, int senderId)
    {
        if (numberOfMessages < 1)
        {
            received.text = received.text + GameManager.players[senderId].username + ": " + msg;
            numberOfMessages++;
        }
        else if (numberOfMessages < 6)
        {
            received.text = received.text + "\n" + GameManager.players[senderId].username + ": " + msg;
            numberOfMessages++;
        }
        else
        {
            string[] subs = received.text.Split('\n');
            received.text = subs[1] + "\n" + subs[2] + "\n" + subs[3] + "\n" + subs[4] + "\n" + subs[5] + "\n" + GameManager.players[senderId].username + ": " + msg;
        }
    }

    public void OnButtonStart(Camera camera)
    {
        GameObject uiGameObject = this.transform.gameObject;
        if (isEnabled)
        {
            isEnabled = false;
            uiGameObject.SetActive(false);
        }
        else
        {
            isEnabled = true;
            uiGameObject.SetActive(true);
            uiGameObject.GetComponent<RectTransform>().eulerAngles = new Vector3(0, camera.transform.eulerAngles.y,0);
            Vector3 cameraPos = camera.transform.position;
            Vector3 cameraDirection = camera.transform.forward;
            float spawnDistance = 2.0f;
            Vector3 spawnPos = cameraPos + cameraDirection * spawnDistance;
            uiGameObject.GetComponent<RectTransform>().position = spawnPos;
        }
    }
}
