using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    [Header("StartQuiz")]
    [SerializeField] private GameObject m_SphereView;
    [SerializeField] private GameObject m_CanvasStart;
    [SerializeField] private GameObject m_SphereStart;
    [SerializeField] private GameObject m_CanvasQuiz;

    [Header("Buttons")]
    private ButtonGaze buttonGazeCurrent;

    [Header("Others")]
    [SerializeField] private List<Vector3> m_PositionsCanvasQuiz;
    [SerializeField] private List<Vector3> m_RotationsCanvasQuiz;
    [SerializeField] private float m_TimeLimit;

    [Header("Results")]
    [SerializeField] private TextMeshProUGUI m_TimePlaying;
    [SerializeField] private TextMeshProUGUI m_HitsInGameplay;

    [Header("Sounds")]
    [SerializeField] private AudioSource m_AudioSourceStartGame;
    [SerializeField] private AudioSource m_AudioSourceHelpYou;
    [SerializeField] private AudioSource m_AudioSourceHit;
    [SerializeField] private AudioSource m_AudioSourceMissed;
    [SerializeField] private AudioSource m_AudioSourceMenu;
    [SerializeField] private AudioSource m_AudioSourceGameplay;

    private float m_Timer;
    private bool activeTime = false;
    private bool activeTimeButtonStart = false;
    private bool activeTimeButtonBackHome = false;
    private bool activeTimeButtonHelpYou = false;
    private int m_IdButton;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Results")
        {
            print("GameMaster.countHits:: " + GameMaster.countHits);
            print("GameMaster.timer:: " + GameMaster.timer);

            GameMaster.timeGameplay = TimeSpan.FromSeconds(GameMaster.timer);

            m_TimePlaying.text = string.Format("Time playing: {0}:{1}", GameMaster.timeGameplay.Minutes, GameMaster.timeGameplay.Seconds);
            m_HitsInGameplay.text = string.Format("Hits playing: {0}/{1}", GameMaster.countHits, 7);
        }
    }

    void Update()
    {
        if (activeTime)
        {
            m_Timer += Time.deltaTime;

            if (m_Timer >= m_TimeLimit)
            {
                activeTime = false;
                VerifyAnswer(buttonGazeCurrent);
                m_Timer = 0;
            }
        }
    }

    public void GazeEnter(ButtonGaze buttonGaze)
    {
        buttonGazeCurrent = buttonGaze;
        activeTime = true;
        m_IdButton = buttonGaze.id;
    }

    public void GazeExit()
    {
        activeTime = false;
        m_Timer = 0;
    }

    private void VerifyAnswer(ButtonGaze buttonGaze)
    {
        if (buttonGaze.isCorret)
        {
            StartCoroutine(AnswerCorrect_Coroutine(buttonGaze.GetComponent<Image>()));          
        }
        else
        {
            StartCoroutine(AnswerIncorrect_Coroutine(buttonGaze.GetComponent<Image>()));
        }
    }

    private IEnumerator AnswerCorrect_Coroutine(Image imageButton)
    {
        m_AudioSourceHit.Play();
        GameMaster.countHits++;
        imageButton.color = Color.Lerp(Color.white, Color.green, .5f);
        
        yield return new WaitForSeconds(.5f);
        
        for (int i = 0; i < GameMaster.Instance.m_Buttons.Count; i++)
        {
            GameMaster.Instance.m_Buttons[i].gameObject.SetActive(false);
            yield return new WaitForSeconds(.3f);
        }

        GameMaster.Instance.SetNewPlace();

        print("Corret");
    }

    private IEnumerator AnswerIncorrect_Coroutine(Image imageButton)
    {
        m_AudioSourceMissed.Play();
        imageButton.color = Color.Lerp(Color.white, Color.red, .5f);

        yield return new WaitForSeconds(.5f);

        for (int i = 0; i < GameMaster.Instance.m_Buttons.Count; i++)
        {
            GameMaster.Instance.m_Buttons[i].gameObject.SetActive(false);
            yield return new WaitForSeconds(.3f);
        }

        GameMaster.Instance.SetNewPlace();

        print("Incorrect");
    }

    private IEnumerator GazeEnterStartQuiz_Coroutine()
    {
        yield return new WaitForSeconds(m_TimeLimit);

        if (activeTimeButtonStart)
        {
            m_SphereView.SetActive(true);
            GameMaster.Instance.SetNewPlace();
            activeTimeButtonStart = false;
            m_AudioSourceMenu.Stop();
            m_AudioSourceStartGame.Play();

            yield return new WaitForSeconds(.5f);

            m_AudioSourceGameplay.Play();
            GameMaster.Instance.startGameplay = true;
            m_CanvasQuiz.SetActive(true);
            m_CanvasStart.SetActive(false);
            m_SphereStart.SetActive(false);
        }
    }

    public void GazeEnterStartQuiz()
    {
        activeTimeButtonStart = true;
        StartCoroutine(GazeEnterStartQuiz_Coroutine());
    }

    public void GazeExitStartQuiz()
    {
        activeTimeButtonStart = false;
    }

    public void NewPointCanvasQuiz()
    {
        m_PositionsCanvasQuiz.Shuffle();

        Vector3 canvasStartAux = m_CanvasQuiz.transform.position;
        int newPosition = UnityEngine.Random.Range(0, m_PositionsCanvasQuiz.Count);
        int newRotation = UnityEngine.Random.Range(0, m_RotationsCanvasQuiz.Count);

        while (m_CanvasQuiz.transform.position == m_PositionsCanvasQuiz[newPosition])
        {
            newPosition = UnityEngine.Random.Range(0, m_PositionsCanvasQuiz.Count);
        }

        m_CanvasQuiz.transform.position = m_PositionsCanvasQuiz[newPosition];
        m_CanvasQuiz.transform.rotation = Quaternion.Euler(m_RotationsCanvasQuiz[newRotation]);
    }

    private IEnumerator GazeEnterBackQuiz_Coroutine()
    {
        yield return new WaitForSeconds(m_TimeLimit);

        if (activeTimeButtonBackHome)
        {
            SceneManager.LoadScene("Gameplay");
        }
    }

    public void GazeEnterBackQuiz()
    {
        activeTimeButtonBackHome = true;
        StartCoroutine(GazeEnterBackQuiz_Coroutine());
    }

    public void GazeExitBackQuiz()
    {
        activeTimeButtonBackHome = false;
    }

    // Dialogs

    private IEnumerator GazeEnterHelpYou_Coroutine()
    {
        yield return new WaitForSeconds(m_TimeLimit);

        if (activeTimeButtonHelpYou)
        {
            m_AudioSourceHelpYou.Play();
        }
    }

    public void GazeEnterHelpYou()
    {
        activeTimeButtonHelpYou = true;
        StartCoroutine(GazeEnterHelpYou_Coroutine());
    }

    public void GazeExitHelpYou()
    {
        m_AudioSourceHelpYou.Stop();
        activeTimeButtonHelpYou = false;
    }
}
