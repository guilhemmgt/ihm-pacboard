using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ChargingBar : MonoBehaviour
{
    [SerializeField] private Image barImage;

    public void DoChargeBar(float time)
    {
        barImage.fillAmount = 0;
        barImage.DOFillAmount(1, time).SetEase(Ease.Linear);
    }
}
