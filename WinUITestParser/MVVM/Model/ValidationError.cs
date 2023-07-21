using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WinUITestParser.MVVM.Model
{
    public class ValidationErrorModel : ObservableObject
    {
        public ObservableCollection<ValidationError> Errors { get; set; }

        public int CountError { get; set; }
        public int CountWarning { get; set; }

        public ValidationErrorModel()
        {
            Errors = new();
            CountError = 0;
            CountWarning = 0;
        }

        public ValidationErrorModel(IEnumerable<ValidationError> errors)
            : this()
        {
            Errors = new(errors);
            CountError = errors.Where(x => x.Type == ValidationErrorType.Error).Count();
            CountWarning = errors.Where(x => x.Type == ValidationErrorType.Warning).Count();
        }

        public void SetNoTarget(IEnumerable<ValidationError> errors)
        {
            foreach (var error in errors)
                Errors.Add(error);
            CountWarning += errors.Count();
        }
    }

    public class ValidationError
    {
        public int LineNumber { get; set; }

        public int LinePosition { get; set; }

        public string Message { get; set; }

        public ValidationErrorType Type { get; set; }

        public string IconPath { get; set; }

        private ValidationError(int lineNumber, int linePosition, string message)
        {
            LineNumber = lineNumber;
            LinePosition = linePosition;
            Message = message;
        }

        public ValidationError(int lineNumber, int linePosition, string message, string type)
            : this(lineNumber, linePosition, message)
        {
            switch (type)
            {
                default:
                case "Error":
                    Type = ValidationErrorType.Error;
                    IconPath = "/Assets/StatusIcon/error.png";
                    break;
                case "Warning":
                    Type = ValidationErrorType.Warning;
                    IconPath = "/Assets/StatusIcon/warning.png";
                    break;
            }
        }

        public ValidationError(int lineNumber, int linePosition, string message, ValidationErrorType type)
            : this(lineNumber, linePosition, message)
        {
            Type = type;
            IconPath = type switch
            {
                ValidationErrorType.Ok => "/Assets/StatusIcon/ok.png",
                ValidationErrorType.Error => "/Assets/StatusIcon/error.png",
                ValidationErrorType.Warning => "/Assets/StatusIcon/warning.png",
                _ => "/Assets/StatusIcon/error.png"
            };
        }

        public override string ToString()
            => $"{Type} Line: {LineNumber} Position: {LinePosition} : {Message}";
    }

    public enum ValidationErrorType
    {
        Ok,
        Error,
        Warning
    }
}
