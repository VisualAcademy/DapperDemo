using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace DapperDemo.Models.Tests
{
    [TestClass]
    public class TableRepositoryTests
    {
        //[TestMethod]
        //public void FirstDataCountIsZero()
        //{
        //    // Arrange
        //    ITableRepository repository = new TableRepository();

        //    // Act
        //    var tables = repository.GetAll();

        //    var expected = 0;

        //    var actual = tables.Count; // 0

        //    // Assert
        //    Assert.AreEqual(expected, actual); // true
        //}

        //[TestMethod]
        //public void SecondInsertOk()
        //{
        //    // Arrange
        //    ITableRepository repository = new TableRepository();

        //    TableViewModel model = new TableViewModel();
        //    model.Note = "두번째";

        //    // Act
        //    var table = repository.Add(model);

        //    var expected = 2;

        //    var actual = table.Id; // 2

        //    // Assert
        //    Assert.AreEqual(expected, actual);
        //}

        //[TestMethod]
        //public void RecordIs()
        //{
        //    // Arrange
        //    ITableRepository repository = new TableRepository();

        //    // Act
        //    var model1 = repository.GetById(1);
        //    var model2 = repository.GetById(2);

        //    // Assert
        //    Assert.AreEqual(1, model1.Id);
        //    Assert.AreEqual("첫번째", model1.Note);
        //    Assert.AreEqual(2, model2.Id);
        //    Assert.AreEqual("두번째", model2.Note);
        //}

        //[TestMethod]
        //public void UpdateSecondDataTest()
        //{
        //    // Arrange
        //    ITableRepository repository = new TableRepository();

        //    // Act
        //    var model = new TableViewModel();
        //    model.Id = 2;
        //    model.Note = "둘째";
        //    repository.Update(model); // 두번째 => 둘째

        //    var data = repository.GetById(2);

        //    // Assert
        //    Assert.AreEqual("둘째", data.Note);
        //}

        //[TestMethod]
        //public void RemoveTestAndCountTest()
        //{
        //    // Arrange
        //    ITableRepository repository = new TableRepository();

        //    // Act
        //    repository.Remove(1); // 1번 자료 삭제

        //    var cnt = repository.GetAll().Count;

        //    // Assert
        //    Assert.AreEqual(1, cnt); // 2 -> 1
        //}

        [TestMethod]
        public void BulkInsertTest()
        {
            // Arrange
            var repository = new TableRepository();

            var tables = new List<TableViewModel> {
                new TableViewModel { Note = "세번째" },
                new TableViewModel { Note = "네번째" }
            };

            // Act
            var rowCount = repository.BulkInsertRecords(tables);

            // Assert
            Assert.AreEqual(2, rowCount);
        }

        [TestMethod]
        public void MultiDataTest()
        {
            // Arrange
            var repository = new TableRepository();

            // Act
            var tables = repository.GetByIds(1, 3, 4); // 2개

            // Assert
            Assert.AreEqual(2, tables.Count); // 2 == 2
        }

        [TestMethod]
        public void DynamicDataTest()
        {
            // Arrange
            var repository = new TableRepository();

            // Act
            var tables = repository.GetDynamicAll();

            // Microsoft.CSharp.dll 참조
            var firstNote = tables.First().Note; // 둘째
            var lastId = tables.Last().Id; // 4
            var lastNote = tables.Last().Note; // 네번째

            // Assert
            Assert.AreEqual(3, tables.Count);
            Assert.AreEqual("둘째", firstNote);
            Assert.AreEqual(4, lastId);
            Assert.AreEqual("네번째", lastNote);
        }


    }
}
