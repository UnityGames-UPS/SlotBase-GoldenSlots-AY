using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

[System.Serializable]
public class BonusController : MonoBehaviour
{
    [Header("Tap Bonus Buttons")]
    [SerializeField]
    private List<BonusChest> Chest_References;
    [SerializeField]
    private TMP_Text[] Bonus_Text;
    [SerializeField]
    internal TMP_Text Total_Bonus;
    [SerializeField]
    private GameObject Bonus_Object;
    [SerializeField]
    private SlotBehaviour slotManager;
    [SerializeField]
    private AudioController _audioManager;
    //[SerializeField]
    //private GameObject PopupPanel;
    [SerializeField]
    private Transform Win_Transform;
    [SerializeField]
    private Transform Loose_Transform;
    private bool gameOver = false;

    // [Header("For Testing Purpose Only...")]

    private int[] m_BonusChestIndices; //Testing Bonus Data To Entered In The Unity Editor
   // private int m_Chest_Index_Count = 0;
    private double m_total_bonus = 0;
   // private bool IsOpening;
    private double multiplier;
    [SerializeField] private SlotBehaviour slotBehaviour;
    private void Start()
    {
        Chest_References[0].m_Chest_Button.onClick.RemoveAllListeners();
        Chest_References[0].m_Chest_Button.onClick.AddListener(delegate { if (!gameOver) { OnClickOpenBonus(0); slotBehaviour.OnBonusChestClick(0); } });

        Chest_References[1].m_Chest_Button.onClick.RemoveAllListeners();
        Chest_References[1].m_Chest_Button.onClick.AddListener(delegate { if (!gameOver) { OnClickOpenBonus(1); slotBehaviour.OnBonusChestClick(1); } });

        Chest_References[2].m_Chest_Button.onClick.RemoveAllListeners();
        Chest_References[2].m_Chest_Button.onClick.AddListener(delegate { if (!gameOver) { OnClickOpenBonus(2); slotBehaviour.OnBonusChestClick(2); } });

        Chest_References[3].m_Chest_Button.onClick.RemoveAllListeners();
        Chest_References[3].m_Chest_Button.onClick.AddListener(delegate { if (!gameOver) { OnClickOpenBonus(3); slotBehaviour.OnBonusChestClick(3); } });

        Chest_References[4].m_Chest_Button.onClick.RemoveAllListeners();
        Chest_References[4].m_Chest_Button.onClick.AddListener(delegate { if (!gameOver) { OnClickOpenBonus(4); slotBehaviour.OnBonusChestClick(4); } });
    }

    internal void StartBonus(List<int> bonusResult, double mult)
    {
        gameOver = false;
        if (Win_Transform) Win_Transform.gameObject.SetActive(false);
        if (Loose_Transform) Loose_Transform.gameObject.SetActive(false);
        Total_Bonus.text = "00";
       
        if (_audioManager) _audioManager.playBgAudio("bonus");
        if(_audioManager) _audioManager.StopWLAaudio();
        if (Bonus_Object) Bonus_Object.SetActive(true);
        m_BonusChestIndices = bonusResult.ToArray();
        
        multiplier = mult;

    }


    private void OnClickOpenBonus(int indexOfChest)
    {
        Chest_References[indexOfChest].m_Chest_Button.interactable = false;
     //   if (IsOpening) return;

         Debug.Log(string.Concat("<color=red>", "Click On Chest Detected... ", indexOfChest, "</color>"));
        StartCoroutine(DisablePassedIndexChest(indexOfChest));
    }

    private IEnumerator DisablePassedIndexChest(int indexOfChest)
    {
       
       // IsOpening = true;
        double bonusAmount = multiplier * m_BonusChestIndices[indexOfChest];
       
        Chest_References[indexOfChest].m_Chest_Button.GetComponent<ImageAnimation>().StartAnimation();
       
        Chest_References[indexOfChest].m_Chest_Button.interactable = false;
        DoAnimationOnChestClick(indexOfChest, bonusAmount);
       

        m_total_bonus += bonusAmount;
        if (m_BonusChestIndices[indexOfChest] == 0)
        {
            gameOver = true;
        }

        yield return new WaitForSeconds(0.5f);
        Chest_References[indexOfChest].m_Chest_Button.GetComponent<ImageAnimation>().StopAnimation();
       

        yield return new WaitForSeconds(0.5f);
        if (m_BonusChestIndices[indexOfChest] == 0)
        {
           
            yield return new WaitForSeconds(1.5f);
            ResetChestBonusButtons();
        }
      //  IsOpening = false;


    }

    private void ResetChestBonusButtons()
    {
        Bonus_Object.SetActive(false);
        if(_audioManager) _audioManager.playBgAudio();


        m_total_bonus = 0;
        multiplier = 0;
        Total_Bonus.text = string.Concat("Bonus Score", "\n\n", m_total_bonus.ToString());
        foreach (var item in Chest_References)
        {
            item.m_Chest_Button.GetComponent<ImageAnimation>().StopAnimation();
            item.m_Chest_Button.interactable = true;
        }
        ResetToDefaultAnimationAfterChestClick();

    }

   
    #region DOTween Animations and Reset Animations
    private void DoAnimationOnChestClick(int m_index, double m_score)
    {
        if (_audioManager) _audioManager.StopWLAaudio();

        BonusChest m_Temp_Chest = Chest_References[m_index];
        Debug.Log(m_score+" .>>>>>>>>>>>>> score     <<<<<<<."+m_index);

        if (m_score > 0)
        {
            m_Temp_Chest.m_Score.text = "+" + m_score.ToString();
            if (_audioManager) _audioManager.PlayWLAudio("bonuswin");

        }
        else
        {

            m_Temp_Chest.m_Score.text = "Game Over";
            if (_audioManager) _audioManager.PlayWLAudio("bonuslose");

        }

        m_Temp_Chest.m_ScoreHolder.SetActive(true);
        DOTweenScale(m_Temp_Chest.m_ScoreHolder.transform, m_Temp_Chest.m_ScoreHolder.transform, 1f);
    }

    private void DOTweenScale(Transform m_rect_transform, Transform m_obj_transform, float m_time)
    {
        m_rect_transform.DOScale
            (
                m_obj_transform.localScale + (Vector3.one * 1.2f),
                m_time
            );
        m_rect_transform.DOLocalMoveY
            (
                m_obj_transform.position.y + 220,
                m_time
            ).OnComplete(() =>
            {

                m_obj_transform.gameObject.SetActive(false);
            });
        //m_obj_transform.localScale = Vector3.one * 1.5f;

        //m_obj_transform.position = m_obj_transform.position + (Vector3.up * 2);
    }

    //This method is used to reset the bonus chest to default and ready for next bonus
    private void ResetToDefaultAnimationAfterChestClick()
    {
        foreach (var i in Chest_References)
        {
            i.m_ScoreHolder.transform.localScale = Vector3.zero;
            i.m_ScoreHolder.transform.localPosition = new Vector3(0.1f, 0.1f, 0.1f);
            i.m_ScoreHolder.SetActive(false);
        }
        slotBehaviour.CheckPopups = false;
    }
    #endregion

    #region Structures Used
    [System.Serializable]
    public struct BonusChest
    {
        public TMP_Text m_Score;
        public Button m_Chest_Button;
        public GameObject m_ScoreHolder;
    }
    #endregion
}
