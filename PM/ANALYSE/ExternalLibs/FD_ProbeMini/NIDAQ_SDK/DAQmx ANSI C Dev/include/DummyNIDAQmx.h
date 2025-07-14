
// NI-DAQmx Typedefs
#ifndef _NI_int8_DEFINED_
#define _NI_int8_DEFINED_
	typedef signed char        int8;
#endif
#ifndef _NI_uInt8_DEFINED_
#define _NI_uInt8_DEFINED_
	typedef unsigned char      uInt8;
#endif
#ifndef _NI_int16_DEFINED_
#define _NI_int16_DEFINED_
	typedef signed short       int16;
#endif
#ifndef _NI_uInt16_DEFINED_
#define _NI_uInt16_DEFINED_
	typedef unsigned short     uInt16;
#endif
#ifndef _NI_int32_DEFINED_
#define _NI_int32_DEFINED_
	typedef signed long        int32;
#endif
#ifndef _NI_uInt32_DEFINED_
#define _NI_uInt32_DEFINED_
	typedef unsigned long      uInt32;
#endif
#ifndef _NI_float32_DEFINED_
#define _NI_float32_DEFINED_
	typedef float              float32;
#endif
#ifndef _NI_float64_DEFINED_
#define _NI_float64_DEFINED_
	typedef double             float64;
#endif
#ifndef _NI_int64_DEFINED_
#define _NI_int64_DEFINED_
#ifdef __linux__
	typedef long long int      int64;
#else
	typedef __int64            int64;
#endif
#endif
#ifndef _NI_uInt64_DEFINED_
#define _NI_uInt64_DEFINED_
#ifdef __linux__
	typedef unsigned long long uInt64;
#else
	typedef unsigned __int64   uInt64;
#endif
#endif

typedef uInt32             bool32;
typedef void*             TaskHandle;

#ifndef TRUE
 #define TRUE            (1L)
#endif
#ifndef FALSE
 #define FALSE           (0L)
#endif
#ifndef NULL
 #define NULL            (0L)
#endif


/******************************************************************************
 *** NI-DAQmx Attributes ******************************************************
 ******************************************************************************/

//********** Buffer Attributes **********
#define DAQmx_Buf_Input_BufSize                                          0x186C // Specifies the number of samples the input buffer can hold for each channel in the task. Zero indicates to allocate no buffer. Use a buffer size of 0 to perform a hardware-timed operation without using a buffer. Setting this property overrides the automatic input buffer allocation that NI-DAQmx performs.
#define DAQmx_Buf_Input_OnbrdBufSize                                     0x230A // Indicates in samples per channel the size of the onboard input buffer of the device.
#define DAQmx_Buf_Output_BufSize                                         0x186D // Specifies the number of samples the output buffer can hold for each channel in the task. Zero indicates to allocate no buffer. Use a buffer size of 0 to perform a hardware-timed operation without using a buffer. Setting this property overrides the automatic output buffer allocation that NI-DAQmx performs.
#define DAQmx_Buf_Output_OnbrdBufSize                                    0x230B // Specifies in samples per channel the size of the onboard output buffer of the device.

//********** Calibration Info Attributes **********
#define DAQmx_SelfCal_Supported                                          0x1860 // Indicates whether the device supports self calibration.
#define DAQmx_SelfCal_LastTemp                                           0x1864 // Indicates in degrees Celsius the temperature of the device at the time of the last self calibration. Compare this temperature to the current onboard temperature to determine if you should perform another calibration.
#define DAQmx_ExtCal_RecommendedInterval                                 0x1868 // Indicates in months the National Instruments recommended interval between each external calibration of the device.
#define DAQmx_ExtCal_LastTemp                                            0x1867 // Indicates in degrees Celsius the temperature of the device at the time of the last external calibration. Compare this temperature to the current onboard temperature to determine if you should perform another calibration.
#define DAQmx_Cal_UserDefinedInfo                                        0x1861 // Specifies a string that contains arbitrary, user-defined information. This number of characters in this string can be no more than Max Size.
#define DAQmx_Cal_UserDefinedInfo_MaxSize                                0x191C // Indicates the maximum length in characters of Information.
#define DAQmx_Cal_DevTemp                                                0x223B // Indicates in degrees Celsius the current temperature of the device.

//********** Channel Attributes **********
#define DAQmx_AI_Max                                                     0x17DD // Specifies the maximum value you expect to measure. This value is in the units you specify with a units property. When you query this property, it returns the coerced maximum value that the device can measure with the current settings.
#define DAQmx_AI_Min                                                     0x17DE // Specifies the minimum value you expect to measure. This value is in the units you specify with a units property.  When you query this property, it returns the coerced minimum value that the device can measure with the current settings.
#define DAQmx_AI_CustomScaleName                                         0x17E0 // Specifies the name of a custom scale for the channel.
#define DAQmx_AI_MeasType                                                0x0695 // Indicates the measurement to take with the analog input channel and in some cases, such as for temperature measurements, the sensor to use.
#define DAQmx_AI_Voltage_Units                                           0x1094 // Specifies the units to use to return voltage measurements from the channel.
#define DAQmx_AI_Temp_Units                                              0x1033 // Specifies the units to use to return temperature measurements from the channel.
#define DAQmx_AI_Thrmcpl_Type                                            0x1050 // Specifies the type of thermocouple connected to the channel. Thermocouple types differ in composition and measurement range.
#define DAQmx_AI_Thrmcpl_CJCSrc                                          0x1035 // Indicates the source of cold-junction compensation.
#define DAQmx_AI_Thrmcpl_CJCVal                                          0x1036 // Specifies the temperature of the cold junction if CJC Source is DAQmx_Val_ConstVal. Specify this value in the units of the measurement.
#define DAQmx_AI_Thrmcpl_CJCChan                                         0x1034 // Indicates the channel that acquires the temperature of the cold junction if CJC Source is DAQmx_Val_Chan. If the channel is a temperature channel, NI-DAQmx acquires the temperature in the correct units. Other channel types, such as a resistance channel with a custom sensor, must use a custom scale to scale values to degrees Celsius.
#define DAQmx_AI_RTD_Type                                                0x1032 // Specifies the type of RTD connected to the channel.
#define DAQmx_AI_RTD_R0                                                  0x1030 // Specifies in ohms the sensor resistance at 0 deg C. The Callendar-Van Dusen equation requires this value. Refer to the sensor documentation to determine this value.
#define DAQmx_AI_RTD_A                                                   0x1010 // Specifies the 'A' constant of the Callendar-Van Dusen equation. NI-DAQmx requires this value when you use a custom RTD.
#define DAQmx_AI_RTD_B                                                   0x1011 // Specifies the 'B' constant of the Callendar-Van Dusen equation. NI-DAQmx requires this value when you use a custom RTD.
#define DAQmx_AI_RTD_C                                                   0x1013 // Specifies the 'C' constant of the Callendar-Van Dusen equation. NI-DAQmx requires this value when you use a custom RTD.
#define DAQmx_AI_Thrmstr_A                                               0x18C9 // Specifies the 'A' constant of the Steinhart-Hart thermistor equation.
#define DAQmx_AI_Thrmstr_B                                               0x18CB // Specifies the 'B' constant of the Steinhart-Hart thermistor equation.
#define DAQmx_AI_Thrmstr_C                                               0x18CA // Specifies the 'C' constant of the Steinhart-Hart thermistor equation.
#define DAQmx_AI_Thrmstr_R1                                              0x1061 // Specifies in ohms the value of the reference resistor if you use voltage excitation. NI-DAQmx ignores this value for current excitation.
#define DAQmx_AI_ForceReadFromChan                                       0x18F8 // Specifies whether to read from the channel if it is a cold-junction compensation channel. By default, an NI-DAQmx Read function does not return data from cold-junction compensation channels.  Setting this property to TRUE forces read operations to return the cold-junction compensation channel data with the other channels in the task.
#define DAQmx_AI_Current_Units                                           0x0701 // Specifies the units to use to return current measurements from the channel.
#define DAQmx_AI_Strain_Units                                            0x0981 // Specifies the units to use to return strain measurements from the channel.
#define DAQmx_AI_StrainGage_GageFactor                                   0x0994 // Specifies the sensitivity of the strain gage.  Gage factor relates the change in electrical resistance to the change in strain. Refer to the sensor documentation for this value.
#define DAQmx_AI_StrainGage_PoissonRatio                                 0x0998 // Specifies the ratio of lateral strain to axial strain in the material you are measuring.
#define DAQmx_AI_StrainGage_Cfg                                          0x0982 // Specifies the bridge configuration of the strain gages.
#define DAQmx_AI_Resistance_Units                                        0x0955 // Specifies the units to use to return resistance measurements.
#define DAQmx_AI_Freq_Units                                              0x0806 // Specifies the units to use to return frequency measurements from the channel.
#define DAQmx_AI_Freq_ThreshVoltage                                      0x0815 // Specifies the voltage level at which to recognize waveform repetitions. You should select a voltage level that occurs only once within the entire period of a waveform. You also can select a voltage that occurs only once while the voltage rises or falls.
#define DAQmx_AI_Freq_Hyst                                               0x0814 // Specifies in volts a window below Threshold Level. The input voltage must pass below Threshold Level minus this value before NI-DAQmx recognizes a waveform repetition at Threshold Level. Hysteresis can improve the measurement accuracy when the signal contains noise or jitter.
#define DAQmx_AI_LVDT_Units                                              0x0910 // Specifies the units to use to return linear position measurements from the channel.
#define DAQmx_AI_LVDT_Sensitivity                                        0x0939 // Specifies the sensitivity of the LVDT. This value is in the units you specify with Sensitivity Units. Refer to the sensor documentation to determine this value.
#define DAQmx_AI_LVDT_SensitivityUnits                                   0x219A // Specifies the units of Sensitivity.
#define DAQmx_AI_RVDT_Units                                              0x0877 // Specifies the units to use to return angular position measurements from the channel.
#define DAQmx_AI_RVDT_Sensitivity                                        0x0903 // Specifies the sensitivity of the RVDT. This value is in the units you specify with Sensitivity Units. Refer to the sensor documentation to determine this value.
#define DAQmx_AI_RVDT_SensitivityUnits                                   0x219B // Specifies the units of Sensitivity.
#define DAQmx_AI_SoundPressure_MaxSoundPressureLvl                       0x223A // Specifies the maximum instantaneous sound pressure level you expect to measure. This value is in decibels, referenced to 20 micropascals. NI-DAQmx uses the maximum sound pressure level to calculate values in pascals for Maximum Value and Minimum Value for the channel.
#define DAQmx_AI_SoundPressure_Units                                     0x1528 // Specifies the units to use to return sound pressure measurements from the channel.
#define DAQmx_AI_Microphone_Sensitivity                                  0x1536 // Specifies the sensitivity of the microphone. This value is in mV/Pa. Refer to the sensor documentation to determine this value.
#define DAQmx_AI_Accel_Units                                             0x0673 // Specifies the units to use to return acceleration measurements from the channel.
#define DAQmx_AI_Accel_Sensitivity                                       0x0692 // Specifies the sensitivity of the accelerometer. This value is in the units you specify with Sensitivity Units. Refer to the sensor documentation to determine this value.
#define DAQmx_AI_Accel_SensitivityUnits                                  0x219C // Specifies the units of Sensitivity.
#define DAQmx_AI_TEDS_Units                                              0x21E0 // Indicates the units defined by TEDS information associated with the channel.
#define DAQmx_AI_Coupling                                                0x0064 // Specifies the coupling for the channel.
#define DAQmx_AI_Impedance                                               0x0062 // Specifies the input impedance of the channel.
#define DAQmx_AI_TermCfg                                                 0x1097 // Specifies the terminal configuration for the channel.
#define DAQmx_AI_InputSrc                                                0x2198 // Specifies the source of the channel. You can use the signal from the I/O connector or one of several calibration signals. Certain devices have a single calibration signal bus. For these devices, you must specify the same calibration signal for all channels you connect to a calibration signal.
#define DAQmx_AI_ResistanceCfg                                           0x1881 // Specifies the resistance configuration for the channel. NI-DAQmx uses this value for any resistance-based measurements, including temperature measurement using a thermistor or RTD.
#define DAQmx_AI_LeadWireResistance                                      0x17EE // Specifies in ohms the resistance of the wires that lead to the sensor.
#define DAQmx_AI_Bridge_Cfg                                              0x0087 // Specifies the type of Wheatstone bridge that the sensor is.
#define DAQmx_AI_Bridge_NomResistance                                    0x17EC // Specifies in ohms the resistance across each arm of the bridge in an unloaded position.
#define DAQmx_AI_Bridge_InitialVoltage                                   0x17ED // Specifies in volts the output voltage of the bridge in the unloaded condition. NI-DAQmx subtracts this value from any measurements before applying scaling equations.
#define DAQmx_AI_Bridge_ShuntCal_Enable                                  0x0094 // Specifies whether to enable a shunt calibration switch. Use Shunt Cal Select to select the switch(es) to enable.
#define DAQmx_AI_Bridge_ShuntCal_Select                                  0x21D5 // Specifies which shunt calibration switch(es) to enable.  Use Shunt Cal Enable to enable the switch(es) you specify with this property.
#define DAQmx_AI_Bridge_ShuntCal_GainAdjust                              0x193F // Specifies the result of a shunt calibration. NI-DAQmx multiplies data read from the channel by the value of this property. This value should be close to 1.0.
#define DAQmx_AI_Bridge_Balance_CoarsePot                                0x17F1 // Specifies by how much to compensate for offset in the signal. This value can be between 0 and 127.
#define DAQmx_AI_Bridge_Balance_FinePot                                  0x18F4 // Specifies by how much to compensate for offset in the signal. This value can be between 0 and 4095.
#define DAQmx_AI_CurrentShunt_Loc                                        0x17F2 // Specifies the shunt resistor location for current measurements.
#define DAQmx_AI_CurrentShunt_Resistance                                 0x17F3 // Specifies in ohms the external shunt resistance for current measurements.
#define DAQmx_AI_Excit_Src                                               0x17F4 // Specifies the source of excitation.
#define DAQmx_AI_Excit_Val                                               0x17F5 // Specifies the amount of excitation that the sensor requires. If Voltage or Current is  DAQmx_Val_Voltage, this value is in volts. If Voltage or Current is  DAQmx_Val_Current, this value is in amperes.
#define DAQmx_AI_Excit_UseForScaling                                     0x17FC // Specifies if NI-DAQmx divides the measurement by the excitation. You should typically set this property to TRUE for ratiometric transducers. If you set this property to TRUE, set Maximum Value and Minimum Value to reflect the scaling.
#define DAQmx_AI_Excit_UseMultiplexed                                    0x2180 // Specifies if the SCXI-1122 multiplexes the excitation to the upper half of the channels as it advances through the scan list.
#define DAQmx_AI_Excit_ActualVal                                         0x1883 // Specifies the actual amount of excitation supplied by an internal excitation source.  If you read an internal excitation source more precisely with an external device, set this property to the value you read.  NI-DAQmx ignores this value for external excitation.
#define DAQmx_AI_Excit_DCorAC                                            0x17FB // Specifies if the excitation supply is DC or AC.
#define DAQmx_AI_Excit_VoltageOrCurrent                                  0x17F6 // Specifies if the channel uses current or voltage excitation.
#define DAQmx_AI_ACExcit_Freq                                            0x0101 // Specifies the AC excitation frequency in Hertz.
#define DAQmx_AI_ACExcit_SyncEnable                                      0x0102 // Specifies whether to synchronize the AC excitation source of the channel to that of another channel. Synchronize the excitation sources of multiple channels to use multichannel sensors. Set this property to FALSE for the master channel and to TRUE for the slave channels.
#define DAQmx_AI_ACExcit_WireMode                                        0x18CD // Specifies the number of leads on the LVDT or RVDT. Some sensors require you to tie leads together to create a four- or five- wire sensor. Refer to the sensor documentation for more information.
#define DAQmx_AI_Atten                                                   0x1801 // Specifies the amount of attenuation to use.
#define DAQmx_AI_Lowpass_Enable                                          0x1802 // Specifies whether to enable the lowpass filter of the channel.
#define DAQmx_AI_Lowpass_CutoffFreq                                      0x1803 // Specifies the frequency in Hertz that corresponds to the -3dB cutoff of the filter.
#define DAQmx_AI_Lowpass_SwitchCap_ClkSrc                                0x1884 // Specifies the source of the filter clock. If you need a higher resolution for the filter, you can supply an external clock to increase the resolution. Refer to the SCXI-1141/1142/1143 User Manual for more information.
#define DAQmx_AI_Lowpass_SwitchCap_ExtClkFreq                            0x1885 // Specifies the frequency of the external clock when you set Clock Source to DAQmx_Val_External.  NI-DAQmx uses this frequency to set the pre- and post- filters on the SCXI-1141, SCXI-1142, and SCXI-1143. On those devices, NI-DAQmx determines the filter cutoff by using the equation f/(100*n), where f is the external frequency, and n is the external clock divisor. Refer to the SCXI-1141/1142/1143 User Manual for more...
#define DAQmx_AI_Lowpass_SwitchCap_ExtClkDiv                             0x1886 // Specifies the divisor for the external clock when you set Clock Source to DAQmx_Val_External. On the SCXI-1141, SCXI-1142, and SCXI-1143, NI-DAQmx determines the filter cutoff by using the equation f/(100*n), where f is the external frequency, and n is the external clock divisor. Refer to the SCXI-1141/1142/1143 User Manual for more information.
#define DAQmx_AI_Lowpass_SwitchCap_OutClkDiv                             0x1887 // Specifies the divisor for the output clock.  NI-DAQmx uses the cutoff frequency to determine the output clock frequency. Refer to the SCXI-1141/1142/1143 User Manual for more information.
#define DAQmx_AI_ResolutionUnits                                         0x1764 // Indicates the units of Resolution Value.
#define DAQmx_AI_Resolution                                              0x1765 // Indicates the resolution of the analog-to-digital converter of the channel. This value is in the units you specify with Resolution Units.
#define DAQmx_AI_RawSampSize                                             0x22DA // Indicates in bits the size of a raw sample from the device.
#define DAQmx_AI_RawSampJustification                                    0x0050 // Indicates the justification of a raw sample from the device.
#define DAQmx_AI_Dither_Enable                                           0x0068 // Specifies whether to enable dithering.  Dithering adds Gaussian noise to the input signal. You can use dithering to achieve higher resolution measurements by over sampling the input signal and averaging the results.
#define DAQmx_AI_ChanCal_HasValidCalInfo                                 0x2297 // Indicates if the channel has calibration information.
#define DAQmx_AI_ChanCal_EnableCal                                       0x2298 // Specifies whether to enable the channel calibration associated with the channel.
#define DAQmx_AI_ChanCal_ApplyCalIfExp                                   0x2299 // Specifies whether to apply the channel calibration to the channel after the expiration date has passed.
#define DAQmx_AI_ChanCal_ScaleType                                       0x229C // Specifies the method or equation form that the calibration scale uses.
#define DAQmx_AI_ChanCal_Table_PreScaledVals                             0x229D // Specifies the reference values collected when calibrating the channel.
#define DAQmx_AI_ChanCal_Table_ScaledVals                                0x229E // Specifies the acquired values collected when calibrating the channel.
#define DAQmx_AI_ChanCal_Poly_ForwardCoeff                               0x229F // Specifies the forward polynomial values used for calibrating the channel.
#define DAQmx_AI_ChanCal_Poly_ReverseCoeff                               0x22A0 // Specifies the reverse polynomial values used for calibrating the channel.
#define DAQmx_AI_ChanCal_OperatorName                                    0x22A3 // Specifies the name of the operator who performed the channel calibration.
#define DAQmx_AI_ChanCal_Desc                                            0x22A4 // Specifies the description entered for the calibration of the channel.
#define DAQmx_AI_ChanCal_Verif_RefVals                                   0x22A1 // Specifies the reference values collected when verifying the calibration. NI-DAQmx stores these values as a record of calibration accuracy and does not use them in the scaling process.
#define DAQmx_AI_ChanCal_Verif_AcqVals                                   0x22A2 // Specifies the acquired values collected when verifying the calibration. NI-DAQmx stores these values as a record of calibration accuracy and does not use them in the scaling process.
#define DAQmx_AI_Rng_High                                                0x1815 // Specifies the upper limit of the input range of the device. This value is in the native units of the device. On E Series devices, for example, the native units is volts.
#define DAQmx_AI_Rng_Low                                                 0x1816 // Specifies the lower limit of the input range of the device. This value is in the native units of the device. On E Series devices, for example, the native units is volts.
#define DAQmx_AI_Gain                                                    0x1818 // Specifies a gain factor to apply to the channel.
#define DAQmx_AI_SampAndHold_Enable                                      0x181A // Specifies whether to enable the sample and hold circuitry of the device. When you disable sample and hold circuitry, a small voltage offset might be introduced into the signal.  You can eliminate this offset by using Auto Zero Mode to perform an auto zero on the channel.
#define DAQmx_AI_AutoZeroMode                                            0x1760 // Specifies when to measure ground. NI-DAQmx subtracts the measured ground voltage from every sample.
#define DAQmx_AI_DataXferMech                                            0x1821 // Specifies the data transfer mode for the device.
#define DAQmx_AI_DataXferReqCond                                         0x188B // Specifies under what condition to transfer data from the onboard memory of the device to the buffer.
#define DAQmx_AI_DataXferCustomThreshold                                 0x230C // Specifies the number of samples that must be in the FIFO to transfer data from the device if Data Transfer Request Condition is DAQmx_Val_OnbrdMemCustomThreshold.
#define DAQmx_AI_MemMapEnable                                            0x188C // Specifies for NI-DAQmx to map hardware registers to the memory space of the customer process, if possible. Mapping to the memory space of the customer process increases performance. However, memory mapping can adversely affect the operation of the device and possibly result in a system crash if software in the process unintentionally accesses the mapped registers.
#define DAQmx_AI_RawDataCompressionType                                  0x22D8 // Specifies the type of compression to apply to raw samples returned from the device.
#define DAQmx_AI_LossyLSBRemoval_CompressedSampSize                      0x22D9 // Specifies the number of bits to return in a raw samples when Raw Data Compression Type is set to DAQmx_Val_LossyLSBRemoval.
#define DAQmx_AI_DevScalingCoeff                                         0x1930 // Indicates the coefficients of a polynomial equation that NI-DAQmx uses to scale values from the native format of the device to volts. Each element of the array corresponds to a term of the equation. For example, if index two of the array is 4, the third term of the equation is 4x^2. Scaling coefficients do not account for any custom scales or sensors contained by the channel.
#define DAQmx_AI_EnhancedAliasRejectionEnable                            0x2294 // Specifies whether to enable enhanced alias rejection. By default, enhanced alias rejection is enabled on supported devices. Leave this property set to the default value for most applications.
#define DAQmx_AO_Max                                                     0x1186 // Specifies the maximum value you expect to generate. The value is in the units you specify with a units property. If you try to write a value larger than the maximum value, NI-DAQmx generates an error. NI-DAQmx might coerce this value to a smaller value if other task settings restrict the device from generating the desired maximum.
#define DAQmx_AO_Min                                                     0x1187 // Specifies the minimum value you expect to generate. The value is in the units you specify with a units property. If you try to write a value smaller than the minimum value, NI-DAQmx generates an error. NI-DAQmx might coerce this value to a larger value if other task settings restrict the device from generating the desired minimum.
#define DAQmx_AO_CustomScaleName                                         0x1188 // Specifies the name of a custom scale for the channel.
#define DAQmx_AO_OutputType                                              0x1108 // Indicates whether the channel generates voltage or current.
#define DAQmx_AO_Voltage_Units                                           0x1184 // Specifies in what units to generate voltage on the channel. Write data to the channel in the units you select.
#define DAQmx_AO_Current_Units                                           0x1109 // Specifies in what units to generate current on the channel. Write data to the channel is in the units you select.
#define DAQmx_AO_OutputImpedance                                         0x1490 // Specifies in ohms the impedance of the analog output stage of the device.
#define DAQmx_AO_LoadImpedance                                           0x0121 // Specifies in ohms the load impedance connected to the analog output channel.
#define DAQmx_AO_IdleOutputBehavior                                      0x2240 // Specifies the state of the channel when no generation is in progress.
#define DAQmx_AO_TermCfg                                                 0x188E // Specifies the terminal configuration of the channel.
#define DAQmx_AO_ResolutionUnits                                         0x182B // Specifies the units of Resolution Value.
#define DAQmx_AO_Resolution                                              0x182C // Indicates the resolution of the digital-to-analog converter of the channel. This value is in the units you specify with Resolution Units.
#define DAQmx_AO_DAC_Rng_High                                            0x182E // Specifies the upper limit of the output range of the device. This value is in the native units of the device. On E Series devices, for example, the native units is volts.
#define DAQmx_AO_DAC_Rng_Low                                             0x182D // Specifies the lower limit of the output range of the device. This value is in the native units of the device. On E Series devices, for example, the native units is volts.
#define DAQmx_AO_DAC_Ref_ConnToGnd                                       0x0130 // Specifies whether to ground the internal DAC reference. Grounding the internal DAC reference has the effect of grounding all analog output channels and stopping waveform generation across all analog output channels regardless of whether the channels belong to the current task. You can ground the internal DAC reference only when Source is DAQmx_Val_Internal and Allow Connecting DAC Reference to Ground at Runtime is...
#define DAQmx_AO_DAC_Ref_AllowConnToGnd                                  0x1830 // Specifies whether to allow grounding the internal DAC reference at run time. You must set this property to TRUE and set Source to DAQmx_Val_Internal before you can set Connect DAC Reference to Ground to TRUE.
#define DAQmx_AO_DAC_Ref_Src                                             0x0132 // Specifies the source of the DAC reference voltage. The value of this voltage source determines the full-scale value of the DAC.
#define DAQmx_AO_DAC_Ref_ExtSrc                                          0x2252 // Specifies the source of the DAC reference voltage if Source is DAQmx_Val_External.
#define DAQmx_AO_DAC_Ref_Val                                             0x1832 // Specifies in volts the value of the DAC reference voltage. This voltage determines the full-scale range of the DAC. Smaller reference voltages result in smaller ranges, but increased resolution.
#define DAQmx_AO_DAC_Offset_Src                                          0x2253 // Specifies the source of the DAC offset voltage. The value of this voltage source determines the full-scale value of the DAC.
#define DAQmx_AO_DAC_Offset_ExtSrc                                       0x2254 // Specifies the source of the DAC offset voltage if Source is DAQmx_Val_External.
#define DAQmx_AO_DAC_Offset_Val                                          0x2255 // Specifies in volts the value of the DAC offset voltage. To achieve best accuracy, the DAC offset value should be hand calibrated.
#define DAQmx_AO_ReglitchEnable                                          0x0133 // Specifies whether to enable reglitching.  The output of a DAC normally glitches whenever the DAC is updated with a new value. The amount of glitching differs from code to code and is generally largest at major code transitions.  Reglitching generates uniform glitch energy at each code transition and provides for more uniform glitches.  Uniform glitch energy makes it easier to filter out the noise introduced from g...
#define DAQmx_AO_Gain                                                    0x0118 // Specifies in decibels the gain factor to apply to the channel.
#define DAQmx_AO_UseOnlyOnBrdMem                                         0x183A // Specifies whether to write samples directly to the onboard memory of the device, bypassing the memory buffer. Generally, you cannot update onboard memory after you start the task. Onboard memory includes data FIFOs.
#define DAQmx_AO_DataXferMech                                            0x0134 // Specifies the data transfer mode for the device.
#define DAQmx_AO_DataXferReqCond                                         0x183C // Specifies under what condition to transfer data from the buffer to the onboard memory of the device.
#define DAQmx_AO_MemMapEnable                                            0x188F // Specifies if NI-DAQmx maps hardware registers to the memory space of the customer process, if possible. Mapping to the memory space of the customer process increases performance. However, memory mapping can adversely affect the operation of the device and possibly result in a system crash if software in the process unintentionally accesses the mapped registers.
#define DAQmx_AO_DevScalingCoeff                                         0x1931 // Indicates the coefficients of a linear equation that NI-DAQmx uses to scale values from a voltage to the native format of the device.  Each element of the array corresponds to a term of the equation. For example, if index two of the array is 4, the third term of the equation is 4x^2.  Scaling coefficients do not account for any custom scales that may be applied to the channel.
#define DAQmx_AO_EnhancedImageRejectionEnable                            0x2241 // Specifies whether to enable the DAC interpolation filter. Disable the interpolation filter to improve DAC signal-to-noise ratio at the expense of degraded image rejection.
#define DAQmx_DI_InvertLines                                             0x0793 // Specifies whether to invert the lines in the channel. If you set this property to TRUE, the lines are at high logic when off and at low logic when on.
#define DAQmx_DI_NumLines                                                0x2178 // Indicates the number of digital lines in the channel.
#define DAQmx_DI_DigFltr_Enable                                          0x21D6 // Specifies whether to enable the digital filter for the line(s) or port(s). You can enable the filter on a line-by-line basis. You do not have to enable the filter for all lines in a channel.
#define DAQmx_DI_DigFltr_MinPulseWidth                                   0x21D7 // Specifies in seconds the minimum pulse width the filter recognizes as a valid high or low state transition.
#define DAQmx_DI_Tristate                                                0x1890 // Specifies whether to tristate the lines in the channel. If you set this property to TRUE, NI-DAQmx tristates the lines in the channel. If you set this property to FALSE, NI-DAQmx does not modify the configuration of the lines even if the lines were previously tristated. Set this property to FALSE to read lines in other tasks or to read output-only lines.
#define DAQmx_DI_DataXferMech                                            0x2263 // Specifies the data transfer mode for the device.
#define DAQmx_DI_DataXferReqCond                                         0x2264 // Specifies under what condition to transfer data from the onboard memory of the device to the buffer.
#define DAQmx_DO_OutputDriveType                                         0x1137 // Specifies the drive type for digital output channels.
#define DAQmx_DO_InvertLines                                             0x1133 // Specifies whether to invert the lines in the channel. If you set this property to TRUE, the lines are at high logic when off and at low logic when on.
#define DAQmx_DO_NumLines                                                0x2179 // Indicates the number of digital lines in the channel.
#define DAQmx_DO_Tristate                                                0x18F3 // Specifies whether to stop driving the channel and set it to a high-impedance state. You must commit the task for this setting to take effect.
#define DAQmx_DO_UseOnlyOnBrdMem                                         0x2265 // Specifies whether to write samples directly to the onboard memory of the device, bypassing the memory buffer. Generally, you cannot update onboard memory after you start the task. Onboard memory includes data FIFOs.
#define DAQmx_DO_DataXferMech                                            0x2266 // Specifies the data transfer mode for the device.
#define DAQmx_DO_DataXferReqCond                                         0x2267 // Specifies under what condition to transfer data from the buffer to the onboard memory of the device.
#define DAQmx_CI_Max                                                     0x189C // Specifies the maximum value you expect to measure. This value is in the units you specify with a units property. When you query this property, it returns the coerced maximum value that the hardware can measure with the current settings.
#define DAQmx_CI_Min                                                     0x189D // Specifies the minimum value you expect to measure. This value is in the units you specify with a units property. When you query this property, it returns the coerced minimum value that the hardware can measure with the current settings.
#define DAQmx_CI_CustomScaleName                                         0x189E // Specifies the name of a custom scale for the channel.
#define DAQmx_CI_MeasType                                                0x18A0 // Indicates the measurement to take with the channel.
#define DAQmx_CI_Freq_Units                                              0x18A1 // Specifies the units to use to return frequency measurements.
#define DAQmx_CI_Freq_Term                                               0x18A2 // Specifies the input terminal of the signal to measure.
#define DAQmx_CI_Freq_StartingEdge                                       0x0799 // Specifies between which edges to measure the frequency of the signal.
#define DAQmx_CI_Freq_MeasMeth                                           0x0144 // Specifies the method to use to measure the frequency of the signal.
#define DAQmx_CI_Freq_MeasTime                                           0x0145 // Specifies in seconds the length of time to measure the frequency of the signal if Method is DAQmx_Val_HighFreq2Ctr. Measurement accuracy increases with increased measurement time and with increased signal frequency. If you measure a high-frequency signal for too long, however, the count register could roll over, which results in an incorrect measurement.
#define DAQmx_CI_Freq_Div                                                0x0147 // Specifies the value by which to divide the input signal if  Method is DAQmx_Val_LargeRng2Ctr. The larger the divisor, the more accurate the measurement. However, too large a value could cause the count register to roll over, which results in an incorrect measurement.
#define DAQmx_CI_Freq_DigFltr_Enable                                     0x21E7 // Specifies whether to apply the pulse width filter to the signal.
#define DAQmx_CI_Freq_DigFltr_MinPulseWidth                              0x21E8 // Specifies in seconds the minimum pulse width the filter recognizes.
#define DAQmx_CI_Freq_DigFltr_TimebaseSrc                                0x21E9 // Specifies the input terminal of the signal to use as the timebase of the pulse width filter.
#define DAQmx_CI_Freq_DigFltr_TimebaseRate                               0x21EA // Specifies in hertz the rate of the pulse width filter timebase. NI-DAQmx uses this value to compute settings for the filter.
#define DAQmx_CI_Freq_DigSync_Enable                                     0x21EB // Specifies whether to synchronize recognition of transitions in the signal to the internal timebase of the device.
#define DAQmx_CI_Period_Units                                            0x18A3 // Specifies the unit to use to return period measurements.
#define DAQmx_CI_Period_Term                                             0x18A4 // Specifies the input terminal of the signal to measure.
#define DAQmx_CI_Period_StartingEdge                                     0x0852 // Specifies between which edges to measure the period of the signal.
#define DAQmx_CI_Period_MeasMeth                                         0x192C // Specifies the method to use to measure the period of the signal.
#define DAQmx_CI_Period_MeasTime                                         0x192D // Specifies in seconds the length of time to measure the period of the signal if Method is DAQmx_Val_HighFreq2Ctr. Measurement accuracy increases with increased measurement time and with increased signal frequency. If you measure a high-frequency signal for too long, however, the count register could roll over, which results in an incorrect measurement.
#define DAQmx_CI_Period_Div                                              0x192E // Specifies the value by which to divide the input signal if Method is DAQmx_Val_LargeRng2Ctr. The larger the divisor, the more accurate the measurement. However, too large a value could cause the count register to roll over, which results in an incorrect measurement.
#define DAQmx_CI_Period_DigFltr_Enable                                   0x21EC // Specifies whether to apply the pulse width filter to the signal.
#define DAQmx_CI_Period_DigFltr_MinPulseWidth                            0x21ED // Specifies in seconds the minimum pulse width the filter recognizes.
#define DAQmx_CI_Period_DigFltr_TimebaseSrc                              0x21EE // Specifies the input terminal of the signal to use as the timebase of the pulse width filter.
#define DAQmx_CI_Period_DigFltr_TimebaseRate                             0x21EF // Specifies in hertz the rate of the pulse width filter timebase. NI-DAQmx uses this value to compute settings for the filter.
#define DAQmx_CI_Period_DigSync_Enable                                   0x21F0 // Specifies whether to synchronize recognition of transitions in the signal to the internal timebase of the device.
#define DAQmx_CI_CountEdges_Term                                         0x18C7 // Specifies the input terminal of the signal to measure.
#define DAQmx_CI_CountEdges_Dir                                          0x0696 // Specifies whether to increment or decrement the counter on each edge.
#define DAQmx_CI_CountEdges_DirTerm                                      0x21E1 // Specifies the source terminal of the digital signal that controls the count direction if Direction is DAQmx_Val_ExtControlled.
#define DAQmx_CI_CountEdges_CountDir_DigFltr_Enable                      0x21F1 // Specifies whether to apply the pulse width filter to the signal.
#define DAQmx_CI_CountEdges_CountDir_DigFltr_MinPulseWidth               0x21F2 // Specifies in seconds the minimum pulse width the filter recognizes.
#define DAQmx_CI_CountEdges_CountDir_DigFltr_TimebaseSrc                 0x21F3 // Specifies the input terminal of the signal to use as the timebase of the pulse width filter.
#define DAQmx_CI_CountEdges_CountDir_DigFltr_TimebaseRate                0x21F4 // Specifies in hertz the rate of the pulse width filter timebase. NI-DAQmx uses this value to compute settings for the filter.
#define DAQmx_CI_CountEdges_CountDir_DigSync_Enable                      0x21F5 // Specifies whether to synchronize recognition of transitions in the signal to the internal timebase of the device.
#define DAQmx_CI_CountEdges_InitialCnt                                   0x0698 // Specifies the starting value from which to count.
#define DAQmx_CI_CountEdges_ActiveEdge                                   0x0697 // Specifies on which edges to increment or decrement the counter.
#define DAQmx_CI_CountEdges_DigFltr_Enable                               0x21F6 // Specifies whether to apply the pulse width filter to the signal.
#define DAQmx_CI_CountEdges_DigFltr_MinPulseWidth                        0x21F7 // Specifies in seconds the minimum pulse width the filter recognizes.
#define DAQmx_CI_CountEdges_DigFltr_TimebaseSrc                          0x21F8 // Specifies the input terminal of the signal to use as the timebase of the pulse width filter.
#define DAQmx_CI_CountEdges_DigFltr_TimebaseRate                         0x21F9 // Specifies in hertz the rate of the pulse width filter timebase. NI-DAQmx uses this value to compute settings for the filter.
#define DAQmx_CI_CountEdges_DigSync_Enable                               0x21FA // Specifies whether to synchronize recognition of transitions in the signal to the internal timebase of the device.
#define DAQmx_CI_AngEncoder_Units                                        0x18A6 // Specifies the units to use to return angular position measurements from the channel.
#define DAQmx_CI_AngEncoder_PulsesPerRev                                 0x0875 // Specifies the number of pulses the encoder generates per revolution. This value is the number of pulses on either signal A or signal B, not the total number of pulses on both signal A and signal B.
#define DAQmx_CI_AngEncoder_InitialAngle                                 0x0881 // Specifies the starting angle of the encoder. This value is in the units you specify with Units.
#define DAQmx_CI_LinEncoder_Units                                        0x18A9 // Specifies the units to use to return linear encoder measurements from the channel.
#define DAQmx_CI_LinEncoder_DistPerPulse                                 0x0911 // Specifies the distance to measure for each pulse the encoder generates on signal A or signal B. This value is in the units you specify with Units.
#define DAQmx_CI_LinEncoder_InitialPos                                   0x0915 // Specifies the position of the encoder when the measurement begins. This value is in the units you specify with Units.
#define DAQmx_CI_Encoder_DecodingType                                    0x21E6 // Specifies how to count and interpret the pulses the encoder generates on signal A and signal B. DAQmx_Val_X1, DAQmx_Val_X2, and DAQmx_Val_X4 are valid for quadrature encoders only. DAQmx_Val_TwoPulseCounting is valid for two-pulse encoders only.
#define DAQmx_CI_Encoder_AInputTerm                                      0x219D // Specifies the terminal to which signal A is connected.
#define DAQmx_CI_Encoder_AInput_DigFltr_Enable                           0x21FB // Specifies whether to apply the pulse width filter to the signal.
#define DAQmx_CI_Encoder_AInput_DigFltr_MinPulseWidth                    0x21FC // Specifies in seconds the minimum pulse width the filter recognizes.
#define DAQmx_CI_Encoder_AInput_DigFltr_TimebaseSrc                      0x21FD // Specifies the input terminal of the signal to use as the timebase of the pulse width filter.
#define DAQmx_CI_Encoder_AInput_DigFltr_TimebaseRate                     0x21FE // Specifies in hertz the rate of the pulse width filter timebase. NI-DAQmx uses this value to compute settings for the filter.
#define DAQmx_CI_Encoder_AInput_DigSync_Enable                           0x21FF // Specifies whether to synchronize recognition of transitions in the signal to the internal timebase of the device.
#define DAQmx_CI_Encoder_BInputTerm                                      0x219E // Specifies the terminal to which signal B is connected.
#define DAQmx_CI_Encoder_BInput_DigFltr_Enable                           0x2200 // Specifies whether to apply the pulse width filter to the signal.
#define DAQmx_CI_Encoder_BInput_DigFltr_MinPulseWidth                    0x2201 // Specifies in seconds the minimum pulse width the filter recognizes.
#define DAQmx_CI_Encoder_BInput_DigFltr_TimebaseSrc                      0x2202 // Specifies the input terminal of the signal to use as the timebase of the pulse width filter.
#define DAQmx_CI_Encoder_BInput_DigFltr_TimebaseRate                     0x2203 // Specifies in hertz the rate of the pulse width filter timebase. NI-DAQmx uses this value to compute settings for the filter.
#define DAQmx_CI_Encoder_BInput_DigSync_Enable                           0x2204 // Specifies whether to synchronize recognition of transitions in the signal to the internal timebase of the device.
#define DAQmx_CI_Encoder_ZInputTerm                                      0x219F // Specifies the terminal to which signal Z is connected.
#define DAQmx_CI_Encoder_ZInput_DigFltr_Enable                           0x2205 // Specifies whether to apply the pulse width filter to the signal.
#define DAQmx_CI_Encoder_ZInput_DigFltr_MinPulseWidth                    0x2206 // Specifies in seconds the minimum pulse width the filter recognizes.
#define DAQmx_CI_Encoder_ZInput_DigFltr_TimebaseSrc                      0x2207 // Specifies the input terminal of the signal to use as the timebase of the pulse width filter.
#define DAQmx_CI_Encoder_ZInput_DigFltr_TimebaseRate                     0x2208 // Specifies in hertz the rate of the pulse width filter timebase. NI-DAQmx uses this value to compute settings for the filter.
#define DAQmx_CI_Encoder_ZInput_DigSync_Enable                           0x2209 // Specifies whether to synchronize recognition of transitions in the signal to the internal timebase of the device.
#define DAQmx_CI_Encoder_ZIndexEnable                                    0x0890 // Specifies whether to use Z indexing for the channel.
#define DAQmx_CI_Encoder_ZIndexVal                                       0x0888 // Specifies the value to which to reset the measurement when signal Z is high and signal A and signal B are at the states you specify with Z Index Phase. Specify this value in the units of the measurement.
#define DAQmx_CI_Encoder_ZIndexPhase                                     0x0889 // Specifies the states at which signal A and signal B must be while signal Z is high for NI-DAQmx to reset the measurement. If signal Z is never high while signal A and signal B are high, for example, you must choose a phase other than DAQmx_Val_AHighBHigh.
#define DAQmx_CI_PulseWidth_Units                                        0x0823 // Specifies the units to use to return pulse width measurements.
#define DAQmx_CI_PulseWidth_Term                                         0x18AA // Specifies the input terminal of the signal to measure.
#define DAQmx_CI_PulseWidth_StartingEdge                                 0x0825 // Specifies on which edge of the input signal to begin each pulse width measurement.
#define DAQmx_CI_PulseWidth_DigFltr_Enable                               0x220A // Specifies whether to apply the pulse width filter to the signal.
#define DAQmx_CI_PulseWidth_DigFltr_MinPulseWidth                        0x220B // Specifies in seconds the minimum pulse width the filter recognizes.
#define DAQmx_CI_PulseWidth_DigFltr_TimebaseSrc                          0x220C // Specifies the input terminal of the signal to use as the timebase of the pulse width filter.
#define DAQmx_CI_PulseWidth_DigFltr_TimebaseRate                         0x220D // Specifies in hertz the rate of the pulse width filter timebase. NI-DAQmx uses this value to compute settings for the filter.
#define DAQmx_CI_PulseWidth_DigSync_Enable                               0x220E // Specifies whether to synchronize recognition of transitions in the signal to the internal timebase of the device.
#define DAQmx_CI_TwoEdgeSep_Units                                        0x18AC // Specifies the units to use to return two-edge separation measurements from the channel.
#define DAQmx_CI_TwoEdgeSep_FirstTerm                                    0x18AD // Specifies the source terminal of the digital signal that starts each measurement.
#define DAQmx_CI_TwoEdgeSep_FirstEdge                                    0x0833 // Specifies on which edge of the first signal to start each measurement.
#define DAQmx_CI_TwoEdgeSep_First_DigFltr_Enable                         0x220F // Specifies whether to apply the pulse width filter to the signal.
#define DAQmx_CI_TwoEdgeSep_First_DigFltr_MinPulseWidth                  0x2210 // Specifies in seconds the minimum pulse width the filter recognizes.
#define DAQmx_CI_TwoEdgeSep_First_DigFltr_TimebaseSrc                    0x2211 // Specifies the input terminal of the signal to use as the timebase of the pulse width filter.
#define DAQmx_CI_TwoEdgeSep_First_DigFltr_TimebaseRate                   0x2212 // Specifies in hertz the rate of the pulse width filter timebase. NI-DAQmx uses this value to compute settings for the filter.
#define DAQmx_CI_TwoEdgeSep_First_DigSync_Enable                         0x2213 // Specifies whether to synchronize recognition of transitions in the signal to the internal timebase of the device.
#define DAQmx_CI_TwoEdgeSep_SecondTerm                                   0x18AE // Specifies the source terminal of the digital signal that stops each measurement.
#define DAQmx_CI_TwoEdgeSep_SecondEdge                                   0x0834 // Specifies on which edge of the second signal to stop each measurement.
#define DAQmx_CI_TwoEdgeSep_Second_DigFltr_Enable                        0x2214 // Specifies whether to apply the pulse width filter to the signal.
#define DAQmx_CI_TwoEdgeSep_Second_DigFltr_MinPulseWidth                 0x2215 // Specifies in seconds the minimum pulse width the filter recognizes.
#define DAQmx_CI_TwoEdgeSep_Second_DigFltr_TimebaseSrc                   0x2216 // Specifies the input terminal of the signal to use as the timebase of the pulse width filter.
#define DAQmx_CI_TwoEdgeSep_Second_DigFltr_TimebaseRate                  0x2217 // Specifies in hertz the rate of the pulse width filter timebase. NI-DAQmx uses this value to compute settings for the filter.
#define DAQmx_CI_TwoEdgeSep_Second_DigSync_Enable                        0x2218 // Specifies whether to synchronize recognition of transitions in the signal to the internal timebase of the device.
#define DAQmx_CI_SemiPeriod_Units                                        0x18AF // Specifies the units to use to return semi-period measurements.
#define DAQmx_CI_SemiPeriod_Term                                         0x18B0 // Specifies the input terminal of the signal to measure.
#define DAQmx_CI_SemiPeriod_StartingEdge                                 0x22FE // Specifies on which edge of the input signal to begin semi-period measurement. Semi-period measurements alternate between high time and low time, starting on this edge.
#define DAQmx_CI_SemiPeriod_DigFltr_Enable                               0x2219 // Specifies whether to apply the pulse width filter to the signal.
#define DAQmx_CI_SemiPeriod_DigFltr_MinPulseWidth                        0x221A // Specifies in seconds the minimum pulse width the filter recognizes.
#define DAQmx_CI_SemiPeriod_DigFltr_TimebaseSrc                          0x221B // Specifies the input terminal of the signal to use as the timebase of the pulse width filter.
#define DAQmx_CI_SemiPeriod_DigFltr_TimebaseRate                         0x221C // Specifies in hertz the rate of the pulse width filter timebase. NI-DAQmx uses this value to compute settings for the filter.
#define DAQmx_CI_SemiPeriod_DigSync_Enable                               0x221D // Specifies whether to synchronize recognition of transitions in the signal to the internal timebase of the device.
#define DAQmx_CI_Timestamp_Units                                         0x22B3 // Specifies the units to use to return timestamp measurements.
#define DAQmx_CI_Timestamp_InitialSeconds                                0x22B4 // Specifies the number of seconds that elapsed since the beginning of the current year. This value is ignored if  Synchronization Method is DAQmx_Val_IRIGB.
#define DAQmx_CI_GPS_SyncMethod                                          0x1092 // Specifies the method to use to synchronize the counter to a GPS receiver.
#define DAQmx_CI_GPS_SyncSrc                                             0x1093 // Specifies the terminal to which the GPS synchronization signal is connected.
#define DAQmx_CI_CtrTimebaseSrc                                          0x0143 // Specifies the terminal of the timebase to use for the counter.
#define DAQmx_CI_CtrTimebaseRate                                         0x18B2 // Specifies in Hertz the frequency of the counter timebase. Specifying the rate of a counter timebase allows you to take measurements in terms of time or frequency rather than in ticks of the timebase. If you use an external timebase and do not specify the rate, you can take measurements only in terms of ticks of the timebase.
#define DAQmx_CI_CtrTimebaseActiveEdge                                   0x0142 // Specifies whether a timebase cycle is from rising edge to rising edge or from falling edge to falling edge.
#define DAQmx_CI_CtrTimebase_DigFltr_Enable                              0x2271 // Specifies whether to apply the pulse width filter to the signal.
#define DAQmx_CI_CtrTimebase_DigFltr_MinPulseWidth                       0x2272 // Specifies in seconds the minimum pulse width the filter recognizes.
#define DAQmx_CI_CtrTimebase_DigFltr_TimebaseSrc                         0x2273 // Specifies the input terminal of the signal to use as the timebase of the pulse width filter.
#define DAQmx_CI_CtrTimebase_DigFltr_TimebaseRate                        0x2274 // Specifies in hertz the rate of the pulse width filter timebase. NI-DAQmx uses this value to compute settings for the filter.
#define DAQmx_CI_CtrTimebase_DigSync_Enable                              0x2275 // Specifies whether to synchronize recognition of transitions in the signal to the internal timebase of the device.
#define DAQmx_CI_Count                                                   0x0148 // Indicates the current value of the count register.
#define DAQmx_CI_OutputState                                             0x0149 // Indicates the current state of the out terminal of the counter.
#define DAQmx_CI_TCReached                                               0x0150 // Indicates whether the counter rolled over. When you query this property, NI-DAQmx resets it to FALSE.
#define DAQmx_CI_CtrTimebaseMasterTimebaseDiv                            0x18B3 // Specifies the divisor for an external counter timebase. You can divide the counter timebase in order to measure slower signals without causing the count register to roll over.
#define DAQmx_CI_DataXferMech                                            0x0200 // Specifies the data transfer mode for the channel.
#define DAQmx_CI_NumPossiblyInvalidSamps                                 0x193C // Indicates the number of samples that the device might have overwritten before it could transfer them to the buffer.
#define DAQmx_CI_DupCountPrevent                                         0x21AC // Specifies whether to enable duplicate count prevention for the channel.
#define DAQmx_CI_Prescaler                                               0x2239 // Specifies the divisor to apply to the signal you connect to the counter source terminal. Scaled data that you read takes this setting into account. You should use a prescaler only when you connect an external signal to the counter source terminal and when that signal has a higher frequency than the fastest onboard timebase.
#define DAQmx_CO_OutputType                                              0x18B5 // Indicates how to define pulses generated on the channel.
#define DAQmx_CO_Pulse_IdleState                                         0x1170 // Specifies the resting state of the output terminal.
#define DAQmx_CO_Pulse_Term                                              0x18E1 // Specifies on which terminal to generate pulses.
#define DAQmx_CO_Pulse_Time_Units                                        0x18D6 // Specifies the units in which to define high and low pulse time.
#define DAQmx_CO_Pulse_HighTime                                          0x18BA // Specifies the amount of time that the pulse is at a high voltage. This value is in the units you specify with Units or when you create the channel.
#define DAQmx_CO_Pulse_LowTime                                           0x18BB // Specifies the amount of time that the pulse is at a low voltage. This value is in the units you specify with Units or when you create the channel.
#define DAQmx_CO_Pulse_Time_InitialDelay                                 0x18BC // Specifies in seconds the amount of time to wait before generating the first pulse.
#define DAQmx_CO_Pulse_DutyCyc                                           0x1176 // Specifies the duty cycle of the pulses. The duty cycle of a signal is the width of the pulse divided by period. NI-DAQmx uses this ratio and the pulse frequency to determine the width of the pulses and the delay between pulses.
#define DAQmx_CO_Pulse_Freq_Units                                        0x18D5 // Specifies the units in which to define pulse frequency.
#define DAQmx_CO_Pulse_Freq                                              0x1178 // Specifies the frequency of the pulses to generate. This value is in the units you specify with Units or when you create the channel.
#define DAQmx_CO_Pulse_Freq_InitialDelay                                 0x0299 // Specifies in seconds the amount of time to wait before generating the first pulse.
#define DAQmx_CO_Pulse_HighTicks                                         0x1169 // Specifies the number of ticks the pulse is high.
#define DAQmx_CO_Pulse_LowTicks                                          0x1171 // Specifies the number of ticks the pulse is low.
#define DAQmx_CO_Pulse_Ticks_InitialDelay                                0x0298 // Specifies the number of ticks to wait before generating the first pulse.
#define DAQmx_CO_CtrTimebaseSrc                                          0x0339 // Specifies the terminal of the timebase to use for the counter. Typically, NI-DAQmx uses one of the internal counter timebases when generating pulses. Use this property to specify an external timebase and produce custom pulse widths that are not possible using the internal timebases.
#define DAQmx_CO_CtrTimebaseRate                                         0x18C2 // Specifies in Hertz the frequency of the counter timebase. Specifying the rate of a counter timebase allows you to define output pulses in seconds rather than in ticks of the timebase. If you use an external timebase and do not specify the rate, you can define output pulses only in ticks of the timebase.
#define DAQmx_CO_CtrTimebaseActiveEdge                                   0x0341 // Specifies whether a timebase cycle is from rising edge to rising edge or from falling edge to falling edge.
#define DAQmx_CO_CtrTimebase_DigFltr_Enable                              0x2276 // Specifies whether to apply the pulse width filter to the signal.
#define DAQmx_CO_CtrTimebase_DigFltr_MinPulseWidth                       0x2277 // Specifies in seconds the minimum pulse width the filter recognizes.
#define DAQmx_CO_CtrTimebase_DigFltr_TimebaseSrc                         0x2278 // Specifies the input terminal of the signal to use as the timebase of the pulse width filter.
#define DAQmx_CO_CtrTimebase_DigFltr_TimebaseRate                        0x2279 // Specifies in hertz the rate of the pulse width filter timebase. NI-DAQmx uses this value to compute settings for the filter.
#define DAQmx_CO_CtrTimebase_DigSync_Enable                              0x227A // Specifies whether to synchronize recognition of transitions in the signal to the internal timebase of the device.
#define DAQmx_CO_Count                                                   0x0293 // Indicates the current value of the count register.
#define DAQmx_CO_OutputState                                             0x0294 // Indicates the current state of the output terminal of the counter.
#define DAQmx_CO_AutoIncrCnt                                             0x0295 // Specifies a number of timebase ticks by which to increment each successive pulse.
#define DAQmx_CO_CtrTimebaseMasterTimebaseDiv                            0x18C3 // Specifies the divisor for an external counter timebase. You can divide the counter timebase in order to generate slower signals without causing the count register to roll over.
#define DAQmx_CO_PulseDone                                               0x190E // Indicates if the task completed pulse generation. Use this value for retriggerable pulse generation when you need to determine if the device generated the current pulse. When you query this property, NI-DAQmx resets it to FALSE.
#define DAQmx_CO_Prescaler                                               0x226D // Specifies the divisor to apply to the signal you connect to the counter source terminal. Pulse generations defined by frequency or time take this setting into account, but pulse generations defined by ticks do not. You should use a prescaler only when you connect an external signal to the counter source terminal and when that signal has a higher frequency than the fastest onboard timebase.
#define DAQmx_CO_RdyForNewVal                                            0x22FF // Indicates whether the counter is ready for new continuous pulse train values.
#define DAQmx_ChanType                                                   0x187F // Indicates the type of the virtual channel.
#define DAQmx_PhysicalChanName                                           0x18F5 // Indicates the name of the physical channel upon which this virtual channel is based.
#define DAQmx_ChanDescr                                                  0x1926 // Specifies a user-defined description for the channel.
#define DAQmx_ChanIsGlobal                                               0x2304 // Indicates whether the channel is a global channel.

//********** Export Signal Attributes **********
#define DAQmx_Exported_AIConvClk_OutputTerm                              0x1687 // Specifies the terminal to which to route the AI Convert Clock.
#define DAQmx_Exported_AIConvClk_Pulse_Polarity                          0x1688 // Indicates the polarity of the exported AI Convert Clock. The polarity is fixed and independent of the active edge of the source of the AI Convert Clock.
#define DAQmx_Exported_10MHzRefClk_OutputTerm                            0x226E // Specifies the terminal to which to route the 10MHz Clock.
#define DAQmx_Exported_20MHzTimebase_OutputTerm                          0x1657 // Specifies the terminal to which to route the 20MHz Timebase.
#define DAQmx_Exported_SampClk_OutputBehavior                            0x186B // Specifies whether the exported Sample Clock issues a pulse at the beginning of a sample or changes to a high state for the duration of the sample.
#define DAQmx_Exported_SampClk_OutputTerm                                0x1663 // Specifies the terminal to which to route the Sample Clock.
#define DAQmx_Exported_SampClk_Pulse_Polarity                            0x1664 // Specifies the polarity of the exported Sample Clock if Output Behavior is DAQmx_Val_Pulse.
#define DAQmx_Exported_SampClkTimebase_OutputTerm                        0x18F9 // Specifies the terminal to which to route the Sample Clock Timebase.
#define DAQmx_Exported_DividedSampClkTimebase_OutputTerm                 0x21A1 // Specifies the terminal to which to route the Divided Sample Clock Timebase.
#define DAQmx_Exported_AdvTrig_OutputTerm                                0x1645 // Specifies the terminal to which to route the Advance Trigger.
#define DAQmx_Exported_AdvTrig_Pulse_Polarity                            0x1646 // Indicates the polarity of the exported Advance Trigger.
#define DAQmx_Exported_AdvTrig_Pulse_WidthUnits                          0x1647 // Specifies the units of Width Value.
#define DAQmx_Exported_AdvTrig_Pulse_Width                               0x1648 // Specifies the width of an exported Advance Trigger pulse. Specify this value in the units you specify with Width Units.
#define DAQmx_Exported_RefTrig_OutputTerm                                0x0590 // Specifies the terminal to which to route the Reference Trigger.
#define DAQmx_Exported_StartTrig_OutputTerm                              0x0584 // Specifies the terminal to which to route the Start Trigger.
#define DAQmx_Exported_AdvCmpltEvent_OutputTerm                          0x1651 // Specifies the terminal to which to route the Advance Complete Event.
#define DAQmx_Exported_AdvCmpltEvent_Delay                               0x1757 // Specifies the output signal delay in periods of the sample clock.
#define DAQmx_Exported_AdvCmpltEvent_Pulse_Polarity                      0x1652 // Specifies the polarity of the exported Advance Complete Event.
#define DAQmx_Exported_AdvCmpltEvent_Pulse_Width                         0x1654 // Specifies the width of the exported Advance Complete Event pulse.
#define DAQmx_Exported_AIHoldCmpltEvent_OutputTerm                       0x18ED // Specifies the terminal to which to route the AI Hold Complete Event.
#define DAQmx_Exported_AIHoldCmpltEvent_PulsePolarity                    0x18EE // Specifies the polarity of an exported AI Hold Complete Event pulse.
#define DAQmx_Exported_ChangeDetectEvent_OutputTerm                      0x2197 // Specifies the terminal to which to route the Change Detection Event.
#define DAQmx_Exported_ChangeDetectEvent_Pulse_Polarity                  0x2303 // Specifies the polarity of an exported Change Detection Event pulse.
#define DAQmx_Exported_CtrOutEvent_OutputTerm                            0x1717 // Specifies the terminal to which to route the Counter Output Event.
#define DAQmx_Exported_CtrOutEvent_OutputBehavior                        0x174F // Specifies whether the exported Counter Output Event pulses or changes from one state to the other when the counter reaches terminal count.
#define DAQmx_Exported_CtrOutEvent_Pulse_Polarity                        0x1718 // Specifies the polarity of the pulses at the output terminal of the counter when Output Behavior is DAQmx_Val_Pulse. NI-DAQmx ignores this property if Output Behavior is DAQmx_Val_Toggle.
#define DAQmx_Exported_CtrOutEvent_Toggle_IdleState                      0x186A // Specifies the initial state of the output terminal of the counter when Output Behavior is DAQmx_Val_Toggle. The terminal enters this state when NI-DAQmx commits the task.
#define DAQmx_Exported_HshkEvent_OutputTerm                              0x22BA // Specifies the terminal to which to route the Handshake Event.
#define DAQmx_Exported_HshkEvent_OutputBehavior                          0x22BB // Specifies the output behavior of the Handshake Event.
#define DAQmx_Exported_HshkEvent_Delay                                   0x22BC // Specifies the number of seconds to delay after the Handshake Trigger deasserts before asserting the Handshake Event.
#define DAQmx_Exported_HshkEvent_Interlocked_AssertedLvl                 0x22BD // Specifies the asserted level of the exported Handshake Event if Output Behavior is DAQmx_Val_Interlocked.
#define DAQmx_Exported_HshkEvent_Interlocked_AssertOnStart               0x22BE // Specifies to assert the Handshake Event when the task starts if Output Behavior is DAQmx_Val_Interlocked.
#define DAQmx_Exported_HshkEvent_Interlocked_DeassertDelay               0x22BF // Specifies in seconds the amount of time to wait after the Handshake Trigger asserts before deasserting the Handshake Event if Output Behavior is DAQmx_Val_Interlocked.
#define DAQmx_Exported_HshkEvent_Pulse_Polarity                          0x22C0 // Specifies the polarity of the exported Handshake Event if Output Behavior is DAQmx_Val_Pulse.
#define DAQmx_Exported_HshkEvent_Pulse_Width                             0x22C1 // Specifies in seconds the pulse width of the exported Handshake Event if Output Behavior is DAQmx_Val_Pulse.
#define DAQmx_Exported_RdyForXferEvent_OutputTerm                        0x22B5 // Specifies the terminal to which to route the Ready for Transfer Event.
#define DAQmx_Exported_RdyForXferEvent_Lvl_ActiveLvl                     0x22B6 // Specifies the polarity of the exported Ready for Transfer Event.
#define DAQmx_Exported_SyncPulseEvent_OutputTerm                         0x223C // Specifies the terminal to which to route the Synchronization Pulse Event.
#define DAQmx_Exported_WatchdogExpiredEvent_OutputTerm                   0x21AA // Specifies the terminal  to which to route the Watchdog Timer Expired Event.

//********** Device Attributes **********
#define DAQmx_Dev_IsSimulated                                            0x22CA // Indicates if the device is a simulated device.
#define DAQmx_Dev_ProductType                                            0x0631 // Indicates the product name of the device.
#define DAQmx_Dev_ProductNum                                             0x231D // Indicates the unique hardware identification number for the device.
#define DAQmx_Dev_SerialNum                                              0x0632 // Indicates the serial number of the device. This value is zero if the device does not have a serial number.
#define DAQmx_Dev_AI_PhysicalChans                                       0x231E // Indicates an array containing the names of the analog input physical channels available on the device.
#define DAQmx_Dev_AO_PhysicalChans                                       0x231F // Indicates an array containing the names of the analog output physical channels available on the device.
#define DAQmx_Dev_DI_Lines                                               0x2320 // Indicates an array containing the names of the digital input lines available on the device.
#define DAQmx_Dev_DI_Ports                                               0x2321 // Indicates an array containing the names of the digital input ports available on the device.
#define DAQmx_Dev_DO_Lines                                               0x2322 // Indicates an array containing the names of the digital output lines available on the device.
#define DAQmx_Dev_DO_Ports                                               0x2323 // Indicates an array containing the names of the digital output ports available on the device.
#define DAQmx_Dev_CI_PhysicalChans                                       0x2324 // Indicates an array containing the names of the counter input physical channels available on the device.
#define DAQmx_Dev_CO_PhysicalChans                                       0x2325 // Indicates an array containing the names of the counter output physical channels available on the device.
#define DAQmx_Dev_BusType                                                0x2326 // Indicates the bus type of the device.
#define DAQmx_Dev_PCI_BusNum                                             0x2327 // Indicates the PCI bus number of the device.
#define DAQmx_Dev_PCI_DevNum                                             0x2328 // Indicates the PCI slot number of the device.
#define DAQmx_Dev_PXI_ChassisNum                                         0x2329 // Indicates the PXI chassis number of the device, as identified in MAX.
#define DAQmx_Dev_PXI_SlotNum                                            0x232A // Indicates the PXI slot number of the device.

//********** Read Attributes **********
#define DAQmx_Read_RelativeTo                                            0x190A // Specifies the point in the buffer at which to begin a read operation. If you also specify an offset with Offset, the read operation begins at that offset relative to the point you select with this property. The default value is DAQmx_Val_CurrReadPos unless you configure a Reference Trigger for the task. If you configure a Reference Trigger, the default value is DAQmx_Val_FirstPretrigSamp.
#define DAQmx_Read_Offset                                                0x190B // Specifies an offset in samples per channel at which to begin a read operation. This offset is relative to the location you specify with RelativeTo.
#define DAQmx_Read_ChannelsToRead                                        0x1823 // Specifies a subset of channels in the task from which to read.
#define DAQmx_Read_ReadAllAvailSamp                                      0x1215 // Specifies whether subsequent read operations read all samples currently available in the buffer or wait for the buffer to become full before reading. NI-DAQmx uses this setting for finite acquisitions and only when the number of samples to read is -1. For continuous acquisitions when the number of samples to read is -1, a read operation always reads all samples currently available in the buffer.
#define DAQmx_Read_AutoStart                                             0x1826 // Specifies if an NI-DAQmx Read function automatically starts the task  if you did not start the task explicitly by using DAQmxStartTask(). The default value is TRUE. When  an NI-DAQmx Read function starts a finite acquisition task, it also stops the task after reading the last sample.
#define DAQmx_Read_OverWrite                                             0x1211 // Specifies whether to overwrite samples in the buffer that you have not yet read.
#define DAQmx_Read_CurrReadPos                                           0x1221 // Indicates in samples per channel the current position in the buffer.
#define DAQmx_Read_AvailSampPerChan                                      0x1223 // Indicates the number of samples available to read per channel. This value is the same for all channels in the task.
#define DAQmx_Read_TotalSampPerChanAcquired                              0x192A // Indicates the total number of samples acquired by each channel. NI-DAQmx returns a single value because this value is the same for all channels.
#define DAQmx_Read_OverloadedChansExist                                  0x2174 // Indicates if the device detected an overload in any channel in the task. Reading this property clears the overload status for all channels in the task. You must read this property before you read Overloaded Channels. Otherwise, you will receive an error.
#define DAQmx_Read_OverloadedChans                                       0x2175 // Indicates the names of any overloaded virtual channels in the task. You must read Overloaded Channels Exist before you read this property. Otherwise, you will receive an error.
#define DAQmx_Read_ChangeDetect_HasOverflowed                            0x2194 // Indicates if samples were missed because change detection events occurred faster than the device could handle them. Some devices detect overflows differently than others.
#define DAQmx_Read_RawDataWidth                                          0x217A // Indicates in bytes the size of a raw sample from the task.
#define DAQmx_Read_NumChans                                              0x217B // Indicates the number of channels that an NI-DAQmx Read function reads from the task. This value is the number of channels in the task or the number of channels you specify with Channels to Read.
#define DAQmx_Read_DigitalLines_BytesPerChan                             0x217C // Indicates the number of bytes per channel that NI-DAQmx returns in a sample for line-based reads. If a channel has fewer lines than this number, the extra bytes are FALSE.
#define DAQmx_Read_WaitMode                                              0x2232 // Specifies how an NI-DAQmx Read function waits for samples to become available.
#define DAQmx_Read_SleepTime                                             0x22B0 // Specifies in seconds the amount of time to sleep after checking for available samples if Wait Mode is DAQmx_Val_Sleep.

//********** Real-Time Attributes **********
#define DAQmx_RealTime_ConvLateErrorsToWarnings                          0x22EE // Specifies if DAQmxWaitForNextSampleClock() and an NI-DAQmx Read function convert late errors to warnings. NI-DAQmx returns no late warnings or errors until the number of warmup iterations you specify with Number Of Warmup Iterations execute.
#define DAQmx_RealTime_NumOfWarmupIters                                  0x22ED // Specifies the number of loop iterations that must occur before DAQmxWaitForNextSampleClock() and an NI-DAQmx Read function return any late warnings or errors. The system needs a number of iterations to stabilize. During this period, a large amount of jitter occurs, potentially causing reads and writes to be late. The default number of warmup iterations is 100. Specify a larger number if needed to stabilize the sys...
#define DAQmx_RealTime_WaitForNextSampClkWaitMode                        0x22EF // Specifies how DAQmxWaitForNextSampleClock() waits for the next Sample Clock pulse.
#define DAQmx_RealTime_ReportMissedSamp                                  0x2319 // Specifies whether an NI-DAQmx Read function returns late errors or warnings when it detects missed Sample Clock pulses. This setting does not affect DAQmxWaitForNextSampleClock(). Set this property to TRUE for applications that need to detect lateness without using DAQmxWaitForNextSampleClock().
#define DAQmx_RealTime_WriteRecoveryMode                                 0x231A // Specifies how NI-DAQmx attempts to recover after missing a Sample Clock pulse when performing counter writes.

//********** Switch Channel Attributes **********
#define DAQmx_SwitchChan_Usage                                           0x18E4 // Specifies how you can use the channel. Using this property acts as a safety mechanism to prevent you from connecting two source channels, for example.
#define DAQmx_SwitchChan_MaxACCarryCurrent                               0x0648 // Indicates in amperes the maximum AC current that the device can carry.
#define DAQmx_SwitchChan_MaxACSwitchCurrent                              0x0646 // Indicates in amperes the maximum AC current that the device can switch. This current is always against an RMS voltage level.
#define DAQmx_SwitchChan_MaxACCarryPwr                                   0x0642 // Indicates in watts the maximum AC power that the device can carry.
#define DAQmx_SwitchChan_MaxACSwitchPwr                                  0x0644 // Indicates in watts the maximum AC power that the device can switch.
#define DAQmx_SwitchChan_MaxDCCarryCurrent                               0x0647 // Indicates in amperes the maximum DC current that the device can carry.
#define DAQmx_SwitchChan_MaxDCSwitchCurrent                              0x0645 // Indicates in amperes the maximum DC current that the device can switch. This current is always against a DC voltage level.
#define DAQmx_SwitchChan_MaxDCCarryPwr                                   0x0643 // Indicates in watts the maximum DC power that the device can carry.
#define DAQmx_SwitchChan_MaxDCSwitchPwr                                  0x0649 // Indicates in watts the maximum DC power that the device can switch.
#define DAQmx_SwitchChan_MaxACVoltage                                    0x0651 // Indicates in volts the maximum AC RMS voltage that the device can switch.
#define DAQmx_SwitchChan_MaxDCVoltage                                    0x0650 // Indicates in volts the maximum DC voltage that the device can switch.
#define DAQmx_SwitchChan_WireMode                                        0x18E5 // Indicates the number of wires that the channel switches.
#define DAQmx_SwitchChan_Bandwidth                                       0x0640 // Indicates in Hertz the maximum frequency of a signal that can pass through the switch without significant deterioration.
#define DAQmx_SwitchChan_Impedance                                       0x0641 // Indicates in ohms the switch impedance. This value is important in the RF domain and should match the impedance of the sources and loads.

//********** Switch Device Attributes **********
#define DAQmx_SwitchDev_SettlingTime                                     0x1244 // Specifies in seconds the amount of time to wait for the switch to settle (or debounce). NI-DAQmx adds this time to the settling time of the motherboard. Modify this property only if the switch does not settle within the settling time of the motherboard. Refer to device documentation for supported settling times.
#define DAQmx_SwitchDev_AutoConnAnlgBus                                  0x17DA // Specifies if NI-DAQmx routes multiplexed channels to the analog bus backplane. Only the SCXI-1127 and SCXI-1128 support this property.
#define DAQmx_SwitchDev_PwrDownLatchRelaysAfterSettling                  0x22DB // Specifies if DAQmxSwitchWaitForSettling() powers down latching relays after waiting for the device to settle.
#define DAQmx_SwitchDev_Settled                                          0x1243 // Indicates when Settling Time expires.
#define DAQmx_SwitchDev_RelayList                                        0x17DC // Indicates a comma-delimited list of relay names.
#define DAQmx_SwitchDev_NumRelays                                        0x18E6 // Indicates the number of relays on the device. This value matches the number of relay names in Relay List.
#define DAQmx_SwitchDev_SwitchChanList                                   0x18E7 // Indicates a comma-delimited list of channel names for the current topology of the device.
#define DAQmx_SwitchDev_NumSwitchChans                                   0x18E8 // Indicates the number of switch channels for the current topology of the device. This value matches the number of channel names in Switch Channel List.
#define DAQmx_SwitchDev_NumRows                                          0x18E9 // Indicates the number of rows on a device in a matrix switch topology. Indicates the number of multiplexed channels on a device in a mux topology.
#define DAQmx_SwitchDev_NumColumns                                       0x18EA // Indicates the number of columns on a device in a matrix switch topology. This value is always 1 if the device is in a mux topology.
#define DAQmx_SwitchDev_Topology                                         0x193D // Indicates the current topology of the device. This value is one of the topology options in DAQmxSwitchSetTopologyAndReset().

//********** Switch Scan Attributes **********
#define DAQmx_SwitchScan_BreakMode                                       0x1247 // Specifies the break mode between each entry in a scan list.
#define DAQmx_SwitchScan_RepeatMode                                      0x1248 // Specifies if the task advances through the scan list multiple times.
#define DAQmx_SwitchScan_WaitingForAdv                                   0x17D9 // Indicates if the switch hardware is waiting for an  Advance Trigger. If the hardware is waiting, it completed the previous entry in the scan list.

//********** Scale Attributes **********
#define DAQmx_Scale_Descr                                                0x1226 // Specifies a description for the scale.
#define DAQmx_Scale_ScaledUnits                                          0x191B // Specifies the units to use for scaled values. You can use an arbitrary string.
#define DAQmx_Scale_PreScaledUnits                                       0x18F7 // Specifies the units of the values that you want to scale.
#define DAQmx_Scale_Type                                                 0x1929 // Indicates the method or equation form that the custom scale uses.
#define DAQmx_Scale_Lin_Slope                                            0x1227 // Specifies the slope, m, in the equation y=mx+b.
#define DAQmx_Scale_Lin_YIntercept                                       0x1228 // Specifies the y-intercept, b, in the equation y=mx+b.
#define DAQmx_Scale_Map_ScaledMax                                        0x1229 // Specifies the largest value in the range of scaled values. NI-DAQmx maps this value to Pre-Scaled Maximum Value. Reads clip samples that are larger than this value. Writes generate errors for samples that are larger than this value.
#define DAQmx_Scale_Map_PreScaledMax                                     0x1231 // Specifies the largest value in the range of pre-scaled values. NI-DAQmx maps this value to Scaled Maximum Value.
#define DAQmx_Scale_Map_ScaledMin                                        0x1230 // Specifies the smallest value in the range of scaled values. NI-DAQmx maps this value to Pre-Scaled Minimum Value. Reads clip samples that are smaller than this value. Writes generate errors for samples that are smaller than this value.
#define DAQmx_Scale_Map_PreScaledMin                                     0x1232 // Specifies the smallest value in the range of pre-scaled values. NI-DAQmx maps this value to Scaled Minimum Value.
#define DAQmx_Scale_Poly_ForwardCoeff                                    0x1234 // Specifies an array of coefficients for the polynomial that converts pre-scaled values to scaled values. Each element of the array corresponds to a term of the equation. For example, if index three of the array is 9, the fourth term of the equation is 9x^3.
#define DAQmx_Scale_Poly_ReverseCoeff                                    0x1235 // Specifies an array of coefficients for the polynomial that converts scaled values to pre-scaled values. Each element of the array corresponds to a term of the equation. For example, if index three of the array is 9, the fourth term of the equation is 9y^3.
#define DAQmx_Scale_Table_ScaledVals                                     0x1236 // Specifies an array of scaled values. These values map directly to the values in Pre-Scaled Values.
#define DAQmx_Scale_Table_PreScaledVals                                  0x1237 // Specifies an array of pre-scaled values. These values map directly to the values in Scaled Values.

//********** System Attributes **********
#define DAQmx_Sys_GlobalChans                                            0x1265 // Indicates an array that contains the names of all global channels saved on the system.
#define DAQmx_Sys_Scales                                                 0x1266 // Indicates an array that contains the names of all custom scales saved on the system.
#define DAQmx_Sys_Tasks                                                  0x1267 // Indicates an array that contains the names of all tasks saved on the system.
#define DAQmx_Sys_DevNames                                               0x193B // Indicates the names of all devices installed in the system.
#define DAQmx_Sys_NIDAQMajorVersion                                      0x1272 // Indicates the major portion of the installed version of NI-DAQ, such as 7 for version 7.0.
#define DAQmx_Sys_NIDAQMinorVersion                                      0x1923 // Indicates the minor portion of the installed version of NI-DAQ, such as 0 for version 7.0.

//********** Task Attributes **********
#define DAQmx_Task_Name                                                  0x1276 // Indicates the name of the task.
#define DAQmx_Task_Channels                                              0x1273 // Indicates the names of all virtual channels in the task.
#define DAQmx_Task_NumChans                                              0x2181 // Indicates the number of virtual channels in the task.
#define DAQmx_Task_Devices                                               0x230E // Indicates an array containing the names of all devices in the task.
#define DAQmx_Task_Complete                                              0x1274 // Indicates whether the task completed execution.

//********** Timing Attributes **********
#define DAQmx_SampQuant_SampMode                                         0x1300 // Specifies if a task acquires or generates a finite number of samples or if it continuously acquires or generates samples.
#define DAQmx_SampQuant_SampPerChan                                      0x1310 // Specifies the number of samples to acquire or generate for each channel if Sample Mode is DAQmx_Val_FiniteSamps. If Sample Mode is DAQmx_Val_ContSamps, NI-DAQmx uses this value to determine the buffer size.
#define DAQmx_SampTimingType                                             0x1347 // Specifies the type of sample timing to use for the task.
#define DAQmx_SampClk_Rate                                               0x1344 // Specifies the sampling rate in samples per channel per second. If you use an external source for the Sample Clock, set this input to the maximum expected rate of that clock.
#define DAQmx_SampClk_MaxRate                                            0x22C8 // Indicates the maximum Sample Clock rate supported by the task, based on other timing settings. For output tasks, the maximum Sample Clock rate is the maximum rate of the DAC. For input tasks, NI-DAQmx calculates the maximum sampling rate differently for multiplexed devices than simultaneous sampling devices.
#define DAQmx_SampClk_Src                                                0x1852 // Specifies the terminal of the signal to use as the Sample Clock.
#define DAQmx_SampClk_ActiveEdge                                         0x1301 // Specifies on which edge of a clock pulse sampling takes place. This property is useful primarily when the signal you use as the Sample Clock is not a periodic clock.
#define DAQmx_SampClk_TimebaseDiv                                        0x18EB // Specifies the number of Sample Clock Timebase pulses needed to produce a single Sample Clock pulse.
#define DAQmx_SampClk_Timebase_Rate                                      0x1303 // Specifies the rate of the Sample Clock Timebase. Some applications require that you specify a rate when you use any signal other than the onboard Sample Clock Timebase. NI-DAQmx requires this rate to calculate other timing parameters.
#define DAQmx_SampClk_Timebase_Src                                       0x1308 // Specifies the terminal of the signal to use as the Sample Clock Timebase.
#define DAQmx_SampClk_Timebase_ActiveEdge                                0x18EC // Specifies on which edge to recognize a Sample Clock Timebase pulse. This property is useful primarily when the signal you use as the Sample Clock Timebase is not a periodic clock.
#define DAQmx_SampClk_Timebase_MasterTimebaseDiv                         0x1305 // Specifies the number of pulses of the Master Timebase needed to produce a single pulse of the Sample Clock Timebase.
#define DAQmx_SampClk_DigFltr_Enable                                     0x221E // Specifies whether to apply the pulse width filter to the signal.
#define DAQmx_SampClk_DigFltr_MinPulseWidth                              0x221F // Specifies in seconds the minimum pulse width the filter recognizes.
#define DAQmx_SampClk_DigFltr_TimebaseSrc                                0x2220 // Specifies the input terminal of the signal to use as the timebase of the pulse width filter.
#define DAQmx_SampClk_DigFltr_TimebaseRate                               0x2221 // Specifies in hertz the rate of the pulse width filter timebase. NI-DAQmx uses this value to compute settings for the filter.
#define DAQmx_SampClk_DigSync_Enable                                     0x2222 // Specifies whether to synchronize recognition of transitions in the signal to the internal timebase of the device.
#define DAQmx_Hshk_DelayAfterXfer                                        0x22C2 // Specifies the number of seconds to wait after a handshake cycle before starting a new handshake cycle.
#define DAQmx_Hshk_StartCond                                             0x22C3 // Specifies the point in the handshake cycle that the device is in when the task starts.
#define DAQmx_Hshk_SampleInputDataWhen                                   0x22C4 // Specifies on which edge of the Handshake Trigger an input task latches the data from the peripheral device.
#define DAQmx_ChangeDetect_DI_RisingEdgePhysicalChans                    0x2195 // Specifies the names of the digital lines or ports on which to detect rising edges. The lines or ports must be used by virtual channels in the task. You also can specify a string that contains a list or range of digital lines or ports.
#define DAQmx_ChangeDetect_DI_FallingEdgePhysicalChans                   0x2196 // Specifies the names of the digital lines or ports on which to detect rising edges. The lines or ports must be used by virtual channels in the task. You also can specify a string that contains a list or range of digital lines or ports.
#define DAQmx_OnDemand_SimultaneousAOEnable                              0x21A0 // Specifies whether to update all channels in the task simultaneously, rather than updating channels independently when you write a sample to that channel.
#define DAQmx_AIConv_Rate                                                0x1848 // Specifies the rate at which to clock the analog-to-digital converter. This clock is specific to the analog input section of multiplexed devices.
#define DAQmx_AIConv_MaxRate                                             0x22C9 // Indicates the maximum convert rate supported by the task, given the current devices and channel count.
#define DAQmx_AIConv_Src                                                 0x1502 // Specifies the terminal of the signal to use as the AI Convert Clock.
#define DAQmx_AIConv_ActiveEdge                                          0x1853 // Specifies on which edge of the clock pulse an analog-to-digital conversion takes place.
#define DAQmx_AIConv_TimebaseDiv                                         0x1335 // Specifies the number of AI Convert Clock Timebase pulses needed to produce a single AI Convert Clock pulse.
#define DAQmx_AIConv_Timebase_Src                                        0x1339 // Specifies the terminal  of the signal to use as the AI Convert Clock Timebase.
#define DAQmx_MasterTimebase_Rate                                        0x1495 // Specifies the rate of the Master Timebase.
#define DAQmx_MasterTimebase_Src                                         0x1343 // Specifies the terminal of the signal to use as the Master Timebase. On an E Series device, you can choose only between the onboard 20MHz Timebase or the RTSI7 terminal.
#define DAQmx_RefClk_Rate                                                0x1315 // Specifies the frequency of the Reference Clock.
#define DAQmx_RefClk_Src                                                 0x1316 // Specifies the terminal of the signal to use as the Reference Clock.
#define DAQmx_SyncPulse_Src                                              0x223D // Specifies the terminal of the signal to use as the synchronization pulse. The synchronization pulse resets the clock dividers and the ADCs/DACs on the device.
#define DAQmx_SyncPulse_SyncTime                                         0x223E // Indicates in seconds the delay required to reset the ADCs/DACs after the device receives the synchronization pulse.
#define DAQmx_SyncPulse_MinDelayToStart                                  0x223F // Specifies in seconds the amount of time that elapses after the master device issues the synchronization pulse before the task starts.
#define DAQmx_DelayFromSampClk_DelayUnits                                0x1304 // Specifies the units of Delay.
#define DAQmx_DelayFromSampClk_Delay                                     0x1317 // Specifies the amount of time to wait after receiving a Sample Clock edge before beginning to acquire the sample. This value is in the units you specify with Delay Units.

//********** Trigger Attributes **********
#define DAQmx_StartTrig_Type                                             0x1393 // Specifies the type of trigger to use to start a task.
#define DAQmx_DigEdge_StartTrig_Src                                      0x1407 // Specifies the name of a terminal where there is a digital signal to use as the source of the Start Trigger.
#define DAQmx_DigEdge_StartTrig_Edge                                     0x1404 // Specifies on which edge of a digital pulse to start acquiring or generating samples.
#define DAQmx_DigEdge_StartTrig_DigFltr_Enable                           0x2223 // Specifies whether to apply the pulse width filter to the signal.
#define DAQmx_DigEdge_StartTrig_DigFltr_MinPulseWidth                    0x2224 // Specifies in seconds the minimum pulse width the filter recognizes.
#define DAQmx_DigEdge_StartTrig_DigFltr_TimebaseSrc                      0x2225 // Specifies the input terminal of the signal to use as the timebase of the pulse width filter.
#define DAQmx_DigEdge_StartTrig_DigFltr_TimebaseRate                     0x2226 // Specifies in hertz the rate of the pulse width filter timebase. NI-DAQmx uses this value to compute settings for the filter.
#define DAQmx_DigEdge_StartTrig_DigSync_Enable                           0x2227 // Specifies whether to synchronize recognition of transitions in the signal to the internal timebase of the device.
#define DAQmx_DigPattern_StartTrig_Src                                   0x1410 // Specifies the physical channels to use for pattern matching. The order of the physical channels determines the order of the pattern. If a port is included, the order of the physical channels within the port is in ascending order.
#define DAQmx_DigPattern_StartTrig_Pattern                               0x2186 // Specifies the digital pattern that must be met for the Start Trigger to occur.
#define DAQmx_DigPattern_StartTrig_When                                  0x1411 // Specifies whether the Start Trigger occurs when the physical channels specified with Source match or differ from the digital pattern specified with Pattern.
#define DAQmx_AnlgEdge_StartTrig_Src                                     0x1398 // Specifies the name of a virtual channel or terminal where there is an analog signal to use as the source of the Start Trigger.
#define DAQmx_AnlgEdge_StartTrig_Slope                                   0x1397 // Specifies on which slope of the trigger signal to start acquiring or generating samples.
#define DAQmx_AnlgEdge_StartTrig_Lvl                                     0x1396 // Specifies at what threshold in the units of the measurement or generation to start acquiring or generating samples. Use Slope to specify on which slope to trigger on this threshold.
#define DAQmx_AnlgEdge_StartTrig_Hyst                                    0x1395 // Specifies a hysteresis level in the units of the measurement or generation. If Slope is DAQmx_Val_RisingSlope, the trigger does not deassert until the source signal passes below  Level minus the hysteresis. If Slope is DAQmx_Val_FallingSlope, the trigger does not deassert until the source signal passes above Level plus the hysteresis.
#define DAQmx_AnlgEdge_StartTrig_Coupling                                0x2233 // Specifies the coupling for the source signal of the trigger if the source is a terminal rather than a virtual channel.
#define DAQmx_AnlgWin_StartTrig_Src                                      0x1400 // Specifies the name of a virtual channel or terminal where there is an analog signal to use as the source of the Start Trigger.
#define DAQmx_AnlgWin_StartTrig_When                                     0x1401 // Specifies whether the task starts acquiring or generating samples when the signal enters or leaves the window you specify with Bottom and Top.
#define DAQmx_AnlgWin_StartTrig_Top                                      0x1403 // Specifies the upper limit of the window. Specify this value in the units of the measurement or generation.
#define DAQmx_AnlgWin_StartTrig_Btm                                      0x1402 // Specifies the lower limit of the window. Specify this value in the units of the measurement or generation.
#define DAQmx_AnlgWin_StartTrig_Coupling                                 0x2234 // Specifies the coupling for the source signal of the trigger if the source is a terminal rather than a virtual channel.
#define DAQmx_StartTrig_Delay                                            0x1856 // Specifies an amount of time to wait after the Start Trigger is received before acquiring or generating the first sample. This value is in the units you specify with Delay Units.
#define DAQmx_StartTrig_DelayUnits                                       0x18C8 // Specifies the units of Delay.
#define DAQmx_StartTrig_Retriggerable                                    0x190F // Specifies whether to enable retriggerable counter pulse generation. When you set this property to TRUE, the device generates pulses each time it receives a trigger. The device ignores a trigger if it is in the process of generating pulses.
#define DAQmx_RefTrig_Type                                               0x1419 // Specifies the type of trigger to use to mark a reference point for the measurement.
#define DAQmx_RefTrig_PretrigSamples                                     0x1445 // Specifies the minimum number of pretrigger samples to acquire from each channel before recognizing the reference trigger. Post-trigger samples per channel are equal to Samples Per Channel minus the number of pretrigger samples per channel.
#define DAQmx_DigEdge_RefTrig_Src                                        0x1434 // Specifies the name of a terminal where there is a digital signal to use as the source of the Reference Trigger.
#define DAQmx_DigEdge_RefTrig_Edge                                       0x1430 // Specifies on what edge of a digital pulse the Reference Trigger occurs.
#define DAQmx_DigPattern_RefTrig_Src                                     0x1437 // Specifies the physical channels to use for pattern matching. The order of the physical channels determines the order of the pattern. If a port is included, the order of the physical channels within the port is in ascending order.
#define DAQmx_DigPattern_RefTrig_Pattern                                 0x2187 // Specifies the digital pattern that must be met for the Reference Trigger to occur.
#define DAQmx_DigPattern_RefTrig_When                                    0x1438 // Specifies whether the Start Trigger occurs when the physical channels specified with Source match or differ from the digital pattern specified with Pattern.
#define DAQmx_AnlgEdge_RefTrig_Src                                       0x1424 // Specifies the name of a virtual channel or terminal where there is an analog signal to use as the source of the Reference Trigger.
#define DAQmx_AnlgEdge_RefTrig_Slope                                     0x1423 // Specifies on which slope of the source signal the Reference Trigger occurs.
#define DAQmx_AnlgEdge_RefTrig_Lvl                                       0x1422 // Specifies in the units of the measurement the threshold at which the Reference Trigger occurs.  Use Slope to specify on which slope to trigger at this threshold.
#define DAQmx_AnlgEdge_RefTrig_Hyst                                      0x1421 // Specifies a hysteresis level in the units of the measurement. If Slope is DAQmx_Val_RisingSlope, the trigger does not deassert until the source signal passes below Level minus the hysteresis. If Slope is DAQmx_Val_FallingSlope, the trigger does not deassert until the source signal passes above Level plus the hysteresis.
#define DAQmx_AnlgEdge_RefTrig_Coupling                                  0x2235 // Specifies the coupling for the source signal of the trigger if the source is a terminal rather than a virtual channel.
#define DAQmx_AnlgWin_RefTrig_Src                                        0x1426 // Specifies the name of a virtual channel or terminal where there is an analog signal to use as the source of the Reference Trigger.
#define DAQmx_AnlgWin_RefTrig_When                                       0x1427 // Specifies whether the Reference Trigger occurs when the source signal enters the window or when it leaves the window. Use Bottom and Top to specify the window.
#define DAQmx_AnlgWin_RefTrig_Top                                        0x1429 // Specifies the upper limit of the window. Specify this value in the units of the measurement.
#define DAQmx_AnlgWin_RefTrig_Btm                                        0x1428 // Specifies the lower limit of the window. Specify this value in the units of the measurement.
#define DAQmx_AnlgWin_RefTrig_Coupling                                   0x1857 // Specifies the coupling for the source signal of the trigger if the source is a terminal rather than a virtual channel.
#define DAQmx_AdvTrig_Type                                               0x1365 // Specifies the type of trigger to use to advance to the next entry in a switch scan list.
#define DAQmx_DigEdge_AdvTrig_Src                                        0x1362 // Specifies the name of a terminal where there is a digital signal to use as the source of the Advance Trigger.
#define DAQmx_DigEdge_AdvTrig_Edge                                       0x1360 // Specifies on which edge of a digital signal to advance to the next entry in a scan list.
#define DAQmx_DigEdge_AdvTrig_DigFltr_Enable                             0x2238 // Specifies whether to apply the pulse width filter to the signal.
#define DAQmx_HshkTrig_Type                                              0x22B7 // Specifies the type of Handshake Trigger to use.
#define DAQmx_Interlocked_HshkTrig_Src                                   0x22B8 // Specifies the source terminal of the Handshake Trigger.
#define DAQmx_Interlocked_HshkTrig_AssertedLvl                           0x22B9 // Specifies the asserted level of the Handshake Trigger.
#define DAQmx_PauseTrig_Type                                             0x1366 // Specifies the type of trigger to use to pause a task.
#define DAQmx_AnlgLvl_PauseTrig_Src                                      0x1370 // Specifies the name of a virtual channel or terminal where there is an analog signal to use as the source of the trigger.
#define DAQmx_AnlgLvl_PauseTrig_When                                     0x1371 // Specifies whether the task pauses above or below the threshold you specify with Level.
#define DAQmx_AnlgLvl_PauseTrig_Lvl                                      0x1369 // Specifies the threshold at which to pause the task. Specify this value in the units of the measurement or generation. Use Pause When to specify whether the task pauses above or below this threshold.
#define DAQmx_AnlgLvl_PauseTrig_Hyst                                     0x1368 // Specifies a hysteresis level in the units of the measurement or generation. If Pause When is DAQmx_Val_AboveLvl, the trigger does not deassert until the source signal passes below Level minus the hysteresis. If Pause When is DAQmx_Val_BelowLvl, the trigger does not deassert until the source signal passes above Level plus the hysteresis.
#define DAQmx_AnlgLvl_PauseTrig_Coupling                                 0x2236 // Specifies the coupling for the source signal of the trigger if the source is a terminal rather than a virtual channel.
#define DAQmx_AnlgWin_PauseTrig_Src                                      0x1373 // Specifies the name of a virtual channel or terminal where there is an analog signal to use as the source of the trigger.
#define DAQmx_AnlgWin_PauseTrig_When                                     0x1374 // Specifies whether the task pauses while the trigger signal is inside or outside the window you specify with Bottom and Top.
#define DAQmx_AnlgWin_PauseTrig_Top                                      0x1376 // Specifies the upper limit of the window. Specify this value in the units of the measurement or generation.
#define DAQmx_AnlgWin_PauseTrig_Btm                                      0x1375 // Specifies the lower limit of the window. Specify this value in the units of the measurement or generation.
#define DAQmx_AnlgWin_PauseTrig_Coupling                                 0x2237 // Specifies the coupling for the source signal of the trigger if the source is a terminal rather than a virtual channel.
#define DAQmx_DigLvl_PauseTrig_Src                                       0x1379 // Specifies the name of a terminal where there is a digital signal to use as the source of the Pause Trigger.
#define DAQmx_DigLvl_PauseTrig_When                                      0x1380 // Specifies whether the task pauses while the signal is high or low.
#define DAQmx_DigLvl_PauseTrig_DigFltr_Enable                            0x2228 // Specifies whether to apply the pulse width filter to the signal.
#define DAQmx_DigLvl_PauseTrig_DigFltr_MinPulseWidth                     0x2229 // Specifies in seconds the minimum pulse width the filter recognizes.
#define DAQmx_DigLvl_PauseTrig_DigFltr_TimebaseSrc                       0x222A // Specifies the input terminal of the signal to use as the timebase of the pulse width filter.
#define DAQmx_DigLvl_PauseTrig_DigFltr_TimebaseRate                      0x222B // Specifies in hertz the rate of the pulse width filter timebase. NI-DAQmx uses this value to compute settings for the filter.
#define DAQmx_DigLvl_PauseTrig_DigSync_Enable                            0x222C // Specifies whether to synchronize recognition of transitions in the signal to the internal timebase of the device.
#define DAQmx_ArmStartTrig_Type                                          0x1414 // Specifies the type of trigger to use to arm the task for a Start Trigger. If you configure an Arm Start Trigger, the task does not respond to a Start Trigger until the device receives the Arm Start Trigger.
#define DAQmx_DigEdge_ArmStartTrig_Src                                   0x1417 // Specifies the name of a terminal where there is a digital signal to use as the source of the Arm Start Trigger.
#define DAQmx_DigEdge_ArmStartTrig_Edge                                  0x1415 // Specifies on which edge of a digital signal to arm the task for a Start Trigger.
#define DAQmx_DigEdge_ArmStartTrig_DigFltr_Enable                        0x222D // Specifies whether to apply the pulse width filter to the signal.
#define DAQmx_DigEdge_ArmStartTrig_DigFltr_MinPulseWidth                 0x222E // Specifies in seconds the minimum pulse width the filter recognizes.
#define DAQmx_DigEdge_ArmStartTrig_DigFltr_TimebaseSrc                   0x222F // Specifies the input terminal of the signal to use as the timebase of the pulse width filter.
#define DAQmx_DigEdge_ArmStartTrig_DigFltr_TimebaseRate                  0x2230 // Specifies in hertz the rate of the pulse width filter timebase. NI-DAQmx uses this value to compute settings for the filter.
#define DAQmx_DigEdge_ArmStartTrig_DigSync_Enable                        0x2231 // Specifies whether to synchronize recognition of transitions in the signal to the internal timebase of the device.

//********** Watchdog Attributes **********
#define DAQmx_Watchdog_Timeout                                           0x21A9 // Specifies in seconds the amount of time until the watchdog timer expires. A value of -1 means the internal timer never expires. Set this input to -1 if you use an Expiration Trigger to expire the watchdog task.
#define DAQmx_WatchdogExpirTrig_Type                                     0x21A3 // Specifies the type of trigger to use to expire a watchdog task.
#define DAQmx_DigEdge_WatchdogExpirTrig_Src                              0x21A4 // Specifies the name of a terminal where a digital signal exists to use as the source of the Expiration Trigger.
#define DAQmx_DigEdge_WatchdogExpirTrig_Edge                             0x21A5 // Specifies on which edge of a digital signal to expire the watchdog task.
#define DAQmx_Watchdog_DO_ExpirState                                     0x21A7 // Specifies the state to which to set the digital physical channels when the watchdog task expires.  You cannot modify the expiration state of dedicated digital input physical channels.
#define DAQmx_Watchdog_HasExpired                                        0x21A8 // Indicates if the watchdog timer expired. You can read this property only while the task is running.

//********** Write Attributes **********
#define DAQmx_Write_RelativeTo                                           0x190C // Specifies the point in the buffer at which to write data. If you also specify an offset with Offset, the write operation begins at that offset relative to this point you select with this property.
#define DAQmx_Write_Offset                                               0x190D // Specifies in samples per channel an offset at which a write operation begins. This offset is relative to the location you specify with Relative To.
#define DAQmx_Write_RegenMode                                            0x1453 // Specifies whether to allow NI-DAQmx to generate the same data multiple times.
#define DAQmx_Write_CurrWritePos                                         0x1458 // Indicates the number of the next sample for the device to generate. This value is identical for all channels in the task.
#define DAQmx_Write_SpaceAvail                                           0x1460 // Indicates in samples per channel the amount of available space in the buffer.
#define DAQmx_Write_TotalSampPerChanGenerated                            0x192B // Indicates the total number of samples generated by each channel in the task. This value is identical for all channels in the task.
#define DAQmx_Write_RawDataWidth                                         0x217D // Indicates in bytes the required size of a raw sample to write to the task.
#define DAQmx_Write_NumChans                                             0x217E // Indicates the number of channels that an NI-DAQmx Write function writes to the task. This value is the number of channels in the task.
#define DAQmx_Write_WaitMode                                             0x22B1 // Specifies how an NI-DAQmx Write function waits for space to become available in the buffer.
#define DAQmx_Write_SleepTime                                            0x22B2 // Specifies in seconds the amount of time to sleep after checking for available buffer space if Wait Mode is DAQmx_Val_Sleep.
#define DAQmx_Write_DigitalLines_BytesPerChan                            0x217F // Indicates the number of bytes expected per channel in a sample for line-based writes. If a channel has fewer lines than this number, NI-DAQmx ignores the extra bytes.

//********** Physical Channel Attributes **********
#define DAQmx_PhysicalChan_TEDS_MfgID                                    0x21DA // Indicates the manufacturer ID of the sensor.
#define DAQmx_PhysicalChan_TEDS_ModelNum                                 0x21DB // Indicates the model number of the sensor.
#define DAQmx_PhysicalChan_TEDS_SerialNum                                0x21DC // Indicates the serial number of the sensor.
#define DAQmx_PhysicalChan_TEDS_VersionNum                               0x21DD // Indicates the version number of the sensor.
#define DAQmx_PhysicalChan_TEDS_VersionLetter                            0x21DE // Indicates the version letter of the sensor.
#define DAQmx_PhysicalChan_TEDS_BitStream                                0x21DF // Indicates the TEDS binary bitstream without checksums.
#define DAQmx_PhysicalChan_TEDS_TemplateIDs                              0x228F // Indicates the IDs of the templates in the bitstream in BitStream.

//********** Persisted Task Attributes **********
#define DAQmx_PersistedTask_Author                                       0x22CC // Indicates the author of the task.
#define DAQmx_PersistedTask_AllowInteractiveEditing                      0x22CD // Indicates whether the task can be edited in the DAQ Assistant.
#define DAQmx_PersistedTask_AllowInteractiveDeletion                     0x22CE // Indicates whether the task can be deleted through MAX.

//********** Persisted Channel Attributes **********
#define DAQmx_PersistedChan_Author                                       0x22D0 // Indicates the author of the global channel.
#define DAQmx_PersistedChan_AllowInteractiveEditing                      0x22D1 // Indicates whether the global channel can be edited in the DAQ Assistant.
#define DAQmx_PersistedChan_AllowInteractiveDeletion                     0x22D2 // Indicates whether the global channel can be deleted through MAX.

//********** Persisted Scale Attributes **********
#define DAQmx_PersistedScale_Author                                      0x22D4 // Indicates the author of the custom scale.
#define DAQmx_PersistedScale_AllowInteractiveEditing                     0x22D5 // Indicates whether the custom scale can be edited in the DAQ Assistant.
#define DAQmx_PersistedScale_AllowInteractiveDeletion                    0x22D6 // Indicates whether the custom scale can be deleted through MAX.


// For backwards compatibility, the DAQmx_ReadWaitMode has to be defined because this was the original spelling
// that has been later on corrected.
#define DAQmx_ReadWaitMode	DAQmx_Read_WaitMode

/******************************************************************************
 *** NI-DAQmx Values **********************************************************
 ******************************************************************************/

/******************************************************/
/***    Non-Attribute Function Parameter Values     ***/
/******************************************************/

//*** Values for the Mode parameter of DAQmxTaskControl ***
#define DAQmx_Val_Task_Start                                              0   // Start
#define DAQmx_Val_Task_Stop                                               1   // Stop
#define DAQmx_Val_Task_Verify                                             2   // Verify
#define DAQmx_Val_Task_Commit                                             3   // Commit
#define DAQmx_Val_Task_Reserve                                            4   // Reserve
#define DAQmx_Val_Task_Unreserve                                          5   // Unreserve
#define DAQmx_Val_Task_Abort                                              6   // Abort

//*** Values for the Options parameter of the event registration functions
#define DAQmx_Val_SynchronousEventCallbacks     (1<<0)     // Synchronous callbacks

//*** Values for the everyNsamplesEventType parameter of DAQmxRegisterEveryNSamplesEvent ***
#define DAQmx_Val_Acquired_Into_Buffer          1     // Acquired Into Buffer
#define DAQmx_Val_Transferred_From_Buffer       2     // Transferred From Buffer


//*** Values for the Action parameter of DAQmxControlWatchdogTask ***
#define DAQmx_Val_ResetTimer                                              0   // Reset Timer
#define DAQmx_Val_ClearExpiration                                         1   // Clear Expiration

//*** Values for the Line Grouping parameter of DAQmxCreateDIChan and DAQmxCreateDOChan ***
#define DAQmx_Val_ChanPerLine                                             0   // One Channel For Each Line
#define DAQmx_Val_ChanForAllLines                                         1   // One Channel For All Lines

//*** Values for the Fill Mode parameter of DAQmxReadAnalogF64, DAQmxReadBinaryI16, DAQmxReadBinaryU16, DAQmxReadBinaryI32, DAQmxReadBinaryU32,
//    DAQmxReadDigitalU8, DAQmxReadDigitalU32, DAQmxReadDigitalLines ***
//*** Values for the Data Layout parameter of DAQmxWriteAnalogF64, DAQmxWriteBinaryI16, DAQmxWriteDigitalU8, DAQmxWriteDigitalU32, DAQmxWriteDigitalLines ***
#define DAQmx_Val_GroupByChannel                                          0   // Group by Channel
#define DAQmx_Val_GroupByScanNumber                                       1   // Group by Scan Number

//*** Values for the Signal Modifiers parameter of DAQmxConnectTerms ***/
#define DAQmx_Val_DoNotInvertPolarity                                     0   // Do not invert polarity
#define DAQmx_Val_InvertPolarity                                          1   // Invert polarity

//*** Values for the Action paramter of DAQmxCloseExtCal ***
#define DAQmx_Val_Action_Commit                                           0   // Commit
#define DAQmx_Val_Action_Cancel                                           1   // Cancel

//*** Values for the Trigger ID parameter of DAQmxSendSoftwareTrigger ***
#define DAQmx_Val_AdvanceTrigger                                          12488 // Advance Trigger

//*** Value set for the ActiveEdge parameter of DAQmxCfgSampClkTiming ***
#define DAQmx_Val_Rising                                                  10280 // Rising
#define DAQmx_Val_Falling                                                 10171 // Falling

//*** Value set SwitchPathType ***
//*** Value set for the output Path Status parameter of DAQmxSwitchFindPath ***
#define DAQmx_Val_PathStatus_Available                                    10431 // Path Available
#define DAQmx_Val_PathStatus_AlreadyExists                                10432 // Path Already Exists
#define DAQmx_Val_PathStatus_Unsupported                                  10433 // Path Unsupported
#define DAQmx_Val_PathStatus_ChannelInUse                                 10434 // Channel In Use
#define DAQmx_Val_PathStatus_SourceChannelConflict                        10435 // Channel Source Conflict
#define DAQmx_Val_PathStatus_ChannelReservedForRouting                    10436 // Channel Reserved for Routing

//*** Value set for the Units parameter of DAQmxCreateAIThrmcplChan, DAQmxCreateAIRTDChan, DAQmxCreateAIThrmstrChanIex, DAQmxCreateAIThrmstrChanVex and DAQmxCreateAITempBuiltInSensorChan ***
#define DAQmx_Val_DegC                                                    10143 // Deg C
#define DAQmx_Val_DegF                                                    10144 // Deg F
#define DAQmx_Val_Kelvins                                                 10325 // Kelvins
#define DAQmx_Val_DegR                                                    10145 // Deg R

//*** Value set for the state parameter of DAQmxSetDigitalPowerUpStates ***
#define DAQmx_Val_High                                                    10192 // High
#define DAQmx_Val_Low                                                     10214 // Low
#define DAQmx_Val_Tristate                                                10310 // Tristate

//*** Value set for the channelType parameter of DAQmxSetAnalogPowerUpStates ***
#define DAQmx_Val_ChannelVoltage                                          0     // Voltage Channel
#define DAQmx_Val_ChannelCurrent                                          1     // Current Channel

//*** Value set RelayPos ***
//*** Value set for the state parameter of DAQmxSwitchGetSingleRelayPos and DAQmxSwitchGetMultiRelayPos ***
#define DAQmx_Val_Open                                                    10437 // Open
#define DAQmx_Val_Closed                                                  10438 // Closed

//*** Value for the Terminal Config parameter of DAQmxCreateAIVoltageChan, DAQmxCreateAICurrentChan and DAQmxCreateAIVoltageChanWithExcit ***
#define DAQmx_Val_Cfg_Default                                             -1 // Default

//*** Value for the Timeout parameter of DAQmxWaitUntilTaskDone
#define DAQmx_Val_WaitInfinitely                                          -1.0

//*** Value for the Number of Samples per Channel parameter of DAQmxReadAnalogF64, DAQmxReadBinaryI16, DAQmxReadBinaryU16,
//    DAQmxReadBinaryI32, DAQmxReadBinaryU32, DAQmxReadDigitalU8, DAQmxReadDigitalU32,
//    DAQmxReadDigitalLines, DAQmxReadCounterF64, DAQmxReadCounterU32 and DAQmxReadRaw ***
#define DAQmx_Val_Auto                                                    -1

// Value set for the Options parameter of DAQmxSaveTask, DAQmxSaveGlobalChan and DAQmxSaveScale
#define DAQmx_Val_Save_Overwrite                (1<<0)
#define DAQmx_Val_Save_AllowInteractiveEditing  (1<<1)
#define DAQmx_Val_Save_AllowInteractiveDeletion (1<<2)


/******************************************************/
/***              Attribute Values                  ***/
/******************************************************/

//*** Values for DAQmx_AI_ACExcit_WireMode ***
//*** Value set ACExcitWireMode ***
#define DAQmx_Val_4Wire                                                       4 // 4-Wire
#define DAQmx_Val_5Wire                                                       5 // 5-Wire

//*** Values for DAQmx_AI_MeasType ***
//*** Value set AIMeasurementType ***
#define DAQmx_Val_Voltage                                                 10322 // Voltage
#define DAQmx_Val_Current                                                 10134 // Current
#define DAQmx_Val_Voltage_CustomWithExcitation                            10323 // More:Voltage:Custom with Excitation
#define DAQmx_Val_Freq_Voltage                                            10181 // Frequency
#define DAQmx_Val_Resistance                                              10278 // Resistance
#define DAQmx_Val_Temp_TC                                                 10303 // Temperature:Thermocouple
#define DAQmx_Val_Temp_Thrmstr                                            10302 // Temperature:Thermistor
#define DAQmx_Val_Temp_RTD                                                10301 // Temperature:RTD
#define DAQmx_Val_Temp_BuiltInSensor                                      10311 // Temperature:Built-in Sensor
#define DAQmx_Val_Strain_Gage                                             10300 // Strain Gage
#define DAQmx_Val_Position_LVDT                                           10352 // Position:LVDT
#define DAQmx_Val_Position_RVDT                                           10353 // Position:RVDT
#define DAQmx_Val_Accelerometer                                           10356 // Accelerometer
#define DAQmx_Val_SoundPressure_Microphone                                10354 // Sound Pressure:Microphone
#define DAQmx_Val_TEDS_Sensor                                             12531 // TEDS Sensor

//*** Values for DAQmx_AO_IdleOutputBehavior ***
//*** Value set AOIdleOutputBehavior ***
#define DAQmx_Val_ZeroVolts                                               12526 // Zero Volts
#define DAQmx_Val_HighImpedance                                           12527 // High Impedance
#define DAQmx_Val_MaintainExistingValue                                   12528 // Maintain Existing Value

//*** Values for DAQmx_AO_OutputType ***
//*** Value set AOOutputChannelType ***
#define DAQmx_Val_Voltage                                                 10322 // Voltage
#define DAQmx_Val_Current                                                 10134 // Current

//*** Values for DAQmx_AI_Accel_SensitivityUnits ***
//*** Value set AccelSensitivityUnits1 ***
#define DAQmx_Val_mVoltsPerG                                              12509 // mVolts/g
#define DAQmx_Val_VoltsPerG                                               12510 // Volts/g

//*** Values for DAQmx_AI_Accel_Units ***
//*** Value set AccelUnits2 ***
#define DAQmx_Val_AccelUnit_g                                             10186 // g
#define DAQmx_Val_MetersPerSecondSquared                                  12470 // m/s^2
#define DAQmx_Val_FromCustomScale                                         10065 // From Custom Scale

//*** Values for DAQmx_SampQuant_SampMode ***
//*** Value set AcquisitionType ***
#define DAQmx_Val_FiniteSamps                                             10178 // Finite Samples
#define DAQmx_Val_ContSamps                                               10123 // Continuous Samples
#define DAQmx_Val_HWTimedSinglePoint                                      12522 // Hardware Timed Single Point

//*** Values for DAQmx_AnlgLvl_PauseTrig_When ***
//*** Value set ActiveLevel ***
#define DAQmx_Val_AboveLvl                                                10093 // Above Level
#define DAQmx_Val_BelowLvl                                                10107 // Below Level

//*** Values for DAQmx_AI_RVDT_Units ***
//*** Value set AngleUnits1 ***
#define DAQmx_Val_Degrees                                                 10146 // Degrees
#define DAQmx_Val_Radians                                                 10273 // Radians
#define DAQmx_Val_FromCustomScale                                         10065 // From Custom Scale

//*** Values for DAQmx_CI_AngEncoder_Units ***
//*** Value set AngleUnits2 ***
#define DAQmx_Val_Degrees                                                 10146 // Degrees
#define DAQmx_Val_Radians                                                 10273 // Radians
#define DAQmx_Val_Ticks                                                   10304 // Ticks
#define DAQmx_Val_FromCustomScale                                         10065 // From Custom Scale

//*** Values for DAQmx_AI_AutoZeroMode ***
//*** Value set AutoZeroType1 ***
#define DAQmx_Val_None                                                    10230 // None
#define DAQmx_Val_Once                                                    10244 // Once
#define DAQmx_Val_EverySample                                             10164 // Every Sample

//*** Values for DAQmx_SwitchScan_BreakMode ***
//*** Value set BreakMode ***
#define DAQmx_Val_NoAction                                                10227 // No Action
#define DAQmx_Val_BreakBeforeMake                                         10110 // Break Before Make

//*** Values for DAQmx_AI_Bridge_Cfg ***
//*** Value set BridgeConfiguration1 ***
#define DAQmx_Val_FullBridge                                              10182 // Full Bridge
#define DAQmx_Val_HalfBridge                                              10187 // Half Bridge
#define DAQmx_Val_QuarterBridge                                           10270 // Quarter Bridge
#define DAQmx_Val_NoBridge                                                10228 // No Bridge

//*** Values for DAQmx_Dev_BusType ***
//*** Value set BusType ***
#define DAQmx_Val_PCI                                                     12582 // PCI
#define DAQmx_Val_PCIe                                                    13612 // PCIe
#define DAQmx_Val_PXI                                                     12583 // PXI
#define DAQmx_Val_SCXI                                                    12584 // SCXI
#define DAQmx_Val_PCCard                                                  12585 // PCCard
#define DAQmx_Val_USB                                                     12586 // USB
#define DAQmx_Val_Unknown                                                 12588 // Unknown

//*** Values for DAQmx_CI_MeasType ***
//*** Value set CIMeasurementType ***
#define DAQmx_Val_CountEdges                                              10125 // Count Edges
#define DAQmx_Val_Freq                                                    10179 // Frequency
#define DAQmx_Val_Period                                                  10256 // Period
#define DAQmx_Val_PulseWidth                                              10359 // Pulse Width
#define DAQmx_Val_SemiPeriod                                              10289 // Semi Period
#define DAQmx_Val_Position_AngEncoder                                     10360 // Position:Angular Encoder
#define DAQmx_Val_Position_LinEncoder                                     10361 // Position:Linear Encoder
#define DAQmx_Val_TwoEdgeSep                                              10267 // Two Edge Separation
#define DAQmx_Val_GPS_Timestamp                                           10362 // GPS Timestamp

//*** Values for DAQmx_AI_Thrmcpl_CJCSrc ***
//*** Value set CJCSource1 ***
#define DAQmx_Val_BuiltIn                                                 10200 // Built-In
#define DAQmx_Val_ConstVal                                                10116 // Constant Value
#define DAQmx_Val_Chan                                                    10113 // Channel

//*** Values for DAQmx_CO_OutputType ***
//*** Value set COOutputType ***
#define DAQmx_Val_Pulse_Time                                              10269 // Pulse:Time
#define DAQmx_Val_Pulse_Freq                                              10119 // Pulse:Frequency
#define DAQmx_Val_Pulse_Ticks                                             10268 // Pulse:Ticks

//*** Values for DAQmx_ChanType ***
//*** Value set ChannelType ***
#define DAQmx_Val_AI                                                      10100 // Analog Input
#define DAQmx_Val_AO                                                      10102 // Analog Output
#define DAQmx_Val_DI                                                      10151 // Digital Input
#define DAQmx_Val_DO                                                      10153 // Digital Output
#define DAQmx_Val_CI                                                      10131 // Counter Input
#define DAQmx_Val_CO                                                      10132 // Counter Output

//*** Values for DAQmx_CI_CountEdges_Dir ***
//*** Value set CountDirection1 ***
#define DAQmx_Val_CountUp                                                 10128 // Count Up
#define DAQmx_Val_CountDown                                               10124 // Count Down
#define DAQmx_Val_ExtControlled                                           10326 // Externally Controlled

//*** Values for DAQmx_CI_Freq_MeasMeth ***
//*** Values for DAQmx_CI_Period_MeasMeth ***
//*** Value set CounterFrequencyMethod ***
#define DAQmx_Val_LowFreq1Ctr                                             10105 // Low Frequency with 1 Counter
#define DAQmx_Val_HighFreq2Ctr                                            10157 // High Frequency with 2 Counters
#define DAQmx_Val_LargeRng2Ctr                                            10205 // Large Range with 2 Counters

//*** Values for DAQmx_AI_Coupling ***
//*** Value set Coupling1 ***
#define DAQmx_Val_AC                                                      10045 // AC
#define DAQmx_Val_DC                                                      10050 // DC
#define DAQmx_Val_GND                                                     10066 // GND

//*** Values for DAQmx_AnlgEdge_StartTrig_Coupling ***
//*** Values for DAQmx_AnlgWin_StartTrig_Coupling ***
//*** Values for DAQmx_AnlgEdge_RefTrig_Coupling ***
//*** Values for DAQmx_AnlgWin_RefTrig_Coupling ***
//*** Values for DAQmx_AnlgLvl_PauseTrig_Coupling ***
//*** Values for DAQmx_AnlgWin_PauseTrig_Coupling ***
//*** Value set Coupling2 ***
#define DAQmx_Val_AC                                                      10045 // AC
#define DAQmx_Val_DC                                                      10050 // DC

//*** Values for DAQmx_AI_CurrentShunt_Loc ***
//*** Value set CurrentShuntResistorLocation1 ***
#define DAQmx_Val_Internal                                                10200 // Internal
#define DAQmx_Val_External                                                10167 // External

//*** Values for DAQmx_AI_Current_Units ***
//*** Values for DAQmx_AO_Current_Units ***
//*** Value set CurrentUnits1 ***
#define DAQmx_Val_Amps                                                    10342 // Amps
#define DAQmx_Val_FromCustomScale                                         10065 // From Custom Scale
#define DAQmx_Val_FromTEDS                                                12516 // From TEDS

//*** Value set CurrentUnits2 ***
#define DAQmx_Val_Amps                                                    10342 // Amps
#define DAQmx_Val_FromCustomScale                                         10065 // From Custom Scale

//*** Values for DAQmx_AI_RawSampJustification ***
//*** Value set DataJustification1 ***
#define DAQmx_Val_RightJustified                                          10279 // Right-Justified
#define DAQmx_Val_LeftJustified                                           10209 // Left-Justified

//*** Values for DAQmx_AI_DataXferMech ***
//*** Values for DAQmx_AO_DataXferMech ***
//*** Values for DAQmx_DI_DataXferMech ***
//*** Values for DAQmx_DO_DataXferMech ***
//*** Values for DAQmx_CI_DataXferMech ***
//*** Value set DataTransferMechanism ***
#define DAQmx_Val_DMA                                                     10054 // DMA
#define DAQmx_Val_Interrupts                                              10204 // Interrupts
#define DAQmx_Val_ProgrammedIO                                            10264 // Programmed I/O
#define DAQmx_Val_USBbulk                                                 12590 // USB Bulk

//*** Values for DAQmx_DO_OutputDriveType ***
//*** Value set DigitalDriveType ***
#define DAQmx_Val_ActiveDrive                                             12573 // Active Drive
#define DAQmx_Val_OpenCollector                                           12574 // Open Collector

//*** Values for DAQmx_Watchdog_DO_ExpirState ***
//*** Value set DigitalLineState ***
#define DAQmx_Val_High                                                    10192 // High
#define DAQmx_Val_Low                                                     10214 // Low
#define DAQmx_Val_Tristate                                                10310 // Tristate
#define DAQmx_Val_NoChange                                                10160 // No Change

//*** Values for DAQmx_DigPattern_StartTrig_When ***
//*** Values for DAQmx_DigPattern_RefTrig_When ***
//*** Value set DigitalPatternCondition1 ***
#define DAQmx_Val_PatternMatches                                          10254 // Pattern Matches
#define DAQmx_Val_PatternDoesNotMatch                                     10253 // Pattern Does Not Match

//*** Values for DAQmx_StartTrig_DelayUnits ***
//*** Value set DigitalWidthUnits1 ***
#define DAQmx_Val_SampClkPeriods                                          10286 // Sample Clock Periods
#define DAQmx_Val_Seconds                                                 10364 // Seconds
#define DAQmx_Val_Ticks                                                   10304 // Ticks

//*** Values for DAQmx_DelayFromSampClk_DelayUnits ***
//*** Value set DigitalWidthUnits2 ***
#define DAQmx_Val_Seconds                                                 10364 // Seconds
#define DAQmx_Val_Ticks                                                   10304 // Ticks

//*** Values for DAQmx_Exported_AdvTrig_Pulse_WidthUnits ***
//*** Value set DigitalWidthUnits3 ***
#define DAQmx_Val_Seconds                                                 10364 // Seconds

//*** Values for DAQmx_CI_Freq_StartingEdge ***
//*** Values for DAQmx_CI_Period_StartingEdge ***
//*** Values for DAQmx_CI_CountEdges_ActiveEdge ***
//*** Values for DAQmx_CI_PulseWidth_StartingEdge ***
//*** Values for DAQmx_CI_TwoEdgeSep_FirstEdge ***
//*** Values for DAQmx_CI_TwoEdgeSep_SecondEdge ***
//*** Values for DAQmx_CI_SemiPeriod_StartingEdge ***
//*** Values for DAQmx_CI_CtrTimebaseActiveEdge ***
//*** Values for DAQmx_CO_CtrTimebaseActiveEdge ***
//*** Values for DAQmx_SampClk_ActiveEdge ***
//*** Values for DAQmx_SampClk_Timebase_ActiveEdge ***
//*** Values for DAQmx_AIConv_ActiveEdge ***
//*** Values for DAQmx_DigEdge_StartTrig_Edge ***
//*** Values for DAQmx_DigEdge_RefTrig_Edge ***
//*** Values for DAQmx_DigEdge_AdvTrig_Edge ***
//*** Values for DAQmx_DigEdge_ArmStartTrig_Edge ***
//*** Values for DAQmx_DigEdge_WatchdogExpirTrig_Edge ***
//*** Value set Edge1 ***
#define DAQmx_Val_Rising                                                  10280 // Rising
#define DAQmx_Val_Falling                                                 10171 // Falling

//*** Values for DAQmx_CI_Encoder_DecodingType ***
//*** Value set EncoderType2 ***
#define DAQmx_Val_X1                                                      10090 // X1
#define DAQmx_Val_X2                                                      10091 // X2
#define DAQmx_Val_X4                                                      10092 // X4
#define DAQmx_Val_TwoPulseCounting                                        10313 // Two Pulse Counting

//*** Values for DAQmx_CI_Encoder_ZIndexPhase ***
//*** Value set EncoderZIndexPhase1 ***
#define DAQmx_Val_AHighBHigh                                              10040 // A High B High
#define DAQmx_Val_AHighBLow                                               10041 // A High B Low
#define DAQmx_Val_ALowBHigh                                               10042 // A Low B High
#define DAQmx_Val_ALowBLow                                                10043 // A Low B Low

//*** Values for DAQmx_AI_Excit_DCorAC ***
//*** Value set ExcitationDCorAC ***
#define DAQmx_Val_DC                                                      10050 // DC
#define DAQmx_Val_AC                                                      10045 // AC

//*** Values for DAQmx_AI_Excit_Src ***
//*** Value set ExcitationSource ***
#define DAQmx_Val_Internal                                                10200 // Internal
#define DAQmx_Val_External                                                10167 // External
#define DAQmx_Val_None                                                    10230 // None

//*** Values for DAQmx_AI_Excit_VoltageOrCurrent ***
//*** Value set ExcitationVoltageOrCurrent ***
#define DAQmx_Val_Voltage                                                 10322 // Voltage
#define DAQmx_Val_Current                                                 10134 // Current

//*** Values for DAQmx_Exported_CtrOutEvent_OutputBehavior ***
//*** Value set ExportActions2 ***
#define DAQmx_Val_Pulse                                                   10265 // Pulse
#define DAQmx_Val_Toggle                                                  10307 // Toggle

//*** Values for DAQmx_Exported_SampClk_OutputBehavior ***
//*** Value set ExportActions3 ***
#define DAQmx_Val_Pulse                                                   10265 // Pulse
#define DAQmx_Val_Lvl                                                     10210 // Level

//*** Values for DAQmx_Exported_HshkEvent_OutputBehavior ***
//*** Value set ExportActions5 ***
#define DAQmx_Val_Interlocked                                             12549 // Interlocked
#define DAQmx_Val_Pulse                                                   10265 // Pulse

//*** Values for DAQmx_AI_Freq_Units ***
//*** Value set FrequencyUnits ***
#define DAQmx_Val_Hz                                                      10373 // Hz
#define DAQmx_Val_FromCustomScale                                         10065 // From Custom Scale

//*** Values for DAQmx_CO_Pulse_Freq_Units ***
//*** Value set FrequencyUnits2 ***
#define DAQmx_Val_Hz                                                      10373 // Hz

//*** Values for DAQmx_CI_Freq_Units ***
//*** Value set FrequencyUnits3 ***
#define DAQmx_Val_Hz                                                      10373 // Hz
#define DAQmx_Val_Ticks                                                   10304 // Ticks
#define DAQmx_Val_FromCustomScale                                         10065 // From Custom Scale

//*** Values for DAQmx_CI_GPS_SyncMethod ***
//*** Value set GpsSignalType1 ***
#define DAQmx_Val_IRIGB                                                   10070 // IRIG-B
#define DAQmx_Val_PPS                                                     10080 // PPS
#define DAQmx_Val_None                                                    10230 // None

//*** Values for DAQmx_Hshk_StartCond ***
//*** Value set HandshakeStartCondition ***
#define DAQmx_Val_Immediate                                               10198 // Immediate
#define DAQmx_Val_WaitForHandshakeTriggerAssert                           12550 // Wait For Handshake Trigger Assert
#define DAQmx_Val_WaitForHandshakeTriggerDeassert                         12551 // Wait For Handshake Trigger Deassert


//*** Values for DAQmx_AI_DataXferReqCond ***
//*** Values for DAQmx_DI_DataXferReqCond ***
//*** Value set InputDataTransferCondition ***
#define DAQmx_Val_OnBrdMemMoreThanHalfFull                                10237 // Onboard Memory More than Half Full
#define DAQmx_Val_OnBrdMemNotEmpty                                        10241 // Onboard Memory Not Empty
#define DAQmx_Val_OnbrdMemCustomThreshold                                 12577 // Onboard Memory Custom Threshold
#define DAQmx_Val_WhenAcqComplete                                         12546 // When Acquisition Complete

//*** Values for DAQmx_AI_TermCfg ***
//*** Value set InputTermCfg ***
#define DAQmx_Val_RSE                                                     10083 // RSE
#define DAQmx_Val_NRSE                                                    10078 // NRSE
#define DAQmx_Val_Diff                                                    10106 // Differential
#define DAQmx_Val_PseudoDiff                                              12529 // Pseudodifferential

//*** Values for DAQmx_AI_LVDT_SensitivityUnits ***
//*** Value set LVDTSensitivityUnits1 ***
#define DAQmx_Val_mVoltsPerVoltPerMillimeter                              12506 // mVolts/Volt/mMeter
#define DAQmx_Val_mVoltsPerVoltPerMilliInch                               12505 // mVolts/Volt/0.001 Inch

//*** Values for DAQmx_AI_LVDT_Units ***
//*** Value set LengthUnits2 ***
#define DAQmx_Val_Meters                                                  10219 // Meters
#define DAQmx_Val_Inches                                                  10379 // Inches
#define DAQmx_Val_FromCustomScale                                         10065 // From Custom Scale

//*** Values for DAQmx_CI_LinEncoder_Units ***
//*** Value set LengthUnits3 ***
#define DAQmx_Val_Meters                                                  10219 // Meters
#define DAQmx_Val_Inches                                                  10379 // Inches
#define DAQmx_Val_Ticks                                                   10304 // Ticks
#define DAQmx_Val_FromCustomScale                                         10065 // From Custom Scale

//*** Values for DAQmx_CI_OutputState ***
//*** Values for DAQmx_CO_Pulse_IdleState ***
//*** Values for DAQmx_CO_OutputState ***
//*** Values for DAQmx_Exported_CtrOutEvent_Toggle_IdleState ***
//*** Values for DAQmx_Exported_HshkEvent_Interlocked_AssertedLvl ***
//*** Values for DAQmx_Interlocked_HshkTrig_AssertedLvl ***
//*** Values for DAQmx_DigLvl_PauseTrig_When ***
//*** Value set Level1 ***
#define DAQmx_Val_High                                                    10192 // High
#define DAQmx_Val_Low                                                     10214 // Low

//*** Values for DAQmx_AIConv_Timebase_Src ***
//*** Value set MIOAIConvertTbSrc ***
#define DAQmx_Val_SameAsSampTimebase                                      10284 // Same as Sample Timebase
#define DAQmx_Val_SameAsMasterTimebase                                    10282 // Same as Master Timebase
#define DAQmx_Val_20MHzTimebase                                           12537 // 20MHz Timebase

//*** Values for DAQmx_AO_DataXferReqCond ***
//*** Values for DAQmx_DO_DataXferReqCond ***
//*** Value set OutputDataTransferCondition ***
#define DAQmx_Val_OnBrdMemEmpty                                           10235 // Onboard Memory Empty
#define DAQmx_Val_OnBrdMemHalfFullOrLess                                  10239 // Onboard Memory Half Full or Less
#define DAQmx_Val_OnBrdMemNotFull                                         10242 // Onboard Memory Less than Full

//*** Values for DAQmx_AO_TermCfg ***
//*** Value set OutputTermCfg ***
#define DAQmx_Val_RSE                                                     10083 // RSE
#define DAQmx_Val_Diff                                                    10106 // Differential
#define DAQmx_Val_PseudoDiff                                              12529 // Pseudodifferential

//*** Values for DAQmx_Read_OverWrite ***
//*** Value set OverwriteMode1 ***
#define DAQmx_Val_OverwriteUnreadSamps                                    10252 // Overwrite Unread Samples
#define DAQmx_Val_DoNotOverwriteUnreadSamps                               10159 // Do Not Overwrite Unread Samples

//*** Values for DAQmx_Exported_AIConvClk_Pulse_Polarity ***
//*** Values for DAQmx_Exported_SampClk_Pulse_Polarity ***
//*** Values for DAQmx_Exported_AdvTrig_Pulse_Polarity ***
//*** Values for DAQmx_Exported_AdvCmpltEvent_Pulse_Polarity ***
//*** Values for DAQmx_Exported_AIHoldCmpltEvent_PulsePolarity ***
//*** Values for DAQmx_Exported_ChangeDetectEvent_Pulse_Polarity ***
//*** Values for DAQmx_Exported_CtrOutEvent_Pulse_Polarity ***
//*** Values for DAQmx_Exported_HshkEvent_Pulse_Polarity ***
//*** Values for DAQmx_Exported_RdyForXferEvent_Lvl_ActiveLvl ***
//*** Value set Polarity2 ***
#define DAQmx_Val_ActiveHigh                                              10095 // Active High
#define DAQmx_Val_ActiveLow                                               10096 // Active Low


//*** Values for DAQmx_AI_RTD_Type ***
//*** Value set RTDType1 ***
#define DAQmx_Val_Pt3750                                                  12481 // Pt3750
#define DAQmx_Val_Pt3851                                                  10071 // Pt3851
#define DAQmx_Val_Pt3911                                                  12482 // Pt3911
#define DAQmx_Val_Pt3916                                                  10069 // Pt3916
#define DAQmx_Val_Pt3920                                                  10053 // Pt3920
#define DAQmx_Val_Pt3928                                                  12483 // Pt3928
#define DAQmx_Val_Custom                                                  10137 // Custom

//*** Values for DAQmx_AI_RVDT_SensitivityUnits ***
//*** Value set RVDTSensitivityUnits1 ***
#define DAQmx_Val_mVoltsPerVoltPerDegree                                  12507 // mVolts/Volt/Degree
#define DAQmx_Val_mVoltsPerVoltPerRadian                                  12508 // mVolts/Volt/Radian

//*** Values for DAQmx_AI_RawDataCompressionType ***
//*** Value set RawDataCompressionType ***
#define DAQmx_Val_None                                                    10230 // None
#define DAQmx_Val_LosslessPacking                                         12555 // Lossless Packing
#define DAQmx_Val_LossyLSBRemoval                                         12556 // Lossy LSB Removal

//*** Values for DAQmx_Read_RelativeTo ***
//*** Value set ReadRelativeTo ***
#define DAQmx_Val_FirstSample                                             10424 // First Sample
#define DAQmx_Val_CurrReadPos                                             10425 // Current Read Position
#define DAQmx_Val_RefTrig                                                 10426 // Reference Trigger
#define DAQmx_Val_FirstPretrigSamp                                        10427 // First Pretrigger Sample
#define DAQmx_Val_MostRecentSamp                                          10428 // Most Recent Sample


//*** Values for DAQmx_Write_RegenMode ***
//*** Value set RegenerationMode1 ***
#define DAQmx_Val_AllowRegen                                              10097 // Allow Regeneration
#define DAQmx_Val_DoNotAllowRegen                                         10158 // Do Not Allow Regeneration

//*** Values for DAQmx_AI_ResistanceCfg ***
//*** Value set ResistanceConfiguration ***
#define DAQmx_Val_2Wire                                                       2 // 2-Wire
#define DAQmx_Val_3Wire                                                       3 // 3-Wire
#define DAQmx_Val_4Wire                                                       4 // 4-Wire

//*** Values for DAQmx_AI_Resistance_Units ***
//*** Value set ResistanceUnits1 ***
#define DAQmx_Val_Ohms                                                    10384 // Ohms
#define DAQmx_Val_FromCustomScale                                         10065 // From Custom Scale
#define DAQmx_Val_FromTEDS                                                12516 // From TEDS

//*** Value set ResistanceUnits2 ***
#define DAQmx_Val_Ohms                                                    10384 // Ohms
#define DAQmx_Val_FromCustomScale                                         10065 // From Custom Scale

//*** Values for DAQmx_AI_ResolutionUnits ***
//*** Values for DAQmx_AO_ResolutionUnits ***
//*** Value set ResolutionType1 ***
#define DAQmx_Val_Bits                                                    10109 // Bits

//*** Values for DAQmx_Hshk_SampleInputDataWhen ***
//*** Value set SampleInputDataWhen ***
#define DAQmx_Val_HandshakeTriggerAsserts                                 12552 // Handshake Trigger Asserts
#define DAQmx_Val_HandshakeTriggerDeasserts                               12553 // Handshake Trigger Deasserts

//*** Values for DAQmx_SampTimingType ***
//*** Value set SampleTimingType ***
#define DAQmx_Val_SampClk                                                 10388 // Sample Clock
#define DAQmx_Val_BurstHandshake                                          12548 // Burst Handshake
#define DAQmx_Val_Handshake                                               10389 // Handshake
#define DAQmx_Val_Implicit                                                10451 // Implicit
#define DAQmx_Val_OnDemand                                                10390 // On Demand
#define DAQmx_Val_ChangeDetection                                         12504 // Change Detection

//*** Values for DAQmx_Scale_Type ***
//*** Value set ScaleType ***
#define DAQmx_Val_Linear                                                  10447 // Linear
#define DAQmx_Val_MapRanges                                               10448 // Map Ranges
#define DAQmx_Val_Polynomial                                              10449 // Polynomial
#define DAQmx_Val_Table                                                   10450 // Table

//*** Values for DAQmx_AI_ChanCal_ScaleType ***
//*** Value set ScaleType2 ***
#define DAQmx_Val_Polynomial                                              10449 // Polynomial
#define DAQmx_Val_Table                                                   10450 // Table

//*** Values for DAQmx_AI_Bridge_ShuntCal_Select ***
//*** Value set ShuntCalSelect ***
#define DAQmx_Val_A                                                       12513 // A
#define DAQmx_Val_B                                                       12514 // B
#define DAQmx_Val_AandB                                                   12515 // A and B

//*** Value set Signal ***
#define DAQmx_Val_AIConvertClock                                          12484 // AI Convert Clock
#define DAQmx_Val_10MHzRefClock                                           12536 // 10MHz Reference Clock
#define DAQmx_Val_20MHzTimebaseClock                                      12486 // 20MHz Timebase Clock
#define DAQmx_Val_SampleClock                                             12487 // Sample Clock
#define DAQmx_Val_AdvanceTrigger                                          12488 // Advance Trigger
#define DAQmx_Val_ReferenceTrigger                                        12490 // Reference Trigger
#define DAQmx_Val_StartTrigger                                            12491 // Start Trigger
#define DAQmx_Val_AdvCmpltEvent                                           12492 // Advance Complete Event
#define DAQmx_Val_AIHoldCmpltEvent                                        12493 // AI Hold Complete Event
#define DAQmx_Val_CounterOutputEvent                                      12494 // Counter Output Event
#define DAQmx_Val_ChangeDetectionEvent                                    12511 // Change Detection Event
#define DAQmx_Val_WDTExpiredEvent                                         12512 // Watchdog Timer Expired Event

//*** Value set Signal2 ***
#define DAQmx_Val_SampleCompleteEvent                                     12530 // Sample Complete Event
#define DAQmx_Val_CounterOutputEvent                                      12494 // Counter Output Event
#define DAQmx_Val_ChangeDetectionEvent                                    12511 // Change Detection Event
#define DAQmx_Val_SampleClock                                             12487 // Sample Clock

//*** Values for DAQmx_AnlgEdge_StartTrig_Slope ***
//*** Values for DAQmx_AnlgEdge_RefTrig_Slope ***
//*** Value set Slope1 ***
#define DAQmx_Val_RisingSlope                                             10280 // Rising
#define DAQmx_Val_FallingSlope                                            10171 // Falling

//*** Values for DAQmx_AI_SoundPressure_Units ***
//*** Value set SoundPressureUnits1 ***
#define DAQmx_Val_Pascals                                                 10081 // Pascals
#define DAQmx_Val_FromCustomScale                                         10065 // From Custom Scale

//*** Values for DAQmx_AI_Lowpass_SwitchCap_ClkSrc ***
//*** Values for DAQmx_AO_DAC_Ref_Src ***
//*** Values for DAQmx_AO_DAC_Offset_Src ***
//*** Value set SourceSelection ***
#define DAQmx_Val_Internal                                                10200 // Internal
#define DAQmx_Val_External                                                10167 // External

//*** Values for DAQmx_AI_StrainGage_Cfg ***
//*** Value set StrainGageBridgeType1 ***
#define DAQmx_Val_FullBridgeI                                             10183 // Full Bridge I
#define DAQmx_Val_FullBridgeII                                            10184 // Full Bridge II
#define DAQmx_Val_FullBridgeIII                                           10185 // Full Bridge III
#define DAQmx_Val_HalfBridgeI                                             10188 // Half Bridge I
#define DAQmx_Val_HalfBridgeII                                            10189 // Half Bridge II
#define DAQmx_Val_QuarterBridgeI                                          10271 // Quarter Bridge I
#define DAQmx_Val_QuarterBridgeII                                         10272 // Quarter Bridge II

//*** Values for DAQmx_AI_Strain_Units ***
//*** Value set StrainUnits1 ***
#define DAQmx_Val_Strain                                                  10299 // Strain
#define DAQmx_Val_FromCustomScale                                         10065 // From Custom Scale

//*** Values for DAQmx_SwitchScan_RepeatMode ***
//*** Value set SwitchScanRepeatMode ***
#define DAQmx_Val_Finite                                                  10172 // Finite
#define DAQmx_Val_Cont                                                    10117 // Continuous

//*** Values for DAQmx_SwitchChan_Usage ***
//*** Value set SwitchUsageTypes ***
#define DAQmx_Val_Source                                                  10439 // Source
#define DAQmx_Val_Load                                                    10440 // Load
#define DAQmx_Val_ReservedForRouting                                      10441 // Reserved for Routing

//*** Value set TEDSUnits ***
#define DAQmx_Val_FromCustomScale                                         10065 // From Custom Scale
#define DAQmx_Val_FromTEDS                                                12516 // From TEDS

//*** Values for DAQmx_AI_Temp_Units ***
//*** Value set TemperatureUnits1 ***
#define DAQmx_Val_DegC                                                    10143 // Deg C
#define DAQmx_Val_DegF                                                    10144 // Deg F
#define DAQmx_Val_Kelvins                                                 10325 // Kelvins
#define DAQmx_Val_DegR                                                    10145 // Deg R
#define DAQmx_Val_FromCustomScale                                         10065 // From Custom Scale

//*** Values for DAQmx_AI_Thrmcpl_Type ***
//*** Value set ThermocoupleType1 ***
#define DAQmx_Val_J_Type_TC                                               10072 // J
#define DAQmx_Val_K_Type_TC                                               10073 // K
#define DAQmx_Val_N_Type_TC                                               10077 // N
#define DAQmx_Val_R_Type_TC                                               10082 // R
#define DAQmx_Val_S_Type_TC                                               10085 // S
#define DAQmx_Val_T_Type_TC                                               10086 // T
#define DAQmx_Val_B_Type_TC                                               10047 // B
#define DAQmx_Val_E_Type_TC                                               10055 // E

//*** Values for DAQmx_CI_Timestamp_Units ***
//*** Value set TimeUnits ***
#define DAQmx_Val_Seconds                                                 10364 // Seconds
#define DAQmx_Val_FromCustomScale                                         10065 // From Custom Scale

//*** Values for DAQmx_CO_Pulse_Time_Units ***
//*** Value set TimeUnits2 ***
#define DAQmx_Val_Seconds                                                 10364 // Seconds

//*** Values for DAQmx_CI_Period_Units ***
//*** Values for DAQmx_CI_PulseWidth_Units ***
//*** Values for DAQmx_CI_TwoEdgeSep_Units ***
//*** Values for DAQmx_CI_SemiPeriod_Units ***
//*** Value set TimeUnits3 ***
#define DAQmx_Val_Seconds                                                 10364 // Seconds
#define DAQmx_Val_Ticks                                                   10304 // Ticks
#define DAQmx_Val_FromCustomScale                                         10065 // From Custom Scale

//*** Values for DAQmx_ArmStartTrig_Type ***
//*** Values for DAQmx_WatchdogExpirTrig_Type ***
//*** Value set TriggerType4 ***
#define DAQmx_Val_DigEdge                                                 10150 // Digital Edge
#define DAQmx_Val_None                                                    10230 // None

//*** Values for DAQmx_AdvTrig_Type ***
//*** Value set TriggerType5 ***
#define DAQmx_Val_DigEdge                                                 10150 // Digital Edge
#define DAQmx_Val_Software                                                10292 // Software
#define DAQmx_Val_None                                                    10230 // None

//*** Values for DAQmx_PauseTrig_Type ***
//*** Value set TriggerType6 ***
#define DAQmx_Val_AnlgLvl                                                 10101 // Analog Level
#define DAQmx_Val_AnlgWin                                                 10103 // Analog Window
#define DAQmx_Val_DigLvl                                                  10152 // Digital Level
#define DAQmx_Val_None                                                    10230 // None

//*** Values for DAQmx_StartTrig_Type ***
//*** Values for DAQmx_RefTrig_Type ***
//*** Value set TriggerType8 ***
#define DAQmx_Val_AnlgEdge                                                10099 // Analog Edge
#define DAQmx_Val_DigEdge                                                 10150 // Digital Edge
#define DAQmx_Val_DigPattern                                              10398 // Digital Pattern
#define DAQmx_Val_AnlgWin                                                 10103 // Analog Window
#define DAQmx_Val_None                                                    10230 // None

//*** Values for DAQmx_HshkTrig_Type ***
//*** Value set TriggerType9 ***
#define DAQmx_Val_Interlocked                                             12549 // Interlocked
#define DAQmx_Val_None                                                    10230 // None

//*** Values for DAQmx_Scale_PreScaledUnits ***
//*** Value set UnitsPreScaled ***
#define DAQmx_Val_Volts                                                   10348 // Volts
#define DAQmx_Val_Amps                                                    10342 // Amps
#define DAQmx_Val_DegF                                                    10144 // Deg F
#define DAQmx_Val_DegC                                                    10143 // Deg C
#define DAQmx_Val_DegR                                                    10145 // Deg R
#define DAQmx_Val_Kelvins                                                 10325 // Kelvins
#define DAQmx_Val_Strain                                                  10299 // Strain
#define DAQmx_Val_Ohms                                                    10384 // Ohms
#define DAQmx_Val_Hz                                                      10373 // Hz
#define DAQmx_Val_Seconds                                                 10364 // Seconds
#define DAQmx_Val_Meters                                                  10219 // Meters
#define DAQmx_Val_Inches                                                  10379 // Inches
#define DAQmx_Val_Degrees                                                 10146 // Degrees
#define DAQmx_Val_Radians                                                 10273 // Radians
#define DAQmx_Val_g                                                       10186 // g
#define DAQmx_Val_MetersPerSecondSquared                                  12470 // m/s^2
#define DAQmx_Val_Pascals                                                 10081 // Pascals
#define DAQmx_Val_FromTEDS                                                12516 // From TEDS

//*** Values for DAQmx_AI_Voltage_Units ***
//*** Value set VoltageUnits1 ***
#define DAQmx_Val_Volts                                                   10348 // Volts
#define DAQmx_Val_FromCustomScale                                         10065 // From Custom Scale
#define DAQmx_Val_FromTEDS                                                12516 // From TEDS

//*** Values for DAQmx_AO_Voltage_Units ***
//*** Value set VoltageUnits2 ***
#define DAQmx_Val_Volts                                                   10348 // Volts
#define DAQmx_Val_FromCustomScale                                         10065 // From Custom Scale

//*** Values for DAQmx_Read_WaitMode ***
//*** Value set WaitMode ***
#define DAQmx_Val_WaitForInterrupt                                        12523 // Wait For Interrupt
#define DAQmx_Val_Poll                                                    12524 // Poll
#define DAQmx_Val_Yield                                                   12525 // Yield
#define DAQmx_Val_Sleep                                                   12547 // Sleep

//*** Values for DAQmx_Write_WaitMode ***
//*** Value set WaitMode2 ***
#define DAQmx_Val_Poll                                                    12524 // Poll
#define DAQmx_Val_Yield                                                   12525 // Yield
#define DAQmx_Val_Sleep                                                   12547 // Sleep

//*** Values for DAQmx_RealTime_WaitForNextSampClkWaitMode ***
//*** Value set WaitMode3 ***
#define DAQmx_Val_WaitForInterrupt                                        12523 // Wait For Interrupt
#define DAQmx_Val_Poll                                                    12524 // Poll

//*** Values for DAQmx_RealTime_WriteRecoveryMode ***
//*** Value set WaitMode4 ***
#define DAQmx_Val_WaitForInterrupt                                        12523 // Wait For Interrupt
#define DAQmx_Val_Poll                                                    12524 // Poll

//*** Values for DAQmx_AnlgWin_StartTrig_When ***
//*** Values for DAQmx_AnlgWin_RefTrig_When ***
//*** Value set WindowTriggerCondition1 ***
#define DAQmx_Val_EnteringWin                                             10163 // Entering Window
#define DAQmx_Val_LeavingWin                                              10208 // Leaving Window

//*** Values for DAQmx_AnlgWin_PauseTrig_When ***
//*** Value set WindowTriggerCondition2 ***
#define DAQmx_Val_InsideWin                                               10199 // Inside Window
#define DAQmx_Val_OutsideWin                                              10251 // Outside Window

//*** Value set WriteBasicTEDSOptions ***
#define DAQmx_Val_WriteToEEPROM                                           12538 // Write To EEPROM
#define DAQmx_Val_WriteToPROM                                             12539 // Write To PROM Once
#define DAQmx_Val_DoNotWrite                                              12540 // Do Not Write

//*** Values for DAQmx_Write_RelativeTo ***
//*** Value set WriteRelativeTo ***
#define DAQmx_Val_FirstSample                                             10424 // First Sample
#define DAQmx_Val_CurrWritePos                                            10430 // Current Write Position



// Switch Topologies
#define DAQmx_Val_Switch_Topology_1127_1_Wire_64x1_Mux            "1127/1-Wire 64x1 Mux"              // 1127/1-Wire 64x1 Mux
#define DAQmx_Val_Switch_Topology_1127_2_Wire_32x1_Mux            "1127/2-Wire 32x1 Mux"              // 1127/2-Wire 32x1 Mux
#define DAQmx_Val_Switch_Topology_1127_2_Wire_4x8_Matrix          "1127/2-Wire 4x8 Matrix"            // 1127/2-Wire 4x8 Matrix
#define DAQmx_Val_Switch_Topology_1127_4_Wire_16x1_Mux            "1127/4-Wire 16x1 Mux"              // 1127/4-Wire 16x1 Mux
#define DAQmx_Val_Switch_Topology_1127_Independent                "1127/Independent"                  // 1127/Independent
#define DAQmx_Val_Switch_Topology_1128_1_Wire_64x1_Mux            "1128/1-Wire 64x1 Mux"              // 1128/1-Wire 64x1 Mux
#define DAQmx_Val_Switch_Topology_1128_2_Wire_32x1_Mux            "1128/2-Wire 32x1 Mux"              // 1128/2-Wire 32x1 Mux
#define DAQmx_Val_Switch_Topology_1128_2_Wire_4x8_Matrix          "1128/2-Wire 4x8 Matrix"            // 1128/2-Wire 4x8 Matrix
#define DAQmx_Val_Switch_Topology_1128_4_Wire_16x1_Mux            "1128/4-Wire 16x1 Mux"              // 1128/4-Wire 16x1 Mux
#define DAQmx_Val_Switch_Topology_1128_Independent                "1128/Independent"                  // 1128/Independent
#define DAQmx_Val_Switch_Topology_1129_2_Wire_16x16_Matrix        "1129/2-Wire 16x16 Matrix"          // 1129/2-Wire 16x16 Matrix
#define DAQmx_Val_Switch_Topology_1129_2_Wire_8x32_Matrix         "1129/2-Wire 8x32 Matrix"           // 1129/2-Wire 8x32 Matrix
#define DAQmx_Val_Switch_Topology_1129_2_Wire_4x64_Matrix         "1129/2-Wire 4x64 Matrix"           // 1129/2-Wire 4x64 Matrix
#define DAQmx_Val_Switch_Topology_1129_2_Wire_Dual_8x16_Matrix    "1129/2-Wire Dual 8x16 Matrix"      // 1129/2-Wire Dual 8x16 Matrix
#define DAQmx_Val_Switch_Topology_1129_2_Wire_Dual_4x32_Matrix    "1129/2-Wire Dual 4x32 Matrix"      // 1129/2-Wire Dual 4x32 Matrix
#define DAQmx_Val_Switch_Topology_1129_2_Wire_Quad_4x16_Matrix    "1129/2-Wire Quad 4x16 Matrix"      // 1129/2-Wire Quad 4x16 Matrix
#define DAQmx_Val_Switch_Topology_1130_1_Wire_256x1_Mux           "1130/1-Wire 256x1 Mux"             // 1130/1-Wire 256x1 Mux
#define DAQmx_Val_Switch_Topology_1130_1_Wire_Dual_128x1_Mux      "1130/1-Wire Dual 128x1 Mux"        // 1130/1-Wire Dual 128x1 Mux
#define DAQmx_Val_Switch_Topology_1130_2_Wire_128x1_Mux           "1130/2-Wire 128x1 Mux"             // 1130/2-Wire 128x1 Mux
#define DAQmx_Val_Switch_Topology_1130_4_Wire_64x1_Mux            "1130/4-Wire 64x1 Mux"              // 1130/4-Wire 64x1 Mux
#define DAQmx_Val_Switch_Topology_1130_1_Wire_4x64_Matrix         "1130/1-Wire 4x64 Matrix"           // 1130/1-Wire 4x64 Matrix
#define DAQmx_Val_Switch_Topology_1130_1_Wire_8x32_Matrix         "1130/1-Wire 8x32 Matrix"           // 1130/1-Wire 8x32 Matrix
#define DAQmx_Val_Switch_Topology_1130_1_Wire_Octal_32x1_Mux      "1130/1-Wire Octal 32x1 Mux"        // 1130/1-Wire Octal 32x1 Mux
#define DAQmx_Val_Switch_Topology_1130_1_Wire_Quad_64x1_Mux       "1130/1-Wire Quad 64x1 Mux"         // 1130/1-Wire Quad 64x1 Mux
#define DAQmx_Val_Switch_Topology_1130_1_Wire_Sixteen_16x1_Mux    "1130/1-Wire Sixteen 16x1 Mux"      // 1130/1-Wire Sixteen 16x1 Mux
#define DAQmx_Val_Switch_Topology_1130_2_Wire_4x32_Matrix         "1130/2-Wire 4x32 Matrix"           // 1130/2-Wire 4x32 Matrix
#define DAQmx_Val_Switch_Topology_1130_2_Wire_Octal_16x1_Mux      "1130/2-Wire Octal 16x1 Mux"        // 1130/2-Wire Octal 16x1 Mux
#define DAQmx_Val_Switch_Topology_1130_2_Wire_Quad_32x1_Mux       "1130/2-Wire Quad 32x1 Mux"         // 1130/2-Wire Quad 32x1 Mux
#define DAQmx_Val_Switch_Topology_1130_4_Wire_Quad_16x1_Mux       "1130/4-Wire Quad 16x1 Mux"         // 1130/4-Wire Quad 16x1 Mux
#define DAQmx_Val_Switch_Topology_1130_Independent                "1130/Independent"                  // 1130/Independent
#define DAQmx_Val_Switch_Topology_1160_16_SPDT                    "1160/16-SPDT"                      // 1160/16-SPDT
#define DAQmx_Val_Switch_Topology_1161_8_SPDT                     "1161/8-SPDT"                       // 1161/8-SPDT
#define DAQmx_Val_Switch_Topology_1163R_Octal_4x1_Mux             "1163R/Octal 4x1 Mux"               // 1163R/Octal 4x1 Mux
#define DAQmx_Val_Switch_Topology_1166_32_SPDT                    "1166/32-SPDT"                      // 1166/32-SPDT
#define DAQmx_Val_Switch_Topology_1167_Independent                "1167/Independent"                  // 1167/Independent
#define DAQmx_Val_Switch_Topology_1169_100_SPST                   "1169/100-SPST"                     // 1169/100-SPST
#define DAQmx_Val_Switch_Topology_1175_1_Wire_196x1_Mux           "1175/1-Wire 196x1 Mux"             // 1175/1-Wire 196x1 Mux
#define DAQmx_Val_Switch_Topology_1175_2_Wire_98x1_Mux            "1175/2-Wire 98x1 Mux"              // 1175/2-Wire 98x1 Mux
#define DAQmx_Val_Switch_Topology_1175_2_Wire_95x1_Mux            "1175/2-Wire 95x1 Mux"              // 1175/2-Wire 95x1 Mux
#define DAQmx_Val_Switch_Topology_1190_Quad_4x1_Mux               "1190/Quad 4x1 Mux"                 // 1190/Quad 4x1 Mux
#define DAQmx_Val_Switch_Topology_1191_Quad_4x1_Mux               "1191/Quad 4x1 Mux"                 // 1191/Quad 4x1 Mux
#define DAQmx_Val_Switch_Topology_1192_8_SPDT                     "1192/8-SPDT"                       // 1192/8-SPDT
#define DAQmx_Val_Switch_Topology_1193_32x1_Mux                   "1193/32x1 Mux"                     // 1193/32x1 Mux
#define DAQmx_Val_Switch_Topology_1193_Dual_16x1_Mux              "1193/Dual 16x1 Mux"                // 1193/Dual 16x1 Mux
#define DAQmx_Val_Switch_Topology_1193_Quad_8x1_Mux               "1193/Quad 8x1 Mux"                 // 1193/Quad 8x1 Mux
#define DAQmx_Val_Switch_Topology_1193_16x1_Terminated_Mux        "1193/16x1 Terminated Mux"          // 1193/16x1 Terminated Mux
#define DAQmx_Val_Switch_Topology_1193_Dual_8x1_Terminated_Mux    "1193/Dual 8x1 Terminated Mux"      // 1193/Dual 8x1 Terminated Mux
#define DAQmx_Val_Switch_Topology_1193_Quad_4x1_Terminated_Mux    "1193/Quad 4x1 Terminated Mux"      // 1193/Quad 4x1 Terminated Mux
#define DAQmx_Val_Switch_Topology_1193_Independent                "1193/Independent"                  // 1193/Independent
#define DAQmx_Val_Switch_Topology_1194_Quad_4x1_Mux               "1194/Quad 4x1 Mux"                 // 1194/Quad 4x1 Mux
#define DAQmx_Val_Switch_Topology_1195_Quad_4x1_Mux               "1195/Quad 4x1 Mux"                 // 1195/Quad 4x1 Mux
#define DAQmx_Val_Switch_Topology_2501_1_Wire_48x1_Mux            "2501/1-Wire 48x1 Mux"              // 2501/1-Wire 48x1 Mux
#define DAQmx_Val_Switch_Topology_2501_1_Wire_48x1_Amplified_Mux  "2501/1-Wire 48x1 Amplified Mux"    // 2501/1-Wire 48x1 Amplified Mux
#define DAQmx_Val_Switch_Topology_2501_2_Wire_24x1_Mux            "2501/2-Wire 24x1 Mux"              // 2501/2-Wire 24x1 Mux
#define DAQmx_Val_Switch_Topology_2501_2_Wire_24x1_Amplified_Mux  "2501/2-Wire 24x1 Amplified Mux"    // 2501/2-Wire 24x1 Amplified Mux
#define DAQmx_Val_Switch_Topology_2501_2_Wire_Dual_12x1_Mux       "2501/2-Wire Dual 12x1 Mux"         // 2501/2-Wire Dual 12x1 Mux
#define DAQmx_Val_Switch_Topology_2501_2_Wire_Quad_6x1_Mux        "2501/2-Wire Quad 6x1 Mux"          // 2501/2-Wire Quad 6x1 Mux
#define DAQmx_Val_Switch_Topology_2501_2_Wire_4x6_Matrix          "2501/2-Wire 4x6 Matrix"            // 2501/2-Wire 4x6 Matrix
#define DAQmx_Val_Switch_Topology_2501_4_Wire_12x1_Mux            "2501/4-Wire 12x1 Mux"              // 2501/4-Wire 12x1 Mux
#define DAQmx_Val_Switch_Topology_2503_1_Wire_48x1_Mux            "2503/1-Wire 48x1 Mux"              // 2503/1-Wire 48x1 Mux
#define DAQmx_Val_Switch_Topology_2503_2_Wire_24x1_Mux            "2503/2-Wire 24x1 Mux"              // 2503/2-Wire 24x1 Mux
#define DAQmx_Val_Switch_Topology_2503_2_Wire_Dual_12x1_Mux       "2503/2-Wire Dual 12x1 Mux"         // 2503/2-Wire Dual 12x1 Mux
#define DAQmx_Val_Switch_Topology_2503_2_Wire_Quad_6x1_Mux        "2503/2-Wire Quad 6x1 Mux"          // 2503/2-Wire Quad 6x1 Mux
#define DAQmx_Val_Switch_Topology_2503_2_Wire_4x6_Matrix          "2503/2-Wire 4x6 Matrix"            // 2503/2-Wire 4x6 Matrix
#define DAQmx_Val_Switch_Topology_2503_4_Wire_12x1_Mux            "2503/4-Wire 12x1 Mux"              // 2503/4-Wire 12x1 Mux
#define DAQmx_Val_Switch_Topology_2527_1_Wire_64x1_Mux            "2527/1-Wire 64x1 Mux"              // 2527/1-Wire 64x1 Mux
#define DAQmx_Val_Switch_Topology_2527_1_Wire_Dual_32x1_Mux       "2527/1-Wire Dual 32x1 Mux"         // 2527/1-Wire Dual 32x1 Mux
#define DAQmx_Val_Switch_Topology_2527_2_Wire_32x1_Mux            "2527/2-Wire 32x1 Mux"              // 2527/2-Wire 32x1 Mux
#define DAQmx_Val_Switch_Topology_2527_2_Wire_Dual_16x1_Mux       "2527/2-Wire Dual 16x1 Mux"         // 2527/2-Wire Dual 16x1 Mux
#define DAQmx_Val_Switch_Topology_2527_4_Wire_16x1_Mux            "2527/4-Wire 16x1 Mux"              // 2527/4-Wire 16x1 Mux
#define DAQmx_Val_Switch_Topology_2527_Independent                "2527/Independent"                  // 2527/Independent
#define DAQmx_Val_Switch_Topology_2529_2_Wire_8x16_Matrix         "2529/2-Wire 8x16 Matrix"           // 2529/2-Wire 8x16 Matrix
#define DAQmx_Val_Switch_Topology_2529_2_Wire_4x32_Matrix         "2529/2-Wire 4x32 Matrix"           // 2529/2-Wire 4x32 Matrix
#define DAQmx_Val_Switch_Topology_2529_2_Wire_Dual_4x16_Matrix    "2529/2-Wire Dual 4x16 Matrix"      // 2529/2-Wire Dual 4x16 Matrix
#define DAQmx_Val_Switch_Topology_2530_1_Wire_128x1_Mux           "2530/1-Wire 128x1 Mux"             // 2530/1-Wire 128x1 Mux
#define DAQmx_Val_Switch_Topology_2530_1_Wire_Dual_64x1_Mux       "2530/1-Wire Dual 64x1 Mux"         // 2530/1-Wire Dual 64x1 Mux
#define DAQmx_Val_Switch_Topology_2530_2_Wire_64x1_Mux            "2530/2-Wire 64x1 Mux"              // 2530/2-Wire 64x1 Mux
#define DAQmx_Val_Switch_Topology_2530_4_Wire_32x1_Mux            "2530/4-Wire 32x1 Mux"              // 2530/4-Wire 32x1 Mux
#define DAQmx_Val_Switch_Topology_2530_1_Wire_4x32_Matrix         "2530/1-Wire 4x32 Matrix"           // 2530/1-Wire 4x32 Matrix
#define DAQmx_Val_Switch_Topology_2530_1_Wire_8x16_Matrix         "2530/1-Wire 8x16 Matrix"           // 2530/1-Wire 8x16 Matrix
#define DAQmx_Val_Switch_Topology_2530_1_Wire_Octal_16x1_Mux      "2530/1-Wire Octal 16x1 Mux"        // 2530/1-Wire Octal 16x1 Mux
#define DAQmx_Val_Switch_Topology_2530_1_Wire_Quad_32x1_Mux       "2530/1-Wire Quad 32x1 Mux"         // 2530/1-Wire Quad 32x1 Mux
#define DAQmx_Val_Switch_Topology_2530_2_Wire_4x16_Matrix         "2530/2-Wire 4x16 Matrix"           // 2530/2-Wire 4x16 Matrix
#define DAQmx_Val_Switch_Topology_2530_2_Wire_Dual_32x1_Mux       "2530/2-Wire Dual 32x1 Mux"         // 2530/2-Wire Dual 32x1 Mux
#define DAQmx_Val_Switch_Topology_2530_2_Wire_Quad_16x1_Mux       "2530/2-Wire Quad 16x1 Mux"         // 2530/2-Wire Quad 16x1 Mux
#define DAQmx_Val_Switch_Topology_2530_4_Wire_Dual_16x1_Mux       "2530/4-Wire Dual 16x1 Mux"         // 2530/4-Wire Dual 16x1 Mux
#define DAQmx_Val_Switch_Topology_2530_Independent                "2530/Independent"                  // 2530/Independent
#define DAQmx_Val_Switch_Topology_2532_1_Wire_16x32_Matrix        "2532/1-Wire 16x32 Matrix"          // 2532/1-Wire 16x32 Matrix
#define DAQmx_Val_Switch_Topology_2532_1_Wire_4x128_Matrix        "2532/1-Wire 4x128 Matrix"          // 2532/1-Wire 4x128 Matrix
#define DAQmx_Val_Switch_Topology_2532_1_Wire_8x64_Matrix         "2532/1-Wire 8x64 Matrix"           // 2532/1-Wire 8x64 Matrix
#define DAQmx_Val_Switch_Topology_2532_1_Wire_Dual_16x16_Matrix   "2532/1-Wire Dual 16x16 Matrix"     // 2532/1-Wire Dual 16x16 Matrix
#define DAQmx_Val_Switch_Topology_2532_1_Wire_Dual_4x64_Matrix    "2532/1-Wire Dual 4x64 Matrix"      // 2532/1-Wire Dual 4x64 Matrix
#define DAQmx_Val_Switch_Topology_2532_1_Wire_Dual_8x32_Matrix    "2532/1-Wire Dual 8x32 Matrix"      // 2532/1-Wire Dual 8x32 Matrix
#define DAQmx_Val_Switch_Topology_2532_1_Wire_Sixteen_2x16_Matrix "2532/1-Wire Sixteen 2x16 Matrix"   // 2532/1-Wire Sixteen 2x16 Matrix
#define DAQmx_Val_Switch_Topology_2532_2_Wire_16x16_Matrix        "2532/2-Wire 16x16 Matrix"          // 2532/2-Wire 16x16 Matrix
#define DAQmx_Val_Switch_Topology_2532_2_Wire_4x64_Matrix         "2532/2-Wire 4x64 Matrix"           // 2532/2-Wire 4x64 Matrix
#define DAQmx_Val_Switch_Topology_2532_2_Wire_8x32_Matrix         "2532/2-Wire 8x32 Matrix"           // 2532/2-Wire 8x32 Matrix
#define DAQmx_Val_Switch_Topology_2564_16_SPST                    "2564/16-SPST"                      // 2564/16-SPST
#define DAQmx_Val_Switch_Topology_2565_16_SPST                    "2565/16-SPST"                      // 2565/16-SPST
#define DAQmx_Val_Switch_Topology_2566_16_SPDT                    "2566/16-SPDT"                      // 2566/16-SPDT
#define DAQmx_Val_Switch_Topology_2567_Independent                "2567/Independent"                  // 2567/Independent
#define DAQmx_Val_Switch_Topology_2568_31_SPST                    "2568/31-SPST"                      // 2568/31-SPST
#define DAQmx_Val_Switch_Topology_2569_100_SPST                   "2569/100-SPST"                     // 2569/100-SPST
#define DAQmx_Val_Switch_Topology_2570_40_SPDT                    "2570/40-SPDT"                      // 2570/40-SPDT
#define DAQmx_Val_Switch_Topology_2575_1_Wire_196x1_Mux           "2575/1-Wire 196x1 Mux"             // 2575/1-Wire 196x1 Mux
#define DAQmx_Val_Switch_Topology_2575_2_Wire_98x1_Mux            "2575/2-Wire 98x1 Mux"              // 2575/2-Wire 98x1 Mux
#define DAQmx_Val_Switch_Topology_2575_2_Wire_95x1_Mux            "2575/2-Wire 95x1 Mux"              // 2575/2-Wire 95x1 Mux
#define DAQmx_Val_Switch_Topology_2576_2_Wire_64x1_Mux            "2576/2-Wire 64x1 Mux"              // 2576/2-Wire 64x1 Mux
#define DAQmx_Val_Switch_Topology_2576_2_Wire_Dual_32x1_Mux       "2576/2-Wire Dual 32x1 Mux"         // 2576/2-Wire Dual 32x1 Mux
#define DAQmx_Val_Switch_Topology_2576_2_Wire_Octal_8x1_Mux       "2576/2-Wire Octal 8x1 Mux"         // 2576/2-Wire Octal 8x1 Mux
#define DAQmx_Val_Switch_Topology_2576_2_Wire_Quad_16x1_Mux       "2576/2-Wire Quad 16x1 Mux"         // 2576/2-Wire Quad 16x1 Mux
#define DAQmx_Val_Switch_Topology_2576_2_Wire_Sixteen_4x1_Mux     "2576/2-Wire Sixteen 4x1 Mux"       // 2576/2-Wire Sixteen 4x1 Mux
#define DAQmx_Val_Switch_Topology_2576_Independent                "2576/Independent"                  // 2576/Independent
#define DAQmx_Val_Switch_Topology_2584_1_Wire_15x1_Mux            "2584/1-Wire 15x1 Mux"              // 2584/1-Wire 15x1 Mux
#define DAQmx_Val_Switch_Topology_2585_1_Wire_10x1_Mux            "2585/1-Wire 10x1 Mux"              // 2585/1-Wire 10x1 Mux
#define DAQmx_Val_Switch_Topology_2586_10_SPST                    "2586/10-SPST"                      // 2586/10-SPST
#define DAQmx_Val_Switch_Topology_2590_4x1_Mux                    "2590/4x1 Mux"                      // 2590/4x1 Mux
#define DAQmx_Val_Switch_Topology_2591_4x1_Mux                    "2591/4x1 Mux"                      // 2591/4x1 Mux
#define DAQmx_Val_Switch_Topology_2593_16x1_Mux                   "2593/16x1 Mux"                     // 2593/16x1 Mux
#define DAQmx_Val_Switch_Topology_2593_Dual_8x1_Mux               "2593/Dual 8x1 Mux"                 // 2593/Dual 8x1 Mux
#define DAQmx_Val_Switch_Topology_2593_8x1_Terminated_Mux         "2593/8x1 Terminated Mux"           // 2593/8x1 Terminated Mux
#define DAQmx_Val_Switch_Topology_2593_Dual_4x1_Terminated_Mux    "2593/Dual 4x1 Terminated Mux"      // 2593/Dual 4x1 Terminated Mux
#define DAQmx_Val_Switch_Topology_2593_Independent                "2593/Independent"                  // 2593/Independent
#define DAQmx_Val_Switch_Topology_2594_4x1_Mux                    "2594/4x1 Mux"                      // 2594/4x1 Mux
#define DAQmx_Val_Switch_Topology_2595_4x1_Mux                    "2595/4x1 Mux"                      // 2595/4x1 Mux
#define DAQmx_Val_Switch_Topology_2596_Dual_6x1_Mux               "2596/Dual 6x1 Mux"                 // 2596/Dual 6x1 Mux
#define DAQmx_Val_Switch_Topology_2597_6x1_Terminated_Mux         "2597/6x1 Terminated Mux"           // 2597/6x1 Terminated Mux
#define DAQmx_Val_Switch_Topology_2598_Dual_Transfer              "2598/Dual Transfer"                // 2598/Dual Transfer
#define DAQmx_Val_Switch_Topology_2599_2_SPDT                     "2599/2-SPDT"                       // 2599/2-SPDT

#define	DAQmxIsReadOrWriteLate(errorCode)	0
#define	DAQmxLoadTask(taskName,taskHandle)	0
#define	DAQmxCreateTask(taskName,taskHandle)	((*(int*)taskHandle)=1)
#define	DAQmxAddGlobalChansToTask(taskHandle,channelNames)	0
#define	DAQmxStartTask(taskHandle)	0
#define	DAQmxStopTask(taskHandle)	0
#define	DAQmxClearTask(taskHandle)	0
#define	DAQmxWaitUntilTaskDone(taskHandle,timeToWait)	0
#define	DAQmxIsTaskDone(taskHandle,isTaskDone)	0
#define	DAQmxTaskControl(taskHandle,action)	0
#define	DAQmxGetNthTaskChannel(taskHandle,index,buffer,bufferSize)	0
#define	DAQmxGetTaskAttribute(taskHandle,attribute,value)	0
#define	DAQmxRegisterEveryNSamplesEvent(task,everyNsamplesEventType,nSamples,options,callbackFunction,callbackData)	0
#define	DAQmxRegisterDoneEvent(task,options,callbackFunction,callbackData)	0
#define	DAQmxRegisterSignalEvent(task,signalID,options,callbackFunction,callbackData)	0
#define	DAQmxCreateAIVoltageChan(taskHandle,physicalChannel,nameToAssignToChannel,terminalConfig,minVal,maxVal,units,customScaleName)	0
#define	DAQmxCreateAICurrentChan(taskHandle,physicalChannel,nameToAssignToChannel,terminalConfig,minVal,maxVal,units,shuntResistorLoc,extShuntResistorVal,customScaleName)	0
#define	DAQmxCreateAIThrmcplChan(taskHandle,physicalChannel,nameToAssignToChannel,minVal,maxVal,units,thermocoupleType,cjcSource,cjcVal,cjcChannel)	0
#define	DAQmxCreateAIRTDChan(taskHandle,physicalChannel,nameToAssignToChannel,minVal,maxVal,units,rtdType,resistanceConfig,currentExcitSource,currentExcitVal,r0)	0
#define	DAQmxCreateAIThrmstrChanIex(taskHandle,physicalChannel,nameToAssignToChannel,minVal,maxVal,units,resistanceConfig,currentExcitSource,currentExcitVal,a,b,c)	0
#define	DAQmxCreateAIThrmstrChanVex(taskHandle,physicalChannel,nameToAssignToChannel,minVal,maxVal,units,resistanceConfig,voltageExcitSource,voltageExcitVal,a,b,c,r1)	0
#define	DAQmxCreateAIFreqVoltageChan(taskHandle,physicalChannel,nameToAssignToChannel,minVal,maxVal,units,thresholdLevel,hysteresis,customScaleName)	0
#define	DAQmxCreateAIResistanceChan(taskHandle,physicalChannel,nameToAssignToChannel,minVal,maxVal,units,resistanceConfig,currentExcitSource,currentExcitVal,customScaleName)	0
#define	DAQmxCreateAIStrainGageChan(taskHandle,physicalChannel,nameToAssignToChannel,minVal,maxVal,units,strainConfig,voltageExcitSource,voltageExcitVal,gageFactor,initialBridgeVoltage,nominalGageResistance,poissonRatio,leadWireResistance,customScaleName)	0
#define	DAQmxCreateAIVoltageChanWithExcit(taskHandle,physicalChannel,nameToAssignToChannel,terminalConfig,minVal,maxVal,units,bridgeConfig,voltageExcitSource,voltageExcitVal,useExcitForScaling,customScaleName)	0
#define	DAQmxCreateAITempBuiltInSensorChan(taskHandle,physicalChannel,nameToAssignToChannel,units)	0
#define	DAQmxCreateAIAccelChan(taskHandle,physicalChannel,nameToAssignToChannel,terminalConfig,minVal,maxVal,units,sensitivity,sensitivityUnits,currentExcitSource,currentExcitVal,customScaleName)	0
#define	DAQmxCreateAIMicrophoneChan(taskHandle,physicalChannel,nameToAssignToChannel,terminalConfig,units,micSensitivity,maxSndPressLevel,currentExcitSource,currentExcitVal,customScaleName)	0
#define	DAQmxCreateAIPosLVDTChan(taskHandle,physicalChannel,nameToAssignToChannel,minVal,maxVal,units,sensitivity,sensitivityUnits,voltageExcitSource,voltageExcitVal,voltageExcitFreq,ACExcitWireMode,customScaleName)	0
#define	DAQmxCreateAIPosRVDTChan(taskHandle,physicalChannel,nameToAssignToChannel,minVal,maxVal,units,sensitivity,sensitivityUnits,voltageExcitSource,voltageExcitVal,voltageExcitFreq,ACExcitWireMode,customScaleName)	0
#define	DAQmxCreateAIDeviceTempChan(taskHandle,physicalChannel,nameToAssignToChannel,units)	0
#define	DAQmxCreateTEDSAIVoltageChan(taskHandle,physicalChannel,nameToAssignToChannel,terminalConfig,minVal,maxVal,units,customScaleName)	0
#define	DAQmxCreateTEDSAICurrentChan(taskHandle,physicalChannel,nameToAssignToChannel,terminalConfig,minVal,maxVal,units,shuntResistorLoc,extShuntResistorVal,customScaleName)	0
#define	DAQmxCreateTEDSAIThrmcplChan(taskHandle,physicalChannel,nameToAssignToChannel,minVal,maxVal,units,cjcSource,cjcVal,cjcChannel)	0
#define	DAQmxCreateTEDSAIRTDChan(taskHandle,physicalChannel,nameToAssignToChannel,minVal,maxVal,units,resistanceConfig,currentExcitSource,currentExcitVal)	0
#define	DAQmxCreateTEDSAIThrmstrChanIex(taskHandle,physicalChannel,nameToAssignToChannel,minVal,maxVal,units,resistanceConfig,currentExcitSource,currentExcitVal)	0
#define	DAQmxCreateTEDSAIThrmstrChanVex(taskHandle,physicalChannel,nameToAssignToChannel,minVal,maxVal,units,resistanceConfig,voltageExcitSource,voltageExcitVal,r1)	0
#define	DAQmxCreateTEDSAIResistanceChan(taskHandle,physicalChannel,nameToAssignToChannel,minVal,maxVal,units,resistanceConfig,currentExcitSource,currentExcitVal,customScaleName)	0
#define	DAQmxCreateTEDSAIStrainGageChan(taskHandle,physicalChannel,nameToAssignToChannel,minVal,maxVal,units,voltageExcitSource,voltageExcitVal,initialBridgeVoltage,leadWireResistance,customScaleName)	0
#define	DAQmxCreateTEDSAIVoltageChanWithExcit(taskHandle,physicalChannel,nameToAssignToChannel,terminalConfig,minVal,maxVal,units,voltageExcitSource,voltageExcitVal,customScaleName)	0
#define	DAQmxCreateTEDSAIAccelChan(taskHandle,physicalChannel,nameToAssignToChannel,terminalConfig,minVal,maxVal,units,currentExcitSource,currentExcitVal,customScaleName)	0
#define	DAQmxCreateTEDSAIMicrophoneChan(taskHandle,physicalChannel,nameToAssignToChannel,terminalConfig,units,maxSndPressLevel,currentExcitSource,currentExcitVal,customScaleName)	0
#define	DAQmxCreateTEDSAIPosLVDTChan(taskHandle,physicalChannel,nameToAssignToChannel,minVal,maxVal,units,voltageExcitSource,voltageExcitVal,voltageExcitFreq,ACExcitWireMode,customScaleName)	0
#define	DAQmxCreateTEDSAIPosRVDTChan(taskHandle,physicalChannel,nameToAssignToChannel,minVal,maxVal,units,voltageExcitSource,voltageExcitVal,voltageExcitFreq,ACExcitWireMode,customScaleName)	0
#define	DAQmxCreateAOVoltageChan(taskHandle,physicalChannel,nameToAssignToChannel,minVal,maxVal,units,customScaleName)	0
#define	DAQmxCreateAOCurrentChan(taskHandle,physicalChannel,nameToAssignToChannel,minVal,maxVal,units,customScaleName)	0
#define	DAQmxCreateDIChan(taskHandle,lines,nameToAssignToLines,lineGrouping)	0
#define	DAQmxCreateDOChan(taskHandle,lines,nameToAssignToLines,lineGrouping)	0
#define	DAQmxCreateCIFreqChan(taskHandle,counter,nameToAssignToChannel,minVal,maxVal,units,edge,measMethod,measTime,divisor,customScaleName)	0
#define	DAQmxCreateCIPeriodChan(taskHandle,counter,nameToAssignToChannel,minVal,maxVal,units,edge,measMethod,measTime,divisor,customScaleName)	0
#define	DAQmxCreateCICountEdgesChan(taskHandle,counter,nameToAssignToChannel,edge,initialCount,countDirection)	0
#define	DAQmxCreateCIPulseWidthChan(taskHandle,counter,nameToAssignToChannel,minVal,maxVal,units,startingEdge,customScaleName)	0
#define	DAQmxCreateCISemiPeriodChan(taskHandle,counter,nameToAssignToChannel,minVal,maxVal,units,customScaleName)	0
#define	DAQmxCreateCITwoEdgeSepChan(taskHandle,counter,nameToAssignToChannel,minVal,maxVal,units,firstEdge,secondEdge,customScaleName)	0
#define	DAQmxCreateCILinEncoderChan(taskHandle,counter,nameToAssignToChannel,decodingType,ZidxEnable,ZidxVal,ZidxPhase,units,distPerPulse,initialPos,customScaleName)	0
#define	DAQmxCreateCIAngEncoderChan(taskHandle,counter,nameToAssignToChannel,decodingType,ZidxEnable,ZidxVal,ZidxPhase,units,pulsesPerRev,initialAngle,customScaleName)	0
#define	DAQmxCreateCIGPSTimestampChan(taskHandle,counter,nameToAssignToChannel,units,syncMethod,customScaleName)	0
#define	DAQmxCreateCOPulseChanFreq(taskHandle,counter,nameToAssignToChannel,units,idleState,initialDelay,freq,dutyCycle)	0
#define	DAQmxCreateCOPulseChanTime(taskHandle,counter,nameToAssignToChannel,units,idleState,initialDelay,lowTime,highTime)	0
#define	DAQmxCreateCOPulseChanTicks(taskHandle,counter,nameToAssignToChannel,sourceTerminal,idleState,initialDelay,lowTicks,highTicks)	0
#define	DAQmxGetAIChanCalCalDate(taskHandle,channelName,year,month,day,hour,minute)	0
#define	DAQmxSetAIChanCalCalDate(taskHandle,channelName,year,month,day,hour,minute)	0
#define	DAQmxGetAIChanCalExpDate(taskHandle,channelName,year,month,day,hour,minute)	0
#define	DAQmxSetAIChanCalExpDate(taskHandle,channelName,year,month,day,hour,minute)	0
#define	DAQmxGetChanAttribute(taskHandle,channel,attribute,value)	0
#define	DAQmxSetChanAttribute(taskHandle,channel,attribute)	0
#define	DAQmxResetChanAttribute(taskHandle,channel,attribute)	0
#define	DAQmxCfgSampClkTiming(taskHandle,source,rate,activeEdge,sampleMode,sampsPerChan)	0
#define	DAQmxCfgHandshakingTiming(taskHandle,sampleMode,sampsPerChan)	0
#define	DAQmxCfgBurstHandshakingTimingImportClock(taskHandle,sampleMode,sampsPerChan,sampleClkRate,sampleClkSrc,sampleClkActiveEdge,pauseWhen,readyEventActiveLevel)	0
#define	DAQmxCfgBurstHandshakingTimingExportClock(taskHandle,sampleMode,sampsPerChan,sampleClkRate,sampleClkOutpTerm,sampleClkPulsePolarity,pauseWhen,readyEventActiveLevel)	0
#define	DAQmxCfgChangeDetectionTiming(taskHandle,risingEdgeChan,fallingEdgeChan,sampleMode,sampsPerChan)	0
#define	DAQmxCfgImplicitTiming(taskHandle,sampleMode,sampsPerChan)	0
#define	DAQmxGetTimingAttribute(taskHandle,attribute,value)	0
#define	DAQmxSetTimingAttribute(taskHandle,attribute)	0
#define	DAQmxResetTimingAttribute(taskHandle,attribute)	0
#define	DAQmxDisableStartTrig(taskHandle)	0
#define	DAQmxCfgDigEdgeStartTrig(taskHandle,triggerSource,triggerEdge)	0
#define	DAQmxCfgAnlgEdgeStartTrig(taskHandle,triggerSource,triggerSlope,triggerLevel)	0
#define	DAQmxCfgAnlgWindowStartTrig(taskHandle,triggerSource,triggerWhen,windowTop,windowBottom)	0
#define	DAQmxCfgDigPatternStartTrig(taskHandle,triggerSource,triggerPattern,triggerWhen)	0
#define	DAQmxDisableRefTrig(taskHandle)	0
#define	DAQmxCfgDigEdgeRefTrig(taskHandle,triggerSource,triggerEdge,pretriggerSamples)	0
#define	DAQmxCfgAnlgEdgeRefTrig(taskHandle,triggerSource,triggerSlope,triggerLevel,pretriggerSamples)	0
#define	DAQmxCfgAnlgWindowRefTrig(taskHandle,triggerSource,triggerWhen,windowTop,windowBottom,pretriggerSamples)	0
#define	DAQmxCfgDigPatternRefTrig(taskHandle,triggerSource,triggerPattern,triggerWhen,pretriggerSamples)	0
#define	DAQmxDisableAdvTrig(taskHandle)	0
#define	DAQmxCfgDigEdgeAdvTrig(taskHandle,triggerSource,triggerEdge)	0
#define	DAQmxGetTrigAttribute(taskHandle,attribute,value)	0
#define	DAQmxSetTrigAttribute(taskHandle,attribute)	0
#define	DAQmxResetTrigAttribute(taskHandle,attribute)	0
#define	DAQmxSendSoftwareTrigger(taskHandle,triggerID)	0
#define	DAQmxReadAnalogF64(taskHandle,numSampsPerChan,timeout,fillMode,readArray,arraySizeInSamps,sampsPerChanRead,reserved)	0
#define	DAQmxReadAnalogScalarF64(taskHandle,timeout,value,reserved)	0
#define	DAQmxReadBinaryI16(taskHandle,numSampsPerChan,timeout,fillMode,readArray,arraySizeInSamps,sampsPerChanRead,reserved)	0
#define	DAQmxReadBinaryU16(taskHandle,numSampsPerChan,timeout,fillMode,readArray,arraySizeInSamps,sampsPerChanRead,reserved)	0
#define	DAQmxReadBinaryI32(taskHandle,numSampsPerChan,timeout,fillMode,readArray,arraySizeInSamps,sampsPerChanRead,reserved)	0
#define	DAQmxReadBinaryU32(taskHandle,numSampsPerChan,timeout,fillMode,readArray,arraySizeInSamps,sampsPerChanRead,reserved)	0
#define	DAQmxReadDigitalU8(taskHandle,numSampsPerChan,timeout,fillMode,readArray,arraySizeInSamps,sampsPerChanRead,reserved)	0
#define	DAQmxReadDigitalU16(taskHandle,numSampsPerChan,timeout,fillMode,readArray,arraySizeInSamps,sampsPerChanRead,reserved)	0
#define	DAQmxReadDigitalU32(taskHandle,numSampsPerChan,timeout,fillMode,readArray,arraySizeInSamps,sampsPerChanRead,reserved)	0
#define	DAQmxReadDigitalScalarU32(taskHandle,timeout,value,reserved)	0
#define	DAQmxReadDigitalLines(taskHandle,numSampsPerChan,timeout,fillMode,readArray,arraySizeInBytes,sampsPerChanRead,numBytesPerSamp,reserved)	0
#define	DAQmxReadCounterF64(taskHandle,numSampsPerChan,timeout,readArray,arraySizeInSamps,sampsPerChanRead,reserved)	0
#define	DAQmxReadCounterU32(taskHandle,numSampsPerChan,timeout,readArray,arraySizeInSamps,sampsPerChanRead,reserved)	0
#define	DAQmxReadCounterScalarF64(taskHandle,timeout,value,reserved)	0
#define	DAQmxReadCounterScalarU32(taskHandle,timeout,value,reserved)	0
#define	DAQmxReadRaw(taskHandle,numSampsPerChan,timeout,readArray,arraySizeInBytes,sampsRead,numBytesPerSamp,reserved)	0
#define	DAQmxGetNthTaskReadChannel(taskHandle,index,buffer,bufferSize)	0
#define	DAQmxGetReadAttribute(taskHandle,attribute,value)	0
#define	DAQmxSetReadAttribute(taskHandle,attribute)	0
#define	DAQmxResetReadAttribute(taskHandle,attribute)	0
#define	DAQmxWriteAnalogF64(taskHandle,numSampsPerChan,autoStart,timeout,dataLayout,writeArray,sampsPerChanWritten,reserved)	0
#define	DAQmxWriteAnalogScalarF64(taskHandle,autoStart,timeout,value,reserved)	0
#define	DAQmxWriteBinaryI16(taskHandle,numSampsPerChan,autoStart,timeout,dataLayout,writeArray,sampsPerChanWritten,reserved)	0
#define	DAQmxWriteBinaryU16(taskHandle,numSampsPerChan,autoStart,timeout,dataLayout,writeArray,sampsPerChanWritten,reserved)	0
#define	DAQmxWriteBinaryI32(taskHandle,numSampsPerChan,autoStart,timeout,dataLayout,writeArray,sampsPerChanWritten,reserved)	0
#define	DAQmxWriteBinaryU32(taskHandle,numSampsPerChan,autoStart,timeout,dataLayout,writeArray,sampsPerChanWritten,reserved)	0
#define	DAQmxWriteDigitalU8(taskHandle,numSampsPerChan,autoStart,timeout,dataLayout,writeArray,sampsPerChanWritten,reserved)	0
#define	DAQmxWriteDigitalU16(taskHandle,numSampsPerChan,autoStart,timeout,dataLayout,writeArray,sampsPerChanWritten,reserved)	0
#define	DAQmxWriteDigitalU32(taskHandle,numSampsPerChan,autoStart,timeout,dataLayout,writeArray,sampsPerChanWritten,reserved)	0
#define	DAQmxWriteDigitalScalarU32(taskHandle,autoStart,timeout,value,reserved)	0
#define	DAQmxWriteDigitalLines(taskHandle,numSampsPerChan,autoStart,timeout,dataLayout,writeArray,sampsPerChanWritten,reserved)	0
#define	DAQmxWriteCtrFreq(taskHandle,numSampsPerChan,autoStart,timeout,dataLayout,frequency,dutyCycle,numSampsPerChanWritten,reserved)	0
#define	DAQmxWriteCtrFreqScalar(taskHandle,autoStart,timeout,frequency,dutyCycle,reserved)	0
#define	DAQmxWriteCtrTime(taskHandle,numSampsPerChan,autoStart,timeout,dataLayout,highTime,lowTime,numSampsPerChanWritten,reserved)	0
#define	DAQmxWriteCtrTimeScalar(taskHandle,autoStart,timeout,highTime,lowTime,reserved)	0
#define	DAQmxWriteCtrTicks(taskHandle,numSampsPerChan,autoStart,timeout,dataLayout,highTicks,lowTicks,numSampsPerChanWritten,reserved)	0
#define	DAQmxWriteCtrTicksScalar(taskHandle,autoStart,timeout,highTicks,lowTicks,reserved)	0
#define	DAQmxWriteRaw(taskHandle,numSamps,autoStart,timeout,writeArray,sampsPerChanWritten,reserved)	(sampsPerChanWritten?*sampsPerChanWritten=numSamps:numSamps)
#define	DAQmxGetWriteAttribute(taskHandle,attribute,value)	0
#define	DAQmxSetWriteAttribute(taskHandle,attribute)	0
#define	DAQmxResetWriteAttribute(taskHandle,attribute)	0
#define	DAQmxExportSignal(taskHandle,signalID,outputTerminal)	0
#define	DAQmxGetExportedSignalAttribute(taskHandle,attribute,value)	0
#define	DAQmxSetExportedSignalAttribute(taskHandle,attribute)	0
#define	DAQmxResetExportedSignalAttribute(taskHandle,attribute)	0
#define	DAQmxCreateLinScale(name,slope,yIntercept,preScaledUnits,scaledUnits)	0
#define	DAQmxCreateMapScale(name,prescaledMin,prescaledMax,scaledMin,scaledMax,preScaledUnits,scaledUnits)	0
#define	DAQmxCreatePolynomialScale(name,forwardCoeffs,numForwardCoeffsIn,reverseCoeffs,numReverseCoeffsIn,preScaledUnits,scaledUnits)	0
#define	DAQmxCreateTableScale(name,prescaledVals,numPrescaledValsIn,scaledVals,numScaledValsIn,preScaledUnits,scaledUnits)	0
#define	DAQmxCalculateReversePolyCoeff(forwardCoeffs,numForwardCoeffsIn,minValX,maxValX,numPointsToCompute,reversePolyOrder,reverseCoeffs)	0
#define	DAQmxGetScaleAttribute(scaleName,attribute,value)	0
#define	DAQmxSetScaleAttribute(scaleName,attribute)	0
#define	DAQmxCfgInputBuffer(taskHandle,numSampsPerChan)	0
#define	DAQmxCfgOutputBuffer(taskHandle,numSampsPerChan)	0
#define	DAQmxGetBufferAttribute(taskHandle,attribute,value)	0
#define	DAQmxSetBufferAttribute(taskHandle,attribute)	0
#define	DAQmxResetBufferAttribute(taskHandle,attribute)	0
#define	DAQmxSwitchCreateScanList(scanList,taskHandle)	0
#define	DAQmxSwitchConnect(switchChannel1,switchChannel2,waitForSettling)	0
#define	DAQmxSwitchConnectMulti(connectionList,waitForSettling)	0
#define	DAQmxSwitchDisconnect(switchChannel1,switchChannel2,waitForSettling)	0
#define	DAQmxSwitchDisconnectMulti(connectionList,waitForSettling)	0
#define	DAQmxSwitchDisconnectAll(deviceName,waitForSettling)	0
#define	DAQmxSwitchSetTopologyAndReset(deviceName,newTopology)	0
#define	DAQmxSwitchFindPath(switchChannel1,switchChannel2,path,pathBufferSize,pathStatus)	0
#define	DAQmxSwitchOpenRelays(relayList,waitForSettling)	0
#define	DAQmxSwitchCloseRelays(relayList,waitForSettling)	0
#define	DAQmxSwitchGetSingleRelayCount(relayName,count)	0
#define	DAQmxSwitchGetMultiRelayCount(relayList,count,countArraySize,numRelayCountsRead)	0
#define	DAQmxSwitchGetSingleRelayPos(relayName,relayPos)	0
#define	DAQmxSwitchGetMultiRelayPos(relayList,relayPos,relayPosArraySize,numRelayPossRead)	0
#define	DAQmxSwitchWaitForSettling(deviceName)	0
#define	DAQmxGetSwitchChanAttribute(switchChannelName,attribute,value)	0
#define	DAQmxSetSwitchChanAttribute(switchChannelName,attribute)	0
#define	DAQmxGetSwitchDeviceAttribute(deviceName,attribute,value)	0
#define	DAQmxSetSwitchDeviceAttribute(deviceName,attribute)	0
#define	DAQmxGetSwitchScanAttribute(taskHandle,attribute,value)	0
#define	DAQmxSetSwitchScanAttribute(taskHandle,attribute)	0
#define	DAQmxResetSwitchScanAttribute(taskHandle,attribute)	0
#define	DAQmxConnectTerms(sourceTerminal,destinationTerminal,signalModifiers)	0
#define	DAQmxDisconnectTerms(sourceTerminal,destinationTerminal)	0
#define	DAQmxTristateOutputTerm(outputTerminal)	0
#define	DAQmxResetDevice(deviceName)	0
#define	DAQmxGetDeviceAttribute(deviceName,attribute,value)	0
#define	DAQmxCreateWatchdogTimerTask(deviceName,taskName,taskHandle,timeout,lines,expState)	0
#define	DAQmxControlWatchdogTask(taskHandle,action)	0
#define	DAQmxGetWatchdogAttribute(taskHandle,lines,attribute,value)	0
#define	DAQmxSetWatchdogAttribute(taskHandle,lines,attribute)	0
#define	DAQmxResetWatchdogAttribute(taskHandle,lines,attribute)	0
#define	DAQmxSelfCal(deviceName)	0
#define	DAQmxPerformBridgeOffsetNullingCal(taskHandle,channel)	0
#define	DAQmxGetSelfCalLastDateAndTime(deviceName,year,month,day,hour,minute)	0
#define	DAQmxGetExtCalLastDateAndTime(deviceName,year,month,day,hour,minute)	0
#define	DAQmxRestoreLastExtCalConst(deviceName)	0
#define	DAQmxESeriesCalAdjust(calHandle,referenceVoltage)	0
#define	DAQmxMSeriesCalAdjust(calHandle,referenceVoltage)	0
#define	DAQmxSSeriesCalAdjust(calHandle,referenceVoltage)	0
#define	DAQmxSCBaseboardCalAdjust(calHandle,referenceVoltage)	0
#define	DAQmxAOSeriesCalAdjust(calHandle,referenceVoltage)	0
#define	DAQmxDeviceSupportsCal(deviceName,calSupported)	0
#define	DAQmxGetCalInfoAttribute(deviceName,attribute,value)	0
#define	DAQmxSetCalInfoAttribute(deviceName,attribute)	0
#define	DAQmxInitExtCal(deviceName,password,calHandle)	0
#define	DAQmxCloseExtCal(calHandle,action)	0
#define	DAQmxChangeExtCalPassword(deviceName,password,newPassword)	0
#define	DAQmxAdjustDSAAICal(calHandle,referenceVoltage)	0
#define	DAQmxAdjustDSAAOCal(calHandle,channel,requestedLowVoltage,actualLowVoltage,requestedHighVoltage,actualHighVoltage,gainSetting)	0
#define	DAQmxAdjustDSATimebaseCal(calHandle,referenceFrequency)	0
#define	DAQmxAdjust4204Cal(calHandle,channelNames,lowPassFreq,trackHoldEnabled,inputVal)	0
#define	DAQmxAdjust4220Cal(calHandle,channelNames,gain,inputVal)	0
#define	DAQmxAdjust4224Cal(calHandle,channelNames,gain,inputVal)	0
#define	DAQmxAdjust4225Cal(calHandle,channelNames,gain,inputVal)	0
#define	DAQmxSetup1102Cal(calHandle,channelNames,gain)	0
#define	DAQmxAdjust1102Cal(calHandle,refVoltage,measOutput)	0
#define	DAQmxSetup1125Cal(calHandle,channelNames,gain)	0
#define	DAQmxAdjust1125Cal(calHandle,refVoltage,measOutput)	0
#define	DAQmxSetup1520Cal(calHandle,channelNames,gain)	0
#define	DAQmxAdjust1520Cal(calHandle,refVoltage,measOutput)	0
#define	DAQmxConfigureTEDS(physicalChannel,filePath)	0
#define	DAQmxClearTEDS(physicalChannel)	0
#define	DAQmxWriteToTEDSFromArray(physicalChannel,bitStream,arraySize,basicTEDSOptions)	0
#define	DAQmxWriteToTEDSFromFile(physicalChannel,filePath,basicTEDSOptions)	0
#define	DAQmxGetPhysicalChanAttribute(physicalChannel,attribute,value)	0
#define	DAQmxWaitForNextSampleClock(taskHandle,timeout,isLate)	0
#define	DAQmxGetRealTimeAttribute(taskHandle,attribute,value)	0
#define	DAQmxSetRealTimeAttribute(taskHandle,attribute)	0
#define	DAQmxResetRealTimeAttribute(taskHandle,attribute)	0
#define	DAQmxSaveTask(taskHandle,saveAs,author,options)	0
#define	DAQmxSaveGlobalChan(taskHandle,channelName,saveAs,author,options)	0
#define	DAQmxSaveScale(scaleName,saveAs,author,options)	0
#define	DAQmxDeleteSavedTask(taskName)	0
#define	DAQmxDeleteSavedGlobalChan(channelName)	0
#define	DAQmxDeleteSavedScale(scaleName)	0
#define	DAQmxGetPersistedTaskAttribute(taskName,attribute,value)	0
#define	DAQmxGetPersistedChanAttribute(channel,attribute,value)	0
#define	DAQmxGetPersistedScaleAttribute(scaleName,attribute,value)	0
#define	DAQmxGetSystemInfoAttribute(attribute,value)	0
#define	DAQmxSetDigitalPowerUpStates(deviceName,channelNames,state)	0
#define	DAQmxSetAnalogPowerUpStates(deviceName,channelNames,state,channelType)	0
#define	DAQmxGetErrorString(errorCode,errorString,bufferSize)	0
#define	DAQmxGetExtendedErrorInfo(errorString,bufferSize)	0
#define	DAQmxGetBufInputBufSize(taskHandle,data)	0
#define	DAQmxSetBufInputBufSize(taskHandle,data)	0
#define	DAQmxResetBufInputBufSize(taskHandle)	0
#define	DAQmxGetBufInputOnbrdBufSize(taskHandle,data)	0
#define	DAQmxGetBufOutputBufSize(taskHandle,data)	0
#define	DAQmxSetBufOutputBufSize(taskHandle,data)	0
#define	DAQmxResetBufOutputBufSize(taskHandle)	0
#define	DAQmxGetBufOutputOnbrdBufSize(taskHandle,data)	0
#define	DAQmxSetBufOutputOnbrdBufSize(taskHandle,data)	0
#define	DAQmxResetBufOutputOnbrdBufSize(taskHandle)	0
#define	DAQmxGetSelfCalSupported(deviceName,data)	0
#define	DAQmxGetSelfCalLastTemp(deviceName,data)	0
#define	DAQmxGetExtCalRecommendedInterval(deviceName,data)	0
#define	DAQmxGetExtCalLastTemp(deviceName,data)	0
#define	DAQmxGetCalUserDefinedInfo(deviceName,data,bufferSize)	0
#define	DAQmxSetCalUserDefinedInfo(deviceName,data)	0
#define	DAQmxGetCalUserDefinedInfoMaxSize(deviceName,data)	0
#define	DAQmxGetCalDevTemp(deviceName,data)	0
#define	DAQmxGetAIMax(taskHandle,channel,data)	0
#define	DAQmxSetAIMax(taskHandle,channel,data)	0
#define	DAQmxResetAIMax(taskHandle,channel)	0
#define	DAQmxGetAIMin(taskHandle,channel,data)	0
#define	DAQmxSetAIMin(taskHandle,channel,data)	0
#define	DAQmxResetAIMin(taskHandle,channel)	0
#define	DAQmxGetAICustomScaleName(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetAICustomScaleName(taskHandle,channel,data)	0
#define	DAQmxResetAICustomScaleName(taskHandle,channel)	0
#define	DAQmxGetAIMeasType(taskHandle,channel,data)	0
#define	DAQmxGetAIVoltageUnits(taskHandle,channel,data)	0
#define	DAQmxSetAIVoltageUnits(taskHandle,channel,data)	0
#define	DAQmxResetAIVoltageUnits(taskHandle,channel)	0
#define	DAQmxGetAITempUnits(taskHandle,channel,data)	0
#define	DAQmxSetAITempUnits(taskHandle,channel,data)	0
#define	DAQmxResetAITempUnits(taskHandle,channel)	0
#define	DAQmxGetAIThrmcplType(taskHandle,channel,data)	0
#define	DAQmxSetAIThrmcplType(taskHandle,channel,data)	0
#define	DAQmxResetAIThrmcplType(taskHandle,channel)	0
#define	DAQmxGetAIThrmcplCJCSrc(taskHandle,channel,data)	0
#define	DAQmxGetAIThrmcplCJCVal(taskHandle,channel,data)	0
#define	DAQmxSetAIThrmcplCJCVal(taskHandle,channel,data)	0
#define	DAQmxResetAIThrmcplCJCVal(taskHandle,channel)	0
#define	DAQmxGetAIThrmcplCJCChan(taskHandle,channel,data,bufferSize)	0
#define	DAQmxGetAIRTDType(taskHandle,channel,data)	0
#define	DAQmxSetAIRTDType(taskHandle,channel,data)	0
#define	DAQmxResetAIRTDType(taskHandle,channel)	0
#define	DAQmxGetAIRTDR0(taskHandle,channel,data)	0
#define	DAQmxSetAIRTDR0(taskHandle,channel,data)	0
#define	DAQmxResetAIRTDR0(taskHandle,channel)	0
#define	DAQmxGetAIRTDA(taskHandle,channel,data)	0
#define	DAQmxSetAIRTDA(taskHandle,channel,data)	0
#define	DAQmxResetAIRTDA(taskHandle,channel)	0
#define	DAQmxGetAIRTDB(taskHandle,channel,data)	0
#define	DAQmxSetAIRTDB(taskHandle,channel,data)	0
#define	DAQmxResetAIRTDB(taskHandle,channel)	0
#define	DAQmxGetAIRTDC(taskHandle,channel,data)	0
#define	DAQmxSetAIRTDC(taskHandle,channel,data)	0
#define	DAQmxResetAIRTDC(taskHandle,channel)	0
#define	DAQmxGetAIThrmstrA(taskHandle,channel,data)	0
#define	DAQmxSetAIThrmstrA(taskHandle,channel,data)	0
#define	DAQmxResetAIThrmstrA(taskHandle,channel)	0
#define	DAQmxGetAIThrmstrB(taskHandle,channel,data)	0
#define	DAQmxSetAIThrmstrB(taskHandle,channel,data)	0
#define	DAQmxResetAIThrmstrB(taskHandle,channel)	0
#define	DAQmxGetAIThrmstrC(taskHandle,channel,data)	0
#define	DAQmxSetAIThrmstrC(taskHandle,channel,data)	0
#define	DAQmxResetAIThrmstrC(taskHandle,channel)	0
#define	DAQmxGetAIThrmstrR1(taskHandle,channel,data)	0
#define	DAQmxSetAIThrmstrR1(taskHandle,channel,data)	0
#define	DAQmxResetAIThrmstrR1(taskHandle,channel)	0
#define	DAQmxGetAIForceReadFromChan(taskHandle,channel,data)	0
#define	DAQmxSetAIForceReadFromChan(taskHandle,channel,data)	0
#define	DAQmxResetAIForceReadFromChan(taskHandle,channel)	0
#define	DAQmxGetAICurrentUnits(taskHandle,channel,data)	0
#define	DAQmxSetAICurrentUnits(taskHandle,channel,data)	0
#define	DAQmxResetAICurrentUnits(taskHandle,channel)	0
#define	DAQmxGetAIStrainUnits(taskHandle,channel,data)	0
#define	DAQmxSetAIStrainUnits(taskHandle,channel,data)	0
#define	DAQmxResetAIStrainUnits(taskHandle,channel)	0
#define	DAQmxGetAIStrainGageGageFactor(taskHandle,channel,data)	0
#define	DAQmxSetAIStrainGageGageFactor(taskHandle,channel,data)	0
#define	DAQmxResetAIStrainGageGageFactor(taskHandle,channel)	0
#define	DAQmxGetAIStrainGagePoissonRatio(taskHandle,channel,data)	0
#define	DAQmxSetAIStrainGagePoissonRatio(taskHandle,channel,data)	0
#define	DAQmxResetAIStrainGagePoissonRatio(taskHandle,channel)	0
#define	DAQmxGetAIStrainGageCfg(taskHandle,channel,data)	0
#define	DAQmxSetAIStrainGageCfg(taskHandle,channel,data)	0
#define	DAQmxResetAIStrainGageCfg(taskHandle,channel)	0
#define	DAQmxGetAIResistanceUnits(taskHandle,channel,data)	0
#define	DAQmxSetAIResistanceUnits(taskHandle,channel,data)	0
#define	DAQmxResetAIResistanceUnits(taskHandle,channel)	0
#define	DAQmxGetAIFreqUnits(taskHandle,channel,data)	0
#define	DAQmxSetAIFreqUnits(taskHandle,channel,data)	0
#define	DAQmxResetAIFreqUnits(taskHandle,channel)	0
#define	DAQmxGetAIFreqThreshVoltage(taskHandle,channel,data)	0
#define	DAQmxSetAIFreqThreshVoltage(taskHandle,channel,data)	0
#define	DAQmxResetAIFreqThreshVoltage(taskHandle,channel)	0
#define	DAQmxGetAIFreqHyst(taskHandle,channel,data)	0
#define	DAQmxSetAIFreqHyst(taskHandle,channel,data)	0
#define	DAQmxResetAIFreqHyst(taskHandle,channel)	0
#define	DAQmxGetAILVDTUnits(taskHandle,channel,data)	0
#define	DAQmxSetAILVDTUnits(taskHandle,channel,data)	0
#define	DAQmxResetAILVDTUnits(taskHandle,channel)	0
#define	DAQmxGetAILVDTSensitivity(taskHandle,channel,data)	0
#define	DAQmxSetAILVDTSensitivity(taskHandle,channel,data)	0
#define	DAQmxResetAILVDTSensitivity(taskHandle,channel)	0
#define	DAQmxGetAILVDTSensitivityUnits(taskHandle,channel,data)	0
#define	DAQmxSetAILVDTSensitivityUnits(taskHandle,channel,data)	0
#define	DAQmxResetAILVDTSensitivityUnits(taskHandle,channel)	0
#define	DAQmxGetAIRVDTUnits(taskHandle,channel,data)	0
#define	DAQmxSetAIRVDTUnits(taskHandle,channel,data)	0
#define	DAQmxResetAIRVDTUnits(taskHandle,channel)	0
#define	DAQmxGetAIRVDTSensitivity(taskHandle,channel,data)	0
#define	DAQmxSetAIRVDTSensitivity(taskHandle,channel,data)	0
#define	DAQmxResetAIRVDTSensitivity(taskHandle,channel)	0
#define	DAQmxGetAIRVDTSensitivityUnits(taskHandle,channel,data)	0
#define	DAQmxSetAIRVDTSensitivityUnits(taskHandle,channel,data)	0
#define	DAQmxResetAIRVDTSensitivityUnits(taskHandle,channel)	0
#define	DAQmxGetAISoundPressureMaxSoundPressureLvl(taskHandle,channel,data)	0
#define	DAQmxSetAISoundPressureMaxSoundPressureLvl(taskHandle,channel,data)	0
#define	DAQmxResetAISoundPressureMaxSoundPressureLvl(taskHandle,channel)	0
#define	DAQmxGetAISoundPressureUnits(taskHandle,channel,data)	0
#define	DAQmxSetAISoundPressureUnits(taskHandle,channel,data)	0
#define	DAQmxResetAISoundPressureUnits(taskHandle,channel)	0
#define	DAQmxGetAIMicrophoneSensitivity(taskHandle,channel,data)	0
#define	DAQmxSetAIMicrophoneSensitivity(taskHandle,channel,data)	0
#define	DAQmxResetAIMicrophoneSensitivity(taskHandle,channel)	0
#define	DAQmxGetAIAccelUnits(taskHandle,channel,data)	0
#define	DAQmxSetAIAccelUnits(taskHandle,channel,data)	0
#define	DAQmxResetAIAccelUnits(taskHandle,channel)	0
#define	DAQmxGetAIAccelSensitivity(taskHandle,channel,data)	0
#define	DAQmxSetAIAccelSensitivity(taskHandle,channel,data)	0
#define	DAQmxResetAIAccelSensitivity(taskHandle,channel)	0
#define	DAQmxGetAIAccelSensitivityUnits(taskHandle,channel,data)	0
#define	DAQmxSetAIAccelSensitivityUnits(taskHandle,channel,data)	0
#define	DAQmxResetAIAccelSensitivityUnits(taskHandle,channel)	0
#define	DAQmxGetAITEDSUnits(taskHandle,channel,data,bufferSize)	0
#define	DAQmxGetAICoupling(taskHandle,channel,data)	0
#define	DAQmxSetAICoupling(taskHandle,channel,data)	0
#define	DAQmxResetAICoupling(taskHandle,channel)	0
#define	DAQmxGetAIImpedance(taskHandle,channel,data)	0
#define	DAQmxSetAIImpedance(taskHandle,channel,data)	0
#define	DAQmxResetAIImpedance(taskHandle,channel)	0
#define	DAQmxGetAITermCfg(taskHandle,channel,data)	0
#define	DAQmxSetAITermCfg(taskHandle,channel,data)	0
#define	DAQmxResetAITermCfg(taskHandle,channel)	0
#define	DAQmxGetAIInputSrc(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetAIInputSrc(taskHandle,channel,data)	0
#define	DAQmxResetAIInputSrc(taskHandle,channel)	0
#define	DAQmxGetAIResistanceCfg(taskHandle,channel,data)	0
#define	DAQmxSetAIResistanceCfg(taskHandle,channel,data)	0
#define	DAQmxResetAIResistanceCfg(taskHandle,channel)	0
#define	DAQmxGetAILeadWireResistance(taskHandle,channel,data)	0
#define	DAQmxSetAILeadWireResistance(taskHandle,channel,data)	0
#define	DAQmxResetAILeadWireResistance(taskHandle,channel)	0
#define	DAQmxGetAIBridgeCfg(taskHandle,channel,data)	0
#define	DAQmxSetAIBridgeCfg(taskHandle,channel,data)	0
#define	DAQmxResetAIBridgeCfg(taskHandle,channel)	0
#define	DAQmxGetAIBridgeNomResistance(taskHandle,channel,data)	0
#define	DAQmxSetAIBridgeNomResistance(taskHandle,channel,data)	0
#define	DAQmxResetAIBridgeNomResistance(taskHandle,channel)	0
#define	DAQmxGetAIBridgeInitialVoltage(taskHandle,channel,data)	0
#define	DAQmxSetAIBridgeInitialVoltage(taskHandle,channel,data)	0
#define	DAQmxResetAIBridgeInitialVoltage(taskHandle,channel)	0
#define	DAQmxGetAIBridgeShuntCalEnable(taskHandle,channel,data)	0
#define	DAQmxSetAIBridgeShuntCalEnable(taskHandle,channel,data)	0
#define	DAQmxResetAIBridgeShuntCalEnable(taskHandle,channel)	0
#define	DAQmxGetAIBridgeShuntCalSelect(taskHandle,channel,data)	0
#define	DAQmxSetAIBridgeShuntCalSelect(taskHandle,channel,data)	0
#define	DAQmxResetAIBridgeShuntCalSelect(taskHandle,channel)	0
#define	DAQmxGetAIBridgeShuntCalGainAdjust(taskHandle,channel,data)	0
#define	DAQmxSetAIBridgeShuntCalGainAdjust(taskHandle,channel,data)	0
#define	DAQmxResetAIBridgeShuntCalGainAdjust(taskHandle,channel)	0
#define	DAQmxGetAIBridgeBalanceCoarsePot(taskHandle,channel,data)	0
#define	DAQmxSetAIBridgeBalanceCoarsePot(taskHandle,channel,data)	0
#define	DAQmxResetAIBridgeBalanceCoarsePot(taskHandle,channel)	0
#define	DAQmxGetAIBridgeBalanceFinePot(taskHandle,channel,data)	0
#define	DAQmxSetAIBridgeBalanceFinePot(taskHandle,channel,data)	0
#define	DAQmxResetAIBridgeBalanceFinePot(taskHandle,channel)	0
#define	DAQmxGetAICurrentShuntLoc(taskHandle,channel,data)	0
#define	DAQmxSetAICurrentShuntLoc(taskHandle,channel,data)	0
#define	DAQmxResetAICurrentShuntLoc(taskHandle,channel)	0
#define	DAQmxGetAICurrentShuntResistance(taskHandle,channel,data)	0
#define	DAQmxSetAICurrentShuntResistance(taskHandle,channel,data)	0
#define	DAQmxResetAICurrentShuntResistance(taskHandle,channel)	0
#define	DAQmxGetAIExcitSrc(taskHandle,channel,data)	0
#define	DAQmxSetAIExcitSrc(taskHandle,channel,data)	0
#define	DAQmxResetAIExcitSrc(taskHandle,channel)	0
#define	DAQmxGetAIExcitVal(taskHandle,channel,data)	0
#define	DAQmxSetAIExcitVal(taskHandle,channel,data)	0
#define	DAQmxResetAIExcitVal(taskHandle,channel)	0
#define	DAQmxGetAIExcitUseForScaling(taskHandle,channel,data)	0
#define	DAQmxSetAIExcitUseForScaling(taskHandle,channel,data)	0
#define	DAQmxResetAIExcitUseForScaling(taskHandle,channel)	0
#define	DAQmxGetAIExcitUseMultiplexed(taskHandle,channel,data)	0
#define	DAQmxSetAIExcitUseMultiplexed(taskHandle,channel,data)	0
#define	DAQmxResetAIExcitUseMultiplexed(taskHandle,channel)	0
#define	DAQmxGetAIExcitActualVal(taskHandle,channel,data)	0
#define	DAQmxSetAIExcitActualVal(taskHandle,channel,data)	0
#define	DAQmxResetAIExcitActualVal(taskHandle,channel)	0
#define	DAQmxGetAIExcitDCorAC(taskHandle,channel,data)	0
#define	DAQmxSetAIExcitDCorAC(taskHandle,channel,data)	0
#define	DAQmxResetAIExcitDCorAC(taskHandle,channel)	0
#define	DAQmxGetAIExcitVoltageOrCurrent(taskHandle,channel,data)	0
#define	DAQmxSetAIExcitVoltageOrCurrent(taskHandle,channel,data)	0
#define	DAQmxResetAIExcitVoltageOrCurrent(taskHandle,channel)	0
#define	DAQmxGetAIACExcitFreq(taskHandle,channel,data)	0
#define	DAQmxSetAIACExcitFreq(taskHandle,channel,data)	0
#define	DAQmxResetAIACExcitFreq(taskHandle,channel)	0
#define	DAQmxGetAIACExcitSyncEnable(taskHandle,channel,data)	0
#define	DAQmxSetAIACExcitSyncEnable(taskHandle,channel,data)	0
#define	DAQmxResetAIACExcitSyncEnable(taskHandle,channel)	0
#define	DAQmxGetAIACExcitWireMode(taskHandle,channel,data)	0
#define	DAQmxSetAIACExcitWireMode(taskHandle,channel,data)	0
#define	DAQmxResetAIACExcitWireMode(taskHandle,channel)	0
#define	DAQmxGetAIAtten(taskHandle,channel,data)	0
#define	DAQmxSetAIAtten(taskHandle,channel,data)	0
#define	DAQmxResetAIAtten(taskHandle,channel)	0
#define	DAQmxGetAILowpassEnable(taskHandle,channel,data)	0
#define	DAQmxSetAILowpassEnable(taskHandle,channel,data)	0
#define	DAQmxResetAILowpassEnable(taskHandle,channel)	0
#define	DAQmxGetAILowpassCutoffFreq(taskHandle,channel,data)	0
#define	DAQmxSetAILowpassCutoffFreq(taskHandle,channel,data)	0
#define	DAQmxResetAILowpassCutoffFreq(taskHandle,channel)	0
#define	DAQmxGetAILowpassSwitchCapClkSrc(taskHandle,channel,data)	0
#define	DAQmxSetAILowpassSwitchCapClkSrc(taskHandle,channel,data)	0
#define	DAQmxResetAILowpassSwitchCapClkSrc(taskHandle,channel)	0
#define	DAQmxGetAILowpassSwitchCapExtClkFreq(taskHandle,channel,data)	0
#define	DAQmxSetAILowpassSwitchCapExtClkFreq(taskHandle,channel,data)	0
#define	DAQmxResetAILowpassSwitchCapExtClkFreq(taskHandle,channel)	0
#define	DAQmxGetAILowpassSwitchCapExtClkDiv(taskHandle,channel,data)	0
#define	DAQmxSetAILowpassSwitchCapExtClkDiv(taskHandle,channel,data)	0
#define	DAQmxResetAILowpassSwitchCapExtClkDiv(taskHandle,channel)	0
#define	DAQmxGetAILowpassSwitchCapOutClkDiv(taskHandle,channel,data)	0
#define	DAQmxSetAILowpassSwitchCapOutClkDiv(taskHandle,channel,data)	0
#define	DAQmxResetAILowpassSwitchCapOutClkDiv(taskHandle,channel)	0
#define	DAQmxGetAIResolutionUnits(taskHandle,channel,data)	0
#define	DAQmxGetAIResolution(taskHandle,channel,data)	0
#define	DAQmxGetAIRawSampSize(taskHandle,channel,data)	0
#define	DAQmxGetAIRawSampJustification(taskHandle,channel,data)	0
#define	DAQmxGetAIDitherEnable(taskHandle,channel,data)	0
#define	DAQmxSetAIDitherEnable(taskHandle,channel,data)	0
#define	DAQmxResetAIDitherEnable(taskHandle,channel)	0
#define	DAQmxGetAIChanCalHasValidCalInfo(taskHandle,channel,data)	0
#define	DAQmxGetAIChanCalEnableCal(taskHandle,channel,data)	0
#define	DAQmxSetAIChanCalEnableCal(taskHandle,channel,data)	0
#define	DAQmxResetAIChanCalEnableCal(taskHandle,channel)	0
#define	DAQmxGetAIChanCalApplyCalIfExp(taskHandle,channel,data)	0
#define	DAQmxSetAIChanCalApplyCalIfExp(taskHandle,channel,data)	0
#define	DAQmxResetAIChanCalApplyCalIfExp(taskHandle,channel)	0
#define	DAQmxGetAIChanCalScaleType(taskHandle,channel,data)	0
#define	DAQmxSetAIChanCalScaleType(taskHandle,channel,data)	0
#define	DAQmxResetAIChanCalScaleType(taskHandle,channel)	0
#define	DAQmxGetAIChanCalTablePreScaledVals(taskHandle,channel,data,arraySizeInSamples)	0
#define	DAQmxSetAIChanCalTablePreScaledVals(taskHandle,channel,data,arraySizeInSamples)	0
#define	DAQmxResetAIChanCalTablePreScaledVals(taskHandle,channel)	0
#define	DAQmxGetAIChanCalTableScaledVals(taskHandle,channel,data,arraySizeInSamples)	0
#define	DAQmxSetAIChanCalTableScaledVals(taskHandle,channel,data,arraySizeInSamples)	0
#define	DAQmxResetAIChanCalTableScaledVals(taskHandle,channel)	0
#define	DAQmxGetAIChanCalPolyForwardCoeff(taskHandle,channel,data,arraySizeInSamples)	0
#define	DAQmxSetAIChanCalPolyForwardCoeff(taskHandle,channel,data,arraySizeInSamples)	0
#define	DAQmxResetAIChanCalPolyForwardCoeff(taskHandle,channel)	0
#define	DAQmxGetAIChanCalPolyReverseCoeff(taskHandle,channel,data,arraySizeInSamples)	0
#define	DAQmxSetAIChanCalPolyReverseCoeff(taskHandle,channel,data,arraySizeInSamples)	0
#define	DAQmxResetAIChanCalPolyReverseCoeff(taskHandle,channel)	0
#define	DAQmxGetAIChanCalOperatorName(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetAIChanCalOperatorName(taskHandle,channel,data)	0
#define	DAQmxResetAIChanCalOperatorName(taskHandle,channel)	0
#define	DAQmxGetAIChanCalDesc(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetAIChanCalDesc(taskHandle,channel,data)	0
#define	DAQmxResetAIChanCalDesc(taskHandle,channel)	0
#define	DAQmxGetAIChanCalVerifRefVals(taskHandle,channel,data,arraySizeInSamples)	0
#define	DAQmxSetAIChanCalVerifRefVals(taskHandle,channel,data,arraySizeInSamples)	0
#define	DAQmxResetAIChanCalVerifRefVals(taskHandle,channel)	0
#define	DAQmxGetAIChanCalVerifAcqVals(taskHandle,channel,data,arraySizeInSamples)	0
#define	DAQmxSetAIChanCalVerifAcqVals(taskHandle,channel,data,arraySizeInSamples)	0
#define	DAQmxResetAIChanCalVerifAcqVals(taskHandle,channel)	0
#define	DAQmxGetAIRngHigh(taskHandle,channel,data)	0
#define	DAQmxSetAIRngHigh(taskHandle,channel,data)	0
#define	DAQmxResetAIRngHigh(taskHandle,channel)	0
#define	DAQmxGetAIRngLow(taskHandle,channel,data)	0
#define	DAQmxSetAIRngLow(taskHandle,channel,data)	0
#define	DAQmxResetAIRngLow(taskHandle,channel)	0
#define	DAQmxGetAIGain(taskHandle,channel,data)	0
#define	DAQmxSetAIGain(taskHandle,channel,data)	0
#define	DAQmxResetAIGain(taskHandle,channel)	0
#define	DAQmxGetAISampAndHoldEnable(taskHandle,channel,data)	0
#define	DAQmxSetAISampAndHoldEnable(taskHandle,channel,data)	0
#define	DAQmxResetAISampAndHoldEnable(taskHandle,channel)	0
#define	DAQmxGetAIAutoZeroMode(taskHandle,channel,data)	0
#define	DAQmxSetAIAutoZeroMode(taskHandle,channel,data)	0
#define	DAQmxResetAIAutoZeroMode(taskHandle,channel)	0
#define	DAQmxGetAIDataXferMech(taskHandle,channel,data)	0
#define	DAQmxSetAIDataXferMech(taskHandle,channel,data)	0
#define	DAQmxResetAIDataXferMech(taskHandle,channel)	0
#define	DAQmxGetAIDataXferReqCond(taskHandle,channel,data)	0
#define	DAQmxSetAIDataXferReqCond(taskHandle,channel,data)	0
#define	DAQmxResetAIDataXferReqCond(taskHandle,channel)	0
#define	DAQmxGetAIDataXferCustomThreshold(taskHandle,channel,data)	0
#define	DAQmxSetAIDataXferCustomThreshold(taskHandle,channel,data)	0
#define	DAQmxResetAIDataXferCustomThreshold(taskHandle,channel)	0
#define	DAQmxGetAIMemMapEnable(taskHandle,channel,data)	0
#define	DAQmxSetAIMemMapEnable(taskHandle,channel,data)	0
#define	DAQmxResetAIMemMapEnable(taskHandle,channel)	0
#define	DAQmxGetAIRawDataCompressionType(taskHandle,channel,data)	0
#define	DAQmxSetAIRawDataCompressionType(taskHandle,channel,data)	0
#define	DAQmxResetAIRawDataCompressionType(taskHandle,channel)	0
#define	DAQmxGetAILossyLSBRemovalCompressedSampSize(taskHandle,channel,data)	0
#define	DAQmxSetAILossyLSBRemovalCompressedSampSize(taskHandle,channel,data)	0
#define	DAQmxResetAILossyLSBRemovalCompressedSampSize(taskHandle,channel)	0
#define	DAQmxGetAIDevScalingCoeff(taskHandle,channel,data,arraySizeInSamples)	0
#define	DAQmxGetAIEnhancedAliasRejectionEnable(taskHandle,channel,data)	0
#define	DAQmxSetAIEnhancedAliasRejectionEnable(taskHandle,channel,data)	0
#define	DAQmxResetAIEnhancedAliasRejectionEnable(taskHandle,channel)	0
#define	DAQmxGetAOMax(taskHandle,channel,data)	0
#define	DAQmxSetAOMax(taskHandle,channel,data)	0
#define	DAQmxResetAOMax(taskHandle,channel)	0
#define	DAQmxGetAOMin(taskHandle,channel,data)	0
#define	DAQmxSetAOMin(taskHandle,channel,data)	0
#define	DAQmxResetAOMin(taskHandle,channel)	0
#define	DAQmxGetAOCustomScaleName(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetAOCustomScaleName(taskHandle,channel,data)	0
#define	DAQmxResetAOCustomScaleName(taskHandle,channel)	0
#define	DAQmxGetAOOutputType(taskHandle,channel,data)	0
#define	DAQmxGetAOVoltageUnits(taskHandle,channel,data)	0
#define	DAQmxSetAOVoltageUnits(taskHandle,channel,data)	0
#define	DAQmxResetAOVoltageUnits(taskHandle,channel)	0
#define	DAQmxGetAOCurrentUnits(taskHandle,channel,data)	0
#define	DAQmxSetAOCurrentUnits(taskHandle,channel,data)	0
#define	DAQmxResetAOCurrentUnits(taskHandle,channel)	0
#define	DAQmxGetAOOutputImpedance(taskHandle,channel,data)	0
#define	DAQmxSetAOOutputImpedance(taskHandle,channel,data)	0
#define	DAQmxResetAOOutputImpedance(taskHandle,channel)	0
#define	DAQmxGetAOLoadImpedance(taskHandle,channel,data)	0
#define	DAQmxSetAOLoadImpedance(taskHandle,channel,data)	0
#define	DAQmxResetAOLoadImpedance(taskHandle,channel)	0
#define	DAQmxGetAOIdleOutputBehavior(taskHandle,channel,data)	0
#define	DAQmxSetAOIdleOutputBehavior(taskHandle,channel,data)	0
#define	DAQmxResetAOIdleOutputBehavior(taskHandle,channel)	0
#define	DAQmxGetAOTermCfg(taskHandle,channel,data)	0
#define	DAQmxSetAOTermCfg(taskHandle,channel,data)	0
#define	DAQmxResetAOTermCfg(taskHandle,channel)	0
#define	DAQmxGetAOResolutionUnits(taskHandle,channel,data)	0
#define	DAQmxSetAOResolutionUnits(taskHandle,channel,data)	0
#define	DAQmxResetAOResolutionUnits(taskHandle,channel)	0
#define	DAQmxGetAOResolution(taskHandle,channel,data)	0
#define	DAQmxGetAODACRngHigh(taskHandle,channel,data)	0
#define	DAQmxSetAODACRngHigh(taskHandle,channel,data)	0
#define	DAQmxResetAODACRngHigh(taskHandle,channel)	0
#define	DAQmxGetAODACRngLow(taskHandle,channel,data)	0
#define	DAQmxSetAODACRngLow(taskHandle,channel,data)	0
#define	DAQmxResetAODACRngLow(taskHandle,channel)	0
#define	DAQmxGetAODACRefConnToGnd(taskHandle,channel,data)	0
#define	DAQmxSetAODACRefConnToGnd(taskHandle,channel,data)	0
#define	DAQmxResetAODACRefConnToGnd(taskHandle,channel)	0
#define	DAQmxGetAODACRefAllowConnToGnd(taskHandle,channel,data)	0
#define	DAQmxSetAODACRefAllowConnToGnd(taskHandle,channel,data)	0
#define	DAQmxResetAODACRefAllowConnToGnd(taskHandle,channel)	0
#define	DAQmxGetAODACRefSrc(taskHandle,channel,data)	0
#define	DAQmxSetAODACRefSrc(taskHandle,channel,data)	0
#define	DAQmxResetAODACRefSrc(taskHandle,channel)	0
#define	DAQmxGetAODACRefExtSrc(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetAODACRefExtSrc(taskHandle,channel,data)	0
#define	DAQmxResetAODACRefExtSrc(taskHandle,channel)	0
#define	DAQmxGetAODACRefVal(taskHandle,channel,data)	0
#define	DAQmxSetAODACRefVal(taskHandle,channel,data)	0
#define	DAQmxResetAODACRefVal(taskHandle,channel)	0
#define	DAQmxGetAODACOffsetSrc(taskHandle,channel,data)	0
#define	DAQmxSetAODACOffsetSrc(taskHandle,channel,data)	0
#define	DAQmxResetAODACOffsetSrc(taskHandle,channel)	0
#define	DAQmxGetAODACOffsetExtSrc(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetAODACOffsetExtSrc(taskHandle,channel,data)	0
#define	DAQmxResetAODACOffsetExtSrc(taskHandle,channel)	0
#define	DAQmxGetAODACOffsetVal(taskHandle,channel,data)	0
#define	DAQmxSetAODACOffsetVal(taskHandle,channel,data)	0
#define	DAQmxResetAODACOffsetVal(taskHandle,channel)	0
#define	DAQmxGetAOReglitchEnable(taskHandle,channel,data)	0
#define	DAQmxSetAOReglitchEnable(taskHandle,channel,data)	0
#define	DAQmxResetAOReglitchEnable(taskHandle,channel)	0
#define	DAQmxGetAOGain(taskHandle,channel,data)	0
#define	DAQmxSetAOGain(taskHandle,channel,data)	0
#define	DAQmxResetAOGain(taskHandle,channel)	0
#define	DAQmxGetAOUseOnlyOnBrdMem(taskHandle,channel,data)	0
#define	DAQmxSetAOUseOnlyOnBrdMem(taskHandle,channel,data)	0
#define	DAQmxResetAOUseOnlyOnBrdMem(taskHandle,channel)	0
#define	DAQmxGetAODataXferMech(taskHandle,channel,data)	0
#define	DAQmxSetAODataXferMech(taskHandle,channel,data)	0
#define	DAQmxResetAODataXferMech(taskHandle,channel)	0
#define	DAQmxGetAODataXferReqCond(taskHandle,channel,data)	0
#define	DAQmxSetAODataXferReqCond(taskHandle,channel,data)	0
#define	DAQmxResetAODataXferReqCond(taskHandle,channel)	0
#define	DAQmxGetAOMemMapEnable(taskHandle,channel,data)	0
#define	DAQmxSetAOMemMapEnable(taskHandle,channel,data)	0
#define	DAQmxResetAOMemMapEnable(taskHandle,channel)	0
#define	DAQmxGetAODevScalingCoeff(taskHandle,channel,data,arraySizeInSamples)	0
#define	DAQmxGetAOEnhancedImageRejectionEnable(taskHandle,channel,data)	0
#define	DAQmxSetAOEnhancedImageRejectionEnable(taskHandle,channel,data)	0
#define	DAQmxResetAOEnhancedImageRejectionEnable(taskHandle,channel)	0
#define	DAQmxGetDIInvertLines(taskHandle,channel,data)	0
#define	DAQmxSetDIInvertLines(taskHandle,channel,data)	0
#define	DAQmxResetDIInvertLines(taskHandle,channel)	0
#define	DAQmxGetDINumLines(taskHandle,channel,data)	0
#define	DAQmxGetDIDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxSetDIDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxResetDIDigFltrEnable(taskHandle,channel)	0
#define	DAQmxGetDIDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxSetDIDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxResetDIDigFltrMinPulseWidth(taskHandle,channel)	0
#define	DAQmxGetDITristate(taskHandle,channel,data)	0
#define	DAQmxSetDITristate(taskHandle,channel,data)	0
#define	DAQmxResetDITristate(taskHandle,channel)	0
#define	DAQmxGetDIDataXferMech(taskHandle,channel,data)	0
#define	DAQmxSetDIDataXferMech(taskHandle,channel,data)	0
#define	DAQmxResetDIDataXferMech(taskHandle,channel)	0
#define	DAQmxGetDIDataXferReqCond(taskHandle,channel,data)	0
#define	DAQmxSetDIDataXferReqCond(taskHandle,channel,data)	0
#define	DAQmxResetDIDataXferReqCond(taskHandle,channel)	0
#define	DAQmxGetDOOutputDriveType(taskHandle,channel,data)	0
#define	DAQmxSetDOOutputDriveType(taskHandle,channel,data)	0
#define	DAQmxResetDOOutputDriveType(taskHandle,channel)	0
#define	DAQmxGetDOInvertLines(taskHandle,channel,data)	0
#define	DAQmxSetDOInvertLines(taskHandle,channel,data)	0
#define	DAQmxResetDOInvertLines(taskHandle,channel)	0
#define	DAQmxGetDONumLines(taskHandle,channel,data)	0
#define	DAQmxGetDOTristate(taskHandle,channel,data)	0
#define	DAQmxSetDOTristate(taskHandle,channel,data)	0
#define	DAQmxResetDOTristate(taskHandle,channel)	0
#define	DAQmxGetDOUseOnlyOnBrdMem(taskHandle,channel,data)	0
#define	DAQmxSetDOUseOnlyOnBrdMem(taskHandle,channel,data)	0
#define	DAQmxResetDOUseOnlyOnBrdMem(taskHandle,channel)	0
#define	DAQmxGetDODataXferMech(taskHandle,channel,data)	0
#define	DAQmxSetDODataXferMech(taskHandle,channel,data)	0
#define	DAQmxResetDODataXferMech(taskHandle,channel)	0
#define	DAQmxGetDODataXferReqCond(taskHandle,channel,data)	0
#define	DAQmxSetDODataXferReqCond(taskHandle,channel,data)	0
#define	DAQmxResetDODataXferReqCond(taskHandle,channel)	0
#define	DAQmxGetCIMax(taskHandle,channel,data)	0
#define	DAQmxSetCIMax(taskHandle,channel,data)	0
#define	DAQmxResetCIMax(taskHandle,channel)	0
#define	DAQmxGetCIMin(taskHandle,channel,data)	0
#define	DAQmxSetCIMin(taskHandle,channel,data)	0
#define	DAQmxResetCIMin(taskHandle,channel)	0
#define	DAQmxGetCICustomScaleName(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCICustomScaleName(taskHandle,channel,data)	0
#define	DAQmxResetCICustomScaleName(taskHandle,channel)	0
#define	DAQmxGetCIMeasType(taskHandle,channel,data)	0
#define	DAQmxGetCIFreqUnits(taskHandle,channel,data)	0
#define	DAQmxSetCIFreqUnits(taskHandle,channel,data)	0
#define	DAQmxResetCIFreqUnits(taskHandle,channel)	0
#define	DAQmxGetCIFreqTerm(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCIFreqTerm(taskHandle,channel,data)	0
#define	DAQmxResetCIFreqTerm(taskHandle,channel)	0
#define	DAQmxGetCIFreqStartingEdge(taskHandle,channel,data)	0
#define	DAQmxSetCIFreqStartingEdge(taskHandle,channel,data)	0
#define	DAQmxResetCIFreqStartingEdge(taskHandle,channel)	0
#define	DAQmxGetCIFreqMeasMeth(taskHandle,channel,data)	0
#define	DAQmxSetCIFreqMeasMeth(taskHandle,channel,data)	0
#define	DAQmxResetCIFreqMeasMeth(taskHandle,channel)	0
#define	DAQmxGetCIFreqMeasTime(taskHandle,channel,data)	0
#define	DAQmxSetCIFreqMeasTime(taskHandle,channel,data)	0
#define	DAQmxResetCIFreqMeasTime(taskHandle,channel)	0
#define	DAQmxGetCIFreqDiv(taskHandle,channel,data)	0
#define	DAQmxSetCIFreqDiv(taskHandle,channel,data)	0
#define	DAQmxResetCIFreqDiv(taskHandle,channel)	0
#define	DAQmxGetCIFreqDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxSetCIFreqDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxResetCIFreqDigFltrEnable(taskHandle,channel)	0
#define	DAQmxGetCIFreqDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxSetCIFreqDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxResetCIFreqDigFltrMinPulseWidth(taskHandle,channel)	0
#define	DAQmxGetCIFreqDigFltrTimebaseSrc(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCIFreqDigFltrTimebaseSrc(taskHandle,channel,data)	0
#define	DAQmxResetCIFreqDigFltrTimebaseSrc(taskHandle,channel)	0
#define	DAQmxGetCIFreqDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxSetCIFreqDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxResetCIFreqDigFltrTimebaseRate(taskHandle,channel)	0
#define	DAQmxGetCIFreqDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxSetCIFreqDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxResetCIFreqDigSyncEnable(taskHandle,channel)	0
#define	DAQmxGetCIPeriodUnits(taskHandle,channel,data)	0
#define	DAQmxSetCIPeriodUnits(taskHandle,channel,data)	0
#define	DAQmxResetCIPeriodUnits(taskHandle,channel)	0
#define	DAQmxGetCIPeriodTerm(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCIPeriodTerm(taskHandle,channel,data)	0
#define	DAQmxResetCIPeriodTerm(taskHandle,channel)	0
#define	DAQmxGetCIPeriodStartingEdge(taskHandle,channel,data)	0
#define	DAQmxSetCIPeriodStartingEdge(taskHandle,channel,data)	0
#define	DAQmxResetCIPeriodStartingEdge(taskHandle,channel)	0
#define	DAQmxGetCIPeriodMeasMeth(taskHandle,channel,data)	0
#define	DAQmxSetCIPeriodMeasMeth(taskHandle,channel,data)	0
#define	DAQmxResetCIPeriodMeasMeth(taskHandle,channel)	0
#define	DAQmxGetCIPeriodMeasTime(taskHandle,channel,data)	0
#define	DAQmxSetCIPeriodMeasTime(taskHandle,channel,data)	0
#define	DAQmxResetCIPeriodMeasTime(taskHandle,channel)	0
#define	DAQmxGetCIPeriodDiv(taskHandle,channel,data)	0
#define	DAQmxSetCIPeriodDiv(taskHandle,channel,data)	0
#define	DAQmxResetCIPeriodDiv(taskHandle,channel)	0
#define	DAQmxGetCIPeriodDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxSetCIPeriodDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxResetCIPeriodDigFltrEnable(taskHandle,channel)	0
#define	DAQmxGetCIPeriodDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxSetCIPeriodDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxResetCIPeriodDigFltrMinPulseWidth(taskHandle,channel)	0
#define	DAQmxGetCIPeriodDigFltrTimebaseSrc(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCIPeriodDigFltrTimebaseSrc(taskHandle,channel,data)	0
#define	DAQmxResetCIPeriodDigFltrTimebaseSrc(taskHandle,channel)	0
#define	DAQmxGetCIPeriodDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxSetCIPeriodDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxResetCIPeriodDigFltrTimebaseRate(taskHandle,channel)	0
#define	DAQmxGetCIPeriodDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxSetCIPeriodDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxResetCIPeriodDigSyncEnable(taskHandle,channel)	0
#define	DAQmxGetCICountEdgesTerm(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCICountEdgesTerm(taskHandle,channel,data)	0
#define	DAQmxResetCICountEdgesTerm(taskHandle,channel)	0
#define	DAQmxGetCICountEdgesDir(taskHandle,channel,data)	0
#define	DAQmxSetCICountEdgesDir(taskHandle,channel,data)	0
#define	DAQmxResetCICountEdgesDir(taskHandle,channel)	0
#define	DAQmxGetCICountEdgesDirTerm(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCICountEdgesDirTerm(taskHandle,channel,data)	0
#define	DAQmxResetCICountEdgesDirTerm(taskHandle,channel)	0
#define	DAQmxGetCICountEdgesCountDirDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxSetCICountEdgesCountDirDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxResetCICountEdgesCountDirDigFltrEnable(taskHandle,channel)	0
#define	DAQmxGetCICountEdgesCountDirDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxSetCICountEdgesCountDirDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxResetCICountEdgesCountDirDigFltrMinPulseWidth(taskHandle,channel)	0
#define	DAQmxGetCICountEdgesCountDirDigFltrTimebaseSrc(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCICountEdgesCountDirDigFltrTimebaseSrc(taskHandle,channel,data)	0
#define	DAQmxResetCICountEdgesCountDirDigFltrTimebaseSrc(taskHandle,channel)	0
#define	DAQmxGetCICountEdgesCountDirDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxSetCICountEdgesCountDirDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxResetCICountEdgesCountDirDigFltrTimebaseRate(taskHandle,channel)	0
#define	DAQmxGetCICountEdgesCountDirDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxSetCICountEdgesCountDirDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxResetCICountEdgesCountDirDigSyncEnable(taskHandle,channel)	0
#define	DAQmxGetCICountEdgesInitialCnt(taskHandle,channel,data)	0
#define	DAQmxSetCICountEdgesInitialCnt(taskHandle,channel,data)	0
#define	DAQmxResetCICountEdgesInitialCnt(taskHandle,channel)	0
#define	DAQmxGetCICountEdgesActiveEdge(taskHandle,channel,data)	0
#define	DAQmxSetCICountEdgesActiveEdge(taskHandle,channel,data)	0
#define	DAQmxResetCICountEdgesActiveEdge(taskHandle,channel)	0
#define	DAQmxGetCICountEdgesDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxSetCICountEdgesDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxResetCICountEdgesDigFltrEnable(taskHandle,channel)	0
#define	DAQmxGetCICountEdgesDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxSetCICountEdgesDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxResetCICountEdgesDigFltrMinPulseWidth(taskHandle,channel)	0
#define	DAQmxGetCICountEdgesDigFltrTimebaseSrc(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCICountEdgesDigFltrTimebaseSrc(taskHandle,channel,data)	0
#define	DAQmxResetCICountEdgesDigFltrTimebaseSrc(taskHandle,channel)	0
#define	DAQmxGetCICountEdgesDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxSetCICountEdgesDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxResetCICountEdgesDigFltrTimebaseRate(taskHandle,channel)	0
#define	DAQmxGetCICountEdgesDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxSetCICountEdgesDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxResetCICountEdgesDigSyncEnable(taskHandle,channel)	0
#define	DAQmxGetCIAngEncoderUnits(taskHandle,channel,data)	0
#define	DAQmxSetCIAngEncoderUnits(taskHandle,channel,data)	0
#define	DAQmxResetCIAngEncoderUnits(taskHandle,channel)	0
#define	DAQmxGetCIAngEncoderPulsesPerRev(taskHandle,channel,data)	0
#define	DAQmxSetCIAngEncoderPulsesPerRev(taskHandle,channel,data)	0
#define	DAQmxResetCIAngEncoderPulsesPerRev(taskHandle,channel)	0
#define	DAQmxGetCIAngEncoderInitialAngle(taskHandle,channel,data)	0
#define	DAQmxSetCIAngEncoderInitialAngle(taskHandle,channel,data)	0
#define	DAQmxResetCIAngEncoderInitialAngle(taskHandle,channel)	0
#define	DAQmxGetCILinEncoderUnits(taskHandle,channel,data)	0
#define	DAQmxSetCILinEncoderUnits(taskHandle,channel,data)	0
#define	DAQmxResetCILinEncoderUnits(taskHandle,channel)	0
#define	DAQmxGetCILinEncoderDistPerPulse(taskHandle,channel,data)	0
#define	DAQmxSetCILinEncoderDistPerPulse(taskHandle,channel,data)	0
#define	DAQmxResetCILinEncoderDistPerPulse(taskHandle,channel)	0
#define	DAQmxGetCILinEncoderInitialPos(taskHandle,channel,data)	0
#define	DAQmxSetCILinEncoderInitialPos(taskHandle,channel,data)	0
#define	DAQmxResetCILinEncoderInitialPos(taskHandle,channel)	0
#define	DAQmxGetCIEncoderDecodingType(taskHandle,channel,data)	0
#define	DAQmxSetCIEncoderDecodingType(taskHandle,channel,data)	0
#define	DAQmxResetCIEncoderDecodingType(taskHandle,channel)	0
#define	DAQmxGetCIEncoderAInputTerm(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCIEncoderAInputTerm(taskHandle,channel,data)	0
#define	DAQmxResetCIEncoderAInputTerm(taskHandle,channel)	0
#define	DAQmxGetCIEncoderAInputDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxSetCIEncoderAInputDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxResetCIEncoderAInputDigFltrEnable(taskHandle,channel)	0
#define	DAQmxGetCIEncoderAInputDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxSetCIEncoderAInputDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxResetCIEncoderAInputDigFltrMinPulseWidth(taskHandle,channel)	0
#define	DAQmxGetCIEncoderAInputDigFltrTimebaseSrc(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCIEncoderAInputDigFltrTimebaseSrc(taskHandle,channel,data)	0
#define	DAQmxResetCIEncoderAInputDigFltrTimebaseSrc(taskHandle,channel)	0
#define	DAQmxGetCIEncoderAInputDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxSetCIEncoderAInputDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxResetCIEncoderAInputDigFltrTimebaseRate(taskHandle,channel)	0
#define	DAQmxGetCIEncoderAInputDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxSetCIEncoderAInputDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxResetCIEncoderAInputDigSyncEnable(taskHandle,channel)	0
#define	DAQmxGetCIEncoderBInputTerm(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCIEncoderBInputTerm(taskHandle,channel,data)	0
#define	DAQmxResetCIEncoderBInputTerm(taskHandle,channel)	0
#define	DAQmxGetCIEncoderBInputDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxSetCIEncoderBInputDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxResetCIEncoderBInputDigFltrEnable(taskHandle,channel)	0
#define	DAQmxGetCIEncoderBInputDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxSetCIEncoderBInputDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxResetCIEncoderBInputDigFltrMinPulseWidth(taskHandle,channel)	0
#define	DAQmxGetCIEncoderBInputDigFltrTimebaseSrc(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCIEncoderBInputDigFltrTimebaseSrc(taskHandle,channel,data)	0
#define	DAQmxResetCIEncoderBInputDigFltrTimebaseSrc(taskHandle,channel)	0
#define	DAQmxGetCIEncoderBInputDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxSetCIEncoderBInputDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxResetCIEncoderBInputDigFltrTimebaseRate(taskHandle,channel)	0
#define	DAQmxGetCIEncoderBInputDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxSetCIEncoderBInputDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxResetCIEncoderBInputDigSyncEnable(taskHandle,channel)	0
#define	DAQmxGetCIEncoderZInputTerm(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCIEncoderZInputTerm(taskHandle,channel,data)	0
#define	DAQmxResetCIEncoderZInputTerm(taskHandle,channel)	0
#define	DAQmxGetCIEncoderZInputDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxSetCIEncoderZInputDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxResetCIEncoderZInputDigFltrEnable(taskHandle,channel)	0
#define	DAQmxGetCIEncoderZInputDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxSetCIEncoderZInputDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxResetCIEncoderZInputDigFltrMinPulseWidth(taskHandle,channel)	0
#define	DAQmxGetCIEncoderZInputDigFltrTimebaseSrc(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCIEncoderZInputDigFltrTimebaseSrc(taskHandle,channel,data)	0
#define	DAQmxResetCIEncoderZInputDigFltrTimebaseSrc(taskHandle,channel)	0
#define	DAQmxGetCIEncoderZInputDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxSetCIEncoderZInputDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxResetCIEncoderZInputDigFltrTimebaseRate(taskHandle,channel)	0
#define	DAQmxGetCIEncoderZInputDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxSetCIEncoderZInputDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxResetCIEncoderZInputDigSyncEnable(taskHandle,channel)	0
#define	DAQmxGetCIEncoderZIndexEnable(taskHandle,channel,data)	0
#define	DAQmxSetCIEncoderZIndexEnable(taskHandle,channel,data)	0
#define	DAQmxResetCIEncoderZIndexEnable(taskHandle,channel)	0
#define	DAQmxGetCIEncoderZIndexVal(taskHandle,channel,data)	0
#define	DAQmxSetCIEncoderZIndexVal(taskHandle,channel,data)	0
#define	DAQmxResetCIEncoderZIndexVal(taskHandle,channel)	0
#define	DAQmxGetCIEncoderZIndexPhase(taskHandle,channel,data)	0
#define	DAQmxSetCIEncoderZIndexPhase(taskHandle,channel,data)	0
#define	DAQmxResetCIEncoderZIndexPhase(taskHandle,channel)	0
#define	DAQmxGetCIPulseWidthUnits(taskHandle,channel,data)	0
#define	DAQmxSetCIPulseWidthUnits(taskHandle,channel,data)	0
#define	DAQmxResetCIPulseWidthUnits(taskHandle,channel)	0
#define	DAQmxGetCIPulseWidthTerm(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCIPulseWidthTerm(taskHandle,channel,data)	0
#define	DAQmxResetCIPulseWidthTerm(taskHandle,channel)	0
#define	DAQmxGetCIPulseWidthStartingEdge(taskHandle,channel,data)	0
#define	DAQmxSetCIPulseWidthStartingEdge(taskHandle,channel,data)	0
#define	DAQmxResetCIPulseWidthStartingEdge(taskHandle,channel)	0
#define	DAQmxGetCIPulseWidthDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxSetCIPulseWidthDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxResetCIPulseWidthDigFltrEnable(taskHandle,channel)	0
#define	DAQmxGetCIPulseWidthDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxSetCIPulseWidthDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxResetCIPulseWidthDigFltrMinPulseWidth(taskHandle,channel)	0
#define	DAQmxGetCIPulseWidthDigFltrTimebaseSrc(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCIPulseWidthDigFltrTimebaseSrc(taskHandle,channel,data)	0
#define	DAQmxResetCIPulseWidthDigFltrTimebaseSrc(taskHandle,channel)	0
#define	DAQmxGetCIPulseWidthDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxSetCIPulseWidthDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxResetCIPulseWidthDigFltrTimebaseRate(taskHandle,channel)	0
#define	DAQmxGetCIPulseWidthDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxSetCIPulseWidthDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxResetCIPulseWidthDigSyncEnable(taskHandle,channel)	0
#define	DAQmxGetCITwoEdgeSepUnits(taskHandle,channel,data)	0
#define	DAQmxSetCITwoEdgeSepUnits(taskHandle,channel,data)	0
#define	DAQmxResetCITwoEdgeSepUnits(taskHandle,channel)	0
#define	DAQmxGetCITwoEdgeSepFirstTerm(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCITwoEdgeSepFirstTerm(taskHandle,channel,data)	0
#define	DAQmxResetCITwoEdgeSepFirstTerm(taskHandle,channel)	0
#define	DAQmxGetCITwoEdgeSepFirstEdge(taskHandle,channel,data)	0
#define	DAQmxSetCITwoEdgeSepFirstEdge(taskHandle,channel,data)	0
#define	DAQmxResetCITwoEdgeSepFirstEdge(taskHandle,channel)	0
#define	DAQmxGetCITwoEdgeSepFirstDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxSetCITwoEdgeSepFirstDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxResetCITwoEdgeSepFirstDigFltrEnable(taskHandle,channel)	0
#define	DAQmxGetCITwoEdgeSepFirstDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxSetCITwoEdgeSepFirstDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxResetCITwoEdgeSepFirstDigFltrMinPulseWidth(taskHandle,channel)	0
#define	DAQmxGetCITwoEdgeSepFirstDigFltrTimebaseSrc(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCITwoEdgeSepFirstDigFltrTimebaseSrc(taskHandle,channel,data)	0
#define	DAQmxResetCITwoEdgeSepFirstDigFltrTimebaseSrc(taskHandle,channel)	0
#define	DAQmxGetCITwoEdgeSepFirstDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxSetCITwoEdgeSepFirstDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxResetCITwoEdgeSepFirstDigFltrTimebaseRate(taskHandle,channel)	0
#define	DAQmxGetCITwoEdgeSepFirstDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxSetCITwoEdgeSepFirstDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxResetCITwoEdgeSepFirstDigSyncEnable(taskHandle,channel)	0
#define	DAQmxGetCITwoEdgeSepSecondTerm(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCITwoEdgeSepSecondTerm(taskHandle,channel,data)	0
#define	DAQmxResetCITwoEdgeSepSecondTerm(taskHandle,channel)	0
#define	DAQmxGetCITwoEdgeSepSecondEdge(taskHandle,channel,data)	0
#define	DAQmxSetCITwoEdgeSepSecondEdge(taskHandle,channel,data)	0
#define	DAQmxResetCITwoEdgeSepSecondEdge(taskHandle,channel)	0
#define	DAQmxGetCITwoEdgeSepSecondDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxSetCITwoEdgeSepSecondDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxResetCITwoEdgeSepSecondDigFltrEnable(taskHandle,channel)	0
#define	DAQmxGetCITwoEdgeSepSecondDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxSetCITwoEdgeSepSecondDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxResetCITwoEdgeSepSecondDigFltrMinPulseWidth(taskHandle,channel)	0
#define	DAQmxGetCITwoEdgeSepSecondDigFltrTimebaseSrc(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCITwoEdgeSepSecondDigFltrTimebaseSrc(taskHandle,channel,data)	0
#define	DAQmxResetCITwoEdgeSepSecondDigFltrTimebaseSrc(taskHandle,channel)	0
#define	DAQmxGetCITwoEdgeSepSecondDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxSetCITwoEdgeSepSecondDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxResetCITwoEdgeSepSecondDigFltrTimebaseRate(taskHandle,channel)	0
#define	DAQmxGetCITwoEdgeSepSecondDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxSetCITwoEdgeSepSecondDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxResetCITwoEdgeSepSecondDigSyncEnable(taskHandle,channel)	0
#define	DAQmxGetCISemiPeriodUnits(taskHandle,channel,data)	0
#define	DAQmxSetCISemiPeriodUnits(taskHandle,channel,data)	0
#define	DAQmxResetCISemiPeriodUnits(taskHandle,channel)	0
#define	DAQmxGetCISemiPeriodTerm(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCISemiPeriodTerm(taskHandle,channel,data)	0
#define	DAQmxResetCISemiPeriodTerm(taskHandle,channel)	0
#define	DAQmxGetCISemiPeriodStartingEdge(taskHandle,channel,data)	0
#define	DAQmxSetCISemiPeriodStartingEdge(taskHandle,channel,data)	0
#define	DAQmxResetCISemiPeriodStartingEdge(taskHandle,channel)	0
#define	DAQmxGetCISemiPeriodDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxSetCISemiPeriodDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxResetCISemiPeriodDigFltrEnable(taskHandle,channel)	0
#define	DAQmxGetCISemiPeriodDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxSetCISemiPeriodDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxResetCISemiPeriodDigFltrMinPulseWidth(taskHandle,channel)	0
#define	DAQmxGetCISemiPeriodDigFltrTimebaseSrc(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCISemiPeriodDigFltrTimebaseSrc(taskHandle,channel,data)	0
#define	DAQmxResetCISemiPeriodDigFltrTimebaseSrc(taskHandle,channel)	0
#define	DAQmxGetCISemiPeriodDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxSetCISemiPeriodDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxResetCISemiPeriodDigFltrTimebaseRate(taskHandle,channel)	0
#define	DAQmxGetCISemiPeriodDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxSetCISemiPeriodDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxResetCISemiPeriodDigSyncEnable(taskHandle,channel)	0
#define	DAQmxGetCITimestampUnits(taskHandle,channel,data)	0
#define	DAQmxSetCITimestampUnits(taskHandle,channel,data)	0
#define	DAQmxResetCITimestampUnits(taskHandle,channel)	0
#define	DAQmxGetCITimestampInitialSeconds(taskHandle,channel,data)	0
#define	DAQmxSetCITimestampInitialSeconds(taskHandle,channel,data)	0
#define	DAQmxResetCITimestampInitialSeconds(taskHandle,channel)	0
#define	DAQmxGetCIGPSSyncMethod(taskHandle,channel,data)	0
#define	DAQmxSetCIGPSSyncMethod(taskHandle,channel,data)	0
#define	DAQmxResetCIGPSSyncMethod(taskHandle,channel)	0
#define	DAQmxGetCIGPSSyncSrc(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCIGPSSyncSrc(taskHandle,channel,data)	0
#define	DAQmxResetCIGPSSyncSrc(taskHandle,channel)	0
#define	DAQmxGetCICtrTimebaseSrc(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCICtrTimebaseSrc(taskHandle,channel,data)	0
#define	DAQmxResetCICtrTimebaseSrc(taskHandle,channel)	0
#define	DAQmxGetCICtrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxSetCICtrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxResetCICtrTimebaseRate(taskHandle,channel)	0
#define	DAQmxGetCICtrTimebaseActiveEdge(taskHandle,channel,data)	0
#define	DAQmxSetCICtrTimebaseActiveEdge(taskHandle,channel,data)	0
#define	DAQmxResetCICtrTimebaseActiveEdge(taskHandle,channel)	0
#define	DAQmxGetCICtrTimebaseDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxSetCICtrTimebaseDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxResetCICtrTimebaseDigFltrEnable(taskHandle,channel)	0
#define	DAQmxGetCICtrTimebaseDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxSetCICtrTimebaseDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxResetCICtrTimebaseDigFltrMinPulseWidth(taskHandle,channel)	0
#define	DAQmxGetCICtrTimebaseDigFltrTimebaseSrc(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCICtrTimebaseDigFltrTimebaseSrc(taskHandle,channel,data)	0
#define	DAQmxResetCICtrTimebaseDigFltrTimebaseSrc(taskHandle,channel)	0
#define	DAQmxGetCICtrTimebaseDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxSetCICtrTimebaseDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxResetCICtrTimebaseDigFltrTimebaseRate(taskHandle,channel)	0
#define	DAQmxGetCICtrTimebaseDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxSetCICtrTimebaseDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxResetCICtrTimebaseDigSyncEnable(taskHandle,channel)	0
#define	DAQmxGetCICount(taskHandle,channel,data)	0
#define	DAQmxGetCIOutputState(taskHandle,channel,data)	0
#define	DAQmxGetCITCReached(taskHandle,channel,data)	0
#define	DAQmxGetCICtrTimebaseMasterTimebaseDiv(taskHandle,channel,data)	0
#define	DAQmxSetCICtrTimebaseMasterTimebaseDiv(taskHandle,channel,data)	0
#define	DAQmxResetCICtrTimebaseMasterTimebaseDiv(taskHandle,channel)	0
#define	DAQmxGetCIDataXferMech(taskHandle,channel,data)	0
#define	DAQmxSetCIDataXferMech(taskHandle,channel,data)	0
#define	DAQmxResetCIDataXferMech(taskHandle,channel)	0
#define	DAQmxGetCINumPossiblyInvalidSamps(taskHandle,channel,data)	0
#define	DAQmxGetCIDupCountPrevent(taskHandle,channel,data)	0
#define	DAQmxSetCIDupCountPrevent(taskHandle,channel,data)	0
#define	DAQmxResetCIDupCountPrevent(taskHandle,channel)	0
#define	DAQmxGetCIPrescaler(taskHandle,channel,data)	0
#define	DAQmxSetCIPrescaler(taskHandle,channel,data)	0
#define	DAQmxResetCIPrescaler(taskHandle,channel)	0
#define	DAQmxGetCOOutputType(taskHandle,channel,data)	0
#define	DAQmxGetCOPulseIdleState(taskHandle,channel,data)	0
#define	DAQmxSetCOPulseIdleState(taskHandle,channel,data)	0
#define	DAQmxResetCOPulseIdleState(taskHandle,channel)	0
#define	DAQmxGetCOPulseTerm(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCOPulseTerm(taskHandle,channel,data)	0
#define	DAQmxResetCOPulseTerm(taskHandle,channel)	0
#define	DAQmxGetCOPulseTimeUnits(taskHandle,channel,data)	0
#define	DAQmxSetCOPulseTimeUnits(taskHandle,channel,data)	0
#define	DAQmxResetCOPulseTimeUnits(taskHandle,channel)	0
#define	DAQmxGetCOPulseHighTime(taskHandle,channel,data)	0
#define	DAQmxSetCOPulseHighTime(taskHandle,channel,data)	0
#define	DAQmxResetCOPulseHighTime(taskHandle,channel)	0
#define	DAQmxGetCOPulseLowTime(taskHandle,channel,data)	0
#define	DAQmxSetCOPulseLowTime(taskHandle,channel,data)	0
#define	DAQmxResetCOPulseLowTime(taskHandle,channel)	0
#define	DAQmxGetCOPulseTimeInitialDelay(taskHandle,channel,data)	0
#define	DAQmxSetCOPulseTimeInitialDelay(taskHandle,channel,data)	0
#define	DAQmxResetCOPulseTimeInitialDelay(taskHandle,channel)	0
#define	DAQmxGetCOPulseDutyCyc(taskHandle,channel,data)	0
#define	DAQmxSetCOPulseDutyCyc(taskHandle,channel,data)	0
#define	DAQmxResetCOPulseDutyCyc(taskHandle,channel)	0
#define	DAQmxGetCOPulseFreqUnits(taskHandle,channel,data)	0
#define	DAQmxSetCOPulseFreqUnits(taskHandle,channel,data)	0
#define	DAQmxResetCOPulseFreqUnits(taskHandle,channel)	0
#define	DAQmxGetCOPulseFreq(taskHandle,channel,data)	0
#define	DAQmxSetCOPulseFreq(taskHandle,channel,data)	0
#define	DAQmxResetCOPulseFreq(taskHandle,channel)	0
#define	DAQmxGetCOPulseFreqInitialDelay(taskHandle,channel,data)	0
#define	DAQmxSetCOPulseFreqInitialDelay(taskHandle,channel,data)	0
#define	DAQmxResetCOPulseFreqInitialDelay(taskHandle,channel)	0
#define	DAQmxGetCOPulseHighTicks(taskHandle,channel,data)	0
#define	DAQmxSetCOPulseHighTicks(taskHandle,channel,data)	0
#define	DAQmxResetCOPulseHighTicks(taskHandle,channel)	0
#define	DAQmxGetCOPulseLowTicks(taskHandle,channel,data)	0
#define	DAQmxSetCOPulseLowTicks(taskHandle,channel,data)	0
#define	DAQmxResetCOPulseLowTicks(taskHandle,channel)	0
#define	DAQmxGetCOPulseTicksInitialDelay(taskHandle,channel,data)	0
#define	DAQmxSetCOPulseTicksInitialDelay(taskHandle,channel,data)	0
#define	DAQmxResetCOPulseTicksInitialDelay(taskHandle,channel)	0
#define	DAQmxGetCOCtrTimebaseSrc(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCOCtrTimebaseSrc(taskHandle,channel,data)	0
#define	DAQmxResetCOCtrTimebaseSrc(taskHandle,channel)	0
#define	DAQmxGetCOCtrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxSetCOCtrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxResetCOCtrTimebaseRate(taskHandle,channel)	0
#define	DAQmxGetCOCtrTimebaseActiveEdge(taskHandle,channel,data)	0
#define	DAQmxSetCOCtrTimebaseActiveEdge(taskHandle,channel,data)	0
#define	DAQmxResetCOCtrTimebaseActiveEdge(taskHandle,channel)	0
#define	DAQmxGetCOCtrTimebaseDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxSetCOCtrTimebaseDigFltrEnable(taskHandle,channel,data)	0
#define	DAQmxResetCOCtrTimebaseDigFltrEnable(taskHandle,channel)	0
#define	DAQmxGetCOCtrTimebaseDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxSetCOCtrTimebaseDigFltrMinPulseWidth(taskHandle,channel,data)	0
#define	DAQmxResetCOCtrTimebaseDigFltrMinPulseWidth(taskHandle,channel)	0
#define	DAQmxGetCOCtrTimebaseDigFltrTimebaseSrc(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetCOCtrTimebaseDigFltrTimebaseSrc(taskHandle,channel,data)	0
#define	DAQmxResetCOCtrTimebaseDigFltrTimebaseSrc(taskHandle,channel)	0
#define	DAQmxGetCOCtrTimebaseDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxSetCOCtrTimebaseDigFltrTimebaseRate(taskHandle,channel,data)	0
#define	DAQmxResetCOCtrTimebaseDigFltrTimebaseRate(taskHandle,channel)	0
#define	DAQmxGetCOCtrTimebaseDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxSetCOCtrTimebaseDigSyncEnable(taskHandle,channel,data)	0
#define	DAQmxResetCOCtrTimebaseDigSyncEnable(taskHandle,channel)	0
#define	DAQmxGetCOCount(taskHandle,channel,data)	0
#define	DAQmxGetCOOutputState(taskHandle,channel,data)	0
#define	DAQmxGetCOAutoIncrCnt(taskHandle,channel,data)	0
#define	DAQmxSetCOAutoIncrCnt(taskHandle,channel,data)	0
#define	DAQmxResetCOAutoIncrCnt(taskHandle,channel)	0
#define	DAQmxGetCOCtrTimebaseMasterTimebaseDiv(taskHandle,channel,data)	0
#define	DAQmxSetCOCtrTimebaseMasterTimebaseDiv(taskHandle,channel,data)	0
#define	DAQmxResetCOCtrTimebaseMasterTimebaseDiv(taskHandle,channel)	0
#define	DAQmxGetCOPulseDone(taskHandle,channel,data)	0
#define	DAQmxGetCOPrescaler(taskHandle,channel,data)	0
#define	DAQmxSetCOPrescaler(taskHandle,channel,data)	0
#define	DAQmxResetCOPrescaler(taskHandle,channel)	0
#define	DAQmxGetCORdyForNewVal(taskHandle,channel,data)	0
#define	DAQmxGetChanType(taskHandle,channel,data)	0
#define	DAQmxGetPhysicalChanName(taskHandle,channel,data,bufferSize)	0
#define	DAQmxGetChanDescr(taskHandle,channel,data,bufferSize)	0
#define	DAQmxSetChanDescr(taskHandle,channel,data)	0
#define	DAQmxResetChanDescr(taskHandle,channel)	0
#define	DAQmxGetChanIsGlobal(taskHandle,channel,data)	0
#define	DAQmxGetExportedAIConvClkOutputTerm(taskHandle,data,bufferSize)	0
#define	DAQmxSetExportedAIConvClkOutputTerm(taskHandle,data)	0
#define	DAQmxResetExportedAIConvClkOutputTerm(taskHandle)	0
#define	DAQmxGetExportedAIConvClkPulsePolarity(taskHandle,data)	0
#define	DAQmxGetExported10MHzRefClkOutputTerm(taskHandle,data,bufferSize)	0
#define	DAQmxSetExported10MHzRefClkOutputTerm(taskHandle,data)	0
#define	DAQmxResetExported10MHzRefClkOutputTerm(taskHandle)	0
#define	DAQmxGetExported20MHzTimebaseOutputTerm(taskHandle,data,bufferSize)	0
#define	DAQmxSetExported20MHzTimebaseOutputTerm(taskHandle,data)	0
#define	DAQmxResetExported20MHzTimebaseOutputTerm(taskHandle)	0
#define	DAQmxGetExportedSampClkOutputBehavior(taskHandle,data)	0
#define	DAQmxSetExportedSampClkOutputBehavior(taskHandle,data)	0
#define	DAQmxResetExportedSampClkOutputBehavior(taskHandle)	0
#define	DAQmxGetExportedSampClkOutputTerm(taskHandle,data,bufferSize)	0
#define	DAQmxSetExportedSampClkOutputTerm(taskHandle,data)	0
#define	DAQmxResetExportedSampClkOutputTerm(taskHandle)	0
#define	DAQmxGetExportedSampClkPulsePolarity(taskHandle,data)	0
#define	DAQmxSetExportedSampClkPulsePolarity(taskHandle,data)	0
#define	DAQmxResetExportedSampClkPulsePolarity(taskHandle)	0
#define	DAQmxGetExportedSampClkTimebaseOutputTerm(taskHandle,data,bufferSize)	0
#define	DAQmxSetExportedSampClkTimebaseOutputTerm(taskHandle,data)	0
#define	DAQmxResetExportedSampClkTimebaseOutputTerm(taskHandle)	0
#define	DAQmxGetExportedDividedSampClkTimebaseOutputTerm(taskHandle,data,bufferSize)	0
#define	DAQmxSetExportedDividedSampClkTimebaseOutputTerm(taskHandle,data)	0
#define	DAQmxResetExportedDividedSampClkTimebaseOutputTerm(taskHandle)	0
#define	DAQmxGetExportedAdvTrigOutputTerm(taskHandle,data,bufferSize)	0
#define	DAQmxSetExportedAdvTrigOutputTerm(taskHandle,data)	0
#define	DAQmxResetExportedAdvTrigOutputTerm(taskHandle)	0
#define	DAQmxGetExportedAdvTrigPulsePolarity(taskHandle,data)	0
#define	DAQmxGetExportedAdvTrigPulseWidthUnits(taskHandle,data)	0
#define	DAQmxSetExportedAdvTrigPulseWidthUnits(taskHandle,data)	0
#define	DAQmxResetExportedAdvTrigPulseWidthUnits(taskHandle)	0
#define	DAQmxGetExportedAdvTrigPulseWidth(taskHandle,data)	0
#define	DAQmxSetExportedAdvTrigPulseWidth(taskHandle,data)	0
#define	DAQmxResetExportedAdvTrigPulseWidth(taskHandle)	0
#define	DAQmxGetExportedRefTrigOutputTerm(taskHandle,data,bufferSize)	0
#define	DAQmxSetExportedRefTrigOutputTerm(taskHandle,data)	0
#define	DAQmxResetExportedRefTrigOutputTerm(taskHandle)	0
#define	DAQmxGetExportedStartTrigOutputTerm(taskHandle,data,bufferSize)	0
#define	DAQmxSetExportedStartTrigOutputTerm(taskHandle,data)	0
#define	DAQmxResetExportedStartTrigOutputTerm(taskHandle)	0
#define	DAQmxGetExportedAdvCmpltEventOutputTerm(taskHandle,data,bufferSize)	0
#define	DAQmxSetExportedAdvCmpltEventOutputTerm(taskHandle,data)	0
#define	DAQmxResetExportedAdvCmpltEventOutputTerm(taskHandle)	0
#define	DAQmxGetExportedAdvCmpltEventDelay(taskHandle,data)	0
#define	DAQmxSetExportedAdvCmpltEventDelay(taskHandle,data)	0
#define	DAQmxResetExportedAdvCmpltEventDelay(taskHandle)	0
#define	DAQmxGetExportedAdvCmpltEventPulsePolarity(taskHandle,data)	0
#define	DAQmxSetExportedAdvCmpltEventPulsePolarity(taskHandle,data)	0
#define	DAQmxResetExportedAdvCmpltEventPulsePolarity(taskHandle)	0
#define	DAQmxGetExportedAdvCmpltEventPulseWidth(taskHandle,data)	0
#define	DAQmxSetExportedAdvCmpltEventPulseWidth(taskHandle,data)	0
#define	DAQmxResetExportedAdvCmpltEventPulseWidth(taskHandle)	0
#define	DAQmxGetExportedAIHoldCmpltEventOutputTerm(taskHandle,data,bufferSize)	0
#define	DAQmxSetExportedAIHoldCmpltEventOutputTerm(taskHandle,data)	0
#define	DAQmxResetExportedAIHoldCmpltEventOutputTerm(taskHandle)	0
#define	DAQmxGetExportedAIHoldCmpltEventPulsePolarity(taskHandle,data)	0
#define	DAQmxSetExportedAIHoldCmpltEventPulsePolarity(taskHandle,data)	0
#define	DAQmxResetExportedAIHoldCmpltEventPulsePolarity(taskHandle)	0
#define	DAQmxGetExportedChangeDetectEventOutputTerm(taskHandle,data,bufferSize)	0
#define	DAQmxSetExportedChangeDetectEventOutputTerm(taskHandle,data)	0
#define	DAQmxResetExportedChangeDetectEventOutputTerm(taskHandle)	0
#define	DAQmxGetExportedChangeDetectEventPulsePolarity(taskHandle,data)	0
#define	DAQmxSetExportedChangeDetectEventPulsePolarity(taskHandle,data)	0
#define	DAQmxResetExportedChangeDetectEventPulsePolarity(taskHandle)	0
#define	DAQmxGetExportedCtrOutEventOutputTerm(taskHandle,data,bufferSize)	0
#define	DAQmxSetExportedCtrOutEventOutputTerm(taskHandle,data)	0
#define	DAQmxResetExportedCtrOutEventOutputTerm(taskHandle)	0
#define	DAQmxGetExportedCtrOutEventOutputBehavior(taskHandle,data)	0
#define	DAQmxSetExportedCtrOutEventOutputBehavior(taskHandle,data)	0
#define	DAQmxResetExportedCtrOutEventOutputBehavior(taskHandle)	0
#define	DAQmxGetExportedCtrOutEventPulsePolarity(taskHandle,data)	0
#define	DAQmxSetExportedCtrOutEventPulsePolarity(taskHandle,data)	0
#define	DAQmxResetExportedCtrOutEventPulsePolarity(taskHandle)	0
#define	DAQmxGetExportedCtrOutEventToggleIdleState(taskHandle,data)	0
#define	DAQmxSetExportedCtrOutEventToggleIdleState(taskHandle,data)	0
#define	DAQmxResetExportedCtrOutEventToggleIdleState(taskHandle)	0
#define	DAQmxGetExportedHshkEventOutputTerm(taskHandle,data,bufferSize)	0
#define	DAQmxSetExportedHshkEventOutputTerm(taskHandle,data)	0
#define	DAQmxResetExportedHshkEventOutputTerm(taskHandle)	0
#define	DAQmxGetExportedHshkEventOutputBehavior(taskHandle,data)	0
#define	DAQmxSetExportedHshkEventOutputBehavior(taskHandle,data)	0
#define	DAQmxResetExportedHshkEventOutputBehavior(taskHandle)	0
#define	DAQmxGetExportedHshkEventDelay(taskHandle,data)	0
#define	DAQmxSetExportedHshkEventDelay(taskHandle,data)	0
#define	DAQmxResetExportedHshkEventDelay(taskHandle)	0
#define	DAQmxGetExportedHshkEventInterlockedAssertedLvl(taskHandle,data)	0
#define	DAQmxSetExportedHshkEventInterlockedAssertedLvl(taskHandle,data)	0
#define	DAQmxResetExportedHshkEventInterlockedAssertedLvl(taskHandle)	0
#define	DAQmxGetExportedHshkEventInterlockedAssertOnStart(taskHandle,data)	0
#define	DAQmxSetExportedHshkEventInterlockedAssertOnStart(taskHandle,data)	0
#define	DAQmxResetExportedHshkEventInterlockedAssertOnStart(taskHandle)	0
#define	DAQmxGetExportedHshkEventInterlockedDeassertDelay(taskHandle,data)	0
#define	DAQmxSetExportedHshkEventInterlockedDeassertDelay(taskHandle,data)	0
#define	DAQmxResetExportedHshkEventInterlockedDeassertDelay(taskHandle)	0
#define	DAQmxGetExportedHshkEventPulsePolarity(taskHandle,data)	0
#define	DAQmxSetExportedHshkEventPulsePolarity(taskHandle,data)	0
#define	DAQmxResetExportedHshkEventPulsePolarity(taskHandle)	0
#define	DAQmxGetExportedHshkEventPulseWidth(taskHandle,data)	0
#define	DAQmxSetExportedHshkEventPulseWidth(taskHandle,data)	0
#define	DAQmxResetExportedHshkEventPulseWidth(taskHandle)	0
#define	DAQmxGetExportedRdyForXferEventOutputTerm(taskHandle,data,bufferSize)	0
#define	DAQmxSetExportedRdyForXferEventOutputTerm(taskHandle,data)	0
#define	DAQmxResetExportedRdyForXferEventOutputTerm(taskHandle)	0
#define	DAQmxGetExportedRdyForXferEventLvlActiveLvl(taskHandle,data)	0
#define	DAQmxSetExportedRdyForXferEventLvlActiveLvl(taskHandle,data)	0
#define	DAQmxResetExportedRdyForXferEventLvlActiveLvl(taskHandle)	0
#define	DAQmxGetExportedSyncPulseEventOutputTerm(taskHandle,data,bufferSize)	0
#define	DAQmxSetExportedSyncPulseEventOutputTerm(taskHandle,data)	0
#define	DAQmxResetExportedSyncPulseEventOutputTerm(taskHandle)	0
#define	DAQmxGetExportedWatchdogExpiredEventOutputTerm(taskHandle,data,bufferSize)	0
#define	DAQmxSetExportedWatchdogExpiredEventOutputTerm(taskHandle,data)	0
#define	DAQmxResetExportedWatchdogExpiredEventOutputTerm(taskHandle)	0
#define	DAQmxGetDevIsSimulated(device,data)	0
#define	DAQmxGetDevProductType(device,data,bufferSize)	0
#define	DAQmxGetDevProductNum(device,data)	0
#define	DAQmxGetDevSerialNum(device,data)	((*data) = 1)
#define	DAQmxGetDevAIPhysicalChans(device,data,bufferSize)	0
#define	DAQmxGetDevAOPhysicalChans(device,data,bufferSize)	0
#define	DAQmxGetDevDILines(device,data,bufferSize)	0
#define	DAQmxGetDevDIPorts(device,data,bufferSize)	0
#define	DAQmxGetDevDOLines(device,data,bufferSize)	0
#define	DAQmxGetDevDOPorts(device,data,bufferSize)	0
#define	DAQmxGetDevCIPhysicalChans(device,data,bufferSize)	0
#define	DAQmxGetDevCOPhysicalChans(device,data,bufferSize)	0
#define	DAQmxGetDevBusType(device,data)	0
#define	DAQmxGetDevPCIBusNum(device,data)	0
#define	DAQmxGetDevPCIDevNum(device,data)	0
#define	DAQmxGetDevPXIChassisNum(device,data)	0
#define	DAQmxGetDevPXISlotNum(device,data)	0
#define	DAQmxGetReadRelativeTo(taskHandle,data)	0
#define	DAQmxSetReadRelativeTo(taskHandle,data)	0
#define	DAQmxResetReadRelativeTo(taskHandle)	0
#define	DAQmxGetReadOffset(taskHandle,data)	0
#define	DAQmxSetReadOffset(taskHandle,data)	0
#define	DAQmxResetReadOffset(taskHandle)	0
#define	DAQmxGetReadChannelsToRead(taskHandle,data,bufferSize)	0
#define	DAQmxSetReadChannelsToRead(taskHandle,data)	0
#define	DAQmxResetReadChannelsToRead(taskHandle)	0
#define	DAQmxGetReadReadAllAvailSamp(taskHandle,data)	0
#define	DAQmxSetReadReadAllAvailSamp(taskHandle,data)	0
#define	DAQmxResetReadReadAllAvailSamp(taskHandle)	0
#define	DAQmxGetReadAutoStart(taskHandle,data)	0
#define	DAQmxSetReadAutoStart(taskHandle,data)	0
#define	DAQmxResetReadAutoStart(taskHandle)	0
#define	DAQmxGetReadOverWrite(taskHandle,data)	0
#define	DAQmxSetReadOverWrite(taskHandle,data)	0
#define	DAQmxResetReadOverWrite(taskHandle)	0
#define	DAQmxGetReadCurrReadPos(taskHandle,data)	0
#define	DAQmxGetReadAvailSampPerChan(taskHandle,data)	0
#define	DAQmxGetReadTotalSampPerChanAcquired(taskHandle,data)	0
#define	DAQmxGetReadOverloadedChansExist(taskHandle,data)	0
#define	DAQmxGetReadOverloadedChans(taskHandle,data,bufferSize)	0
#define	DAQmxGetReadChangeDetectHasOverflowed(taskHandle,data)	0
#define	DAQmxGetReadRawDataWidth(taskHandle,data)	0
#define	DAQmxGetReadNumChans(taskHandle,data)	0
#define	DAQmxGetReadDigitalLinesBytesPerChan(taskHandle,data)	0
#define	DAQmxGetReadWaitMode(taskHandle,data)	0
#define	DAQmxSetReadWaitMode(taskHandle,data)	0
#define	DAQmxResetReadWaitMode(taskHandle)	0
#define	DAQmxGetReadSleepTime(taskHandle,data)	0
#define	DAQmxSetReadSleepTime(taskHandle,data)	0
#define	DAQmxResetReadSleepTime(taskHandle)	0
#define	DAQmxGetRealTimeConvLateErrorsToWarnings(taskHandle,data)	0
#define	DAQmxSetRealTimeConvLateErrorsToWarnings(taskHandle,data)	0
#define	DAQmxResetRealTimeConvLateErrorsToWarnings(taskHandle)	0
#define	DAQmxGetRealTimeNumOfWarmupIters(taskHandle,data)	0
#define	DAQmxSetRealTimeNumOfWarmupIters(taskHandle,data)	0
#define	DAQmxResetRealTimeNumOfWarmupIters(taskHandle)	0
#define	DAQmxGetRealTimeWaitForNextSampClkWaitMode(taskHandle,data)	0
#define	DAQmxSetRealTimeWaitForNextSampClkWaitMode(taskHandle,data)	0
#define	DAQmxResetRealTimeWaitForNextSampClkWaitMode(taskHandle)	0
#define	DAQmxGetRealTimeReportMissedSamp(taskHandle,data)	0
#define	DAQmxSetRealTimeReportMissedSamp(taskHandle,data)	0
#define	DAQmxResetRealTimeReportMissedSamp(taskHandle)	0
#define	DAQmxGetRealTimeWriteRecoveryMode(taskHandle,data)	0
#define	DAQmxSetRealTimeWriteRecoveryMode(taskHandle,data)	0
#define	DAQmxResetRealTimeWriteRecoveryMode(taskHandle)	0
#define	DAQmxGetSwitchChanUsage(switchChannelName,data)	0
#define	DAQmxSetSwitchChanUsage(switchChannelName,data)	0
#define	DAQmxGetSwitchChanMaxACCarryCurrent(switchChannelName,data)	0
#define	DAQmxGetSwitchChanMaxACSwitchCurrent(switchChannelName,data)	0
#define	DAQmxGetSwitchChanMaxACCarryPwr(switchChannelName,data)	0
#define	DAQmxGetSwitchChanMaxACSwitchPwr(switchChannelName,data)	0
#define	DAQmxGetSwitchChanMaxDCCarryCurrent(switchChannelName,data)	0
#define	DAQmxGetSwitchChanMaxDCSwitchCurrent(switchChannelName,data)	0
#define	DAQmxGetSwitchChanMaxDCCarryPwr(switchChannelName,data)	0
#define	DAQmxGetSwitchChanMaxDCSwitchPwr(switchChannelName,data)	0
#define	DAQmxGetSwitchChanMaxACVoltage(switchChannelName,data)	0
#define	DAQmxGetSwitchChanMaxDCVoltage(switchChannelName,data)	0
#define	DAQmxGetSwitchChanWireMode(switchChannelName,data)	0
#define	DAQmxGetSwitchChanBandwidth(switchChannelName,data)	0
#define	DAQmxGetSwitchChanImpedance(switchChannelName,data)	0
#define	DAQmxGetSwitchDevSettlingTime(deviceName,data)	0
#define	DAQmxSetSwitchDevSettlingTime(deviceName,data)	0
#define	DAQmxGetSwitchDevAutoConnAnlgBus(deviceName,data)	0
#define	DAQmxSetSwitchDevAutoConnAnlgBus(deviceName,data)	0
#define	DAQmxGetSwitchDevPwrDownLatchRelaysAfterSettling(deviceName,data)	0
#define	DAQmxSetSwitchDevPwrDownLatchRelaysAfterSettling(deviceName,data)	0
#define	DAQmxGetSwitchDevSettled(deviceName,data)	0
#define	DAQmxGetSwitchDevRelayList(deviceName,data,bufferSize)	0
#define	DAQmxGetSwitchDevNumRelays(deviceName,data)	0
#define	DAQmxGetSwitchDevSwitchChanList(deviceName,data,bufferSize)	0
#define	DAQmxGetSwitchDevNumSwitchChans(deviceName,data)	0
#define	DAQmxGetSwitchDevNumRows(deviceName,data)	0
#define	DAQmxGetSwitchDevNumColumns(deviceName,data)	0
#define	DAQmxGetSwitchDevTopology(deviceName,data,bufferSize)	0
#define	DAQmxGetSwitchScanBreakMode(taskHandle,data)	0
#define	DAQmxSetSwitchScanBreakMode(taskHandle,data)	0
#define	DAQmxResetSwitchScanBreakMode(taskHandle)	0
#define	DAQmxGetSwitchScanRepeatMode(taskHandle,data)	0
#define	DAQmxSetSwitchScanRepeatMode(taskHandle,data)	0
#define	DAQmxResetSwitchScanRepeatMode(taskHandle)	0
#define	DAQmxGetSwitchScanWaitingForAdv(taskHandle,data)	0
#define	DAQmxGetScaleDescr(scaleName,data,bufferSize)	0
#define	DAQmxSetScaleDescr(scaleName,data)	0
#define	DAQmxGetScaleScaledUnits(scaleName,data,bufferSize)	0
#define	DAQmxSetScaleScaledUnits(scaleName,data)	0
#define	DAQmxGetScalePreScaledUnits(scaleName,data)	0
#define	DAQmxSetScalePreScaledUnits(scaleName,data)	0
#define	DAQmxGetScaleType(scaleName,data)	0
#define	DAQmxGetScaleLinSlope(scaleName,data)	0
#define	DAQmxSetScaleLinSlope(scaleName,data)	0
#define	DAQmxGetScaleLinYIntercept(scaleName,data)	0
#define	DAQmxSetScaleLinYIntercept(scaleName,data)	0
#define	DAQmxGetScaleMapScaledMax(scaleName,data)	0
#define	DAQmxSetScaleMapScaledMax(scaleName,data)	0
#define	DAQmxGetScaleMapPreScaledMax(scaleName,data)	0
#define	DAQmxSetScaleMapPreScaledMax(scaleName,data)	0
#define	DAQmxGetScaleMapScaledMin(scaleName,data)	0
#define	DAQmxSetScaleMapScaledMin(scaleName,data)	0
#define	DAQmxGetScaleMapPreScaledMin(scaleName,data)	0
#define	DAQmxSetScaleMapPreScaledMin(scaleName,data)	0
#define	DAQmxGetScalePolyForwardCoeff(scaleName,data,arraySizeInSamples)	0
#define	DAQmxSetScalePolyForwardCoeff(scaleName,data,arraySizeInSamples)	0
#define	DAQmxGetScalePolyReverseCoeff(scaleName,data,arraySizeInSamples)	0
#define	DAQmxSetScalePolyReverseCoeff(scaleName,data,arraySizeInSamples)	0
#define	DAQmxGetScaleTableScaledVals(scaleName,data,arraySizeInSamples)	0
#define	DAQmxSetScaleTableScaledVals(scaleName,data,arraySizeInSamples)	0
#define	DAQmxGetScaleTablePreScaledVals(scaleName,data,arraySizeInSamples)	0
#define	DAQmxSetScaleTablePreScaledVals(scaleName,data,arraySizeInSamples)	0
#define	DAQmxGetSysGlobalChans(data,bufferSize)	0
#define	DAQmxGetSysScales(data,bufferSize)	0
#define	DAQmxGetSysTasks(data,bufferSize)	0
#define	DAQmxGetSysDevNames(data,bufferSize)	memset(data,0,bufferSize);
#define	DAQmxGetSysNIDAQMajorVersion(data)	0
#define	DAQmxGetSysNIDAQMinorVersion(data)	0
#define	DAQmxGetTaskName(taskHandle,data,bufferSize)	0
#define	DAQmxGetTaskChannels(taskHandle,data,bufferSize)	0
#define	DAQmxGetTaskNumChans(taskHandle,data)	0
#define	DAQmxGetTaskDevices(taskHandle,data,bufferSize)	0
#define	DAQmxGetTaskComplete(taskHandle,data)	0
#define	DAQmxGetSampQuantSampMode(taskHandle,data)	0
#define	DAQmxSetSampQuantSampMode(taskHandle,data)	0
#define	DAQmxResetSampQuantSampMode(taskHandle)	0
#define	DAQmxGetSampQuantSampPerChan(taskHandle,data)	0
#define	DAQmxSetSampQuantSampPerChan(taskHandle,data)	0
#define	DAQmxResetSampQuantSampPerChan(taskHandle)	0
#define	DAQmxGetSampTimingType(taskHandle,data)	0
#define	DAQmxSetSampTimingType(taskHandle,data)	0
#define	DAQmxResetSampTimingType(taskHandle)	0
#define	DAQmxGetSampClkRate(taskHandle,data)	0
#define	DAQmxSetSampClkRate(taskHandle,data)	0
#define	DAQmxResetSampClkRate(taskHandle)	0
#define	DAQmxGetSampClkMaxRate(taskHandle,data)	0
#define	DAQmxGetSampClkSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetSampClkSrc(taskHandle,data)	0
#define	DAQmxResetSampClkSrc(taskHandle)	0
#define	DAQmxGetSampClkActiveEdge(taskHandle,data)	0
#define	DAQmxSetSampClkActiveEdge(taskHandle,data)	0
#define	DAQmxResetSampClkActiveEdge(taskHandle)	0
#define	DAQmxGetSampClkTimebaseDiv(taskHandle,data)	0
#define	DAQmxSetSampClkTimebaseDiv(taskHandle,data)	0
#define	DAQmxResetSampClkTimebaseDiv(taskHandle)	0
#define	DAQmxGetSampClkTimebaseRate(taskHandle,data)	0
#define	DAQmxSetSampClkTimebaseRate(taskHandle,data)	0
#define	DAQmxResetSampClkTimebaseRate(taskHandle)	0
#define	DAQmxGetSampClkTimebaseSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetSampClkTimebaseSrc(taskHandle,data)	0
#define	DAQmxResetSampClkTimebaseSrc(taskHandle)	0
#define	DAQmxGetSampClkTimebaseActiveEdge(taskHandle,data)	0
#define	DAQmxSetSampClkTimebaseActiveEdge(taskHandle,data)	0
#define	DAQmxResetSampClkTimebaseActiveEdge(taskHandle)	0
#define	DAQmxGetSampClkTimebaseMasterTimebaseDiv(taskHandle,data)	0
#define	DAQmxSetSampClkTimebaseMasterTimebaseDiv(taskHandle,data)	0
#define	DAQmxResetSampClkTimebaseMasterTimebaseDiv(taskHandle)	0
#define	DAQmxGetSampClkDigFltrEnable(taskHandle,data)	0
#define	DAQmxSetSampClkDigFltrEnable(taskHandle,data)	0
#define	DAQmxResetSampClkDigFltrEnable(taskHandle)	0
#define	DAQmxGetSampClkDigFltrMinPulseWidth(taskHandle,data)	0
#define	DAQmxSetSampClkDigFltrMinPulseWidth(taskHandle,data)	0
#define	DAQmxResetSampClkDigFltrMinPulseWidth(taskHandle)	0
#define	DAQmxGetSampClkDigFltrTimebaseSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetSampClkDigFltrTimebaseSrc(taskHandle,data)	0
#define	DAQmxResetSampClkDigFltrTimebaseSrc(taskHandle)	0
#define	DAQmxGetSampClkDigFltrTimebaseRate(taskHandle,data)	0
#define	DAQmxSetSampClkDigFltrTimebaseRate(taskHandle,data)	0
#define	DAQmxResetSampClkDigFltrTimebaseRate(taskHandle)	0
#define	DAQmxGetSampClkDigSyncEnable(taskHandle,data)	0
#define	DAQmxSetSampClkDigSyncEnable(taskHandle,data)	0
#define	DAQmxResetSampClkDigSyncEnable(taskHandle)	0
#define	DAQmxGetHshkDelayAfterXfer(taskHandle,data)	0
#define	DAQmxSetHshkDelayAfterXfer(taskHandle,data)	0
#define	DAQmxResetHshkDelayAfterXfer(taskHandle)	0
#define	DAQmxGetHshkStartCond(taskHandle,data)	0
#define	DAQmxSetHshkStartCond(taskHandle,data)	0
#define	DAQmxResetHshkStartCond(taskHandle)	0
#define	DAQmxGetHshkSampleInputDataWhen(taskHandle,data)	0
#define	DAQmxSetHshkSampleInputDataWhen(taskHandle,data)	0
#define	DAQmxResetHshkSampleInputDataWhen(taskHandle)	0
#define	DAQmxGetChangeDetectDIRisingEdgePhysicalChans(taskHandle,data,bufferSize)	0
#define	DAQmxSetChangeDetectDIRisingEdgePhysicalChans(taskHandle,data)	0
#define	DAQmxResetChangeDetectDIRisingEdgePhysicalChans(taskHandle)	0
#define	DAQmxGetChangeDetectDIFallingEdgePhysicalChans(taskHandle,data,bufferSize)	0
#define	DAQmxSetChangeDetectDIFallingEdgePhysicalChans(taskHandle,data)	0
#define	DAQmxResetChangeDetectDIFallingEdgePhysicalChans(taskHandle)	0
#define	DAQmxGetOnDemandSimultaneousAOEnable(taskHandle,data)	0
#define	DAQmxSetOnDemandSimultaneousAOEnable(taskHandle,data)	0
#define	DAQmxResetOnDemandSimultaneousAOEnable(taskHandle)	0
#define	DAQmxGetAIConvRate(taskHandle,data)	0
#define	DAQmxSetAIConvRate(taskHandle,data)	0
#define	DAQmxResetAIConvRate(taskHandle)	0
#define	DAQmxGetAIConvMaxRate(taskHandle,data)	0
#define	DAQmxGetAIConvSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetAIConvSrc(taskHandle,data)	0
#define	DAQmxResetAIConvSrc(taskHandle)	0
#define	DAQmxGetAIConvActiveEdge(taskHandle,data)	0
#define	DAQmxSetAIConvActiveEdge(taskHandle,data)	0
#define	DAQmxResetAIConvActiveEdge(taskHandle)	0
#define	DAQmxGetAIConvTimebaseDiv(taskHandle,data)	0
#define	DAQmxSetAIConvTimebaseDiv(taskHandle,data)	0
#define	DAQmxResetAIConvTimebaseDiv(taskHandle)	0
#define	DAQmxGetAIConvTimebaseSrc(taskHandle,data)	0
#define	DAQmxSetAIConvTimebaseSrc(taskHandle,data)	0
#define	DAQmxResetAIConvTimebaseSrc(taskHandle)	0
#define	DAQmxGetMasterTimebaseRate(taskHandle,data)	0
#define	DAQmxSetMasterTimebaseRate(taskHandle,data)	0
#define	DAQmxResetMasterTimebaseRate(taskHandle)	0
#define	DAQmxGetMasterTimebaseSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetMasterTimebaseSrc(taskHandle,data)	0
#define	DAQmxResetMasterTimebaseSrc(taskHandle)	0
#define	DAQmxGetRefClkRate(taskHandle,data)	0
#define	DAQmxSetRefClkRate(taskHandle,data)	0
#define	DAQmxResetRefClkRate(taskHandle)	0
#define	DAQmxGetRefClkSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetRefClkSrc(taskHandle,data)	0
#define	DAQmxResetRefClkSrc(taskHandle)	0
#define	DAQmxGetSyncPulseSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetSyncPulseSrc(taskHandle,data)	0
#define	DAQmxResetSyncPulseSrc(taskHandle)	0
#define	DAQmxGetSyncPulseSyncTime(taskHandle,data)	0
#define	DAQmxGetSyncPulseMinDelayToStart(taskHandle,data)	0
#define	DAQmxSetSyncPulseMinDelayToStart(taskHandle,data)	0
#define	DAQmxResetSyncPulseMinDelayToStart(taskHandle)	0
#define	DAQmxGetDelayFromSampClkDelayUnits(taskHandle,data)	0
#define	DAQmxSetDelayFromSampClkDelayUnits(taskHandle,data)	0
#define	DAQmxResetDelayFromSampClkDelayUnits(taskHandle)	0
#define	DAQmxGetDelayFromSampClkDelay(taskHandle,data)	0
#define	DAQmxSetDelayFromSampClkDelay(taskHandle,data)	0
#define	DAQmxResetDelayFromSampClkDelay(taskHandle)	0
#define	DAQmxGetStartTrigType(taskHandle,data)	0
#define	DAQmxSetStartTrigType(taskHandle,data)	0
#define	DAQmxResetStartTrigType(taskHandle)	0
#define	DAQmxGetDigEdgeStartTrigSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetDigEdgeStartTrigSrc(taskHandle,data)	0
#define	DAQmxResetDigEdgeStartTrigSrc(taskHandle)	0
#define	DAQmxGetDigEdgeStartTrigEdge(taskHandle,data)	0
#define	DAQmxSetDigEdgeStartTrigEdge(taskHandle,data)	0
#define	DAQmxResetDigEdgeStartTrigEdge(taskHandle)	0
#define	DAQmxGetDigEdgeStartTrigDigFltrEnable(taskHandle,data)	0
#define	DAQmxSetDigEdgeStartTrigDigFltrEnable(taskHandle,data)	0
#define	DAQmxResetDigEdgeStartTrigDigFltrEnable(taskHandle)	0
#define	DAQmxGetDigEdgeStartTrigDigFltrMinPulseWidth(taskHandle,data)	0
#define	DAQmxSetDigEdgeStartTrigDigFltrMinPulseWidth(taskHandle,data)	0
#define	DAQmxResetDigEdgeStartTrigDigFltrMinPulseWidth(taskHandle)	0
#define	DAQmxGetDigEdgeStartTrigDigFltrTimebaseSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetDigEdgeStartTrigDigFltrTimebaseSrc(taskHandle,data)	0
#define	DAQmxResetDigEdgeStartTrigDigFltrTimebaseSrc(taskHandle)	0
#define	DAQmxGetDigEdgeStartTrigDigFltrTimebaseRate(taskHandle,data)	0
#define	DAQmxSetDigEdgeStartTrigDigFltrTimebaseRate(taskHandle,data)	0
#define	DAQmxResetDigEdgeStartTrigDigFltrTimebaseRate(taskHandle)	0
#define	DAQmxGetDigEdgeStartTrigDigSyncEnable(taskHandle,data)	0
#define	DAQmxSetDigEdgeStartTrigDigSyncEnable(taskHandle,data)	0
#define	DAQmxResetDigEdgeStartTrigDigSyncEnable(taskHandle)	0
#define	DAQmxGetDigPatternStartTrigSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetDigPatternStartTrigSrc(taskHandle,data)	0
#define	DAQmxResetDigPatternStartTrigSrc(taskHandle)	0
#define	DAQmxGetDigPatternStartTrigPattern(taskHandle,data,bufferSize)	0
#define	DAQmxSetDigPatternStartTrigPattern(taskHandle,data)	0
#define	DAQmxResetDigPatternStartTrigPattern(taskHandle)	0
#define	DAQmxGetDigPatternStartTrigWhen(taskHandle,data)	0
#define	DAQmxSetDigPatternStartTrigWhen(taskHandle,data)	0
#define	DAQmxResetDigPatternStartTrigWhen(taskHandle)	0
#define	DAQmxGetAnlgEdgeStartTrigSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetAnlgEdgeStartTrigSrc(taskHandle,data)	0
#define	DAQmxResetAnlgEdgeStartTrigSrc(taskHandle)	0
#define	DAQmxGetAnlgEdgeStartTrigSlope(taskHandle,data)	0
#define	DAQmxSetAnlgEdgeStartTrigSlope(taskHandle,data)	0
#define	DAQmxResetAnlgEdgeStartTrigSlope(taskHandle)	0
#define	DAQmxGetAnlgEdgeStartTrigLvl(taskHandle,data)	0
#define	DAQmxSetAnlgEdgeStartTrigLvl(taskHandle,data)	0
#define	DAQmxResetAnlgEdgeStartTrigLvl(taskHandle)	0
#define	DAQmxGetAnlgEdgeStartTrigHyst(taskHandle,data)	0
#define	DAQmxSetAnlgEdgeStartTrigHyst(taskHandle,data)	0
#define	DAQmxResetAnlgEdgeStartTrigHyst(taskHandle)	0
#define	DAQmxGetAnlgEdgeStartTrigCoupling(taskHandle,data)	0
#define	DAQmxSetAnlgEdgeStartTrigCoupling(taskHandle,data)	0
#define	DAQmxResetAnlgEdgeStartTrigCoupling(taskHandle)	0
#define	DAQmxGetAnlgWinStartTrigSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetAnlgWinStartTrigSrc(taskHandle,data)	0
#define	DAQmxResetAnlgWinStartTrigSrc(taskHandle)	0
#define	DAQmxGetAnlgWinStartTrigWhen(taskHandle,data)	0
#define	DAQmxSetAnlgWinStartTrigWhen(taskHandle,data)	0
#define	DAQmxResetAnlgWinStartTrigWhen(taskHandle)	0
#define	DAQmxGetAnlgWinStartTrigTop(taskHandle,data)	0
#define	DAQmxSetAnlgWinStartTrigTop(taskHandle,data)	0
#define	DAQmxResetAnlgWinStartTrigTop(taskHandle)	0
#define	DAQmxGetAnlgWinStartTrigBtm(taskHandle,data)	0
#define	DAQmxSetAnlgWinStartTrigBtm(taskHandle,data)	0
#define	DAQmxResetAnlgWinStartTrigBtm(taskHandle)	0
#define	DAQmxGetAnlgWinStartTrigCoupling(taskHandle,data)	0
#define	DAQmxSetAnlgWinStartTrigCoupling(taskHandle,data)	0
#define	DAQmxResetAnlgWinStartTrigCoupling(taskHandle)	0
#define	DAQmxGetStartTrigDelay(taskHandle,data)	0
#define	DAQmxSetStartTrigDelay(taskHandle,data)	0
#define	DAQmxResetStartTrigDelay(taskHandle)	0
#define	DAQmxGetStartTrigDelayUnits(taskHandle,data)	0
#define	DAQmxSetStartTrigDelayUnits(taskHandle,data)	0
#define	DAQmxResetStartTrigDelayUnits(taskHandle)	0
#define	DAQmxGetStartTrigRetriggerable(taskHandle,data)	0
#define	DAQmxSetStartTrigRetriggerable(taskHandle,data)	0
#define	DAQmxResetStartTrigRetriggerable(taskHandle)	0
#define	DAQmxGetRefTrigType(taskHandle,data)	0
#define	DAQmxSetRefTrigType(taskHandle,data)	0
#define	DAQmxResetRefTrigType(taskHandle)	0
#define	DAQmxGetRefTrigPretrigSamples(taskHandle,data)	0
#define	DAQmxSetRefTrigPretrigSamples(taskHandle,data)	0
#define	DAQmxResetRefTrigPretrigSamples(taskHandle)	0
#define	DAQmxGetDigEdgeRefTrigSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetDigEdgeRefTrigSrc(taskHandle,data)	0
#define	DAQmxResetDigEdgeRefTrigSrc(taskHandle)	0
#define	DAQmxGetDigEdgeRefTrigEdge(taskHandle,data)	0
#define	DAQmxSetDigEdgeRefTrigEdge(taskHandle,data)	0
#define	DAQmxResetDigEdgeRefTrigEdge(taskHandle)	0
#define	DAQmxGetDigPatternRefTrigSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetDigPatternRefTrigSrc(taskHandle,data)	0
#define	DAQmxResetDigPatternRefTrigSrc(taskHandle)	0
#define	DAQmxGetDigPatternRefTrigPattern(taskHandle,data,bufferSize)	0
#define	DAQmxSetDigPatternRefTrigPattern(taskHandle,data)	0
#define	DAQmxResetDigPatternRefTrigPattern(taskHandle)	0
#define	DAQmxGetDigPatternRefTrigWhen(taskHandle,data)	0
#define	DAQmxSetDigPatternRefTrigWhen(taskHandle,data)	0
#define	DAQmxResetDigPatternRefTrigWhen(taskHandle)	0
#define	DAQmxGetAnlgEdgeRefTrigSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetAnlgEdgeRefTrigSrc(taskHandle,data)	0
#define	DAQmxResetAnlgEdgeRefTrigSrc(taskHandle)	0
#define	DAQmxGetAnlgEdgeRefTrigSlope(taskHandle,data)	0
#define	DAQmxSetAnlgEdgeRefTrigSlope(taskHandle,data)	0
#define	DAQmxResetAnlgEdgeRefTrigSlope(taskHandle)	0
#define	DAQmxGetAnlgEdgeRefTrigLvl(taskHandle,data)	0
#define	DAQmxSetAnlgEdgeRefTrigLvl(taskHandle,data)	0
#define	DAQmxResetAnlgEdgeRefTrigLvl(taskHandle)	0
#define	DAQmxGetAnlgEdgeRefTrigHyst(taskHandle,data)	0
#define	DAQmxSetAnlgEdgeRefTrigHyst(taskHandle,data)	0
#define	DAQmxResetAnlgEdgeRefTrigHyst(taskHandle)	0
#define	DAQmxGetAnlgEdgeRefTrigCoupling(taskHandle,data)	0
#define	DAQmxSetAnlgEdgeRefTrigCoupling(taskHandle,data)	0
#define	DAQmxResetAnlgEdgeRefTrigCoupling(taskHandle)	0
#define	DAQmxGetAnlgWinRefTrigSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetAnlgWinRefTrigSrc(taskHandle,data)	0
#define	DAQmxResetAnlgWinRefTrigSrc(taskHandle)	0
#define	DAQmxGetAnlgWinRefTrigWhen(taskHandle,data)	0
#define	DAQmxSetAnlgWinRefTrigWhen(taskHandle,data)	0
#define	DAQmxResetAnlgWinRefTrigWhen(taskHandle)	0
#define	DAQmxGetAnlgWinRefTrigTop(taskHandle,data)	0
#define	DAQmxSetAnlgWinRefTrigTop(taskHandle,data)	0
#define	DAQmxResetAnlgWinRefTrigTop(taskHandle)	0
#define	DAQmxGetAnlgWinRefTrigBtm(taskHandle,data)	0
#define	DAQmxSetAnlgWinRefTrigBtm(taskHandle,data)	0
#define	DAQmxResetAnlgWinRefTrigBtm(taskHandle)	0
#define	DAQmxGetAnlgWinRefTrigCoupling(taskHandle,data)	0
#define	DAQmxSetAnlgWinRefTrigCoupling(taskHandle,data)	0
#define	DAQmxResetAnlgWinRefTrigCoupling(taskHandle)	0
#define	DAQmxGetAdvTrigType(taskHandle,data)	0
#define	DAQmxSetAdvTrigType(taskHandle,data)	0
#define	DAQmxResetAdvTrigType(taskHandle)	0
#define	DAQmxGetDigEdgeAdvTrigSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetDigEdgeAdvTrigSrc(taskHandle,data)	0
#define	DAQmxResetDigEdgeAdvTrigSrc(taskHandle)	0
#define	DAQmxGetDigEdgeAdvTrigEdge(taskHandle,data)	0
#define	DAQmxSetDigEdgeAdvTrigEdge(taskHandle,data)	0
#define	DAQmxResetDigEdgeAdvTrigEdge(taskHandle)	0
#define	DAQmxGetDigEdgeAdvTrigDigFltrEnable(taskHandle,data)	0
#define	DAQmxSetDigEdgeAdvTrigDigFltrEnable(taskHandle,data)	0
#define	DAQmxResetDigEdgeAdvTrigDigFltrEnable(taskHandle)	0
#define	DAQmxGetHshkTrigType(taskHandle,data)	0
#define	DAQmxSetHshkTrigType(taskHandle,data)	0
#define	DAQmxResetHshkTrigType(taskHandle)	0
#define	DAQmxGetInterlockedHshkTrigSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetInterlockedHshkTrigSrc(taskHandle,data)	0
#define	DAQmxResetInterlockedHshkTrigSrc(taskHandle)	0
#define	DAQmxGetInterlockedHshkTrigAssertedLvl(taskHandle,data)	0
#define	DAQmxSetInterlockedHshkTrigAssertedLvl(taskHandle,data)	0
#define	DAQmxResetInterlockedHshkTrigAssertedLvl(taskHandle)	0
#define	DAQmxGetPauseTrigType(taskHandle,data)	0
#define	DAQmxSetPauseTrigType(taskHandle,data)	0
#define	DAQmxResetPauseTrigType(taskHandle)	0
#define	DAQmxGetAnlgLvlPauseTrigSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetAnlgLvlPauseTrigSrc(taskHandle,data)	0
#define	DAQmxResetAnlgLvlPauseTrigSrc(taskHandle)	0
#define	DAQmxGetAnlgLvlPauseTrigWhen(taskHandle,data)	0
#define	DAQmxSetAnlgLvlPauseTrigWhen(taskHandle,data)	0
#define	DAQmxResetAnlgLvlPauseTrigWhen(taskHandle)	0
#define	DAQmxGetAnlgLvlPauseTrigLvl(taskHandle,data)	0
#define	DAQmxSetAnlgLvlPauseTrigLvl(taskHandle,data)	0
#define	DAQmxResetAnlgLvlPauseTrigLvl(taskHandle)	0
#define	DAQmxGetAnlgLvlPauseTrigHyst(taskHandle,data)	0
#define	DAQmxSetAnlgLvlPauseTrigHyst(taskHandle,data)	0
#define	DAQmxResetAnlgLvlPauseTrigHyst(taskHandle)	0
#define	DAQmxGetAnlgLvlPauseTrigCoupling(taskHandle,data)	0
#define	DAQmxSetAnlgLvlPauseTrigCoupling(taskHandle,data)	0
#define	DAQmxResetAnlgLvlPauseTrigCoupling(taskHandle)	0
#define	DAQmxGetAnlgWinPauseTrigSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetAnlgWinPauseTrigSrc(taskHandle,data)	0
#define	DAQmxResetAnlgWinPauseTrigSrc(taskHandle)	0
#define	DAQmxGetAnlgWinPauseTrigWhen(taskHandle,data)	0
#define	DAQmxSetAnlgWinPauseTrigWhen(taskHandle,data)	0
#define	DAQmxResetAnlgWinPauseTrigWhen(taskHandle)	0
#define	DAQmxGetAnlgWinPauseTrigTop(taskHandle,data)	0
#define	DAQmxSetAnlgWinPauseTrigTop(taskHandle,data)	0
#define	DAQmxResetAnlgWinPauseTrigTop(taskHandle)	0
#define	DAQmxGetAnlgWinPauseTrigBtm(taskHandle,data)	0
#define	DAQmxSetAnlgWinPauseTrigBtm(taskHandle,data)	0
#define	DAQmxResetAnlgWinPauseTrigBtm(taskHandle)	0
#define	DAQmxGetAnlgWinPauseTrigCoupling(taskHandle,data)	0
#define	DAQmxSetAnlgWinPauseTrigCoupling(taskHandle,data)	0
#define	DAQmxResetAnlgWinPauseTrigCoupling(taskHandle)	0
#define	DAQmxGetDigLvlPauseTrigSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetDigLvlPauseTrigSrc(taskHandle,data)	0
#define	DAQmxResetDigLvlPauseTrigSrc(taskHandle)	0
#define	DAQmxGetDigLvlPauseTrigWhen(taskHandle,data)	0
#define	DAQmxSetDigLvlPauseTrigWhen(taskHandle,data)	0
#define	DAQmxResetDigLvlPauseTrigWhen(taskHandle)	0
#define	DAQmxGetDigLvlPauseTrigDigFltrEnable(taskHandle,data)	0
#define	DAQmxSetDigLvlPauseTrigDigFltrEnable(taskHandle,data)	0
#define	DAQmxResetDigLvlPauseTrigDigFltrEnable(taskHandle)	0
#define	DAQmxGetDigLvlPauseTrigDigFltrMinPulseWidth(taskHandle,data)	0
#define	DAQmxSetDigLvlPauseTrigDigFltrMinPulseWidth(taskHandle,data)	0
#define	DAQmxResetDigLvlPauseTrigDigFltrMinPulseWidth(taskHandle)	0
#define	DAQmxGetDigLvlPauseTrigDigFltrTimebaseSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetDigLvlPauseTrigDigFltrTimebaseSrc(taskHandle,data)	0
#define	DAQmxResetDigLvlPauseTrigDigFltrTimebaseSrc(taskHandle)	0
#define	DAQmxGetDigLvlPauseTrigDigFltrTimebaseRate(taskHandle,data)	0
#define	DAQmxSetDigLvlPauseTrigDigFltrTimebaseRate(taskHandle,data)	0
#define	DAQmxResetDigLvlPauseTrigDigFltrTimebaseRate(taskHandle)	0
#define	DAQmxGetDigLvlPauseTrigDigSyncEnable(taskHandle,data)	0
#define	DAQmxSetDigLvlPauseTrigDigSyncEnable(taskHandle,data)	0
#define	DAQmxResetDigLvlPauseTrigDigSyncEnable(taskHandle)	0
#define	DAQmxGetArmStartTrigType(taskHandle,data)	0
#define	DAQmxSetArmStartTrigType(taskHandle,data)	0
#define	DAQmxResetArmStartTrigType(taskHandle)	0
#define	DAQmxGetDigEdgeArmStartTrigSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetDigEdgeArmStartTrigSrc(taskHandle,data)	0
#define	DAQmxResetDigEdgeArmStartTrigSrc(taskHandle)	0
#define	DAQmxGetDigEdgeArmStartTrigEdge(taskHandle,data)	0
#define	DAQmxSetDigEdgeArmStartTrigEdge(taskHandle,data)	0
#define	DAQmxResetDigEdgeArmStartTrigEdge(taskHandle)	0
#define	DAQmxGetDigEdgeArmStartTrigDigFltrEnable(taskHandle,data)	0
#define	DAQmxSetDigEdgeArmStartTrigDigFltrEnable(taskHandle,data)	0
#define	DAQmxResetDigEdgeArmStartTrigDigFltrEnable(taskHandle)	0
#define	DAQmxGetDigEdgeArmStartTrigDigFltrMinPulseWidth(taskHandle,data)	0
#define	DAQmxSetDigEdgeArmStartTrigDigFltrMinPulseWidth(taskHandle,data)	0
#define	DAQmxResetDigEdgeArmStartTrigDigFltrMinPulseWidth(taskHandle)	0
#define	DAQmxGetDigEdgeArmStartTrigDigFltrTimebaseSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetDigEdgeArmStartTrigDigFltrTimebaseSrc(taskHandle,data)	0
#define	DAQmxResetDigEdgeArmStartTrigDigFltrTimebaseSrc(taskHandle)	0
#define	DAQmxGetDigEdgeArmStartTrigDigFltrTimebaseRate(taskHandle,data)	0
#define	DAQmxSetDigEdgeArmStartTrigDigFltrTimebaseRate(taskHandle,data)	0
#define	DAQmxResetDigEdgeArmStartTrigDigFltrTimebaseRate(taskHandle)	0
#define	DAQmxGetDigEdgeArmStartTrigDigSyncEnable(taskHandle,data)	0
#define	DAQmxSetDigEdgeArmStartTrigDigSyncEnable(taskHandle,data)	0
#define	DAQmxResetDigEdgeArmStartTrigDigSyncEnable(taskHandle)	0
#define	DAQmxGetWatchdogTimeout(taskHandle,data)	0
#define	DAQmxSetWatchdogTimeout(taskHandle,data)	0
#define	DAQmxResetWatchdogTimeout(taskHandle)	0
#define	DAQmxGetWatchdogExpirTrigType(taskHandle,data)	0
#define	DAQmxSetWatchdogExpirTrigType(taskHandle,data)	0
#define	DAQmxResetWatchdogExpirTrigType(taskHandle)	0
#define	DAQmxGetDigEdgeWatchdogExpirTrigSrc(taskHandle,data,bufferSize)	0
#define	DAQmxSetDigEdgeWatchdogExpirTrigSrc(taskHandle,data)	0
#define	DAQmxResetDigEdgeWatchdogExpirTrigSrc(taskHandle)	0
#define	DAQmxGetDigEdgeWatchdogExpirTrigEdge(taskHandle,data)	0
#define	DAQmxSetDigEdgeWatchdogExpirTrigEdge(taskHandle,data)	0
#define	DAQmxResetDigEdgeWatchdogExpirTrigEdge(taskHandle)	0
#define	DAQmxGetWatchdogDOExpirState(taskHandle,channel,data)	0
#define	DAQmxSetWatchdogDOExpirState(taskHandle,channel,data)	0
#define	DAQmxResetWatchdogDOExpirState(taskHandle,channel)	0
#define	DAQmxGetWatchdogHasExpired(taskHandle,data)	0
#define	DAQmxGetWriteRelativeTo(taskHandle,data)	0
#define	DAQmxSetWriteRelativeTo(taskHandle,data)	0
#define	DAQmxResetWriteRelativeTo(taskHandle)	0
#define	DAQmxGetWriteOffset(taskHandle,data)	0
#define	DAQmxSetWriteOffset(taskHandle,data)	0
#define	DAQmxResetWriteOffset(taskHandle)	0
#define	DAQmxGetWriteRegenMode(taskHandle,data)	0
#define	DAQmxSetWriteRegenMode(taskHandle,data)	0
#define	DAQmxResetWriteRegenMode(taskHandle)	0
#define	DAQmxGetWriteCurrWritePos(taskHandle,data)	0
#define	DAQmxGetWriteSpaceAvail(taskHandle,data)	0
#define	DAQmxGetWriteTotalSampPerChanGenerated(taskHandle,data)	0
#define	DAQmxGetWriteRawDataWidth(taskHandle,data)	0
#define	DAQmxGetWriteNumChans(taskHandle,data)	0
#define	DAQmxGetWriteWaitMode(taskHandle,data)	0
#define	DAQmxSetWriteWaitMode(taskHandle,data)	0
#define	DAQmxResetWriteWaitMode(taskHandle)	0
#define	DAQmxGetWriteSleepTime(taskHandle,data)	0
#define	DAQmxSetWriteSleepTime(taskHandle,data)	0
#define	DAQmxResetWriteSleepTime(taskHandle)	0
#define	DAQmxGetWriteDigitalLinesBytesPerChan(taskHandle,data)	0
#define	DAQmxGetPhysicalChanTEDSMfgID(physicalChannel,data)	0
#define	DAQmxGetPhysicalChanTEDSModelNum(physicalChannel,data)	0
#define	DAQmxGetPhysicalChanTEDSSerialNum(physicalChannel,data)	0
#define	DAQmxGetPhysicalChanTEDSVersionNum(physicalChannel,data)	0
#define	DAQmxGetPhysicalChanTEDSVersionLetter(physicalChannel,data,bufferSize)	0
#define	DAQmxGetPhysicalChanTEDSBitStream(physicalChannel,data,arraySizeInSamples)	0
#define	DAQmxGetPhysicalChanTEDSTemplateIDs(physicalChannel,data,arraySizeInSamples)	0
#define	DAQmxGetPersistedTaskAuthor(taskName,data,bufferSize)	0
#define	DAQmxGetPersistedTaskAllowInteractiveEditing(taskName,data)	0
#define	DAQmxGetPersistedTaskAllowInteractiveDeletion(taskName,data)	0
#define	DAQmxGetPersistedChanAuthor(channel,data,bufferSize)	0
#define	DAQmxGetPersistedChanAllowInteractiveEditing(channel,data)	0
#define	DAQmxGetPersistedChanAllowInteractiveDeletion(channel,data)	0
#define	DAQmxGetPersistedScaleAuthor(scaleName,data,bufferSize)	0
#define	DAQmxGetPersistedScaleAllowInteractiveEditing(scaleName,data)	0
#define	DAQmxGetPersistedScaleAllowInteractiveDeletion(scaleName,data)	0

#define DAQmxSetAIUsbXferReqSize(taskHandle, channel, data) 0
													



/******************************************************************************
 *** NI-DAQmx Error Codes *****************************************************
 ******************************************************************************/

#define DAQmxSuccess                                  (0)

#define DAQmxFailed(error)                            ((error)<0)

// Error and Warning Codes
#define DAQmxErrorCOCannotKeepUpInHWTimedSinglePoint                                    (-209805)
#define DAQmxErrorWaitForNextSampClkDetected3OrMoreSampClks                             (-209803)
#define DAQmxErrorWaitForNextSampClkDetectedMissedSampClk                               (-209802)
#define DAQmxErrorWriteNotCompleteBeforeSampClk                                         (-209801)
#define DAQmxErrorReadNotCompleteBeforeSampClk                                          (-209800)
#define DAQmxErrorInvalidAttributeName                                                  (-201086)
#define DAQmxErrorCJCChanNameMustBeSetWhenCJCSrcIsScannableChan                         (-201085)
#define DAQmxErrorHiddenChanMissingInChansPropertyInCfgFile                             (-201084)
#define DAQmxErrorChanNamesNotSpecdInCfgFile                                            (-201083)
#define DAQmxErrorDuplicateHiddenChanNamesInCfgFile                                     (-201082)
#define DAQmxErrorDuplicateChanNameInCfgFile                                            (-201081)
#define DAQmxErrorInvalidSCCModuleForSlotSpecd                                          (-201080)
#define DAQmxErrorInvalidSCCSlotNumberSpecd                                             (-201079)
#define DAQmxErrorInvalidSectionIdentifier                                              (-201078)
#define DAQmxErrorInvalidSectionName                                                    (-201077)
#define DAQmxErrorDAQmxVersionNotSupported                                              (-201076)
#define DAQmxErrorSWObjectsFoundInFile                                                  (-201075)
#define DAQmxErrorHWObjectsFoundInFile                                                  (-201074)
#define DAQmxErrorLocalChannelSpecdWithNoParentTask                                     (-201073)
#define DAQmxErrorTaskReferencesMissingLocalChannel                                     (-201072)
#define DAQmxErrorTaskReferencesLocalChannelFromOtherTask                               (-201071)
#define DAQmxErrorTaskMissingChannelProperty                                            (-201070)
#define DAQmxErrorInvalidLocalChanName                                                  (-201069)
#define DAQmxErrorInvalidEscapeCharacterInString                                        (-201068)
#define DAQmxErrorInvalidTableIdentifier                                                (-201067)
#define DAQmxErrorValueFoundInInvalidColumn                                             (-201066)
#define DAQmxErrorMissingStartOfTable                                                   (-201065)
#define DAQmxErrorFileMissingRequiredDAQmxHeader                                        (-201064)
#define DAQmxErrorDeviceIDDoesNotMatch                                                  (-201063)
#define DAQmxErrorBufferedOperationsNotSupportedOnSelectedLines                         (-201062)
#define DAQmxErrorPropertyConflictsWithScale                                            (-201061)
#define DAQmxErrorInvalidINIFileSyntax                                                  (-201060)
#define DAQmxErrorDeviceInfoFailedPXIChassisNotIdentified                               (-201059)
#define DAQmxErrorInvalidHWProductNumber                                                (-201058)
#define DAQmxErrorInvalidHWProductType                                                  (-201057)
#define DAQmxErrorInvalidNumericFormatSpecd                                             (-201056)
#define DAQmxErrorDuplicatePropertyInObject                                             (-201055)
#define DAQmxErrorInvalidEnumValueSpecd                                                 (-201054)
#define DAQmxErrorTEDSSensorPhysicalChannelConflict                                     (-201053)
#define DAQmxErrorTooManyPhysicalChansForTEDSInterfaceSpecd                             (-201052)
#define DAQmxErrorIncapableTEDSInterfaceControllingDeviceSpecd                          (-201051)
#define DAQmxErrorSCCCarrierSpecdIsMissing                                              (-201050)
#define DAQmxErrorIncapableSCCDigitizingDeviceSpecd                                     (-201049)
#define DAQmxErrorAccessorySettingNotApplicable                                         (-201048)
#define DAQmxErrorDeviceAndConnectorSpecdAlreadyOccupied                                (-201047)
#define DAQmxErrorIllegalAccessoryTypeForDeviceSpecd                                    (-201046)
#define DAQmxErrorInvalidDeviceConnectorNumberSpecd                                     (-201045)
#define DAQmxErrorInvalidAccessoryName                                                  (-201044)
#define DAQmxErrorMoreThanOneMatchForSpecdDevice                                        (-201043)
#define DAQmxErrorNoMatchForSpecdDevice                                                 (-201042)
#define DAQmxErrorProductTypeAndProductNumberConflict                                   (-201041)
#define DAQmxErrorExtraPropertyDetectedInSpecdObject                                    (-201040)
#define DAQmxErrorRequiredPropertyMissing                                               (-201039)
#define DAQmxErrorCantSetAuthorForLocalChan                                             (-201038)
#define DAQmxErrorInvalidTimeValue                                                      (-201037)
#define DAQmxErrorInvalidTimeFormat                                                     (-201036)
#define DAQmxErrorDigDevChansSpecdInModeOtherThanParallel                               (-201035)
#define DAQmxErrorCascadeDigitizationModeNotSupported                                   (-201034)
#define DAQmxErrorSpecdSlotAlreadyOccupied                                              (-201033)
#define DAQmxErrorInvalidSCXISlotNumberSpecd                                            (-201032)
#define DAQmxErrorAddressAlreadyInUse                                                   (-201031)
#define DAQmxErrorSpecdDeviceDoesNotSupportRTSI                                         (-201030)
#define DAQmxErrorSpecdDeviceIsAlreadyOnRTSIBus                                         (-201029)
#define DAQmxErrorIdentifierInUse                                                       (-201028)
#define DAQmxErrorWaitForNextSampleClockOrReadDetected3OrMoreMissedSampClks             (-201027)
#define DAQmxErrorHWTimedAndDataXferPIO                                                 (-201026)
#define DAQmxErrorNonBufferedAndHWTimed                                                 (-201025)
#define DAQmxErrorCTROutSampClkPeriodShorterThanGenPulseTrainPeriodPolled               (-201024)
#define DAQmxErrorCTROutSampClkPeriodShorterThanGenPulseTrainPeriod2                    (-201023)
#define DAQmxErrorCOCannotKeepUpInHWTimedSinglePointPolled                              (-201022)
#define DAQmxErrorWriteRecoveryCannotKeepUpInHWTimedSinglePoint                         (-201021)
#define DAQmxErrorNoChangeDetectionOnSelectedLineForDevice                              (-201020)
#define DAQmxErrorSMIOPauseTriggersNotSupportedWithChannelExpansion                     (-201019)
#define DAQmxErrorClockMasterForExternalClockNotLongestPipeline                         (-201018)
#define DAQmxErrorUnsupportedUnicodeByteOrderMarker                                     (-201017)
#define DAQmxErrorTooManyInstructionsInLoopInScript                                     (-201016)
#define DAQmxErrorPLLNotLocked                                                          (-201015)
#define DAQmxErrorIfElseBlockNotAllowedInFiniteRepeatLoopInScript                       (-201014)
#define DAQmxErrorIfElseBlockNotAllowedInConditionalRepeatLoopInScript                  (-201013)
#define DAQmxErrorClearIsLastInstructionInIfElseBlockInScript                           (-201012)
#define DAQmxErrorInvalidWaitDurationBeforeIfElseBlockInScript                          (-201011)
#define DAQmxErrorMarkerPosInvalidBeforeIfElseBlockInScript                             (-201010)
#define DAQmxErrorInvalidSubsetLengthBeforeIfElseBlockInScript                          (-201009)
#define DAQmxErrorInvalidWaveformLengthBeforeIfElseBlockInScript                        (-201008)
#define DAQmxErrorGenerateOrFiniteWaitInstructionExpectedBeforeIfElseBlockInScript      (-201007)
#define DAQmxErrorCalPasswordNotSupported                                               (-201006)
#define DAQmxErrorSetupCalNeededBeforeAdjustCal                                         (-201005)
#define DAQmxErrorMultipleChansNotSupportedDuringCalSetup                               (-201004)
#define DAQmxErrorDevCannotBeAccessed                                                   (-201003)
#define DAQmxErrorSampClkRateDoesntMatchSampClkSrc                                      (-201002)
#define DAQmxErrorSampClkRateNotSupportedWithEARDisabled                                (-201001)
#define DAQmxErrorLabVIEWVersionDoesntSupportDAQmxEvents                                (-201000)
#define DAQmxErrorCOReadyForNewValNotSupportedWithOnDemand                              (-200999)
#define DAQmxErrorCIHWTimedSinglePointNotSupportedForMeasType                           (-200998)
#define DAQmxErrorOnDemandNotSupportedWithHWTimedSinglePoint                            (-200997)
#define DAQmxErrorHWTimedSinglePointAndDataXferNotProgIO                                (-200996)
#define DAQmxErrorMemMapAndHWTimedSinglePoint                                           (-200995)
#define DAQmxErrorCannotSetPropertyWhenHWTimedSinglePointTaskIsRunning                  (-200994)
#define DAQmxErrorCTROutSampClkPeriodShorterThanGenPulseTrainPeriod                     (-200993)
#define DAQmxErrorTooManyEventsGenerated                                                (-200992)
#define DAQmxErrorMStudioCppRemoveEventsBeforeStop                                      (-200991)
#define DAQmxErrorCAPICannotRegisterSyncEventsFromMultipleThreads                       (-200990)
#define DAQmxErrorReadWaitNextSampClkWaitMismatchTwo                                    (-200989)
#define DAQmxErrorReadWaitNextSampClkWaitMismatchOne                                    (-200988)
#define DAQmxErrorDAQmxSignalEventTypeNotSupportedByChanTypesOrDevicesInTask            (-200987)
#define DAQmxErrorCannotUnregisterDAQmxSoftwareEventWhileTaskIsRunning                  (-200986)
#define DAQmxErrorAutoStartWriteNotAllowedEventRegistered                               (-200985)
#define DAQmxErrorAutoStartReadNotAllowedEventRegistered                                (-200984)
#define DAQmxErrorCannotGetPropertyWhenTaskNotReservedCommittedOrRunning                (-200983)
#define DAQmxErrorSignalEventsNotSupportedByDevice                                      (-200982)
#define DAQmxErrorEveryNSamplesAcqIntoBufferEventNotSupportedByDevice                   (-200981)
#define DAQmxErrorEveryNSampsTransferredFromBufferEventNotSupportedByDevice             (-200980)
#define DAQmxErrorCAPISyncEventsTaskStateChangeNotAllowedFromDifferentThread            (-200979)
#define DAQmxErrorDAQmxSWEventsWithDifferentCallMechanisms                              (-200978)
#define DAQmxErrorCantSaveChanWithPolyCalScaleAndAllowInteractiveEdit                   (-200977)
#define DAQmxErrorChanDoesNotSupportCJC                                                 (-200976)
#define DAQmxErrorCOReadyForNewValNotSupportedWithHWTimedSinglePoint                    (-200975)
#define DAQmxErrorDACAllowConnToGndNotSupportedByDevWhenRefSrcExt                       (-200974)
#define DAQmxErrorCantGetPropertyTaskNotRunning                                         (-200973)
#define DAQmxErrorCantSetPropertyTaskNotRunning                                         (-200972)
#define DAQmxErrorCantSetPropertyTaskNotRunningCommitted                                (-200971)
#define DAQmxErrorAIEveryNSampsEventIntervalNotMultipleOf2                              (-200970)
#define DAQmxErrorInvalidTEDSPhysChanNotAI                                              (-200969)
#define DAQmxErrorCAPICannotPerformTaskOperationInAsyncCallback                         (-200968)
#define DAQmxErrorEveryNSampsTransferredFromBufferEventAlreadyRegistered                (-200967)
#define DAQmxErrorEveryNSampsAcqIntoBufferEventAlreadyRegistered                        (-200966)
#define DAQmxErrorEveryNSampsTransferredFromBufferNotForInput                           (-200965)
#define DAQmxErrorEveryNSampsAcqIntoBufferNotForOutput                                  (-200964)
#define DAQmxErrorAOSampTimingTypeDifferentIn2Tasks                                     (-200963)
#define DAQmxErrorCouldNotDownloadFirmwareHWDamaged                                     (-200962)
#define DAQmxErrorCouldNotDownloadFirmwareFileMissingOrDamaged                          (-200961)
#define DAQmxErrorCannotRegisterDAQmxSoftwareEventWhileTaskIsRunning                    (-200960)
#define DAQmxErrorDifferentRawDataCompression                                           (-200959)
#define DAQmxErrorConfiguredTEDSInterfaceDevNotDetected                                 (-200958)
#define DAQmxErrorCompressedSampSizeExceedsResolution                                   (-200957)
#define DAQmxErrorChanDoesNotSupportCompression                                         (-200956)
#define DAQmxErrorDifferentRawDataFormats                                               (-200955)
#define DAQmxErrorSampClkOutputTermIncludesStartTrigSrc                                 (-200954)
#define DAQmxErrorStartTrigSrcEqualToSampClkSrc                                         (-200953)
#define DAQmxErrorEventOutputTermIncludesTrigSrc                                        (-200952)
#define DAQmxErrorCOMultipleWritesBetweenSampClks                                       (-200951)
#define DAQmxErrorDoneEventAlreadyRegistered                                            (-200950)
#define DAQmxErrorSignalEventAlreadyRegistered                                          (-200949)
#define DAQmxErrorCannotHaveTimedLoopAndDAQmxSignalEventsInSameTask                     (-200948)
#define DAQmxErrorNeedLabVIEW711PatchToUseDAQmxEvents                                   (-200947)
#define DAQmxErrorStartFailedDueToWriteFailure                                          (-200946)
#define DAQmxErrorDataXferCustomThresholdNotDMAXferMethodSpecifiedForDev                (-200945)
#define DAQmxErrorDataXferRequestConditionNotSpecifiedForCustomThreshold                (-200944)
#define DAQmxErrorDataXferCustomThresholdNotSpecified                                   (-200943)
#define DAQmxErrorCAPISyncCallbackNotSupportedInLVRT                                    (-200942)
#define DAQmxErrorCalChanReversePolyCoefNotSpecd                                        (-200941)
#define DAQmxErrorCalChanForwardPolyCoefNotSpecd                                        (-200940)
#define DAQmxErrorChanCalRepeatedNumberInPreScaledVals                                  (-200939)
#define DAQmxErrorChanCalTableNumScaledNotEqualNumPrescaledVals                         (-200938)
#define DAQmxErrorChanCalTableScaledValsNotSpecd                                        (-200937)
#define DAQmxErrorChanCalTablePreScaledValsNotSpecd                                     (-200936)
#define DAQmxErrorChanCalScaleTypeNotSet                                                (-200935)
#define DAQmxErrorChanCalExpired                                                        (-200934)
#define DAQmxErrorChanCalExpirationDateNotSet                                           (-200933)
#define DAQmxError3OutputPortCombinationGivenSampTimingType653x                         (-200932)
#define DAQmxError3InputPortCombinationGivenSampTimingType653x                          (-200931)
#define DAQmxError2OutputPortCombinationGivenSampTimingType653x                         (-200930)
#define DAQmxError2InputPortCombinationGivenSampTimingType653x                          (-200929)
#define DAQmxErrorPatternMatcherMayBeUsedByOneTrigOnly                                  (-200928)
#define DAQmxErrorNoChansSpecdForPatternSource                                          (-200927)
#define DAQmxErrorChangeDetectionChanNotInTask                                          (-200926)
#define DAQmxErrorChangeDetectionChanNotTristated                                       (-200925)
#define DAQmxErrorWaitModeValueNotSupportedNonBuffered                                  (-200924)
#define DAQmxErrorWaitModePropertyNotSupportedNonBuffered                               (-200923)
#define DAQmxErrorCantSavePerLineConfigDigChanSoInteractiveEditsAllowed                 (-200922)
#define DAQmxErrorCantSaveNonPortMultiLineDigChanSoInteractiveEditsAllowed              (-200921)
#define DAQmxErrorBufferSizeNotMultipleOfEveryNSampsEventIntervalNoIrqOnDev             (-200920)
#define DAQmxErrorGlobalTaskNameAlreadyChanName                                         (-200919)
#define DAQmxErrorGlobalChanNameAlreadyTaskName                                         (-200918)
#define DAQmxErrorAOEveryNSampsEventIntervalNotMultipleOf2                              (-200917)
#define DAQmxErrorSampleTimebaseDivisorNotSupportedGivenTimingType                      (-200916)
#define DAQmxErrorHandshakeEventOutputTermNotSupportedGivenTimingType                   (-200915)
#define DAQmxErrorChangeDetectionOutputTermNotSupportedGivenTimingType                  (-200914)
#define DAQmxErrorReadyForTransferOutputTermNotSupportedGivenTimingType                 (-200913)
#define DAQmxErrorRefTrigOutputTermNotSupportedGivenTimingType                          (-200912)
#define DAQmxErrorStartTrigOutputTermNotSupportedGivenTimingType                        (-200911)
#define DAQmxErrorSampClockOutputTermNotSupportedGivenTimingType                        (-200910)
#define DAQmxError20MhzTimebaseNotSupportedGivenTimingType                              (-200909)
#define DAQmxErrorSampClockSourceNotSupportedGivenTimingType                            (-200908)
#define DAQmxErrorRefTrigTypeNotSupportedGivenTimingType                                (-200907)
#define DAQmxErrorPauseTrigTypeNotSupportedGivenTimingType                              (-200906)
#define DAQmxErrorHandshakeTrigTypeNotSupportedGivenTimingType                          (-200905)
#define DAQmxErrorStartTrigTypeNotSupportedGivenTimingType                              (-200904)
#define DAQmxErrorRefClkSrcNotSupported                                                 (-200903)
#define DAQmxErrorDataVoltageLowAndHighIncompatible                                     (-200902)
#define DAQmxErrorInvalidCharInDigPatternString                                         (-200901)
#define DAQmxErrorCantUsePort3AloneGivenSampTimingTypeOn653x                            (-200900)
#define DAQmxErrorCantUsePort1AloneGivenSampTimingTypeOn653x                            (-200899)
#define DAQmxErrorPartialUseOfPhysicalLinesWithinPortNotSupported653x                   (-200898)
#define DAQmxErrorPhysicalChanNotSupportedGivenSampTimingType653x                       (-200897)
#define DAQmxErrorCanExportOnlyDigEdgeTrigs                                             (-200896)
#define DAQmxErrorRefTrigDigPatternSizeDoesNotMatchSourceSize                           (-200895)
#define DAQmxErrorStartTrigDigPatternSizeDoesNotMatchSourceSize                         (-200894)
#define DAQmxErrorChangeDetectionRisingAndFallingEdgeChanDontMatch                      (-200893)
#define DAQmxErrorPhysicalChansForChangeDetectionAndPatternMatch653x                    (-200892)
#define DAQmxErrorCanExportOnlyOnboardSampClk                                           (-200891)
#define DAQmxErrorInternalSampClkNotRisingEdge                                          (-200890)
#define DAQmxErrorRefTrigDigPatternChanNotInTask                                        (-200889)
#define DAQmxErrorRefTrigDigPatternChanNotTristated                                     (-200888)
#define DAQmxErrorStartTrigDigPatternChanNotInTask                                      (-200887)
#define DAQmxErrorStartTrigDigPatternChanNotTristated                                   (-200886)
#define DAQmxErrorPXIStarAndClock10Sync                                                 (-200885)
#define DAQmxErrorGlobalChanCannotBeSavedSoInteractiveEditsAllowed                      (-200884)
#define DAQmxErrorTaskCannotBeSavedSoInteractiveEditsAllowed                            (-200883)
#define DAQmxErrorInvalidGlobalChan                                                     (-200882)
#define DAQmxErrorEveryNSampsEventAlreadyRegistered                                     (-200881)
#define DAQmxErrorEveryNSampsEventIntervalZeroNotSupported                              (-200880)
#define DAQmxErrorChanSizeTooBigForU16PortWrite                                         (-200879)
#define DAQmxErrorChanSizeTooBigForU16PortRead                                          (-200878)
#define DAQmxErrorBufferSizeNotMultipleOfEveryNSampsEventIntervalWhenDMA                (-200877)
#define DAQmxErrorWriteWhenTaskNotRunningCOTicks                                        (-200876)
#define DAQmxErrorWriteWhenTaskNotRunningCOFreq                                         (-200875)
#define DAQmxErrorWriteWhenTaskNotRunningCOTime                                         (-200874)
#define DAQmxErrorAOMinMaxNotSupportedDACRangeTooSmall                                  (-200873)
#define DAQmxErrorAOMinMaxNotSupportedGivenDACRange                                     (-200872)
#define DAQmxErrorAOMinMaxNotSupportedGivenDACRangeAndOffsetVal                         (-200871)
#define DAQmxErrorAOMinMaxNotSupportedDACOffsetValInappropriate                         (-200870)
#define DAQmxErrorAOMinMaxNotSupportedGivenDACOffsetVal                                 (-200869)
#define DAQmxErrorAOMinMaxNotSupportedDACRefValTooSmall                                 (-200868)
#define DAQmxErrorAOMinMaxNotSupportedGivenDACRefVal                                    (-200867)
#define DAQmxErrorAOMinMaxNotSupportedGivenDACRefAndOffsetVal                           (-200866)
#define DAQmxErrorWhenAcqCompAndNumSampsPerChanExceedsOnBrdBufSize                      (-200865)
#define DAQmxErrorWhenAcqCompAndNoRefTrig                                               (-200864)
#define DAQmxErrorWaitForNextSampClkNotSupported                                        (-200863)
#define DAQmxErrorDevInUnidentifiedPXIChassis                                           (-200862)
#define DAQmxErrorMaxSoundPressureMicSensitivitRelatedAIPropertiesNotSupportedByDev     (-200861)
#define DAQmxErrorMaxSoundPressureAndMicSensitivityNotSupportedByDev                    (-200860)
#define DAQmxErrorAOBufferSizeZeroForSampClkTimingType                                  (-200859)
#define DAQmxErrorAOCallWriteBeforeStartForSampClkTimingType                            (-200858)
#define DAQmxErrorInvalidCalLowPassCutoffFreq                                           (-200857)
#define DAQmxErrorSimulationCannotBeDisabledForDevCreatedAsSimulatedDev                 (-200856)
#define DAQmxErrorCannotAddNewDevsAfterTaskConfiguration                                (-200855)
#define DAQmxErrorDifftSyncPulseSrcAndSampClkTimebaseSrcDevMultiDevTask                 (-200854)
#define DAQmxErrorTermWithoutDevInMultiDevTask                                          (-200853)
#define DAQmxErrorSyncNoDevSampClkTimebaseOrSyncPulseInPXISlot2                         (-200852)
#define DAQmxErrorPhysicalChanNotOnThisConnector                                        (-200851)
#define DAQmxErrorNumSampsToWaitNotGreaterThanZeroInScript                              (-200850)
#define DAQmxErrorNumSampsToWaitNotMultipleOfAlignmentQuantumInScript                   (-200849)
#define DAQmxErrorEveryNSamplesEventNotSupportedForNonBufferedTasks                     (-200848)
#define DAQmxErrorBufferedAndDataXferPIO                                                (-200847)
#define DAQmxErrorCannotWriteWhenAutoStartFalseAndTaskNotRunning                        (-200846)
#define DAQmxErrorNonBufferedAndDataXferInterrupts                                      (-200845)
#define DAQmxErrorWriteFailedMultipleCtrsWithFREQOUT                                    (-200844)
#define DAQmxErrorReadNotCompleteBefore3SampClkEdges                                    (-200843)
#define DAQmxErrorCtrHWTimedSinglePointAndDataXferNotProgIO                             (-200842)
#define DAQmxErrorPrescalerNot1ForInputTerminal                                         (-200841)
#define DAQmxErrorPrescalerNot1ForTimebaseSrc                                           (-200840)
#define DAQmxErrorSampClkTimingTypeWhenTristateIsFalse                                  (-200839)
#define DAQmxErrorOutputBufferSizeNotMultOfXferSize                                     (-200838)
#define DAQmxErrorSampPerChanNotMultOfXferSize                                          (-200837)
#define DAQmxErrorWriteToTEDSFailed                                                     (-200836)
#define DAQmxErrorSCXIDevNotUsablePowerTurnedOff                                        (-200835)
#define DAQmxErrorCannotReadWhenAutoStartFalseBufSizeZeroAndTaskNotRunning              (-200834)
#define DAQmxErrorCannotReadWhenAutoStartFalseHWTimedSinglePtAndTaskNotRunning          (-200833)
#define DAQmxErrorCannotReadWhenAutoStartFalseOnDemandAndTaskNotRunning                 (-200832)
#define DAQmxErrorSimultaneousAOWhenNotOnDemandTiming                                   (-200831)
#define DAQmxErrorMemMapAndSimultaneousAO                                               (-200830)
#define DAQmxErrorWriteFailedMultipleCOOutputTypes                                      (-200829)
#define DAQmxErrorWriteToTEDSNotSupportedOnRT                                           (-200828)
#define DAQmxErrorVirtualTEDSDataFileError                                              (-200827)
#define DAQmxErrorTEDSSensorDataError                                                   (-200826)
#define DAQmxErrorDataSizeMoreThanSizeOfEEPROMOnTEDS                                    (-200825)
#define DAQmxErrorPROMOnTEDSContainsBasicTEDSData                                       (-200824)
#define DAQmxErrorPROMOnTEDSAlreadyWritten                                              (-200823)
#define DAQmxErrorTEDSDoesNotContainPROM                                                (-200822)
#define DAQmxErrorHWTimedSinglePointNotSupportedAI                                      (-200821)
#define DAQmxErrorHWTimedSinglePointOddNumChansInAITask                                 (-200820)
#define DAQmxErrorCantUseOnlyOnBoardMemWithProgrammedIO                                 (-200819)
#define DAQmxErrorSwitchDevShutDownDueToHighTemp                                        (-200818)
#define DAQmxErrorExcitationNotSupportedWhenTermCfgDiff                                 (-200817)
#define DAQmxErrorTEDSMinElecValGEMaxElecVal                                            (-200816)
#define DAQmxErrorTEDSMinPhysValGEMaxPhysVal                                            (-200815)
#define DAQmxErrorCIOnboardClockNotSupportedAsInputTerm                                 (-200814)
#define DAQmxErrorInvalidSampModeForPositionMeas                                        (-200813)
#define DAQmxErrorTrigWhenAOHWTimedSinglePtSampMode                                     (-200812)
#define DAQmxErrorDAQmxCantUseStringDueToUnknownChar                                    (-200811)
#define DAQmxErrorDAQmxCantRetrieveStringDueToUnknownChar                               (-200810)
#define DAQmxErrorClearTEDSNotSupportedOnRT                                             (-200809)
#define DAQmxErrorCfgTEDSNotSupportedOnRT                                               (-200808)
#define DAQmxErrorProgFilterClkCfgdToDifferentMinPulseWidthBySameTask1PerDev            (-200807)
#define DAQmxErrorProgFilterClkCfgdToDifferentMinPulseWidthByAnotherTask1PerDev         (-200806)
#define DAQmxErrorNoLastExtCalDateTimeLastExtCalNotDAQmx                                (-200804)
#define DAQmxErrorCannotWriteNotStartedAutoStartFalseNotOnDemandHWTimedSglPt            (-200803)
#define DAQmxErrorCannotWriteNotStartedAutoStartFalseNotOnDemandBufSizeZero             (-200802)
#define DAQmxErrorCOInvalidTimingSrcDueToSignal                                         (-200801)
#define DAQmxErrorCIInvalidTimingSrcForSampClkDueToSampTimingType                       (-200800)
#define DAQmxErrorCIInvalidTimingSrcForEventCntDueToSampMode                            (-200799)
#define DAQmxErrorNoChangeDetectOnNonInputDigLineForDev                                 (-200798)
#define DAQmxErrorEmptyStringTermNameNotSupported                                       (-200797)
#define DAQmxErrorMemMapEnabledForHWTimedNonBufferedAO                                  (-200796)
#define DAQmxErrorDevOnboardMemOverflowDuringHWTimedNonBufferedGen                      (-200795)
#define DAQmxErrorCODAQmxWriteMultipleChans                                             (-200794)
#define DAQmxErrorCantMaintainExistingValueAOSync                                       (-200793)
#define DAQmxErrorMStudioMultiplePhysChansNotSupported                                  (-200792)
#define DAQmxErrorCantConfigureTEDSForChan                                              (-200791)
#define DAQmxErrorWriteDataTypeTooSmall                                                 (-200790)
#define DAQmxErrorReadDataTypeTooSmall                                                  (-200789)
#define DAQmxErrorMeasuredBridgeOffsetTooHigh                                           (-200788)
#define DAQmxErrorStartTrigConflictWithCOHWTimedSinglePt                                (-200787)
#define DAQmxErrorSampClkRateExtSampClkTimebaseRateMismatch                             (-200786)
#define DAQmxErrorInvalidTimingSrcDueToSampTimingType                                   (-200785)
#define DAQmxErrorVirtualTEDSFileNotFound                                               (-200784)
#define DAQmxErrorMStudioNoForwardPolyScaleCoeffs                                       (-200783)
#define DAQmxErrorMStudioNoReversePolyScaleCoeffs                                       (-200782)
#define DAQmxErrorMStudioNoPolyScaleCoeffsUseCalc                                       (-200781)
#define DAQmxErrorMStudioNoForwardPolyScaleCoeffsUseCalc                                (-200780)
#define DAQmxErrorMStudioNoReversePolyScaleCoeffsUseCalc                                (-200779)
#define DAQmxErrorCOSampModeSampTimingTypeSampClkConflict                               (-200778)
#define DAQmxErrorDevCannotProduceMinPulseWidth                                         (-200777)
#define DAQmxErrorCannotProduceMinPulseWidthGivenPropertyValues                         (-200776)
#define DAQmxErrorTermCfgdToDifferentMinPulseWidthByAnotherTask                         (-200775)
#define DAQmxErrorTermCfgdToDifferentMinPulseWidthByAnotherProperty                     (-200774)
#define DAQmxErrorDigSyncNotAvailableOnTerm                                             (-200773)
#define DAQmxErrorDigFilterNotAvailableOnTerm                                           (-200772)
#define DAQmxErrorDigFilterEnabledMinPulseWidthNotCfg                                   (-200771)
#define DAQmxErrorDigFilterAndSyncBothEnabled                                           (-200770)
#define DAQmxErrorHWTimedSinglePointAOAndDataXferNotProgIO                              (-200769)
#define DAQmxErrorNonBufferedAOAndDataXferNotProgIO                                     (-200768)
#define DAQmxErrorProgIODataXferForBufferedAO                                           (-200767)
#define DAQmxErrorTEDSLegacyTemplateIDInvalidOrUnsupported                              (-200766)
#define DAQmxErrorTEDSMappingMethodInvalidOrUnsupported                                 (-200765)
#define DAQmxErrorTEDSLinearMappingSlopeZero                                            (-200764)
#define DAQmxErrorAIInputBufferSizeNotMultOfXferSize                                    (-200763)
#define DAQmxErrorNoSyncPulseExtSampClkTimebase                                         (-200762)
#define DAQmxErrorNoSyncPulseAnotherTaskRunning                                         (-200761)
#define DAQmxErrorAOMinMaxNotInGainRange                                                (-200760)
#define DAQmxErrorAOMinMaxNotInDACRange                                                 (-200759)
#define DAQmxErrorDevOnlySupportsSampClkTimingAO                                        (-200758)
#define DAQmxErrorDevOnlySupportsSampClkTimingAI                                        (-200757)
#define DAQmxErrorTEDSIncompatibleSensorAndMeasType                                     (-200756)
#define DAQmxErrorTEDSMultipleCalTemplatesNotSupported                                  (-200755)
#define DAQmxErrorTEDSTemplateParametersNotSupported                                    (-200754)
#define DAQmxErrorParsingTEDSData                                                       (-200753)
#define DAQmxErrorMultipleActivePhysChansNotSupported                                   (-200752)
#define DAQmxErrorNoChansSpecdForChangeDetect                                           (-200751)
#define DAQmxErrorInvalidCalVoltageForGivenGain                                         (-200750)
#define DAQmxErrorInvalidCalGain                                                        (-200749)
#define DAQmxErrorMultipleWritesBetweenSampClks                                         (-200748)
#define DAQmxErrorInvalidAcqTypeForFREQOUT                                              (-200747)
#define DAQmxErrorSuitableTimebaseNotFoundTimeCombo2                                    (-200746)
#define DAQmxErrorSuitableTimebaseNotFoundFrequencyCombo2                               (-200745)
#define DAQmxErrorRefClkRateRefClkSrcMismatch                                           (-200744)
#define DAQmxErrorNoTEDSTerminalBlock                                                   (-200743)
#define DAQmxErrorCorruptedTEDSMemory                                                   (-200742)
#define DAQmxErrorTEDSNotSupported                                                      (-200741)
#define DAQmxErrorTimingSrcTaskStartedBeforeTimedLoop                                   (-200740)
#define DAQmxErrorPropertyNotSupportedForTimingSrc                                      (-200739)
#define DAQmxErrorTimingSrcDoesNotExist                                                 (-200738)
#define DAQmxErrorInputBufferSizeNotEqualSampsPerChanForFiniteSampMode                  (-200737)
#define DAQmxErrorFREQOUTCannotProduceDesiredFrequency2                                 (-200736)
#define DAQmxErrorExtRefClkRateNotSpecified                                             (-200735)
#define DAQmxErrorDeviceDoesNotSupportDMADataXferForNonBufferedAcq                      (-200734)
#define DAQmxErrorDigFilterMinPulseWidthSetWhenTristateIsFalse                          (-200733)
#define DAQmxErrorDigFilterEnableSetWhenTristateIsFalse                                 (-200732)
#define DAQmxErrorNoHWTimingWithOnDemand                                                (-200731)
#define DAQmxErrorCannotDetectChangesWhenTristateIsFalse                                (-200730)
#define DAQmxErrorCannotHandshakeWhenTristateIsFalse                                    (-200729)
#define DAQmxErrorLinesUsedForStaticInputNotForHandshakingControl                       (-200728)
#define DAQmxErrorLinesUsedForHandshakingControlNotForStaticInput                       (-200727)
#define DAQmxErrorLinesUsedForStaticInputNotForHandshakingInput                         (-200726)
#define DAQmxErrorLinesUsedForHandshakingInputNotForStaticInput                         (-200725)
#define DAQmxErrorDifferentDITristateValsForChansInTask                                 (-200724)
#define DAQmxErrorTimebaseCalFreqVarianceTooLarge                                       (-200723)
#define DAQmxErrorTimebaseCalFailedToConverge                                           (-200722)
#define DAQmxErrorInadequateResolutionForTimebaseCal                                    (-200721)
#define DAQmxErrorInvalidAOGainCalConst                                                 (-200720)
#define DAQmxErrorInvalidAOOffsetCalConst                                               (-200719)
#define DAQmxErrorInvalidAIGainCalConst                                                 (-200718)
#define DAQmxErrorInvalidAIOffsetCalConst                                               (-200717)
#define DAQmxErrorDigOutputOverrun                                                      (-200716)
#define DAQmxErrorDigInputOverrun                                                       (-200715)
#define DAQmxErrorAcqStoppedDriverCantXferDataFastEnough                                (-200714)
#define DAQmxErrorChansCantAppearInSameTask                                             (-200713)
#define DAQmxErrorInputCfgFailedBecauseWatchdogExpired                                  (-200712)
#define DAQmxErrorAnalogTrigChanNotExternal                                             (-200711)
#define DAQmxErrorTooManyChansForInternalAIInputSrc                                     (-200710)
#define DAQmxErrorTEDSSensorNotDetected                                                 (-200709)
#define DAQmxErrorPrptyGetSpecdActiveItemFailedDueToDifftValues                         (-200708)
#define DAQmxErrorRoutingDestTermPXIClk10InNotInSlot2                                   (-200706)
#define DAQmxErrorRoutingDestTermPXIStarXNotInSlot2                                     (-200705)
#define DAQmxErrorRoutingSrcTermPXIStarXNotInSlot2                                      (-200704)
#define DAQmxErrorRoutingSrcTermPXIStarInSlot16AndAbove                                 (-200703)
#define DAQmxErrorRoutingDestTermPXIStarInSlot16AndAbove                                (-200702)
#define DAQmxErrorRoutingDestTermPXIStarInSlot2                                         (-200701)
#define DAQmxErrorRoutingSrcTermPXIStarInSlot2                                          (-200700)
#define DAQmxErrorRoutingDestTermPXIChassisNotIdentified                                (-200699)
#define DAQmxErrorRoutingSrcTermPXIChassisNotIdentified                                 (-200698)
#define DAQmxErrorFailedToAcquireCalData                                                (-200697)
#define DAQmxErrorBridgeOffsetNullingCalNotSupported                                    (-200696)
#define DAQmxErrorAIMaxNotSpecified                                                     (-200695)
#define DAQmxErrorAIMinNotSpecified                                                     (-200694)
#define DAQmxErrorOddTotalBufferSizeToWrite                                             (-200693)
#define DAQmxErrorOddTotalNumSampsToWrite                                               (-200692)
#define DAQmxErrorBufferWithWaitMode                                                    (-200691)
#define DAQmxErrorBufferWithHWTimedSinglePointSampMode                                  (-200690)
#define DAQmxErrorCOWritePulseLowTicksNotSupported                                      (-200689)
#define DAQmxErrorCOWritePulseHighTicksNotSupported                                     (-200688)
#define DAQmxErrorCOWritePulseLowTimeOutOfRange                                         (-200687)
#define DAQmxErrorCOWritePulseHighTimeOutOfRange                                        (-200686)
#define DAQmxErrorCOWriteFreqOutOfRange                                                 (-200685)
#define DAQmxErrorCOWriteDutyCycleOutOfRange                                            (-200684)
#define DAQmxErrorInvalidInstallation                                                   (-200683)
#define DAQmxErrorRefTrigMasterSessionUnavailable                                       (-200682)
#define DAQmxErrorRouteFailedBecauseWatchdogExpired                                     (-200681)
#define DAQmxErrorDeviceShutDownDueToHighTemp                                           (-200680)
#define DAQmxErrorNoMemMapWhenHWTimedSinglePoint                                        (-200679)
#define DAQmxErrorWriteFailedBecauseWatchdogExpired                                     (-200678)
#define DAQmxErrorDifftInternalAIInputSrcs                                              (-200677)
#define DAQmxErrorDifftAIInputSrcInOneChanGroup                                         (-200676)
#define DAQmxErrorInternalAIInputSrcInMultChanGroups                                    (-200675)
#define DAQmxErrorSwitchOpFailedDueToPrevError                                          (-200674)
#define DAQmxErrorWroteMultiSampsUsingSingleSampWrite                                   (-200673)
#define DAQmxErrorMismatchedInputArraySizes                                             (-200672)
#define DAQmxErrorCantExceedRelayDriveLimit                                             (-200671)
#define DAQmxErrorDACRngLowNotEqualToMinusRefVal                                        (-200670)
#define DAQmxErrorCantAllowConnectDACToGnd                                              (-200669)
#define DAQmxErrorWatchdogTimeoutOutOfRangeAndNotSpecialVal                             (-200668)
#define DAQmxErrorNoWatchdogOutputOnPortReservedForInput                                (-200667)
#define DAQmxErrorNoInputOnPortCfgdForWatchdogOutput                                    (-200666)
#define DAQmxErrorWatchdogExpirationStateNotEqualForLinesInPort                         (-200665)
#define DAQmxErrorCannotPerformOpWhenTaskNotReserved                                    (-200664)
#define DAQmxErrorPowerupStateNotSupported                                              (-200663)
#define DAQmxErrorWatchdogTimerNotSupported                                             (-200662)
#define DAQmxErrorOpNotSupportedWhenRefClkSrcNone                                       (-200661)
#define DAQmxErrorSampClkRateUnavailable                                                (-200660)
#define DAQmxErrorPrptyGetSpecdSingleActiveChanFailedDueToDifftVals                     (-200659)
#define DAQmxErrorPrptyGetImpliedActiveChanFailedDueToDifftVals                         (-200658)
#define DAQmxErrorPrptyGetSpecdActiveChanFailedDueToDifftVals                           (-200657)
#define DAQmxErrorNoRegenWhenUsingBrdMem                                                (-200656)
#define DAQmxErrorNonbufferedReadMoreThanSampsPerChan                                   (-200655)
#define DAQmxErrorWatchdogExpirationTristateNotSpecdForEntirePort                       (-200654)
#define DAQmxErrorPowerupTristateNotSpecdForEntirePort                                  (-200653)
#define DAQmxErrorPowerupStateNotSpecdForEntirePort                                     (-200652)
#define DAQmxErrorCantSetWatchdogExpirationOnDigInChan                                  (-200651)
#define DAQmxErrorCantSetPowerupStateOnDigInChan                                        (-200650)
#define DAQmxErrorPhysChanNotInTask                                                     (-200649)
#define DAQmxErrorPhysChanDevNotInTask                                                  (-200648)
#define DAQmxErrorDigInputNotSupported                                                  (-200647)
#define DAQmxErrorDigFilterIntervalNotEqualForLines                                     (-200646)
#define DAQmxErrorDigFilterIntervalAlreadyCfgd                                          (-200645)
#define DAQmxErrorCantResetExpiredWatchdog                                              (-200644)
#define DAQmxErrorActiveChanTooManyLinesSpecdWhenGettingPrpty                           (-200643)
#define DAQmxErrorActiveChanNotSpecdWhenGetting1LinePrpty                               (-200642)
#define DAQmxErrorDigPrptyCannotBeSetPerLine                                            (-200641)
#define DAQmxErrorSendAdvCmpltAfterWaitForTrigInScanlist                                (-200640)
#define DAQmxErrorDisconnectionRequiredInScanlist                                       (-200639)
#define DAQmxErrorTwoWaitForTrigsAfterConnectionInScanlist                              (-200638)
#define DAQmxErrorActionSeparatorRequiredAfterBreakingConnectionInScanlist              (-200637)
#define DAQmxErrorConnectionInScanlistMustWaitForTrig                                   (-200636)
#define DAQmxErrorActionNotSupportedTaskNotWatchdog                                     (-200635)
#define DAQmxErrorWfmNameSameAsScriptName                                               (-200634)
#define DAQmxErrorScriptNameSameAsWfmName                                               (-200633)
#define DAQmxErrorDSFStopClock                                                          (-200632)
#define DAQmxErrorDSFReadyForStartClock                                                 (-200631)
#define DAQmxErrorWriteOffsetNotMultOfIncr                                              (-200630)
#define DAQmxErrorDifferentPrptyValsNotSupportedOnDev                                   (-200629)
#define DAQmxErrorRefAndPauseTrigConfigured                                             (-200628)
#define DAQmxErrorFailedToEnableHighSpeedInputClock                                     (-200627)
#define DAQmxErrorEmptyPhysChanInPowerUpStatesArray                                     (-200626)
#define DAQmxErrorActivePhysChanTooManyLinesSpecdWhenGettingPrpty                       (-200625)
#define DAQmxErrorActivePhysChanNotSpecdWhenGetting1LinePrpty                           (-200624)
#define DAQmxErrorPXIDevTempCausedShutDown                                              (-200623)
#define DAQmxErrorInvalidNumSampsToWrite                                                (-200622)
#define DAQmxErrorOutputFIFOUnderflow2                                                  (-200621)
#define DAQmxErrorRepeatedAIPhysicalChan                                                (-200620)
#define DAQmxErrorMultScanOpsInOneChassis                                               (-200619)
#define DAQmxErrorInvalidAIChanOrder                                                    (-200618)
#define DAQmxErrorReversePowerProtectionActivated                                       (-200617)
#define DAQmxErrorInvalidAsynOpHandle                                                   (-200616)
#define DAQmxErrorFailedToEnableHighSpeedOutput                                         (-200615)
#define DAQmxErrorCannotReadPastEndOfRecord                                             (-200614)
#define DAQmxErrorAcqStoppedToPreventInputBufferOverwriteOneDataXferMech                (-200613)
#define DAQmxErrorZeroBasedChanIndexInvalid                                             (-200612)
#define DAQmxErrorNoChansOfGivenTypeInTask                                              (-200611)
#define DAQmxErrorSampClkSrcInvalidForOutputValidForInput                               (-200610)
#define DAQmxErrorOutputBufSizeTooSmallToStartGen                                       (-200609)
#define DAQmxErrorInputBufSizeTooSmallToStartAcq                                        (-200608)
#define DAQmxErrorExportTwoSignalsOnSameTerminal                                        (-200607)
#define DAQmxErrorChanIndexInvalid                                                      (-200606)
#define DAQmxErrorRangeSyntaxNumberTooBig                                               (-200605)
#define DAQmxErrorNULLPtr                                                               (-200604)
#define DAQmxErrorScaledMinEqualMax                                                     (-200603)
#define DAQmxErrorPreScaledMinEqualMax                                                  (-200602)
#define DAQmxErrorPropertyNotSupportedForScaleType                                      (-200601)
#define DAQmxErrorChannelNameGenerationNumberTooBig                                     (-200600)
#define DAQmxErrorRepeatedNumberInScaledValues                                          (-200599)
#define DAQmxErrorRepeatedNumberInPreScaledValues                                       (-200598)
#define DAQmxErrorLinesAlreadyReservedForOutput                                         (-200597)
#define DAQmxErrorSwitchOperationChansSpanMultipleDevsInList                            (-200596)
#define DAQmxErrorInvalidIDInListAtBeginningOfSwitchOperation                           (-200595)
#define DAQmxErrorMStudioInvalidPolyDirection                                           (-200594)
#define DAQmxErrorMStudioPropertyGetWhileTaskNotVerified                                (-200593)
#define DAQmxErrorRangeWithTooManyObjects                                               (-200592)
#define DAQmxErrorCppDotNetAPINegativeBufferSize                                        (-200591)
#define DAQmxErrorCppCantRemoveInvalidEventHandler                                      (-200590)
#define DAQmxErrorCppCantRemoveEventHandlerTwice                                        (-200589)
#define DAQmxErrorCppCantRemoveOtherObjectsEventHandler                                 (-200588)
#define DAQmxErrorDigLinesReservedOrUnavailable                                         (-200587)
#define DAQmxErrorDSFFailedToResetStream                                                (-200586)
#define DAQmxErrorDSFReadyForOutputNotAsserted                                          (-200585)
#define DAQmxErrorSampToWritePerChanNotMultipleOfIncr                                   (-200584)
#define DAQmxErrorAOPropertiesCauseVoltageBelowMin                                      (-200583)
#define DAQmxErrorAOPropertiesCauseVoltageOverMax                                       (-200582)
#define DAQmxErrorPropertyNotSupportedWhenRefClkSrcNone                                 (-200581)
#define DAQmxErrorAIMaxTooSmall                                                         (-200580)
#define DAQmxErrorAIMaxTooLarge                                                         (-200579)
#define DAQmxErrorAIMinTooSmall                                                         (-200578)
#define DAQmxErrorAIMinTooLarge                                                         (-200577)
#define DAQmxErrorBuiltInCJCSrcNotSupported                                             (-200576)
#define DAQmxErrorTooManyPostTrigSampsPerChan                                           (-200575)
#define DAQmxErrorTrigLineNotFoundSingleDevRoute                                        (-200574)
#define DAQmxErrorDifferentInternalAIInputSources                                       (-200573)
#define DAQmxErrorDifferentAIInputSrcInOneChanGroup                                     (-200572)
#define DAQmxErrorInternalAIInputSrcInMultipleChanGroups                                (-200571)
#define DAQmxErrorCAPIChanIndexInvalid                                                  (-200570)
#define DAQmxErrorCollectionDoesNotMatchChanType                                        (-200569)
#define DAQmxErrorOutputCantStartChangedRegenerationMode                                (-200568)
#define DAQmxErrorOutputCantStartChangedBufferSize                                      (-200567)
#define DAQmxErrorChanSizeTooBigForU32PortWrite                                         (-200566)
#define DAQmxErrorChanSizeTooBigForU8PortWrite                                          (-200565)
#define DAQmxErrorChanSizeTooBigForU32PortRead                                          (-200564)
#define DAQmxErrorChanSizeTooBigForU8PortRead                                           (-200563)
#define DAQmxErrorInvalidDigDataWrite                                                   (-200562)
#define DAQmxErrorInvalidAODataWrite                                                    (-200561)
#define DAQmxErrorWaitUntilDoneDoesNotIndicateDone                                      (-200560)
#define DAQmxErrorMultiChanTypesInTask                                                  (-200559)
#define DAQmxErrorMultiDevsInTask                                                       (-200558)
#define DAQmxErrorCannotSetPropertyWhenTaskRunning                                      (-200557)
#define DAQmxErrorCannotGetPropertyWhenTaskNotCommittedOrRunning                        (-200556)
#define DAQmxErrorLeadingUnderscoreInString                                             (-200555)
#define DAQmxErrorTrailingSpaceInString                                                 (-200554)
#define DAQmxErrorLeadingSpaceInString                                                  (-200553)
#define DAQmxErrorInvalidCharInString                                                   (-200552)
#define DAQmxErrorDLLBecameUnlocked                                                     (-200551)
#define DAQmxErrorDLLLock                                                               (-200550)
#define DAQmxErrorSelfCalConstsInvalid                                                  (-200549)
#define DAQmxErrorInvalidTrigCouplingExceptForExtTrigChan                               (-200548)
#define DAQmxErrorWriteFailsBufferSizeAutoConfigured                                    (-200547)
#define DAQmxErrorExtCalAdjustExtRefVoltageFailed                                       (-200546)
#define DAQmxErrorSelfCalFailedExtNoiseOrRefVoltageOutOfCal                             (-200545)
#define DAQmxErrorExtCalTemperatureNotDAQmx                                             (-200544)
#define DAQmxErrorExtCalDateTimeNotDAQmx                                                (-200543)
#define DAQmxErrorSelfCalTemperatureNotDAQmx                                            (-200542)
#define DAQmxErrorSelfCalDateTimeNotDAQmx                                               (-200541)
#define DAQmxErrorDACRefValNotSet                                                       (-200540)
#define DAQmxErrorAnalogMultiSampWriteNotSupported                                      (-200539)
#define DAQmxErrorInvalidActionInControlTask                                            (-200538)
#define DAQmxErrorPolyCoeffsInconsistent                                                (-200537)
#define DAQmxErrorSensorValTooLow                                                       (-200536)
#define DAQmxErrorSensorValTooHigh                                                      (-200535)
#define DAQmxErrorWaveformNameTooLong                                                   (-200534)
#define DAQmxErrorIdentifierTooLongInScript                                             (-200533)
#define DAQmxErrorUnexpectedIDFollowingSwitchChanName                                   (-200532)
#define DAQmxErrorRelayNameNotSpecifiedInList                                           (-200531)
#define DAQmxErrorUnexpectedIDFollowingRelayNameInList                                  (-200530)
#define DAQmxErrorUnexpectedIDFollowingSwitchOpInList                                   (-200529)
#define DAQmxErrorInvalidLineGrouping                                                   (-200528)
#define DAQmxErrorCtrMinMax                                                             (-200527)
#define DAQmxErrorWriteChanTypeMismatch                                                 (-200526)
#define DAQmxErrorReadChanTypeMismatch                                                  (-200525)
#define DAQmxErrorWriteNumChansMismatch                                                 (-200524)
#define DAQmxErrorOneChanReadForMultiChanTask                                           (-200523)
#define DAQmxErrorCannotSelfCalDuringExtCal                                             (-200522)
#define DAQmxErrorMeasCalAdjustOscillatorPhaseDAC                                       (-200521)
#define DAQmxErrorInvalidCalConstCalADCAdjustment                                       (-200520)
#define DAQmxErrorInvalidCalConstOscillatorFreqDACValue                                 (-200519)
#define DAQmxErrorInvalidCalConstOscillatorPhaseDACValue                                (-200518)
#define DAQmxErrorInvalidCalConstOffsetDACValue                                         (-200517)
#define DAQmxErrorInvalidCalConstGainDACValue                                           (-200516)
#define DAQmxErrorInvalidNumCalADCReadsToAverage                                        (-200515)
#define DAQmxErrorInvalidCfgCalAdjustDirectPathOutputImpedance                          (-200514)
#define DAQmxErrorInvalidCfgCalAdjustMainPathOutputImpedance                            (-200513)
#define DAQmxErrorInvalidCfgCalAdjustMainPathPostAmpGainAndOffset                       (-200512)
#define DAQmxErrorInvalidCfgCalAdjustMainPathPreAmpGain                                 (-200511)
#define DAQmxErrorInvalidCfgCalAdjustMainPreAmpOffset                                   (-200510)
#define DAQmxErrorMeasCalAdjustCalADC                                                   (-200509)
#define DAQmxErrorMeasCalAdjustOscillatorFrequency                                      (-200508)
#define DAQmxErrorMeasCalAdjustDirectPathOutputImpedance                                (-200507)
#define DAQmxErrorMeasCalAdjustMainPathOutputImpedance                                  (-200506)
#define DAQmxErrorMeasCalAdjustDirectPathGain                                           (-200505)
#define DAQmxErrorMeasCalAdjustMainPathPostAmpGainAndOffset                             (-200504)
#define DAQmxErrorMeasCalAdjustMainPathPreAmpGain                                       (-200503)
#define DAQmxErrorMeasCalAdjustMainPathPreAmpOffset                                     (-200502)
#define DAQmxErrorInvalidDateTimeInEEPROM                                               (-200501)
#define DAQmxErrorUnableToLocateErrorResources                                          (-200500)
#define DAQmxErrorDotNetAPINotUnsigned32BitNumber                                       (-200499)
#define DAQmxErrorInvalidRangeOfObjectsSyntaxInString                                   (-200498)
#define DAQmxErrorAttemptToEnableLineNotPreviouslyDisabled                              (-200497)
#define DAQmxErrorInvalidCharInPattern                                                  (-200496)
#define DAQmxErrorIntermediateBufferFull                                                (-200495)
#define DAQmxErrorLoadTaskFailsBecauseNoTimingOnDev                                     (-200494)
#define DAQmxErrorCAPIReservedParamNotNULLNorEmpty                                      (-200493)
#define DAQmxErrorCAPIReservedParamNotNULL                                              (-200492)
#define DAQmxErrorCAPIReservedParamNotZero                                              (-200491)
#define DAQmxErrorSampleValueOutOfRange                                                 (-200490)
#define DAQmxErrorChanAlreadyInTask                                                     (-200489)
#define DAQmxErrorVirtualChanDoesNotExist                                               (-200488)
#define DAQmxErrorChanNotInTask                                                         (-200486)
#define DAQmxErrorTaskNotInDataNeighborhood                                             (-200485)
#define DAQmxErrorCantSaveTaskWithoutReplace                                            (-200484)
#define DAQmxErrorCantSaveChanWithoutReplace                                            (-200483)
#define DAQmxErrorDevNotInTask                                                          (-200482)
#define DAQmxErrorDevAlreadyInTask                                                      (-200481)
#define DAQmxErrorCanNotPerformOpWhileTaskRunning                                       (-200479)
#define DAQmxErrorCanNotPerformOpWhenNoChansInTask                                      (-200478)
#define DAQmxErrorCanNotPerformOpWhenNoDevInTask                                        (-200477)
#define DAQmxErrorCannotPerformOpWhenTaskNotRunning                                     (-200475)
#define DAQmxErrorOperationTimedOut                                                     (-200474)
#define DAQmxErrorCannotReadWhenAutoStartFalseAndTaskNotRunningOrCommitted              (-200473)
#define DAQmxErrorCannotWriteWhenAutoStartFalseAndTaskNotRunningOrCommitted             (-200472)
#define DAQmxErrorTaskVersionNew                                                        (-200470)
#define DAQmxErrorChanVersionNew                                                        (-200469)
#define DAQmxErrorEmptyString                                                           (-200467)
#define DAQmxErrorChannelSizeTooBigForPortReadType                                      (-200466)
#define DAQmxErrorChannelSizeTooBigForPortWriteType                                     (-200465)
#define DAQmxErrorExpectedNumberOfChannelsVerificationFailed                            (-200464)
#define DAQmxErrorNumLinesMismatchInReadOrWrite                                         (-200463)
#define DAQmxErrorOutputBufferEmpty                                                     (-200462)
#define DAQmxErrorInvalidChanName                                                       (-200461)
#define DAQmxErrorReadNoInputChansInTask                                                (-200460)
#define DAQmxErrorWriteNoOutputChansInTask                                              (-200459)
#define DAQmxErrorPropertyNotSupportedNotInputTask                                      (-200457)
#define DAQmxErrorPropertyNotSupportedNotOutputTask                                     (-200456)
#define DAQmxErrorGetPropertyNotInputBufferedTask                                       (-200455)
#define DAQmxErrorGetPropertyNotOutputBufferedTask                                      (-200454)
#define DAQmxErrorInvalidTimeoutVal                                                     (-200453)
#define DAQmxErrorAttributeNotSupportedInTaskContext                                    (-200452)
#define DAQmxErrorAttributeNotQueryableUnlessTaskIsCommitted                            (-200451)
#define DAQmxErrorAttributeNotSettableWhenTaskIsRunning                                 (-200450)
#define DAQmxErrorDACRngLowNotMinusRefValNorZero                                        (-200449)
#define DAQmxErrorDACRngHighNotEqualRefVal                                              (-200448)
#define DAQmxErrorUnitsNotFromCustomScale                                               (-200447)
#define DAQmxErrorInvalidVoltageReadingDuringExtCal                                     (-200446)
#define DAQmxErrorCalFunctionNotSupported                                               (-200445)
#define DAQmxErrorInvalidPhysicalChanForCal                                             (-200444)
#define DAQmxErrorExtCalNotComplete                                                     (-200443)
#define DAQmxErrorCantSyncToExtStimulusFreqDuringCal                                    (-200442)
#define DAQmxErrorUnableToDetectExtStimulusFreqDuringCal                                (-200441)
#define DAQmxErrorInvalidCloseAction                                                    (-200440)
#define DAQmxErrorExtCalFunctionOutsideExtCalSession                                    (-200439)
#define DAQmxErrorInvalidCalArea                                                        (-200438)
#define DAQmxErrorExtCalConstsInvalid                                                   (-200437)
#define DAQmxErrorStartTrigDelayWithExtSampClk                                          (-200436)
#define DAQmxErrorDelayFromSampClkWithExtConv                                           (-200435)
#define DAQmxErrorFewerThan2PreScaledVals                                               (-200434)
#define DAQmxErrorFewerThan2ScaledValues                                                (-200433)
#define DAQmxErrorPhysChanOutputType                                                    (-200432)
#define DAQmxErrorPhysChanMeasType                                                      (-200431)
#define DAQmxErrorInvalidPhysChanType                                                   (-200430)
#define DAQmxErrorLabVIEWEmptyTaskOrChans                                               (-200429)
#define DAQmxErrorLabVIEWInvalidTaskOrChans                                             (-200428)
#define DAQmxErrorInvalidRefClkRate                                                     (-200427)
#define DAQmxErrorInvalidExtTrigImpedance                                               (-200426)
#define DAQmxErrorHystTrigLevelAIMax                                                    (-200425)
#define DAQmxErrorLineNumIncompatibleWithVideoSignalFormat                              (-200424)
#define DAQmxErrorTrigWindowAIMinAIMaxCombo                                             (-200423)
#define DAQmxErrorTrigAIMinAIMax                                                        (-200422)
#define DAQmxErrorHystTrigLevelAIMin                                                    (-200421)
#define DAQmxErrorInvalidSampRateConsiderRIS                                            (-200420)
#define DAQmxErrorInvalidReadPosDuringRIS                                               (-200419)
#define DAQmxErrorImmedTrigDuringRISMode                                                (-200418)
#define DAQmxErrorTDCNotEnabledDuringRISMode                                            (-200417)
#define DAQmxErrorMultiRecWithRIS                                                       (-200416)
#define DAQmxErrorInvalidRefClkSrc                                                      (-200415)
#define DAQmxErrorInvalidSampClkSrc                                                     (-200414)
#define DAQmxErrorInsufficientOnBoardMemForNumRecsAndSamps                              (-200413)
#define DAQmxErrorInvalidAIAttenuation                                                  (-200412)
#define DAQmxErrorACCouplingNotAllowedWith50OhmImpedance                                (-200411)
#define DAQmxErrorInvalidRecordNum                                                      (-200410)
#define DAQmxErrorZeroSlopeLinearScale                                                  (-200409)
#define DAQmxErrorZeroReversePolyScaleCoeffs                                            (-200408)
#define DAQmxErrorZeroForwardPolyScaleCoeffs                                            (-200407)
#define DAQmxErrorNoReversePolyScaleCoeffs                                              (-200406)
#define DAQmxErrorNoForwardPolyScaleCoeffs                                              (-200405)
#define DAQmxErrorNoPolyScaleCoeffs                                                     (-200404)
#define DAQmxErrorReversePolyOrderLessThanNumPtsToCompute                               (-200403)
#define DAQmxErrorReversePolyOrderNotPositive                                           (-200402)
#define DAQmxErrorNumPtsToComputeNotPositive                                            (-200401)
#define DAQmxErrorWaveformLengthNotMultipleOfIncr                                       (-200400)
#define DAQmxErrorCAPINoExtendedErrorInfoAvailable                                      (-200399)
#define DAQmxErrorCVIFunctionNotFoundInDAQmxDLL                                         (-200398)
#define DAQmxErrorCVIFailedToLoadDAQmxDLL                                               (-200397)
#define DAQmxErrorNoCommonTrigLineForImmedRoute                                         (-200396)
#define DAQmxErrorNoCommonTrigLineForTaskRoute                                          (-200395)
#define DAQmxErrorF64PrptyValNotUnsignedInt                                             (-200394)
#define DAQmxErrorRegisterNotWritable                                                   (-200393)
#define DAQmxErrorInvalidOutputVoltageAtSampClkRate                                     (-200392)
#define DAQmxErrorStrobePhaseShiftDCMBecameUnlocked                                     (-200391)
#define DAQmxErrorDrivePhaseShiftDCMBecameUnlocked                                      (-200390)
#define DAQmxErrorClkOutPhaseShiftDCMBecameUnlocked                                     (-200389)
#define DAQmxErrorOutputBoardClkDCMBecameUnlocked                                       (-200388)
#define DAQmxErrorInputBoardClkDCMBecameUnlocked                                        (-200387)
#define DAQmxErrorInternalClkDCMBecameUnlocked                                          (-200386)
#define DAQmxErrorDCMLock                                                               (-200385)
#define DAQmxErrorDataLineReservedForDynamicOutput                                      (-200384)
#define DAQmxErrorInvalidRefClkSrcGivenSampClkSrc                                       (-200383)
#define DAQmxErrorNoPatternMatcherAvailable                                             (-200382)
#define DAQmxErrorInvalidDelaySampRateBelowPhaseShiftDCMThresh                          (-200381)
#define DAQmxErrorStrainGageCalibration                                                 (-200380)
#define DAQmxErrorInvalidExtClockFreqAndDivCombo                                        (-200379)
#define DAQmxErrorCustomScaleDoesNotExist                                               (-200378)
#define DAQmxErrorOnlyFrontEndChanOpsDuringScan                                         (-200377)
#define DAQmxErrorInvalidOptionForDigitalPortChannel                                    (-200376)
#define DAQmxErrorUnsupportedSignalTypeExportSignal                                     (-200375)
#define DAQmxErrorInvalidSignalTypeExportSignal                                         (-200374)
#define DAQmxErrorUnsupportedTrigTypeSendsSWTrig                                        (-200373)
#define DAQmxErrorInvalidTrigTypeSendsSWTrig                                            (-200372)
#define DAQmxErrorRepeatedPhysicalChan                                                  (-200371)
#define DAQmxErrorResourcesInUseForRouteInTask                                          (-200370)
#define DAQmxErrorResourcesInUseForRoute                                                (-200369)
#define DAQmxErrorRouteNotSupportedByHW                                                 (-200368)
#define DAQmxErrorResourcesInUseForExportSignalPolarity                                 (-200367)
#define DAQmxErrorResourcesInUseForInversionInTask                                      (-200366)
#define DAQmxErrorResourcesInUseForInversion                                            (-200365)
#define DAQmxErrorExportSignalPolarityNotSupportedByHW                                  (-200364)
#define DAQmxErrorInversionNotSupportedByHW                                             (-200363)
#define DAQmxErrorOverloadedChansExistNotRead                                           (-200362)
#define DAQmxErrorInputFIFOOverflow2                                                    (-200361)
#define DAQmxErrorCJCChanNotSpecd                                                       (-200360)
#define DAQmxErrorCtrExportSignalNotPossible                                            (-200359)
#define DAQmxErrorRefTrigWhenContinuous                                                 (-200358)
#define DAQmxErrorIncompatibleSensorOutputAndDeviceInputRanges                          (-200357)
#define DAQmxErrorCustomScaleNameUsed                                                   (-200356)
#define DAQmxErrorPropertyValNotSupportedByHW                                           (-200355)
#define DAQmxErrorPropertyValNotValidTermName                                           (-200354)
#define DAQmxErrorResourcesInUseForProperty                                             (-200353)
#define DAQmxErrorCJCChanAlreadyUsed                                                    (-200352)
#define DAQmxErrorForwardPolynomialCoefNotSpecd                                         (-200351)
#define DAQmxErrorTableScaleNumPreScaledAndScaledValsNotEqual                           (-200350)
#define DAQmxErrorTableScalePreScaledValsNotSpecd                                       (-200349)
#define DAQmxErrorTableScaleScaledValsNotSpecd                                          (-200348)
#define DAQmxErrorIntermediateBufferSizeNotMultipleOfIncr                               (-200347)
#define DAQmxErrorEventPulseWidthOutOfRange                                             (-200346)
#define DAQmxErrorEventDelayOutOfRange                                                  (-200345)
#define DAQmxErrorSampPerChanNotMultipleOfIncr                                          (-200344)
#define DAQmxErrorCannotCalculateNumSampsTaskNotStarted                                 (-200343)
#define DAQmxErrorScriptNotInMem                                                        (-200342)
#define DAQmxErrorOnboardMemTooSmall                                                    (-200341)
#define DAQmxErrorReadAllAvailableDataWithoutBuffer                                     (-200340)
#define DAQmxErrorPulseActiveAtStart                                                    (-200339)
#define DAQmxErrorCalTempNotSupported                                                   (-200338)
#define DAQmxErrorDelayFromSampClkTooLong                                               (-200337)
#define DAQmxErrorDelayFromSampClkTooShort                                              (-200336)
#define DAQmxErrorAIConvRateTooHigh                                                     (-200335)
#define DAQmxErrorDelayFromStartTrigTooLong                                             (-200334)
#define DAQmxErrorDelayFromStartTrigTooShort                                            (-200333)
#define DAQmxErrorSampRateTooHigh                                                       (-200332)
#define DAQmxErrorSampRateTooLow                                                        (-200331)
#define DAQmxErrorPFI0UsedForAnalogAndDigitalSrc                                        (-200330)
#define DAQmxErrorPrimingCfgFIFO                                                        (-200329)
#define DAQmxErrorCannotOpenTopologyCfgFile                                             (-200328)
#define DAQmxErrorInvalidDTInsideWfmDataType                                            (-200327)
#define DAQmxErrorRouteSrcAndDestSame                                                   (-200326)
#define DAQmxErrorReversePolynomialCoefNotSpecd                                         (-200325)
#define DAQmxErrorDevAbsentOrUnavailable                                                (-200324)
#define DAQmxErrorNoAdvTrigForMultiDevScan                                              (-200323)
#define DAQmxErrorInterruptsInsufficientDataXferMech                                    (-200322)
#define DAQmxErrorInvalidAttentuationBasedOnMinMax                                      (-200321)
#define DAQmxErrorCabledModuleCannotRouteSSH                                            (-200320)
#define DAQmxErrorCabledModuleCannotRouteConvClk                                        (-200319)
#define DAQmxErrorInvalidExcitValForScaling                                             (-200318)
#define DAQmxErrorNoDevMemForScript                                                     (-200317)
#define DAQmxErrorScriptDataUnderflow                                                   (-200316)
#define DAQmxErrorNoDevMemForWaveform                                                   (-200315)
#define DAQmxErrorStreamDCMBecameUnlocked                                               (-200314)
#define DAQmxErrorStreamDCMLock                                                         (-200313)
#define DAQmxErrorWaveformNotInMem                                                      (-200312)
#define DAQmxErrorWaveformWriteOutOfBounds                                              (-200311)
#define DAQmxErrorWaveformPreviouslyAllocated                                           (-200310)
#define DAQmxErrorSampClkTbMasterTbDivNotAppropriateForSampTbSrc                        (-200309)
#define DAQmxErrorSampTbRateSampTbSrcMismatch                                           (-200308)
#define DAQmxErrorMasterTbRateMasterTbSrcMismatch                                       (-200307)
#define DAQmxErrorSampsPerChanTooBig                                                    (-200306)
#define DAQmxErrorFinitePulseTrainNotPossible                                           (-200305)
#define DAQmxErrorExtMasterTimebaseRateNotSpecified                                     (-200304)
#define DAQmxErrorExtSampClkSrcNotSpecified                                             (-200303)
#define DAQmxErrorInputSignalSlowerThanMeasTime                                         (-200302)
#define DAQmxErrorCannotUpdatePulseGenProperty                                          (-200301)
#define DAQmxErrorInvalidTimingType                                                     (-200300)
#define DAQmxErrorPropertyUnavailWhenUsingOnboardMemory                                 (-200297)
#define DAQmxErrorCannotWriteAfterStartWithOnboardMemory                                (-200295)
#define DAQmxErrorNotEnoughSampsWrittenForInitialXferRqstCondition                      (-200294)
#define DAQmxErrorNoMoreSpace                                                           (-200293)
#define DAQmxErrorSamplesCanNotYetBeWritten                                             (-200292)
#define DAQmxErrorGenStoppedToPreventIntermediateBufferRegenOfOldSamples                (-200291)
#define DAQmxErrorGenStoppedToPreventRegenOfOldSamples                                  (-200290)
#define DAQmxErrorSamplesNoLongerWriteable                                              (-200289)
#define DAQmxErrorSamplesWillNeverBeGenerated                                           (-200288)
#define DAQmxErrorNegativeWriteSampleNumber                                             (-200287)
#define DAQmxErrorNoAcqStarted                                                          (-200286)
#define DAQmxErrorSamplesNotYetAvailable                                                (-200284)
#define DAQmxErrorAcqStoppedToPreventIntermediateBufferOverflow                         (-200283)
#define DAQmxErrorNoRefTrigConfigured                                                   (-200282)
#define DAQmxErrorCannotReadRelativeToRefTrigUntilDone                                  (-200281)
#define DAQmxErrorSamplesNoLongerAvailable                                              (-200279)
#define DAQmxErrorSamplesWillNeverBeAvailable                                           (-200278)
#define DAQmxErrorNegativeReadSampleNumber                                              (-200277)
#define DAQmxErrorExternalSampClkAndRefClkThruSameTerm                                  (-200276)
#define DAQmxErrorExtSampClkRateTooLowForClkIn                                          (-200275)
#define DAQmxErrorExtSampClkRateTooHighForBackplane                                     (-200274)
#define DAQmxErrorSampClkRateAndDivCombo                                                (-200273)
#define DAQmxErrorSampClkRateTooLowForDivDown                                           (-200272)
#define DAQmxErrorProductOfAOMinAndGainTooSmall                                         (-200271)
#define DAQmxErrorInterpolationRateNotPossible                                          (-200270)
#define DAQmxErrorOffsetTooLarge                                                        (-200269)
#define DAQmxErrorOffsetTooSmall                                                        (-200268)
#define DAQmxErrorProductOfAOMaxAndGainTooLarge                                         (-200267)
#define DAQmxErrorMinAndMaxNotSymmetric                                                 (-200266)
#define DAQmxErrorInvalidAnalogTrigSrc                                                  (-200265)
#define DAQmxErrorTooManyChansForAnalogRefTrig                                          (-200264)
#define DAQmxErrorTooManyChansForAnalogPauseTrig                                        (-200263)
#define DAQmxErrorTrigWhenOnDemandSampTiming                                            (-200262)
#define DAQmxErrorInconsistentAnalogTrigSettings                                        (-200261)
#define DAQmxErrorMemMapDataXferModeSampTimingCombo                                     (-200260)
#define DAQmxErrorInvalidJumperedAttr                                                   (-200259)
#define DAQmxErrorInvalidGainBasedOnMinMax                                              (-200258)
#define DAQmxErrorInconsistentExcit                                                     (-200257)
#define DAQmxErrorTopologyNotSupportedByCfgTermBlock                                    (-200256)
#define DAQmxErrorBuiltInTempSensorNotSupported                                         (-200255)
#define DAQmxErrorInvalidTerm                                                           (-200254)
#define DAQmxErrorCannotTristateTerm                                                    (-200253)
#define DAQmxErrorCannotTristateBusyTerm                                                (-200252)
#define DAQmxErrorNoDMAChansAvailable                                                   (-200251)
#define DAQmxErrorInvalidWaveformLengthWithinLoopInScript                               (-200250)
#define DAQmxErrorInvalidSubsetLengthWithinLoopInScript                                 (-200249)
#define DAQmxErrorMarkerPosInvalidForLoopInScript                                       (-200248)
#define DAQmxErrorIntegerExpectedInScript                                               (-200247)
#define DAQmxErrorPLLBecameUnlocked                                                     (-200246)
#define DAQmxErrorPLLLock                                                               (-200245)
#define DAQmxErrorDDCClkOutDCMBecameUnlocked                                            (-200244)
#define DAQmxErrorDDCClkOutDCMLock                                                      (-200243)
#define DAQmxErrorClkDoublerDCMBecameUnlocked                                           (-200242)
#define DAQmxErrorClkDoublerDCMLock                                                     (-200241)
#define DAQmxErrorSampClkDCMBecameUnlocked                                              (-200240)
#define DAQmxErrorSampClkDCMLock                                                        (-200239)
#define DAQmxErrorSampClkTimebaseDCMBecameUnlocked                                      (-200238)
#define DAQmxErrorSampClkTimebaseDCMLock                                                (-200237)
#define DAQmxErrorAttrCannotBeReset                                                     (-200236)
#define DAQmxErrorExplanationNotFound                                                   (-200235)
#define DAQmxErrorWriteBufferTooSmall                                                   (-200234)
#define DAQmxErrorSpecifiedAttrNotValid                                                 (-200233)
#define DAQmxErrorAttrCannotBeRead                                                      (-200232)
#define DAQmxErrorAttrCannotBeSet                                                       (-200231)
#define DAQmxErrorNULLPtrForC_Api                                                       (-200230)
#define DAQmxErrorReadBufferTooSmall                                                    (-200229)
#define DAQmxErrorBufferTooSmallForString                                               (-200228)
#define DAQmxErrorNoAvailTrigLinesOnDevice                                              (-200227)
#define DAQmxErrorTrigBusLineNotAvail                                                   (-200226)
#define DAQmxErrorCouldNotReserveRequestedTrigLine                                      (-200225)
#define DAQmxErrorTrigLineNotFound                                                      (-200224)
#define DAQmxErrorSCXI1126ThreshHystCombination                                         (-200223)
#define DAQmxErrorAcqStoppedToPreventInputBufferOverwrite                               (-200222)
#define DAQmxErrorTimeoutExceeded                                                       (-200221)
#define DAQmxErrorInvalidDeviceID                                                       (-200220)
#define DAQmxErrorInvalidAOChanOrder                                                    (-200219)
#define DAQmxErrorSampleTimingTypeAndDataXferMode                                       (-200218)
#define DAQmxErrorBufferWithOnDemandSampTiming                                          (-200217)
#define DAQmxErrorBufferAndDataXferMode                                                 (-200216)
#define DAQmxErrorMemMapAndBuffer                                                       (-200215)
#define DAQmxErrorNoAnalogTrigHW                                                        (-200214)
#define DAQmxErrorTooManyPretrigPlusMinPostTrigSamps                                    (-200213)
#define DAQmxErrorInconsistentUnitsSpecified                                            (-200212)
#define DAQmxErrorMultipleRelaysForSingleRelayOp                                        (-200211)
#define DAQmxErrorMultipleDevIDsPerChassisSpecifiedInList                               (-200210)
#define DAQmxErrorDuplicateDevIDInList                                                  (-200209)
#define DAQmxErrorInvalidRangeStatementCharInList                                       (-200208)
#define DAQmxErrorInvalidDeviceIDInList                                                 (-200207)
#define DAQmxErrorTriggerPolarityConflict                                               (-200206)
#define DAQmxErrorCannotScanWithCurrentTopology                                         (-200205)
#define DAQmxErrorUnexpectedIdentifierInFullySpecifiedPathInList                        (-200204)
#define DAQmxErrorSwitchCannotDriveMultipleTrigLines                                    (-200203)
#define DAQmxErrorInvalidRelayName                                                      (-200202)
#define DAQmxErrorSwitchScanlistTooBig                                                  (-200201)
#define DAQmxErrorSwitchChanInUse                                                       (-200200)
#define DAQmxErrorSwitchNotResetBeforeScan                                              (-200199)
#define DAQmxErrorInvalidTopology                                                       (-200198)
#define DAQmxErrorAttrNotSupported                                                      (-200197)
#define DAQmxErrorUnexpectedEndOfActionsInList                                          (-200196)
#define DAQmxErrorPowerBudgetExceeded                                                   (-200195)
#define DAQmxErrorHWUnexpectedlyPoweredOffAndOn                                         (-200194)
#define DAQmxErrorSwitchOperationNotSupported                                           (-200193)
#define DAQmxErrorOnlyContinuousScanSupported                                           (-200192)
#define DAQmxErrorSwitchDifferentTopologyWhenScanning                                   (-200191)
#define DAQmxErrorDisconnectPathNotSameAsExistingPath                                   (-200190)
#define DAQmxErrorConnectionNotPermittedOnChanReservedForRouting                        (-200189)
#define DAQmxErrorCannotConnectSrcChans                                                 (-200188)
#define DAQmxErrorCannotConnectChannelToItself                                          (-200187)
#define DAQmxErrorChannelNotReservedForRouting                                          (-200186)
#define DAQmxErrorCannotConnectChansDirectly                                            (-200185)
#define DAQmxErrorChansAlreadyConnected                                                 (-200184)
#define DAQmxErrorChanDuplicatedInPath                                                  (-200183)
#define DAQmxErrorNoPathToDisconnect                                                    (-200182)
#define DAQmxErrorInvalidSwitchChan                                                     (-200181)
#define DAQmxErrorNoPathAvailableBetween2SwitchChans                                    (-200180)
#define DAQmxErrorExplicitConnectionExists                                              (-200179)
#define DAQmxErrorSwitchDifferentSettlingTimeWhenScanning                               (-200178)
#define DAQmxErrorOperationOnlyPermittedWhileScanning                                   (-200177)
#define DAQmxErrorOperationNotPermittedWhileScanning                                    (-200176)
#define DAQmxErrorHardwareNotResponding                                                 (-200175)
#define DAQmxErrorInvalidSampAndMasterTimebaseRateCombo                                 (-200173)
#define DAQmxErrorNonZeroBufferSizeInProgIOXfer                                         (-200172)
#define DAQmxErrorVirtualChanNameUsed                                                   (-200171)
#define DAQmxErrorPhysicalChanDoesNotExist                                              (-200170)
#define DAQmxErrorMemMapOnlyForProgIOXfer                                               (-200169)
#define DAQmxErrorTooManyChans                                                          (-200168)
#define DAQmxErrorCannotHaveCJTempWithOtherChans                                        (-200167)
#define DAQmxErrorOutputBufferUnderwrite                                                (-200166)
#define DAQmxErrorSensorInvalidCompletionResistance                                     (-200163)
#define DAQmxErrorVoltageExcitIncompatibleWith2WireCfg                                  (-200162)
#define DAQmxErrorIntExcitSrcNotAvailable                                               (-200161)
#define DAQmxErrorCannotCreateChannelAfterTaskVerified                                  (-200160)
#define DAQmxErrorLinesReservedForSCXIControl                                           (-200159)
#define DAQmxErrorCouldNotReserveLinesForSCXIControl                                    (-200158)
#define DAQmxErrorCalibrationFailed                                                     (-200157)
#define DAQmxErrorReferenceFrequencyInvalid                                             (-200156)
#define DAQmxErrorReferenceResistanceInvalid                                            (-200155)
#define DAQmxErrorReferenceCurrentInvalid                                               (-200154)
#define DAQmxErrorReferenceVoltageInvalid                                               (-200153)
#define DAQmxErrorEEPROMDataInvalid                                                     (-200152)
#define DAQmxErrorCabledModuleNotCapableOfRoutingAI                                     (-200151)
#define DAQmxErrorChannelNotAvailableInParallelMode                                     (-200150)
#define DAQmxErrorExternalTimebaseRateNotKnownForDelay                                  (-200149)
#define DAQmxErrorFREQOUTCannotProduceDesiredFrequency                                  (-200148)
#define DAQmxErrorMultipleCounterInputTask                                              (-200147)
#define DAQmxErrorCounterStartPauseTriggerConflict                                      (-200146)
#define DAQmxErrorCounterInputPauseTriggerAndSampleClockInvalid                         (-200145)
#define DAQmxErrorCounterOutputPauseTriggerInvalid                                      (-200144)
#define DAQmxErrorCounterTimebaseRateNotSpecified                                       (-200143)
#define DAQmxErrorCounterTimebaseRateNotFound                                           (-200142)
#define DAQmxErrorCounterOverflow                                                       (-200141)
#define DAQmxErrorCounterNoTimebaseEdgesBetweenGates                                    (-200140)
#define DAQmxErrorCounterMaxMinRangeFreq                                                (-200139)
#define DAQmxErrorCounterMaxMinRangeTime                                                (-200138)
#define DAQmxErrorSuitableTimebaseNotFoundTimeCombo                                     (-200137)
#define DAQmxErrorSuitableTimebaseNotFoundFrequencyCombo                                (-200136)
#define DAQmxErrorInternalTimebaseSourceDivisorCombo                                    (-200135)
#define DAQmxErrorInternalTimebaseSourceRateCombo                                       (-200134)
#define DAQmxErrorInternalTimebaseRateDivisorSourceCombo                                (-200133)
#define DAQmxErrorExternalTimebaseRateNotknownForRate                                   (-200132)
#define DAQmxErrorAnalogTrigChanNotFirstInScanList                                      (-200131)
#define DAQmxErrorNoDivisorForExternalSignal                                            (-200130)
#define DAQmxErrorAttributeInconsistentAcrossRepeatedPhysicalChannels                   (-200128)
#define DAQmxErrorCannotHandshakeWithPort0                                              (-200127)
#define DAQmxErrorControlLineConflictOnPortC                                            (-200126)
#define DAQmxErrorLines4To7ConfiguredForOutput                                          (-200125)
#define DAQmxErrorLines4To7ConfiguredForInput                                           (-200124)
#define DAQmxErrorLines0To3ConfiguredForOutput                                          (-200123)
#define DAQmxErrorLines0To3ConfiguredForInput                                           (-200122)
#define DAQmxErrorPortConfiguredForOutput                                               (-200121)
#define DAQmxErrorPortConfiguredForInput                                                (-200120)
#define DAQmxErrorPortConfiguredForStaticDigitalOps                                     (-200119)
#define DAQmxErrorPortReservedForHandshaking                                            (-200118)
#define DAQmxErrorPortDoesNotSupportHandshakingDataIO                                   (-200117)
#define DAQmxErrorCannotTristate8255OutputLines                                         (-200116)
#define DAQmxErrorTemperatureOutOfRangeForCalibration                                   (-200113)
#define DAQmxErrorCalibrationHandleInvalid                                              (-200112)
#define DAQmxErrorPasswordRequired                                                      (-200111)
#define DAQmxErrorIncorrectPassword                                                     (-200110)
#define DAQmxErrorPasswordTooLong                                                       (-200109)
#define DAQmxErrorCalibrationSessionAlreadyOpen                                         (-200108)
#define DAQmxErrorSCXIModuleIncorrect                                                   (-200107)
#define DAQmxErrorAttributeInconsistentAcrossChannelsOnDevice                           (-200106)
#define DAQmxErrorSCXI1122ResistanceChanNotSupportedForCfg                              (-200105)
#define DAQmxErrorBracketPairingMismatchInList                                          (-200104)
#define DAQmxErrorInconsistentNumSamplesToWrite                                         (-200103)
#define DAQmxErrorIncorrectDigitalPattern                                               (-200102)
#define DAQmxErrorIncorrectNumChannelsToWrite                                           (-200101)
#define DAQmxErrorIncorrectReadFunction                                                 (-200100)
#define DAQmxErrorPhysicalChannelNotSpecified                                           (-200099)
#define DAQmxErrorMoreThanOneTerminal                                                   (-200098)
#define DAQmxErrorMoreThanOneActiveChannelSpecified                                     (-200097)
#define DAQmxErrorInvalidNumberSamplesToRead                                            (-200096)
#define DAQmxErrorAnalogWaveformExpected                                                (-200095)
#define DAQmxErrorDigitalWaveformExpected                                               (-200094)
#define DAQmxErrorActiveChannelNotSpecified                                             (-200093)
#define DAQmxErrorFunctionNotSupportedForDeviceTasks                                    (-200092)
#define DAQmxErrorFunctionNotInLibrary                                                  (-200091)
#define DAQmxErrorLibraryNotPresent                                                     (-200090)
#define DAQmxErrorDuplicateTask                                                         (-200089)
#define DAQmxErrorInvalidTask                                                           (-200088)
#define DAQmxErrorInvalidChannel                                                        (-200087)
#define DAQmxErrorInvalidSyntaxForPhysicalChannelRange                                  (-200086)
#define DAQmxErrorMinNotLessThanMax                                                     (-200082)
#define DAQmxErrorSampleRateNumChansConvertPeriodCombo                                  (-200081)
#define DAQmxErrorAODuringCounter1DMAConflict                                           (-200079)
#define DAQmxErrorAIDuringCounter0DMAConflict                                           (-200078)
#define DAQmxErrorInvalidAttributeValue                                                 (-200077)
#define DAQmxErrorSuppliedCurrentDataOutsideSpecifiedRange                              (-200076)
#define DAQmxErrorSuppliedVoltageDataOutsideSpecifiedRange                              (-200075)
#define DAQmxErrorCannotStoreCalConst                                                   (-200074)
#define DAQmxErrorSCXIModuleNotFound                                                    (-200073)
#define DAQmxErrorDuplicatePhysicalChansNotSupported                                    (-200072)
#define DAQmxErrorTooManyPhysicalChansInList                                            (-200071)
#define DAQmxErrorInvalidAdvanceEventTriggerType                                        (-200070)
#define DAQmxErrorDeviceIsNotAValidSwitch                                               (-200069)
#define DAQmxErrorDeviceDoesNotSupportScanning                                          (-200068)
#define DAQmxErrorScanListCannotBeTimed                                                 (-200067)
#define DAQmxErrorConnectOperatorInvalidAtPointInList                                   (-200066)
#define DAQmxErrorUnexpectedSwitchActionInList                                          (-200065)
#define DAQmxErrorUnexpectedSeparatorInList                                             (-200064)
#define DAQmxErrorExpectedTerminatorInList                                              (-200063)
#define DAQmxErrorExpectedConnectOperatorInList                                         (-200062)
#define DAQmxErrorExpectedSeparatorInList                                               (-200061)
#define DAQmxErrorFullySpecifiedPathInListContainsRange                                 (-200060)
#define DAQmxErrorConnectionSeparatorAtEndOfList                                        (-200059)
#define DAQmxErrorIdentifierInListTooLong                                               (-200058)
#define DAQmxErrorDuplicateDeviceIDInListWhenSettling                                   (-200057)
#define DAQmxErrorChannelNameNotSpecifiedInList                                         (-200056)
#define DAQmxErrorDeviceIDNotSpecifiedInList                                            (-200055)
#define DAQmxErrorSemicolonDoesNotFollowRangeInList                                     (-200054)
#define DAQmxErrorSwitchActionInListSpansMultipleDevices                                (-200053)
#define DAQmxErrorRangeWithoutAConnectActionInList                                      (-200052)
#define DAQmxErrorInvalidIdentifierFollowingSeparatorInList                             (-200051)
#define DAQmxErrorInvalidChannelNameInList                                              (-200050)
#define DAQmxErrorInvalidNumberInRepeatStatementInList                                  (-200049)
#define DAQmxErrorInvalidTriggerLineInList                                              (-200048)
#define DAQmxErrorInvalidIdentifierInListFollowingDeviceID                              (-200047)
#define DAQmxErrorInvalidIdentifierInListAtEndOfSwitchAction                            (-200046)
#define DAQmxErrorDeviceRemoved                                                         (-200045)
#define DAQmxErrorRoutingPathNotAvailable                                               (-200044)
#define DAQmxErrorRoutingHardwareBusy                                                   (-200043)
#define DAQmxErrorRequestedSignalInversionForRoutingNotPossible                         (-200042)
#define DAQmxErrorInvalidRoutingDestinationTerminalName                                 (-200041)
#define DAQmxErrorInvalidRoutingSourceTerminalName                                      (-200040)
#define DAQmxErrorRoutingNotSupportedForDevice                                          (-200039)
#define DAQmxErrorWaitIsLastInstructionOfLoopInScript                                   (-200038)
#define DAQmxErrorClearIsLastInstructionOfLoopInScript                                  (-200037)
#define DAQmxErrorInvalidLoopIterationsInScript                                         (-200036)
#define DAQmxErrorRepeatLoopNestingTooDeepInScript                                      (-200035)
#define DAQmxErrorMarkerPositionOutsideSubsetInScript                                   (-200034)
#define DAQmxErrorSubsetStartOffsetNotAlignedInScript                                   (-200033)
#define DAQmxErrorInvalidSubsetLengthInScript                                           (-200032)
#define DAQmxErrorMarkerPositionNotAlignedInScript                                      (-200031)
#define DAQmxErrorSubsetOutsideWaveformInScript                                         (-200030)
#define DAQmxErrorMarkerOutsideWaveformInScript                                         (-200029)
#define DAQmxErrorWaveformInScriptNotInMem                                              (-200028)
#define DAQmxErrorKeywordExpectedInScript                                               (-200027)
#define DAQmxErrorBufferNameExpectedInScript                                            (-200026)
#define DAQmxErrorProcedureNameExpectedInScript                                         (-200025)
#define DAQmxErrorScriptHasInvalidIdentifier                                            (-200024)
#define DAQmxErrorScriptHasInvalidCharacter                                             (-200023)
#define DAQmxErrorResourceAlreadyReserved                                               (-200022)
#define DAQmxErrorSelfTestFailed                                                        (-200020)
#define DAQmxErrorADCOverrun                                                            (-200019)
#define DAQmxErrorDACUnderflow                                                          (-200018)
#define DAQmxErrorInputFIFOUnderflow                                                    (-200017)
#define DAQmxErrorOutputFIFOUnderflow                                                   (-200016)
#define DAQmxErrorSCXISerialCommunication                                               (-200015)
#define DAQmxErrorDigitalTerminalSpecifiedMoreThanOnce                                  (-200014)
#define DAQmxErrorDigitalOutputNotSupported                                             (-200012)
#define DAQmxErrorInconsistentChannelDirections                                         (-200011)
#define DAQmxErrorInputFIFOOverflow                                                     (-200010)
#define DAQmxErrorTimeStampOverwritten                                                  (-200009)
#define DAQmxErrorStopTriggerHasNotOccurred                                             (-200008)
#define DAQmxErrorRecordNotAvailable                                                    (-200007)
#define DAQmxErrorRecordOverwritten                                                     (-200006)
#define DAQmxErrorDataNotAvailable                                                      (-200005)
#define DAQmxErrorDataOverwrittenInDeviceMemory                                         (-200004)
#define DAQmxErrorDuplicatedChannel                                                     (-200003)
#define DAQmxWarningTimestampCounterRolledOver                                           (200003)
#define DAQmxWarningInputTerminationOverloaded                                           (200004)
#define DAQmxWarningADCOverloaded                                                        (200005)
#define DAQmxWarningPLLUnlocked                                                          (200007)
#define DAQmxWarningCounter0DMADuringAIConflict                                          (200008)
#define DAQmxWarningCounter1DMADuringAOConflict                                          (200009)
#define DAQmxWarningStoppedBeforeDone                                                    (200010)
#define DAQmxWarningRateViolatesSettlingTime                                             (200011)
#define DAQmxWarningRateViolatesMaxADCRate                                               (200012)
#define DAQmxWarningUserDefInfoStringTooLong                                             (200013)
#define DAQmxWarningTooManyInterruptsPerSecond                                           (200014)
#define DAQmxWarningPotentialGlitchDuringWrite                                           (200015)
#define DAQmxWarningDevNotSelfCalibratedWithDAQmx                                        (200016)
#define DAQmxWarningAISampRateTooLow                                                     (200017)
#define DAQmxWarningAIConvRateTooLow                                                     (200018)
#define DAQmxWarningReadOffsetCoercion                                                   (200019)
#define DAQmxWarningPretrigCoercion                                                      (200020)
#define DAQmxWarningSampValCoercedToMax                                                  (200021)
#define DAQmxWarningSampValCoercedToMin                                                  (200022)
#define DAQmxWarningPropertyVersionNew                                                   (200024)
#define DAQmxWarningUserDefinedInfoTooLong                                               (200025)
#define DAQmxWarningCAPIStringTruncatedToFitBuffer                                       (200026)
#define DAQmxWarningSampClkRateTooLow                                                    (200027)
#define DAQmxWarningPossiblyInvalidCTRSampsInFiniteDMAAcq                                (200028)
#define DAQmxWarningRISAcqCompletedSomeBinsNotFilled                                     (200029)
#define DAQmxWarningPXIDevTempExceedsMaxOpTemp                                           (200030)
#define DAQmxWarningOutputGainTooLowForRFFreq                                            (200031)
#define DAQmxWarningOutputGainTooHighForRFFreq                                           (200032)
#define DAQmxWarningMultipleWritesBetweenSampClks                                        (200033)
#define DAQmxWarningDeviceMayShutDownDueToHighTemp                                       (200034)
#define DAQmxWarningRateViolatesMinADCRate                                               (200035)
#define DAQmxWarningSampClkRateAboveDevSpecs                                             (200036)
#define DAQmxWarningCOPrevDAQmxWriteSettingsOverwrittenForHWTimedSinglePoint             (200037)
#define DAQmxWarningLowpassFilterSettlingTimeExceedsUserTimeBetween2ADCConversions       (200038)
#define DAQmxWarningLowpassFilterSettlingTimeExceedsDriverTimeBetween2ADCConversions     (200039)
#define DAQmxWarningSampClkRateViolatesSettlingTimeForGen                                (200040)
#define DAQmxWarningInvalidCalConstValueForAI                                            (200041)
#define DAQmxWarningInvalidCalConstValueForAO                                            (200042)
#define DAQmxWarningChanCalExpired                                                       (200043)
#define DAQmxWarningUnrecognizedEnumValueEncounteredInStorage                            (200044)
#define DAQmxWarningReadNotCompleteBeforeSampClk                                         (209800)
#define DAQmxWarningWriteNotCompleteBeforeSampClk                                        (209801)
#define DAQmxWarningWaitForNextSampClkDetectedMissedSampClk                              (209802)
#define DAQmxErrorInvalidSignalModifier_Routing                                          (-89150)
#define DAQmxErrorRoutingDestTermPXIClk10InNotInSlot2_Routing                            (-89149)
#define DAQmxErrorRoutingDestTermPXIStarXNotInSlot2_Routing                              (-89148)
#define DAQmxErrorRoutingSrcTermPXIStarXNotInSlot2_Routing                               (-89147)
#define DAQmxErrorRoutingSrcTermPXIStarInSlot16AndAbove_Routing                          (-89146)
#define DAQmxErrorRoutingDestTermPXIStarInSlot16AndAbove_Routing                         (-89145)
#define DAQmxErrorRoutingDestTermPXIStarInSlot2_Routing                                  (-89144)
#define DAQmxErrorRoutingSrcTermPXIStarInSlot2_Routing                                   (-89143)
#define DAQmxErrorRoutingDestTermPXIChassisNotIdentified_Routing                         (-89142)
#define DAQmxErrorRoutingSrcTermPXIChassisNotIdentified_Routing                          (-89141)
#define DAQmxErrorTrigLineNotFoundSingleDevRoute_Routing                                 (-89140)
#define DAQmxErrorNoCommonTrigLineForRoute_Routing                                       (-89139)
#define DAQmxErrorResourcesInUseForRouteInTask_Routing                                   (-89138)
#define DAQmxErrorResourcesInUseForRoute_Routing                                         (-89137)
#define DAQmxErrorRouteNotSupportedByHW_Routing                                          (-89136)
#define DAQmxErrorResourcesInUseForInversionInTask_Routing                               (-89135)
#define DAQmxErrorResourcesInUseForInversion_Routing                                     (-89134)
#define DAQmxErrorInversionNotSupportedByHW_Routing                                      (-89133)
#define DAQmxErrorResourcesInUseForProperty_Routing                                      (-89132)
#define DAQmxErrorRouteSrcAndDestSame_Routing                                            (-89131)
#define DAQmxErrorDevAbsentOrUnavailable_Routing                                         (-89130)
#define DAQmxErrorInvalidTerm_Routing                                                    (-89129)
#define DAQmxErrorCannotTristateTerm_Routing                                             (-89128)
#define DAQmxErrorCannotTristateBusyTerm_Routing                                         (-89127)
#define DAQmxErrorCouldNotReserveRequestedTrigLine_Routing                               (-89126)
#define DAQmxErrorTrigLineNotFound_Routing                                               (-89125)
#define DAQmxErrorRoutingPathNotAvailable_Routing                                        (-89124)
#define DAQmxErrorRoutingHardwareBusy_Routing                                            (-89123)
#define DAQmxErrorRequestedSignalInversionForRoutingNotPossible_Routing                  (-89122)
#define DAQmxErrorInvalidRoutingDestinationTerminalName_Routing                          (-89121)
#define DAQmxErrorInvalidRoutingSourceTerminalName_Routing                               (-89120)
#define DAQmxErrorCouldNotConnectToServer_Routing                                        (-88900)
#define DAQmxErrorDeviceNameNotFound_Routing                                             (-88717)
#define DAQmxErrorLocalRemoteDriverVersionMismatch_Routing                               (-88716)
#define DAQmxErrorDuplicateDeviceName_Routing                                            (-88715)
#define DAQmxErrorRuntimeAborting_Routing                                                (-88710)
#define DAQmxErrorRuntimeAborted_Routing                                                 (-88709)
#define DAQmxErrorResourceNotInPool_Routing                                              (-88708)
#define DAQmxErrorDriverDeviceGUIDNotFound_Routing                                       (-88705)
#define DAQmxErrorValueInvalid                                                           (-54023)
#define DAQmxErrorValueNotInSet                                                          (-54022)
#define DAQmxErrorValueOutOfRange                                                        (-54021)
#define DAQmxErrorTypeUnknown                                                            (-54020)
#define DAQmxErrorInterconnectBridgeRouteReserved                                        (-54012)
#define DAQmxErrorInterconnectBridgeRouteNotPossible                                     (-54011)
#define DAQmxErrorInterconnectLineReserved                                               (-54010)
#define DAQmxErrorInterconnectBusNotFound                                                (-54002)
#define DAQmxErrorEndpointNotFound                                                       (-54001)
#define DAQmxErrorResourceNotFound                                                       (-54000)
#define DAQmxErrorPALBusResetOccurred                                                    (-50800)
#define DAQmxErrorPALWaitInterrupted                                                     (-50700)
#define DAQmxErrorPALMessageUnderflow                                                    (-50651)
#define DAQmxErrorPALMessageOverflow                                                     (-50650)
#define DAQmxErrorPALThreadAlreadyDead                                                   (-50604)
#define DAQmxErrorPALThreadStackSizeNotSupported                                         (-50603)
#define DAQmxErrorPALThreadControllerIsNotThreadCreator                                  (-50602)
#define DAQmxErrorPALThreadHasNoThreadObject                                             (-50601)
#define DAQmxErrorPALThreadCouldNotRun                                                   (-50600)
#define DAQmxErrorPALSyncTimedOut                                                        (-50550)
#define DAQmxErrorPALReceiverSocketInvalid                                               (-50503)
#define DAQmxErrorPALSocketListenerInvalid                                               (-50502)
#define DAQmxErrorPALSocketListenerAlreadyRegistered                                     (-50501)
#define DAQmxErrorPALDispatcherAlreadyExported                                           (-50500)
#define DAQmxErrorPALDMALinkEventMissed                                                  (-50450)
#define DAQmxErrorPALBusError                                                            (-50413)
#define DAQmxErrorPALRetryLimitExceeded                                                  (-50412)
#define DAQmxErrorPALTransferOverread                                                    (-50411)
#define DAQmxErrorPALTransferOverwritten                                                 (-50410)
#define DAQmxErrorPALPhysicalBufferFull                                                  (-50409)
#define DAQmxErrorPALPhysicalBufferEmpty                                                 (-50408)
#define DAQmxErrorPALLogicalBufferFull                                                   (-50407)
#define DAQmxErrorPALLogicalBufferEmpty                                                  (-50406)
#define DAQmxErrorPALTransferAborted                                                     (-50405)
#define DAQmxErrorPALTransferStopped                                                     (-50404)
#define DAQmxErrorPALTransferInProgress                                                  (-50403)
#define DAQmxErrorPALTransferNotInProgress                                               (-50402)
#define DAQmxErrorPALCommunicationsFault                                                 (-50401)
#define DAQmxErrorPALTransferTimedOut                                                    (-50400)
#define DAQmxErrorPALMemoryBlockCheckFailed                                              (-50354)
#define DAQmxErrorPALMemoryPageLockFailed                                                (-50353)
#define DAQmxErrorPALMemoryFull                                                          (-50352)
#define DAQmxErrorPALMemoryAlignmentFault                                                (-50351)
#define DAQmxErrorPALMemoryConfigurationFault                                            (-50350)
#define DAQmxErrorPALDeviceInitializationFault                                           (-50303)
#define DAQmxErrorPALDeviceNotSupported                                                  (-50302)
#define DAQmxErrorPALDeviceUnknown                                                       (-50301)
#define DAQmxErrorPALDeviceNotFound                                                      (-50300)
#define DAQmxErrorPALFeatureDisabled                                                     (-50265)
#define DAQmxErrorPALComponentBusy                                                       (-50264)
#define DAQmxErrorPALComponentAlreadyInstalled                                           (-50263)
#define DAQmxErrorPALComponentNotUnloadable                                              (-50262)
#define DAQmxErrorPALComponentNeverLoaded                                                (-50261)
#define DAQmxErrorPALComponentAlreadyLoaded                                              (-50260)
#define DAQmxErrorPALComponentCircularDependency                                         (-50259)
#define DAQmxErrorPALComponentInitializationFault                                        (-50258)
#define DAQmxErrorPALComponentImageCorrupt                                               (-50257)
#define DAQmxErrorPALFeatureNotSupported                                                 (-50256)
#define DAQmxErrorPALFunctionNotFound                                                    (-50255)
#define DAQmxErrorPALFunctionObsolete                                                    (-50254)
#define DAQmxErrorPALComponentTooNew                                                     (-50253)
#define DAQmxErrorPALComponentTooOld                                                     (-50252)
#define DAQmxErrorPALComponentNotFound                                                   (-50251)
#define DAQmxErrorPALVersionMismatch                                                     (-50250)
#define DAQmxErrorPALFileFault                                                           (-50209)
#define DAQmxErrorPALFileWriteFault                                                      (-50208)
#define DAQmxErrorPALFileReadFault                                                       (-50207)
#define DAQmxErrorPALFileSeekFault                                                       (-50206)
#define DAQmxErrorPALFileCloseFault                                                      (-50205)
#define DAQmxErrorPALFileOpenFault                                                       (-50204)
#define DAQmxErrorPALDiskFull                                                            (-50203)
#define DAQmxErrorPALOSFault                                                             (-50202)
#define DAQmxErrorPALOSInitializationFault                                               (-50201)
#define DAQmxErrorPALOSUnsupported                                                       (-50200)
#define DAQmxErrorPALCalculationOverflow                                                 (-50175)
#define DAQmxErrorPALHardwareFault                                                       (-50152)
#define DAQmxErrorPALFirmwareFault                                                       (-50151)
#define DAQmxErrorPALSoftwareFault                                                       (-50150)
#define DAQmxErrorPALMessageQueueFull                                                    (-50108)
#define DAQmxErrorPALResourceAmbiguous                                                   (-50107)
#define DAQmxErrorPALResourceBusy                                                        (-50106)
#define DAQmxErrorPALResourceInitialized                                                 (-50105)
#define DAQmxErrorPALResourceNotInitialized                                              (-50104)
#define DAQmxErrorPALResourceReserved                                                    (-50103)
#define DAQmxErrorPALResourceNotReserved                                                 (-50102)
#define DAQmxErrorPALResourceNotAvailable                                                (-50101)
#define DAQmxErrorPALResourceOwnedBySystem                                               (-50100)
#define DAQmxErrorPALBadToken                                                            (-50020)
#define DAQmxErrorPALBadThreadMultitask                                                  (-50019)
#define DAQmxErrorPALBadLibrarySpecifier                                                 (-50018)
#define DAQmxErrorPALBadAddressSpace                                                     (-50017)
#define DAQmxErrorPALBadWindowType                                                       (-50016)
#define DAQmxErrorPALBadAddressClass                                                     (-50015)
#define DAQmxErrorPALBadWriteCount                                                       (-50014)
#define DAQmxErrorPALBadWriteOffset                                                      (-50013)
#define DAQmxErrorPALBadWriteMode                                                        (-50012)
#define DAQmxErrorPALBadReadCount                                                        (-50011)
#define DAQmxErrorPALBadReadOffset                                                       (-50010)
#define DAQmxErrorPALBadReadMode                                                         (-50009)
#define DAQmxErrorPALBadCount                                                            (-50008)
#define DAQmxErrorPALBadOffset                                                           (-50007)
#define DAQmxErrorPALBadMode                                                             (-50006)
#define DAQmxErrorPALBadDataSize                                                         (-50005)
#define DAQmxErrorPALBadPointer                                                          (-50004)
#define DAQmxErrorPALBadSelector                                                         (-50003)
#define DAQmxErrorPALBadDevice                                                           (-50002)
#define DAQmxErrorPALIrrelevantAttribute                                                 (-50001)
#define DAQmxErrorPALValueConflict                                                       (-50000)
#define DAQmxWarningPALValueConflict                                                      (50000)
#define DAQmxWarningPALIrrelevantAttribute                                                (50001)
#define DAQmxWarningPALBadDevice                                                          (50002)
#define DAQmxWarningPALBadSelector                                                        (50003)
#define DAQmxWarningPALBadPointer                                                         (50004)
#define DAQmxWarningPALBadDataSize                                                        (50005)
#define DAQmxWarningPALBadMode                                                            (50006)
#define DAQmxWarningPALBadOffset                                                          (50007)
#define DAQmxWarningPALBadCount                                                           (50008)
#define DAQmxWarningPALBadReadMode                                                        (50009)
#define DAQmxWarningPALBadReadOffset                                                      (50010)
#define DAQmxWarningPALBadReadCount                                                       (50011)
#define DAQmxWarningPALBadWriteMode                                                       (50012)
#define DAQmxWarningPALBadWriteOffset                                                     (50013)
#define DAQmxWarningPALBadWriteCount                                                      (50014)
#define DAQmxWarningPALBadAddressClass                                                    (50015)
#define DAQmxWarningPALBadWindowType                                                      (50016)
#define DAQmxWarningPALBadThreadMultitask                                                 (50019)
#define DAQmxWarningPALResourceOwnedBySystem                                              (50100)
#define DAQmxWarningPALResourceNotAvailable                                               (50101)
#define DAQmxWarningPALResourceNotReserved                                                (50102)
#define DAQmxWarningPALResourceReserved                                                   (50103)
#define DAQmxWarningPALResourceNotInitialized                                             (50104)
#define DAQmxWarningPALResourceInitialized                                                (50105)
#define DAQmxWarningPALResourceBusy                                                       (50106)
#define DAQmxWarningPALResourceAmbiguous                                                  (50107)
#define DAQmxWarningPALFirmwareFault                                                      (50151)
#define DAQmxWarningPALHardwareFault                                                      (50152)
#define DAQmxWarningPALOSUnsupported                                                      (50200)
#define DAQmxWarningPALOSFault                                                            (50202)
#define DAQmxWarningPALFunctionObsolete                                                   (50254)
#define DAQmxWarningPALFunctionNotFound                                                   (50255)
#define DAQmxWarningPALFeatureNotSupported                                                (50256)
#define DAQmxWarningPALComponentInitializationFault                                       (50258)
#define DAQmxWarningPALComponentAlreadyLoaded                                             (50260)
#define DAQmxWarningPALComponentNotUnloadable                                             (50262)
#define DAQmxWarningPALMemoryAlignmentFault                                               (50351)
#define DAQmxWarningPALMemoryHeapNotEmpty                                                 (50355)
#define DAQmxWarningPALTransferNotInProgress                                              (50402)
#define DAQmxWarningPALTransferInProgress                                                 (50403)
#define DAQmxWarningPALTransferStopped                                                    (50404)
#define DAQmxWarningPALTransferAborted                                                    (50405)
#define DAQmxWarningPALLogicalBufferEmpty                                                 (50406)
#define DAQmxWarningPALLogicalBufferFull                                                  (50407)
#define DAQmxWarningPALPhysicalBufferEmpty                                                (50408)
#define DAQmxWarningPALPhysicalBufferFull                                                 (50409)
#define DAQmxWarningPALTransferOverwritten                                                (50410)
#define DAQmxWarningPALTransferOverread                                                   (50411)
#define DAQmxWarningPALDispatcherAlreadyExported                                          (50500)
#define DAQmxWarningPALSyncAbandoned                                                      (50551)
#define DAQmxWarningValueNotInSet                                                         (54022)
