using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Data.ORM.Test {
    [TestClass]
    public class EntityReaderTest {
        [TestMethod]
        public void TestRead() {

        }
    }

    internal sealed class MySelector : Metadata.Mapping.EntitySelector {
        public override bool TryCreateEntity(object[] values, out object entity) {
            throw new NotImplementedException();
        }
    }
}
