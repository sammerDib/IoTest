using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.Shared.MathLib.Geometry;

namespace UnitySC.PM.Shared.MathLib.Transform
{
    public class Transform2DMark
    {
        public class Transform2DLink
        {
            public List<String> Tags { get; set; }
            public Transform2DMark Mark { get; set; }

            public Transform2DLink()
            {
                Tags = new List<string>();
            }
        }

        public MatrixCorrection Correction { get; set; }
        public string Tag { get; set; }
        public List<Transform2DLink> NextLinks { get; set; }
        public List<Transform2DLink> PreviousLinks { get; set; }

        public Transform2DMark(string tag, MatrixCorrection correction)
        {
            Tag = tag;
            Correction = correction;
            NextLinks = new List<Transform2DLink>();
            PreviousLinks = new List<Transform2DLink>();
        }

        public void AddNext(Transform2DMark nextMark)
        {
            Transform2DLink NewLink = new Transform2DLink();
            NewLink.Mark = nextMark;
            NextLinks.Add(NewLink);
        }

        public void AddPrevious(Transform2DMark previousMark)
        {
            Transform2DLink NewLink = new Transform2DLink();
            NewLink.Mark = previousMark;
            PreviousLinks.Add(NewLink);
        }

        public bool ComputeNext(string tag, string sourceTag, List<string> tagCall, bool add)
        {
            bool isFound = false;
            if ((sourceTag != Tag) || (add == true))
            {
                foreach (Transform2DLink Link in NextLinks)
                {
                    if (!isFound)
                    {
                        if (Link.Mark.Tag == tag)
                        {
                            isFound = true;
                            if (add)
                            {
                                if (Link.Tags.IndexOf(tag) < 0)
                                {
                                    Link.Tags.Add(tag);
                                }
                            }
                        }
                        else
                        {
                            if (tagCall.IndexOf(Link.Mark.Tag) < 0)
                            {
                                isFound = Link.Mark.ComputeNext(tag, sourceTag, tagCall, false);

                                if (!isFound)
                                {
                                    isFound = Link.Mark.ComputePrevious(tag, sourceTag, tagCall, false);
                                }
                                else
                                {
                                    if (add)
                                    {
                                        if (Link.Tags.IndexOf(tag) < 0)
                                        {
                                            Link.Tags.Add(tag);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                tagCall.Add(Tag);
            }
            return isFound;
        }

        public bool ComputePrevious(string tag, string sourceTag, List<string> tagCall, bool add)
        {
            bool isFound = false;

            if ((sourceTag != Tag) || (add == true))
            {
                foreach (Transform2DLink Link in PreviousLinks)
                {
                    if (!isFound)
                    {
                        if (Link.Mark.Tag == tag)
                        {
                            isFound = true;
                            if (add)
                            {
                                if (Link.Tags.IndexOf(tag) < 0)
                                {
                                    Link.Tags.Add(tag);
                                }
                            }
                        }
                        else
                        {
                            if (tagCall.IndexOf(Link.Mark.Tag) < 0)
                            {
                                isFound = Link.Mark.ComputePrevious(tag, sourceTag, tagCall, false);
                                if (!isFound)
                                {
                                    isFound = Link.Mark.ComputeNext(tag, sourceTag, tagCall, false);
                                }
                                else
                                {
                                    if (add)
                                    {
                                        if (Link.Tags.IndexOf(tag) < 0)
                                        {
                                            Link.Tags.Add(tag);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                tagCall.Add(Tag);
            }
            return isFound;
        }
    }

    public class Transform2DMarkList : List<Transform2DMark>
    {
        public Point4D ApplyCorrection(Point4D posIni)
        {
            return ApplyCorrection(posIni, this.ElementAt(0).Tag, this.ElementAt(Count).Tag);
        }

        public Point4D ApplyCorrection(Point4D posIni, string markTagSource, string markTagDest)
        {
            Point4D Result = GetMatrixCorrection(markTagSource, markTagDest).ApplyCorrection(posIni);
            return Result;
        }

        public Point4D ApplyRevertCorrection(Point4D posIni)
        {
            return ApplyRevertCorrection(posIni, this.ElementAt(0).Tag, this.ElementAt(Count).Tag);
        }

        public Point4D ApplyRevertCorrection(Point4D posIni, string markTagSource, string markTagDest)
        {
            Point4D Result = GetMatrixCorrection(markTagSource, markTagDest).ApplyRevertCorrection(posIni);
            return Result;
        }

        public MatrixCorrection GetMatrixCorrection(string markTagSource, string markTagDest)
        {
            Matrix GlobalMatrix = new MatrixCorrection().Matrix;

            Transform2DMark LeafSource = this.First(c => c.Tag == markTagSource);
            Transform2DMark LeafDest = this.First(c => c.Tag == markTagDest);

            Transform2DMark CurrentLeaf = LeafSource;

            while (CurrentLeaf.Tag != markTagDest)
            {
                Transform2DMark NextLeaf = null;
                Transform2DMark PreviousLeaf = null;
                bool isNext = false; ;

                if (CurrentLeaf.NextLinks.Count(c => c.Tags.IndexOf(markTagDest) >= 0) > 0)
                {
                    NextLeaf = CurrentLeaf.NextLinks.First(c => c.Tags.IndexOf(markTagDest) >= 0).Mark;
                    isNext = true;
                }
                else
                {
                    PreviousLeaf = CurrentLeaf.PreviousLinks.First(c => c.Tags.IndexOf(markTagDest) >= 0).Mark;
                }
                if (isNext)
                {
                    GlobalMatrix = CurrentLeaf.Correction.Matrix.DotProduct(GlobalMatrix);
                    CurrentLeaf = NextLeaf;
                }
                else
                {
                    GlobalMatrix = PreviousLeaf.Correction.InvMatrix.DotProduct(GlobalMatrix);
                    CurrentLeaf = PreviousLeaf;
                }
            }
            return new MatrixCorrection(GlobalMatrix);
        }

        public void ComputePaths()
        {
            foreach (Transform2DMark Source in this)
            {
                foreach (Transform2DMark Target in this)
                {
                    if (Source.Tag != Target.Tag)
                    {
                        if (!Source.ComputeNext(Target.Tag, Source.Tag, new List<string>(), true))
                        {
                            Source.ComputePrevious(Target.Tag, Source.Tag, new List<string>(), true);
                        }
                    }
                }
            }
        }
    }
}