using System;
using System.Data.DataEntities.Dynamic;
using System.Data.Metadata.DataEntities.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Data.DataEntities.Test {

    [TestClass]
    public class DynamicEntityTypeTest {

        [TestMethod]
        public void TestFreeze() {
            DynamicEntityType dt = new DynamicEntityType("test");
            Assert.IsFalse(dt.IsFrozen);

            dt.RegisterField("a", typeof(int));
            dt.Freeze();

            Assert.IsTrue(dt.IsFrozen);
            try {
                dt.RegisterField("b", typeof(int));
            }
            catch (InvalidOperationException ex) {
                Assert.IsNotNull(ex);
            }
        }
    }
}
