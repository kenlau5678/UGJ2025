using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;  // �ǵ����� DOTween �����ռ�

public class Window : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    private Tween currentTween;

    private Vector3 originalScale;

    [Header("��������")]
    public float hoverScale = 1.1f;     // ����ʱ�Ŵ����
    public float clickScale = 0.9f;     // ���˲����С����
    public float animDuration = 0.2f;   // ����ʱ��
    public Ease animEase = Ease.OutBack; // �������ߣ����Ի��� Ease.OutElastic��
    private void Awake()
    {

    }
    void Start()
    {
        originalScale = transform.localScale;
    }
    private void Update()//ÿһ֡  
    {
        
    }

    private void FixedUpdate()//ÿһ�̶�Ƶ��֡
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayScaleAnim(originalScale * hoverScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PlayScaleAnim(originalScale);
    }

    public void Click()
    {
        // ���������������С��ָ�������״̬
        if (currentTween != null) currentTween.Kill();
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(originalScale * clickScale, 0.1f).SetEase(Ease.OutQuad));
        seq.Append(transform.DOScale(originalScale * hoverScale, 0.2f).SetEase(Ease.OutBack));
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        // ���������������С��ָ�������״̬
        if (currentTween != null) currentTween.Kill();
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(originalScale * clickScale, 0.1f).SetEase(Ease.OutQuad));
        seq.Append(transform.DOScale(originalScale * hoverScale, 0.2f).SetEase(Ease.OutBack));
    }

    private void PlayScaleAnim(Vector3 target)
    {
        if (currentTween != null) currentTween.Kill();
        currentTween = transform.DOScale(target, animDuration).SetEase(animEase);
    }
}
