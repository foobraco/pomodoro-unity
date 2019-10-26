using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using PopupWindow = UnityEngine.UIElements.PopupWindow;


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
    private PopupWindow popupWindow;
    private Label currentTime;

    public void OnEnable()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UnityPomodoro.uxml");
        VisualElement labelFromUXML = visualTree.CloneTree();
        root.Add(labelFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/UnityPomodoro.uss");
        VisualElement labelWithStyle = new Label("Unity Timer");
        labelWithStyle.styleSheets.Add(styleSheet);
        root.Add(labelWithStyle);

        workTime = new FloatField("Work Interval (in minutes):");
        root.Add(workTime);
        
        breakTime = new FloatField("Break Interval (in minutes):");
        root.Add(breakTime);

        toggleStateButton = new Button(ToggleTimerState) {text = "Start"};
        root.Add(toggleStateButton);
        
        currentTime = new Label();
        root.Add(currentTime);
        
        popupWindow = new PopupWindow();
        popupWindow.SetEnabled(false);
        
        root.Add(popupWindow);

        isOn = false;
    }

    private void ToggleTimerState()
    {
        if (!isOn)
        {
            isOn = true;
            startTime = DateTime.Now;
            toggleStateButton.text = "Stop";
            timerState = TimerState.WORK;

        }
        else
        {
            isOn = false;
            toggleStateButton.text = "Start";
        }
    }

    protected void Update()
    {
        if (isOn)
        {
            var timePassed = DateTime.Now - startTime;
            switch (timerState)
            {
                case TimerState.WORK:
                    currentTime.text = $"{timePassed.Minutes}:{timePassed.Seconds}:{timePassed.Milliseconds}";
                    if (timePassed.Minutes >= workTime.value)
                    {
                        popupWindow.text = "Time to take a break!";
                        timerState = TimerState.BREAK;
                        popupWindow.SetEnabled(true);
                    }
                    break;
                
                case TimerState.BREAK: 
                    currentTime.text = $"{timePassed.Minutes}:{timePassed.Seconds}:{timePassed.Milliseconds}";                    
                    if (timePassed.Minutes >= breakTime.value)
                    {
                        popupWindow.text = "Time to get back to work!";
                        timerState = TimerState.WORK;
                        popupWindow.SetEnabled(true);
                    }
                    break;
            }
        }
    }
}