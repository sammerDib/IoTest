using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Data.ColorMap;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.ImageViewer
{
    public abstract class BaseMatrixViewerVM : ObservableObject, IDisposable
    {
        #region Properties

        protected MatrixDefinition Matrix { get; }

        #endregion

        #region Fields

        protected ColorMap ColorMap;
        protected float Min;
        protected float Max;
        
        #endregion

        protected BaseMatrixViewerVM(MatrixDefinition matrix)
        {
            Matrix = matrix;
        }

        #region Public Methods

        public virtual void UpdateColorMap(ColorMap colorMap)
        {
            ColorMap = colorMap;
        }

        public virtual void UpdateMinMax(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public virtual void Initialize(ColorMap colorMap, float min, float max)
        {
            ColorMap = colorMap;
            Min = min;
            Max = max;
        }

        #endregion

        #region Protected Methods

        protected static float GetMeasureFromCoordinate(int x, int y, IReadOnlyList<float> measures, int rowSize)
        {
            int valueIndex = x + y * rowSize;
            if (valueIndex >= measures.Count) return 0;
            return measures[valueIndex];
        }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
        }

        #endregion
    }
}
