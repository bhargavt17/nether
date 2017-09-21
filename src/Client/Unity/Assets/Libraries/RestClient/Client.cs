using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;


#if !NETFX_CORE || UNITY_ANDROID
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
#endif

namespace RestClient
{
	public class Client
	{
		public string Url { get; private set; }

		/// <summary>
		/// Creates a new REST Client
		/// </summary>
		public Client (string url, bool forceHttps = true)
		{
			if (forceHttps) {
                Url = HttpsUri (url);
			}
			Debug.Log ("App Url: " + DomainName(Url));

			// required for running in Windows and Android
			#if !NETFX_CORE || UNITY_ANDROID
			Debug.Log ("ServerCertificateValidation");
			ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
			#endif
		}

		public override string ToString ()
		{
			return this.Url;
		}

		/// <summary>
		/// Changes 'http' to be 'https' instead
		/// </summary>
		private static string HttpsUri (string appUrl)
		{
			return Regex.Replace (appUrl, "(?si)^http://", "https://").TrimEnd ('/');
		}

		private static string DomainName (string url) {
			var match = Regex.Match(url, @"^(https:\/\/|http:\/\/)(www\.)?([a-z0-9-_]+\.[a-z]+)", RegexOptions.IgnoreCase);
			if (match.Groups.Count == 4 && match.Groups[3].Value.Length > 0) {
				return match.Groups[3].Value;
			}
			return url;
		}

		#if !NETFX_CORE || UNITY_ANDROID
		private bool RemoteCertificateValidationCallback (System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
            // Check the certificate to see if it was issued from Azure
            Debug.Log("@ " + certificate.Subject);
			if (certificate.Subject.Contains (DomainName(Url))) {
				return true;
			} else {
				return false;
			}
		}
		#endif

	}
}
