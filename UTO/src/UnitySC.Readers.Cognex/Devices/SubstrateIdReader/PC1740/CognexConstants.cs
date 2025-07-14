namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740
{
    public static class CognexConstants
    {
        public const string CognexEquipmentAlias = "Cognex";

        //Equipment => Ocr
        public const string User     = "admin";
        public const string Password = "";

        //EQUIPMENT CONTROL low level communication details
        public const int CommandTimeout   = 90000;
        public const string GoodPrompt    = ">";
        public const string NotGoodPrompt = "?";
        public const int StatusWordLength = 4;

        //OCRREADER MOTOR
        public const string OCRMotorCommunicationId   = "X";
        public const string OCRMotorCommandPostfix    = "\r\n";
        public const string OCRMotorEndReplyIndicator = "\r\n";
        public const int OCRMotorMaxReplyLength       = 100;

        //COGNEX
        public const int MaxCognexMessageLength           = 100;
        public const int CognexTCPConnectTimeout          = 5000;
        public const string CognexCommandPrefix           = "";
        public const string CognexCommandPostfix          = "\r\n";
        public const string CognexEndReplyIndicatorLogin  = ":";
        public const string CognexInvalidCommandIndicator = "[Invalid Command]";
        public const char CognexParametersSeparator       = ',';
        public const string CognexLoginStartIndicator     = "User:";
        public const string CognexPasswordIndicator       = "Password:";
        public const string CognexLoginCompletedIndicator = "User Logged In\r\n";
        public const string CognexLoginWelcome            = "Welcome to In-Sight";

        //READ
        public const string CognexEndReplyIndicatorRead        = "\r\n";
        public const byte CognexScribeStartLength              = 1; // "["
        public const byte CognexScribeEndingLength             = 1; // "]"
        public const byte CognexReadMacroParamNumber           = 2; // string + result
        public const string CognexNotReadScribeIndicator       = @"*";
        public const string CognexNotReadScribeSymbolIndicator = @"?";

        /// <summary>
        /// Composite commands names
        /// </summary>
        public const string MacroLoginRequest = "LoginRequest";

        public const string MacroLogin = "Login";
        public const string MacroRead = "Read";

        /// <summary>
        /// Reply Codes
        /// </summary>
        public const string CognexGoodAcknowledge = "1";

        public static readonly string[][] SOErrorStrings = new[]
        {
            new[] { "0", "Unrecognized command" },
            new[] { "-1", "The value given for int is either out of range, or is not a valid integer" },
            new[] { "-2", "The command could not be executed" },
            new[]
            {
                "-5",
                "The communications flag was successful but the sensor did not go Online because the sensor is set Offline manually through the In-Sight Explorer user interface or by a Discrete I/O signal"
            },
            new[] { "-6", "User does not have Full Access to execute the command" }
        };

        public static readonly string[][] SW8ErrorStrings = new[]
        {
            new[] { "0", "Unrecognized command" },
            new[] { "-1", "The number is either out of range (0 to 8) or not an integer." },
            new[] { "-2", "The command could not be executed" },
            new[] { "-6", "User does not have Full Access to execute the command" }
        };

        public static readonly string[][] LFErrorStrings = new[]
        {
            new[] { "0", "Unrecognized command" }, new[] { "-1", "The filename is missing" },
            new[] { "-2", "The job failed to load, the sensor is online, or the file was not found" },
            new[] { "-4", "The job to be loaded was not found, or the sensor is out of memory" },
            new[] { "-6", "User does not have Full Access to execute the command" }
        };

        public static readonly string[][] SetImageFileNameErrorStrings = new[]
        {
            new[] { "0", "Unrecognized command" }, new[] { "-1", "The cell ID is invalid" },
            new[]
            {
                "-2",
                "The input string is longer than the specified maximum string length in the EditString function or the cell does not contain an EditString function"
            },
            new[] { "-6", "User does not have Full Access to execute the command" }
        };

        public static readonly string[][] ReadErrorStrings = new[]
        {
            new[] { "0", "Unrecognized command" },
            new[] { "-1", "The number is either out of range (0 to 8) or not an integer" },
            new[] { "-2", "The command could not be executed, or the sensor is Offline" },
            new[] { "-6", "User does not have Full Access to execute the command" }
        };

        public static readonly string[][] GetFileListErrorStrings = new[]
        {
            new[] { "1", "The command was executed successfully" }, new[] { "0", "Unrecognized command" },
            new[] { "-1", "The input is invalid or has exceeded 15 bytes in length" },
            new[] { "-2", "The command could not be executed" }
        };

        public static readonly string[][] GetImageErrorStrings = new[]
        {
            new[] { "1", "The command was executed successfully" }, new[] { "0", "Unrecognized command" },
            new[] { "-4", "The In-Sight sensor is out of memory" },
            new[] { "-6", "User does not have Full Access to execute the command." }
        };

        public static readonly string[][] GVJobErrorStrings = new[]
        {
            new[] { "0", "Unrecognized command" }, new[] { "-1", "symbolic tag is invalid" },
            new[] { "-2", "command could not be executed" }
        };
    }
}
