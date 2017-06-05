using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using Newtonsoft.Json;
using webApiClient.Models;

namespace webApiClient
{
    class Program
    {
        static HttpClient client = new HttpClient();

        static string uri = "http://vautointerview.azurewebsites.net/";

        static string datasetId = "";        

        static string datasetIdPath = "api/datasetId";
        

        static async Task<Response> SubmitAnswerTaskAsync(Answer answer)
        {


            Response r = null;

            Console.WriteLine("**** About to submit the answer ****");

            HttpResponseMessage response = await client.PostAsJsonAsync("api/" + datasetId + "/answer", answer);
            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            { 
                string json = await response.Content.ReadAsStringAsync();

                r = JsonConvert.DeserializeObject<Response>(json);
            }


            


            // return URI of the created resource.
            return r;
        }

        static async Task<RootObject> getDatasetIdAsync()
        {
            RootObject dsRootObject = null;

            string path = datasetIdPath;

            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                dsRootObject = await response.Content.ReadAsAsync<RootObject>();
            }
            return dsRootObject;

        }

        static async Task<VehicleIdList> getVehicleListIdsAsync()
        {
            VehicleIdList vehicleList = null;

            string path = "api/" + datasetId + "/vehicles";

            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                vehicleList = await response.Content.ReadAsAsync<VehicleIdList>();
            }
            return vehicleList;

        }

        static async Task<Vehicle> getVehicleDetailAsync(int vehicleId)
        {
            Vehicle vehicle = null;

            string path = "api/" + datasetId + "/vehicles/" + vehicleId;

            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                vehicle = await response.Content.ReadAsAsync<Vehicle>();
            }
            return vehicle;

        }

        static async Task<Dealer> getDealerDetailAsync(int dealerId)
        {
            Dealer dealer = null;

            string path = "api/" + datasetId + "/dealers/" + dealerId;

            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                dealer = await response.Content.ReadAsAsync<Dealer>();
            }
            return dealer;

        }

        

        static void Main()
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri(uri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                
                /*
                * 1) Step one get DataSetId
                * 2) Step two get a list an array of all vehicles
                * 3) get vehicle info 1 one 1 - you will get a dealer id along with it 
                * 4) get dealer info and may be store in a hashtable or some kind of data structure
                * 5) reconstruct the answer - to post to the answer endpoint 
                * 
                */
                
                //1) Get the dataset Id
                RootObject rootObject = null;
                rootObject = await getDatasetIdAsync();
                datasetId = rootObject.datasetId;
                Console.WriteLine("Dataset ID : " + rootObject.datasetId);


                //2) Get Vehicle List Ids 
                VehicleIdList vehicleIdList = null;
                vehicleIdList = await getVehicleListIdsAsync();
                Console.WriteLine("Vehicle Ids List :" + string.Join(", ", vehicleIdList.vehicleIds.ToArray()));

                //3) Get Vehicle Info objects 

                List<Vehicle> vehicles = new List<Vehicle>();

                foreach (int vehicleId in vehicleIdList.vehicleIds)
                {
                    Console.WriteLine("Processing " + vehicleId);
                    Vehicle vehicle = await getVehicleDetailAsync(vehicleId);
                    vehicles.Add(vehicle);
                    Console.WriteLine("Dealer Id :" + vehicle.dealerId);
                }

                //4) get Dealer Details 

                List<Dealer> dealers = new List<Dealer>();

                for (int i = 0; i < vehicles.Count; i++)
                {
                    Console.WriteLine("Processing Dealer" + vehicles[i].dealerId);
                    Dealer dealer = await getDealerDetailAsync(vehicles[i].dealerId);  
                    dealers.Add(dealer);
                    Console.WriteLine("Dealer Name :" + dealer.name);
                   
                }

                //5) time to submit answer 
                // i am using more linq library to get distinct dealers - https://morelinq.github.io/
                // i am just trying to get a distinct list of all dealers - since list received got many repitive ones
                var uniqueDealers = dealers.DistinctBy(d => d.dealerId);

                //building the final answer object - to post to answer
                Answer answer = new Answer();
                
                foreach (var item in uniqueDealers)
                {
                    
                    Console.WriteLine("Processing Dealer : " + item.dealerId);
                    Dealer tempDealer = new Dealer();
                    tempDealer.dealerId = item.dealerId;
                    tempDealer.name = item.name;
                    //basically I am getting all vehicles associated by one dealer id
                    tempDealer.vehicles = vehicles.Where(d => d.dealerId == item.dealerId).ToList();
                    //also here - i am setting dealerId = 0 for all vehicles - so it won't be included in my json Post 
                    tempDealer.vehicles.ToList().ForEach(x => { x.dealerId = 0; });
                    answer.dealers.Add(tempDealer);                    
                }


                //call to post to answer - and receive the response object
                Response r = await SubmitAnswerTaskAsync(answer);

                if (r != null)
                {
                    Console.WriteLine("success : " + r.success);
                    Console.WriteLine("message : " + r.message);
                    Console.WriteLine("totalMilliseconds : " + r.totalMilliseconds);

                }

                




                Console.ReadLine();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

    }
}
