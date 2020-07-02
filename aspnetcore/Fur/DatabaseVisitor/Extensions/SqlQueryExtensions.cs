﻿using Fur.DatabaseVisitor.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Fur.DatabaseVisitor.Extensions
{
    /// <summary>
    /// 原始Sql执行拓展
    /// </summary>
    public static class SqlQueryExtensions
    {

        public static DataTable SqlQuery(this DatabaseFacade databaseFacade, string sql, params object[] parameters)
        {
            var (dbConnection, dbCommand) = databaseFacade.WrapperDbConnectionAndCommand(sql, parameters);
            var dbDataReader = dbCommand.ExecuteReader();
            var dataTable = new DataTable();
            dataTable.Load(dbDataReader);
            dbDataReader.Close();
            dbConnection.Close();
            dbCommand.Parameters.Clear();
            return dataTable;
        }

        public static IEnumerable<T> SqlQuery<T>(this DatabaseFacade databaseFacade, string sql, params object[] parameters)
        {
            return SqlQuery(databaseFacade, sql, parameters).ToEnumerable<T>();
        }

        public static async Task<DataTable> SqlQueryAsync(this DatabaseFacade databaseFacade, string sql, params object[] parameters)
        {
            var (dbConnection, dbCommand) = await databaseFacade.WrapperDbConnectionAndCommandAsync(sql, parameters);
            var dbDataReader = await dbCommand.ExecuteReaderAsync();
            var dataTable = new DataTable();
            dataTable.Load(dbDataReader);
            await dbDataReader.CloseAsync();
            await dbConnection.CloseAsync();
            dbCommand.Parameters.Clear();
            return dataTable;
        }

        public static Task<IEnumerable<T>> SqlQueryAsync<T>(this DatabaseFacade databaseFacade, string sql, params object[] parameters)
        {
            return SqlQueryAsync(databaseFacade, sql, parameters).ToEnumerableAsync<T>();
        }

        #region 包装数据库链接和执行命令对象 -/* private static (DbConnection, DbCommand) WrapperDbConnectionAndCommand(this DatabaseFacade databaseFacade, string sql, params object[] parameters)
        /// <summary>
        /// 包装数据库链接和执行命令对象
        /// </summary>
        /// <param name="databaseFacade"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static (DbConnection, DbCommand) WrapperDbConnectionAndCommand(this DatabaseFacade databaseFacade, string sql, params object[] parameters)
        {
            var dbConnection = new ProfiledDbConnection(databaseFacade.GetDbConnection(), MiniProfiler.Current);
            if (dbConnection.State == ConnectionState.Closed)
            {
                dbConnection.Open();
            }

            DbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = sql;
            Helper.FixedAndCombineSqlParameters(ref dbCommand, parameters);

            return (dbConnection, dbCommand);
        }
        #endregion

        #region 包装数据库链接和执行命令对象 -/* private async static Task<(DbConnection, DbCommand)> WrapperDbConnectionAndCommandAsync(this DatabaseFacade databaseFacade, string sql, params object[] parameters)
        /// <summary>
        /// 包装数据库链接和执行命令对象
        /// </summary>
        /// <param name="databaseFacade"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private async static Task<(DbConnection, DbCommand)> WrapperDbConnectionAndCommandAsync(this DatabaseFacade databaseFacade, string sql, params object[] parameters)
        {
            var dbConnection = new ProfiledDbConnection(databaseFacade.GetDbConnection(), MiniProfiler.Current);
            if (dbConnection.State == ConnectionState.Closed)
            {
                await dbConnection.OpenAsync();
            }

            DbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = sql;
            Helper.FixedAndCombineSqlParameters(ref dbCommand, parameters);

            return (dbConnection, dbCommand);
        }
        #endregion

    }
}