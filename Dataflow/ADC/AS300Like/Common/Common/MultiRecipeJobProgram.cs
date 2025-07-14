using CIM300EXPERTLib;

using Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common2017
{
    public class WaferProgram
    {
        public int Slot;
        public String FullRecipeFilePathName;
        public string CustomField;
        public bool MustCheckCustomFieldValidation;

        public WaferProgram(int slot, string recipeName, string customField, bool mustCheckCustomFieldValidation)
        {
            Slot = slot;
            FullRecipeFilePathName = recipeName;
            CustomField = customField;
            MustCheckCustomFieldValidation = mustCheckCustomFieldValidation;

        }

        public bool Validated
        {
            get
            {
                if ((Slot < 1) || (Slot > 25))
                    return false;
                if (String.IsNullOrEmpty(CustomField) && MustCheckCustomFieldValidation)
                    return false;
                return !String.IsNullOrEmpty(FullRecipeFilePathName);
            }
        }
    }
    public class IdenticalRecipeJobProgram : MultiRecipeJobProgram
    {
        String m_fullRecipeFilePathName;
        public IdenticalRecipeJobProgram(EFEMStationID portID, string carrierID, List<WaferProgram> wafersProgramList) 
            : base(portID, carrierID)
        {
            m_fullRecipeFilePathName = wafersProgramList[0].FullRecipeFilePathName;
            bool failed = wafersProgramList.Any(w => w.FullRecipeFilePathName != m_fullRecipeFilePathName);
            if (failed)
                throw new Exception("Elements of list do not have identical recipe names");
            WafersProgramList = wafersProgramList;
        }

        public string FullRecipeFilePathName { get => m_fullRecipeFilePathName;  }
    }

    public class MultiRecipeJobProgram
    {
        protected EFEMStationID m_PortID;
        protected String m_CarrierID;
        protected List<WaferProgram> m_wafersProgramList = new List<WaferProgram>();

        public MultiRecipeJobProgram(EFEMStationID portID, String carrierID)
        {
            PortID = portID;
            CarrierID = carrierID;
        }

        public EFEMStationID PortID { get => m_PortID; set => m_PortID = value; }
        public string CarrierID { get => m_CarrierID; set => m_CarrierID = value; }
        public List<WaferProgram> WafersProgramList { get => m_wafersProgramList; set => m_wafersProgramList = value; }

        public void AddWafer(WaferProgram wafer)
        {
            WafersProgramList.Add(wafer);

        }
        public void Clear()
        {
            WafersProgramList.Clear();
        }        

        public List<IdenticalRecipeJobProgram> GetIRJPrograms()
        {
            try
            {
                List<IdenticalRecipeJobProgram> result = new List<IdenticalRecipeJobProgram>();
                List<String> AlreadyFoundRecipeNameList = new List<string>();
                WafersProgramList.Sort((w1, w2) => w1.Slot.CompareTo(w2.Slot));
                foreach (var wfp in WafersProgramList)
                {
                    List<WaferProgram> newWaferPrgromList = WafersProgramList.FindAll(w => (w.FullRecipeFilePathName == wfp.FullRecipeFilePathName) && (!AlreadyFoundRecipeNameList.Any(i => i == w.FullRecipeFilePathName)));
                    if (newWaferPrgromList.Count > 0)
                    {
                        AlreadyFoundRecipeNameList.Add(wfp.FullRecipeFilePathName);
                        IdenticalRecipeJobProgram newIRJPrg = new IdenticalRecipeJobProgram(PortID, CarrierID, newWaferPrgromList);
                        result.Add(newIRJPrg);
                    }
                }
                return result;
            }
            catch  (Exception ex)
            {
                return null;
            }

        }

        public int SlotCount
        {
            get => WafersProgramList.Count;
        }

        public bool CheckValidation(out string msgErr)
        {
            if (PortID == EFEMStationID.stidUnknown)
            {
                msgErr = "PortID invalid";
                return false;
            }
            if (String.IsNullOrEmpty(CarrierID))
            {
                msgErr = "CarrierID is invalid";
                return false;
            }
            foreach (WaferProgram wp in WafersProgramList)
            {
                if (!wp.Validated)
                {
                    msgErr = $"Wafer slot #{wp.Slot} is invalid";
                    return false;
                }
            }
            msgErr = "";
            return true;
        }
    }
}
