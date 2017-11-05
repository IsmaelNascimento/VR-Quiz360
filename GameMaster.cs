using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    private static GameMaster m_Instance;
    public static GameMaster Instance
    {
        get
        {
            if(m_Instance == null)
            {
                m_Instance = FindObjectOfType(typeof(GameMaster)) as GameMaster;
            }

            return m_Instance;
        }
    }

    [SerializeField] private List<Text> m_TextAnswers;
    public List<ButtonGaze> m_Buttons = new List<ButtonGaze>();
    [SerializeField] private List<string> m_Answers = new List<string>();
    public List<Material> places = new List<Material>();
    [SerializeField] private MeshRenderer m_SphereView;
    [SerializeField] private UIFadeTransition m_FadeScreen;
    private int m_PlaceCurrent = 0;

    [HideInInspector] public static int countHits = 0;
    [HideInInspector] public static TimeSpan timeGameplay;
    public static float timer = 0;
    [HideInInspector] public bool startGameplay;
    [SerializeField] private UiManager m_UiManager;

    private void Start()
    {
        places.Shuffle();

        startGameplay = false;
        countHits = 0;
        timeGameplay = new TimeSpan();
        timer = 0;
    }

    private void Update()
    {
        if (startGameplay)
        {
            timer += Time.deltaTime;
        }
    }

    [ContextMenu("New Place")]
    public void SetNewPlace()
    {
        StartCoroutine(SetNewPlace_Coroutine());
    }

    [ContextMenu("FinishQuiz")]
    private void FinishQuiz()
    {
        startGameplay = false;
        SceneManager.LoadScene("Results");
    }

    public IEnumerator SetNewPlace_Coroutine()
    {
        m_FadeScreen.gameObject.SetActive(true);
        m_FadeScreen.TransitionIn(FindObjectOfType<UIFadeTransition>().transform);

        yield return new WaitForSeconds(.5f);

        if (m_PlaceCurrent < places.Count)
        {
            m_PlaceCurrent++;
            m_FadeScreen.TransitionOut(FindObjectOfType<UIFadeTransition>().transform);
            m_SphereView.material = places[m_PlaceCurrent - 1];
        }
        else if (m_PlaceCurrent == places.Count)
        {
            m_FadeScreen.TransitionOut(FindObjectOfType<UIFadeTransition>().transform);
            FinishQuiz();
        }

        m_FadeScreen.gameObject.SetActive(false);

        m_TextAnswers.Shuffle();
        m_Answers.Shuffle();
        m_Buttons.Shuffle();
        
        for (int i = 0; i < m_TextAnswers.Count - 1; i++)
        {
            m_Buttons[i].isCorret = false;

            if (m_Answers[i] != places[m_PlaceCurrent - 1].name)
            {
                m_TextAnswers[i].text = m_Answers[i];
            }
            else
            {
                m_TextAnswers[i].text = m_Answers[i + 2];
            }
        }

        m_TextAnswers[m_Buttons.Count - 1].text = places[m_PlaceCurrent - 1].name;
        m_Buttons[m_Buttons.Count - 1].isCorret = true;

        m_UiManager.NewPointCanvasQuiz();
        yield return new WaitForSeconds(.5f);

        for (int i = 0; i < m_Buttons.Count; i++)
        {
            m_Buttons[i].GetComponent<Image>().color = Color.white;
            m_Buttons[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(.3f);
        }
    }
}