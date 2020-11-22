# ASP.NET Core 3.1: TOTP & WebAuthn Playground

## Features

An ASP.NET Core 5.0 MVC featuring InMemory storage with EF Core, Identity and FIDO2.

- ASP.NET Core 5.0
- ASP.NET Core Identity
- Entity Framework Core
- [FIDO2](https://www.nuget.org/packages/Rsk.AspNetCore.Fido) by [IdentityServer](https://www.identityserver.com/)

## URI

The app will be served on:

```
https://localhost:443/
http://localhost:80/
```

## Project setup

Setup a local domain, in this case I'm using `dev.local`.

### Windows

Edit your hosts file and add `127.0.0.1 dev.local` line at the bottom of the file.

Create self-signed certificate:

```powershell
$cert = New-SelfSignedCertificate -certstorelocation cert:\localmachine\my -dnsname dev.local
```

Verify domain name, capabilities and thumbprint:

```powershell
$cert
```

Create a password used to protect the certificate:

```powershell
$pwd = ConvertTo-SecureString -String "type a secure password here" -Force -AsPlainText
```

Create local variable that will contain the path to our certificate:

```powershell
$certpath = "Cert:\localMachine\my\$($cert.Thumbprint)"
```

Export the certificate as a .pfx:

```powershell
Export-PfxCertificate -Cert $certpath -FilePath C:\Temp\localhost.pfx -Password $pwd
```

Add exported .pfx to Trusted Root Certification Authorities and copy it to the root directory of the project (next to Program.cs).

Open a terminal and cd to the directory where the project is located and execute the following:

```bash
dotnet user-secrets set "Kestrel:Certificates:Default:Password" "the secure password you used above"
dotnet user-secrets list
```

Finally, run the app by issuing:

```
dotnet restore
dotnet run
```

**Note:** The sample contains console logs in order to see what's happening. You should remove or exclude this when using in production.
