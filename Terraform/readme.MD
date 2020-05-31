# Terraform
This directory contains Terraform templates that create the CollAction Fargate environment.


## Deploying
The infrastructure is managed using Terraform Cloud. 

TODO: describe how to deploy.


## Configuration parameters
The container configuration parameters are injected via AWS SSM Parameter store. In order to add new parameters, add the new paraneters to the ssm.tf file. Values that should remain secret (e.g. passwords, API keys etc) can be added to the securestring_parameters variable. Other values can go in string_parameters.

After committing the file the parameter will be created and added to the task definition of the container.

You can set the actual value via the AWS Console. Log in, go to the Systems Manager service and select 'Parameter Store' under the 'Application Management' section in the menu on the left.