using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Blink : MonoBehaviour
{
    public float blinkGap, eyesShutTime;
    public bool eyesShut;
    public GameObject blinkImage;
    public TextMeshProUGUI counterText;
    private float counter;
    void Start()
    {
        counter = blinkGap;
        blinkImage.SetActive(false);
        StartCoroutine(BlinkGapTimer());
    }
    void Update() {
        counter = counter - Time.deltaTime;
        counterText.text = counter.ToString("F2");
    }
    IEnumerator BlinkGapTimer() {
        yield return new WaitForSeconds(blinkGap);
        blinkImage.SetActive(true);
        eyesShut = true;
        StartCoroutine(EyesShutTimer());
    }
    IEnumerator EyesShutTimer() {
        yield return new WaitForSeconds(eyesShutTime);
        blinkImage.SetActive(false);
        eyesShut = false;
        counter = blinkGap;
        StartCoroutine(BlinkGapTimer());
    }
}
