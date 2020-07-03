variable "securestring_parameters" {
  type = set(string)
  default = [
    "AdminEmail",
    "AdminPassword",
    "ApplicationInsights/InstrumentationKey",
    "Authentication/Facebook/AppId",
    "Authentication/Facebook/AppSecret",
    "Authentication/Google/ClientId",
    "Authentication/Google/ClientSecret",
    "Authentication/Twitter/ConsumerKey",
    "Authentication/Twitter/ConsumerSecret",
    "DisqusSiteId",
    "MailChimpAccount",
    "MailChimpUserId",
    "MailChimpKey",
    "MailChimpNewsletterListId",
    "S3AwsAccessKey",
    "S3AwsAccessKeyID",
    "SesAwsAccessKey",
    "SesAwsAccessKeyId",
    "SlackHook",
    "StripeChargeableWebhookSecret",
    "StripePaymentEventWebhookSecret",
    "StripePublicApiKey",
    "StripeSecretApiKey",
    "GoogleAnalyticsID",
    "FacebookPixelID"
  ]
}

variable "string_parameters" {
  type = set(string)
  default = [
    "APPINSIGHTS_JAVASCRIPT_ENABLED",
    "FromAddress",
    "MailChimpServer",
    "PublicAddress",
    "S3Bucket",
    "S3Region",
    "SeedTestData",
    "SesRegion",
    "AllowedCorsOrigins"
  ]
}