using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public partial class UIController : MonoBehaviour
{ 
    private readonly Regex _inputFieldFilter = new Regex(@"^[FBRLUD][2']?(?:(?:(\s[FBRLUD][2']?)+$)|$)");
    [SerializeField] private GameObject shuffleButtonObj;
    [SerializeField] private GameObject launchButtonObj;
    [SerializeField] private GameObject resetButtonObj;
    [SerializeField] private GameObject solveButtonObj;
    [SerializeField] private GameObject stopButtonObj;
    [SerializeField] private GameObject inputFieldObj;
    [SerializeField] private GameObject rubickObj;
    [SerializeField] private Color inputFieldStartColor = Color.white;
    [SerializeField] private Color inputFieldWrongColor = new Color(1.0f, 0.0f, 0.0f, 0.5f);
    [SerializeField] private Color inputFieldCorrectColor = new Color(0.0f, 1.0f, 0.0f, 0.5f);
    private SidesController _sidesController;
    private InputField _inputField;
    private Button _shuffleButton;
    private Button _launchButton;
    private Button _resetButton;
    private Button _solveButton;
    private Button _stopButton;
    private bool _inputIsValid;
    
    private void Start()
    {
        _shuffleButton = shuffleButtonObj
            ? shuffleButtonObj.GetComponent<Button>()
            : GameObject.Find("ShuffleButton").GetComponent<Button>();
        _launchButton = launchButtonObj
            ? launchButtonObj.GetComponent<Button>()
            : GameObject.Find("LaunchButton").GetComponent<Button>();
        _resetButton = resetButtonObj
            ? resetButtonObj.GetComponent<Button>()
            : GameObject.Find("ResetButton").GetComponent<Button>();
        _solveButton = solveButtonObj
            ? solveButtonObj.GetComponent<Button>()
            : GameObject.Find("SolveButton").GetComponent<Button>();
        _stopButton = stopButtonObj
            ? stopButtonObj.GetComponent<Button>()
            : GameObject.Find("StopButton").GetComponent<Button>();
        _inputField = inputFieldObj
            ? inputFieldObj.GetComponent<InputField>()
            : GameObject.Find("InputField").GetComponent<InputField>();
        _sidesController = rubickObj
            ? rubickObj.GetComponent<SidesController>()
            : GameObject.Find("Rubick").GetComponent<SidesController>();
        _launchButton.interactable = false;
        _solveButton.interactable = false;
    }

    private void Update()
    {
        _stopButton.interactable = _sidesController.isAnyRotating;
    }
}
