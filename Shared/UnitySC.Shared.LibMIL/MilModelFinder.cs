using System;
using System.ComponentModel;
using System.Drawing;

using Matrox.MatroxImagingLibrary;

//using System.Windows.Forms;

namespace UnitySC.Shared.LibMIL
{
    ///////////////////////////////////////////////////////////////////////
    //
    // A class representing a model of the MIL model finder module
    //
    ///////////////////////////////////////////////////////////////////////
    public class MilModelFinder : AMilModel
    {
        //=================================================================
        // Constants
        //=================================================================
        public const int DefaultTimeOut = (int)MIL.M_DISABLE;

        public const int MaximumActionInInteractiveForm = 40000;

        private int _allocOffsetX;
        private int _allocOffsetY;

        //=================================================================
        // Enum
        //=================================================================
        public enum EPolarity : int
        {
            Default = (int)MIL.M_DEFAULT,
            Any = (int)MIL.M_ANY,
            Reverse = (int)MIL.M_REVERSE,
            Same = (int)MIL.M_SAME,

            [Description("Same or reverse")]
            SameOrReverse = (int)MIL.M_SAME_OR_REVERSE
        }

        public enum ESpeed : int
        {
            Default = (int)MIL.M_DEFAULT,
            Low = (int)MIL.M_LOW,
            Medium = (int)MIL.M_MEDIUM,
            High = (int)MIL.M_HIGH,

            [Description("Very high")]
            VeryHigh = (int)MIL.M_VERY_HIGH
        }

        public enum EAccuracy : int
        {
            Default = (int)MIL.M_DEFAULT,
            Medium = (int)MIL.M_MEDIUM,
            High = (int)MIL.M_HIGH,
        }

        [Flags()]
        public enum ETimeOut : int
        {
            disable = (int)MIL.M_DISABLE
        }

        //=================================================================
        // Properties
        //=================================================================
        public int ContextType
        {
            get { return (int)MIL.MmodInquire(_milId, MIL.M_CONTEXT, MIL.M_CONTEXT_TYPE); }
        }

        //-----------------------------------------------------------------
        // Model's reference position relative to the top-left corner of the model's source image
        //-----------------------------------------------------------------
        public override int OriginalX
        {
            get { return ReferenceX + AllocOffsetX; }
        }

        public override int OriginalY
        {
            get { return ReferenceY + AllocOffsetY; }
        }

        //-----------------------------------------------------------------
        // Model's search region
        //-----------------------------------------------------------------
        public int PositionX
        {
            get { return (int)MIL.MmodInquire(_milId, 0, MIL.M_POSITION_X); }
            set
            {
                MIL.MmodControl(MilId, 0, MIL.M_POSITION_X, value);
            }
        }

        public int PositionDeltaNegX
        {
            get { return (int)MIL.MmodInquire(_milId, 0, MIL.M_POSITION_DELTA_NEG_X); }
            set
            {
                MIL.MmodControl(MilId, 0, MIL.M_POSITION_DELTA_NEG_X, value);
            }
        }

        public int PositionDeltaPosX
        {
            get { return (int)MIL.MmodInquire(_milId, 0, MIL.M_POSITION_DELTA_POS_X); }
            set
            {
                MIL.MmodControl(MilId, 0, MIL.M_POSITION_DELTA_POS_X, value);
            }
        }

        public int PositionY
        {
            get { return (int)MIL.MmodInquire(_milId, 0, MIL.M_POSITION_Y); }
            set
            {
                MIL.MmodControl(MilId, 0, MIL.M_POSITION_Y, value);
            }
        }

        public int PositionDeltaNegY
        {
            get { return (int)MIL.MmodInquire(_milId, 0, MIL.M_POSITION_DELTA_NEG_Y); }
            set
            {
                MIL.MmodControl(MilId, 0, MIL.M_POSITION_DELTA_NEG_Y, value);
            }
        }

        public int PositionDeltaPosY
        {
            get { return (int)MIL.MmodInquire(_milId, 0, MIL.M_POSITION_DELTA_POS_Y); }
            set
            {
                MIL.MmodControl(MilId, 0, MIL.M_POSITION_DELTA_POS_Y, value);
            }
        }

        public bool EnablePosition
        {
            get
            {
                MIL_INT enable = MIL.MmodInquire(_milId, MIL.M_CONTEXT, MIL.M_SEARCH_POSITION_RANGE);
                return enable == MIL.M_ENABLE;
            }
            set
            {
                int enable = value ? MIL.M_ENABLE : MIL.M_DISABLE;
                MIL.MmodControl(MilId, MIL.M_CONTEXT, MIL.M_SEARCH_POSITION_RANGE, enable);
                checkPropertyMilError("Search position range", "Position");
            }
        }

        public override Rectangle Position
        {
            get
            {
                return new Rectangle(
                    PositionX - PositionDeltaNegX,
                    PositionY - PositionDeltaNegY,
                    PositionDeltaNegX + PositionDeltaPosX,
                    PositionDeltaNegY + PositionDeltaPosY
                    );
            }
            set
            {
                PositionX = value.X;
                PositionY = value.Y;
                PositionDeltaNegX = 0;
                PositionDeltaNegY = 0;
                PositionDeltaPosX = value.Width;
                PositionDeltaPosY = value.Height;
            }
        }

        //-----------------------------------------------------------------
        // Reference point
        //-----------------------------------------------------------------
        public override int ReferenceX
        {
            get
            {
                int val = (int)MIL.MmodInquire(MilId, 0, MIL.M_REFERENCE_X);
                if (val == MIL.M_DEFAULT)
                    val = (int)MIL.MmodInquire(MilId, 0, MIL.M_REFERENCE_X + MIL.M_DEFAULT);
                return val;
            }
            set
            {
                MIL.MmodControl(MilId, 0, MIL.M_REFERENCE_X, value);
                checkPropertyMilError("X", "Reference point");
            }
        }

        public override int ReferenceY
        {
            get
            {
                int val = (int)MIL.MmodInquire(MilId, 0, MIL.M_REFERENCE_Y);
                if (val == MIL.M_DEFAULT)
                    val = (int)MIL.MmodInquire(MilId, 0, MIL.M_REFERENCE_Y + MIL.M_DEFAULT);
                return val;
            }
            set
            {
                MIL.MmodControl(MilId, 0, MIL.M_REFERENCE_Y, value);
                checkPropertyMilError("Y", "Reference point");
            }
        }

        public double ReferenceAngle
        {
            get { return (int)MIL.MmodInquire(MilId, 0, MIL.M_REFERENCE_ANGLE); }
            set
            {
                MIL.MmodControl(MilId, 0, MIL.M_REFERENCE_ANGLE, value);
                checkPropertyMilError("Angle", "Reference point");
            }
        }

        //-----------------------------------------------------------------
        // Rotation
        //-----------------------------------------------------------------
        public double Angle
        {
            get
            {
                double res = 0.0d;
                MIL.MmodInquire(MilId, 0, MIL.M_ANGLE, ref res);
                return res;
            }
            set
            {
                MIL.MmodControl(MilId, 0, MIL.M_ANGLE, value);
                checkPropertyMilError("Angle", "Rotation");
            }
        }

        public double AngleDeltaNeg
        {
            get
            {
                double res = 0.0d;
                MIL.MmodInquire(MilId, 0, MIL.M_ANGLE_DELTA_NEG, ref res);
                return res;
            }
            set
            {
                MIL.MmodControl(MilId, 0, MIL.M_ANGLE_DELTA_NEG, value);
                checkPropertyMilError("Angle delta negative", "Rotation");
            }
        }

        public double AngleDeltaPos
        {
            get
            {
                double res = 0.0d;
                MIL.MmodInquire(MilId, 0, MIL.M_ANGLE_DELTA_POS, ref res);
                return res;
            }
            set
            {
                MIL.MmodControl(MilId, 0, MIL.M_ANGLE_DELTA_POS, value);
                checkPropertyMilError("Angle delta positive", "Rotation");
            }
        }

        public bool EnableRotation
        {
            get
            {
                MIL_INT enable = MIL.MmodInquire(MilId, MIL.M_CONTEXT, MIL.M_SEARCH_ANGLE_RANGE);
                return enable == MIL.M_ENABLE;
            }
            set
            {
                int enable = value ? MIL.M_ENABLE : MIL.M_DISABLE;
                MIL.MmodControl(MilId, MIL.M_CONTEXT, MIL.M_SEARCH_ANGLE_RANGE, enable);
                checkPropertyMilError("Search angle range", "Rotation");
            }
        }

        //-----------------------------------------------------------------
        // Scaling
        //-----------------------------------------------------------------
        public double Scale
        {
            get
            {
                double res = 0.0d;
                MIL.MmodInquire(MilId, 0, MIL.M_SCALE, ref res);
                return res;
            }
            set
            {
                MIL.MmodControl(MilId, 0, MIL.M_SCALE, value);
                checkPropertyMilError("Scale", "Scaling");
            }
        }

        public double ScaleMaxFactor
        {
            get
            {
                double res = 0.0d;
                MIL.MmodInquire(MilId, 0, MIL.M_SCALE_MAX_FACTOR, ref res);
                return res;
            }
            set
            {
                MIL.MmodControl(MilId, 0, MIL.M_SCALE_MAX_FACTOR, value);
                checkPropertyMilError("Maximum scale factor", "Scaling");
            }
        }

        public double ScaleMinFactor
        {
            get
            {
                double res = 0.0d;
                MIL.MmodInquire(MilId, 0, MIL.M_SCALE_MIN_FACTOR, ref res);
                return res;
            }
            set
            {
                MIL.MmodControl(MilId, 0, MIL.M_SCALE_MIN_FACTOR, value);
                checkPropertyMilError("Minimum scale factor", "Scaling");
            }
        }

        //-----------------------------------------------------------------
        // Advanced
        //-----------------------------------------------------------------
        public override double Acceptance
        {
            get { return (int)MIL.MmodInquire(MilId, 0, MIL.M_ACCEPTANCE); }
            set
            {
                MIL.MmodControl(MilId, 0, MIL.M_ACCEPTANCE, value);
                checkPropertyMilError("Acceptance", "Advanced");
            }
        }

        public override double Certainty
        {
            get { return (int)MIL.MmodInquire(MilId, 0, MIL.M_CERTAINTY); }
            set
            {
                MIL.MmodControl(MilId, 0, MIL.M_CERTAINTY, value);
                checkPropertyMilError("Certainty", "Advanced");
            }
        }

        public override int Number
        {
            get
            {
                int val = (int)MIL.MmodInquire(MilId, 0, MIL.M_NUMBER);
                if (val == MIL.M_DEFAULT)
                    val = (int)MIL.MmodInquire(MilId, 0, MIL.M_NUMBER + MIL.M_DEFAULT);
                return val;
            }
            set
            {
                MIL.MmodControl(MilId, 0, MIL.M_NUMBER, value);
                checkPropertyMilError("Number", "Advanced");
            }
        }

        public double Smoothness
        {
            get { return (int)MIL.MmodInquire(MilId, MIL.M_CONTEXT, MIL.M_SMOOTHNESS); }
            set
            {
                MIL.MmodControl(MilId, MIL.M_CONTEXT, MIL.M_SMOOTHNESS, value);
                checkPropertyMilError("Smoothness", "Advanced");
            }
        }

        public EPolarity Polarity
        {
            get { return (EPolarity)(int)MIL.MmodInquire(MilId, 0, MIL.M_POLARITY); }
            set
            {
                MIL.MmodControl(MilId, 0, MIL.M_POLARITY, (int)value);
                checkPropertyMilError("Polarity", "Advanced");
            }
        }

        public ETimeOut TimeOut
        {
            get { return (ETimeOut)(int)MIL.MmodInquire(MilId, MIL.M_CONTEXT, MIL.M_TIMEOUT); }
            set
            {
                MIL.MmodControl(MilId, MIL.M_CONTEXT, MIL.M_TIMEOUT, (int)value);
                checkPropertyMilError("Time out", "Advanced");
            }
        }

        //-----------------------------------------------------------------
        // Search context
        //-----------------------------------------------------------------
        public ESpeed Speed
        {
            get { return (ESpeed)(int)MIL.MmodInquire(MilId, MIL.M_CONTEXT, MIL.M_SPEED); }
            set
            {
                MIL.MmodControl(MilId, MIL.M_CONTEXT, MIL.M_SPEED, (int)value);
                checkPropertyMilError("Speed", "Context");
            }
        }

        public EAccuracy Accuracy
        {
            get { return (EAccuracy)(int)MIL.MmodInquire(MilId, MIL.M_CONTEXT, MIL.M_ACCURACY); }
            set
            {
                MIL.MmodControl(MilId, MIL.M_CONTEXT, MIL.M_ACCURACY, (int)value);
                checkPropertyMilError("Accuracy", "Context");
            }
        }

        //-----------------------------------------------------------------
        // Position in Model's source image
        //-----------------------------------------------------------------
        public override int AllocOffsetX
        {
            get { return _allocOffsetX; }
            set
            {
                _allocOffsetX = value;
                if (value >= 0)
                {
                    MIL.MmodControl(_milId, 0, MIL.M_ALLOC_OFFSET_X, value);
                    checkPropertyMilError("AllocOffsetX", "Model size");
                }
            }
        }

        public override int AllocOffsetY
        {
            get { return _allocOffsetY; }
            set
            {
                _allocOffsetY = value;
                if (value >= 0)
                {
                    MIL.MmodControl(_milId, 0, MIL.M_ALLOC_OFFSET_Y, value);
                    checkPropertyMilError("AllocOffsetY", "Model size");
                }
            }
        }

        public override int AllocSizeX
        {
            get { return (int)MIL.MmodInquire(_milId, 0, MIL.M_ALLOC_SIZE_X); }
        }

        public override int AllocSizeY
        {
            get { return (int)MIL.MmodInquire(_milId, 0, MIL.M_ALLOC_SIZE_Y); }
        }

        public override Rectangle AllocRectangle
        {
            get { return new Rectangle(AllocOffsetX, AllocOffsetY, AllocSizeX, AllocSizeY); }
        }

        //-----------------------------------------------------------------
        // Other
        //-----------------------------------------------------------------
        public override bool IsPreprocessed
        {
            get { return MIL.MmodInquire(_milId, MIL.M_CONTEXT, MIL.M_PREPROCESSED) == 1; }
        }

        public override MIL_ID OwnerSystem
        {
            get { return (MIL_ID)MIL.MmodInquire(_milId, MIL.M_CONTEXT, MIL.M_OWNER_SYSTEM); }
        }

        public MIL_ID NumberModels
        {
            get { return (MIL_ID)MIL.MmodInquire(_milId, MIL.M_CONTEXT, MIL.M_NUMBER_MODELS); }
        }

        //=================================================================
        // Constructor
        //=================================================================
        public MilModelFinder()
        {
            Usage = EUsage.WholeDie;
        }

        //=================================================================
        // Functions
        //=================================================================

        //-----------------------------------------------------------------
        // Set default value at all properties
        // Note that you can pass "+M_DEFAULT" to MmodInquire() to get the default value.
        // But then you always get the default value
        //-----------------------------------------------------------------
        public void ResetProperties()
        {
            ReferenceAngle = 0.0d;
            Angle = 0.0d;
            AngleDeltaNeg = 10.0;
            AngleDeltaPos = 10.0;
            EnableRotation = true;
            Scale = 1.0d;
            ScaleMaxFactor = 1.1;
            ScaleMinFactor = 0.9;
            Acceptance = 70;
            Certainty = 90;
            Number = 1;
            Smoothness = 50.0;
            TimeOut = (ETimeOut)MilModelFinder.DefaultTimeOut;
            Polarity = MilModelFinder.EPolarity.Same;
            Speed = MilModelFinder.ESpeed.VeryHigh;
            Accuracy = MilModelFinder.EAccuracy.Medium;
            PositionX = MIL.M_ALL;
            PositionDeltaNegX = 0;
            PositionDeltaPosX = MIL.M_INFINITE;
            PositionY = MIL.M_ALL;
            PositionDeltaNegY = 0;
            PositionDeltaPosY = MIL.M_INFINITE;
            EnablePosition = true;
        }

        //-----------------------------------------------------------------
        // Dispose
        //-----------------------------------------------------------------
        protected override void Dispose(bool disposing)
        {
            if (_milId != MIL.M_NULL)
            {
                MIL.MmodFree(_milId);
                _milId = MIL.M_NULL;
            }

            base.Dispose(disposing);
        }

        //-----------------------------------------------------------------
        // Load a model from disk
        //-----------------------------------------------------------------
        public override void Restore(MIL_ID systemId, string filename)
        {
            if (_milId != MIL.M_NULL)
                throw new ApplicationException("reusing model");

            Name = System.IO.Path.GetFileNameWithoutExtension(filename);
            MIL.MmodRestore(filename, systemId, MIL.M_DEFAULT, ref _milId);
            AMilId.checkMilError("Failed to load model \"" + filename + "\"");

            _allocOffsetX = (int)MIL.MmodInquire(_milId, 0, MIL.M_ALLOC_OFFSET_X);
            _allocOffsetY = (int)MIL.MmodInquire(_milId, 0, MIL.M_ALLOC_OFFSET_Y);
        }

        public long Stream(byte[] memPtr, MIL_ID systemId, long operation, long streamType, double version = MIL.M_DEFAULT, long controlFlag = MIL.M_DEFAULT)
        {
            if (_milId != MIL.M_NULL)
                throw new ApplicationException("reusing model");

            Name = "";

            MIL_INT size = 0;
            MIL.MmodStream(memPtr, systemId, operation, streamType, version, controlFlag, ref _milId, ref size);
            AMilId.checkMilError("Failed to stream model");

            if (ContextType == MIL.M_GEOMETRIC_CONTROLLED)
            {
                _allocOffsetX = (int)MIL.MmodInquire(_milId, 0, MIL.M_ALLOC_OFFSET_X);
                _allocOffsetY = (int)MIL.MmodInquire(_milId, 0, MIL.M_ALLOC_OFFSET_Y);
            }
            return size;
        }

        //-----------------------------------------------------------------
        // Save a model to disk
        //-----------------------------------------------------------------
        public override void Save(string filename)
        {
            MIL.MmodSave(filename, _milId, MIL.M_DEFAULT);
            AMilId.checkMilError("Failed to save model \"" + filename + "\"");
        }

        //-----------------------------------------------------------------
        // Preprocess the model
        //-----------------------------------------------------------------
        public override void Preprocess(MilImage milTypicalImage)
        {
            Preprocess();
        }

        public void Preprocess()
        {
            MIL.MmodPreprocess(_milId, MIL.M_DEFAULT);
            AMilId.checkMilError("Failed to preprocess model");
        }

        //-----------------------------------------------------------------
        // Draw the model position in an image
        //-----------------------------------------------------------------
        public override void Draw(MilImage milDestImage,
                                  int operation = MIL.M_DRAW_BOX + MIL.M_DRAW_POSITION + MIL.M_DRAW_EDGES,
                                  int index = MIL.M_DEFAULT,
                                  int controlFlag = MIL.M_DEFAULT)
        {
            MIL.MmodDraw(MIL.M_DEFAULT, _milId, milDestImage.MilId, operation, index, controlFlag);
            AMilId.checkMilError("Failed to draw model");
        }

        public override void Draw(MilGraphicsContext milGC,
                                  int operation = MIL.M_DRAW_BOX + MIL.M_DRAW_POSITION + MIL.M_DRAW_EDGES,
                                  int index = MIL.M_DEFAULT,
                                  int controlFlag = MIL.M_DEFAULT)
        {
            MIL.MmodDraw(milGC.MilId, _milId, milGC.Image.MilId, operation, index, controlFlag);
            AMilId.checkMilError("Failed to draw model");
        }

        //-----------------------------------------------------------------
        // Alloc Model
        //-----------------------------------------------------------------
        public void Alloc(MIL_ID systemId)
        {
            Alloc(systemId, MIL.M_GEOMETRIC_CONTROLLED, MIL.M_DEFAULT);
        }

        public void Alloc(MIL_ID systemId, long modelFinderType, long controlFlag)
        {
            if (_milId != MIL.M_NULL)
                throw new ApplicationException("reusing model");

            MIL.MmodAlloc(systemId, modelFinderType, controlFlag, ref _milId);
            AMilId.checkMilError("Failed to allocate model");
        }

        //-----------------------------------------------------------------
        // Define Model
        //-----------------------------------------------------------------
        public void Define(MilImage milImage, Rectangle rect)
        {
            MIL.MmodDefine(_milId, MIL.M_IMAGE, milImage.MilId, rect.X, rect.Y, rect.Width, rect.Height);
            AMilId.checkMilError("Failed to define model finder");
            _allocOffsetX = rect.X;
            _allocOffsetY = rect.Y;
            ResetProperties();
        }

        public void DefineCircle(double polarity, double radius)
        {
            MIL.MmodDefine(_milId, MIL.M_CIRCLE, polarity, radius, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT);
            AMilId.checkMilError("Failed to define model finder");
        }

        //-----------------------------------------------------------------
        // Create a result object
        //-----------------------------------------------------------------
        public override AMilResult NewResult()
        {
            return new MilModelFinderResult(this);
        }

        //-----------------------------------------------------------------
        // Show interactive form
        //In a form mil.M_INTERACTIVE, the number of returned action changes depending on the
        //number of action previously performed. It is therefore difficult to determine if the
        //user has not performed action or not. That's why the limit of action in a form has
        //been limited to MilModelFinder._MaximumActionInInteractiveForm.
        //if the user make MilModelFinder._MaximumActionInInteractiveForm actions in a
        //MIL.M_INTERACTIVE form, DialogResult.Cancel has been return
        //-----------------------------------------------------------------
        /*public DialogResult ShowInteractiveModelForm()
        {
            int ValueStart = (int)MIL.MmodInquire(_milId, MIL.M_CONTEXT, MIL.M_MODIFICATION_COUNT);

            MIL.MmodControl(_milId, MIL.M_CONTEXT, MIL.M_INTERACTIVE, MIL.M_DEFAULT);
            AMilId.checkMilError("Failed to set model properties with MIL.M_INTERACTIVE");

            int ValueEnd = (int)MIL.MmodInquire(_milId, MIL.M_CONTEXT, MIL.M_MODIFICATION_COUNT);

            if ((ValueEnd - ValueStart) < MilModelFinder._MaximumActionInInteractiveForm)
                return DialogResult.OK;
            else
                return DialogResult.Cancel;
        }
        public DialogResult ShowInteractiveMaskForm()
        {
            int ValueStart = (int)MIL.MmodInquire(_milId, MIL.M_CONTEXT, MIL.M_MODIFICATION_COUNT);

            MIL.MmodMask(_milId, 0, MIL.M_NULL, MIL.M_INTERACTIVE, MIL.M_DEFAULT);
            AMilId.checkMilError("Failed to set mask with MIL.M_INTERACTIVE");

            int ValueEnd = (int)MIL.MmodInquire(_milId, MIL.M_CONTEXT, MIL.M_MODIFICATION_COUNT);

            if ((ValueEnd - ValueStart) < MilModelFinder._MaximumActionInInteractiveForm)
                return DialogResult.OK;
            else
                return DialogResult.Cancel;
        }*/

        //-----------------------------------------------------------------
        // Mask
        //-----------------------------------------------------------------
        public void Mask(MIL_ID mask, int maskType)
        {
            MIL.MmodMask(MilId, 0, mask, maskType, MIL.M_DEFAULT);
            AMilId.checkMilError("Mask(MilImage mask, int maskType) : failed");
        }
    }
}