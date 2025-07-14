using System;
using System.Text;

using PI;

using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    /// <summary>
    /// This class wraps the provided C# API published by Physical Instrumente (PI) 'PI_GCS2.cs'.
    ///
    /// Function signatures are as closed as the PI API, mainly for testing purposes:
    ///   - the return type is a integer indicating if the call was successful (1) or not (0) -- except for the connect function,
    ///   - values to be received (getters starting with 'q') or defined (setters) are set as a function parameters.
    ///
    /// Some simplifications were made though:
    ///   - Single axis requests are implemented,
    ///   - 'ConnectionID' and 'AxisName' are implicilty passed to the API call (and thus must be initialized before any call using them),
    ///   - Some internal IDs (e.g. waveGeneratorId) are specified in the Configuration,
    ///   - Array lenghts are deduced whenever it is possible.
    /// </summary>
    public class PIE709ControllerApiWrapper : PiezoControllerApiWrapper
    {
        public PIE709ControllerConfig Configuration;

        public PIE709ControllerApiWrapper(PIE709ControllerConfig configuration)
        {
            Configuration = configuration;
        }

        /******************
         *   CONNECTION   *
         ******************/
        public virtual int ConnectionID { get; set; } = -1;

        // Open using the RS-232 standard
        public virtual void ConnectRS232(int port, int baudRate)
        {
            ConnectionID = GCS2.ConnectRS232(port, baudRate);
        }

        // Close
        public virtual void CloseConnection(int connectionID)
        { GCS2.CloseConnection(connectionID); }

#pragma warning disable IDE1006  //Disable warning on naming rule convention
        /************
         *   MISC   *
         ************/

        public virtual int qHLP(StringBuilder helpAsString)
        { return GCS2.qHLP(ConnectionID, helpAsString, helpAsString.Capacity); }

        public virtual int qHPA(StringBuilder supportedParameters)
        { return GCS2.qHPA(ConnectionID, supportedParameters, supportedParameters.Capacity); }

        /**************
         *   RIGHTS   *
         **************/

        public virtual void CCL(CommandLevel commandLevel, string password)
        { ErrorHandler(GCS2.CCL(ConnectionID, (int)commandLevel, password)); }

        /******************
         *   PROPERTIES   *
         ******************/

        // Axis names
        public string AxisName { get; set; } = null;

        public virtual int qSAI(StringBuilder axisNames)
        { return GCS2.qSAI(ConnectionID, axisNames, axisNames.Capacity); }

        // Servo mode
        public virtual int qSVO(int[] servoModeValue)
        { return GCS2.qSVO(ConnectionID, AxisName, servoModeValue); }

        public virtual int SVO(int[] servoModeValue)
        { return GCS2.SVO(ConnectionID, AxisName, servoModeValue); }

        // Velocity
        public virtual int qVEL(double[] velocity)
        { return GCS2.qVEL(ConnectionID, AxisName, velocity); }

        public virtual int VEL(double[] velocity)
        { return GCS2.VEL(ConnectionID, AxisName, velocity); }

        // Min Position
        public virtual int qTMN(double[] minPosition)
        { return GCS2.qTMN(ConnectionID, AxisName, minPosition); }

        // Max Position
        public virtual int qTMX(double[] maxPosition)
        { return GCS2.qTMX(ConnectionID, AxisName, maxPosition); }

        // Current Position
        public virtual int qPOS(double[] currentPosition)
        { return GCS2.qPOS(ConnectionID, AxisName, currentPosition); }

        // Auto Zero
        public virtual int qATZ(int[] iAtzResultArray)
        { return GCS2.qATZ(ConnectionID, AxisName, iAtzResultArray); }

        public virtual int ATZ(double[] dLowvoltageArray)
        { return GCS2.ATZ(ConnectionID, AxisName, new double[] { double.NaN }, new int[] { 1 }); }

        /******************
         *    MOVEMENT    *
         ******************/

        // Status
        public virtual int IsMoving(int[] isMoving)
        { return GCS2.IsMoving(ConnectionID, AxisName, isMoving); }

        // Position command
        public virtual int MOV(double[] position)
        { return GCS2.MOV(ConnectionID, AxisName, position); }

        /******************
         *   TRIGGER IN   *
         ******************/
        private int[] triggerInLineId => new int[] { Configuration.TriggerInLineId };

        // State
        public virtual int qTRI(int[] triggerInState)
        { return GCS2.qTRI(ConnectionID, triggerInLineId, triggerInState, 1); }

        public virtual int TRI(int[] triggerInState)
        { return GCS2.TRI(ConnectionID, triggerInLineId, triggerInState, 1); }

        // Configuration
        public virtual int qCTI(StringBuilder paramValue, TriggerInParam paramKey)
        { return GCS2.qCTI(ConnectionID, triggerInLineId, new int[] { (int)paramKey }, paramValue, 1, paramValue.Capacity); }

        public virtual int CTI(string paramValue, TriggerInParam paramKey)
        { return GCS2.CTI(ConnectionID, triggerInLineId, new int[] { (int)paramKey }, paramValue, 1); }

        // Signal
        public virtual int qDIO(int[] triggerInSignal)
        { return GCS2.qDIO(ConnectionID, triggerInLineId, triggerInSignal, 1); }

        /******************
         *   TRIGGER OUT  *
         ******************/
        private int[] triggerOutLineId => new int[] { Configuration.TriggerOutLineId };

        // Configuration
        public virtual int qCTO(double[] paramValue, TriggerOutParam paramKey)
        { return GCS2.qCTO(ConnectionID, triggerOutLineId, new int[] { (int)paramKey }, paramValue, 1); }

        public virtual int CTO(double[] paramValue, TriggerOutParam paramKey)
        { return GCS2.CTO(ConnectionID, triggerOutLineId, new int[] { (int)paramKey }, paramValue, 1); }

        /******************
         * WAVE GENERATOR *
         ******************/
        private int[] waveGeneratorId => new int[] { Configuration.WaveGeneratorId };

        // Wave generator running state
        public virtual int IsGeneratorRunning(int[] isGeneratorRunning)
        { return GCS2.IsGeneratorRunning(ConnectionID, waveGeneratorId, isGeneratorRunning, 1); }

        // Wave generator offset
        public virtual int qWOS(double[] generatorOffset)
        { return GCS2.qWOS(ConnectionID, waveGeneratorId, generatorOffset, 1); }

        public virtual int WOS(double[] generatorOffset)
        { return GCS2.WOS(ConnectionID, waveGeneratorId, generatorOffset, 1); }

        // Wave Id plugged to the generator
        public virtual int qWSL(int[] waveTableId)
        { return GCS2.qWSL(ConnectionID, waveGeneratorId, waveTableId, 1); }

        public virtual int WSL(int[] waveTableId)
        { return GCS2.WSL(ConnectionID, waveGeneratorId, waveTableId, 1); }

        // Generator starting mode
        public virtual int qWGO(int[] generatorStartingMode)
        { return GCS2.qWGO(ConnectionID, waveGeneratorId, generatorStartingMode, 1); }

        public virtual int WGO(int[] generatorStartingMode)
        { return GCS2.WGO(ConnectionID, waveGeneratorId, generatorStartingMode, 1); }

        // Wave forms definition
        public virtual void WAV_LIN(int waveTableId, int firstPointOffset, int pointsCount, int writeOption, int speedUpDownPointsCount, double amplitude, double amplitudeOffset)
        { ErrorHandler(GCS2.WAV_LIN(ConnectionID, waveTableId, firstPointOffset, pointsCount, writeOption, speedUpDownPointsCount, amplitude, amplitudeOffset, firstPointOffset + pointsCount)); }

        // Waves
        public virtual void qGWD_SYNC(int waveTableId, int firstPointOffset, int wavePointsCount, double[] wave)
        { ErrorHandler(GCS2.qGWD_SYNC(ConnectionID, waveTableId, firstPointOffset, wavePointsCount, wave)); }

        // Wave points count
        public virtual void qWAV(int waveTableId, WaveParam waveParamKey, double[] waveParamValue)
        { ErrorHandler(GCS2.qWAV(ConnectionID, new int[] { waveTableId }, new int[] { (int)waveParamKey }, waveParamValue, 1)); }

        // Generator's number of output cycles
        public virtual int qWGC(int[] cycleCount)
        { return GCS2.qWGC(ConnectionID, waveGeneratorId, cycleCount, 1); }

        public virtual int WGC(int[] cycleCount)
        { return GCS2.WGC(ConnectionID, waveGeneratorId, cycleCount, 1); }

        // Generator's table rate
        public virtual int qWTR(int[] tableRate)
        { return GCS2.qWTR(ConnectionID, waveGeneratorId, tableRate, new int[1], 1); }

        /******************
         *      MISC      *
         ******************/

        // Harware specific parameters
        public virtual int qSPA(double[] paramValue, uint paramKey)
        { return GCS2.qSPA(ConnectionID, AxisName, new uint[] { paramKey }, paramValue, null, 0); }

        public virtual int SPA(double[] paramValue, uint paramKey)
        { return GCS2.SPA(ConnectionID, AxisName, new uint[] { paramKey }, paramValue, null); }

        // Errors
        public virtual int GetError()
        { return GCS2.GetError(ConnectionID); }

        public virtual int TranslateError(int errorCode, StringBuilder errorMessage)
        { return GCS2.TranslateError(errorCode, errorMessage, errorMessage.Capacity); }

        /******************
         *    HELPERS     *
         ******************/

        private bool isFailure(int operationResult) => operationResult == (int)OperationResult.Fail;

        /// <summary>
        /// Helper wrapping the call with some logs and throws an error in case of failure.
        /// </summary>
        /// <param name="operationResult">Operation result.</param>
        /// <exception cref="Exception">Trown if an error occurs.</exception>
        public void ErrorHandler(int operationResult)
        {
            if (isFailure(operationResult))
            {
                int errorCode = GetError();
                var translatedErrorMessage = new StringBuilder(1024);
                TranslateError(errorCode, translatedErrorMessage);
                throw new Exception($"PI Controller error occured: error code={errorCode}, message={translatedErrorMessage}");
            }
        }

        public int DynamicMethodCall(string methodName, object[] parameters)
        {
            return (int)GetType().GetMethod(methodName).Invoke(this, parameters);
        }

        /// <summary>
        /// Helper intended to simplify api calls to get values.
        /// </summary>
        /// <typeparam name="T">Parameter type to be returned.</typeparam>
        /// <param name="apiMethodName">Api method name to be called.</param>
        /// <returns>The parameter value.</returns>
        public T WrappedGetMethod<T>(string apiMethodName, object optionalParameter = null)
        {
            var response = new T[1];
            object[] parameters = optionalParameter == null ? new object[] { response } : new object[] { response, optionalParameter };
            ErrorHandler(DynamicMethodCall(apiMethodName, parameters));
            return response[0];
        }

        public string WrappedGetMethod(string methodName, object optionalParameter = null, int stringBufferCapacity = 1024)
        {
            var response = new StringBuilder(stringBufferCapacity);
            object[] parameters = optionalParameter == null ? new object[] { response } : new object[] { response, optionalParameter };
            ErrorHandler(DynamicMethodCall(methodName, parameters));
            return response.ToString();
        }

        /// <summary>
        /// Helper intended to simplify api calls to set values.
        /// </summary>
        /// <typeparam name="T">Parameter type to be set.</typeparam>
        /// <param name="apiMethodName">Api method name to be called.</param>
        /// <param name="value">Parameter value to be set.</param>
        public void WrappedSetMethod<T>(string apiMethodName, T value, object optionalParameter = null)
        {
            object[] parameters = optionalParameter == null ? new object[] { new T[] { value } } : new object[] { new T[] { value }, optionalParameter };
            ErrorHandler(DynamicMethodCall(apiMethodName, parameters));
        }

        public void WrappedSetMethod(string apiMethodName, string value, object optionalParameter = null)
        {
            object[] parameters = optionalParameter == null ? new object[] { value } : new object[] { value, optionalParameter };
            ErrorHandler(DynamicMethodCall(apiMethodName, parameters));
        }
    }

#pragma warning restore IDE1006
}
