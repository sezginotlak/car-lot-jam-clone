using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Object : MonoBehaviour
{
    public List<GridCell> pathToDestination = new List<GridCell>();
    public GridCell currentOccupiedCell;
    public Outline outline;
    public GameObject emoji;
    public Sprite angryEmoji, happyEmoji;
    public LayerMask layerMaskForStart;
    public int id;
    internal Vector3 emojiLocalScale;

    internal GameController gameController;
    internal FindShortestPath findShortestPath;
    internal GenerateGrid generateGrid;

    bool isOnEnable;

    public virtual void Start()
    {
        Invoke(nameof(AssignScripts), 0.2f);
        emojiLocalScale = emoji.transform.localScale;
    }

    private void AssignScripts()
    {
        gameController = GameController.Instance;
        findShortestPath = gameController.FindShortestPath;
        generateGrid = findShortestPath.generateGrid;
    }

    // Oyun baþlangýcýnda hangi hücrelerin dolu olduðunu belirtmek için kullanýlýyor
    private IEnumerator MarkOccupiedCell(Collider other)
    {
        yield return new WaitForSeconds(0.2f);
        currentOccupiedCell = other.transform.GetComponent<GridCell>();
        currentOccupiedCell.IsBlocked = true;
        isOnEnable = true;
    }

    public void OpenHappyEmoji()
    {
        StartCoroutine(IEOpenHappyEmoji());
    }

    IEnumerator IEOpenHappyEmoji()
    {
        emoji.SetActive(true);
        emoji.GetComponent<SpriteRenderer>().sprite = happyEmoji;
        emoji.transform.DOPunchScale(emoji.transform.localScale * 1.05f, 0.5f, 0).SetLoops(-1).OnUpdate(() => emoji.transform.LookAt(Camera.main.transform));
        yield return new WaitForSeconds(1f);
        emoji.transform.DOKill();
        emoji.transform.localScale = emojiLocalScale;
        emoji.gameObject.SetActive(false);
    }

    public void OpenAngryEmoji()
    {
        StartCoroutine(IEOpenAngryEmoji());
    }

    IEnumerator IEOpenAngryEmoji()
    {
        emoji.SetActive(true);
        emoji.GetComponent<SpriteRenderer>().sprite = angryEmoji;
        emoji.transform.DOPunchScale(emoji.transform.localScale * 1.05f, 0.5f, 0).SetLoops(-1).OnUpdate(() => emoji.transform.LookAt(Camera.main.transform));
        yield return new WaitForSeconds(1f);
        emoji.transform.DOKill();
        emoji.transform.localScale = emojiLocalScale;
        emoji.gameObject.SetActive(false);
    }

    public virtual void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.layer == 3)
        {
            if (isOnEnable) return;

            StartCoroutine(MarkOccupiedCell(other));
        }

    }
}
