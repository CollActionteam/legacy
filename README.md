# CollAction

CollAction is the world's first official crowdacting platform that aims to solve collective action problems with the ultimate goal of having positive social and ecological impact. 
Copyright (C) 2017, Stichting CollAction

![build status](https://travis-ci.org/CollActionteam/CollAction.svg?branch=master)

## Running the project

Docker is the easiest way to get this running: 

```
docker-compose build
docker-compose up -d
```

You can also run our services outside docker.

To run the backend you need:
* dotnet core 3.1 or higher
* Visual Studio or Visual Studio code can be used to run the project. If you don't like IDEs you can use `dotnet run` to run it through the command line.

To run the frontend you need:
* A recent version of npm and yarn
* To install the dependencies run `yarn`
* To run the project in development mode, use `npm run start`

## Dependencies

### Backend

Our backend is build using ASP Core 3.1. We use:
* GraphQL.net for our APIs together with:
  * GraphiQL for development
  * GraphQL.EntityFramework to retrieve the data
* Hangfire to run background jobs
* Serilog to handle our logging
* ImageSharp for image processing
* MSTest/Moq for testing
* Postgres to store our data

### Frontend

Our frontend is build using react. We use:
* Fontawesome/Fortawesome for icons
* Apollo client for GraphQL interaction
* Material UI
* Formik to handle form validation
* Node SASS for our css
* react-dropzone for a file upload UI
* react-slick/slick-carousel for our project carousel

### External

We use the following external services:
* Netlify to host our frontend
* Amazon ECS + EC2 to host our backend
* Stripe to receive donations
* Amazon S3 to host our project images
* Amazon SES to send transactional e-mails
* Mailchimp for maillists

We're using BrowserStack for our front-end testing!
[![https://www.browserstack.com](https://bstacksupport.zendesk.com/attachments/token/cpafMa8RUtwGNsKwZEUuzZLAI/?name=Logo-01.svg)](https://www.browserstack.com)

## Settings

When you run the site, you can configure the following settings through either environment variables or dotnet-secrets:

* DbUser: The postgres user
* DbPassword: The postgres password
* DbHost: The postgres host
* Db: The postgres database
* SeedTestData: Seed the database with random test data (true or false)
* PublicAddress: A ;-separated list of frontend addresses that can access the backend. The first in the list is assumed to be the canonical address. CORS is used to restrict access to other addresses.
* AdminPassword: The admin password used for the admin user
* AdminEmail: The admin e-mail address used for the admin user
* MaxNumberProjectEmails: How many project e-mails can project-owners send (defaults to 4)
* MaxImageDimensionPixels: The maximum image dimension (either width or height) a project image can have (defaults to 1600)
* MailChimpTestListId: The mailchimp list-id used for integration tests
* MailChimpNewsletterListId: The mailchimp list-id used to subscribe users
* MailChimpKey: The secret mailchimp key used to access the MailChimp API
* S3Region: The S3 region used to host our images
* S3Bucket: The S3 bucket-name used to host our images
* S3AwsAccessKeyID: The AWS access key ID we use to access the S3 API
* S3AwsAccessKey: The AWS access key we use to access the S3 API
* SesRegion: The region we host SES in
* SesAwsAccessKeyID: The AWS access key ID we use to access the SES API
* SesAwsAccessKey: The AWS access key we use to access the SES API
* FromAddress: The from-address we use for our SES e-mails
* StripePublicApiKey: The stripe public (shareable) key used to access the stripe API
* StripeSecretApiKey: The stripe secret (non-shareable) key used to access the stripe API
* StripeChargeableWebhookSecret: The webhook secret used for the chargeable endpoint (pings back when we can initiate a payment)
* StripePaymentEventWebhookSecret: The webhook secret used for the event endpoint (used for logging stripe events)
* ASPNETCORE_URLS: Which ports/protocols to expose (example: https://*:44301) 
* ASPNETCORE_ENVIRONMENT: Which environment are we hosting in (Development/Staging/Production)
