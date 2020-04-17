/////////////////////////////////////////////////////////////
// CryptoHelper
//
// Cryptography的简易封装类，提供DES字符串加密解密和MD5校验码计算。
//
// API:
//
// Encrypt(string text, string key = null) -- 使用密钥(key)来加密字符串(text)，返回密文字符串
// Decrypt(string text, string key = null) -- 使用密钥(key)来解密字符串(text)，如果密钥正确则返回明文字符串，错误则返回null
// MD5(string text) -- 计算字符串(text)的MD5校验码
//
// Abin
// 2018-6-02
/////////////////////////////////////////////////////////////

using System;
using System.Text;
using System.Security.Cryptography;

namespace kittingStatus.jabil.web.DAL
{
	class CryptoHelper
	{
		/// <summary> 
		/// DES加密，使用密钥(key)来加密字符串(text)
		/// </summary> 
		/// <param name="text">明文字符串</param> 
		/// <param name="key">密钥</param> 
		/// <returns>密文字符串</returns> 
		public static string Encrypt(string text, string key = null)
		{
			DESCryptoServiceProvider des = new DESCryptoServiceProvider();
			des.Key = Encoding.UTF8.GetBytes(NormalizeKey(key));
			des.Mode = CipherMode.ECB;
			des.Padding = PaddingMode.PKCS7;

			byte[] data = Encoding.UTF8.GetBytes(text);
			ICryptoTransform transform = des.CreateEncryptor();
			byte[] result = transform.TransformFinalBlock(data, 0, data.Length);
			return Convert.ToBase64String(result, 0, result.Length);
		}

		/// <summary> 
		/// DES解密，使用密钥(key)来解密字符串(text)
		/// </summary> 
		/// <param name="text">密文字符串</param> 
		/// <param name="key">密钥</param> 
		/// <returns>如果密钥正确则返回明文字符串，密钥错误则返回null</returns> 
		public static string Decrypt(string text, string key = null)
		{
			try
			{
				DESCryptoServiceProvider des = new DESCryptoServiceProvider();
				des.Key = Encoding.UTF8.GetBytes(NormalizeKey(key));
				des.Mode = CipherMode.ECB;
				des.Padding = PaddingMode.PKCS7;

				byte[] data = Convert.FromBase64String(text);
				ICryptoTransform transform = des.CreateDecryptor();
				byte[] result = transform.TransformFinalBlock(data, 0, data.Length);
				return Encoding.UTF8.GetString(result);
			}
			catch
			{
				return null; // 解密失败
			}
		}

		/// <summary> 
		/// MD5计算
		/// </summary> 
		/// <param name="text">字符串</param> 
		/// <returns>MD5校验码（32位字符串）</returns> 
		public static string MD5(string text)
		{
			byte[] data = Encoding.UTF8.GetBytes(text);
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] output = md5.ComputeHash(data);
			return BitConverter.ToString(output).Replace("-", "");			
		}

		/// <summary> 
		/// 密钥预处理
		/// </summary> 
		/// <param name="key">密钥</param> 
		/// <returns>处理过的密钥，长度已确保为8个字符</returns> 
		private static string NormalizeKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return "JABIL.ME"; // 默认密钥
			}

			// DES加解密的密钥长度必须为8，如果用户输入的密钥长度不是8，我们先计算其MD5，再从结果中的特定位置提取8个字符作为替代密钥
			if (key.Length != 8)
			{
				string md5 = MD5(key);
				key = md5.Substring(0, 2) + md5.Substring(8, 2) + md5.Substring(16, 2) + md5.Substring(24, 2);				
			}

			return key;
		}		
	}
}
