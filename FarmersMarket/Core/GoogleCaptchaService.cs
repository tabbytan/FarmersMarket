﻿using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace FarmersMarket.Core
{
	public class GoogleCaptchaService


	{
		private readonly IOptionsMonitor<GoogleCaptchaConfig> _config;

		public GoogleCaptchaService( IOptionsMonitor<GoogleCaptchaConfig> config)
		{
			_config = config;
		}
		public async Task<bool> VerifyToken(string token)
		{
			try
			{
				var url = $"https://www.google.com/recaptcha/api/siteverify?secret={_config.CurrentValue.SecretKey}&response={token}";

				using (var client = new HttpClient())
				{
					var httpResult = await client.GetAsync(url);
					if(httpResult.StatusCode != System.Net.HttpStatusCode.OK)
					{
						return false;
					}
					var responseString = await httpResult.Content.ReadAsStringAsync();
					var googleresult = JsonConvert.DeserializeObject<GoogleCaptchaResponse>(responseString);
					return googleresult.success && googleresult.score >= 0.5;
				}
			}
			catch (Exception e)
			{
				return false;
			}
		}
	}
}
