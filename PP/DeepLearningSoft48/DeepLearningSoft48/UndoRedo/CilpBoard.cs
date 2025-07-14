//using GraphModel;
namespace ADCv10.UndoRedo
{
    public class AdcClipBoard
    {
        private static AdcClipBoard _instance = null;


        public static AdcClipBoard Instance()
        {
            return _instance ?? (_instance = new AdcClipBoard());
        }

        //private GraphViewModel _graph;

        //public GraphViewModel Graph
        //{
        //	get
        //	{
        //		return _graph;
        //	}
        //	set
        //	{
        //		_graph = value;
        //	}
        //}

        public bool IsEmpty()
        {
            //	if (Graph != null)
            //	{
            //		return Graph.Nodes.Count == 0;
            //	}
            return true;
        }

        public void Clear()
        {
            //if (Graph != null)
            //{
            //	Graph.Nodes.Clear();
            //	Graph.Connections.Clear();
            //}
        }

    }
}
