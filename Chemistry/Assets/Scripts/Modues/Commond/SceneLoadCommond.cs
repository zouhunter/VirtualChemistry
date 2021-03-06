﻿using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
/// <summary>
/// 传入的数据
/// </summary>
public class SceneData : IDisposable
{
    public string SceneName { get; set; }
    public LoadSceneMode Mode { get; set; }

    public SceneData(string sceneName, LoadSceneMode loadMode)
    {
        SceneName = sceneName;
        Mode = loadMode;
    }
    public SceneData(string sceneName)
    {
        SceneName = sceneName;
    }
    public void Dispose()
    {
        //?
    }
}

public class SceneLoadCommond :Command<SceneData>, ITimerBehaviour
{
    //场景名
    public static string ExpSceneName = "Laboratory";
    UnityEngine.AsyncOperation operation;
    private const float operTime = 2f;

    public override string Acceptor
    {
        get
        {
            return "sceneLoad";
        }
    }
    /// <summary>
    /// 下载资源，然后加载场景
    /// </summary>
    /// <param name="notification"></param>
    public override void Execute(SceneData sceneData)
    {
        BundleUISystem.UIGroup.Open<StartLoadingPanel>();

        //加载场景
        operation = SceneManager.LoadSceneAsync(sceneData.SceneName, sceneData.Mode);

        if (sceneData.SceneName == ExpSceneName)
        {
            //开始计时
            operation.allowSceneActivation = false;
            TimerInfo timerInfo = new TimerInfo("SceneLoadCommond", this, 1);
            TimerManager.Instance.AddTimerEvent(timerInfo);
        }
    }

    float progress;


    public void TimerUpdate()
    {
        Facade.SendNotification<float>(AppConfig.EventKey.FillImage, progress);//.Current.InvokeEvents<float>("LoadProgress", progress);
        if (operation.progress < 0.9f)
        {
            //追赶
            if (progress < operation.progress)
            {
                progress += Time.deltaTime;
            }
        }
        else
        {
            if (progress < 1f)
            {
                progress += Time.deltaTime;
            }
            else
            {
                TimerManager.Instance.RemoveTimerEvent("SceneLoadCommond");
                operation.allowSceneActivation = true;
            }
        }
    }
}
