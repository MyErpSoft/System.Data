using System;
using System.Data.DataEntities.Dynamic;
using System.Data.DataEntities.Metadata.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Data.DataEntities.Test {
    [TestClass]
    public class DynamicFieldTest {
        [TestMethod]
        public void TestStructTypeField() {
            DynamicEntityType dt = new DynamicEntityType("StructTest");
            var int32Field = dt.RegisterField("Int32", typeof(int));
            var decimalField = dt.RegisterField("Decimal", typeof(decimal));
            var sizeField = dt.RegisterField("Size", typeof(Size));

            //default value
            var entity = new DynamicEntity(dt);
            Assert.AreEqual(0, int32Field.GetValue(entity));
            Assert.AreEqual(0m, decimalField.GetValue(entity));
            Assert.AreEqual(new Size(), sizeField.GetValue(entity));

            //set new value
            int32Field.SetValue(entity, 1);
            decimalField.SetValue(entity, 1m);
            sizeField.SetValue(entity, new Size() { Width = 3, Height = 2 });

            Assert.AreEqual(1, int32Field.GetValue(entity));
            Assert.AreEqual(1m, decimalField.GetValue(entity));
            Assert.AreEqual(new Size() { Width = 3, Height = 2 }, sizeField.GetValue(entity));

            //reset default Value;
            int32Field.SetValue(entity, 0);
            decimalField.SetValue(entity, 0m);
            sizeField.SetValue(entity, new Size());

            Assert.AreEqual(0, int32Field.GetValue(entity));
            Assert.AreEqual(0m, decimalField.GetValue(entity));
            Assert.AreEqual(new Size(), sizeField.GetValue(entity));

        }

        [TestMethod]
        public void TestMoreField() {
            DynamicEntityType dt = new DynamicEntityType("StructTest");
            DynamicEntityField int32Field;
            DynamicEntityField decimalField;
            DynamicEntityField sizeField;

            for (int i = 0; i < 20; i++) {
                dt.RegisterField("Int32" + i.ToString(), typeof(int));
                dt.RegisterField("Decimal" + i.ToString(), typeof(decimal));
                dt.RegisterField("Size" + i.ToString(), typeof(Size));
            }

            int32Field = dt.RegisterField("Int32", typeof(int));
            decimalField = dt.RegisterField("Decimal", typeof(decimal));
            sizeField = dt.RegisterField("Size", typeof(Size));

            //default value
            var entity = new DynamicEntity(dt);
            Assert.AreEqual(0, int32Field.GetValue(entity));
            Assert.AreEqual(0m, decimalField.GetValue(entity));
            Assert.AreEqual(new Size(), sizeField.GetValue(entity));

            //set new value
            int32Field.SetValue(entity, 1);
            decimalField.SetValue(entity, 1m);
            sizeField.SetValue(entity, new Size() { Width = 3, Height = 2 });

            Assert.AreEqual(1, int32Field.GetValue(entity));
            Assert.AreEqual(1m, decimalField.GetValue(entity));
            Assert.AreEqual(new Size() { Width = 3, Height = 2 }, sizeField.GetValue(entity));

            //reset default Value;
            int32Field.ResetValue(entity);
            decimalField.ResetValue(entity);
            sizeField.ResetValue(entity);

            Assert.AreEqual(0, int32Field.GetValue(entity));
            Assert.AreEqual(0m, decimalField.GetValue(entity));
            Assert.AreEqual(new Size(), sizeField.GetValue(entity));

        }

        [TestMethod]
        public void TestObjectTypeField() {
            DynamicEntityType dt = new DynamicEntityType("ObjectTest");
            var objField = dt.RegisterField("obj1", typeof(MyForm));
            var entity = new DynamicEntity(dt);
            
            //default value
            Assert.AreEqual(null, objField.GetValue(entity));

            //set new value
            var v = new MyForm();
            objField.SetValue(entity, v);
            Assert.AreEqual(v, objField.GetValue(entity));

            //reset
            objField.SetValue(entity, null);
            Assert.IsNull(objField.GetValue(entity));

            var v2 = new MyForm2();
            objField.SetValue(entity, v2);
            Assert.AreEqual(v2, objField.GetValue(entity));
            objField.ResetValue(entity);
            Assert.IsNull(objField.GetValue(entity));
        }

        [TestMethod]
        public void TestNullable() {
            DynamicEntityType dt = new DynamicEntityType("ObjectTest");
            var objField = dt.RegisterField("obj1", typeof(int?));
            var entity = new DynamicEntity(dt);

            //default value
            Assert.AreEqual(null, objField.GetValue(entity));

            //set new value
            objField.SetValue(entity, 1);
            Assert.AreEqual(1, objField.GetValue(entity));

            //reset
            objField.SetValue(entity, null);
            Assert.IsNull(objField.GetValue(entity));

            objField.SetValue(entity, 1);
            Assert.AreEqual(1, objField.GetValue(entity));
            objField.ResetValue(entity);
            Assert.IsNull(objField.GetValue(entity));
        }

        [TestMethod]
        public void TestDynamicRegister() {
            DynamicEntityType dt = new DynamicEntityType("ObjectTest");
            var int32Field = dt.RegisterField("Int32", typeof(int));
            var entity = new DynamicEntity(dt);

            int32Field.SetValue(entity, 2);
            Assert.AreEqual(2, int32Field.GetValue(entity));

            var decimalField = dt.RegisterField("Decimal", typeof(decimal));
            decimalField.SetValue(entity, 3m);
            Assert.AreEqual(3m, decimalField.GetValue(entity));
            
        }

        [TestMethod]
        public void TestRegisterComplexProperty() {
            var dt = new DynamicEntityType("Csutomer");
            var idField = dt.RegisterField("Id", typeof(int));

            var dt2 = new DynamicEntityType("Address");
            var homeField = dt.RegisterField("Home", typeof(string));

            var addressField = dt.RegisterField("Address", dt2);

            var obj = new DynamicEntity(dt);
            Assert.IsNull(addressField.GetValue(obj));

            var address = new DynamicEntity(dt2);
            addressField.SetValue(obj, address);
            Assert.AreEqual(address, addressField.GetValue(obj));

            Exception ex2 = null;
            try {
                addressField.SetValue(obj, new DynamicEntity(dt));
            }
            catch (Exception ex) {
                ex2 = ex;
            }
            Assert.IsNotNull(ex2 != null);

            ex2 = null;
            try {
                addressField.SetValue(obj, 5);
            }
            catch (Exception ex) {
                ex2 = ex;
            }
            Assert.IsNotNull(ex2 != null);
        }

        [TestMethod]
        public void TestGetSetValuePerformance() {
            DynamicEntityType dt = new DynamicEntityType("StructTest");
            var int32Field = dt.RegisterField("Int32", typeof(int));
            var entity = new DynamicEntity(dt);

            PerformanceTest.Do(9000000, 20000000, new TimeSpan(0, 0, 1), (i) => {
                int32Field.SetValue(entity, i);
            });

            PerformanceTest.Do(15000000, 60000000, new TimeSpan(0, 0, 1), (i) => {
                int j = (int)int32Field.GetValue(entity);
            });

            var int32NullField = dt.RegisterField("Int32Null", typeof(int?));
            PerformanceTest.Do(9000000, 20000000, new TimeSpan(0, 0, 1), (i) => {
                int32NullField.SetValue(entity, i);
            });

            PerformanceTest.Do(15000000, 60000000, new TimeSpan(0, 0, 1), (i) => {
                int32NullField.GetValue(entity);
                //int? j = (int?)Why this should more time?
            });

            var objField = dt.RegisterField("Obj", typeof(MyForm));
            var value = new MyForm();
            PerformanceTest.Do(9000000, 20000000, new TimeSpan(0, 0, 1), (i) => {
                objField.SetValue(entity, value);
            });

            PerformanceTest.Do(15000000, 60000000, new TimeSpan(0, 0, 1), (i) => {
                MyForm j = (MyForm)objField.GetValue(entity);
            });
        }

        [TestMethod]
        public void TestGetSetValuePerformance2() {
            DynamicEntityType dt = new DynamicEntityType("StructTest");
             //using dict
            for (int i = 0; i < 31; i++) {
                dt.RegisterField("A" + i.ToString(), typeof(int));
            }
           var int32Field = dt.RegisterField("Int32", typeof(int));
           var entity = new DynamicEntity(dt);

            PerformanceTest.Do(6000000, 10000000, new TimeSpan(0, 0, 1), (i) => {
                int32Field.SetValue(entity, i);
            });

            PerformanceTest.Do(8000000, 15000000, new TimeSpan(0, 0, 1), (i) => {
                int j = (int)int32Field.GetValue(entity);
            });

            var int32NullField = dt.RegisterField("Int32Null", typeof(int?));
            PerformanceTest.Do(6000000, 10000000, new TimeSpan(0, 0, 1), (i) => {
                int32NullField.SetValue(entity, i);
            });

            PerformanceTest.Do(8000000, 15000000, new TimeSpan(0, 0, 1), (i) => {
                int32NullField.GetValue(entity);
                //int? j = (int?)Why this should more time?
            });

            var objField = dt.RegisterField("Obj", typeof(MyForm));
            var value = new MyForm();
            PerformanceTest.Do(6000000, 15000000, new TimeSpan(0, 0, 1), (i) => {
                objField.SetValue(entity, value);
            });

            PerformanceTest.Do(8000000, 15000000, new TimeSpan(0, 0, 1), (i) => {
                MyForm j = (MyForm)objField.GetValue(entity);
            });
        }
    }


}
