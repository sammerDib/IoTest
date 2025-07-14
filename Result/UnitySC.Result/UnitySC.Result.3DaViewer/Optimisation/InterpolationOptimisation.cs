namespace UnitySC.Result._3DaViewer.Optimisation
{
    public static class InterpolationOptimisation
    {
        #region Performances Helpers

        /// <summary>
        /// Get the physical memory capacity of the computer running the program.
        /// </summary>
        /// <returns> The physical memory capacity in GB. </returns>
        //private long GetPhysicalMemoryCapacity()
        //{
        //    ComputerInfo CI = new ComputerInfo();
        //    return (long)CI.TotalPhysicalMemory;
        //}

        /// <summary>
        /// Find out if the computer has a dedicated graphic card (AMD or Nvidia).
        /// </summary>
        /// <returns> True if the computer has a dedicated graphic card, false otherwise (Intel(R) HD graphics or Intel(R) Iris chipset). </returns>
        //private bool HasADedicatedGraphicCard()
        //{
        //    bool bDedicatedGraphicCard = false;

        //    foreach (var item in new ManagementObjectSearcher("SELECT Description  FROM Win32_VideoController").Get())
        //    {
        //        if (item.Properties["Description"].Value.ToString().Contains("NVIDIA") || item.Properties["Description"].Value.ToString().Contains("AMD"))
        //        {
        //            bDedicatedGraphicCard = true;
        //        }
        //    }
        //    return bDedicatedGraphicCard;
        //}

        /// <summary>
        /// Get the number of cores of the processor.
        /// </summary>
        /// <returns> The number of cores. </returns>
        //private int GetProcessorNumberOfCores()
        //{
        //    int iNumberOfCores = 0;

        //    foreach (var item in new ManagementObjectSearcher("SELECT NumberOfCores FROM Win32_Processor").Get())
        //    {
        //        iNumberOfCores = Convert.ToInt32(item["NumberOfCores"]);
        //    }

        //    return iNumberOfCores;
        //}

        /// <summary>
        /// Get the number of logical processors.
        /// </summary>
        /// <returns> The number of logical processors. </returns>
        //private int GetProcessorNumberOfLogicalProcessors()
        //{
        //    return Environment.ProcessorCount;
        //}

        #endregion


        //public static float[,] Optimize(float[] matrix, int matrixSizeX)
        //{
        //    // TODO CONTEXT

        //    var m_rcChildWindow = new Rectangle(0, 0, 1000, 1000);
        //    int DefCameraZoom = 0;


        //    // Depend on the computer performance, maximum surface size = (m_iMaxSurfaceSize - 1) x (m_iMaxSurfaceSize - 1).
        //    int m_iMaxSurfaceSize = 750;

        //    int iResolution = 0;

        //    int iDataSizeX = m_rcChildWindow.Width;
        //    int iDataSizeY = m_rcChildWindow.Height;

        //    if (Math.Max(iDataSizeX, iDataSizeY) <= m_iMaxSurfaceSize / 2)
        //    {
        //        iResolution = 0;
        //    }
        //    else if (iDataSizeX != m_iMaxSurfaceSize / 2 && iDataSizeY != m_iMaxSurfaceSize / 2)
        //    {
        //        // Calculate the reduction factor.
        //        // iResolution = 0 si Max(iDataSizeX, iDataSizeY) < MAXSIZE, otherwise equals the division quotient.
        //        iResolution = (int)(Math.Log(Math.Max(iDataSizeX, iDataSizeY) / (m_iMaxSurfaceSize / 2)) / Math.Log(2));
        //    }


        //    if (iResolution == 0)
        //    {
        //        // Compute default camera zoom.
        //        //DefCameraZoom = (int)(Math.Sqrt(p_afData.Count()));
        //        DefCameraZoom = (int)Math.Sqrt(m_rcChildWindow.Width * m_rcChildWindow.Height);

        //        // Create an array of SurfacePoint from the p_afData, not too many points.
        //        // m_sgs3dSurfaceGridWithoutNaN.Data = CreateSurfaceFromData(p_afData, p_iDataSizeX, p_iDataSizeY, p_fMinThreshlod);

        //        // TODO ???
        //        return new float[0,0];
        //    }
        //    else if (iResolution > 0)
        //    {
        //        float[,] afArrayOptimised = null;

        //        iDataSizeX = (int)(iDataSizeX / Math.Pow(2, iResolution));
        //        iDataSizeY = (int)(iDataSizeY / Math.Pow(2, iResolution));

        //        // Compute default camera zoom.
        //        DefCameraZoom = (int)Math.Sqrt(iDataSizeX * iDataSizeY);
        //        // Set the factor used to scale displayed mouse coordinates.
        //        var m_iMeshOptimisationFactor = (int)Math.Pow(2, iResolution);

        //        // Optimise surface, too many points.
        //        afArrayOptimised = BilinearInterpolation(matrix, matrixSizeX, iResolution, m_rcChildWindow);

        //        //m_sgs3dSurfaceGridWithoutNaN.Data = CreateSurfaceFromDataWithOptimisation(afArrayOptimised, p_fMinThreshlod);

        //        return afArrayOptimised;
        //    }

        //    return new float[0,0];
        //}
    }
}
