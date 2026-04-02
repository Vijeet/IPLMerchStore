// Quick debug test to understand API errors
using System.Net;
using IplMerchStore.Domain.Entities;
using IplMerchStore.Domain.Enums;
using IplMerchStore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var factory = new WebApplicationFactory<Program>();
var client = factory.CreateClient();

// Make request 1: Basic
Console.WriteLine("Test 1: Basic GET /api/products");
var response = await client.GetAsync("/api/products");
Console.WriteLine($"Status: {response.StatusCode}");
var content = await response.Content.ReadAsStringAsync();
Console.WriteLine($"Response (first 500 chars): {content[..Math.Min(500, content.Length)]}");
Console.WriteLine();

// Make request 2: With activeOnly filter
Console.WriteLine("Test 2: GET /api/products?activeOnly=true");
response = await client.GetAsync("/api/products?activeOnly=true");
Console.WriteLine($"Status: {response.StatusCode}");
content = await response.Content.ReadAsStringAsync();
Console.WriteLine($"Response (first 500 chars): {content[..Math.Min(500, content.Length)]}");
