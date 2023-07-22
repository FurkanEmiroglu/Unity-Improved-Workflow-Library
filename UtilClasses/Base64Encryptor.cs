namespace IW.UtilClasses
{
	public static class Base64Encryptor
	{
		/// <summary>
		/// Encrypts data using base64
		/// </summary>
		/// <param name="data"></param>
		/// <returns>Encrypted data</returns>
		public static string Encrypt(string data)
		{
			var bytes = System.Text.Encoding.UTF8.GetBytes(data);
			return System.Convert.ToBase64String(bytes);
		}
		
		/// <summary>
		/// Decrypts base64 data
		/// </summary>
		/// <param name="data"></param>
		/// <returns>Decrypted data as string</returns>
		public static string Decrypt(string data)
		{
			var bytes = System.Convert.FromBase64String(data);
			return System.Text.Encoding.UTF8.GetString(bytes);
		}
		
		/// <summary>
		/// Encrypts data using base64
		/// </summary>
		/// <param name="data"></param>
		/// <returns>Encrypted data</returns>
		public static string ToEncrypted(this string data) => Encrypt(data);
		
		/// <summary>
		/// Decrypts base64 data
		/// </summary>
		/// <param name="data"></param>
		/// <returns>Decrypted data as string</returns>
		public static string FromEncrypted(this string data) => Decrypt(data);
	}

}