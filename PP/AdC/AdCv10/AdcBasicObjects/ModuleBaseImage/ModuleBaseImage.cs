using System.Windows.Controls;
using ADCEngine;
using LibProcessing;
using AdcBasicObjects.Rendering;

namespace AdcBasicObjects
{
	///////////////////////////////////////////////////////////////////////
	// Classe de base des modules qui traitent des images
	///////////////////////////////////////////////////////////////////////
	public class ModuleBaseImage: ModuleBase
	{
		private static ProcessingClass _processClass = new ProcessingClassCpp();

		//=================================================================
		// Constructeur
		//=================================================================
		public ModuleBaseImage(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
		}

		//=================================================================
		// 
		//=================================================================
		public override void Process(ModuleBase parent, ObjectBase obj)
		{
			ProcessChildren(obj);
		}

		/// <summary>
		/// By default, create an image renderig
		/// </summary>
		/// <returns></returns>
		public override UserControl GetRenderingUI()
		{
			if (Rendering == null)
				Rendering = new ImageRendering(this, memoOutObj);
			else
				Rendering.Data = memoOutObj;

			return Rendering.RenderingUI;
		}


	}
}
