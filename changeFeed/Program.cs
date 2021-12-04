using Microsoft.Azure.Cosmos;

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace changeFeed
{
    class Program
    {

        public static CosmosClient client { get; set; }
        static void Main(string[] args)
        {
          
            Task.Run(async () =>
            {
                client = new CosmosClient("");

                var database = client.GetDatabase("changefeed");


                var container1 = database.GetContainer("lease");
                var containerData = database.GetContainer("shopping");
                var cpf = containerData.GetChangeFeedProcessorBuilder<dynamic>("cfpLib-1", Processchange)
                            .WithLeaseContainer(container1)
                            .WithInstanceName("instance-1")
                            //.WithStartTime(DateTime.MinValue.ToUniversalTime())
                            .Build();

                await cpf.StartAsync();
                Console.WriteLine("Started changefeed processor --- press any key to stop");
                Console.ReadKey(true);

            }).Wait();


        }

        static async Task Processchange(IReadOnlyCollection<dynamic> changes, CancellationToken cancellationToken)
        {
            foreach (var item in changes)
            {
                Console.WriteLine($"Document changed {item.id}");
                Console.Write(item.ToString());
            }
        }
    }
}
