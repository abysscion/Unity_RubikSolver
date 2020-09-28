public partial class UIController
{ 
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
    
    public void ShuffleButton_OnClick()
    {
        _sidesController.ShuffleSides();
    }
    
    public void LaunchButton_OnClick()
    {
        foreach (var command in _inputField.text.Split(' '))
            _sidesController.AddRotationToQueue(Util.TextToRotationCommand(command));
        _inputField.image.color = inputFieldStartColor;
        _inputField.text = string.Empty;
        _launchButton.interactable = false;
    }

    public void SolveButton_OnClick()
    {
        
    }
    
    public void ResetButton_OnClick()
    {
        _sidesController.resetSidesRequired = true;
    }
    
    public void StopButton_OnClick()
    {
        _sidesController.StopRotating();
    }
}
