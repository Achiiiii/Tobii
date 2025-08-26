using System;
using System.Collections;
using System.Collections.Generic;
using Tobii.Research.Unity;
using UnityEngine;
using UnityEngine.UI;

public class TobiiController : MonoBehaviour
{
    public RectTransform eyeTrackRect;
    public TMPro.TMP_Text content;
    public TMPro.TMP_Text contentUnder;
    public TrackBoxGuide trackBoxGuide;
    public GameObject testChoice;
    public GameObject basicTest;
    public GameObject advancedTest;
    public GameObject test1;
    public GameObject test2;
    public GameObject test3;
    public GameObject blackTestBtn;
    public GameObject colorTestBtn;
    public GameObject startBtn;
    public GameObject blackTest;
    public GameObject colorTest;
    public GameObject backBtn;
    public Test2 test2Script;
    public Test3 test3Script;
    public GameObject pointer;
    public GameObject[] stars;
    public Sprite starFull;
    public Sprite starEmpty;
    public GazePointVisualizer gazePointVisualizer;
    public AudioSource audioSource;
    public AudioClip questionAudio;
    public AudioClip test1Audio;
    public AudioClip test2Audio;
    public AudioClip test3Audio;
    public AudioClip againAudio;
    public AudioClip blackTestAudio;
    public AudioClip colorTestAudio;
    public AudioClip[] resultAudios;
    public PointController blackPointController;
    public PointController colorPointController;

    [SerializeField]
    [Tooltip("This key will show or hide the track box guide.")]
    private KeyCode _toggleKey = KeyCode.None;

    private float testDuration;
    private float startTime;
    private float endTime;
    private bool isScale = true;
    private string test1String = "";
    private Page curPage = Page.home;

    private enum Page
    {
        home,
        basic,
        advanced,
        t1,
        t2,
        t3,
        black,
        color
    }

    void Start()
    {
        gazePointVisualizer.SetRatio(eyeTrackRect.localScale.x);
    }

    void Update()
    {
        if (Input.GetKeyDown(_toggleKey))
        {
            if (isScale)
            {
                eyeTrackRect.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                eyeTrackRect.localScale = new Vector3(0.42f, 0.42f, 1);
            }
            gazePointVisualizer.SetRatio(eyeTrackRect.localScale.x);
            isScale = !isScale;
        }
    }
    public void BasicTestClick()
    {
        testChoice.SetActive(false);
        basicTest.SetActive(true);
        PlayAudio(questionAudio);
        curPage = Page.basic;
    }
    public void AdvancedTestClick()
    {
        testChoice.SetActive(false);
        advancedTest.SetActive(true);
        PlayAudio(questionAudio);
        curPage = Page.advanced;
    }

    public void Test1Click()
    {
        basicTest.SetActive(false);
        startBtn.SetActive(true);
        ContentControl(
            "Please keep your head still and focus on the red cross in the center of the screen for 10 seconds. This will be done twice, with a break between each.\nWhen you're ready, stare to begin in 3 seconds.",
            "Test time: ~20 sec"
        );
        curPage = Page.t1;
        PlayAudio(test1Audio);
    }
    public void Test2Click()
    {
        basicTest.SetActive(false);
        startBtn.SetActive(true);
        ContentControl(
            "Please first look at the center dot. When another dot appears around it, quickly and accurately shift your gaze to that dot. \nWhen you're ready, stare to begin in 3 seconds.",
            "Test time: 30 sec ~ 5 min"
        );
        curPage = Page.t2;
        PlayAudio(test2Audio);
    }
    public void Test3Click()
    {
        basicTest.SetActive(false);
        startBtn.SetActive(true);
        ContentControl(
            "Please follow the moving dot on the screen with your eyes! \nWhen you're ready, stare to begin in 3 seconds.",
            "Test time: ~20 sec"
        );
        curPage = Page.t3;
        PlayAudio(test3Audio);
    }

    public void BlackTestClick()
    {
        advancedTest.SetActive(false);
        startBtn.SetActive(true);
        ContentControl(
            "Please look at the numbered circles in order from 1 to 25 on the screen. The system will connect them automatically when you look at the correct number. \nWhen you're ready, stare to begin in 3 seconds.",
            "Test time: 50 sec ~ 20 min"
        );
        curPage = Page.black;
        PlayAudio(blackTestAudio);
    }
    public void ColorTestClick()
    {
        advancedTest.SetActive(false);
        startBtn.SetActive(true);
        ContentControl(
            "Please follow the numbers in order from 1 to 25, alternating between red and yellow numbers (Red 1 → Yellow 2 → Red 3 → Yellow 4…). The circles are randomly placed, and the system will auto-connect when a number is correctly fixated. \nWhen you're ready, stare to begin in 3 seconds.",
            "Test time: 3 min ~ 8 min"
        );
        curPage = Page.color;
        PlayAudio(colorTestAudio);
    }

    public void StartClick()
    {
        gazePointVisualizer.ToggleGazeDot(false);
        startBtn.SetActive(false);
        TestPageControl(true);
        startTime = Time.time;
        audioSource.Stop();
    }

    private void TestPageControl(bool value)
    {
        switch (curPage)
        {
            case Page.t1:
                test1.SetActive(value);
                if (value)
                {
                    gazePointVisualizer.StartRecord();
                    gazePointVisualizer.ToggleLineActive(true);
                    StartCoroutine(Test1Start("first"));
                }
                break;
            case Page.t2:
                test2.SetActive(value);
                if (value)
                {
                    gazePointVisualizer.StartRecord();
                    gazePointVisualizer.ToggleLineActive(true);
                    test2Script.Test2Start();
                }
                break;
            case Page.t3:
                test3.SetActive(value);
                if (value)
                {
                    gazePointVisualizer.StartRecord();
                    gazePointVisualizer.ToggleLineActive(true);
                    test3Script.Test3Start();
                }
                break;
            case Page.black:
                blackTest.SetActive(value);
                break;
            case Page.color:
                colorTest.SetActive(value);
                break;
            default:
                break;
        }
    }
    private IEnumerator Test1Start(string times)
    {
        yield return new WaitForSeconds(10f);
        if (test1String == "first")
        {
            test1String = "";
            gazePointVisualizer.StopRecord("stability_2_", true);
            contentUnder.text = "";
            ShowResult(2);
        }
        else if (times == "first")
        {
            test1String = "first";
            gazePointVisualizer.StopRecord("stability_1_", true);
            gazePointVisualizer.ToggleGazeDot(true);
            ContentControl("Take a short break before the second round. \nWhen you're ready, stare to begin in 3 seconds.", "");
            PlayAudio(againAudio);
        }
        test1.SetActive(false);
        startBtn.SetActive(true);
        gazePointVisualizer.ToggleLineActive(false);

    }
    private void ContentControl(string _content, string _contentUnder)
    {
        content.text = _content;
        contentUnder.text = _contentUnder;
    }
    private void ShowResult(int score)
    {
        startBtn.SetActive(false);
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(true);
            if (i <= score) stars[i].GetComponent<Image>().sprite = starFull;
            else stars[i].GetComponent<Image>().sprite = starEmpty;
        }
        switch (score)
        {
            case 0:
                content.text = "Test complete! \nYour results need some improvement, but continued practice will help you progress steadily.";
                break;
            case 1:
                content.text = "Test complete! \nYou've reached a good level. We believe you’ll keep improving!";
                break;
            case 2:
                content.text = "Test complete! \nYour results are excellent. Keep up the great work!";
                break;
            default:
                break;
        }
        PlayAudio(resultAudios[score]);
        StartCoroutine(DelayHideResult());
    }

    private IEnumerator DelayHideResult()
    {
        yield return new WaitForSeconds(10f);
        gazePointVisualizer.ToggleGazeDot(true);
        foreach (GameObject star in stars)
        {
            star.SetActive(false);
        }
        startBtn.SetActive(false);
        switch (curPage)
        {
            case Page.t1:
            case Page.t2:
            case Page.t3:
                curPage = Page.basic;
                basicTest.SetActive(true);
                break;
            case Page.black:
            case Page.color:
                curPage = Page.advanced;
                advancedTest.SetActive(true);
                break;

        }
        ContentControl(
            "Which eye-tracking test would you like to perform today? \nPlease look at the option for 3 seconds to select.",
            ""
        );
        PlayAudio(questionAudio);
    }

    private void PlayAudio(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void EndTest()
    {
        endTime = Time.time;
        testDuration = endTime - startTime;
        contentUnder.text = "Result: " + Mathf.Round(testDuration).ToString() + " seconds";
        int score;
        if (curPage == Page.black)
        {
            if (testDuration <= 60) score = 2;
            else if (testDuration <= 73) score = 1;
            else score = 0;

        }
        else if (curPage == Page.color)
        {
            if (testDuration <= 180) score = 2;
            else if (testDuration <= 236) score = 1;
            else score = 0;
        }
        else if (curPage == Page.t2)
        {
            if (testDuration <= 30) score = 2;
            else if (testDuration <= 60) score = 1;
            else score = 0;
        }
        else
        {
            score = 2;
        }

        ShowResult(score);
        TestPageControl(false);
    }
    public void Test2End()
    {
        gazePointVisualizer.StopRecord("pavisic_", true);
        gazePointVisualizer.ToggleLineActive(false);
        contentUnder.text = "";
        EndTest();
    }
    public void Test3End(string times)
    {
        if (times == "first")
        {
            gazePointVisualizer.StopRecord("track_1_", true);
            gazePointVisualizer.ToggleGazeDot(true);
            ContentControl("Take a short break before the second round. \nWhen you're ready, stare to begin in 3 seconds.", "");
            PlayAudio(againAudio);
        }
        else
        {
            gazePointVisualizer.StopRecord("track_2_", true);
            contentUnder.text = "";
            ShowResult(2);
        }
        test3.SetActive(false);
        startBtn.SetActive(true);
        gazePointVisualizer.ToggleLineActive(false);

    }
    public void Reset()
    {
        StopAllCoroutines();
        gazePointVisualizer.ToggleGazeDot(true);
        gazePointVisualizer.ToggleLineActive(false);
        gazePointVisualizer.StopRecord("", false);
        pointer.SetActive(false);
        if (curPage == Page.black)
            blackPointController.Reset();
        if (curPage == Page.color)
            colorPointController.Reset();
        foreach (GameObject star in stars)
        {
            star.SetActive(false);
        }

        trackBoxGuide.gameObject.SetActive(true);
        trackBoxGuide.TrackBoxGuideActive = true;
        testChoice.SetActive(true);
        basicTest.SetActive(false);
        advancedTest.SetActive(false);
        test1.SetActive(false);
        test2.SetActive(false);
        test3.SetActive(false);
        blackTest.SetActive(false);
        colorTest.SetActive(false);
        startBtn.SetActive(false);
        content.text = "Which eye-tracking test would you like to perform today? \nPlease look at the option for 3 seconds to select.";
        contentUnder.text = "";
        trackBoxGuide.Reset();
        audioSource.Stop();
        test1String = "";
        test2Script.Reset();
        test3Script.Reset();
        curPage = Page.home;
    }
    public void BackBtn()
    {
        audioSource.Stop();
        switch (curPage)
        {
            case Page.home:
                gazePointVisualizer.ToggleGazeDot(true);
                pointer.SetActive(false);
                trackBoxGuide.gameObject.SetActive(true);
                trackBoxGuide.TrackBoxGuideActive = true;
                trackBoxGuide.Reset();
                break;
            case Page.basic:
                curPage = Page.home;
                basicTest.SetActive(false);
                testChoice.SetActive(true);
                PlayAudio(questionAudio);
                break;
            case Page.advanced:
                curPage = Page.home;
                advancedTest.SetActive(false);
                testChoice.SetActive(true);
                PlayAudio(questionAudio);
                break;
            case Page.t1:
                gazePointVisualizer.ToggleGazeDot(true);
                startBtn.SetActive(false);
                basicTest.SetActive(true);
                ContentControl(
                    "Which eye-tracking test would you like to perform today? \nPlease look at the option for 3 seconds to select.",
                    ""
                );
                PlayAudio(questionAudio);
                StopAllCoroutines();
                gazePointVisualizer.ToggleLineActive(false);
                gazePointVisualizer.StopRecord("", false);
                test1String = "";
                TestPageControl(false);
                curPage = Page.basic;
                break;
            case Page.t2:
                gazePointVisualizer.ToggleGazeDot(true);
                startBtn.SetActive(false);
                basicTest.SetActive(true);
                ContentControl(
                    "Which eye-tracking test would you like to perform today? \nPlease look at the option for 3 seconds to select.",
                    ""
                );
                PlayAudio(questionAudio);
                StopAllCoroutines();
                gazePointVisualizer.ToggleLineActive(false);
                gazePointVisualizer.StopRecord("", false);
                test2Script.Reset();
                TestPageControl(false);
                curPage = Page.basic;
                break;
            case Page.t3:
                gazePointVisualizer.ToggleGazeDot(true);
                startBtn.SetActive(false);
                basicTest.SetActive(true);
                ContentControl(
                    "Which eye-tracking test would you like to perform today? \nPlease look at the option for 3 seconds to select.",
                    ""
                );
                PlayAudio(questionAudio);
                gazePointVisualizer.ToggleLineActive(false);
                gazePointVisualizer.StopRecord("", false);
                test3Script.Reset();
                TestPageControl(false);
                curPage = Page.basic;
                break;
            case Page.black:
                gazePointVisualizer.ToggleGazeDot(true);
                startBtn.SetActive(false);
                advancedTest.SetActive(true);
                ContentControl(
                    "Which eye-tracking test would you like to perform today? \nPlease look at the option for 3 seconds to select.",
                    ""
                );
                PlayAudio(questionAudio);
                blackPointController.Reset();
                TestPageControl(false);
                curPage = Page.advanced;
                break;
            case Page.color:
                gazePointVisualizer.ToggleGazeDot(true);
                startBtn.SetActive(false);
                advancedTest.SetActive(true);
                ContentControl(
                    "Which eye-tracking test would you like to perform today? \nPlease look at the option for 3 seconds to select.",
                    ""
                );
                PlayAudio(questionAudio);
                colorPointController.Reset();
                TestPageControl(false);
                curPage = Page.advanced;
                break;
        }
    }
}
