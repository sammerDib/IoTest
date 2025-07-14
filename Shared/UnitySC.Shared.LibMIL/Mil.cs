using System;
using System.Collections.Generic;

using Matrox.MatroxImagingLibrary;

namespace UnitySC.Shared.LibMIL
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    public class Mil
    {
        public MilApplication Application;
        private Dictionary<string, MilSystem> Systems = new Dictionary<string, MilSystem>();
        public MilSystem HostSystem;
        private bool _ownsApplication = false;

        //=================================================================
        // Instance
        //=================================================================
        public static Mil Instance { get; } = new Mil();

        ///=================================================================
        /// <summary>
        /// Initialize MIL à partir des IDs d'application/system déjà alloués.
        /// </summary>
        ///=================================================================
        public void InitFromIDs(MIL_ID applicationId, MIL_ID systemId)
        {
            if (Application != null)
                throw new ApplicationException("MIL is already initialized");

            Console.WriteLine("Initialize MIL from external Application/System ID...");
            Application = new MilApplication();
            Application.Attach(applicationId);
            HostSystem = new MilSystem();
            HostSystem.Attach(systemId);
            _ownsApplication = false;
        }

        //=================================================================
        // Allocate
        //=================================================================
        public void Allocate()
        {
            if (Application != null)
                throw new ApplicationException("MIL is already initialized");
            _ownsApplication = true;

            Application = new MilApplication();
            Application.Allocate(MIL.M_DEFAULT);
            Application.Control(MIL.M_ERROR, MIL.M_THROW_EXCEPTION);

            Console.WriteLine("Allocating MIL system ...");

            HostSystem = new MilSystem();
            HostSystem.Allocate(MIL.M_SYSTEM_HOST);
            AMilId.checkMilError("Could not allocate MIL system");

            Systems.Add(MIL.M_SYSTEM_HOST, HostSystem);
        }

        //=================================================================
        //
        //=================================================================
        public MilSystem GetSystemInstance(string descriptor)
        {
            lock (this)
            {
                MilSystem milSystem;
                bool exists = Systems.TryGetValue(descriptor, out milSystem);
                if (!exists)
                {
                    milSystem = new MilSystem();
                    milSystem.Allocate(descriptor, MIL.M_DEV0);
                    Systems.Add(descriptor, milSystem);
                }

                return milSystem;
            }
        }

        //=================================================================
        // Free
        // On ne peut appeler Free() que depuis le thread qui a appelé
        // Allocate(). Donc on ne peut pas libérer automatiquement dans le
        // destructeur :-(
        //=================================================================
        public void Free()
        {
            if (!_ownsApplication)
                return;

            foreach (MilSystem sys in Systems.Values)
                sys.Dispose();
            Systems = null;

            if (Application != null)
                Application.Dispose();
            Application = null;
        }

        //=================================================================
        //
        //=================================================================
        public MIL_INT LicenseModules { get { return MIL.MappInquire(MIL.M_DEFAULT, MIL.M_LICENSE_MODULES); } }
    }
}