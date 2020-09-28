using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{ 
    private readonly Regex _inputFieldFilter = new Regex(@"^[FBRLUD][2']?(?:(?:(\s[FBRLUD][2']?)+$)|$)");
    [SerializeField] private GameObject launchButtonObj;
    [SerializeField] private GameObject inputFieldObj;
    [SerializeField] private GameObject rubickObj;
    [SerializeField] private Color inputFieldStartColor = Color.white;
    [SerializeField] private Color inputFieldWrongColor = new Color(1.0f, 0.0f, 0.0f, 0.5f);
    [SerializeField] private Color inputFieldCorrectColor = new Color(0.0f, 1.0f, 0.0f, 0.5f);
    private SidesController _sidesController;
    private InputField _inputField;
    private Button _launchButton;
    private bool _inputIsValid;

    public void InputField_OnValueChanged()
    {
        if (string.IsNullOrEmpty(_inputField.text))
        {
            _inputField.image.color = inputFieldStartColor;
            return;
        }

        if (_inputFieldFilter.IsMatch(_inputField.text))
        {
            _inputField.image.color = inputFieldCorrectColor;
            _launchButton.interactable = true;
        }
        else
        {
            _inputField.image.color = inputFieldWrongColor;
            _launchButton.interactable = false;
        }
    }

    public void InputFiled_OnEndEdit()
    {
    }
    
    public void LaunchButton_OnClick()
    {
        foreach (var command in _inputField.text.Split(' '))
            _sidesController.AddRotationToQueue(Util.TextToRotationCommand(command));
        _inputField.image.color = inputFieldStartColor;
        _inputField.text = string.Empty;
        _launchButton.interactable = false;
    }

    private void Start()
    {
        _inputField = inputFieldObj
            ? inputFieldObj.GetComponent<InputField>()
            : GameObject.Find("InputField").GetComponent<InputField>();
        _launchButton = launchButtonObj
            ? launchButtonObj.GetComponent<Button>()
            : GameObject.Find("LaunchButton").GetComponent<Button>();
        _sidesController = rubickObj
            ? rubickObj.GetComponent<SidesController>()
            : GameObject.Find("Rubick").GetComponent<SidesController>();
        _launchButton.interactable = false;
    }
}
