# ASP.NET Core + Vue.js: TOTP Playground

## Background

### What is a Two-Factor authentication?

> Two-Factor Authentication or TFA is a method where your users are required to log in with two “factors”: a password, and a code from a device that they carry with them. That device used to be a special-purpose device, or a token device, but nowadays that device can just as well be a mobile phone.

Described by [Joel Franusic](https://www.twilio.com/blog/2013/04/add-two-factor-authentication-to-your-website-with-google-authenticator-and-twilio-sms.html)

They are a few patterns for implementing the code generated for the TFA. TOTP (Time-based One-time Password Algorithm) is one mostly used in Google Authenticator and Microsoft Authenticator apps. These TOTP authenticators are based off the [RFC6238](http://tools.ietf.org/html/rfc6238) standard.

## Frontend Features

A Vue.js + TypeScript and Vuetify SPA project scaffolded with Vue CLI 3.

- Vue.js 2.5
- Vue CLI 3
- Vue Router
- Vuex
- Vuetify
- TypeScript

### Other Features

- Axios
- NProgress
- QRCanvas
- RxJS
- VueCookies

## Backend Features

An ASP.NET Core RESTful API featuring InMemory storage with EF Core, an object-object mapper, fully-structured events logs and Swagger.

- ASP.NET Core 2.2
- ASP.NET Core Identity
- Entity Framework Core

### Other Features

- AutoMapper
- Fluent Validation
- Serilog
- Swagger
