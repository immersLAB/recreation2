/*===============================================================================
Copyright (c) 2021 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
===============================================================================*/

using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class VuforiaStateInfo : MonoBehaviour
{
    public GameObject TextObject;

    const string ACTIVE_TARGETS_TITLE = "<b>Active Targets: </b>";

    string mTargetStatusInfo;
    string mVuMarkTrackableStateInfo;

    readonly Dictionary<string, string> mTargetsStatus = new Dictionary<string, string>();

    void Start()
    {
        VuforiaApplication.Instance.OnVuforiaStarted += OnVuforiaStarted;
    }

    void OnDestroy()
    {
        VuforiaApplication.Instance.OnVuforiaStarted -= OnVuforiaStarted;
    }

    void OnVuforiaStarted()
    {
        UpdateText();
    }

    /// <summary>
    /// Public method to be called by an EventHandler's Lost/Found Events
    /// </summary>
    /// <param name="observerBehaviour"></param>
    public void TargetStatusChanged(ObserverBehaviour observerBehaviour)
    {
        var status = GetStatusString(observerBehaviour.TargetStatus);

        var targetName = observerBehaviour.TargetName;
        if (mTargetsStatus.ContainsKey(targetName))
            mTargetsStatus[targetName] = status;
        else
            mTargetsStatus.Add(targetName, status);

        UpdateText();
    }

    void UpdateText()
    {
        UpdateInfo();

        var completeInfo = ACTIVE_TARGETS_TITLE;

        if (mTargetStatusInfo.Length > 0)
            completeInfo += $"\n{mTargetStatusInfo}";

        SampleUtil.AssignStringToTextComponent(TextObject ? TextObject : gameObject, completeInfo);
    }

    void UpdateInfo()
    {
        mTargetStatusInfo = GetTargetsStatusInfo();
    }

    string GetStatusString(TargetStatus targetStatus)
    {
        return $"{targetStatus.Status} -- {targetStatus.StatusInfo}";
    }
    
    string GetTargetsStatusInfo()
    {
        var targetsAsMultiLineString = "";
        
        foreach (var targetStatus in mTargetsStatus)
            targetsAsMultiLineString += "\n" + targetStatus.Key + ": " + targetStatus.Value;

        return targetsAsMultiLineString;
    }
}
