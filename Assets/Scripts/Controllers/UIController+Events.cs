using System;
using Solver;
using Util;

namespace Controllers
{
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
                _sidesController.AddRotationToQueue(Util.Tools.TextToRotationCommand(command));
            _inputField.image.color = inputFieldStartColor;
            _inputField.text = string.Empty;
        }

        public void SolveButton_OnClick()
        {
            if (!outputPanelObj.activeSelf)
            {
                _outputField.text = string.Empty;
                outputPanelObj.SetActive(true);
            }
            
            var solverSides = _sidesController.GetSolverSides();
            var cube = new SolverCube(solverSides);
            var solver = new RubikSolver(cube);
            var text = string.Empty;

            solver.SolveStep0();
            solver.SolveStep1();
            solver.SolveStep2();
            solver.SolveStep3();
            solver.SolveStep4();
            foreach (var rotation in solver.GetRotationsArray())
                text = string.Concat(text, Tools.RotationCommandToText(rotation), " ");
            _outputField.text = text.Length > 0 ? text.Remove(text.Length - 1) : text;
        }

        public void CloseOutputPanelButton_OnClick()
        {
            outputPanelObj.SetActive(false);
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
}
