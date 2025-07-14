using System.Windows.Controls;

using AdcBasicObjects.Rendering;

using ADCEngine;

using LibProcessing;

namespace BasicModules
{
    ///////////////////////////////////////////////////////////////////////
    // Classe de base des modules qui traitent des images
    ///////////////////////////////////////////////////////////////////////
    public abstract class ImageModuleBase : ModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassCpp();

        //=================================================================
        // Constructeur
        //=================================================================
        public ImageModuleBase(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
        }

        //=================================================================
        // Rendering
        //=================================================================
        private ImageRenderingViewModel _renderingVM;

        public override UserControl RenderingUI
        {
            get
            {
                ImageRenderingView renderingView = ImageRenderingView.DefaultInstance;
                if (_renderingVM == null)
                    _renderingVM = new ImageRenderingViewModel(this);
                renderingView.DataContext = _renderingVM;

                return renderingView;
            }
        }

    }
}
