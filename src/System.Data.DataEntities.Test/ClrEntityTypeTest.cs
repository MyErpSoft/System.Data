using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Metadata.DataEntities;

namespace System.Data.DataEntities.Test {

    [TestClass]
    public class ClrEntityTypeTest {
        [TestMethod]
        public void TestClrEntityType() {
            var dtForm2 = typeof(MyForm2).GetEntityType();
            var dtForm = typeof(MyForm).GetEntityType();

            Assert.AreEqual("System.Data.DataEntities.Test.MyForm2", dtForm2.FullName);
            Assert.AreEqual("System.Data.DataEntities.Test", dtForm2.Namespace);
            Assert.AreEqual("MyForm2", dtForm2.Name);
            Assert.IsFalse(dtForm2.IsAbstract);
            Assert.IsFalse(dtForm2.IsSealed);
            Assert.AreEqual(typeof(MyForm2), dtForm2.UnderlyingSystemType);
            Assert.IsInstanceOfType(dtForm2.CreateInstance(), typeof(MyForm2));

            //来自基类的属性
            var widthProperty = dtForm2.GetProperty("Width");
            Assert.ReferenceEquals(typeof(int).GetEntityType(), widthProperty.PropertyType);
            MyForm2 f2 = new MyForm2();
            //属性描述符
            widthProperty.SetValue(f2, 3);
            Assert.AreEqual<int>(3, (int)widthProperty.GetValue(f2));

            var w2Property = dtForm.GetProperty("Width");
            //目前的实现是希望基类和派生类公用属性描述符。
            Assert.ReferenceEquals(widthProperty, w2Property);

            IEntityProperty m;
            Assert.IsFalse(dtForm2.TryGetProperty("error", out m));
            Assert.IsNull(m);
        }
    }
}
