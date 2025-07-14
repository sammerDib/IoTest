using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySCSharedAlgosCppWrapper;

namespace SharedCpp.WrapperTests
{
    [TestClass]
    public class Profile2dTests
    {
        [TestMethod]
        public void Count()
        {
            var profile = new Profile2d {
                new Point2d( -0.5, 1.0 ),
                new Point2d( 0.0, 2.0 ),
                new Point2d( 0.5, 3.0 ),
            };
            Assert.AreEqual(3, profile.Count());
        }

        [TestMethod]
        public void WhereSelect()
        {
            var profile = new Profile2d {
                new Point2d( -0.5, 1.0 ),
                new Point2d( 0.0, 1.0 ),
                new Point2d( 0.5, 3.0 ),
            };

            var filtered = profile.Where(pt => pt.Y == 1.0).Select(pt => pt.X).ToList();

            Assert.AreEqual(2, filtered.Count());
            Assert.AreEqual(-0.5, filtered[0]);
            Assert.AreEqual(0.0, filtered[1]);
        }

        [TestMethod]
        public void IndexerGet()
        {
            var profile = new Profile2d {
                new Point2d( -0.5, 1.0 ),
                new Point2d( 0.0, 1.0 ),
                new Point2d( 0.5, 3.0 ),
            };

            Assert.AreEqual(-0.5, profile[0].X);
            Assert.AreEqual(1.0, profile[0].Y);
            Assert.AreEqual(0.0, profile[1].X);
            Assert.AreEqual(1.0, profile[1].Y);
            Assert.AreEqual(0.5, profile[2].X);
            Assert.AreEqual(3.0, profile[2].Y);
        }

        [TestMethod]
        public void IndexerSet()
        {
            var profile = new Profile2d {
                new Point2d( -0.5, 1.0 ),
                new Point2d( 0.0, 1.0 ),
                new Point2d( 0.5, 3.0 ),
            };

            profile[0].X = -1.0;
            profile[0].Y = 100.0;
            profile[1] = new Point2d(-0.5, 50.0);

            Assert.AreEqual(-1.0, profile[0].X);
            Assert.AreEqual(100.0, profile[0].Y);
            Assert.AreEqual(-0.5, profile[1].X);
            Assert.AreEqual(50.0, profile[1].Y);
            Assert.AreEqual(0.5, profile[2].X);
            Assert.AreEqual(3.0, profile[2].Y);
        }

        [TestMethod]
        public void ModifyThroughIterator()
        {
            var profile = new Profile2d {
                new Point2d( -0.5, 1.0 ),
                new Point2d( 0.0, 1.0 ),
                new Point2d( 0.5, 3.0 ),
            };

            foreach (var point in profile)
            {
                point.X /= 2.0;
                point.Y = 50.0;
            }

            Assert.AreEqual(-0.25, profile[0].X);
            Assert.AreEqual(50.0, profile[0].Y);
            Assert.AreEqual(0.0, profile[1].X);
            Assert.AreEqual(50.0, profile[1].Y);
            Assert.AreEqual(0.25, profile[2].X);
            Assert.AreEqual(50.0, profile[2].Y);
        }
    }
}
