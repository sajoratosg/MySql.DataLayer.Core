﻿using NUnit.Framework;
using MySql.DataLayer.Core.Repository;
using MySql.DataLayer.Core;
using MySql.DataLayer.Core.Connection;
using MySql.DataLayer.Core.Attributes.EntityConfig.Table;
using System.Collections.Generic;
using System;
using MySql.DataLayer.Core.Attributes.StoredProcedureConfig.StoredProcedure;
using MySql.DataLayer.Core.UnitTest.GivenRepository.StoredProcedures;
using System.Threading.Tasks;

namespace MySql.DataLayer.Core.UnitTest.GivenRepository.When_StoredProcedure_Is_Called
{
    public class And_StoredProcedure_Exec
    {
        IMySqlConnectionFactory _connectionFactory;
        Repository _repository;

        [SetUp]
        public void Setup()
        {
            if (Environment.GetEnvironmentVariable("MYSQL_HOST") == null)
            {
                Environment.SetEnvironmentVariable("MYSQL_HOST", "192.168.99.100");
            }
            Database.Create(out _connectionFactory);
            Database.CreateFooTable(_connectionFactory);
            Database.InsertFooTable(_connectionFactory, 100);
            Database.CreateFooStoredProcedureWithParameter(_connectionFactory);
            Database.CreateFooStoredProcedureWithoutParameter(_connectionFactory);
            Database.CreateFooStoredProcedureWithNullParameter(_connectionFactory);

            _repository = new Repository(_connectionFactory);
        }

        [Test]
        public void SP_With_Parameter_Should_Return_Success()
        {
            List<QueryParameter> queryParameters = new List<QueryParameter>();
            queryParameters.Add(new QueryParameter
            {
                ParameterName = "limitToSelect",
                ParameterValue = 50
            });

            List<FooStoredProcedureWithParameter> resultList =
                                           _repository.ExecuteStoredProcedureAsync<FooStoredProcedureWithParameter>(queryParameters.ToArray()).Result;

            Assert.IsTrue(resultList != null);
        }

        [Test]
        public void SP_Without_Parameter_Should_Return_Success()
        {
            List<FooStoredProcedureWithoutParameter> resultList =
                                           _repository.ExecuteStoredProcedureAsync<FooStoredProcedureWithoutParameter>().Result;

            Assert.IsTrue(resultList != null);
        }

        [Test]
        public void SP_With_Null_Parameter_Should_Return_Success()
        {
            List<QueryParameter> queryParameters = new List<QueryParameter>();
            queryParameters.Add(new QueryParameter
            {
                ParameterName = "idValue",
                ParameterValue = Guid.NewGuid()
            });
            queryParameters.Add(new QueryParameter
            {
                ParameterName = "descriptionValue",
                ParameterValue = null
            });

            var result = _repository.ExecuteStoredProcedureReturnAffectedRowsAsync<FooStoredProcedureWithNullParameter>(queryParameters.ToArray()).Result;

            Assert.IsTrue(result == 1);
        }

        [TearDown]
        public virtual void TearDown()
        {
            Database.Drop(_connectionFactory);
        }

    }
}
