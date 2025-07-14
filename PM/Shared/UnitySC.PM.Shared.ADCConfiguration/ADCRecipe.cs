using System;
using System.Collections.Generic;

namespace UnitySC.PM.Shared.ADCConfiguration
{
    public class ADCRecipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public System.DateTime Created { get; set; }
        public int CreatorUserId { get; set; }
        public int Version { get; set; }
        public string DataLoaderTypes { get; set; }

        public override string ToString()
        {
            return string.Format("{0}-v{1}", Name, Version);
        }

        public IEnumerable<RecipeType> RecipeTypes
        {
            get
            {
               return Array.ConvertAll(DataLoaderTypes.Split(','), value => (RecipeType)Int32.Parse(value));
            }           
        }
    }

    public enum RecipeType
    {
        Topography = 0,
        BrightField2D = 1,
        Darkfield = 2,
        BrightFieldPattern = 3,
        EyeEdge = 4,
        NanoTopography = 5,
        Lightspeed = 6,     
        BrightField3D = 7,
        EdgeInspect = 8,
        Topography_BackSide = 9
    }
}
