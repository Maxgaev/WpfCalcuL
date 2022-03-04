using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfCalcuL.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace WpfCalcuL.ViewModel
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Событие изменение свойств в XAML
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(PropertyName);
            return true;
        }
        #endregion


        #region Поля

        private Calc calculation;

        private string lastOperation;
        private bool newDisplayRequired = false;
        private string fullExpression;
        private string display;
        #endregion

        #region Конструктор

        public MainWindowViewModel()
        {
            calculation = new Calc();

            
            display = "0";
            FirstOperand = string.Empty;
            SecondOperand = string.Empty;
            Operation = string.Empty;
            lastOperation = string.Empty;
            fullExpression = string.Empty;

            
            OperationButtonPressCommand = new RelayCommand(OnOperationButtonPressCommandExecute, CanOperationButtonPressCommandExecuted);
            SingularOperationButtonPressCommand = new RelayCommand(OnSingularOperationButtonPressCommandExecute, CanSingularOperationButtonPressCommandExecuted);
            DigitButtonPressCommand = new RelayCommand(OnDigitButtonPressCommandExecute, CanDigitButtonPressCommandExecuted);
        }
        #endregion

        #region Свойства

        public string FirstOperand
        {
            get => calculation.FirstOperand;
            set { calculation.FirstOperand = value; }
        }

        
        public string SecondOperand
        {
            get => calculation.SecondOperand;
            set { calculation.SecondOperand = value; }
        }

        
        public string Operation
        {
            get => calculation.Operation;
            set { calculation.Operation = value; }
        }

        public string LastOperation
        {
            get => lastOperation;
            set { lastOperation = value; }
        }

        
        public string Result
        {
            get => calculation.Result;
        }

        
        public string Display
        {
            get => display;
            set => Set(ref display, value);
        }

        
        public string FullExpression
        {
            get => fullExpression;
            set => Set(ref fullExpression, value);
        }
        #endregion

        #region Команды


        public ICommand OperationButtonPressCommand { get; }

        private void OnOperationButtonPressCommandExecute(object p)
        {
            if (FirstOperand == string.Empty || LastOperation == "=")
            {
                FirstOperand = display;
                LastOperation = p.ToString();
            }
            else
            {
                SecondOperand = display;
                Operation = lastOperation;
                calculation.CalculateResult();

                if (Operation == "/" && SecondOperand == "0")
                {
                    Display = "Ошибка";
                    newDisplayRequired = true;
                    return;
                }
                else
                {
                    FullExpression = Math.Round(Convert.ToDouble(FirstOperand), 10) + Operation
                        + Math.Round(Convert.ToDouble(SecondOperand), 10) + "="
                        + Math.Round(Convert.ToDouble(Result), 10);

                    
                    LastOperation = p.ToString();
                    Display = Result;
                    FirstOperand = display;
                }
            }
            newDisplayRequired = true;
        }

        private bool CanOperationButtonPressCommandExecuted(object p) => true;

        public ICommand SingularOperationButtonPressCommand { get; }

        private void OnSingularOperationButtonPressCommandExecute(object p)
        {
            FirstOperand = Display;
            Operation = p.ToString();
            calculation.CalculateResult();

            if (Operation == "√х" && Convert.ToDouble(FirstOperand) < 0)
            {
                Display = "Ошибка";
                newDisplayRequired = true;
                return;
            }
            else
            {
                FullExpression = Operation + "(" + Math.Round(Convert.ToDouble(FirstOperand), 10) + ")="
                    + Math.Round(Convert.ToDouble(Result), 10);

               
                LastOperation = "=";
                Display = Result;
                FirstOperand = display;
                newDisplayRequired = true;
            }
        }

        private bool CanSingularOperationButtonPressCommandExecuted(object p) => true;

        public ICommand DigitButtonPressCommand { get; }

        private void OnDigitButtonPressCommandExecute(object p)
        {
            switch (p)
            {
                case "C":
                    Display = "0";
                    FirstOperand = string.Empty;
                    SecondOperand = string.Empty;
                    Operation = string.Empty;
                    LastOperation = string.Empty;
                    break;
                case ",":
                    if (newDisplayRequired)
                    {
                        Display = "0,";
                    }
                    else
                    {
                        if (!display.Contains(","))
                        {
                            Display = display + ",";
                        }
                    }
                    break;
                default:
                    Display = display == "0" || newDisplayRequired ? p.ToString() : display + p.ToString();
                    break;
            }
            newDisplayRequired = false;
        }

        private bool CanDigitButtonPressCommandExecuted(object p) => true;
        #endregion
    }
}
