using Moq;
using NUnit.Framework;
using Patient.Domain.DataTransferObjects;
using Patient.Domain.Interfaces;
using Patient.Domain.Models;
using Patient.Services.Interfaces;
using Patient.Services.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class PatientserviceTests
    {
        private IPatientService _patientService;
        private List<PatientEntity> _patients;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IRepository<PatientEntity>> _mockPatientRepository;

        #region One time setup before all tests
        [OneTimeSetUp]
        public void Setup()
        {
            _patients = SetUpPatients();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
        }
        #endregion

        #region  setup before each test
        [SetUp]
        public void ReInitializeTest()
        {
            _patients = SetUpPatients();
            _mockPatientRepository = new Mock<IRepository<PatientEntity>>();
        }
        #endregion

        #region unit tests
        
        [Test]
        public void GetAllPatients_ThereIsPatients_ShouldReturnListOfPatients()
        {
            // Arrange
            _mockPatientRepository.Setup(p => p.GetAllNoTracking).Returns(_patients.AsQueryable());
            _mockUnitOfWork.Setup(s => s.Repository<PatientEntity>()).Returns(_mockPatientRepository.Object);
            _patientService = new PatientService(_mockUnitOfWork.Object);

            // Act
            var patients = _patientService.GetAllPatients();

            var patientsList = patients.Select(x => new PatientEntity
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            patientsList.RemoveAt(0);

            // Assert
            var comparer = new PatientEntityComparer();
            CollectionAssert.AreEqual(patientsList.OrderBy(product => product, comparer),
                                     _patients.OrderBy(product => product, comparer), comparer);

            Assert.AreEqual(2, patientsList.Count);

            Assert.IsInstanceOf(typeof(List<PatientDTO>), patients);
        }

        [Test]
        public void GetAllPatients_IfThereIsNoPatients_ShouldReturnEmptyList()
        {
            _patients.Clear();
            _mockPatientRepository.Setup(p => p.GetAllNoTracking).Returns(_patients.AsQueryable());
            _mockUnitOfWork.Setup(s => s.Repository<PatientEntity>()).Returns(_mockPatientRepository.Object);
            _patientService = new PatientService(_mockUnitOfWork.Object);

            var patients = _patientService.GetAllPatients();

            Assert.AreEqual(0, patients.Count);
        }

        [Test]
        public void GetPatientById_PatientIdExists_ShouldReturnTheRightPatientItem()
        {
            _mockPatientRepository.Setup(p => p.GetByIdAsync(It.IsAny<object[]>()))
                                  .ReturnsAsync(new Func<object[], PatientEntity>(id => _patients.Find(p => p.Id ==int.Parse(id[0].ToString()))));
            _mockUnitOfWork.Setup(s => s.Repository<PatientEntity>()).Returns(_mockPatientRepository.Object);
            _patientService = new PatientService(_mockUnitOfWork.Object);

            var patient = _patientService.GetPatientByIdAsync(1).GetAwaiter().GetResult();

            var patientFromTestList = _patients.Find(a => a.Id == 1);

            Assert.AreEqual(patientFromTestList.Id, patient.Id);
            Assert.AreEqual(patientFromTestList.Name, patient.Name);
        }

        [Test]
        public void GetPatientById_PatientIdNotFound_ShouldReturnNull()
        {
            _mockPatientRepository.Setup(p => p.GetByIdAsync(It.IsAny<int>()))
                                  .ReturnsAsync(new Func<object[], PatientEntity>(id => _patients.Find(p => p.Id == int.Parse(id[0].ToString()))));
            _mockUnitOfWork.Setup(s => s.Repository<PatientEntity>()).Returns(_mockPatientRepository.Object);
            _patientService = new PatientService(_mockUnitOfWork.Object);

            var patient = _patientService.GetPatientByIdAsync(0).GetAwaiter().GetResult();

            Assert.Null(patient);
        }


        //[Test]
        //public void GetAllPatients_IfExceptionOccurs_ShouldThrowException()
        //{
        //    _mockPatientRepository.Setup(p => p.GetAllNoTracking).Throws(new Exception("An internal error occured"));
        //    _mockUnitOfWork.Setup(s => s.Repository<PatientEntity>()).Returns(_mockPatientRepository.Object);
        //    _patientService = new PatientService(_mockUnitOfWork.Object);

        //    Assert.Throws<Exception>(() => _patientService.GetAllPatients());
        //}
        #endregion

        #region tear down after every test 

        /// <summary>
        /// Tears down each test data
        /// </summary>
        [TearDown]
        public void DisposeTest()
        {
            _patientService = null;
            _mockPatientRepository = null;
            _patients = null;
        }
        #endregion

        #region One time tear down after all tests

        [OneTimeTearDown]
        public void DisposeAllObjects()
        {
            _patients = null;
            _mockUnitOfWork = null;
        }
        #endregion

        #region Private members methods

        private IRepository<PatientEntity> SetUpPatientRepository()
        {
            var mockRepo = new Mock<IRepository<PatientEntity>>();

            mockRepo.Setup(p => p.GetAllNoTracking).Returns(_patients.AsQueryable());

            mockRepo.Setup(p => p.GetByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(new Func<int, PatientEntity>(id => _patients.Find(p => p.Id == id)));

            return mockRepo.Object;
        }

        #endregion

        #region Helpers and Data Initializer
        private List<PatientEntity> SetUpPatients()
        {
            return new List<PatientEntity>()
            {
                new PatientEntity
                {
                    Id=1,
                    Name="Henrik Karlsson"
                },
                new PatientEntity
                {
                    Id=2,
                    Name="Erik Henriksson"
                },
            };
        }

        public class PatientEntityComparer : IComparer, IComparer<PatientEntity>
        {
            public int Compare(object expected, object actual)
            {
                var lhs = expected as PatientEntity;
                var rhs = actual as PatientEntity;
                if (lhs == null || rhs == null) throw new InvalidOperationException();
                return Compare(lhs, rhs);
            }

            public int Compare(PatientEntity expected, PatientEntity actual)
            {
                int temp;
                return (temp = expected.Id.CompareTo(actual.Id)) != 0 ? temp : expected.Name.CompareTo(actual.Name);
            }
        }

        #endregion
    }
}