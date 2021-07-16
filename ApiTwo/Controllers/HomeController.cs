﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityModel;

namespace ApiTwo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        [Route("/")]
        public async Task<IActionResult> Index()
        {
            var serverClient = httpClientFactory.CreateClient();
            var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync("https://localhost:44395/");
            var token = await serverClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "client_id",
                ClientSecret = "client_secret",

                Scope = "ApiOne"
            });
            var apiClient = httpClientFactory.CreateClient();
            apiClient.SetBearerToken(token.AccessToken);
            var response = await apiClient.GetAsync("https://localhost:44368/secret");
            var content = await response.Content.ReadAsStringAsync();
            return Ok(new {
                access_token = token.AccessToken,
                message = content
            });
        }
    }
}
