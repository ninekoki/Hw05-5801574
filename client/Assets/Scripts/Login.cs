using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.IO;
using Newtonsoft.Json;

public class Login : MonoBehaviour {

    public GameObject NoticePanel;
    public GameObject LoginPanel;
    public GameObject ScorePanel;

    [SerializeField] Text userText;
    [SerializeField] Text nameText;
    [SerializeField] InputField passText;
    [SerializeField] GameObject createPanel;
    [SerializeField] GameObject playerScorePrefab;

    [SerializeField] internal string newUser;
    [SerializeField] internal string newPassword;
    [SerializeField] internal string newName;
    [SerializeField] internal string newRequest;

    [SerializeField] Player[] players;
    [SerializeField] Player[] sortedPlayers;

    [SerializeField] string URL;

    // Use this for initialization
    void Start()
    {
        GetPlayer();
    }

    void GetPlayer()
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL + "users");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            string responseBody = new StreamReader(stream).ReadToEnd();

            print(responseBody);

            players = JsonConvert.DeserializeObject<Player[]>(responseBody);
            print(players[0].name);
        }
        catch (WebException ex)
        {

        }
    }

    public void RegisterButton()
    {
        GetPlayer();
        if (CreateStepOne() == "01")
        {
            NoticePanel.SetActive(true);
            NoticePanel.transform.GetChild(0).GetComponent<Text>().text = "Please enter username";
            return;
        }
        else if (CreateStepOne() == "02")
        {
            NoticePanel.SetActive(true);
            NoticePanel.transform.GetChild(0).GetComponent<Text>().text = "Please enter password";
            return;
        }
        else if (CreateStepOne() == "03")
        {
            NoticePanel.SetActive(true);
            NoticePanel.transform.GetChild(0).GetComponent<Text>().text = "Your username is dupilcated";
            return;
        }
        else
        {
            newRequest = URL + "user/add/user?" + CreateStepOne();
        }

        createPanel.SetActive(true);

    }

    public void CreateNameButton()
    {
        GetPlayer();
        if (SetName() == "04")
        {
            NoticePanel.SetActive(true);
            createPanel.SetActive(false);
            NoticePanel.transform.GetChild(0).GetComponent<Text>().text = "Please Enter Your Name";
            return;
        }
        else if (SetName() == "05")
        {
            NoticePanel.SetActive(true);
            createPanel.SetActive(false);
            NoticePanel.transform.GetChild(0).GetComponent<Text>().text = "Your name is dupilcated to someone";
            return;
        }
        else
        {
            int newScore = Random.Range(0, 1000);
            newRequest = URL + "user/add/user?" + CreateStepOne() + SetName() + "score=" + newScore;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(newRequest);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            string responseBody = new StreamReader(stream).ReadToEnd();

            print(responseBody);
            createPanel.SetActive(false);

        }
    }

    public string CreateStepOne()
    {
        string newText = "";
        if (userText.text != "")
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (userText.text == players[i].username)
                {
                    return "03";
                }
            }
            newText += ("username=" + userText.text + "&");
            newUser = userText.text;
        }
        else
        {
            return "01";
        }
        if (passText.text != "")
        {
            newText += ("pass=" + passText.text + "&");
            newPassword = passText.text;
            createPanel.SetActive(true);
            return newText;
        }
        else
        {
            return "02";
        }
    }

    public string SetName() {
        string newText;
        if (nameText.text != "")
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (nameText.text == players[i].name)
                {
                    return "05";
                }
            }
            newName = nameText.text;
            newText = ("name=" + nameText.text + "&");
            return newText;
        }
        else
        {
            return "04";
        }
    }

    public void CancelSetName()
    {
        if (createPanel.activeInHierarchy)
        {
            createPanel.SetActive(false);
        }
    }

    public void CancelNotice()
    {
        if (NoticePanel.activeInHierarchy)
        {
            NoticePanel.SetActive(false);
        }
    }

    public void LoginButton()
    {
        GetPlayer();
        if (userText.text != "")
        {
            if (passText.text != "")
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (userText.text == players[i].username)
                    {
                        if (passText.text == players[i].password)
                        {
                            LoginPanel.SetActive(false);
                            sortedPlayers = SortingPlayer();
                            for (int x = 0; x < ScorePanel.transform.GetChild(0).GetChild(0).GetChild(0).childCount; x++)
                            {
                                Destroy(ScorePanel.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(x).gameObject);
                            }
                            for (int j = 0; j < sortedPlayers.Length; j++)
                            {
                                GameObject playerScore = Instantiate(playerScorePrefab, Vector3.zero, Quaternion.identity, ScorePanel.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.transform);
                                playerScore.transform.GetChild(0).GetComponent<Text>().text = sortedPlayers[j].name;
                                playerScore.transform.GetChild(1).GetComponent<Text>().text = (sortedPlayers[j].score).ToString();
                            }
                            NoticePanel.SetActive(false);
                            ScorePanel.SetActive(true);
                        }
                        else
                        {
                            NoticePanel.SetActive(true);
                            NoticePanel.transform.GetChild(0).GetComponent<Text>().text = "Invalid password";
                        }
                    }
                    else
                    {
                        NoticePanel.SetActive(true);
                        NoticePanel.transform.GetChild(0).GetComponent<Text>().text = "Invalid Username";
                    }
                }
            }
            else
            {
                NoticePanel.SetActive(true);
                NoticePanel.transform.GetChild(0).GetComponent<Text>().text = "Please Enter Your Password";
            }
        }
        else
        {
            NoticePanel.SetActive(true);
            NoticePanel.transform.GetChild(0).GetComponent<Text>().text = "Please Enter Your Username";
        }
    }

    public void LogoutButton()
    {
        if (ScorePanel.activeInHierarchy)
        {
            ScorePanel.SetActive(false);
            LoginPanel.SetActive(true);
        }
    }

    Player[] SortingPlayer()
    {
        sortedPlayers = players;
        int max;
        Player temp;
        for (int i = 0; i < sortedPlayers.Length; i++)
        {
            max = i;
            for (int j = i + 1; j < sortedPlayers.Length; j++)
            {
                if (sortedPlayers[j].score > sortedPlayers[max].score)
                {
                    max = j;
                }
            }
            if (max != i)
            {
                temp = sortedPlayers[i];
                sortedPlayers[i] = sortedPlayers[max];
                sortedPlayers[max] = temp;
            }
        }
        return sortedPlayers;
    }
}
