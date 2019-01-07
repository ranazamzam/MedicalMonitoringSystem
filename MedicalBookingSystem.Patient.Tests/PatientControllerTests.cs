using MedicalBookingSystem.Patient.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Patient.Domain.DataTransferObjects;
using Patient.Domain.Models;
using Patient.Services.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Tests
{
    public class PatientControllerTests
    {
        private static Mock<ILogger<PatientController>> _loggerMock;
        private Mock<IPatientService> _patientServiceMock;
        private List<PatientDTO> _patients;
        private PatientController _patientsController;

        #region One time setup before all tests
        [OneTimeSetUp]
        public void Setup()
        {
            _patients = SetUpPatients();
            _loggerMock = new Mock<ILogger<PatientController>>();
        }
        #endregion

        #region  setup before each test
        [SetUp]
        public void ReInitializeTest()
        {
            _patients = SetUpPatients();
            _patientServiceMock = new Mock<IPatientService>();
        }
        #endregion

        #region unit tests

        /// <summary>
        /// Should return all patients if 
        /// </summary>
        [Test]
        public void GetAllPatients_ThereIsPatients_ShouldReturnOkAndListOfPatients()
        {
            // Arrange
            _patientServiceMock.Setup(p => p.GetAllPatients()).Returns(_patients);
            _patientsController = new PatientController(_patientServiceMock.Object, _loggerMock.Object);

            // Act
            var response = _patientsController.GetAllPatients();

            var okResult = response as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var returnedPatientDTOs = okResult.Value as List<PatientDTO>;
            Assert.IsNotNull(returnedPatientDTOs);
            Assert.IsInstanceOf(typeof(List<PatientDTO>), returnedPatientDTOs);
            Assert.AreEqual(returnedPatientDTOs.Any(), true);

            var comparer = new PatientDTOComparer();
            CollectionAssert.AreEqual(returnedPatientDTOs.OrderBy(product => product, comparer),
                                     _patients.OrderBy(product => product, comparer), comparer);

            Assert.AreEqual(2, returnedPatientDTOs.Count);

        }

        [Test]
        public void GetAllPatients_IfThereIsNoPatients_ShouldReturnNotFoundAndEmptyList()
        {
            // Arrange
            _patients.Clear();
            _patientServiceMock.Setup(p => p.GetAllPatients()).Returns(_patients);
            _patientsController = new PatientController(_patientServiceMock.Object, _loggerMock.Object);

            // Act
            var response = _patientsController.GetAllPatients();

            var notFoundResult = response as NotFoundResult;

            // Assert
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public void GetAllPatients_IfThrowException_ShouldLogAndReturnExceptionMessage()
        {
            // Arrange
            _patientServiceMock.Setup(p => p.GetAllPatients()).Throws<Exception>();
            _patientsController = new PatientController(_patientServiceMock.Object, _loggerMock.Object);

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _patientsController.GetAllPatients());
            Assert.That(ex.Message, Is.EqualTo("An exception occured."));
        }

        [Test]
        public void GetPatientById_ThereIsPatient_ShouldReturnOkAndExpectedPatient()
        {
            // Arrange
            _patientServiceMock.Setup(p => p.GetPatientById(It.IsAny<int>()))
                                 .Returns(new Func<int, PatientDTO>(id => _patients.Find(p => p.Id == id)));
            _patientsController = new PatientController(_patientServiceMock.Object, _loggerMock.Object);

            // Act
            var response = _patientsController.GetPatientById(1);

            var okResult = response as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var returnedPatientDTO = okResult.Value as PatientDTO;
            Assert.IsNotNull(returnedPatientDTO);
            Assert.IsInstanceOf(typeof(PatientDTO), returnedPatientDTO);

            var patientFromTestList = _patients.Find(a => a.Id == 1);
            Assert.AreEqual(patientFromTestList.Id, returnedPatientDTO.Id);
            Assert.AreEqual(patientFromTestList.Name, returnedPatientDTO.Name);

        }

        [Test]
        public void GetPatientById_PatientNotFound_ShouldReturnNotFoundAndNullPatient()
        {
            // Arrange
            _patientServiceMock.Setup(p => p.GetPatientById(It.IsAny<int>()))
                               .Returns(new Func<int, PatientDTO>(id => _patients.Find(p => p.Id == id)));
            _patientsController = new PatientController(_patientServiceMock.Object, _loggerMock.Object);

            // Act
            var response = _patientsController.GetPatientById(10);

            var notFoundResult = response as NotFoundResult;

            // Assert
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public void GetPatientById_InvalidId_ShouldReturnBadRequest()
        {
            // Arrange
            _patientsController = new PatientController(_patientServiceMock.Object, _loggerMock.Object);

            // Act
            var response = _patientsController.GetPatientById(-1);

            var badRequestResult = response as BadRequestResult;

            // Assert
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [Test]
        public void GetPatientById_IfThrowException_ShouldLogAndReturnExceptionMessage()
        {
            // Arrange
            _patientServiceMock.Setup(p => p.GetPatientById(It.IsAny<int>())).Throws<Exception>();
            _patientsController = new PatientController(_patientServiceMock.Object, _loggerMock.Object);

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _patientsController.GetPatientById(1));
            Assert.That(ex.Message, Is.EqualTo("An exception occured."));
        }
        #endregion

        #region tear down after every test 

        [TearDown]
        public void DisposeTest()
        {
            _patientsController = null;
            _patientServiceMock = null;
            _patients = null;
        }
        #endregion

        #region One time tear down after all tests

        [OneTimeTearDown]
        public void DisposeAllObjects()
        {
            _patients = null;
            _loggerMock = null;
        }
        #endregion

        #region Helpers and Data Initializer
        private List<PatientDTO> SetUpPatients()
        {
            return new List<PatientDTO>()
            {
                new PatientDTO
                {
                    Id=1,
                    Name="Henrik Karlsson Test Patient"
                },
                new PatientDTO
                {
                    Id=2,
                    Name="Erik Henriksson Test Patient"
                },
            };
        }

        public class PatientDTOComparer : IComparer, IComparer<PatientDTO>
        {
            public int Compare(object expected, object actual)
            {
                var lhs = expected as PatientDTO;
                var rhs = actual as PatientDTO;
                if (lhs == null || rhs == null) throw new InvalidOperationException();
                return Compare(lhs, rhs);
            }

            public int Compare(PatientDTO expected, PatientDTO actual)
            {
                int temp;
                return (temp = expected.Id.CompareTo(actual.Id)) != 0 ? temp : expected.Name.CompareTo(actual.Name);
            }
        }

        #endregion
    }
}