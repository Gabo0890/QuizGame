using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public struct Answer
{
    [SerializeField] private string _info;
    public string Info { get { return _info; } }

    // Sprite für Antworten kann Separat hinzugefügt werden
    [SerializeField] private Sprite _answerImage;
     public Sprite AnswerImage { get { return _answerImage; } }


    [SerializeField] private bool _isCorrect;
    public bool IsCorrect { get { return _isCorrect; } }

}

[CreateAssetMenu(fileName = "New Question", menuName = "Quiz/new Question")]
public class Question : ScriptableObject
{
    public enum AnswerType { Multi, Single }

    [SerializeField] private string _info = string.Empty;
    public string Info { get { return _info; } }

    //use Answer Image
    [SerializeField] private bool _useAnswerImage;
    public bool UseAnswerImage { get { return _useAnswerImage; } }

    //bool variable, um auszuwählen, ob ein Bild der Frage angehängen soll
    [SerializeField] bool _useImage = false;
    public bool UseImage { get { return _useImage; } }

    //Create Sprite to drag "Image" here
    [SerializeField] Sprite _chosenImage;
    public Sprite ChosenImage { get { return _chosenImage; } }

    [SerializeField] Answer[] _answers = null;
    public Answer[] Answers { get { return _answers; } }


    //notwendig um Bild für AnswerImage innerhalb der AnswerData aufzurufen?

  //  [SerializeField] Answer _answersImage;
  //  public Answer AnswersImage { get { return _answersImage; } }

    





    //Parameters

    [SerializeField] private bool _useTimer = false;
    public bool UseTimer { get { return _useTimer; } }

    [SerializeField] private int _timer = 0;
    public int Timer { get { return _timer; } }

    [SerializeField] private AnswerType _answerType = AnswerType.Multi;
    public AnswerType GetAnswerType { get { return _answerType; } }

    [SerializeField] private int _addScore = 10;
    public int AddScore { get { return _addScore; } }

   

    public List<int> GetCorrectAnswers ()
    {
        List<int> CorrectAnswers = new List<int>();
        for (int i=0; i< Answers.Length; i++)
        {
            if (Answers[i].IsCorrect)
            {
                CorrectAnswers.Add(i);
            }
        }
        return CorrectAnswers;
    }

}
