using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AidboxDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Create fhir client https://<your box name>.aidbox.app
                var client = new FhirClient("https://tutorial.aidbox.app");

                // Currently Aidbox doesn't support XML format
                // We need explicitly set it to JSON
                client.PreferredFormat = ResourceFormat.Json;

                // Subscribe OnBeforeRequest in order to set authorization header
                client.OnBeforeRequest += Client_OnBeforeRequest;

                // Create sample resource
                var patient = new Patient
                {
                    Name = new List<HumanName>
                    {
                        new HumanName
                        {
                            Given = new List<string> { "John" },
                            Family = "Doe"
                        }
                    }
                };

                // Create sample resource in box
                var newPatient = client.Create(patient);

                // Show identifier of newly created patient
                Console.WriteLine("New patient: {0}", newPatient.Id);

                // Read newly created patient from server
                var readPatient = client.Read<Patient>("Patient/" + newPatient.Id);

                // Show it id, given name and family
                Console.WriteLine("Read patient: {0} {1} {2}", readPatient.Id, readPatient.Name[0].Given.First(), readPatient.Name[0].Family);
            }
            catch (Exception e)
            {
                // Show error is something go wrong
                Console.WriteLine("Something go wrong: {0}", e);
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static void Client_OnBeforeRequest(object sender, BeforeRequestEventArgs e)
        {
            // Use basic authorization
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("aidboxdemo:123456"));

            // Add authorization header to request
            e.RawRequest.Headers.Add("Authorization", "Basic " + credentials);
        }
    }
}
