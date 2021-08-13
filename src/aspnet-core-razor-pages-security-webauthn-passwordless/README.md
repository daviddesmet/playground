# ASP.NET Core 5.0: FIDO2 Passwordless Auth Playground

## Features

An ASP.NET Core 5.0 MVC featuring InMemory storage with EF Core, Identity and FIDO2.

- ASP.NET Core 5.0
- ASP.NET Core Identity
- Entity Framework Core
- [FIDO2](https://www.nuget.org/packages/Rsk.AspNetCore.Fido) by [IdentityServer](https://www.identityserver.com/)

## URI

The app will be served on:

```
https://dev.local
https://localhost:443/
http://localhost:80/
```

## Project setup

Setup a local domain, in this case I'm using `dev.local`.

Edit your hosts file and add `127.0.0.1 dev.local` line at the bottom of the file.

### Windows

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

### Linux or macOS

Create a RSA key pair, you will be asked for a passphrase:

```bash
ssh-keygen -t rsa -b 4096 -m PEM -f localhost
```

Convert the RSA key to PEM format:

```bash
sudo openssl rsa -in localhost -pubout -out localhost.pem
```

Create a config file with [DNS SAN](https://en.wikipedia.org/wiki/Subject_Alternative_Name) so the latest versions of the browsers will trust it:

```bash
touch localhost.conf
vi localhost.conf
```

```
[ req ]
prompt              = no
default_bits        = 4096
default_keyfile     = localhost.pem
distinguished_name  = subject
req_extensions      = req_ext
x509_extensions     = x509_ext
string_mask         = utf8only

# The Subject DN can be formed using X501 or RFC 4514 (see RFC 4519 for a description).
#   Its sort of a mashup. For example, RFC 4514 does not provide emailAddress.
[ subject ]
countryName         = GB
stateOrProvinceName = London
localityName        = London
organizationName    = David De Smet


# Use a friendly name here because its presented to the user. The server's DNS
#   names are placed in Subject Alternate Names. Plus, DNS names here is deprecated
#   by both IETF and CA/Browser Forums. If you place a DNS name here, then you 
#   must include the DNS name in the SAN too (otherwise, Chrome and others that
#   strictly follow the CA/Browser Baseline Requirements will fail).
commonName          = Localhost development
emailAddress        = dev@example.com

# Section x509_ext is used when generating a self-signed certificate. I.e., openssl req -x509 ...
[ x509_ext ]

subjectKeyIdentifier    = hash
authorityKeyIdentifier  = keyid,issuer

# You only need digitalSignature below. *If* you don't allow
#   RSA Key transport (i.e., you use ephemeral cipher suites), then
#   omit keyEncipherment because that's key transport.
basicConstraints        = CA:FALSE
keyUsage                = digitalSignature, keyEncipherment
subjectAltName          = @alternate_names
nsComment               = "OpenSSL Generated Certificate"

# RFC 5280, Section 4.2.1.12 makes EKU optional
#   CA/Browser Baseline Requirements, Appendix (B)(3)(G) makes me confused
#   In either case, you probably only need serverAuth.
# extendedKeyUsage      = serverAuth, clientAuth

# Section req_ext is used when generating a certificate signing request. I.e., openssl req ...
[ req_ext ]

subjectKeyIdentifier    = hash

basicConstraints        = CA:FALSE
keyUsage                = digitalSignature, keyEncipherment
subjectAltName          = @alternate_names
nsComment               = "OpenSSL Generated Certificate"

# RFC 5280, Section 4.2.1.12 makes EKU optional
#   CA/Browser Baseline Requirements, Appendix (B)(3)(G) makes me confused
#   In either case, you probably only need serverAuth.
# extendedKeyUsage      = serverAuth, clientAuth

[ alternate_names ]

DNS.1         = dev.local

# Add these if you need them. But usually you don't want them or
#   need them in production. You may need them for development.
# DNS.5       = localhost
# DNS.6       = localhost.localdomain
# DNS.7       = 127.0.0.1

# IPv6 localhost
# DNS.8       = ::1
```

Create self-signed certificate, you will be using the above conf file for supplying the needed certificate information:

```bash
openssl req -config localhost.conf -new -x509 -days 365 -sha256 -newkey rsa:4096 -nodes -keyout localhost.key -out localhost.crt
```

Create a decrypted version of the private key (optional):

```bash
openssl rsa -in localhost.key -out localhost.decrypted.key
```

Convert the self-signed certificate to PKCS#12 format, you will be asked for a password:

```bash
openssl pkcs12 -export -in localhost.crt -inkey localhost.key -out localhost.pfx
```

To trust the self-signed certificate:

On **macOS**, open *Keychain Access* app and drag the certificate into it, locate the added certificate in the *Certificates* section and double click on it, expand and enter the trust section and under *When using this certificate* select *Always Trust*.

On **Linux**, it [varies depending on which flavour of Linux](https://chromium.googlesource.com/chromium/src/+/refs/heads/lkgr/docs/linux/cert_management.md) you're using. For Ubuntu 16.04 issue the following commands:

```bash
# Install the cert utils
sudo apt install libnss3-tools
# Trust the certificate for SSL 
pk12util -d sql:$HOME/.pki/nssdb -i localhost.pfx
# Trust a self-signed server certificate
certutil -d sql:$HOME/.pki/nssdb -A -t "P,," -n 'dev cert' -i localhost.crt
```

### Safe storage of app secrets in development

Open a terminal and cd to the directory where the project is located and execute the following:

```bash
dotnet user-secrets set "Kestrel:Certificates:Default:Password" "the secure password you used above"
dotnet user-secrets list
```

Those secrets will be locally stored in

```bash
# Windows
%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json

# Linux/macOS
~/.microsoft/usersecrets/<user_secrets_id>/secrets.json
```

Finally, run the app by issuing:

```
dotnet restore
dotnet run
```

**Note:** The sample contains console logs in order to see what's happening. You should remove or exclude this when using in production.
