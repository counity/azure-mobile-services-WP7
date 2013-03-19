using System;
using NUnit.Framework;

namespace Test.coUnity.WindowsAzure.MobileServices
{
    [TestFixture]
    public class MobileServiceTableTest : TestBase
    {
        [Test]
        public void CanInsert()
        {
            var o = new TestType { IntColumn = 17, TextColumn = "some text to be stored"};
            Client.GetTable<TestType>().Insert(o);
            Assert.AreNotEqual(0, o.Id);
        }

        [Test]
        public void CanInsertAndSelect()
        {
            var now = DateTime.Now;

            var o = new TestType
                        {
                            Bool = true,
                            DateTime = now,
                            Double = 17.17,
                            Float = 13.13888999,
                            IntColumn = 7,
                            TextColumn = "some text to be stored"
                        };
            var table = Client.GetTable<TestType>();

            table.Insert(o);
            var target = table.Get(o.Id);
            Assert.IsNotNull(target);
            Assert.AreEqual(o.Id, target.Id);
            Assert.AreEqual(o.Bool, target.Bool);
            Assert.That(o.DateTime, Is.EqualTo(target.DateTime).Within(TimeSpan.FromMilliseconds(1))); //datetime is truncated on insert
            Assert.That(now, Is.EqualTo(target.DateTime).Within(TimeSpan.FromMilliseconds(1))); //datetime is truncated on insert
            Assert.AreEqual(o.Double, target.Double);
            Assert.AreEqual(o.Float, target.Float);
            Assert.AreEqual(o.IntColumn, target.IntColumn);
            Assert.AreEqual(o.TextColumn, target.TextColumn);
        }

        [Test]
        public void CanUpdate()
        {
            var o = new TestType
            {
                Bool = true,
                DateTime = DateTime.UtcNow,
                Double = 17.17,
                Float = 13.13888999,
                IntColumn = 7,
                TextColumn = "some text to be stored"
            };
            var table = Client.GetTable<TestType>();

            table.Insert(o);

            o.Bool = false;
            o.DateTime = o.DateTime.AddHours(1);
            o.Double = o.Double + 17;
            o.Float = o.Float + 17;
            o.TextColumn = o.TextColumn + o.TextColumn;
            o.IntColumn = o.IntColumn + o.IntColumn;

            table.Update(o);

            var target = table.Get(o.Id);

            Assert.IsNotNull(target);
            Assert.AreEqual(o.Id, target.Id);
            Assert.AreEqual(o.Bool, target.Bool);
            Assert.That(o.DateTime, Is.EqualTo(target.DateTime).Within(TimeSpan.FromMilliseconds(1))); //datetime is truncated on insert
            Assert.AreEqual(o.Double, target.Double);
            Assert.AreEqual(o.Float, target.Float);
            Assert.AreEqual(o.IntColumn, target.IntColumn);
            Assert.AreEqual(o.TextColumn, target.TextColumn);
        }

        [Test]
        public void CanDelete()
        {
            var o = new TestType { IntColumn = 17, TextColumn = "some text to be stored" };
            var table = Client.GetTable<TestType>();
            table.Insert(o);
            table.Delete(o);
            Assert.IsNull(table.Get(o.Id));
        }

        [ExpectedException(typeof(ArgumentException))]
        [Test]
        public void CannotInsertWithSetId()
        {
            var obj = new TestType { Id = 1000, IntColumn = 17, TextColumn = "some text to be stored" };
            Client.GetTable<TestType>().Insert(obj);
        }

        [Test]
        public void CanInsertIdOnly()
        {
            var obj = new OnlyId();
            Client.GetTable<OnlyId>().Insert(obj);
            Assert.AreNotEqual(0, obj.Id);
        }
    }
}
