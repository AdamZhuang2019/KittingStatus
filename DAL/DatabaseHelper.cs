//////////////////////////////////////////////////////////
// DatabaseHelper
//
// 数据库访问抽象基类，派生类必须重载以下方法：
//
// DbConnection NewConnection() - 创建新DbConnection对象
// DbCommand NewCommand() - 创建新DbCommand对象
// DbParameter NewParameter() - 创建新DbParameter存储过程参数对象
// DbDataAdapter NewDataAdapter() -- 创建新DbDataAdapter对象
//
// Abin
// 2018-3-12
//////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Configuration;

namespace kittingStatus.jabil.web.DAL
{
    public abstract class DatabaseHelper
	{
		#region 主要成员变量
		protected DbConnection m_connection = null;
		#endregion

		#region 公开属性
		/// <summary> 
		/// 属性，get/set 获取或设置数据库连接字符串 
		/// </summary> 		
		public virtual string ConnectionString
		{
			get
			{
				return m_connection.ConnectionString;
			}

			set
			{				
				m_connection.ConnectionString = ValidateConnectString(value);
			}
		}		
		
		/// <summary> 
		/// 属性，get 检查数据库连接是否已打开 
		/// </summary> 	
		public virtual bool Opened
		{
			get
			{
				return m_connection.State == ConnectionState.Open;
			}
		}

		/// <summary> 
		/// 属性，数据库标签 
		/// </summary>
		public string Flag { get; protected set; } = null;
		#endregion

		#region 构造与析构
		/// <summary> 
		/// 构造函数，以连接字符串为参数
		/// </summary> 	
		public DatabaseHelper(string connectString = null, string flag = null)
		{
			Flag = flag;
			m_connection = NewConnection();
			ConnectionString = connectString;
		}		

		~DatabaseHelper()
		{
			Close();
		}
		#endregion

		#region 数据库打开与关闭
		/// <summary> 
		/// 打开数据库连接
		/// </summary> 
		public virtual void Open()
		{
			if (!Opened)
			{
				m_connection.Open();
			}
		}

		/// <summary> 
		/// 关闭数据库连接
		/// </summary> 
		public virtual void Close()
		{
			m_connection.Close();
		}
		#endregion

		#region 数据库命令执行
		/// <summary> 
		/// 执行SQL语句，返回影响的记录数 
		/// </summary> 
		/// <param name="sql">SQL语句</param> 
		/// <param name="parameters">存储过程参数</param> 
		/// <returns>影响的记录数</returns> 
		public virtual int Execute(string sql, ProcedureParameter[] parameters = null, bool procedure = false)
		{
			DbCommand command = PrepareCommand(sql, parameters, procedure);

			try
			{
				return command.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				throw e;
			}
			finally
			{
				CloseIfAutoOpened();
			}			
		}		

		/// <summary> 
		/// 执行多条SQL语句，实现数据库事务。 
		/// </summary> 
		/// <param name="sqlList">多条SQL语句</param> 
		public virtual void ExecuteTransaction(List<string> sqlList)
		{
			DbCommand command = PrepareCommand(null, null, false);
			DbTransaction transaction = m_connection.BeginTransaction();
			command.Transaction = transaction;

			try
			{
				for (int i = 0; i < sqlList.Count; i++)
				{
					string sql = sqlList[i];
					if (!string.IsNullOrWhiteSpace(sql))
					{
						command.CommandText = sql;
						command.ExecuteNonQuery();
					}
				}

				transaction.Commit();		
			}
			catch (Exception e)
			{
				if (Opened) // connection有可能此时已经被断开
				{
					transaction.Rollback();
				}
				
				throw e;
			}
			finally
			{
				CloseIfAutoOpened();
			}
		}
		#endregion

		#region 多行查询
		/// <summary> 
		/// 执行查询语句，返回DataTable对象
		/// </summary> 
		/// <param name="sql">查询语句</param>
		/// <param name="parameters">存储过程参数</param> 
		/// <returns>DataTable对象</returns> 
		public virtual DataTable QueryDataTable(string sql, ProcedureParameter[] parameters = null, bool procedure = false)
		{
			DbCommand command = PrepareCommand(sql, parameters, procedure);

			try
			{
				DbDataAdapter adapter = NewDataAdapter();
				adapter.SelectCommand = command;
				DataTable dt = new DataTable();
				adapter.Fill(dt);
				return dt;
			}
			catch (Exception e)
			{
				throw e;
			}
			finally
			{
				CloseIfAutoOpened();
			}			
		}
		#endregion

		#region 单行查询
		public virtual DataRow QueryDataRow(string sql, ProcedureParameter[] parameters = null, bool procedure = false)
		{
			DataTable dt = QueryDataTable(sql, parameters, procedure);
			DataRow dr = null;
			if (dt.Rows.Count > 0)
			{
				dr = dt.Rows[0];
			}
			return dr;
		}
		#endregion

		#region 单值查询
		/// <summary> 
		/// 执行查询语句，返回Object对象
		/// </summary> 
		/// <param name="sql">查询语句</param>
		/// <param name="parameters">存储过程参数</param> 
		/// <returns>Object对象</returns> 
		public virtual Object QueryObject(string sql, ProcedureParameter[] parameters = null, bool procedure = false)
		{
			DbCommand command = PrepareCommand(sql, parameters, procedure);

			try
			{
				object obj = command.ExecuteScalar();
				if ((Object.Equals(obj, null)) || (Object.Equals(obj, DBNull.Value)))
				{
					obj = null;
				}

				return obj;
			}
			catch (Exception e)
			{
				throw e;
			}
			finally
			{
				CloseIfAutoOpened();
			}			
		}		

		/// <summary> 
		/// 执行查询语句，返回int值
		/// </summary> 
		/// <param name="sql">查询语句</param>
		/// <param name="defaultVal">当查询失败时的默认返回值</param>
		/// <param name="parameters">存储过程参数</param> 
		/// <returns>int值</returns> 
		public int QueryInt(string sql, int defaultVal = 0, ProcedureParameter[] parameters = null, bool procedure = false)
		{
			Object obj = QueryObject(sql, parameters, procedure);
			if (obj == null)
			{
				return defaultVal;
			}

			try
			{
				return Convert.ToInt32(obj);
			}
			catch
			{
				return defaultVal;
			}
		}		

		/// <summary> 
		/// 执行查询语句，返回double值
		/// </summary> 
		/// <param name="sql">查询语句</param>
		/// <param name="defaultVal">当查询失败时的默认返回值</param>
		/// <param name="parameters">存储过程参数</param>		/// 
		/// <returns>double值</returns> 
		public double QueryDouble(string sql, double defaultVal = 0.0, ProcedureParameter[] parameters = null, bool procedure = false)
		{
			Object obj = QueryObject(sql, parameters, procedure);
			if (obj == null)
			{
				return defaultVal;
			}

			try
			{
				return Convert.ToDouble(obj);
			}
			catch
			{
				return defaultVal;
			}
		}		

		/// <summary> 
		/// 执行查询语句，返回decimal值
		/// </summary> 
		/// <param name="sql">查询语句</param>
		/// <param name="defaultVal">当查询失败时的默认返回值</param>
		/// <param name="parameters">存储过程参数</param> 
		/// <returns>decimal值</returns> 
		public decimal QueryDecimal(string sql, decimal defaultVal = 0, ProcedureParameter[] parameters = null, bool procedure = false)
		{
			Object obj = QueryObject(sql, parameters, procedure);
			if (obj == null)
			{
				return defaultVal;
			}

			try
			{
				return Convert.ToDecimal(obj);
			}
			catch
			{
				return defaultVal;
			}
		}		

		/// <summary> 
		/// 执行查询语句，返回字符串
		/// </summary> 
		/// <param name="sql">查询语句</param>
		/// <param name="defaultVal">当查询失败时的默认返回值</param>
		/// <param name="parameters">存储过程参数</param> 
		/// <returns>字符串</returns>
		public string QueryString(string sql, string defaultVal = "", ProcedureParameter[] parameters = null, bool procedure = false)
		{
			Object obj = QueryObject(sql, parameters, procedure);
			if (obj == null)
			{
				return defaultVal;
			}

			try
			{
				return Convert.ToString(obj);
			}
			catch
			{
				return defaultVal;
			}
		}		

		/// <summary> 
		/// 执行查询语句，返回DateTime对象
		/// </summary> 
		/// <param name="sql">查询语句</param>
		/// <param name="parameters">存储过程参数</param> 
		/// <returns>DateTime对象</returns>
		public DateTime QueryDateTime(string sql, ProcedureParameter[] parameters = null, bool procedure = false)
		{
			Object obj = QueryObject(sql, parameters, procedure);
			if (obj == null)
			{
				return DateTime.MinValue;
			}

			try
			{
				return Convert.ToDateTime(obj);
			}
			catch
			{
				return DateTime.MinValue;
			}
		}		

		/// <summary> 
		/// 执行查询语句，返回格式化的日期字符串，默认格式"yyyy-MM-dd"
		/// </summary> 
		/// <param name="sql">查询语句</param>
		/// <param name="format">日期格式</param>
		/// <param name="parameters">存储过程参数</param> 
		/// <returns>格式化的日期字符串</returns>
		public string QueryDateString(string sql, string format = "yyyy-MM-dd", ProcedureParameter[] parameters = null, bool procedure = false)
		{
			return QueryDateTimeString(sql, format, parameters, procedure);
		}		

		/// <summary> 
		/// 执行查询语句，返回格式化的日期时间字符串，默认格式"yyyy-MM-dd HH:mm:ss"
		/// </summary> 
		/// <param name="sql">查询语句</param>
		/// <param name="format">日期时间格式</param>
		/// <param name="parameters">存储过程参数</param> 
		/// <returns>格式化的日期时间字符串</returns>
		public string QueryDateTimeString(string sql, string format = "yyyy-MM-dd HH:mm:ss", ProcedureParameter[] parameters = null, bool procedure = false)
		{
			DateTime dt = QueryDateTime(sql, parameters, procedure);
			if (dt > DateTime.MinValue)
			{
				return dt.ToString(format);
			}
			return null;
		}
		#endregion

		#region 静态方法
		/// <summary> 
		/// 确保字符串不会引发数据库错误
		/// </summary> 
		/// <param name="text">原字符串</param>		
		/// <returns>安全字符串</returns>
		public static string SafeString(string text)
		{
			if (text != null)
			{
				text = text.Replace("'", "''");
			}
			return text;
		}

		/// <summary> 
		/// SQL注入检测
		/// </summary> 
		/// <param name="text">原字符串</param>		
		/// <returns>包含的不安全关键词， 如果返回null表示原字符串安全</returns>
		public static readonly string[] DANGEROUS_KEYWORDS = { "*", "%", ",", ";", "--", "and ", "or ", "exec ", "select ", "insert ", "delete ", "update ", "truncate ", "drop ", "create ", "alter " };
		public static string ContainDangerousKeywords(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}

			text = text.Replace("\r", "");
			text = text.Replace("\n", " ");
			text = text.Replace("\t", " ");
			text = text.ToLower();

			foreach (string keyword in DANGEROUS_KEYWORDS)
			{
				if (text.IndexOf(keyword) != -1)
				{
					return keyword;
				}
			}

			return null;
		}
		#endregion

		#region 抽象成员
		protected abstract DbConnection NewConnection();
		protected abstract DbCommand NewCommand();
		protected abstract DbParameter NewParameter();
		protected abstract DbDataAdapter NewDataAdapter();
		#endregion

		#region 辅助方法
		/// <summary> 
		/// 智能判断数据库连接字符串
		/// </summary> 
		/// <param name="connectionString">字符串</param>		
		/// <returns>数据库连接字符串</returns>
		protected virtual string ValidateConnectString(string connectionString)
		{
			if (string.IsNullOrWhiteSpace(connectionString))
			{
				return null;
			}

			// 不符合数据库连接字符串格式特征，尝试查找应用程序配置文件（web.config或app.config）中的"ConnectionStrings"相关字段
			if (connectionString.Length < 31)
			{
				connectionString = ConfigurationManager.ConnectionStrings[connectionString].ConnectionString;
				if (connectionString == null)
				{
					return null;
				}
			}			

			// 检查是否为DES加密字符串（使用默认密钥）
			string test = CryptoHelper.Decrypt(connectionString);
			if (test != null)
			{
				return test;
			}

			return connectionString;
		}

		/// <summary> 
		/// 创建并初始化数据库命令对象
		/// </summary> 
		/// <param name="sql">SQL语句</param>	
		/// <param name="parameters">存储过程参数</param> 
		/// <returns>DbCommand命令对象</returns>
		protected virtual DbCommand PrepareCommand(string sql, ProcedureParameter[] parameters, bool procedure)
		{
			TrackAutoOpened(); // 首先保存auto-opened状态
			Open();

			DbCommand command = NewCommand();
			if (!string.IsNullOrWhiteSpace(sql))
			{
				command.CommandText = sql;
			}

			command.Connection = m_connection;

			if (parameters == null)
			{
				// 一般调用
				command.CommandType = CommandType.Text;
			}
			else
			{
				// 带参数调用，可能是存储过程，也可能是带参sql
				command.CommandType = procedure ? CommandType.StoredProcedure : CommandType.Text;

				foreach (ProcedureParameter param in parameters)
				{
					// 生成DbParameter对象并加入命令参数表
					DbParameter dbp = NewParameter();
					dbp.ParameterName = param.Name;										
					dbp.Direction = param.Direction;

					if (param.Type != null)
					{
						dbp.DbType = (DbType)param.Type;
					}

					if (param.Size > 0)
					{
						dbp.Size = param.Size;
					}

					if (param.Value == null && (param.Direction == ParameterDirection.Input || param.Direction == ParameterDirection.InputOutput))
					{
						dbp.Value = DBNull.Value;
					}
					else
					{
						dbp.Value = param.Value;
					}

					command.Parameters.Add(dbp);
				}
			}

			return command;
		}
		#endregion

		#region 私有成员
		// 用于追踪数据库连接是否由当前call打开的，如是，则当前call返回前必须关闭连接
		private bool m_autoOpened = false;

		private void TrackAutoOpened()
		{
			m_autoOpened = !Opened;
		}

		private void CloseIfAutoOpened()
		{
			if (m_autoOpened)
			{
				m_autoOpened = false;
				Close();
			}
		}
		#endregion
	}

	#region ProcedureParameter
	//////////////////////////////////////////////////////////
	// ProcedureParameter
	//
	// 数据库存储过程参数
	// DbParameter类的重载太过繁琐，所以另外定义这个简单的类。
	//////////////////////////////////////////////////////////

	public class ProcedureParameter
	{
		public String Name { get; set; }
		public Object Type { get; set; }
		public int Size { get; set; }
		public ParameterDirection Direction { get; set; }
		public Object Value { get; set; }

		/// <summary> 
		/// 构造函数
		/// </summary> 
		/// <param name="name">参数名称</param>
		/// <param name="type">参数类型，一般为派生类自定义的数据库枚举类型</param>
		/// <param name="size">参数大小</param>
		/// <param name="value">参数的值</param>
		/// <param name="direction">输入/输出方向</param>
		public ProcedureParameter(String name, Object value, Object type = null, int size = 0, ParameterDirection direction = ParameterDirection.Input)
		{
			Name = name;
			Type = type;
			Size = size;
			Value = value;
			Direction = direction;
		}
	}
	#endregion
}