using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;



public class GameManager : MonoBehaviour
{
    Question[] _questions = null;
    public Question[] Questions { get { return _questions; } }

    
   public AnswerData answerData;

    [SerializeField] GameEvents events = null;

    [SerializeField] Animator timerAnimator = null;
    [SerializeField] TextMeshProUGUI timerText = null;
    [SerializeField] Color timerHalfWayOutColor = Color.yellow;
    [SerializeField] Color timerAlmostOutColor = Color.red;
    private Color timerDefaultColor = Color.white;

    //Animator for QuestionImage
    [SerializeField] Animator questionImageAnimator = null;


    //QuestionImage 
    private int questionImageStateParameterHash = 0;




    private List<AnswerData> PickedAnswers = new List<AnswerData>();
    private List<int> FinishedQuestions = new List<int>();
    private int currentQuestion = 0;

    private int timerStateParaHash = 0;


    private IEnumerator IE_WaitTillNextRound = null;
    private IEnumerator IE_StartTImer = null;


    private bool IsFinished
    {
        get
        {
            return (FinishedQuestions.Count < Questions.Length) ? false : true;
        }
    }

    void OnEnable()
    {
        events.UpdateQuestionAnswer += UpdateAnswers;
    }

    void OnDisable()
    {
        events.UpdateQuestionAnswer -= UpdateAnswers;
    }

     void Awake()
    {
        events.CurrentFinalScore = 0;
    }

    void Start ()
    {
      //  answerData.answerImageAnimator = gameObject.GetComponent<Animator>();

        events.StartupHighScore = PlayerPrefs.GetInt(GameUtility.SavePrefKey);

        timerDefaultColor = timerText.color;
        LoadQUestions();
         
        timerStateParaHash = Animator.StringToHash("TimerState");

        //QuestionImageState
        questionImageStateParameterHash = Animator.StringToHash("QuestionImageState");


        //useless?
       // answerData.answerImageStateParameterHash = Animator.StringToHash("AnswerImageState");

        var seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        UnityEngine.Random.InitState(seed);

        Display();
    }

    public void UpdateAnswers(AnswerData newAnswer)
    {
        


        if(Questions[currentQuestion].GetAnswerType == Question.AnswerType.Single)
        {
            foreach(var answer in PickedAnswers)
            {
                if(answer != newAnswer)
                {
                    answer.Reset();
                }
                PickedAnswers.Clear();
                PickedAnswers.Add(newAnswer);
            }
        }
        else
        {
            bool alreadyPicked = PickedAnswers.Exists(x => x == newAnswer);
            if (alreadyPicked)
            {
                PickedAnswers.Remove(newAnswer);
            }
            else
            {
                PickedAnswers.Add(newAnswer);
            }
        }
    }

    public void EaraseAnswers ()
    {
        PickedAnswers = new List<AnswerData>();
    }
    

    public void Display ()
    {
        EaraseAnswers();
        var question = GetRandomQuestion();
        

        if(events.UpdateQuestionUI != null)
        {
            events.UpdateQuestionUI(question);
        }
        else
        {
            Debug.LogWarning("Ups! Something went wrong while trying to display new Question UI Data. GameEvents.UpdateQuestionUI is null. Issue occured in GameManager.Display() method.");
        }
        //Timer Bool
        if (question.UseTimer)
        {
            UpdateTime(question.UseTimer);
        }

        //QuestionImage Bool
       if (question.UseImage)
        {
            UpdateQuestionImage(question.UseImage); 
        }

        //AnswerImage Bool
        if (question.UseAnswerImage)
        {
            //  answerData.UpdateAnswerImage(question.UseAnswerImage);
            UpdateAnswerImage(question.UseAnswerImage);
        }
    }

    public void Accept()
    {
        UpdateTime(false);
        //Image
        UpdateQuestionImage(false);
        //AnswerImage
        //UpdateAnswerImage(false);
        bool isCorrect = CheckAnswers();
        FinishedQuestions.Add(currentQuestion);

        UpdateScore((isCorrect) ? Questions[currentQuestion].AddScore : -Questions[currentQuestion].AddScore);

        if (IsFinished)
        {
            SetHighscore();
        }

        var type = (IsFinished) ? UIManager.ResolutionScreenType.Finish : (isCorrect) ? UIManager.ResolutionScreenType.Correct : UIManager.ResolutionScreenType.Incorrect;
       
        if(events.DisplayResolutionScreen != null)
        {
            events.DisplayResolutionScreen(type, Questions[currentQuestion].AddScore);
        }

        if(IE_WaitTillNextRound != null)
        {
            StopCoroutine(IE_WaitTillNextRound);
        }
        IE_WaitTillNextRound = WaitTillNextRound();
        StartCoroutine(IE_WaitTillNextRound);
    }



    void UpdateTime(bool state)
    {
        switch (state)
        {
            case true:
                IE_StartTImer = StartTimer();
                StartCoroutine(IE_StartTImer);

                timerAnimator.SetInteger(timerStateParaHash, 2);
                break;
            case false:
                if (IE_StartTImer != null)
                {
                    StopCoroutine(IE_StartTImer);
                }

                timerAnimator.SetInteger(timerStateParaHash, 1);
                break;

        }
    }


    //Switchstatement, damit der GM zwischen den einzelnen Fällen unterscheiden kann
    void UpdateQuestionImage(bool state)
    {
        switch (state)
        {
           
            //bedeutet, dass wir das Bild einblenden
            case true:
              
               questionImageAnimator.SetInteger(questionImageStateParameterHash, 1);         
                break;
            //bedeutet, dass wir das Bild ausblenden
            case false:
                questionImageAnimator.SetInteger(questionImageStateParameterHash, 0);
                break;
        }
    }


    public void UpdateAnswerImage(bool state)
    {
        switch (state)
        {
            //means that answer Images are enabled
            case true:
              
                //   Debug.Log(answerData.answerImageAnimator);
                  //answerData.answerImageAnimator.SetInteger(answerImageStateParameterHash, 1);
                  // Debug.Log(answerImageStateParameterHash);
                break;

            case false:
                //answerImageAnimator.SetInteger(answerImageStateParameterHash, 0);
                break;
        }
    }

    IEnumerator StartTimer()
    {
        var totalTime = Questions[currentQuestion].Timer;
        var timeLeft = totalTime;

        timerText.color = timerDefaultColor;

        while (timeLeft > 0)
        {
            timeLeft--;

            if (timeLeft < totalTime / 2 && timeLeft > totalTime / 4)
            {
                timerText.color = timerHalfWayOutColor;
            }
            if (timeLeft < totalTime / 4)
            {
                timerText.color = timerAlmostOutColor;
            }

            timerText.text = timeLeft.ToString();
            yield return new WaitForSeconds(1.0f);
        }
        Accept();
    }

    IEnumerator WaitTillNextRound()
    {
        yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);
        Display();
    }

    Question GetRandomQuestion ()
    {
        var randomIndex = GetRandomQuestionIndex();
        currentQuestion = randomIndex;

        return Questions[currentQuestion];
    }

    int GetRandomQuestionIndex()
    {
        var ramdom = 0;
        if(FinishedQuestions.Count < Questions.Length)
        {
            do
            {
                ramdom = UnityEngine.Random.Range(0, Questions.Length);
            } while (FinishedQuestions.Contains(ramdom) || ramdom == currentQuestion);
        }
        return ramdom;
    }

    bool CheckAnswers()
    {
        if (!CompareAnswers())
        {
            return false;
        }
        return true;
    }

    bool CompareAnswers()
    {
        if(PickedAnswers.Count > 0)
        {
            List<int> c = Questions[currentQuestion].GetCorrectAnswers();
            List<int> p = PickedAnswers.Select(x => x.AnswerIndex).ToList();

            var f = c.Except(p).ToList();
            var s = p.Except(c).ToList();

            return !f.Any() && !s.Any();
        }
        return false;
    }
   void LoadQUestions()
    {
        Object[] objs = Resources.LoadAll("Questions", typeof(Question));
        _questions = new Question[objs.Length];
        for (int i = 0; i < objs.Length; i++ )
        {
            _questions[i] = (Question) objs[i];
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    private void SetHighscore()
    {
        var highscore = PlayerPrefs.GetInt(GameUtility.SavePrefKey);
        if (highscore < events.CurrentFinalScore)
        {
            PlayerPrefs.SetInt(GameUtility.SavePrefKey, events.CurrentFinalScore);
        }
    }

    private void UpdateScore(int add)
    {
        events.CurrentFinalScore += add;

        if(events.ScoreUpdated != null)
        {
            events.ScoreUpdated();
        }
    }
}
