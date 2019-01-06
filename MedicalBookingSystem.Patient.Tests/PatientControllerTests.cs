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
        public void GetAllPatients_ThereIsPatients_ShouldReturnListOfPatients()
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
        public void GetAllPatients_IfThereIsNoPatients_ShouldReturnEmptyList()
        {
            //_patients.Clear();
            //_mockPatientRepository.Setup(p => p.GetAllNoTracking).Returns(_patients.AsQueryable());
            //_mockUnitOfWork.Setup(s => s.Repository<PatientEntity>()).Returns(_mockPatientRepository.Object);
            //_patientService = new PatientService(_mockUnitOfWork.Object);

            //var patients = _patientService.GetAllPatients();

            //Assert.AreEqual(0, patients.Count);
        }

        [Test]
        public void GetPatientById_PatientIdExists_ShouldReturnTheRightPatientItem()
        {
            //_mockPatientRepository.Setup(p => p.GetByIdAsync(It.IsAny<object[]>()))
            //                      .ReturnsAsync(new Func<object[], PatientEntity>(id => _patients.Find(p => p.Id == int.Parse(id[0].ToString()))));
            //_mockUnitOfWork.Setup(s => s.Repository<PatientEntity>()).Returns(_mockPatientRepository.Object);
            //_patientService = new PatientService(_mockUnitOfWork.Object);

            //var patient = _patientService.GetPatientByIdAsync(1).GetAwaiter().GetResult();

            //var patientFromTestList = _patients.Find(a => a.Id == 1);

            //Assert.AreEqual(patientFromTestList.Id, patient.Id);
            //Assert.AreEqual(patientFromTestList.Name, patient.Name);
        }

        [Test]
        public void GetPatientById_PatientIdNotFound_ShouldReturnNull()
        {
            //_mockPatientRepository.Setup(p => p.GetByIdAsync(It.IsAny<int>()))
            //                      .ReturnsAsync(new Func<object[], PatientEntity>(id => _patients.Find(p => p.Id == int.Parse(id[0].ToString()))));
            //_mockUnitOfWork.Setup(s => s.Repository<PatientEntity>()).Returns(_mockPatientRepository.Object);
            //_patientService = new PatientService(_mockUnitOfWork.Object);

            //var patient = _patientService.GetPatientByIdAsync(0).GetAwaiter().GetResult();

            //Assert.Null(patient);
        }
        #endregion

        #region tear down after every test 

        /// <summary>
        /// Tears down each test data
        /// </summary>
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
                    Name="Henrik Karlsson"
                },
                new PatientDTO
                {
                    Id=2,
                    Name="Erik Henriksson"
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