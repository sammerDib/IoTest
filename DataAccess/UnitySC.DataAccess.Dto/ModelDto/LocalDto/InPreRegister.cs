using System;
using System.Runtime.Serialization;

using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.DataAccess.Dto.ModelDto.LocalDto
{
    [DataContract]
    public class InPreRegister
    {
        /// <summary>
        /// for Serialization only
        /// </summary>
        public InPreRegister()
        {

        }

        /// <summary>
        /// default constructor (advanced user)
        /// </summary>
        public InPreRegister(ResultType restyp)
        {
            ResultType = restyp;
        }

        /// <summary>
        /// for registering another item of an already register or known result (same job, chamber and slot)
        /// </summary>
        public InPreRegister(ResultType restyp, long parentResultId, byte idx = 0, ResultFilterTag tag = ResultFilterTag.None)
        {
            ResultType = restyp;

            ParentResultId = parentResultId;
            System.Diagnostics.Debug.Assert(ParentResultId != -1);

            Idx = idx;
            FilterTag = tag;
        }

        /// <summary>
        /// for registering result without knowning jobid
        /// </summary>
        public InPreRegister(ResultType restyp, string jobName, string lotName, string tCRecipeName, int toolId, int chamberId, int productId, int recipeId, string waferBaseName, int slotId, byte idx = 0, ResultFilterTag tag = ResultFilterTag.None)
        {
            ResultType = restyp;

            JobName = jobName;
            LotName = lotName;
            TCRecipeName = tCRecipeName;
            ToolId = toolId;

            ChamberId = chamberId;
            ProductId = productId;
            RecipeId = recipeId;

            WaferBaseName = waferBaseName;
            SlotId = slotId;
            System.Diagnostics.Debug.Assert(0 < slotId && slotId <= 25);

            Idx = idx;
            FilterTag = tag;

        }

        /// <summary>
        /// for registering result without known jobid from database
        /// </summary>
        public InPreRegister(ResultType restyp, int jobid, int chamberId, int productId, int recipeId, string waferBaseName, int slotId, byte idx = 0, ResultFilterTag tag = ResultFilterTag.None)
        {
            ResultType = restyp;

            JobId = jobid;

            ChamberId = chamberId;
            ProductId = productId;
            RecipeId = recipeId;

            WaferBaseName = waferBaseName;
            SlotId = slotId;
            System.Diagnostics.Debug.Assert(0 < slotId && slotId <= 25);

            Idx = idx;
            FilterTag = tag;
        }


        [DataMember]
        public string JobName { get; set; } = string.Empty; // can be skipped only if JobId or ParentResultId are known
        [DataMember]
        public string LotName { get; set; } = string.Empty; // can be skipped if JobId or ParentResultId are known
        [DataMember]
        public string TCRecipeName { get; set; } = string.Empty; // can be skipped if JobId or ParentResultId are known -- same as Dataflow Recipe Nmae
        [DataMember]
        public int ToolId { get; set; } = -1;  // can be skipped if JobId or ParentResultId are known -- Id is the primary key in database in Tool Table 
        [DataMember]
        public int ToolKey { get; set; } = -1;  // can be skipped if JobId or ParentResultId are known or if toolId is Known -- key is the id key of the tool in the configuration (different for ChamberId)

        [DataMember]
        public int JobId { get; set; } = -1;  // can be skipped Only if ParentResultId is already known -- Id is a primary key in database in Job Table
        [DataMember]
        public int ChamberId { get; set; } = -1; // can be skipped Only if ParentResultId is already known or if we use ChamberKey/ToolKey instead) -- Id is a primary key in database in Chamber Table
        [DataMember]
        public int ChamberKey { get; set; } = -1; // can be skipped Only if ParentResultId is already known or if we use directly chamberID --- key is the id key of the chamber in the configuration (different for ChamberId)

        [DataMember]
        public int ProductId { get; set; } = -1; // can be skipped Only if ParentResultId is already known -- Id is a primary key in database in Product Table
        [DataMember]
        public int RecipeId { get; set; } = -1;  // can be skipped Only if ParentResultId is already known -- Id is a primary key in database in Recipe Table
        [DataMember]
        public string WaferBaseName { get; set; } = string.Empty; // can be skipped Only if ParentResultId is already known
        [DataMember]
        public int SlotId { get; set; } = -1; // can be skipped Only if ParentResultId is already known -- generaly an int [1..25]

        [DataMember]
        public long ParentResultId { get; set; } = -1; // if already known (use for registering result with same job, recipe, slot)

        [DataMember]
        public byte Idx { get; set; } = 0;  //index only use for several iteration of the same resultType within 1 recipe. start at 0. index should be increment regarding the same ResultType used within the same recipe

        #region MANDATORY 
        [DataMember]
        public ResultType ResultType { get; set; } = 0; // Mandatory 
        #endregion //MANDATORY 

        #region OPTIONAL  //automatically generated 
        [DataMember]
        public DateTime DateTimeRun { get; set; } = DateTime.Now; // date time use and store in Database tables (Wafer, Result, Item...)

        [DataMember]
        public string LabelName { get; set; } = string.Empty; // if left empty generate LabelName from ResultType And idx -- can be specified if a custommer want its own Label for its measure/result

        [DataMember]
        virtual public string FileName { get; set; } = string.Empty; // FileName is always generated for Result but Mandatory for InPreRegisterAcquisition Acqusition

        [DataMember]
        public ResultFilterTag FilterTag { get; set; } = ResultFilterTag.None; // for future devellopment (at this time allow to filter result in Viewer)

        #endregion //OPTIONAL 
    }

    [DataContract]
    public class InPreRegisterAcquisition : InPreRegister
    {

        /// <summary>
        /// for Serialization only
        /// </summary>
        public InPreRegisterAcquisition() : base() { }

        /// <summary>
        /// default constructor (advanced user)
        /// </summary>
        public InPreRegisterAcquisition(ResultType restyp, string filename, string pathname) : base(restyp)
        {
            System.Diagnostics.Debug.Assert(restyp.GetResultCategory() == ResultCategory.Acquisition);
            FileName = filename;
            PathName = pathname;
        }

        /// <summary>
        /// for registering another item of an already register or known result (same job, chamber and slot)
        /// </summary>
        public InPreRegisterAcquisition(ResultType restyp, string filename, string pathname, long parentResultId, byte idx = 0, ResultFilterTag tag = ResultFilterTag.None) : base(restyp, parentResultId, idx, tag)
        {
            System.Diagnostics.Debug.Assert(restyp.GetResultCategory() == ResultCategory.Acquisition);
            FileName = filename;
            PathName = pathname;
        }

        /// <summary>
        /// for registering result without knowning jobid
        /// </summary>
        public InPreRegisterAcquisition(ResultType restyp, string filename, string pathname, string jobName, string lotName, string tCRecipeName, int toolKey, int chamberKey, int productId, int recipeId, string waferBaseName, int slotId, byte idx = 0, ResultFilterTag tag = ResultFilterTag.None)
           : base(restyp, jobName, lotName, tCRecipeName, toolKey, chamberKey, productId, recipeId, waferBaseName, slotId, idx, tag)
        {
            System.Diagnostics.Debug.Assert(restyp.GetResultCategory() == ResultCategory.Acquisition);
            FileName = filename;
            PathName = pathname;
        }

        /// <summary>
        /// for registering result with jobid from database
        /// </summary>
        public InPreRegisterAcquisition(ResultType restyp, string filename, string pathname, int jobid, int chamberKey, int productId, int recipeId, string waferBaseName, int slotId, byte idx = 0)
           : base(restyp, jobid, chamberKey, productId, recipeId, waferBaseName, slotId, idx)
        {
            System.Diagnostics.Debug.Assert(restyp.GetResultCategory() == ResultCategory.Acquisition);
            FileName = filename;
            PathName = pathname;
        }

        #region MANDATORY 
        [DataMember]
        public override string FileName { get; set; } = string.Empty; //Mandatory for InPreRegisterAcquisition Acqusition

        [DataMember]
        public string PathName { get; set; } = string.Empty;
        #endregion //MANDATORY 
    }

    static public class InPreRegisterAcqHelper
    {
        static public string FileNameToThumbName(string acqfilename, string thumbextension = null)
        { return $"{System.IO.Path.GetFileNameWithoutExtension(acqfilename)}_thumb.{thumbextension ?? "png"}"; }

        static public string FullAcqFilePath(this InPreRegisterAcquisition preAcq)
        { return System.IO.Path.Combine(preAcq.PathName, preAcq.FileName); }

        static public string FullAcqFilePath(string pathName, string fileName)
        { return System.IO.Path.Combine(pathName, fileName); }

        static public string FileNameThumbnail(this InPreRegisterAcquisition preAcq, string thumbnailExtension = null)
        { return FileNameToThumbName(preAcq.FileName, thumbnailExtension); }

        static public string FullAcqFilePathThumbnail(this InPreRegisterAcquisition preAcq, string thumbnailExtension = null)
        { return System.IO.Path.Combine(preAcq.PathName, System.IO.Path.GetDirectoryName(preAcq.FileName), preAcq.FileNameThumbnail(thumbnailExtension)); }

        static public string FullAcqFilePathThumbnail(string pathName, string fileName, string thumbnailExtension = null)
        { return System.IO.Path.Combine(pathName, System.IO.Path.GetDirectoryName(fileName), FileNameToThumbName(fileName, thumbnailExtension)); }

    }



}
