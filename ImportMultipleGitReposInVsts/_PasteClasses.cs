using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportMultipleGitReposInVsts
{











    public class CreateImportRequestResponse
    {
        public int importRequestId { get; set; }
        public CreateImportRequestResponse_Repository repository { get; set; }
        public CreateImportRequestResponse_Parameters parameters { get; set; }
        public string status { get; set; }
        public CreateImportRequestResponse_Detailedstatus detailedStatus { get; set; }
        public string url { get; set; }
    }

    public class CreateImportRequestResponse_Repository
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public CreateImportRequestResponse_Project project { get; set; }
        public string remoteUrl { get; set; }
    }

    public class CreateImportRequestResponse_Project
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string state { get; set; }
        public int revision { get; set; }
        public string visibility { get; set; }
    }

    public class CreateImportRequestResponse_Parameters
    {
        public object tfvcSource { get; set; }
        public CreateImportRequestResponse_Gitsource gitSource { get; set; }
        public bool deleteServiceEndpointAfterImportIsDone { get; set; }
    }

    public class CreateImportRequestResponse_Gitsource
    {
        public string url { get; set; }
        public bool overwrite { get; set; }
    }

    public class CreateImportRequestResponse_Detailedstatus
    {
        public int currentStep { get; set; }
        public string[] allSteps { get; set; }
    }








    public class CreateImportRequestRequest
    {
        public CreateImportRequestRequest_Parameters parameters { get; set; }
    }

    public class CreateImportRequestRequest_Parameters
    {
        public CreateImportRequestRequest_Gitsource gitSource { get; set; }
        public string serviceEndpointId { get; set; }
        public bool deleteServiceEndpointAfterImportIsDone { get; set; }
    }

    public class CreateImportRequestRequest_Gitsource
    {
        public string url { get; set; }
    }













    public class CreateRepoResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public CreateRepoResponse_Project project { get; set; }
        public string remoteUrl { get; set; }
    }

    public class CreateRepoResponse_Project
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string state { get; set; }
        public int revision { get; set; }
        public string visibility { get; set; }
    }












    public class CreateRepoRequest
    {
        public string name { get; set; }
        public CreateRepoRequest_Project project { get; set; }
    }

    public class CreateRepoRequest_Project
    {
        public string id { get; set; }
    }
















    public class CreateServiceEndpointResponse
    {
        public CreateServiceEndpointResponse_Data data { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public CreateServiceEndpointResponse_Createdby createdBy { get; set; }
        public CreateServiceEndpointResponse_Authorization authorization { get; set; }
        public string groupScopeId { get; set; }
        public CreateServiceEndpointResponse_Administratorsgroup administratorsGroup { get; set; }
        public CreateServiceEndpointResponse_Readersgroup readersGroup { get; set; }
        public bool isReady { get; set; }
    }

    public class CreateServiceEndpointResponse_Data
    {
    }

    public class CreateServiceEndpointResponse_Createdby
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string uniqueName { get; set; }
    }

    public class CreateServiceEndpointResponse_Authorization
    {
        public CreateImportRequestRequest_Parameters parameters { get; set; }
        public string scheme { get; set; }
    }

    public class CreateServiceEndpointResponse_Parameters
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class CreateServiceEndpointResponse_Administratorsgroup
    {
        public string id { get; set; }
    }

    public class CreateServiceEndpointResponse_Readersgroup
    {
        public string id { get; set; }
    }

















    public class CreateServiceEndpointRequest
    {
        public string name { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public CreateServiceEndpointRequest_Authorization authorization { get; set; }
    }

    public class CreateServiceEndpointRequest_Authorization
    {
        public string scheme { get; set; }
        public CreateServiceEndpointRequest_Parameters parameters { get; set; }
    }

    public class CreateServiceEndpointRequest_Parameters
    {
        public string username { get; set; }
        public string password { get; set; }
    }
















    public class repositories
    {
        public repositories_Value[] value { get; set; }
        public int count { get; set; }
    }

    public class repositories_Value
    {
        public string id { get; set; }
        public string name { get; set; }
        public string magic_repo_name => $"{project.name}-{name}".Replace(" ", "-");
        public string url { get; set; }
        public repositories_Project project { get; set; }
        public string defaultBranch { get; set; }
        public string remoteUrl { get; set; }
    }

    public class repositories_Project
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string state { get; set; }
        public int revision { get; set; }
        public string visibility { get; set; }
    }








}
