using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


[Serializable()]
public class AnswerData : MonoBehaviour
{
  
    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI infoTextObject;

    [SerializeField] Image answerImage;
    public Image AnswerImage { get { return answerImage; } }

    //AnswerImageAnimator
    [SerializeField] public Animator answerImageAnimator = null;


    [SerializeField] Image toggle;

    public int answerImageStateParameterHash = 0; 

   



    [Header("Textures")]
    [SerializeField] Sprite uncheckedToggle;
    [SerializeField] Sprite checkedToggle;

    [Header("References")]
    [SerializeField] GameEvents events;

    private RectTransform _rect;
    public RectTransform Rect
    {
        get
        {
            if(_rect == null)
            {
                _rect = GetComponent<RectTransform>() ?? gameObject.AddComponent<RectTransform>();
            }
            return _rect;
        }
    }

    private int _answerindex = -1;
    public int AnswerIndex { get { return _answerindex; } }

    private bool Checked = false;

    public void GetAnswerImage()
    {
        answerImageAnimator.SetInteger(answerImageStateParameterHash, 1);
    }

    public void UpdateData(string info, int index)
    {
        infoTextObject.text = info;
        _answerindex = index;
        //entweder hier oder unten in UpdateUI()
        //  elementsUI.AnswerImage.sprite = answer.AnswerImage;
        
    }

    public void UpdateImage(Animator animator)
    {
        answerImageAnimator = animator;
        animator.SetInteger(answerImageStateParameterHash, 1);
    }

    public void Reset()
    {
        Checked = false;
        UpdateUI();
     
    }



    public void SwitchState()
    {
        Checked = !Checked;
        UpdateUI();

        if(events.UpdateQuestionAnswer != null)
        {
            
            events.UpdateQuestionAnswer(this);
        }
    }

    void UpdateUI()
    {
        toggle.sprite = (Checked) ? checkedToggle : uncheckedToggle;
       
        //scheint hier endlich richtig zu sein
        //für später, wenn sprite drüber gelegt werden soll
      //  elementsUI.AnswerImage.sprite = answer.AnswerImage;
    }
  
}

  


