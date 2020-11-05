using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public partial class UIController : MonoBehaviour
    { 
        private readonly Regex _inputFieldFilter = new Regex(@"^[FBRLUD][2']?(?:(?:(\s[FBRLUD][2']?)+$)|$)");
        [SerializeField] private GameObject shuffleButtonObj = null;
        [SerializeField] private GameObject launchButtonObj = null;
        [SerializeField] private GameObject outputPanelObj = null;
        [SerializeField] private GameObject resetButtonObj = null;
        [SerializeField] private GameObject solveButtonObj = null;
        [SerializeField] private GameObject stopButtonObj = null;
        [SerializeField] private GameObject inputFieldObj = null;
        [SerializeField] private GameObject rubikObj = null;
        [SerializeField] private Color inputFieldStartColor = Color.white;
        [SerializeField] private Color inputFieldWrongColor = new Color(1.0f, 0.0f, 0.0f, 0.5f);
        [SerializeField] private Color inputFieldCorrectColor = new Color(0.0f, 1.0f, 0.0f, 0.5f);
        private SidesController _sidesController;
        private InputField _outputField;
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
            _inputField = inputFieldObj
                ? inputFieldObj.GetComponent<InputField>()
                : GameObject.Find("OutputField").GetComponent<InputField>();
            _sidesController = rubikObj
                ? rubikObj.GetComponent<SidesController>()
                : GameObject.Find("Rubick").GetComponent<SidesController>();
            _outputField = outputPanelObj.GetComponentInChildren<InputField>();
            _launchButton.interactable = false;
            outputPanelObj.SetActive(false);
        }

        private void Update()
        {
            if (_sidesController.isAnyRotating)
            {
                _stopButton.interactable = true;
                _resetButton.interactable = false;
                _solveButton.interactable = false;
                _shuffleButton.interactable = false;
            }
            else
            {
                _stopButton.interactable = false;
                _resetButton.interactable = true;
                _solveButton.interactable = true;
                _shuffleButton.interactable = true;
            }
        }
    }
}
