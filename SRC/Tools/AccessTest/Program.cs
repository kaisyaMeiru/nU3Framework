using System;
using System.Net.Http;
using System.Threading.Tasks;
using nU3.Connectivity.Implementations;
using nU3.Core.Interfaces;

namespace DataAccessTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting Data Access Layer Verification (AccessTest)...");

            try
            {
                // 1. Setup
                var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };
                IDataService dataService = new ServiceAdapter(httpClient);
                Console.WriteLine("[Setup] ServiceAdapter initialized with base URL: http://localhost:5000");

                // Initialize SampleBizLogic with manual DI
                var bizLogic = new SampleBizLogic(dataService);
                Console.WriteLine("[Setup] SampleBizLogic initialized.");

                // 2. Test ExecuteAsync (Run a dummy save via BizLogic)
                Console.WriteLine("\n[Test 1] Testing BizLogic.SaveItemAsync...");
                
                try
                {
                    await bizLogic.SaveItemAsync("Test Item", 123);
                    Console.WriteLine("[Success] SaveItemAsync completed (Unexpected - backend should not exist)");
                }
                catch (HttpRequestException ex)
                {
                     Console.WriteLine($"[Expected Failure] SaveItemAsync failed as expected (No Backend): {ex.Message}");
                }
                catch (Exception ex)
                {
                     Console.WriteLine($"[Unexpected Error] SaveItemAsync failed: {ex.GetType().Name} - {ex.Message}");
                }

                // 3. Test QueryAsync via BizLogic
                 Console.WriteLine("\n[Test 2] Testing BizLogic.GetItemsAsync...");
                try
                {
                    var list = await bizLogic.GetItemsAsync();
                    Console.WriteLine($"[Success] GetItemsAsync completed. Count: {list.Count}");
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"[Expected Failure] GetItemsAsync failed as expected (No Backend): {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Unexpected Error] GetItemsAsync failed: {ex.GetType().Name} - {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Critical Error] Test aborted: {ex.ToString()}");
            }
            
            Console.WriteLine("\nTest Finished.");
        }
    }
}
