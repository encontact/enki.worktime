using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkTime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace WorkTime.Tests {
	[TestClass()]
	public class RecurrentExceptionsBucketTests {
		[TestInitialize]
		public void Setup() {

		}

		[TestCleanup]
		public void TearDown() {

		}

		[TestMethod()]
		public void AddTest() {
			var date2000 = new NodaTime.LocalDateTime(2000, 01, 01, 0, 0);
			var date2001 = new NodaTime.LocalDateTime(2000, 01, 01, 0, 0);

			var recurrentBucket = new RecurrentExceptionsBucket();
			try {
				recurrentBucket.Add(date2000, 480, 960);
			} catch {
				Assert.Fail();
			}

			try {
				recurrentBucket.Add(date2001, 0, 960);
				Assert.Fail();
			} catch (Exception e) {
				Assert.AreEqual("There is already an item to the date indicated in the list.", e.Message);
			}
		}
		
		[TestMethod()]
		public void HasTest() {
			var date2000 = new NodaTime.LocalDateTime(2000, 01, 01, 0, 0);
			var date2001 = new NodaTime.LocalDateTime(2000, 01, 01, 0, 0);

			var recurrentBucket = new RecurrentExceptionsBucket();
			recurrentBucket.Add(date2000, 480, 960);

			Assert.IsTrue(recurrentBucket.Has(date2000));
			Assert.IsTrue(recurrentBucket.Has(date2001));
			Assert.IsFalse(recurrentBucket.Has(new NodaTime.LocalDateTime(2005, 10, 05, 0, 0)));
		}

		[TestMethod()]
		public void GetPeriodTest() {
			var date2000 = new NodaTime.LocalDateTime(2000, 01, 01, 0, 0);
			var date2001 = new NodaTime.LocalDateTime(2000, 01, 01, 0, 0);

			var recurrentBucket = new RecurrentExceptionsBucket();
			recurrentBucket.Add(date2000, 480, 960);

			Assert.AreEqual(new Tuple<short, short>(480, 960), recurrentBucket.GetPeriod(date2000));
			Assert.AreEqual(new Tuple<short, short>(480, 960), recurrentBucket.GetPeriod(date2001));
			try {
				recurrentBucket.GetPeriod(new NodaTime.LocalDateTime(2005, 10, 05, 0, 0));
				Assert.Fail();
			} catch (KeyNotFoundException ke) {
				Assert.IsTrue(true);
			} catch (Exception e) {
				Assert.Fail();
			}
		}
	}
}
