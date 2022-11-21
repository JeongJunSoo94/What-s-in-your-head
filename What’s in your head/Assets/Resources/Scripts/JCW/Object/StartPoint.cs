using JCW.AudioCtrl;
using JCW.Dialog;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class StartPoint : MonoBehaviour
{
    StringBuilder currentStageSection;
    StringBuilder dialogString;
    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            Debug.Log("���� �Ŵ����� �����Ƿ� �ߴ�");
            return;
        }
        // ���� ���������� ���� ��������
        currentStageSection = new(10, 10);
        currentStageSection.Append("S");
        currentStageSection.Append(GameManager.Instance.curStageIndex);
        currentStageSection.Append("S");
        currentStageSection.Append(GameManager.Instance.curStageType);
        Debug.Log("���� ���������� ���� : " + currentStageSection.ToString());

        dialogString = new(10, 10);
        dialogString.Append(currentStageSection);
        dialogString.Append("_N");
        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.SetDialogs(dialogString.ToString());
            dialogString.Replace("_N", "_E", 4, 2);
            DialogManager.Instance.SetDialogs(dialogString.ToString());
            dialogString.Replace("_E", "_S", 4, 2);
            DialogManager.Instance.SetDialogs(dialogString.ToString());
        }       

        StartCoroutine(nameof(WaitForPlayer));       
    }

    IEnumerator WaitForPlayer()
    {
        yield return new WaitUntil(() => PhotonNetwork.PlayerList.Length == 2);
        if(PhotonNetwork.IsMasterClient)
            SoundManager.Instance.PlayBGM_RPC(currentStageSection.ToString());
        yield break;
    }
}
