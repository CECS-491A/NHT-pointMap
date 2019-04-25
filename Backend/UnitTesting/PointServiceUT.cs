using System;
using System.Data.Entity.Validation;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.Services;
using static ServiceLayer.Services.ExceptionService;

namespace UnitTesting
{
    [TestClass]
    public class PointServiceUT
    {
        Point newPoint;
        TestingUtils tu;
        PointService ps;
        DatabaseContext _db;

        public PointServiceUT()
        {
            tu = new TestingUtils();
            _db = tu.CreateDataBaseContext();
            ps = new PointService(_db);
        }

        [TestMethod]
        public void Create_Point_Test_Longtitude_Latitude_Limits()
        {
            //testing valid values for longitude and latitude
            newPoint = tu.CreatePointObject(-180, -90);
            var result = ps.CreatePoint(newPoint);
            Assert.IsNotNull(result);

            newPoint = tu.CreatePointObject(180, 90);
            result = ps.CreatePoint(newPoint);
            Assert.IsNotNull(result);

            //testing invalid values for longitude and latitude
            newPoint = tu.CreatePointObject(-181, -90);
            Assert.ThrowsException<InvalidPointException>(() => ps.CreatePoint(newPoint));

            newPoint = tu.CreatePointObject(-180, -91);
            Assert.ThrowsException<InvalidPointException>(() => ps.CreatePoint(newPoint));

            newPoint = tu.CreatePointObject(181, 90);
            Assert.ThrowsException<InvalidPointException>(() => ps.CreatePoint(newPoint));

            newPoint = tu.CreatePointObject(180, 91);
            Assert.ThrowsException<InvalidPointException>(() => ps.CreatePoint(newPoint));
        }

        [TestMethod]
        public void Update_Point_Test_Longtitude_Latitude_Limits()
        {
            //setting up point in db for update testing
            newPoint = tu.CreatePointObject(-180, -90);
            tu.CreatePointInDb(newPoint);

            //testing valid values for longitude and latitude
            newPoint = tu.CreatePointObject(-180, -90);
            var result = ps.UpdatePoint(newPoint);
            Assert.IsNotNull(result);

            newPoint = tu.CreatePointObject(180, 90);
            result = ps.UpdatePoint(newPoint);
            Assert.IsNotNull(result);

            //testing invalid values for longitude and latitude
            newPoint = tu.CreatePointObject(-181, -90);
            Assert.ThrowsException<InvalidPointException>(() => ps.UpdatePoint(newPoint));

            newPoint = tu.CreatePointObject(-180, -91);
            Assert.ThrowsException<InvalidPointException>(() => ps.UpdatePoint(newPoint));

            newPoint = tu.CreatePointObject(181, 90);
            Assert.ThrowsException<InvalidPointException>(() => ps.UpdatePoint(newPoint));

            newPoint = tu.CreatePointObject(180, 91);
            Assert.ThrowsException<InvalidPointException>(() => ps.UpdatePoint(newPoint));
        }

        [TestMethod]
        public void Create_Point_Success()
        {
            // Arrange
            newPoint = tu.CreatePointObject();
            var expected = newPoint;
            using (_db = tu.CreateDataBaseContext())
            {
                // Act
                var response = ps.CreatePoint(newPoint);
                _db.SaveChanges();

                //Assert
                Assert.IsNotNull(response);
                Assert.AreSame(response, expected);
            }
        }

        [TestMethod]
        public void Create_Point_RetrieveNew_Success()
        {
            // Arrange
            newPoint = tu.CreatePointObject();
            var expected = newPoint;

            using (_db = tu.CreateDataBaseContext())
            {
                // Act
                Point response = ps.CreatePoint(newPoint);
                _db.SaveChanges();

                //Assert
                var result = _db.Points.Find(newPoint.Id);
                Assert.IsNotNull(response);
                Assert.IsNotNull(result);
                Assert.AreSame(result, expected);
            }
        }

        [TestMethod]
        public void Create_Point_Fail_ExceptionThrown()
        {
            // Arrange
            newPoint = new Point
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow

                // missing required fields
            };
            var expected = newPoint;

            using (_db = tu.CreateDataBaseContext())
            {
                // ACT
                var response = ps.CreatePoint(newPoint);
                try
                {
                    _db.SaveChanges();
                }
                catch (DbEntityValidationException)
                {
                    //catch error
                    // detach Point attempted to be created from the db context - rollback
                    _db.Entry(response).State = System.Data.Entity.EntityState.Detached;
                }
                var result = _db.Points.Find(newPoint.Id);

                // Assert
                Assert.IsNull(result);
                Assert.IsNotNull(response);
                Assert.AreEqual(expected, response);
                Assert.AreNotEqual(expected, result);
            }
        }

        [TestMethod]
        public void Delete_Point_Success()
        {
            // Arrange
            newPoint = tu.CreatePointInDb();

            var expectedResponse = newPoint;

            using (_db = tu.CreateDataBaseContext())
            {
                // Act
                var response = ps.DeletePoint(newPoint.Id);
                _db.SaveChanges();
                var result = _db.Points.Find(expectedResponse.Id);

                // Assert
                Assert.IsNotNull(response);
                Assert.IsNull(result);
                Assert.AreEqual(response.Id, expectedResponse.Id);
            }
        }

        [TestMethod]
        public void Delete_Point_NonExisting()
        {
            // Arrange
            Guid nonExistingId = Guid.NewGuid();

            var expectedResponse = nonExistingId;

            using (_db = new DatabaseContext())
            {
                // Act
                var response = ps.DeletePoint(nonExistingId);
                // will return null if Point does not exist
                _db.SaveChanges();
                var result = _db.Points.Find(expectedResponse);

                // Assert
                Assert.IsNull(response);
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void Update_Point_Success()
        {
            // Arrange
            newPoint = tu.CreatePointInDb();
            newPoint.Description = "new Description";
            var expectedResponse = newPoint;
            var expectedResult = newPoint;

            // ACT
            using (_db = tu.CreateDataBaseContext())
            {
                var response = ps.UpdatePoint(newPoint);
                _db.SaveChanges();
                var result = _db.Points.Find(expectedResult.Id);

                // Assert
                Assert.IsNotNull(response);
                Assert.IsNotNull(result);
                Assert.AreEqual(result.Id, expectedResult.Id);
                Assert.AreEqual(result.Description, expectedResult.Description);
            }

        }

        [TestMethod]
        public void Update_Point_NonExisting_why()
        {
            // Arrange
            newPoint = tu.CreatePointObject();
            newPoint.Description = "unecessary Description";
            var expectedResponse = newPoint;
            var expectedResult = newPoint;

            // ACT
            using (_db = tu.CreateDataBaseContext())
            {
                var response = ps.UpdatePoint(newPoint);
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception)
                {
                    // catch error
                    // rollback changes
                    _db.Entry(newPoint).State = System.Data.Entity.EntityState.Detached;
                }
                var result = _db.Points.Find(expectedResult.Id);

                // Assert
                Assert.IsNotNull(response);
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void Update_Point_OnRequiredValue()
        {
            // Arrange
            newPoint = tu.CreatePointInDb();
            var expectedResult = newPoint;
            newPoint.Name = "new name";
            var expectedResponse = newPoint;

            // ACT
            using (_db = tu.CreateDataBaseContext())
            {
                var response = ps.UpdatePoint(newPoint);
                try
                {
                    _db.SaveChanges();
                }
                catch (DbEntityValidationException)
                {
                    // catch error
                    // rollback changes
                    _db.Entry(response).CurrentValues.SetValues(_db.Entry(response).OriginalValues);
                    _db.Entry(response).State = System.Data.Entity.EntityState.Unchanged;
                }
                var result = _db.Points.Find(expectedResult.Id);

                // Assert
                Assert.IsNotNull(response);
                Assert.AreEqual(expectedResponse, response);
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedResult, result);
            }
        }

        [TestMethod]
        public void Get_Point_Success()
        {
            // Arrange 

            newPoint = tu.CreatePointInDb();
            var expectedResult = newPoint;

            // ACT
            using (_db = tu.CreateDataBaseContext())
            {
                var result = ps.GetPoint(expectedResult.Id);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedResult.Id, result.Id);
            }
        }

        [TestMethod]
        public void Get_Point_NonExisting()
        {
            // Arrange
            Guid nonExistingPoint = Guid.NewGuid();
            Point expectedResult = null;

            // Act
            using (_db = tu.CreateDataBaseContext())
            {
                var result = ps.GetPoint(nonExistingPoint);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(expectedResult, result);
            }
        }
    }
}
