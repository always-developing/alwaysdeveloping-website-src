---
title: "HashiCorp Vault with C#"
lead: "A step by step guide to integrating into HashiCorp vault with C#"
Published: "08/10/2022 01:00:00+0200"
slug: "2022-08-hashicorp-vault"
draft: true
toc: true
categories:
    - Blog
tags:
    - c#
    - .net
    - blog
    - hashicorp
    - vault
    - beginner

---

## Introduction

The requirement was very simple (or so I thought) - create a locally running [HashiCorp Vault](https://www.vaultproject.io/) instance, setup a simple C# client to connect to the vault, and get an understanding how how the Vault operates. At almost every step of the process I ran into issue - which lead me to putting together this guide.

This guide will go through step by step:
- How to configure the Vault for local development
- Connect to the Vault using VaultSharp
- Connect to the Vault using the REST api

This is a long guide, but will go through each aspect step by step and by the end you should have an up and running local Vault instance, and be able to connect via the CLI, the C# _VaultSharp_ library as well as the HTTP API using C#.

---

### Why?

So why did I struggle at almost every step, and why did I feel this guide could help?

- `Documentation is inaccurate`: Sometimes the guide and sample code snippets are just inaccurate or don't work. Some manual correction is required
- `Documentation is incomplete`: In some cases it was difficult to track down comprehensive documentation on what I was trying to perform. If there was documentation, it sometimes didn't adequately  
- `VaultSharp has missing functionality`: This is not a criticism of the great work done by the contributors to this free library, but there is some functionality available via the Vault CLI, not available via _VaultSharp_ (if it is is available, I could not find it, or documentation of it anywhere)
- `Few C# REST api examples available`: This wasn't the biggest issue, but it did take a few attempts to get the C# HttpClient to successfully perform operations on the Vault. This was mostly down to having to translate the working _curl_ and _CLI_ examples into C# http requests.

---

## HashiCorp Vault

In short, [HashiCorp Vault](https://www.vaultproject.io/) (also referred to as the _Vault_ in the rest of this post) allows for _secure, store and tightly control access to tokens, passwords, certificates, encryption keys for protecting secrets and other sensitive data using a UI, CLI, or HTTP API_.

There are other key vaults available on the market (Azure Key Vault, AWS KMS, GCP KMS) - but Hashicorp Vault is the defacto standard.

In this post we'll explore accessing the Vault using all three methods - the UI, CLI and HTTP api.

---

### Prerequisites

The guide below was written on a machine running Windows 11, so the requirements mentioned below will differ if working on another OS.

- `HashiCorp Vault download`: The exe is [available here](https://www.vaultproject.io/downloads) and is a requirement for this guide. This executable serves two purposes:
    - Allows for a dev instance of the Vault server to be run
    - Serves as a CLI tool to connect to the Vault server instance

    Once downloaded, is is recommended (but not required) making the `Vault binary` available on the `PATH` (so it can be executed from any directory when working in the command prompt/Powershell). The steps to perform this are [available here](https://stackoverflow.com/questions/1618280/where-can-i-set-path-to-make-exe-on-windows).

- `Docker Desktop`: This is not a requirement, as the Vault server can be run from the above mentioned download. However the guide also covers running a _container instance_ of the Vault and if using this method then [Docker Desktop](https://www.docker.com/products/docker-desktop/) (or similar) is require to be installed.

---

### Vault Startup

The below guides will demonstrate how to get a Vault instance up and running - it will not go into detail on persisting the Vault information to a permanent store. This guide is primarily aimed at getting a Vault up and running for _development_ purposes.

---

#### Binary

This is by far the easiest method to get an instance of the Vault up and running. This is **not** the route I initially took, as I was interested in getting the _container version_ up and running - however I **highly** recommend this as a starting point if te goal is to just get a working, integrated sample up and running.

To stand a Vault Server instance, from either the _Command Prompt_ or _PowerShell_, execute the following:

``` powershell
vault server -dev
```

The output should look something like this:

``` terminal
WARNING! dev mode is enabled! In this mode, Vault runs entirely in-memory
and starts unsealed with a single unseal key. The root token is already
authenticated to the CLI, so you can immediately begin using Vault.

You may need to set the following environment variable:

PowerShell:
    $env:VAULT_ADDR="http://127.0.0.1:8200"
cmd.exe:
    set VAULT_ADDR=http://127.0.0.1:8200

The unseal key and root token are displayed below in case you want to
seal/unseal the Vault or re-authenticate.

Unseal Key: ANYzmKrmlp3eO4skaJFjmhKE2dEVLzbKSxecR+XGb0o=
Root Token: hvs.9kouFib30HrpNSxDWTCzmczk

Development mode should NOT be used in production installations!
```

The Vault instance should now be available by browsing to `http://127.0.0.1:8200`, and you should be able to login using the `Root Token` provided in the output.

Keep in mind, this instance does NOT use any persistent storage, and if the server is stopped (by closing the prompt, or pressing _Ctrl-C_) then any information saved to the Vault will be lost.

---

#### Docker

Trying to get a _Docker_ version of the Vault up and running proved trickier than I thought or expected it to be. All of the below attempts were executed with the Windows version of [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed, configured to use Linux containers.

- `Attempt 1:`

    The [Vault page on DockerHub](https://hub.docker.com/_/vault) gives the below command as an example for running the vault with a completely in-memory server:

    ``` terminal
    docker run --cap-add=IPC_LOCK -e 'VAULT_DEV_ROOT_TOKEN_ID=myroot' -e 'VAULT_DEV_LISTEN_ADDRESS=0.0.0.0:1234' vault
    ```

    While this command did successfully startup a Vault container instance, no ports were exposed, so the vault URL wasn't accessible (or if it was, I couldn't figure it out). At this point I probably should have explored how to expose the ports - but I did not, I was hoping to find something that _just worked_.

- `Attempt 2:`

    While reading through the various documentation, I had also came across this official [using HashiCorp Vault C# client with .NET Core Guide](hhttps://learn.hashicorp.com/tutorials/vault/dotnet-httpclient) which contained a link to a [Vault Guide repository](https://github.com/hashicorp/vault-guides). As part of the guide and repository is a [demo_setup.sh file](https://github.com/hashicorp/vault-guides/blob/master/secrets/dotnet-vault/demo_setup.sh). Exactly what I needed! (or so I thought)

    However, executing the `demo_setup.sh` script, resulted in ...nothing. No error messages, but also no positive responses from the prompt. Looking at the contents of the script, a [docker-compose](https://github.com/hashicorp/vault-guides/blob/master/secrets/dotnet-vault/docker-compose.yml) is invoked.

    I tried running the docker-compose independently which resulted in ...a timeout. The _docker-compose_ tries to startup and instance of `Vault` and `MS SQL Server` (for persistent storage) - however the it seems like the `MS SQL Server` image in the _docker-compose_ is no longer valid.

- `Successful Attempt:`

    For development purposes, I didn't need a persistent store, so the sample _docker-compose.yml_ was modified, removing the requirement for the MS SQL database.

    The result was this _docker-compose.yml_:

    ``` yml
    version: "3.3"
    services:
    vault:
        image: vault:1.6.1
        restart: always
        command: [ 'vault', 'server', '-dev', '-dev-listen-address=0.0.0.0:8200']
        environment:
        VAULT_DEV_ROOT_TOKEN_ID: "some-root-token"
        ports:
        - "8200:8200"
        cap_add:
        - IPC_LOCK
        networks:
        vpcbr:
            ipv4_address: 10.5.0.2

    networks:
    vpcbr:
        driver: bridge
        ipam:
        config:
            - subnet: 10.5.0.0/16
    ```

    Executing this with:

    ``` terminal
    docker-compose up -d
    ```

    We finally have a result!

    ``` terminal
    [+] Running 6/6
    - vault Pulled                              13.1s
    - 21c83c524219 Pull complete                5.2s
    - 2552eed26cd4 Pull complete                5.3s
    - b6096191328a Pull complete                7.2s
    - 63cf312915ed Pull complete                7.3s
    - ba2ada45d14d Pull complete                7.3s
    [+] Running 2/2
    - Network vaultpoc_vpcbr      Created       0.9s
    - Container vaultpoc-vault-1  Started       1.3s   
    ```

    Browsing to `http://localhost:8200` (the port specified in the docker-compose) - the Vault logon screen can be accessed! Using the default root token specified (`some-root-token` if using the above yml), allowed access into the Vault.

We now had a repeatable container based process which _just works!_

---

### Vault Configuration

Now that the Vault is up and running, the next step is to configure the Vault, as well as create and configure credentials for our application. At this point there is only one _root_ user configured, which has full access to all areas of the Vault. Using the root credentials might be fine for development purposes, but it is important to understand how the security in Vault works, how the future application will interact with Vault, and how permission are assigned.

Some of the steps below make use of the [`Vault Command Line Interface (CLI)`](https://www.vaultproject.io/downloads) tool, which should be downloaded. It consists of a single exe which is used to invoke commands against the Vault instance.

As part of using the CLI, it is also recommended to set an environment variables for the `vault url` - this is not required, but will save time later as the URL will not need to be specified for each CLI command ( specifying `-address="http://localhost:8200"` on every command):

To set the environment variable in **Powershell**, execute the following:

``` powershell
$env:VAULT_ADDR='http://0.0.0.0:8200'
```

OR 

Or in **Command Prompt**,  execute the following:

``` terminal
set VAULT_ADDR=http://0.0.0.0:8200
```

The steps below which use the CLI, will show the **PowerShell** command, but the _Command Prompt_ commands would be almost identical.

A note: the above will only set the environment variables for the lifetime of the shell session - if closed and reopened, the environment variable will no longer be set and the above commands will need to be run again.

---

#### Key-Value Engine

This guide focuses on using the Vault `key-value pair functionality` to store application secrets - there is however other functionality available, which if required, would need to be enabled as with the `key-value pair` functionality.

The first step to being able to store `key value pairs` is to enable the functionality. The following steps will essentially create a _path_ (folder) in the Vault, which will store specific type of information - in this case, a folder called `secrets` is being created, which will store `key-value pairs`. A _path_ can be thought of a _folder structure_ within the Vault in which secrets will be stored, with each folder able to have its own permissions. 

This step only needs to be performed once, unless specifically deciding that additional _root paths_ are required.

The easiest way to enable the _kv-engine_ (kv) is via the UI (although it can also be done via the CLI):
- Log into the Vault UI as the root
- By default, there should be a `secret` path already visible under the `Secrets` menu (the default screen once logged in), linked to a _kv engine_ (the description under the name _secret_ will specify _"v2 kv\_xxx"_)
    - If there is no `secret` engine listed, OR if you would like to create another path, click `Enable new engine +` on the top right-hand corner of the grid
    - Select `Generic -> KV` (first option) and click `Next`
    - Specify the `Path` - this will be the name under which all the key-value pairs will be stored. It can be anything, but should be something which makes sense to your usage (secrets, settings etc) and click `Enable Engine`

Step 1 done! - **secrets engine enabled!**

---

#### Secret

The next step is to add a test secret (key/value pair) to `secret` area (the _secret_ path, or if a different path was manually in the above step, which name was specified there)

For the next few steps, we'll switch to **PowerShell** - first a login needs to be performed:

```powershell 
vault login
```

Type in the `root token` once prompted, and you should have success (if the address environment variable was not configured, you may need to that as a parameter as well):

``` terminal
Key                  Value
---                  -----
token                some-root-token
token_accessor       D8XSBVu53WTDDktgjYGdOdDg
token_duration       ∞
token_renewable      false
token_policies       ["root"]
identity_policies    []
policies             ["root"]
```

Now to create the `key-value pair`:

``` powershell
vault kv put secret/firstapp secretkey=mypassword
```

This is instructing the CLI to:
- Create/update a secret at the path `secret/firstapp` (This will be the _folder_ and _subfolder_ where the secrets specific to this application will be stored)
- With a key of `secretkey`
- And a value of `mypassword`

Browsing to the Vault UI, there should now see a _path_ (folder) under `secret` called _firstapp_ which contains one secret, with the key _secretkey_.

This can also be verified using the CLI, by executing:

``` powershell
vault kv get secret/firstapp
```

The output:

```terminal
====== Data ======
Key          Value
---          -----
secretkey    mypassword
```

The standard used by this guide, is that each separate application will have its own path under `secret` (e.g. _secret/appname_), with each path with its own permissions. If configuring secrets for another application, then the above can be executed again, replacing "firstapp" with the new application's name.

Step 2 done! - **secret created successfully!**

---

#### AppRole

Next up is enabling the `approle` functionality - this will only need to be performed once on the Vault instance.

The `approle` auth method is designed to allow machines or applications to authenticate to the Vault using a _roleId and secretId_. This step effectively just turns on this functionality in the Vault.

To enable `approle` execute the following command in PowerShell:

``` powershell 
vault auth enable approle
```

The successful response:

``` terminal
Success! Enabled approle auth method at: approle/
```

If the `approle` auth method has already been enabled, the following will be the response:

``` terminal
Error enabling approle auth: Error making API request.

URL: POST http://0.0.0.0:8200/v1/sys/auth/approle
Code: 400. Errors:

* path is already in use at approle/
```

Step 3 done! - **Approle functionality enabled!**

---

#### Policy

Permission to a specific _path_ are controlled with a `policy`. The `policy` is configured, and then attached to one or more roles (roles are created in the next step), which allows an application authenticating with a specific _roleId and secretId_ to have specifically controlled access to one or more _paths_.

First create a `policy file` (this can be used as a template for future applications), named `app-policy.hcl` (although the name itself is not import):

``` yml
# Login with AppRole 
path "auth/approle/login" {
  capabilities =  [ "create", "read" ]
} 

# Read data from secret/firstapp
path "secret/data/firstapp" {
  capabilities =  [ "read" ]
}
```

This policy file allows:
- _create_ and _read_ permissions on the _auth/approle/login_ path. Effectively if an `approle` is assigned this policy, it can login
- _read_ permissions on the _secret/data/firstapp_ path. The user entity assigned this policy can `only` read the _key-value pairs_ `only` on that specific path

Now that we have a policy defined (in a file), it needs to be created in the Vault. Cchanging the file path to wherever the policy files is stored, execute:

```powershell
vault policy write firstapp-policy c:\app-policy.hcl
```

This creates a new policy called `firstapp-policy`, using the contents of the `app-policy.hcl` file.

Step 4 done! - **Policy create!**

---

#### Role (with policy)

Finally, its time to `create the role` (the application user), with the above defined policy assigned to it:

``` powershell
vault write auth/approle/role/firstapp-role policies="firstapp-policy"
```

This command creates a new role called `firstapp-role`, and assigns it the policy called `firstapp-policy`.

Even though the role is now created, we have no information about it (yet). What is the _role-Id_ (name) and _secret-Id_ (password)?

- Let's retrieve the _role-Id_ for the role:

    ``` powershell
    vault read auth/approle/role/firstapp-role/role-id
    ```

    And _role-Id_ should be returned:

    ``` terminal
    Key        Value
    ---        -----
    role_id    cb476d64-4614-1038-0dd5-b344700f3f3a
    ```

- And then the _secret-Id_ for the role (notice the addition of the -f parameter):

    ```powershell
    vault write -f auth/approle/role/firstapp-role/secret-id
    ```

    And _secret-Id_ should be returned:

    ``` terminal
    Key                   Value
    ---                   -----
    secret_id             5c1587dc-83b0-fe15-cc8e-196e6b777150
    secret_id_accessor    256b2ca7-5bce-3ce7-45b3-ceccc53fc6a3
    ```

Lastly, we can test the new credentials by using them to log in via the CLI:


``` powershell
vault write auth/approle/login role_id=cb476d64-4614-1038-0dd5-b344700f3f3a secret_id=5c1587dc-83b0-fe15-cc8e-196e6b777150
```

Receiving back a token along with other role information:

``` terminal
Key                     Value
---                     -----
token                   s.NWPZT9b6KuoakFfO9R9OZgPO
token_accessor          rl4mmRW0dDvvpevDlYjnkjKO
token_duration          768h
token_renewable         true
token_policies          ["default" "firstapp-policy"]
identity_policies       []
policies                ["default" "firstapp-policy"]
token_meta_role_name    firstapp-role
```

The _token_ can be used to log into the UI to confirm what the _application will be able to see_. If you try to browse to the _secret_ path, you may get a `Not Authorized` message - this is because the policy linked to the application role, does not have permission to see a list of the folders under _secret_.
If you browse directly to `http://localhost:8200/ui/vault/secrets/secret/show/firstapp` however, you should see the list of secrets on that specific path - which the policy **does allow**.

Step 5 done! - **Role successfully created!** (with roleId and secretId)


---

### Configuration wrap-up

We have now successfully configured the Vault instance with all the functionality we need for `basic integration`. We also looked at creating a `policy` and creating and linking an `app role` to policy.

Next up, we'll look at some C# code, using `VaultSharp` to integrate with the Vault.

---

## VaultSharp

[VaultSharp](https://github.com/rajanadar/VaultSharp) describes itself as _the most comprehensive cross-platform .NET Library for HashiCorp's Vault_, and simplifies the integration into HashiCorp Vault when using .NET.

For most operations performed on the Vault via code, `VaultSharp` is the easiest way, however there are some operations which I was unable to do using VaultSharp, in which case the integration needs to be handled manually (see the next section onthe REST api).

For the below samples, a NuGet package reference to `VaultSharp 1.7.0.4` was added to a C# project.

### Reading secrets

Now that we finally have a correctly setup and configured Vault, with credentials for our application, we can finally try to connect via C# code.

Once the Vault is configured, connecting and interacting with it via C# code is very easy:

``` csharp
// Use the roleId and secretId generated
IAuthMethodInfo authMethod = new AppRoleAuthMethodInfo(
    "cb476d64-4614-1038-0dd5-b344700f3f3a", 
    "5c1587dc-83b0-fe15-cc8e-196e6b777150");

var vaultClientSettings = 
    new VaultClientSettings("http://127.0.0.1:8200", authMethod);

IVaultClient vaultClient = new VaultClient(vaultClientSettings);

try
{
    // the "mountPoint" is the root path
    // the "path" is the rest of the path
    Secret<SecretData> secrets = await vaultClient
        .V1.Secrets.KeyValue.V2.ReadSecretAsync(
            path: "firstapp", mountPoint: "secret");

    // access all the key-value pairs
    foreach(var item in secrets.Data.Data)
    {
        Console.WriteLine($"key:'{item.Key}' value:'{item.Value}'");
    }
}
catch (Exception ex)
{
    // try-catch just here for now to help 
    // debug any connection issues
    _ = ex;
}

```

Going through the official _VaultSharp_ documentation, it wasn't immediately obvious what the values of _path_ and _mountPoint_ should be:
- `mountPoint`: is the root path to read from, _secret_ in our case. If a `key-value pair` engine was added with a different name when configuring Vault in the previous steps, then this would be that value
- `path`: this is the path, excluding the root, from which the key-value pairs are retrieved. This allows for fine control over which _path_ (or folder) to read the secrets from

The output from executing the above:

```terminal
key:'secretkey' value:'mypassword'
```

---

### Writing secrets

As it stands, `permission denied` error will occur if trying to _write_ a secret - this is because our application role is linked to the `firstapp-policy`, which only has `read` permissions to the _firstapp_ path.

So to be able to create secrets using the approle credentials, the `policy linked to it need to be updated`. This can be done either:

- In the Vault user interface, by logging in as the root user, browsing to `Policies -> firstapp-policy -> Edit policy` and editing the policy directly
- Or by `updating the policy file`, then executing the following command again to write the update policy to the Vault (updating the existing one):
    ``` powershell
    vault policy write firstapp-policy C:\app-policy.hcl
    ```

Whether using the Vault UI or editing the file, the `update` permissions needs to be added to the policy capabilities, and the policy should look as follows:

``` terminal
# Login with AppRole 
path "auth/approle/login" {
  capabilities =  [ "create", "read" ]
} 

# Read data from secret/firstapp
path "secret/data/firstapp" {
  capabilities =  [ "read", "update" ]
}
```

With the policy updated, the role now has permissions to `create/write` secrets:

``` csharp
IAuthMethodInfo authMethod = new AppRoleAuthMethodInfo(
    "cb476d64-4614-1038-0dd5-b344700f3f3a", 
    "5c1587dc-83b0-fe15-cc8e-196e6b777150");

var vaultClientSettings = 
    new VaultClientSettings("http://127.0.0.1:8200", authMethod);

IVaultClient vaultClient = new VaultClient(vaultClientSettings);

try
{
    var newSecrets = new Dictionary<string, object>
    {
        ["DbPassword"] = "password123"
    };

    // Patch method called
    await vaultClient.V1.Secrets.KeyValue.V2.PatchSecretAsync(
        path: "firstapp", newSecrets, mountPoint: "secret");
}
catch (Exception ex)
{
    _ = ex;
}
```

---

## Quirks and tips

These are a few quirks I've encountered working with Vault and VaultSharp, which was not obvious while initially getting the two to integrate:

- `Policy path vs Code path`: 
    Its not immediately obvious when going through documentation, but in the policy `data` (or `metadata`) is added to the path, for example, `secret/data/firstapp`:
    ``` yml
    path "secret/data/firstapp" {
        capabilities =  [ "read", "update" ]
    }
    ```
    However when using `VaultSharp`, or the `CLI` to retrieve secrets, the `data` portion is dropped:
    - Using CLI:
        ``` powershell
            vault kv get secret/firstapp
        ```

    - Using VaultSharp:
        ```csharp
            await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(
                path: "firstapp", mountPoint: "secret");
        ```
- `Write secret vs Patch secret`: 
    In Vault, when changing the secrets in a _path_ (folder), a new version of the _path_ is created with the updated version of the secrets.  
    When `writing` or using _put_ to create a secret, `all previous secrets will be lost` and a `new version of the path created, with just the new secret`.  
    However when `patching` then `all secrets in the current version are brought into the new version`, along with any changes.

    - `Write/Put`: Existing secrets are lost in the new version:

        In PowerShell:

        ``` powershell
        vault kv put secret/firstapp newkey=mynewpassword
        ```

        And in C# code:

        ``` csharp
        var newSecrets = new Dictionary<string, object>
        {
            ["newkey"] = "mynewpassword"
        };
        // WriteSecretAsync is used here, vs PatchSecretAsync
        await vaultClient.V1.Secrets.KeyValue.V2.WriteSecretAsync(
            path: "firstapp", newSecrets, mountPoint: "secret");
        ```

    - `Patch`: Existing secrets are brought into the new version:

        In PowerShell:

        ``` powershell
        vault kv patch secret/firstapp newkey=mynewpassword
        ```

        And in C# code:

        ``` csharp
        var newSecrets = new Dictionary<string, object>
        {
            ["newkey"] = "mynewpassword"
        };
        // PatchSecretAsync is used here, vs WriteSecretAsync
        await vaultClient.V1.Secrets.KeyValue.V2.PatchSecretAsync(
            path: "firstapp", newSecrets, mountPoint: "secret");
        ```
---

## Rest API

In certain cases, `VaultSharp` does not provide functionality available via the Vault CLI (or at least, I was not able to find it amongst the VaultSharp code or documentation) - in these cases an alternative option has to be used. Luckily, Vault exposes all it's functionality via a REST api (which is what the CLI actually uses as well). This means any functionality available from the CLI can be reproduced in code by doing an HTTP call.

An example of a call which `VaultSharp` does not (at the time of writing this post) support, is the ability to `create an app role though code`. This is possible via the CLI, as it was done in previous steps:

``` powershell
vault write auth/approle/role/firstapp-role policies="firstapp-policy"
```

If we want to be able to perform this functionality vai code, it'll have to be via an HTTP call. Let's write some C# code to setup the permission for a new application. It will:
- Create a policy for a new application (using VaultSharp)
- Create an approle for a new application (using the REST API)
- Get the _roleId_ for the newly created role (using the REST API)

---

### Policy creation

VaultSharp does have functionality to create policies, so VaultSharp can be leveraged for this step:

``` csharp
// use the root token (or preferably another token which has permissions)
IAuthMethodInfo authMethod = new TokenAuthMethodInfo(config.Token);
var vaultClientSettings = new VaultClientSettings(config.Url, authMethod);

IVaultClient vaultClient = new VaultClient(vaultClientSettings);

// specify the application name
var applicationName = "newapplication";
// define the policy as a string
string appPolicy = @"
    # Login with AppRole 
    path ""auth/approle/login"" {{
    capabilities =  [""create"", ""read""]
    }}

    # Read test data (v2)
    path ""secret/data/{0}"" {{
    capabilities =  [""read""]
    }}";

    // define the policy. 
    var policy = new ACLPolicy
    {
        Name = String.Format($"{applicationName}-policy", applicationName),
        Policy = String.Format(appPolicy, applicationName)
    };

    // create the policy
    await vaultClient.V1.System.WriteACLPolicyAsync(policy);
```

Here a different _IAuthMethodInfo_ implementation is used vs the previous examples - this is because this operation is being performed by the _root user_ (or preferably another "admin" user/token with similar permissions). Apart from that, the usage of the VaultSharp library remains the same.

Step 1 done! - **Policy created successfully!**

Now that we have a policy, let's look at using `HttpClient` to call the REST api to create an `approle`.

---

### Approle creation

Invoking the REST api methods is similar to using the CLI, so the CLI command can be used as a template for the HTTP call:

``` powershell
vault write auth/approle/role/firstapp-role policies="firstapp-policy"
```

Translating this into C#:

``` csharp
// specify the application name
var applicationName = "newapplication";

// Ideally getting the client is done via the HttpClientFactory
var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("http://localhost:8200");

// this is the same path as used in the CLI, just with v1 pre-pended
HttpRequestMessage requestMessage = new(
    HttpMethod.Post, $"v1/auth/approle/role/{applicationName}-role");

// add the token as a header to the request
requestMessage.Headers.Add("X-Vault-Token", "some-root-token");

// set the contents of the request in json representation of the same
// params passed in the CLI
requestMessage.Content = new StringContent(
    $"{{\"policies\": \"{applicationName}-policy\"}}", 
    Encoding.UTF8, "application/json");
// send the request!
var response = await httpClient.SendAsync(requestMessage);

```

Step 2 done! - **Approle created successfully!**

Assuming no errors have occurred, the `approle` is now created - we just need to lookup the _roleId_ and _secretId_.

---

### RoleId lookup

Performing the _roleId_ follows very similar steps to the approle creation.

For reference, the powershell to perform this action:

``` powershell
vault read auth/approle/role/firstapp-role/role-id
```

And converting this into C# code:

``` csharp
// specify the application name
var applicationName = "newapplication";

// Ideally getting the client is done via the HttpClientFactory
var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("http://localhost:8200");

// this is the same path as used in the CLI, just with v1 pre-pended
HttpRequestMessage requestMessage = 
    new(HttpMethod.Get, $"v1/auth/approle/role/{applicationName}-role/role-id");
// add the token as a header to the request
requestMessage.Headers.Add("X-Vault-Token", "some-root-token");

// send the request!
var response = await httpClient.SendAsync(requestMessage);

if (!response.IsSuccessStatusCode)
{
    throw new HttpRequestException(
        $"Error getting role id: {response.ReasonPhrase}", 
        null, 
        response.StatusCode);
}

var roleResponse = await response.Content.ReadFromJsonAsync<GetRoleIdResponse>();

```

The _GetRoleIdResponse_ entity is a custom class which is defined as follows:

``` csharp
public class GetRoleIdResponse
{
    public GetRoleDataResponse data { get; set; }
}

public class GetRoleDataResponse
{
    public Guid role_id { get; set; }
}
```

Step 3 done! - **RoleId retrieved successfully!**

The same process can be applied to lookup the _secretId_, as well as execute any other Vault CLI command.

---

## In summary

In this post we had a look at how to do the following:
- Successfully running an instance of the HashiCorp vault using
    - The Windows binary 
    - A Docker image
- The Vault configuration steps required to be able to connect with an app level roleId and secretId:
    - Enabling the key-value pair engine
    - Creating a _path_ to hold secrets
    - Enabling the approle authentication method
    - Creating a policy
    - Creating a role and linking a policy to it
- Using VaultSharp to read secrets from, and write secrets to the Vault
- Using the Vault Rest API to perform functionality comparable to the Vault CLI (especially for use cases not covered by VaultSharp)

The hope is this guide will help others not have to go through some of the struggles I encountered while integrating with, and exploring the HashiCorp Vault functionality for the first time.

---

## References

[HashiCorp Vault](https://www.vaultproject.io/)  
[HashiCorp Vault Download](https://www.vaultproject.io/downloads)  
[Docker Desktop](https://www.docker.com/products/docker-desktop/)  
[Vault on DockerHub](https://hub.docker.com/_/vault)  
[HashiCorp Vault C# client with .NET Core Guide](hhttps://learn.hashicorp.com/tutorials/vault/dotnet-httpclient)  
[HashiCorp Vault Guide repository](https://github.com/hashicorp/vault-guides)  
[VaultSharp](https://github.com/rajanadar/VaultSharp)