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
            "请保持头部不动，专心看着萤幕中央的红十字10秒，总共做2次，每次结束会休息。\n\n准备好后，请凝视选项3秒。",
            "测验时间：约 20 秒钟"
        );
        curPage = Page.t1;
        PlayAudio(test1Audio);
    }
    public void Test2Click()
    {
        basicTest.SetActive(false);
        startBtn.SetActive(true);
        ContentControl(
            "请先注视画面中央的原点, 当周围出现另一个圆点时请快速且准确地将视线移动到原点上。\n\n准备好后，请凝视选项3秒。",
            "测验时间：约 30 秒至 5 分钟"
        );
        curPage = Page.t2;
        PlayAudio(test2Audio);
    }
    public void Test3Click()
    {
        basicTest.SetActive(false);
        startBtn.SetActive(true);
        ContentControl(
            "请您将视线跟着画面中的圆点移动！\n\n准备好后，请凝视选项3秒。",
            "测验时间：约 20 秒钟"
        );
        curPage = Page.t3;
        PlayAudio(test3Audio);
    }

    public void BlackTestClick()
    {
        advancedTest.SetActive(false);
        startBtn.SetActive(true);
        ContentControl(
            "请依照数字顺序（1 到 25），将视线依序注视萤幕上随机分布的圆圈。每注视正确的数字，系统会自动连线。\n\n。准备好后，请凝视开始3秒。",
            "测验时间：约 50 秒至 20 分钟"
        );
        curPage = Page.black;
        PlayAudio(blackTestAudio);
    }
    public void ColorTestClick()
    {
        advancedTest.SetActive(false);
        startBtn.SetActive(true);
        ContentControl(
            "请依照圆圈内的数字顺序，从 1 到 25 进行注视，并交替使用红色与黄色数字（红1 > 黄2 > 红3 > 黄4...）。圆圈随机分布于画面中，注视正确会自动连线。\n\n。准备好后，请凝视开始3秒。",
            "测验时间：约 3 至 8 分钟。"
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
            ContentControl("休息一下，准备第二次测试。\n\n准备好后，请凝视开始3秒。", "");
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
                content.text = "测验结束！\n您的测验结果需要再加油，持续练习会让您越来越进步。";
                break;
            case 1:
                content.text = "测验结束！\n您已达到不错的水准！相信您能越来越好！";
                break;
            case 2:
                content.text = "测验结束！\n您的测验结果非常优秀！请继续保持！";
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
            "请问您今天想进行哪一种眼动测试呢？\n（凝视选项3秒）",
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
        contentUnder.text = "测验结果： " + Mathf.Round(testDuration).ToString() + " 秒";
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
            ContentControl("休息一下，准备第二次测试。\n\n准备好后，请凝视选项3秒。", "");
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
        content.text = "请问您今天想进行哪一种眼动测试呢？\n（凝视选项3秒）";
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
                    "请问您今天想进行哪一种眼动测试呢？\n（凝视选项3秒）",
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
                    "请问您今天想进行哪一种眼动测试呢？\n（凝视选项3秒）",
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
                    "请问您今天想进行哪一种眼动测试呢？\n（凝视选项3秒）",
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
                    "请问您今天想进行哪一种眼动测试呢？\n（凝视选项3秒）",
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
                    "请问您今天想进行哪一种眼动测试呢？\n（凝视选项3秒）",
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
