using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using PopupWindow = UnityEngine.UIElements.PopupWindow;

namespace xyz.bryanalvarado.unityPomodoro
{
public class UnityPomodoro : EditorWindow
{
    private enum TimerState
    {
        WORK,
        BREAK
    }
    
    [MenuItem("Window/UIElements/Unity-Pomodoro")]
    public static void ShowExample()
    {
        UnityPomodoro wnd = GetWindow<UnityPomodoro>();
        wnd.titleContent = new GUIContent("Unity-Pomodoro");
    }

    private DateTime startTime;
    private bool isOn;
    private FloatField workTime;
    private FloatField breakTime;
    private Button toggleStateButton;
    private TimerState timerState;
    private Label currentTime;

    public void OnEnable()
    {
        VisualElement root = rootVisualElement;
        
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/Unity-Pomodoro/UnityPomodoro.uss");
        VisualElement labelWithStyle = new Label("Unity Timer");
        labelWithStyle.styleSheets.Add(styleSheet);
        root.Add(labelWithStyle);

        workTime = new FloatField("Work Interval (in minutes):");
        workTime.value = 1f;
        root.Add(workTime);
        
        breakTime = new FloatField("Break Interval (in minutes):");
        breakTime.value = 1f;
        root.Add(breakTime);

        toggleStateButton = new Button(ToggleTimerState) {text = "Start"};
        root.Add(toggleStateButton);
        
        currentTime = new Label();
        currentTime.styleSheets.Add(styleSheet);
        root.Add(currentTime);

        isOn = false;
    }

    protected void Reset()
    {
        isOn = false;
        startTime = DateTime.Now;
    }

    private void ToggleTimerState()
    {
        if (!isOn && workTime.value >= 1f && breakTime.value >= 1f)
        {
            isOn = true;
            startTime = DateTime.Now;
            toggleStateButton.text = "Stop";
            timerState = TimerState.WORK;

        }
        else
        {
            isOn = false;
            currentTime.text = string.Empty;
            toggleStateButton.text = "Start";
        }
    }

    protected void Update()
    {
        if (isOn && workTime.value >= 1f && breakTime.value >= 1f)
        {
            var timePassed = DateTime.Now - startTime;
            switch (timerState)
            {
                case TimerState.WORK:
                    currentTime.text = $"{timePassed.Minutes}:{timePassed.Seconds}:{timePassed.Milliseconds}";
                    if (timePassed.Minutes >= workTime.value)
                    {
                        timerState = TimerState.BREAK;
                        startTime = DateTime.Now;
                        EditorUtility.DisplayDialog("Time to take a break!",
                            "Go and stretch your legs, have a glass or water, relax!", "OK");
                    }
                    break;
                
                case TimerState.BREAK: 
                    currentTime.text = $"{timePassed.Minutes}:{timePassed.Seconds}:{timePassed.Milliseconds}";                    
                    if (timePassed.Minutes >= breakTime.value)
                    {
                        timerState = TimerState.WORK;
                        startTime = DateTime.Now;
                        EditorUtility.DisplayDialog("Time to get back to work!",
                            "Remember to stay focus! You'll do a great work!", "OK");
                    }
                    break;
            }
        }
    }
}    

}

