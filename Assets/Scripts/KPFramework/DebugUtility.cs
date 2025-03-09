using Debug = UnityEngine.Debug;
using System.Runtime.CompilerServices;

namespace KPFramework
{
    public class DebugUtility
    {
        public static void LogError(ErrorType errorType, string value, [CallerFilePath]string filePath = "", [CallerMemberName]string nameOfMethod="")
        {
            string nameOfScript = System.IO.Path.GetFileNameWithoutExtension(filePath);

            value = $"<color=yellow>{value}</color>";
            switch (errorType)
            {
                case ErrorType.SwitchCaseNotFound:
                    ErrorLogHandler(0, $"Specified case({value}) is not implemented.");
                    break;

                case ErrorType.WrongEventParameter:
                    ErrorLogHandler(1, $"Wrong parameters sent to event({value}).");
                    break;

                case ErrorType.WaitForSingleton:
                    ErrorLogHandler(2, $"You should wait for singleton initialization.");
                    break;

                case ErrorType.KeyNotFound:
                    ErrorLogHandler(3, $"Key({value}) not found.");
                    break;

                case ErrorType.NotInitialized:
                    ErrorLogHandler(4, $"{value} not initialized.");
                    break;

                //case ErrorType.UILocalizationKeyIsBypassed:
                //    ErrorLogHandler(5, $"Localization key bypassed on: {value}");
                //    break;

                case ErrorType.NotImplemented:
                    ErrorLogHandler(6, $"{value} not implemented.");
                    break;

                case ErrorType.ComponentNotFound:
                    ErrorLogHandler(7, $"Component {value} not found.");
                    break;

                case ErrorType.MethodParameterIsNull:
                    ErrorLogHandler(8, $"Method parameter {value} not found.");
                    break;

                case ErrorType.Localization:
                    ErrorLogHandler(9, $"Localization key {value} couldn't found.", "cyan");
                    break;

                case ErrorType.DuplicatedKey:
                    ErrorLogHandler(10, $"{value} duplicated!");
                    break;

                case ErrorType.TagShouldBe:
                    ErrorLogHandler(11, $"Tag should be: {value}");
                    break;
                case ErrorType.MajorError:
                    ErrorLogHandler(666, $"MAJOR ERROR: {value}");
                    ErrorLogHandler(666, $"MAJOR ERROR: {value}");
                    ErrorLogHandler(666, $"MAJOR ERROR: {value}");
                    ErrorLogHandler(666, $"MAJOR ERROR: {value}");
                    ErrorLogHandler(666, $"MAJOR ERROR: {value}");
                    ErrorLogHandler(666, $"MAJOR ERROR: {value}");
                    break;

                default:
                    LogError(ErrorType.SwitchCaseNotFound, errorType.ToString());
                    break;
            }

            void ErrorLogHandler(int errorCode, string msg, string color = "yellow")
            {
                
                Log($"ERROR: <color={color}>{errorCode}</color>, {msg}. <color=white>{nameOfScript}.{nameOfMethod}</color>", DebugType.Error);
            }
        }

        public static void Log(string log, DebugType logType)
        {
            var color = "white";
            var editorOnly = false;
            switch (logType)
            {
                case DebugType.EditorOnly:
                    editorOnly = true; break;
                case DebugType.ForLater:
                    color = "pink";
                    editorOnly = true; break;
                case DebugType.Error:
                    color = "red";
                    editorOnly = false; break;
                case DebugType.Warning:
                    color = "yellow";
                    editorOnly = true; break;
            }

            if (editorOnly)
            {
#if UNITY_EDITOR
                Debug.Log($"<color={color}>EDITOR: {log}</color>");
#endif
            }
            else
            {
                Debug.Log($"<color={color}>{log}</color>");
            }
        }
    }
}
